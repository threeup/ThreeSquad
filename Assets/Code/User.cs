
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class ContentMgr
{
    private static int nextContentUID = 0;
    public static int GetContentUID()
    {
        nextContentUID++;
        return nextContentUID;
    }

}

public class BlockContent
{
    public UIProperties uiprop;
    public BlockProperties blockprop;
    public int uid;
    public BlockContent(UIProperties uiprop, BlockProperties blockprop)
    {
        this.uiprop = uiprop;
        this.blockprop = blockprop;
        uid = ContentMgr.GetContentUID();
        this.uiprop.uid = uid;
    }
}

public class User : MonoBehaviour
{
    public Actor actor;
    public List<BlockContent> blockInventory;

    private List<UIProperties> blockUI;
    private List<UIProperties> commandUI;
    private bool blockDirty = false;
    private bool commandDirty = false;

    public void Initialize()
    {
        actor = null;
        blockInventory = new List<BlockContent>();
        blockUI = new List<UIProperties>();
        commandUI = new List<UIProperties>();
        blockDirty = true;
        commandDirty = true;
        FillDefault();

    }

    public void FillDefault()
    {
        Vars vars = Vars.Instance;
        AddBlockInventory(vars.blockDict[BlockType.ROCK]);
        AddBlockInventory(vars.blockDict[BlockType.WOOD]);
        AddBlockInventory(vars.blockDict[BlockType.WOOD]);
        AddBlockInventory(vars.blockDict[BlockType.PAPER]);
        AddBlockInventory(vars.blockDict[BlockType.PAPER]);
        AddBlockInventory(vars.blockDict[BlockType.SPAWN]);
        AddBlockInventory(vars.blockDict[BlockType.SPAWN]);
        
    }

    public void AddBlockInventory(BlockProperties blockprop)
    {
        UIProperties uiprop = new UIProperties(blockprop.name, (int)blockprop.type);
        BlockContent content = new BlockContent(uiprop, blockprop);
        blockInventory.Add(content);
    }

    public void RemoveBlockInventory(int uid)
    {
        for(int i=0; i<blockInventory.Count; ++i)
        {
            if (blockInventory[i].uid == uid)
            {
                blockInventory.RemoveAt(i);
                blockDirty = true;
                Debug.Log("remove"+i);
                break;
            }
        }
    }

    public List<UIProperties> GetBlockUI()
    {
        if (blockDirty)
        {
            blockUI.Clear();
            for(int i=0; i<blockInventory.Count; ++i)
            {
                blockUI.Add(blockInventory[i].uiprop);
            }
            blockDirty = false;
        }
        return blockUI;
    }

    public List<UIProperties> GetCommandUI()
    {
        if (commandDirty)
        {
            commandUI.Clear();
            commandUI.Add(new UIProperties("SHOOT", 0));
            commandUI.Add(new UIProperties("MOVE", 1));
            commandUI.Add(new UIProperties("WAIT", 2));
            commandDirty = false;
        }
        return commandUI;
    }
}
