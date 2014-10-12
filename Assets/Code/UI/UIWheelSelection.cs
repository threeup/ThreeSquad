using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class UIWheelSelection : MonoBehaviour {
    public GameObject go;
    public Image image;
    public Text textObj;
    public int index;

    public UIProperties prop;
    
    public UIWheelSelection next;
    public UIWheelSelection prev;
    public Vector2 position;
    
    public bool isSelected = false;
    public bool isMoving = false;
    
    public Queue<Vector3> destinationQueue;
    public Vector2 destinationPosition;
    public int destinationIndex;

    public float tweenDuration = 0.0f;

    public void Init(GameObject go, int index)
    {
        this.go = go;
        this.image = go.GetComponent<Image>() as Image;
        this.index = index;
        Vector3 pos = go.transform.position;
        this.position = new Vector2(pos.x, pos.y);
        this.destinationPosition = this.position;
        destinationQueue = new Queue<Vector3>();
    }

    public void Redraw()
    {
        Vector3 pos = go.transform.position;
        this.position = new Vector2(pos.x, pos.y);
        this.destinationPosition = this.position;
    }

    public void SetPrev(UIWheelSelection selection)
    {
        this.prev = selection;
        selection.next = this;
    }

    public void MoveTo(UIWheelSelection other, float tweenDuration)
    {
        this.tweenDuration = tweenDuration;
        Vector3 destination = new Vector3(other.position.x, other.position.y, other.index);
        destinationQueue.Enqueue(destination);
        CheckQueue();
    }

    public void CheckQueue()
    {
        if (isMoving || destinationQueue.Count == 0)
        {
            return;
        }
        Vector3 destination = destinationQueue.Dequeue();
        destinationPosition.x = destination.x;
        destinationPosition.y = destination.y;
        destinationIndex = (int)destination.z;
        LeanTween.move(go, destinationPosition, tweenDuration).setEase(LeanTweenType.easeInOutQuad);
        isMoving = true;
    }

    public void FinishMoving()
    {
        position = destinationPosition;
        index = destinationIndex;
        isMoving = false;
        CheckQueue();
    }

    public UIWheelSelection GetNextOrPrev(bool isNext)
    {
        return isNext ? next: prev;
    }

    public void SetSelected(bool val)
    {
        isSelected = val;
        image.color = isSelected ? Color.white : Color.gray;
    }

    public void SetUIProp(UIProperties prop)
    {
        this.prop = prop;
        textObj.text = prop.text;
        
    }

    public void ClearUIProp()
    {
        textObj.text = "-";
    }
}
