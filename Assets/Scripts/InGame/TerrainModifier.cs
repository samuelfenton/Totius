using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainModifier : MonoBehaviour
{
    public enum TERRAIN_TOOL {HEIGHT_ADJUST, LEVEL, SMOOTH, TOOL_COUNT}

    public TERRAIN_TOOL m_currentTerrainTool = TERRAIN_TOOL.HEIGHT_ADJUST;

    private NodeSelector m_nodeSelector = null;

    private InGame_SceneController m_inGameSceneController = null;
    private WorldController m_worldController = null;

    /// <summary>
    /// Initialise the terrain modifying tool
    /// </summary>
    public void InitTerrainModifier()
    {
        m_inGameSceneController = (InGame_SceneController)MasterController.Instance.m_sceneController;
        m_worldController = m_inGameSceneController.m_worldController;

        Flyover_Camera flyoverCamera = FindObjectOfType<Flyover_Camera>();

        //Find node selector and set it up
        m_nodeSelector = FindObjectOfType<NodeSelector>();

        if (m_nodeSelector != null && flyoverCamera != null)
            m_nodeSelector.Init(flyoverCamera.gameObject);
    }

    /// <summary>
    /// Update the terrain modifying tool
    /// Get input and change as needed
    /// </summary>
    public void UpdateTerrainModifer()
    {
        //Scrolling
        int scrollVal = MasterController.Instance.m_input.GetScroll();

        if (scrollVal != 0)
        {
            m_nodeSelector.NextSelectionType(scrollVal == 1 ? CommonEnums.STEP_DIRECTION.FORWARD : CommonEnums.STEP_DIRECTION.BACKWARD);
        }

        //Update node
        m_nodeSelector.UpdateNodeSelection();

        //Modifications
        switch (m_currentTerrainTool)
        {
            case TERRAIN_TOOL.HEIGHT_ADJUST:
                if(MasterController.Instance.m_input.GetKey(InputController.INPUT_KEY.LIGHT_ATTACK) == InputController.INPUT_STATE.DOWNED)
                {
                    HeightAdjust(CommonEnums.STEP_DIRECTION.FORWARD);
                }
                else if(MasterController.Instance.m_input.GetKey(InputController.INPUT_KEY.HEAVY_ATTACK) == InputController.INPUT_STATE.DOWNED)
                {
                    HeightAdjust(CommonEnums.STEP_DIRECTION.BACKWARD);
                }
                break;
            case TERRAIN_TOOL.LEVEL:
                if (MasterController.Instance.m_input.GetKey(InputController.INPUT_KEY.LIGHT_ATTACK) == InputController.INPUT_STATE.DOWNED)
                {
                    Level();
                }
                break;
            case TERRAIN_TOOL.SMOOTH:
                if (MasterController.Instance.m_input.GetKey(InputController.INPUT_KEY.LIGHT_ATTACK) == InputController.INPUT_STATE.DOWNED)
                {
                    Smooth();
                }
                break;
            default:
                break;
        }

    }


    /// <summary>
    /// Adjust the height
    /// Will move all nodes selected up/down in height 
    /// </summary>
    /// <param name="p_direction">Direction to move in, assuming that forwards is up, backwards if down</param>
    private void HeightAdjust(CommonEnums.STEP_DIRECTION p_direction)
    {
        Node[] groupedNodes = m_nodeSelector.m_storedNodeGroup;

        if (groupedNodes.Length == 0)
            return;

        for (int nodeIndex = 0; nodeIndex < groupedNodes.Length; nodeIndex++)
        {
            groupedNodes[nodeIndex].ModifyElevation((int)p_direction);
        }

        m_worldController.UpdateMeshNodeGroup(groupedNodes);

        m_nodeSelector.UpdateSelection();
    }

    /// <summary>
    /// Will slowly move all nodes towards the same height as the central node
    /// </summary>
    private void Level()
    {
        Node[] groupedNodes = m_nodeSelector.m_storedNodeGroup;

        Node centralNode = m_nodeSelector.m_selectedNode;

        if (groupedNodes.Length == 0 || centralNode == null)
            return;

        float centralElevation = centralNode.m_elevation;

        MoveTowardsElevation(groupedNodes, centralElevation);

        m_worldController.UpdateMeshNodeGroup(groupedNodes);

        m_nodeSelector.UpdateSelection();
    }

    /// <summary>
    /// Will attempt to smooth all nodes towards the average height
    /// </summary>
    private void Smooth()
    {
        Node[] groupedNodes = m_nodeSelector.m_storedNodeGroup;

        if (groupedNodes.Length <=1)//Ignore if only one point
            return;

        float averageHeight = 0.0f;

        //Get average
        for (int nodeIndex = 0; nodeIndex < groupedNodes.Length; nodeIndex++)
        {
            averageHeight += groupedNodes[nodeIndex].m_elevation;
        }

        averageHeight = MOARMaths.SnapTowardsIncrement(averageHeight / groupedNodes.Length, CommonData.ELEVATION_INCREMENT);

        //Apply to nodes
        MoveTowardsElevation(groupedNodes, averageHeight);

        m_worldController.UpdateMeshNodeGroup(groupedNodes);

        m_nodeSelector.UpdateSelection();
    }

    /// <summary>
    /// Move gorup of nodes towards a single elevation
    /// </summary>
    /// <param name="p_groupedNodes">Nodes to move</param>
    /// <param name="p_targetElevation">Target elevation</param>
    private void MoveTowardsElevation(Node[] p_groupedNodes, float p_targetElevation)
    {
        //Apply to nodes
        for (int nodeIndex = 0; nodeIndex < p_groupedNodes.Length; nodeIndex++)
        {
            Node modifyingNode = p_groupedNodes[nodeIndex];

            float elevaitonDif = p_targetElevation - modifyingNode.m_elevation;

            if (elevaitonDif > CommonData.ELEVATION_INCREMENT_HALF) //Its higher, move up
            {
                modifyingNode.ModifyElevation(1);
            }
            else if (elevaitonDif < -CommonData.ELEVATION_INCREMENT_HALF)//Its lower, move down
            {
                modifyingNode.ModifyElevation(-1);
            }
            else//Approx no dif, hard set
            {
                modifyingNode.SetElevation(p_targetElevation);
            }
        }
    }
}
