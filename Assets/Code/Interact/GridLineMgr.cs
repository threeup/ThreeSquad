
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridLineMgr : MonoBehaviour
{
    public CameraMgr cameraMgr;
    public List<GridLine> lines;
    private Rect lastCameraRect;

    public void Initialize()
    {
        int radius = 20;
        for(int j = -radius; j<radius; j+=5)
        {
            for(int i = -radius; i<radius; i+=5)
            {
                GridLine across = FactoryExtra.Instance.GetGridLine();
                across.Init(i,j, 5f, 0.1f);
                across.go.transform.parent = this.transform;
                lines.Add(across);
                GridLine down = FactoryExtra.Instance.GetGridLine();
                down.Init(i,j, 0.1f, 5f);
                down.go.transform.parent = this.transform;
                lines.Add(down);
            }
        }
    }

    public void Update()
    {
        Rect cameraRect = cameraMgr.cameraRect;
        bool isDirty = (Mathf.Abs(lastCameraRect.x-cameraRect.x)+Mathf.Abs(lastCameraRect.x-cameraRect.x) > 0.1f);
        if (isDirty)
        {
            lastCameraRect = cameraRect;
            float deltaTime = Time.deltaTime;
            for(int i=lines.Count-1; i>=0; --i)
            {
                lines[i].UpdateGridLine(deltaTime, cameraRect);
            }
        }
    }

    public void AfterMove(Vector2 destination)
    {

        float centerX = Mathf.Round(destination.x/5f)*5f;
        float centerY = Mathf.Round(destination.y/5f)*5f;
        for(int i=lines.Count-1; i>=0; --i)
        {
            lines[i].SetOffset(centerX, centerY);
        }
    }
}