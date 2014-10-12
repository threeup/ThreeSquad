using UnityEngine;
using System.Collections;
using UnityEngine.SocialPlatforms;

public enum CameraState
{
    NOTREADY,
    READY,
    BUSY,
}

[System.Serializable]
public class CameraMachine : StateMachine<CameraState>
{
}

public class CameraMgr : MonoBehaviour {

    public static CameraMgr Instance;
    public CameraMachine machine;

    public GridLineMgr gridLineMgr;//wiredup

    public Vector3 centerThree;
    public Vector2 center;
    public Vector2 destination;

    public Rect cameraRect;

    private bool isInit = false;
    private bool moveReady = false;

    void Start()
    {
        if (Instance != null)
        {
            Debug.LogError("CameraMgr Already exists");
        }
        Instance = this;
    }


    public void Initialize()
    {
        machine = new CameraMachine();
        machine.Initialize(this);
        machine.AddEnterListener(OnNotReady);
        machine.AddEnterListener(OnReady);
        machine.AddUpdateListener(UpdateReady);
        machine.AddEnterListener(OnBusy);
        machine.AddUpdateListener(UpdateBusy);
        
        machine.SetState(CameraState.BUSY);
        cameraRect = new Rect(0, 0, 30, 30);
        isInit = true;
    }
    
    public void OnNotReady(object owner)
    {
        moveReady = false;
    }


    public void OnReady(object owner)
    {
        moveReady = true;
    }

    public void OnActive(object owner)
    {
        
    }


    public void OnBusy(object owner)
    {
        
    }

    void Update() 
    {
        if (!isInit)
        {
            return;
        }
        centerThree = this.gameObject.transform.position;
        center.x = centerThree.x;
        center.y = centerThree.z;


        Vector2 radius = new Vector2(cameraRect.width/2,cameraRect.height/2);
        Vector2 topLeft = center - radius;
        float offsetY = 10f;
        cameraRect.x = topLeft.x;
        cameraRect.y = topLeft.y + offsetY;
        machine.MachineUpdate(Time.deltaTime);
    }

    void UpdateReady(float deltaTime)
    {

    }



    void UpdateBusy(float deltaTime)
    {
        
        Vector2 delta = destination - center;

        center += delta;
        centerThree.x = center.x;
        centerThree.z = center.y;
        this.gameObject.transform.position = centerThree;

        machine.SetState(CameraState.READY);

    }

    public void MoveTo(Vector2 pos)
    {
        destination.x = pos.x;
        destination.y = pos.y;
        if (CanMove())
        {
            machine.SetState(CameraState.BUSY);
        }
        gridLineMgr.AfterMove(destination);
    }

    public void Move(Vector2 rel)
    {
        if (CanMove())
        {
            destination.x = center.x - 10f*(rel.x);
            destination.y = center.y - 10f*(rel.y);
            machine.SetState(CameraState.BUSY);
        }
    }

    public bool IsBusy()
    {
        return machine.IsState(CameraState.BUSY);
    }

    public bool CanMove()
    {
        return !UIMgr.Instance.IsConsumeInput() && moveReady;
    }


}
