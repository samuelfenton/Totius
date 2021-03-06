﻿using System.Collections;
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
    private Mesh m_mesh = null;
    private Texture2D m_meshTexture = null;

    private Vector2Int m_currentTraversalToPosition = Vector2Int.zero;
    private Coroutine m_traversingCoroutine = null;
    private Coroutine m_meshBuilderCoroutine = null;

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

        //Build new mesh
        m_mesh = new Mesh();
        Vector3[] verts = new Vector3[Cell.CELL_SIZE_3X * Cell.CELL_SIZE_3X];
        Vector2[] UVs = new Vector2[Cell.CELL_SIZE_3X * Cell.CELL_SIZE_3X];
        int[] tris = new int[(Cell.CELL_SIZE_3X - 1) * (Cell.CELL_SIZE_3X - 1) * 6];

        //verts/uvs
        for (int yVertIndex = 0; yVertIndex < Cell.CELL_SIZE_3X; yVertIndex++)
        {
            for (int xVertIndex = 0; xVertIndex < Cell.CELL_SIZE_3X; xVertIndex++)
            {
                int vertIndex = xVertIndex + yVertIndex * Cell.CELL_SIZE_3X;

                verts[vertIndex] = new Vector3(xVertIndex - Cell.CELL_SIZE, 0.0f, yVertIndex - Cell.CELL_SIZE); //Want local relative to center
                UVs[vertIndex] = new Vector2((float)xVertIndex / Cell.CELL_SIZE_3X, (float)yVertIndex / Cell.CELL_SIZE_3X);
            }
        }

        //tris
        for (int yIndex = 0; yIndex < Cell.CELL_SIZE_3X - 1; yIndex++)
        {
            for (int xIndex = 0; xIndex < Cell.CELL_SIZE_3X - 1; xIndex++)
            {
                int currentVertIndex = xIndex + yIndex * Cell.CELL_SIZE_3X;
                int triStartingIndex = (xIndex + yIndex * (Cell.CELL_SIZE_3X - 1)) * 6;

                tris[triStartingIndex + 0] = currentVertIndex; //Top Left
                tris[triStartingIndex + 1] = currentVertIndex + Cell.CELL_SIZE_3X; //Bottom Left
                tris[triStartingIndex + 2] = currentVertIndex + 1; //Top Right
                tris[triStartingIndex + 3] = currentVertIndex + 1; //Top Right
                tris[triStartingIndex + 4] = currentVertIndex + Cell.CELL_SIZE_3X; //Bottom Left
                tris[triStartingIndex + 5] = currentVertIndex + 1 + Cell.CELL_SIZE_3X; //Bottom Right
            }
        }

        m_mesh.vertices = verts;
        m_mesh.uv = UVs;
        m_mesh.SetTriangles(tris, 0);

        //Apply new mesh
        m_meshFilter.mesh = m_mesh;
        m_meshCollider.sharedMesh = m_mesh;

        m_meshTexture = new Texture2D(Cell.CELL_SIZE * 3, Cell.CELL_SIZE * 3);

        meshRenderer.material.SetTexture("Main_Texture", m_meshTexture);

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

        m_traversingCoroutine = StartCoroutine(UpdateCurrentCell(Vector2Int.zero));
    }

    /// <summary>
    /// Called to update visual cell
    /// TODO ensure isnt currently updating
    /// </summary>
    /// <param name="">New cell position</param>
    public void EnteredNewCell(Vector2Int p_newCurrent)
    {
        if (m_currentCell == p_newCurrent) //Already at cell
            return;

        if (m_currentTraversalToPosition == p_newCurrent) //Already traversing towards
            return;

        m_currentTraversalToPosition = p_newCurrent;

        //If already in progress, stop
        if (m_meshBuilderCoroutine != null)
            StopCoroutine(m_meshBuilderCoroutine);

        if (m_traversingCoroutine != null)
            StopCoroutine(m_traversingCoroutine);

        m_traversingCoroutine = StartCoroutine(UpdateCurrentCell(p_newCurrent));
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
    ///     mesh verts will be in range (-CellSize -> 2xCellSize)
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
        if (m_meshBuilderCoroutine != null)
            StopCoroutine(m_meshBuilderCoroutine);

        m_meshBuilderCoroutine = StartCoroutine(UpdateWorldMesh(p_newCurrent));

        while (m_meshBuilderCoroutine != null)
            yield return null;

        m_traversingCoroutine = null;

        m_currentCell = m_currentTraversalToPosition;
    }

    /// <summary>
    /// Based off center cell, build mesh of the center and surrounding
    /// </summary>
    /// <param name="p_centerPosition">Center cell</param>
    /// <returns></returns>
    private IEnumerator UpdateWorldMesh(Vector2Int p_centerPosition)
    {
        //Build main grid 3x3 size, take 3 frames to complete
        Node[,] nodeGrid3x3 = new Node[Cell.CELL_SIZE_3X, Cell.CELL_SIZE_3X];

        for (int yCellIndex = -1; yCellIndex <= 1; yCellIndex++)
        {
            for (int xCellIndex = -1; xCellIndex <= 1; xCellIndex++)
            {
                Vector2Int offset = new Vector2Int(xCellIndex, yCellIndex);
                Vector2Int cellPosition = p_centerPosition + offset;

                AddSubArray(nodeGrid3x3, m_cells[cellPosition].GetAllNodes(), offset);
            }

            yield return null;
        }

        //Build mesh up from 3x3 grid, takes one frame
        Vector3[] verts = new Vector3[Cell.CELL_SIZE_3X * Cell.CELL_SIZE_3X];

        //verts/uvs
        for (int yVertIndex = 0; yVertIndex < Cell.CELL_SIZE_3X; yVertIndex++)
        {
            for (int xVertIndex = 0; xVertIndex < Cell.CELL_SIZE_3X; xVertIndex++)
            {
                int vertIndex = xVertIndex + yVertIndex * Cell.CELL_SIZE_3X;

                verts[vertIndex] = new Vector3(xVertIndex - Cell.CELL_SIZE, nodeGrid3x3[xVertIndex, yVertIndex].m_globalPosition.y, yVertIndex - Cell.CELL_SIZE); //Want local relative to center
            }
        }

        yield return null;

        //Build texture map
        Color[] textureColours = new Color[Cell.CELL_SIZE_3X * Cell.CELL_SIZE_3X];
        for (int yVertIndex = 0; yVertIndex < Cell.CELL_SIZE_3X; yVertIndex++)
        {
            for (int xVertIndex = 0; xVertIndex < Cell.CELL_SIZE_3X; xVertIndex++)
            {
                int vertIndex = xVertIndex + yVertIndex * Cell.CELL_SIZE_3X;

                textureColours[vertIndex] = CommonData.m_nodeTypeColor[nodeGrid3x3[xVertIndex, yVertIndex].m_nodeBiome];
            }
        }

        yield return null;

        //Build mesh 1 frame
        RebuiltMesh(verts, textureColours);

        //Place world controller where it needs to be
        transform.position = new Vector3(p_centerPosition.x * Cell.CELL_SIZE, 0.0f, p_centerPosition.y * Cell.CELL_SIZE);

        m_meshBuilderCoroutine = null;
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

    /// <summary>
    /// Given a new new set of verts update everything
    /// </summary>
    /// <param name="p_verts">All verts</param>
    /// <param name="p_textureColors">Texture Colours</param>
    private void RebuiltMesh(Vector3[] p_verts, Color[] p_textureColors)
    {
        m_mesh.vertices = p_verts;
        m_mesh.RecalculateNormals();
        
        m_meshCollider.sharedMesh = null;
        m_meshCollider.sharedMesh = m_mesh;

        m_meshTexture.SetPixels(p_textureColors);
        m_meshTexture.Apply();
    }

    /// <summary>
    /// When node has been changed, update its postion in mesh vertex, and texture map
    /// </summary>
    /// <param name="p_node">Node requiring update</param>
    public void UpdateMeshNode(Node p_node)
    {
        Vector3[] verts = m_mesh.vertices;
        Color[] textureColours = m_meshTexture.GetPixels();

        Vector2Int nodeCellDir = p_node.m_parentCell.m_cellGrid - m_currentCell;
        Vector2Int node3x3Position = new Vector2Int(Cell.CELL_SIZE * (nodeCellDir.x + 1) + p_node.m_nodeGrid.x, Cell.CELL_SIZE * (nodeCellDir.y + 1) + p_node.m_nodeGrid.y);

        int vertIndex = node3x3Position.x + node3x3Position.y * Cell.CELL_SIZE_3X;

        if (vertIndex < 0 || vertIndex >= verts.Length || vertIndex >= textureColours.Length) //Early breakout
            return;

        verts[vertIndex] = new Vector3(node3x3Position.x - Cell.CELL_SIZE, p_node.m_globalPosition.y, node3x3Position.y - Cell.CELL_SIZE); //Want local relative to center
        textureColours[vertIndex] = CommonData.m_nodeTypeColor[p_node.m_nodeBiome];

        //Apply changes
        RebuiltMesh(verts, textureColours);
    }

    /// <summary>
    /// When node has been changed, update its postion in mesh vertex, and texture map
    /// This varient is use to update multiple at once
    /// </summary>
    /// <param name="p_nodes">Nodes requiring update</param>
    public void UpdateMeshNodeGroup(Node[] p_nodes)
    {
        Vector3[] verts = m_mesh.vertices;
        Color[] textureColours = m_meshTexture.GetPixels();

        for (int nodeIndex = 0; nodeIndex < p_nodes.Length; nodeIndex++)
        {
            Node node = p_nodes[nodeIndex];

            Vector2Int nodeCellDir = node.m_parentCell.m_cellGrid - m_currentCell;
            Vector2Int node3x3Position = new Vector2Int(Cell.CELL_SIZE * (nodeCellDir.x + 1) + node.m_nodeGrid.x, Cell.CELL_SIZE * (nodeCellDir.y + 1) + node.m_nodeGrid.y);

            int vertIndex = node3x3Position.x + node3x3Position.y * Cell.CELL_SIZE_3X;

            if (vertIndex < 0 || vertIndex >= verts.Length || vertIndex >= textureColours.Length) //Early breakout
                return;

            verts[vertIndex] = new Vector3(node3x3Position.x - Cell.CELL_SIZE, node.m_globalPosition.y, node3x3Position.y - Cell.CELL_SIZE); //Want local relative to center
            textureColours[vertIndex] = CommonData.m_nodeTypeColor[node.m_nodeBiome];
        }

        //Apply changes
        RebuiltMesh(verts, textureColours);
    }

    /// <summary>
    /// Given a origin node, get node using offset
    /// </summary>
    /// <param name="p_orgin">Origin node</param>
    /// <param name="p_offset">Offset to travel looking for node</param>
    /// <returns>Node at given position, else null</returns>
    public Node GetNodeFromOffset(Node p_orgin, Vector2Int p_offset)
    {
        //Grid of nded
        GlobalNodeGridToLocal(p_orgin.m_globalNodeGrid + p_offset, out Vector2Int p_cellGrid, out Vector2Int p_localNodeGrid);

        if(m_cells.TryGetValue(p_cellGrid, out Cell cell))
        {
            return cell.GetNode(p_localNodeGrid);
        }

        return null;
    }

    /// <summary>
    /// Convert a glbal position into a cell grid and local node grid
    /// Doesnt guarentee node exists
    /// </summary>
    /// <param name="p_globalNodeGrid">Global grid of node</param>
    /// <param name="p_cellGrid">Cell grid for node</param>
    /// <param name="p_localNodeGrid">Local node grid within cell</param>
    public void GlobalNodeGridToLocal(Vector2Int p_globalNodeGrid, out Vector2Int p_cellGrid, out Vector2Int p_localNodeGrid)
    {
        p_cellGrid = Vector2Int.zero;
        p_localNodeGrid = Vector2Int.zero;

        //Get cell, only valid technique when positive
        p_cellGrid.x = p_globalNodeGrid.x / Cell.CELL_SIZE;
        p_cellGrid.y = p_globalNodeGrid.y / Cell.CELL_SIZE;

        //Fix result for negitive positions
        if (p_globalNodeGrid.x < 0)
            p_cellGrid.x -= 1;
        if (p_globalNodeGrid.y < 0)
            p_cellGrid.y -= 1;

        //Get local grid, only valid technique when positive
        p_localNodeGrid.x = p_globalNodeGrid.x % Cell.CELL_SIZE;
        p_localNodeGrid.y = p_globalNodeGrid.y % Cell.CELL_SIZE;

        //Fix result for negitive positions
        if (p_globalNodeGrid.x < 0)
            p_localNodeGrid.x += Cell.CELL_SIZE;
        if (p_globalNodeGrid.y < 0)
            p_localNodeGrid.y += Cell.CELL_SIZE;
    }

    /// <summary>
    /// Given position, get the cell the object is currently in
    /// </summary>
    /// <param name="p_position">Position to look for</param>
    /// <returns>Cell position</returns>
    public Vector2Int DetermineCell(Vector3 p_position)
    {
        Vector2Int cellGrid = Vector2Int.zero;

        //Get cell, only valid technique when positive
        cellGrid.x = Mathf.FloorToInt(p_position.x) / Cell.CELL_SIZE;
        cellGrid.y = Mathf.FloorToInt(p_position.z) / Cell.CELL_SIZE;

        //Fix result for negitive positions
        if (p_position.x < 0.0f)
            cellGrid.x -= 1;
        if (p_position.z < 0.0f)
            cellGrid.y -= 1;

        return cellGrid;
    }

    /// <summary>
    /// Given position, get the cell the object is currently in
    /// </summary>
    /// <param name="p_position">Position to look for</param>
    /// <returns>Cell position</returns>
    public Vector2Int DetermineNode(Vector3 p_position)
    {
        Vector2Int nodeGrid = Vector2Int.zero;

        //Get node, only valid technique when positive
        nodeGrid.x = Mathf.FloorToInt(p_position.x) % Cell.CELL_SIZE;
        nodeGrid.y = Mathf.FloorToInt(p_position.z) % Cell.CELL_SIZE;

        //Fix result for negitive positions
        if (p_position.x < 0.0f)
            nodeGrid.x += Cell.CELL_SIZE;
        if (p_position.z < 0.0f)
            nodeGrid.y += Cell.CELL_SIZE;

        return nodeGrid;
    }



    /// <summary>
    /// Get the closest node based off a world position
    /// </summary>
    /// <param name="p_position">Position to look for</param>
    /// <returns>Node closest, null when none found</returns>
    public Node GetClosestNode(Vector3 p_position)
    {
        Vector2Int cellGridPosition = DetermineCell(p_position);

        if(m_cells.TryGetValue(cellGridPosition, out Cell cell))
        {
            Vector2Int nodePosition = DetermineNode(p_position);

            return cell.GetNode(nodePosition);
        }

        return null;
    }
}
