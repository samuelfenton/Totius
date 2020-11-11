using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterController : MonoBehaviour
{
    public static MasterController Instance { get; private set; }
 
    [HideInInspector]
    public InputController m_input = null;

    [HideInInspector]
    public SceneController m_sceneController = null;

    /// <summary>
    /// Setup singleton functionality for the master controller
    /// </summary>
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        GameObject topObject = gameObject;
        while (topObject.transform.parent != null)
            topObject = topObject.transform.parent.gameObject;

        DontDestroyOnLoad(topObject);

        m_input = gameObject.AddComponent<InputController>();

        m_sceneController = FindObjectOfType<SceneController>();

        if (m_sceneController == null)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            Debug.LogError(name + ": Unable to find SceneController.cs, this is required within game scene");
#endif
            return;
        }

        m_sceneController.InitScene();
    }

    private void Update()
    {
        m_input.UpdateInput();
    }

    private void loadscene()
    {
        /// <summary>
        /// Awake on scene controller
        /// Note: Do not override, this is used to ensure update wont be called till InitScene() has been
        /// </summary>
        //Ensure that the laodede scene controlers are set to not enabled.
        //Only enable as they are loaded in

    }
}
