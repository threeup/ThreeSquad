using UnityEngine;
using System.Collections;
using UnityEngine.SocialPlatforms;

public enum InputState
{
    NOTREADY,
    READY,
    ACTIVE,
    BUSY,
}

public enum InputType
{
    UIMGR,
    CAMERA,
    PLACE,
}

[System.Serializable]
public class InputMachine : StateMachine<InputState>
{
}

public class InputMgr : MonoBehaviour {

    public static InputMgr Instance;
    public InputMachine machine;

    public CameraMgr cameraMgr;
    public UIMgr uiMgr;
    public Maker maker;
    public InputType fingerZone;

    private Vector2[] touchCurrentPos;
    private Vector2[] touchStartPos;

    private Vector2 screenSize;
    private bool mouseDown = false;
    private bool isInit = false;
    private bool canInput = false;
    private int fingerCount = 0;

    void Start()
    {
        if (Instance != null)
        {
            Debug.LogError("InputMgr Already exists");
        }
        Instance = this;
    }


    public void Initialize()
    {
        touchStartPos = new Vector2[2];
        touchCurrentPos = new Vector2[2];
        screenSize = new Vector2(Screen.width, Screen.height);
        machine = new InputMachine();
        machine.Initialize(this);
        machine.AddEnterListener(OnNotReady);
        machine.AddEnterListener(OnReady);
        machine.AddUpdateListener(UpdateReady);
        machine.AddEnterListener(OnActive);
        machine.AddUpdateListener(UpdateActive);
        machine.AddEnterListener(OnBusy);
        machine.AddUpdateListener(UpdateBusy);
		machine.AddChangeListener(OnChange);
        
        uiMgr = UIMgr.Instance;
        cameraMgr = CameraMgr.Instance;
        maker = Maker.Instance;
        machine.SetState(InputState.READY);

        isInit = true;
    }
    
	public void OnChange(int val	)
	{
		
	}

    public void OnNotReady(object owner)
    {
        canInput = false;
    }


    public void OnReady(object owner)
    {
        canInput = true;
    }

    public void OnActive(object owner)
    {
        canInput = true;
    }

    public void OnPending(object owner)
    {
        canInput = true;
    }

    public void OnBusy(object owner)
    {
        canInput = false;
    }

    void Update() 
    {
        if (!isInit)
        {
            return;
        }

        if (!canInput)
        {
            machine.MachineUpdate(Time.deltaTime);
            return;
        }
        
        fingerCount = 0;
        foreach (Touch touch in Input.touches) {
            if (touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled)
            {
                touchCurrentPos[fingerCount] = touch.position;
                if (touch.phase == TouchPhase.Began)
                {
                    touchStartPos[fingerCount] = touchCurrentPos[fingerCount];
                }
                fingerCount++;

            }
        }
        bool shouldMouseDown = Input.GetMouseButton(0);
        if (shouldMouseDown)
        {
            touchCurrentPos[fingerCount] = Input.mousePosition;
            if (mouseDown == false)
            {
                touchStartPos[fingerCount] = touchCurrentPos[fingerCount];
            }
            fingerCount++;
        }
        mouseDown = shouldMouseDown;

        if (fingerCount > 0)
        {
            CheckFingerZone();
        }
        maker.UpdateInput(machine.State);
        machine.MachineUpdate(Time.deltaTime);
    }

    void UpdateActive(float deltaTime)
    {
        if (fingerCount == 0)
        {
            FinishActive();
        }
    }

    void UpdateReady(float deltaTime)
    {
        if (fingerCount > 0)
        {
            machine.SetState(InputState.ACTIVE);
        }
    }

    void UpdateBusy(float deltaTime)
    {
        if (!maker.IsBusy() && !cameraMgr.IsBusy())
        {
            machine.SetState(InputState.READY);
        }
    }

    private void CheckFingerZone()
    {

        if (uiMgr.InBounds(touchCurrentPos[0]))
        {
            fingerZone = InputType.UIMGR;
        }
        else
        {
            Vector2 normalizedTouch = Norm(touchCurrentPos[0]);
            if (normalizedTouch.x > 0.33 && 
                normalizedTouch.x < 0.66 && 
                normalizedTouch.y > 0.33 && 
                normalizedTouch.y < 0.66)
            {
                fingerZone = InputType.PLACE;
            }
            else
            {
                fingerZone = InputType.CAMERA;    
            }
        }
        Dbg.Instance.SetLabel(2, "Inp "+((InputType)fingerZone).ToString());
    }

    private void FinishActive()
    {
        Vector2 current = Norm(touchCurrentPos[0]);
        Vector2 start = Norm(touchStartPos[0]);
        switch(fingerZone)
        {
            case InputType.UIMGR:
                uiMgr.HandleClick(touchCurrentPos[0]);
                break;
            case InputType.PLACE:
                maker.Click(current);
                break;
            case InputType.CAMERA:
                cameraMgr.Move(current-start);
                break;
        }
        machine.SetState(InputState.BUSY);
    }
    public Vector2 Norm(Vector2 vec)
    {
        vec.x /= screenSize.x;
        vec.y /= screenSize.y;
        return vec;
    }

    public Vector2 FirstTouchStart()
    {
        return touchStartPos[0];
    }

    public Vector2 FirstTouchCurrent()
    {
        return touchCurrentPos[0];
    }



}
