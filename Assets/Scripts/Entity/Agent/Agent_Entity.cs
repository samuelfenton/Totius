using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent_Entity : Entity
{
    /// <summary>
    /// Initialise the entity
    /// Note: Dont use start/awake on entities, this ensures correct load order
    /// </summary>
    public override void InitEntity()
    {
        base.InitEntity();
    }

    /// <summary>
    /// Update the entity, should be called from a scene controller during update
    /// Note: Dont use update on entities, this ensures when pausing game etc functions arent called
    /// </summary>
    public override void UpdateEntity()
    {
        base.UpdateEntity();
    }

    /// <summary>
    /// Fixed update the entity, should be called from a scene controller during fixed update
    /// Note: Dont use fixed update on entities, this ensures when pausing game etc functions arent called
    /// </summary>
    public override void FixedUpdateEntity()
    {
        base.FixedUpdateEntity();
    }
}
