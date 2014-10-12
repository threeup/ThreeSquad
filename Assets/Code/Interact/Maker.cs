using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public enum MakerState
{
    NOTREADY,
    READY,
    ACTIVE,
    PENDING,
    BUSY,
}

public enum MakerMode
{
    BLOCK,
    COMMAND,
}
public enum BlockType
{
    CURSOR,
    SPAWN,
    ROCK,
    WOOD,
    PAPER,
}

public enum CommandType
{
    SHOOT,
    WAIT,
    FORWARD,
    ROTATELEFT,
    ROTATERIGHT,
    STRAFELEFT,
    STRAFERIGHT,
}

[System.Serializable]
public class MakerMachine : StateMachine<MakerState>
{
}

public class Maker : MonoBehaviour {

    public static Maker Instance;
    public MakerMachine machine;

    public bool isBlock = true;

    public MakerMode mode;
    public BlockType blockType;
    public CommandType commandType;

    public UIWheel wheel;
    private bool wheelDirty;
    private List<UIProperties> uiprops;

    public User user;


    void Start()
    {
        if (Instance != null)
        {
            Debug.LogError("Maker Already exists");
        }
        Instance = this;
        uiprops = new List<UIProperties>();
    }


    public void Initialize()
    {
        machine = new MakerMachine();
        machine.Initialize(this);
        machine.AddEnterListener(OnNotReady);
        machine.AddEnterListener(OnReady);
        machine.AddEnterListener(OnActive);
        machine.AddEnterListener(OnPending);
        machine.AddEnterListener(OnBusy);
		machine.AddChangeListener(OnChange);
        
        wheelDirty = true;
        machine.SetState(MakerState.READY);
    }
    
	public void OnChange(int val	)
	{
		Dbg.Instance.SetLabel(1, "Mak "+((MakerState)machine.GetActiveState()).ToString());
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

    public void OnPending(object owner)
    {

    }

    public void OnBusy(object owner)
    {

    }

    public void SelectType(int typ)
    {
        switch(mode)
        {
            case MakerMode.BLOCK:
                blockType = (BlockType)typ;
                break;
            case MakerMode.COMMAND:
                commandType = (CommandType)typ;
                break;
        }
    }

    public void Place(Vector2 pos)
    {
        user.RemoveBlockInventory(wheel.GetSelectedUID());
    }


    public bool IsReady()
    {
        return machine.IsState(MakerState.READY);
    }
    public bool IsBusy()
    {
        return machine.IsState(MakerState.BUSY);
    }

    public void PopulateWheel()
    {
        uiprops.Clear();
        if (user == null)
        {
            user = UserMgr.Instance.users[0];
        }
        switch(mode)
        {
            case MakerMode.BLOCK:
                uiprops.AddRange(user.GetBlockUI());
                break;
            case MakerMode.COMMAND:
                uiprops.AddRange(user.GetCommandUI());
                break;
        }
        wheel.SetContents(uiprops);
    }

    public void Update()
    {
        if (wheelDirty && IsReady() && wheel.IsReady())
        {
            PopulateWheel();
        }
    }
}
