using UnityEngine;
using System.Collections;
using UnityEngine.SocialPlatforms;

[System.Serializable]
public struct UIProperties
{
    public string text;
    public int val;
    public int uid;
    //public Material mat;

    public UIProperties(string text, int val)
    {
        this.text = text;
        this.val = val;
        this.uid = -1;
    }
}

public class UIMgr : MonoBehaviour {

    public static UIMgr Instance;
    public GeneralMachine machine;

    public UIWheel uiWheel; //wiredup

    private bool consumeInput = false;
    public Vector2 screenSize;
    public Vector2 screenSizeRef = new Vector2(480, 800);
    public Vector2 screenFactor;
    
    void Start()
    {
        if (Instance != null)
        {
            Debug.LogError("UIMgr Already exists");
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
        CalculateScreenFactor();
        uiWheel.Initialize();
        
        machine.SetState(GeneralState.READY);
    }

    public Vector2 CalculateScreenFactor()
    {
        screenSize = new Vector2(Screen.width, Screen.height);
        screenFactor = new Vector2(screenSize.x/screenSizeRef.x, screenSize.y/screenSizeRef.y);
        return screenFactor;
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

    public void Place(Vector2 pos)
    {

    }


    public bool IsBusy()
    {
        return machine.IsState(GeneralState.BUSY);
    }

    public bool IsConsumeInput()
    {
        return consumeInput || uiWheel.IsConsumeInput();
    }
}
