using UnityEngine;

public class Node
{
    public Vector2Int m_localPosition = Vector2Int.zero;
    public Vector3 m_globalPosition = Vector3.zero;

    public float m_elevation = 0.0f;
    public float m_moisture = 0.0f;

    public CommonEnums.NODE_TYPE m_nodeType = CommonEnums.NODE_TYPE.GROUND;
    public CommonEnums.NODE_BIOME m_nodeBiome = CommonEnums.NODE_BIOME.OCEAN;

    #region Stored Variables
    public Cell m_parentCell = null;
    private InGame_SceneController m_inGameSceneController = null;
    #endregion  

    /// <summary>
    /// Init node
    /// </summary>
    /// <param name="p_localPosition">Local position in cell</param>
    /// <param name="p_elevation">Elevation of node</param>
    /// <param name="p_moisture">Moisture of node</param>
    /// <param name="p_parentCell">Cell this node is child of</param>
    public void InitNode(Vector2Int p_localPosition, float p_elevation, float p_moisture, Cell p_parentCell)
    {
        m_localPosition = p_localPosition;
        m_elevation = Mathf.Clamp(p_elevation, 0.0f, 1.0f);
        m_moisture = Mathf.Clamp(p_moisture, 0.0f, 1.0f);
        m_parentCell = p_parentCell;

        m_inGameSceneController = (InGame_SceneController)MasterController.Instance.m_sceneController;

        UpdateStats();
    }

    /// <summary>
    /// Modify elevation by value
    /// Still clamps
    /// </summary>
    /// <param name="p_newVal">New value</param>
    public void ModifyElevation(float p_value)
    {
        m_elevation = Mathf.Clamp(m_elevation + p_value, 0.0f, 1.0f);

        UpdateStats();

        m_inGameSceneController.m_worldController.UpdateMeshNode(this);
    }

    /// <summary>
    /// Hard set elevation to new value
    /// Still clamps
    /// </summary>
    /// <param name="p_newVal">New value</param>
    public void SetElevation(float p_newVal)
    {
        m_elevation = Mathf.Clamp(p_newVal, 0.0f, 1.0f);

        UpdateStats();

        m_inGameSceneController.m_worldController.UpdateMeshNode(this);
    }

    /// <summary>
    /// Modify moisture by value
    /// Still clamps
    /// </summary>
    /// <param name="p_newVal">New value</param>
    public void ModifyMoisture(float p_value)
    {
        m_elevation = Mathf.Clamp(m_moisture + p_value, 0.0f, 1.0f);

        UpdateStats();

        m_inGameSceneController.m_worldController.UpdateMeshNode(this);
    }

    /// <summary>
    /// Hard set moisture to new value
    /// Still clamps
    /// </summary>
    /// <param name="p_newVal">New value</param>
    public void SetMoisture(float p_newVal)
    {
        m_moisture = Mathf.Clamp(p_newVal, 0.0f, 1.0f);

        UpdateStats();

        m_inGameSceneController.m_worldController.UpdateMeshNode(this);
    }

    private void UpdateStats()
    {
        m_nodeBiome = CommonData.GetNodeBiome(m_elevation, m_moisture);
        m_nodeType = CommonData.GetNodeType(m_nodeBiome);

        m_globalPosition = new Vector3(m_parentCell.m_cellGridPosition.x * Cell.CELL_SIZE + m_localPosition.x, m_elevation * CommonData.NODE_MAX_HEIGHT, m_parentCell.m_cellGridPosition.y * Cell.CELL_SIZE + m_localPosition.y);
    }
}
