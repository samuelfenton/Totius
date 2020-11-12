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
                break;
            case TERRAIN_TOOL.SMOOTH:
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

        float modifyAmount = p_direction == CommonEnums.STEP_DIRECTION.FORWARD ? 0.25f : -0.25f;

        for (int nodeIndex = 0; nodeIndex < groupedNodes.Length; nodeIndex++)
        {
            groupedNodes[nodeIndex].ModifyElevation(modifyAmount);
        }

        m_worldController.UpdateMeshNodeGroup(groupedNodes);

        m_nodeSelector.UpdateSelection();
    }

    /// <summary>
    /// Will slowly move all nodes towards the same height as the central node
    /// </summary>
    private void Level()
    {

    }

    /// <summary>
    /// Will attempt to smooth all nodes towards the average height
    /// </summary>
    private void Smooth()
    {

    }
}
