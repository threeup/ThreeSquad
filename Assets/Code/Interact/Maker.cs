using UnityEngine;
using System.Collections;
using UnityEngine.SocialPlatforms;

public enum MakerState
{
    NOTREADY,
    READY,
    ACTIVE,
    PENDING,
    BUSY,
}

public enum MakerType
{
    CURSOR,
    ROCK,
    WOOD,
    PAPER,
}

public class MakerMachine : StateMachine<MakerState>
{
}

public class Maker : MonoBehaviour {

    public static Maker Instance;
    public MakerMachine machine;

    public MakerType mtype;


    void Start()
    {
        if (Instance != null)
        {
            Debug.LogError("Maker Already exists");
        }
        Instance = this;
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
        mtype = (MakerType)typ;
    }

    public void Place(Vector2 pos)
    {

    }


    public bool IsBusy()
    {
        return machine.IsState(MakerState.BUSY);
    }
}
