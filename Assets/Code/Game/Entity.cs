
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


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

    public void SetTransparent(bool trnsp)
    {
        entityRenderer.material.shader = trnsp ? FactoryEntity.Instance.entityShaderTrnsp : FactoryEntity.Instance.entityShaderOpaque;
    }

    public void SetAlpha(float alpha)
    {
        Color matColor = entityRenderer.material.color;
        matColor.a = alpha;
        entityRenderer.material.color = matColor;   
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
