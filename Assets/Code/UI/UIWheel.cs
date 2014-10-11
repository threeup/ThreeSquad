using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class UIWheelSelection : MonoBehaviour {
    public GameObject go;
    public Button button;
    public int index;
    
    public UIWheelSelection next;
    public UIWheelSelection prev;
    public Vector2 position;
    
    public bool isSelected = false;
    public bool isMoving = false;
    
    public Queue<Vector3> destinationQueue;
    public Vector2 destinationPosition;
    public int destinationIndex;

    public float tweenDuration = 0.0f;

    public void Init(GameObject go, int index)
    {
        this.go = go;
        this.button = go.GetComponent<Button>() as Button;
        this.index = index;
        Vector3 pos = go.transform.position;
        this.position = new Vector2(pos.x, pos.y);
        this.destinationPosition = this.position;
        destinationQueue = new Queue<Vector3>();
    }

    public void Redraw()
    {
        Vector3 pos = go.transform.position;
        this.position = new Vector2(pos.x, pos.y);
        this.destinationPosition = this.position;
    }

    public void SetPrev(UIWheelSelection selection)
    {
        this.prev = selection;
        selection.next = this;
    }

    public void MoveTo(UIWheelSelection other, float tweenDuration)
    {
        this.tweenDuration = tweenDuration;
        Vector3 destination = new Vector3(other.position.x, other.position.y, other.index);
        destinationQueue.Enqueue(destination);
        CheckQueue();
    }

    public void CheckQueue()
    {
        if (isMoving || destinationQueue.Count == 0)
        {
            return;
        }
        Vector3 destination = destinationQueue.Dequeue();
        destinationPosition.x = destination.x;
        destinationPosition.y = destination.y;
        destinationIndex = (int)destination.z;
        LeanTween.move(go, destinationPosition, tweenDuration).setEase(LeanTweenType.easeInOutQuad);
        isMoving = true;
    }

    public void FinishMoving()
    {
        position = destinationPosition;
        index = destinationIndex;
        isMoving = false;
        CheckQueue();
    }

    public UIWheelSelection GetNextOrPrev(bool isNext)
    {
        return isNext ? next: prev;
    }

    public void SetSelected(bool val)
    {
        isSelected = val;
    }
}

public class UIWheel : MonoBehaviour {

    public static UIWheel Instance;
    public GeneralMachine machine;

    public GameObject container;
    public List<GameObject> buttons;
    public List<UIWheelSelection> selections;
    public UIWheelSelection selected;

    private Vector2 anchorPos;
    private Vector2 centerPos;
    private bool consumeInput = false;
    private bool hasClickDown = false;

    private float threshold = 40f;
    private float buttonRadius = 0f;
    private float tweenDuration = 0.25f;
    private float lockTimer = 0.0f;

    private DeviceOrientation lastDeviceOrientation = DeviceOrientation.Unknown;
    private Vector3 lastScreenFactor = Vector3.one;

    public bool isInit = false;

    
    void Start()
    {
        if (Instance != null)
        {
            Debug.LogError("UIWheel Already exists");
        }
        Instance = this;
    }


    public void Initialize()
    {
        machine = new GeneralMachine();
        machine.Initialize(this);
        machine.AddEnterListener(OnNotReady);
        machine.AddEnterListener(OnReady);
        machine.AddEnterListener(OnActive);
        machine.AddEnterListener(OnBusy);
        machine.AddUpdateListener(UpdateBusy);
        


        lastScreenFactor = UIMgr.Instance.CalculateScreenFactor();
        for(int i=0; i< buttons.Count; ++i)
        {
            Utilities.ApplyScale(buttons[i], lastScreenFactor);
            UIWheelSelection selection = buttons[i].AddComponent<UIWheelSelection>();
            selection.Init(buttons[i], i);
            selections.Add(selection);
            if (i>0)
            {
                selection.SetPrev(selections[i-1]);
            }
        }

        selections[0].SetPrev(selections[buttons.Count-1]);
        

        Select(0);
        centerPos = selected.position;
        buttonRadius = (selected.transform as RectTransform).rect.height/2f * selected.transform.localScale.y;
        Utilities.ApplyScale(container, lastScreenFactor);

        machine.SetState(GeneralState.READY);
        isInit = true;
    }

    void CheckForOrientation() 
    {
        if (lastDeviceOrientation != Input.deviceOrientation)
        {
            lastDeviceOrientation = Input.deviceOrientation;
            CheckForChange();
        }
    }

    public void CheckForChange()
    {
        Vector2 nextScreenFactor = UIMgr.Instance.CalculateScreenFactor();
        if (Mathf.Abs(nextScreenFactor.x-lastScreenFactor.x)+Mathf.Abs(nextScreenFactor.x-lastScreenFactor.x) > 0.1)
        {
            Debug.Log("redrawing"+nextScreenFactor);
            for(int i=0; i< selections.Count; ++i)
            {
                GameObject go = selections[i].go;
                Utilities.ReverseScale(go, lastScreenFactor);
                Utilities.ApplyScale(go, nextScreenFactor);
                selections[i].Redraw();

            }
            centerPos = selected.position;
            buttonRadius = (selected.transform as RectTransform).rect.height/2f * selected.transform.localScale.y;

            Utilities.ReverseScale(container, lastScreenFactor);
            Utilities.ApplyScale(container, nextScreenFactor);
            lastScreenFactor = nextScreenFactor;
        }
    }

    public void Update()
    {
        if (isInit)
        {
            CheckForChange();
            machine.MachineUpdate(Time.deltaTime);
        }
    }


    public void OnNotReady(object owner)
    {

    }


    public void OnReady(object owner)
    {
        
    }

    public void OnActive(object owner)
    {

    }

    public void OnBusy(object owner)
    {

    }

    public void UpdateBusy(float deltaTime)
    {
        lockTimer -= deltaTime;
        if (lockTimer < 0.0f)
        {
            for(int i=0;i<selections.Count;++i)
            {
                selections[i].FinishMoving();
            }
            Select(0);
            machine.SetState(GeneralState.ACTIVE);
        }
    }

    public void Place(Vector2 pos)
    {

    }

    public void OnClickDown()
    {
        consumeInput = true;
        StartCoroutine(ClickDownRoutine());
    }

    IEnumerator ClickDownRoutine()
    {
        yield return null;
        anchorPos = InputMgr.Instance.FirstTouchCurrent();
        //Debug.Log("anchor"+anchorPos.y);
        hasClickDown = true;
    }

    IEnumerator ConsumeInputRoutine(bool val)
    {
        yield return null;
        consumeInput = val;
    }

    public void OnClickUp()
    {
        if (IsBusy())
        {
            return;
        }
        if (hasClickDown)
        {
            Vector2 upPos = InputMgr.Instance.FirstTouchCurrent();
            float diff = upPos.y - anchorPos.y;
            //Debug.Log("upPos"+upPos.y+" anchor"+anchorPos.y);
            if (diff > threshold || diff < -threshold)
            {
                Rotate(diff < -threshold);
                anchorPos = upPos;
            }
            else
            {
                Push(upPos);
            }
        }
        hasClickDown = false;
        StartCoroutine(ConsumeInputRoutine(false));
    }

    public void OnClickDrag()
    {
        if (IsBusy())
        {
            return;
        }
        Vector2 current = InputMgr.Instance.FirstTouchCurrent();
        float diff = current.y - anchorPos.y;
        if (diff > threshold || diff < -threshold)
        {
            //Debug.Log("current"+current.y);
            Rotate(diff < -threshold);
            anchorPos = current;
        }
    }

    public void Rotate(bool isUp)
    {
        lockTimer = tweenDuration;
        for(int i=0; i < selections.Count; ++i)
        {
            selections[i].MoveTo(selections[i].GetNextOrPrev(isUp), tweenDuration);
        }
        machine.SetState(GeneralState.BUSY);
    }

    public void Push(Vector2 pos)
    {
        float diff = centerPos.y-pos.y;
        if (diff > buttonRadius || diff < -buttonRadius)
        {
            Rotate(diff > threshold);
        }
    }

    public void Select(int idx)
    {
        if(selected != null)
        {
            selected.SetSelected(false);
        }
        selected = selections.Find(x=>x.index==idx);
        selected.SetSelected(true);
    }


    public bool IsBusy()
    {
        return machine.IsState(GeneralState.BUSY);
    }

    public bool IsConsumeInput()
    {
        return consumeInput;
    }
}
