
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlockMgr : MonoBehaviour {

	public static BlockMgr Instance;
    public List<Entity> blockList;


    void Start()
    {
        if (Instance != null)
        {
            Debug.LogError("BlockMgr Already exists");
        }
        Instance = this;
    }

    public void AddBlock(Entity entity)
    {
    	blockList.Add(entity);
    	entity.transform.parent = this.gameObject.transform;
    }
}
