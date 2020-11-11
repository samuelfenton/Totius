using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeSelector : MonoBehaviour
{
    /// <summary>
    /// Debug stuff
    /// 
    ///             Debug.Log("Looking at: cell: " + cell.m_cellGridPosition + " node: " + nodePosition);
    /// </summary>


    [Header("Selector Variables")]
    public float m_selectorDistance = 2.5f;
    public float m_heightChange = 0.05f;

    #region Stored Variables
    private GameObject m_followObject = null;
    private InGame_SceneController m_inGameSceneController = null;
    #endregion  

    public void Init(GameObject p_followObject)
    {
        m_followObject = p_followObject;

        m_inGameSceneController = (InGame_SceneController)MasterController.Instance.m_sceneController;
    }

    private void Update()
    {
        if (m_followObject == null)
            return;

        //Get selected node
        Vector3 placementVector = m_followObject.transform.position + m_followObject.transform.forward * m_selectorDistance;

        //TODO first attempt to get ray cast position, if not possible, get closet within threshold, else nothing

        Node selectedNode = m_inGameSceneController.m_worldController.GetClosestNode(placementVector);

        if (selectedNode == null)
            transform.position = new Vector3(0.0f, -1000.0f, 0.0f);
        else
        { 
            transform.position = selectedNode.m_globalPosition;
            
            //Apply any changes wanted

            if(MasterController.Instance.m_input.GetKey(InputController.INPUT_KEY.LIGHT_ATTACK) == InputController.INPUT_STATE.DOWNED)
            {
                selectedNode.ModifyElevation(m_heightChange);
            }
            else if(MasterController.Instance.m_input.GetKey(InputController.INPUT_KEY.HEAVY_ATTACK) == InputController.INPUT_STATE.DOWNED)
            {
                selectedNode.ModifyElevation(-m_heightChange);
            }
        }
    }
}
