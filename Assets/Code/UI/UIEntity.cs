using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIEntity : MonoBehaviour {

    public GeneralMachine machine;

    public GameObject container;
    public Rect containerRect;
    protected bool consumeInput = false;
    
    public bool isInit = false;

    
    public virtual void Start()
    {
    }


    public virtual void Initialize()
    {
        machine = new GeneralMachine();
        machine.Initialize(this);
        machine.AddEnterListener(OnNotReady);
        machine.AddEnterListener(OnReady);
        machine.AddEnterListener(OnActive);
        machine.AddEnterListener(OnBusy);
        machine.AddUpdateListener(UpdateBusy);       

        machine.SetState(GeneralState.READY);
        isInit = true;
    }

    public virtual void Update()
    {
        if (isInit)
        {
            machine.MachineUpdate(Time.deltaTime);
        }
    }


    public virtual void OnNotReady(object owner)
    {

    }


    public virtual void OnReady(object owner)
    {
        
    }

    public virtual void OnActive(object owner)
    {

    }

    public virtual void OnBusy(object owner)
    {

    }

    public virtual void UpdateBusy(float deltaTime)
    {

    }

    public virtual bool IsConsumeInput()
    {
        return consumeInput;
    }

    public bool IsReady()
    {
        return machine.IsState(GeneralState.READY);
    }
    public bool IsActive()
    {
        return machine.IsState(GeneralState.ACTIVE);
    }
    public bool IsBusy()
    {
        return machine.IsState(GeneralState.BUSY);
    }
    public bool InBounds(Vector2 pos)
    {
        return containerRect.Contains(pos);
    }
}
