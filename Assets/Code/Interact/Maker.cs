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

    public Entity cursor;
    public BlockProperties cursorProp;
    private bool cursorMaterialDirty = true;

    public UIWheel wheel;
    private bool wheelDirty;
    private List<UIProperties> uiprops;

    public User user;

    private Vector3 centerPoint;
    private Vector3 offsetPoint;

    void Start()
    {
        if (Instance != null)
        {
            Debug.LogError("Maker Already exists");
        }
        Instance = this;
        uiprops = new List<UIProperties>();
        centerPoint = new Vector3(0.5f, 0.5f, 0f);
        offsetPoint = new Vector3(0f, 0f, 0f);
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
        cursorProp = Vars.Instance.blockDict[BlockType.CURSOR];
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
                BlockType shouldBlockType = (BlockType)typ;
                if (shouldBlockType != blockType)
                {
                    blockType = shouldBlockType;
                    cursorMaterialDirty = true;
                }
                break;
            case MakerMode.COMMAND:
                CommandType shouldCommandType = (CommandType)typ;
                if (shouldCommandType != commandType)
                {
                    commandType = shouldCommandType;
                    cursorMaterialDirty = true;
                }
                break;
        }
        
    }

    public void Click(Vector2 pos)
    {
        switch(mode)
        {
            case MakerMode.BLOCK:
                ClickBlock(pos);
                break;
        }
    }

    public void ClickBlock(Vector2 pos)
    {
        CheckCursor();
        if (ClickOnCursor(pos))
        {
            PlaceBlock();
        }
        else
        {
            offsetPoint.x = pos.x-centerPoint.x;
            offsetPoint.y = pos.y-centerPoint.y;
        }
    }

    public void PlaceBlock()
    {
        cursor.SetTransparent(false);
        cursor.SetPhysics(Vars.Instance.blockDict[blockType]);
        BlockMgr.Instance.AddBlock(cursor);
        cursor = null;
        CheckCursor();

        user.RemoveBlockInventory(wheel.GetSelectedUID());
        PopulateWheel();
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
        if (wheel.IsActive())
        {
            SelectType(wheel.GetSelectedTypeVal());
        }
    }

    public void UpdateInput(InputState inputState)
    {
        if (inputState == InputState.READY)
        {
            CheckCursor();
            SetCursorMaterial();
            AlignCursor();
        }
    }

    public void CheckCursor()
    {
        if (cursor == null)
        {
            cursor = FactoryEntity.Instance.GetBlock(cursorProp);
        }
    }

    public void SetCursorMaterial()
    {
        if (cursorMaterialDirty)
        {
            switch(mode)
            {
                case MakerMode.BLOCK:
                    cursor.SetMaterial(Vars.Instance.blockDict[blockType].mat);
                    cursor.SetTransparent(true);
                    cursor.SetAlpha(0.5f);
                    break;
                case MakerMode.COMMAND:
                    cursor.SetMaterial(Vars.Instance.commandDict[commandType].mat);
                    break;
            }
            cursorMaterialDirty = false;
        }
    }

    public void AlignCursor()
    {
        CameraMgr cameraMgr = CameraMgr.Instance; 
        Vector3 clickPoint = centerPoint+offsetPoint;
        Vector2 center = Camera.main.ViewportToScreenPoint(clickPoint);
        Ray ray = cameraMgr.camera.ScreenPointToRay(center);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100))
        {
            Debug.DrawLine(ray.origin, hit.point);
            Vector3 cursorPoint = hit.point;
            cursorPoint.x = Mathf.Round(cursorPoint.x/1f)*1f;
            cursorPoint.z = Mathf.Round(cursorPoint.z/1f)*1f;
            cursorPoint.y = 1f;
            cursor.transform.position = cursorPoint;
        }

        
    }

    public bool ClickOnCursor(Vector2 clickPoint)
    {
        CameraMgr cameraMgr = CameraMgr.Instance; 
        Vector2 center = Camera.main.ViewportToScreenPoint(clickPoint);
        Ray ray = cameraMgr.camera.ScreenPointToRay(center);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100))
        {
            Debug.DrawLine(ray.origin, hit.point,Color.blue,3f);
            if (hit.collider.gameObject == cursor.gameObject)
            {
                return true;
            }
        }
        return false;
    }
}
