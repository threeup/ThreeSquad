
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class GridLine : MonoBehaviour
{
    public GameObject go;
    public Rect srcLineRect;
    public Rect lineRect;
    public Renderer lineRenderer;
    public bool isVisible;


    public void Init(float x, float y, float w, float h)
    {
        srcLineRect = new Rect(x,y,w,h);
        SetOffset(0f,0f);
        go.transform.localScale = new Vector3(w,h,0.1f);
        SetVisible(false);
    }

    public void SetOffset(float x, float y)
    {
        lineRect = srcLineRect;
        lineRect.x += x;
        lineRect.y += y;    
        go.transform.position = new Vector3(lineRect.x,0f,lineRect.y);
    }

    public virtual void Reset()
    {
        
    }

    public void UpdateGridLine(float deltaTime, Rect cameraRect)
    {

        bool shouldVisible = Utilities.Intersect(cameraRect, this.lineRect);
        if (isVisible != shouldVisible)
        {
            SetVisible(shouldVisible);
        }
    }

    public void SetVisible(bool val)
    {
        lineRenderer.enabled = val;
        isVisible = val;
    }

    public virtual void DestroySelf()
    {
        Reset();
        FactoryExtra.Instance.PoolGridLine(this);
    }
}
