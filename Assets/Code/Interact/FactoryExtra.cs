
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FactoryExtra : MonoBehaviour {


    public static FactoryExtra Instance;

    public GridLine gridLinePrefab;
    public Queue<GridLine> gridLinePool;
    
    void Start()
    {
        if (Instance != null)
        {
            Debug.LogError("FactoryEntity Already exists");
        }
        Instance = this;
        gridLinePool = new Queue<GridLine>();
    }

    public GridLine GetGridLine()
    {
        if (gridLinePool.Count > 0) {
            return gridLinePool.Dequeue();
        } else {
            return Instantiate(gridLinePrefab) as GridLine;
        }
    }

    public void PoolGridLine(GridLine obj)
    {
        gridLinePool.Enqueue(obj);
    }
    
}
