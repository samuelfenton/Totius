using UnityEngine;

public class Node
{
    public Vector2Int m_localPosition = Vector2Int.zero;
    public float m_height = 0.0f;

    public CommonEnums.NODE_TYPE m_nodeType = CommonEnums.NODE_TYPE.GRASS;

    private Cell m_parentCell = null;

    public void InitNode(Vector2Int p_localPosition, float p_height, Cell p_parentCell, CommonEnums.NODE_TYPE p_nodeType)
    {
        m_localPosition = p_localPosition;
        m_height = p_height;
        m_parentCell = p_parentCell;
        m_nodeType = p_nodeType;
    }

    /// <summary>
    /// Determine a nodes global position
    /// Take parent cell into account
    /// </summary>
    /// <returns>Global position when valid, default to negativeInfinty</returns>
    private Vector3 GetGlobalPosition()
    {
        if (m_parentCell == null)
            return Vector3.negativeInfinity;

        return new Vector3(m_parentCell.m_position.x * Cell.CELL_SIZE + m_localPosition.x, m_height, m_parentCell.m_position.y * Cell.CELL_SIZE + m_localPosition.y);
    }
}
