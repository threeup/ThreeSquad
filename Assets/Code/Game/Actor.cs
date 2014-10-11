
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[System.Serializable]
public struct ActorProperties
{
    public Material mat;
}

public class Actor : Entity
{
    public override void Reset()
    {
        base.Reset();
    }

    public void UpdateActor(float deltaTime)
    {
        UpdateEntity(deltaTime);
    }

    public void SetPhysics(ActorProperties prop)
    {
        
    }

    public override void DestroySelf()
    {
        Reset();
        FactoryEntity.Instance.PoolActor(this);
    }
}
