using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Utilities
{
	public static void ApplyScale(GameObject go, Vector2 screenFactor)
    {
        Vector3 scale = go.transform.localScale;
        scale.x *= screenFactor.x;
        scale.y *= screenFactor.y;
        go.transform.localScale = scale;
        RectTransform rectTrans = (RectTransform)go.transform;
        Vector3 pos = rectTrans.anchoredPosition;
        pos.x *= screenFactor.x;
        pos.y *= screenFactor.y;
        rectTrans.anchoredPosition = pos;
    }
    
    public static void ReverseScale(GameObject go, Vector2 screenFactor)
    {
    	Vector3 scale = go.transform.localScale;
        scale.x /= screenFactor.x;
        scale.y /= screenFactor.y;
        go.transform.localScale = scale;
        RectTransform rectTrans = (RectTransform)go.transform;
        Vector3 pos = rectTrans.anchoredPosition;
        pos.x /= screenFactor.x;
        pos.y /= screenFactor.y;
        rectTrans.anchoredPosition = pos;
    }

    public static bool Intersect(Rect a, Rect b)
    {
        return (a.x < b.xMax) && (a.xMax > b.x) && (a.y < b.yMax) && (a.yMax > b.y);
    }
}