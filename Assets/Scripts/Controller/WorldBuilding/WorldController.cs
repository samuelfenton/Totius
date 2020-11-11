using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class WorldController : MonoBehaviour
{
    public Vector2Int m_currentCell = Vector2Int.zero;

    public Dictionary<Vector2Int, Cell> m_cells = new Dictionary<Vector2Int, Cell>();

    private MeshFilter m_meshFilter = null;
    private MeshCollider m_meshCollider = null;
    private Texture2D m_meshTexture = null;

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

        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();

        if (m_meshFilter == null || m_meshCollider == null || meshRenderer == null)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            Debug.LogError(name + ": World controller is missing a mesh filter/mesh collider/mesh renderer, this is required");
#endif

            Destroy(gameObject);
            return;
        }

        m_meshTexture = new Texture2D(Cell.CELL_SIZE * 3, Cell.CELL_SIZE * 3);

        meshRenderer.material.mainTexture = m_meshTexture;

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
    /// Called to update visual cell
    /// TODO ensure isnt currently updating
    /// </summary>
    /// <param name="">New cell position</param>
    public void EnteredNewCell(Vector2Int p_newCurrent)
    {
        StartCoroutine(UpdateCurrentCell(p_newCurrent));
    }

    /// <summary>
    /// Update the current cell, load/generate neighbours as needed
    /// Build new mesh
    /// 
    ///     -------
    ///     | | | |
    ///     -------
    ///     | | | |
    ///     --x----
    ///     | | | |
    ///     -------
    ///     x is where world controller is placed
    ///     mesh is built with this in mind
    /// 
    /// </summary>
    /// <param name="p_newCurrent">New current position</param>
    private IEnumerator UpdateCurrentCell(Vector2Int p_newCurrent)
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

        //Check if already building new mesh
        if (m_worldBuilder != null)
            StopCoroutine(m_worldBuilder);

        m_worldBuilder = StartCoroutine(UpdateWorldMesh(p_newCurrent));
    }

    /// <summary>
    /// Based off center cell, build mesh of the center and surrounding
    /// </summary>
    /// <param name="p_centerPosition">Center cell</param>
    /// <returns></returns>
    private IEnumerator UpdateWorldMesh(Vector2Int p_centerPosition)
    {
        Mesh newMesh = new Mesh();
        int size3x3 = Cell.CELL_SIZE * 3;

        //Build main grid 3x3 size, take 3 frames to complete
        Node[,] nodeGrid3x3 = new Node[size3x3, size3x3];

        for (int yCellIndex = -1; yCellIndex <= 1; yCellIndex++)
        {
            for (int xCellIndex = -1; xCellIndex <= 1; xCellIndex++)
            {
                Vector2Int offset = new Vector2Int(xCellIndex, yCellIndex);
                Vector2Int cellPosition = p_centerPosition + offset;

                AddSubArray(nodeGrid3x3, m_cells[cellPosition].m_nodes, offset);
            }

            yield return null;
        }

        //Build mesh up from 3x3 grid, takes one frame
        Vector3[] verts = new Vector3[size3x3 * size3x3];
        Vector2[] UVs = new Vector2[size3x3 * size3x3];
        int[] tris = new int[(size3x3 - 1) * (size3x3 - 1) * 6];

        //verts/uvs
        for (int yVertIndex = 0; yVertIndex < size3x3; yVertIndex++)
        {
            for (int xVertIndex = 0; xVertIndex < size3x3; xVertIndex++)
            {
                int vertIndex = xVertIndex + yVertIndex * size3x3;

                verts[vertIndex] = new Vector3(xVertIndex - Cell.CELL_SIZE, nodeGrid3x3[xVertIndex, yVertIndex].m_globalPosition.y, yVertIndex - Cell.CELL_SIZE); //Want local relative to center
                UVs[vertIndex] = new Vector2((float)xVertIndex/size3x3, (float)yVertIndex/ size3x3);
            }
        }

        //tris
        for (int yIndex = 0; yIndex < size3x3 - 1; yIndex++)
        {
            for (int xIndex = 0; xIndex < size3x3 - 1; xIndex++)
            {
                int currentVertIndex = xIndex + yIndex * size3x3;
                int triStartingIndex = (xIndex + yIndex * (size3x3 - 1)) * 6;

                tris[triStartingIndex + 0] = currentVertIndex; //Top Left
                tris[triStartingIndex + 1] = currentVertIndex + size3x3; //Bottom Left
                tris[triStartingIndex + 2] = currentVertIndex + 1; //Top Right
                tris[triStartingIndex + 3] = currentVertIndex + 1; //Top Right
                tris[triStartingIndex + 4] = currentVertIndex + size3x3; //Bottom Left
                tris[triStartingIndex + 5] = currentVertIndex + 1 + size3x3; //Bottom Right
            }
        }
        yield return null;

        //Build texture map
        Color[] textureColours = new Color[size3x3 * size3x3];
        for (int yVertIndex = 0; yVertIndex < size3x3; yVertIndex++)
        {
            for (int xVertIndex = 0; xVertIndex < size3x3; xVertIndex++)
            {
                int vertIndex = xVertIndex + yVertIndex * size3x3;

                textureColours[vertIndex] = CommonData.m_nodeTypeColor[nodeGrid3x3[xVertIndex, yVertIndex].m_nodeBiome];
            }
        }
        yield return null;

        //Build mesh 1 frame
        newMesh.vertices = verts;
        newMesh.uv = UVs;
        newMesh.SetTriangles(tris, 0);
        newMesh.RecalculateNormals();

        yield return null;

        //Apply 
        m_meshFilter.mesh = newMesh;
        m_meshCollider.sharedMesh = newMesh;

        m_meshTexture.SetPixels(textureColours);
        m_meshTexture.Apply();

        //Place world controller where it needs to be
        transform.position = new Vector3(p_centerPosition.x * Cell.CELL_SIZE, 0.0f, p_centerPosition.y * Cell.CELL_SIZE);

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
