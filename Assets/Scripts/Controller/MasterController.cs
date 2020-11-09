using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterController : MonoBehaviour
{
    public const int SEED = 1337;
    
    public static MasterController Instance { get; private set; }


    private WorldController m_worldController = null;

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

        InitInGame();
    }

    /// <summary>
    /// Initialise the in game scene
    /// </summary>
    private void InitInGame()
    {
        m_worldController = FindObjectOfType<WorldController>();

        if(m_worldController == null)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            Debug.LogError(name + ": Unable to find WorldController.cs, this is required within game scene");
#endif
        }
        else
        {
            m_worldController.InitWorld();
        }
    }
}
