using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGame_SceneController : SceneController
{
    [HideInInspector]
    public WorldController m_worldController = null;

    private Entity[] m_sceneEntities;
    
    /// <summary>
    /// Initialise the scene controller, should be called from master controller
    /// Note: Dont use start/awake on controller, this ensures correct load order
    /// </summary>
    public override void InitScene()
    {
        base.InitScene();

        //World controller
        m_worldController = FindObjectOfType<WorldController>();

        if (m_worldController == null)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            Debug.LogError(name + ": Unable to find WorldController.cs, this is required within game scene");
#endif
            return;
        }

        m_worldController.InitWorld();


        //Entities
        m_sceneEntities = FindObjectsOfType<Entity>();

        foreach (Entity entity in m_sceneEntities)
        {
            entity.InitEntity();
        }

        //Setup mosue locking
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    /// <summary>
    /// Update
    /// </summary>
    public override void Update()
    {
        base.Update();

        foreach (Entity entity in m_sceneEntities)
        {
            entity.UpdateEntity();
        }
    }

    /// <summary>
    /// Fixed update
    /// </summary>
    public override void FixedUpdate()
    {
        base.FixedUpdate();

        foreach (Entity entity in m_sceneEntities)
        {
            entity.FixedUpdateEntity();
        }
    }
}
