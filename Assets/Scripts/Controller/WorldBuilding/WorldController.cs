using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshCollider))]
public class WorldController : MonoBehaviour
{
    public Vector2Int m_currentCell = Vector2Int.zero;

    public Dictionary<Vector2Int, Cell> m_cells = new Dictionary<Vector2Int, Cell>();

    public MeshFilter m_meshFilter = null;
    public MeshCollider m_meshCollider = null;

    private Coroutine m_worldBuilder = null;

    /// <summary>
    /// Build the world
    /// Use files where available otherwise create starting area
    /// </summary>
    public void InitWorld()
    {
        //Setup varibles
        m_meshFilter = GetComponent<MeshFilter>();
        m_meshCollider = GetComponent<MeshCollider>();

        if (m_meshFilter == null || m_meshCollider == null)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            Debug.LogError(name + ": World controller is missing a mesh filter/mesh collider, this is required");
#endif

            Destroy(gameObject);
            return;
        }    

        //TODO if has save files

        //Assuming no save file
        for (int yIndex = -2; yIndex <= 2; yIndex++)
        {
            for (int xIndex = -2; xIndex <= 2; xIndex++)
            {
                Vector2Int cellPosition = new Vector2Int(xIndex, yIndex);

                Cell newCell = new Cell();
                newCell.Generate(cellPosition);

                m_cells.Add(cellPosition, newCell);
            }
        }

        StartCoroutine(UpdateCurrentCell(Vector2Int.zero));
    }

    /// <summary>
    /// Update the current cell, load/generate neighbours as needed
    /// Build new mesh
    /// </summary>
    /// <param name="p_newCurrent">New current position</param>
    public IEnumerator UpdateCurrentCell(Vector2Int p_newCurrent)
    {
        //Ensure all cells are generated
        for (int yIndex = -1; yIndex <= 1; yIndex++)
        {
            for (int xIndex = -1; xIndex <= 1; xIndex++)
            {
                Vector2Int cellPosition = p_newCurrent + new Vector2Int(xIndex, yIndex);

                if (!m_cells.ContainsKey(cellPosition))
                {
                    Cell newCell = new Cell();
                    newCell.Generate(cellPosition);

                    m_cells.Add(cellPosition, newCell);

                    yield return null;
                }
            }
        }

        m_worldBuilder = StartCoroutine(UpdateWorldMesh(p_newCurrent));
    }

    /// <summary>
    /// Based off center cell, build mesh of the center and surrounding
    /// </summary>
    /// <param name="p_centerPosition">Center cell</param>
    /// <returns></returns>
    public IEnumerator UpdateWorldMesh(Vector2Int p_centerPosition)
    {
        //Check if already building new mesh
        if(m_worldBuilder != null)
        {
            StopCoroutine(m_worldBuilder);
            m_worldBuilder = null;
        }

        Mesh newMesh = new Mesh();

        //Build main grid 3x3 size, take 3 frames to complete
        Node[,] nodeGrid3x3 = new Node[Cell.CELL_SIZE * 3, Cell.CELL_SIZE * 3];
        for (int yIndex = -1; yIndex <= 1; yIndex++)
        {
            for (int xIndex = -1; xIndex <= 1; xIndex++)
            {
                Vector2Int offset = new Vector2Int(xIndex, yIndex);
                Vector2Int cellPosition = p_centerPosition + offset;

                AddSubArray(nodeGrid3x3, m_cells[cellPosition].m_nodes, offset);
            }

            yield return null;
        }

        m_worldBuilder = null;
    }

    /// <summary>
    /// Add a sub arry to a larger one
    /// </summary>
    /// <param name="p_mainArray">Main arry copying data into</param>
    /// <param name="p_subArray">Sub array, should be a third the size of the master</param>
    /// <param name="p_offset">Offset should range from -1,-1 -> 1,1 , where -1,-1 is bottom left of array </param>
    private void AddSubArray(Node[,] p_mainArray, Node[,] p_subArray, Vector2Int p_offset)
    {
        Vector2Int startCellPosition = p_offset + Vector2Int.one; //-1,-1 becomes 0,0 as 0,0 is bottom left
        startCellPosition *= Cell.CELL_SIZE;

        for (int y_index = 0; y_index < Cell.CELL_SIZE; y_index++)
        {
            for (int x_index = 0; x_index < Cell.CELL_SIZE; x_index++)
            {
                Vector2Int nodePosition = new Vector2Int(x_index, y_index);
                Vector2Int mainNodePosition = nodePosition + startCellPosition;

                p_mainArray[mainNodePosition.x, mainNodePosition.y] = p_subArray[x_index, y_index];
            }
        }
    }
}
