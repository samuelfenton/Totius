using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu_UIController : UIController
{
    /// <summary>
    /// Initialise the UI controller, should be called from scene controller
    /// Note: Dont use start/awake on controller, this ensures correct load order
    /// </summary>
    public override void InitUI()
    {
        base.InitUI();
    }

    /// <summary>
    /// Update the UI, should be called from scene controller during update
    /// Note: Dont use update on entities, this ensures when pausing game etc functions arent called
    /// </summary>
    public override void UpdateUI()
    {
        base.UpdateUI();
    }
}
