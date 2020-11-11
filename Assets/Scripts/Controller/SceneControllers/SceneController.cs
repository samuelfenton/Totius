using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController : MonoBehaviour
{
    protected UIController m_UIController = null;

    /// <summary>
    /// Awake on scene controller
    /// Note: Do not override, this is used to ensure update wont be called till InitScene() has been
    /// </summary>
    protected void Awake()
    {
        enabled = false;
    }

    /// <summary>
    /// Initialise the scene controller, should be called from master controller
    /// Note: Dont use start/awake on controller, this ensures correct load order
    /// </summary>
    public virtual void InitScene()
    {
        m_UIController = FindObjectOfType<UIController>();

        if (m_UIController == null)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            Debug.LogError(name + ": Unable to find UIController.cs, this is required within game scene");
#endif
            return;
        }

        m_UIController.InitUI();

        enabled = true;
    }

    /// <summary>
    /// Update
    /// </summary>
    public virtual void Update()
    {

    }

    /// <summary>
    /// Fixed update
    /// </summary>
    public virtual void FixedUpdate()
    {

    }
}
