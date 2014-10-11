using UnityEngine;
using System.Collections;
using UnityEngine.SocialPlatforms;

public class World : MonoBehaviour {

    public static World Instance;
    public GeneralMachine machine;

    public Vector2 centerPos = Vector2.zero;
    public int currentWidth = 0;
    public int currentHeight = 0;

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
        machine = new GeneralMachine();
        machine.Initialize(this);
        machine.AddEnterListener(OnNotReady);
        machine.AddEnterListener(OnReady);
        machine.AddEnterListener(OnActive);
        machine.AddEnterListener(OnBusy);
        
        machine.SetState(GeneralState.READY);
    }

    public void OnNotReady(object owner)
    {

    }


    public void OnReady(object owner)
    {
        machine.SetState(GeneralState.ACTIVE);   
    }

    public void OnActive(object owner)
    {
        ExpandWorldToFit(3,3);

    }

    public void OnBusy(object owner)
    {

    }

    public void ExpandWorldToFit(int w, int h)
    {
        if (currentWidth >= w && currentHeight >= h)
        {
            return;
        }
        StartCoroutine(ExpandWorldRoutine(w,h));
    }

    IEnumerator ExpandWorldRoutine(int w, int h)
    {
        machine.SetState(GeneralState.BUSY);    
        float floorRadius = 20.0f;
        GameObject go = null;
        for(int i=currentWidth; i< w; ++i)
        {
            for (int j=currentHeight; j < h; ++j)
            {
                Entity ent = FactoryEntity.Instance.GetFloor();
                go = ent.gameObject;
                go.transform.position = new Vector3(i*floorRadius, 0, j*floorRadius);
                go.name = "Floor"+i+"-"+j;
                go.transform.parent = this.gameObject.transform;
                yield return null;//unnecessary for now
            }
        }

        currentWidth = w;
        currentHeight = h;
        centerPos.x = currentWidth*floorRadius/2f;
        centerPos.y = currentHeight*floorRadius/2f;
        CameraMgr.Instance.MoveTo(centerPos);
        machine.SetState(GeneralState.ACTIVE);
        yield return null;
    }

}
