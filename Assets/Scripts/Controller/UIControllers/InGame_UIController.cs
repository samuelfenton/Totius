using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGame_UIController : UIController
{
    public UI_Toolbar m_UIToolbar = null;

    /// <summary>
    /// Initialise the UI controller, should be called from scene controller
    /// Note: Dont use start/awake on controller, this ensures correct load order
    /// </summary>
    public override void InitUI()
    {
        base.InitUI();

        m_UIToolbar = GetComponentInChildren<UI_Toolbar>();
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
