using UnityEngine;

public class Node
{
    public const float HEIGHT_INCREMENT = 0.25f;

    public Vector2Int m_nodeGrid = Vector2Int.zero;
    public Vector2Int m_globalNodeGrid = Vector2Int.zero;
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
    /// <param name="p_localGrid">Local position in cell</param>
    /// <param name="p_elevation">Elevation of node</param>
    /// <param name="p_moisture">Moisture of node</param>
    /// <param name="p_parentCell">Cell this node is child of</param>
    public void InitNode(Vector2Int p_localGrid, float p_elevation, float p_moisture, Cell p_parentCell)
    {
        m_nodeGrid = p_localGrid;

        m_elevation = Mathf.Clamp(MOARMaths.SnapTowardsIncrement(p_elevation, CommonData.ELEVATION_INCREMENT), 0.0f, 1.0f);
        m_moisture = Mathf.Clamp(p_moisture, 0.0f, 1.0f);
        m_parentCell = p_parentCell;
        
        m_inGameSceneController = (InGame_SceneController)MasterController.Instance.m_sceneController;

        m_globalNodeGrid = new Vector2Int(p_parentCell.m_cellGrid.x * Cell.CELL_SIZE + m_nodeGrid.x, p_parentCell.m_cellGrid.y * Cell.CELL_SIZE + m_nodeGrid.y);

        UpdateStats();
    }

    /// <summary>
    /// Modify elevation using distance to change
    /// </summary>
    /// <param name="p_steps">distance to move</param>
    public void ModifyElevation(float p_distance)
    {
        int stepCount = Mathf.RoundToInt(p_distance / CommonData.NODE_VERTICAL_COUNT);

        m_elevation = Mathf.Clamp(m_elevation + stepCount * CommonData.ELEVATION_INCREMENT, 0.0f, 1.0f); // p_value/CommonData.NODE_MAX_HEIGHT to get actual elevation change for a given distance

        UpdateStats();
    }

    /// <summary>
    /// Modify elevation using step changes
    /// </summary>
    /// <param name="p_steps">how many steps to take</param>
    public void ModifyElevation(int p_steps)
    {
        m_elevation = Mathf.Clamp(m_elevation + p_steps * CommonData.ELEVATION_INCREMENT, 0.0f, 1.0f); // p_value/CommonData.NODE_MAX_HEIGHT to get actual elevation change for a given distance

        UpdateStats();
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
    }

    /// <summary>
    /// Update the nodes stats beased off its elevation and moisture
    /// </summary>
    private void UpdateStats()
    {
        m_nodeBiome = CommonData.GetNodeBiome(m_elevation, m_moisture);
        m_nodeType = CommonData.GetNodeType(m_nodeBiome);

        float height = MOARMaths.SnapTowardsIncrement(m_elevation * CommonData.NODE_MAX_HEIGHT, HEIGHT_INCREMENT);

        m_globalPosition = new Vector3(m_parentCell.m_cellGrid.x * Cell.CELL_SIZE + m_nodeGrid.x, height, m_parentCell.m_cellGrid.y * Cell.CELL_SIZE + m_nodeGrid.y);
    }
}
