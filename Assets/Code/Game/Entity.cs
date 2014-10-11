
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[System.Serializable]
public struct BlockProperties
{
    public string name;
    public Material mat;
    public float mass;
    public float radius;
}


[System.Serializable]
public struct ProjectileProperties
{
    public string name;
    public Material mat;
    public float mass;
    public float radius;
    public Vector2 vel;
}

public class Entity : MonoBehaviour
{
    public GameObject go;
    public Renderer entityRenderer;
    public bool hasTimeToLive;
    public float timeToLive;

    public virtual void Reset()
    {
        hasTimeToLive = false;
    }

    public void UpdateEntity(float deltaTime)
    {
        if (hasTimeToLive)
        {
            timeToLive -= deltaTime;
            if (timeToLive <= 0f)
            {
                DestroySelf();
            }
        }
    }

    public void SetMaterial(Material mat)
    {
        entityRenderer.material = mat;
    }

    public void SetPhysics(ProjectileProperties prop)
    {

    }
    public void SetPhysics(BlockProperties prop)
    {
        
    }

    public virtual void DestroySelf()
    {
        Reset();
        FactoryEntity.Instance.PoolEntity(this);
    }
}
