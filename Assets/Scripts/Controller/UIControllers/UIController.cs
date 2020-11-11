using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    /// <summary>
    /// Initialise the UI controller, should be called from scene controller
    /// Note: Dont use start/awake on controller, this ensures correct load order
    /// </summary>
    public virtual void InitUI()
    {

    }

    /// <summary>
    /// Update the UI, should be called from scene controller during update
    /// Note: Dont use update on entities, this ensures when pausing game etc functions arent called
    /// </summary>
    public virtual void UpdateUI()
    {

    }
}
