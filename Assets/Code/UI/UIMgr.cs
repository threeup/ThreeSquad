using UnityEngine;
using System.Collections;
using UnityEngine.SocialPlatforms;

[System.Serializable]
public struct UIProperties
{
    public string text;
    public int typeval;
    public int uid;
    //public Material mat;

    public UIProperties(string text, int typeval)
    {
        this.text = text;
        this.typeval = typeval;
        this.uid = -1;
    }
}

public class UIMgr : MonoBehaviour {

    public static UIMgr Instance;
    public GeneralMachine machine;

    public UIEntity[] entityList;

    private bool consumeInput = false;
    private Rect containerRect;
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
        for(int i=0; i<entityList.Length; ++i)
        {
            entityList[i].Initialize();
        }
        
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
        if (consumeInput)
        {
            return true;
        }
        for(int i=0; i<entityList.Length; ++i)
        {
            if (entityList[i].IsConsumeInput())
            {
                return true;
            }
        }
        return false;

    }

    public bool InBounds(Vector2 pos)
    {
        if (containerRect.Contains(pos))
        {
            return true;
        }
        for(int i=0; i<entityList.Length; ++i)
        {
            if (entityList[i].InBounds(pos))
            {
                return true;
            }
        }
        return false;
    }

    public bool HandleClick(Vector2 pos)
    {
        return true;
    }
}
