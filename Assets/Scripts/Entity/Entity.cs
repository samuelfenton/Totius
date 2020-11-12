using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Entity : MonoBehaviour
{
    protected Rigidbody m_rigidbody = null;

    /// <summary>
    /// Initialise the entity
    /// Note: Dont use start/awake on entities, this ensures correct load order
    /// </summary>
    public virtual void InitEntity()
    {
        m_rigidbody = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Update the entity, should be called from a scene controller during update
    /// Note: Dont use update on entities, this ensures when pausing game etc functions arent called
    /// </summary>
    public virtual void UpdateEntity()
    {

    }

    /// <summary>
    /// Fixed update the entity, should be called from a scene controller during fixed update
    /// Note: Dont use fixed update on entities, this ensures when pausing game etc functions arent called
    /// </summary>
    public virtual void FixedUpdateEntity()
    {

    }
}
