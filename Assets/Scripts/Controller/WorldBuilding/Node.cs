using UnityEngine;

public class Node
{
    public Vector2Int m_localPosition = Vector2Int.zero;
    public Vector3 m_globalPosition = Vector3.zero;

    public float m_elevation = 0.0f;
    public float m_moisture = 0.0f;

    public CommonEnums.NODE_TYPE m_nodeType = CommonEnums.NODE_TYPE.GROUND;
    public CommonEnums.NODE_BIOME m_nodeBiome = CommonEnums.NODE_BIOME.OCEAN;

    private Cell m_parentCell = null;

    public void InitNode(Vector2Int p_localPosition, Cell p_parentCell)
    {
        m_localPosition = p_localPosition;
        m_parentCell = p_parentCell;
        m_elevation = CommonNoiseGen.GetNodeElevation(new Vector2Int(p_localPosition.x + m_parentCell.m_position.x * Cell.CELL_SIZE, p_localPosition.y + m_parentCell.m_position.y * Cell.CELL_SIZE));
        
        m_nodeBiome = CommonData.GetNodeBiome(m_elevation, m_moisture);
        m_nodeType = CommonData.GetNodeType(m_nodeBiome);

        m_globalPosition = new Vector3(m_parentCell.m_position.x * Cell.CELL_SIZE + m_localPosition.x, m_elevation * CommonData.NODE_MAX_HEIGHT, m_parentCell.m_position.y * Cell.CELL_SIZE + m_localPosition.y);
    }
}
