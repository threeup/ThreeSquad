using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIWheel : UIEntity {

    public static UIWheel Instance;

    public List<GameObject> buttons;
    public List<UIProperties> contents;
    public List<UIWheelSelection> selections;
    public UIWheelSelection selected;

    private Vector2 anchorPos;
    private Vector2 centerPos;
    private bool hasClickDown = false;

    private float threshold = 40f;
    private float buttonRadius = 0f;
    private float tweenDuration = 0.25f;
    private float lockTimer = 0.0f;

    private DeviceOrientation lastDeviceOrientation = DeviceOrientation.Unknown;
    private Vector3 lastScreenFactor = Vector3.one;

    
    public override void Start()
    {
        if (Instance != null)
        {
            Debug.LogError("UIWheel Already exists");
        }
        Instance = this;
        contents = new List<UIProperties>();
    }


    public override void Initialize()
    {
        lastScreenFactor = UIMgr.Instance.CalculateScreenFactor();
        for(int i=0; i< buttons.Count; ++i)
        {
            Utilities.ApplyScale(buttons[i], lastScreenFactor);
            UIWheelSelection selection = buttons[i].GetComponent<UIWheelSelection>();
            selection.Init(buttons[i], i);
            selections.Add(selection);
            if (i>0)
            {
                selection.SetPrev(selections[i-1]);
            }
            selection.SetSelected(false);
        }

        selections[0].SetPrev(selections[buttons.Count-1]);
        

        Select(0);
        centerPos = selected.position;
        buttonRadius = (selected.transform as RectTransform).rect.height/2f * selected.transform.localScale.y;
        Utilities.ApplyScale(container, lastScreenFactor);
        FinishResize();
        
        base.Initialize();

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
            containerRect = (container.transform as RectTransform).rect;
            FinishResize();
            lastScreenFactor = nextScreenFactor;
        }
    }

    public void FinishResize()
    {
        Transform trans = container.transform;
        RectTransform recttrans = trans as RectTransform;
        containerRect.width = recttrans.rect.width;
        containerRect.height = recttrans.rect.height;
        containerRect.x = trans.position.x - containerRect.width/2f;
        containerRect.y = trans.position.y - containerRect.height/2f;
        
    }

    public override void Update()
    {
        if (isInit)
        {
            CheckForChange();
        }
        base.Update();
    }

    public override void UpdateBusy(float deltaTime)
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

    public void SetContents(List<UIProperties> uiprops)
    {
        contents.Clear();
        contents.AddRange(uiprops);
        RefreshContents();
    }

    public void RefreshContents()
    {
        if (isInit)
        {
            for(int i=0; i<selections.Count; ++i)
            {
                if (i<contents.Count)
                {
                    selections[i].SetUIProp(contents[i]);
                }
                else
                {
                    selections[i].ClearUIProp();   
                }
            }
        }
    }

    public int GetSelectedUID()
    {
        return selected.prop.uid;
    }

    public int GetSelectedTypeVal()
    {
        return selected.prop.typeval;
    }

}
