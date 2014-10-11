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
    SELECT,
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
        
        machine.SetState(InputState.READY);

        isInit = true;
    }
    
	public void OnChange(int val	)
	{
		Dbg.Instance.SetLabel(2, "Inp "+((InputState)machine.GetActiveState()).ToString());
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
        machine.MachineUpdate(Time.deltaTime);
    }

    void UpdateActive(float deltaTime)
    {
        if (fingerCount > 0)
        {
            
        }
        else
        {
            //Debug.Log("User has " + fingerCount + " finger(s) touching the screen"+touchCurrentPos[0]);
            FinishActive();
        }
    }

    void UpdateReady(float deltaTime)
    {
         if (fingerCount > 0)
        {
            //Debug.Log("User has " + fingerCount + " finger(s) touching the screen"+touchCurrentPos[0]);
            machine.SetState(InputState.ACTIVE);
        }
    }

    void UpdateBusy(float deltaTime)
    {
        if (!Maker.Instance.IsBusy() && !CameraMgr.Instance.IsBusy())
        {
            machine.SetState(InputState.READY);
        }
    }

    private void CheckFingerZone()
    {
        if (touchCurrentPos[0].y < 0.1)
        {
            fingerZone = InputType.SELECT;
        }
        else if (touchStartPos[0].x > 0.4 && 
            touchStartPos[0].x < 0.6 && 
            touchStartPos[0].y > 0.4 && 
            touchStartPos[0].y < 0.6)
        {
            fingerZone = InputType.PLACE;
        }
        else
        {
            fingerZone = InputType.CAMERA;    
        }
    }

    private void FinishActive()
    {
        Vector2 current = Norm(touchCurrentPos[0]);
        Vector2 start = Norm(touchStartPos[0]);
        switch(fingerZone)
        {
            case InputType.SELECT:
                Maker.Instance.SelectType((int)Mathf.Round(current.x / 0.33f));
                break;
            case InputType.PLACE:
                Maker.Instance.Place(current);
                break;
            case InputType.CAMERA:
                CameraMgr.Instance.Move(current-start);
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
