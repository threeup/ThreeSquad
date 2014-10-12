
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class CombatMgr : MonoBehaviour {

	public static CombatMgr Instance;

    public List<Actor> actorList;
    public List<Entity> projectileList;


    void Start()
    {
        if (Instance != null)
        {
            Debug.LogError("CombatMgr Already exists");
        }
        Instance = this;
    }

    public void AddActor(Actor actor)
    {
    	actorList.Add(actor);
    	actor.transform.parent = this.gameObject.transform;
    }

    public void AddProjectile(Entity proj)
    {
    	projectileList.Add(proj);
    	proj.transform.parent = this.gameObject.transform;
    }
}
