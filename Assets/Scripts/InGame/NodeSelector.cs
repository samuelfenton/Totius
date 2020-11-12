using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeSelector : MonoBehaviour
{
    private const int MAX_SELECTOR_COUNT = 81; //Should be the largest selectable area, being SQUARE9X9 = 9x9 selectors = 81

    #region Selection Dirs

    /* 9x9 template, use tab indents to allow visual
    {
        new Vector2Int(-4, 4),  new Vector2Int(-3, 4),  new Vector2Int(-2, 4),  new Vector2Int(-1, 4),  new Vector2Int(0, 4),   new Vector2Int(1, 4),   new Vector2Int(2, 4),   new Vector2Int(3, 4),   new Vector2Int(4, 4), 
        new Vector2Int(-4, 3),  new Vector2Int(-3, 3),  new Vector2Int(-2, 3),  new Vector2Int(-1, 3),  new Vector2Int(0, 3),   new Vector2Int(1, 3),   new Vector2Int(2, 3),   new Vector2Int(3, 3),   new Vector2Int(4, 3), 
        new Vector2Int(-4, 2),  new Vector2Int(-3, 2),  new Vector2Int(-2, 2),  new Vector2Int(-1, 2),  new Vector2Int(0, 2),   new Vector2Int(1, 2),   new Vector2Int(2, 2),   new Vector2Int(3, 2),   new Vector2Int(4, 2), 
        new Vector2Int(-4, 1),  new Vector2Int(-3, 1),  new Vector2Int(-2, 1),  new Vector2Int(-1, 1),  new Vector2Int(0, 1),   new Vector2Int(1, 1),   new Vector2Int(2, 1),   new Vector2Int(3, 1),   new Vector2Int(4, 1), 
        new Vector2Int(-4, 0),  new Vector2Int(-3, 0),  new Vector2Int(-2, 0),  new Vector2Int(-1, 0),  new Vector2Int(0, 0),   new Vector2Int(1, 0),   new Vector2Int(2, 0),   new Vector2Int(3, 0),   new Vector2Int(4, 0), 
        new Vector2Int(-4, -1), new Vector2Int(-3, -1), new Vector2Int(-2, -1), new Vector2Int(-1, -1), new Vector2Int(0, -1),  new Vector2Int(1, -1),  new Vector2Int(2, -1),  new Vector2Int(3, -1),  new Vector2Int(4, -1), 
        new Vector2Int(-4, -2), new Vector2Int(-3, -2), new Vector2Int(-2, -2), new Vector2Int(-1, -2), new Vector2Int(0, -2),  new Vector2Int(1, -2),  new Vector2Int(2, -2),  new Vector2Int(3, -2),  new Vector2Int(4, -2), 
        new Vector2Int(-4, -3), new Vector2Int(-3, -3), new Vector2Int(-2, -3), new Vector2Int(-1, -3), new Vector2Int(0, -3),  new Vector2Int(1, -3),  new Vector2Int(2, -3),  new Vector2Int(3, -3),  new Vector2Int(4, -3), 
        new Vector2Int(-4, -4), new Vector2Int(-3, -4), new Vector2Int(-2, -4), new Vector2Int(-1, -4), new Vector2Int(0, -4),  new Vector2Int(1, -4),  new Vector2Int(2, -4),  new Vector2Int(3, -4),  new Vector2Int(4, -4)
    }
    */
    private Dictionary<SELECTION_TYPE, Vector2Int[]> m_selectionDirs = new Dictionary<SELECTION_TYPE, Vector2Int[]>()
    {
        { SELECTION_TYPE.SINGLE, new Vector2Int[]
            {
            
            
            
            
                                                                                                            new Vector2Int(0, 0)



            }
        },
        { SELECTION_TYPE.SQUARE3X3, new Vector2Int[]
            {
             
             
             
                                                                                    new Vector2Int(-1, 1),  new Vector2Int(0, 1),   new Vector2Int(1, 1),   
                                                                                    new Vector2Int(-1, 0),  new Vector2Int(0, 0),   new Vector2Int(1, 0),   
                                                                                    new Vector2Int(-1, -1), new Vector2Int(0, -1),  new Vector2Int(1, -1)  
 
 
 
            }
        },
        { SELECTION_TYPE.SQUARE5X5, new Vector2Int[]
            {
             
             
                                                            new Vector2Int(-2, 2),  new Vector2Int(-1, 2),  new Vector2Int(0, 2),   new Vector2Int(1, 2),   new Vector2Int(2, 2),   
                                                            new Vector2Int(-2, 1),  new Vector2Int(-1, 1),  new Vector2Int(0, 1),   new Vector2Int(1, 1),   new Vector2Int(2, 1),   
                                                            new Vector2Int(-2, 0),  new Vector2Int(-1, 0),  new Vector2Int(0, 0),   new Vector2Int(1, 0),   new Vector2Int(2, 0),   
                                                            new Vector2Int(-2, -1), new Vector2Int(-1, -1), new Vector2Int(0, -1),  new Vector2Int(1, -1),  new Vector2Int(2, -1),  
                                                            new Vector2Int(-2, -2), new Vector2Int(-1, -2), new Vector2Int(0, -2),  new Vector2Int(1, -2),  new Vector2Int(2, -2)  
             
             
            }
        },
        { SELECTION_TYPE.SQUARE7X7, new Vector2Int[]
            {
            
                                    new Vector2Int(-3, 3),  new Vector2Int(-2, 3),  new Vector2Int(-1, 3),  new Vector2Int(0, 3),   new Vector2Int(1, 3),   new Vector2Int(2, 3),   new Vector2Int(3, 3),    
                                    new Vector2Int(-3, 2),  new Vector2Int(-2, 2),  new Vector2Int(-1, 2),  new Vector2Int(0, 2),   new Vector2Int(1, 2),   new Vector2Int(2, 2),   new Vector2Int(3, 2),    
                                    new Vector2Int(-3, 1),  new Vector2Int(-2, 1),  new Vector2Int(-1, 1),  new Vector2Int(0, 1),   new Vector2Int(1, 1),   new Vector2Int(2, 1),   new Vector2Int(3, 1),    
                                    new Vector2Int(-3, 0),  new Vector2Int(-2, 0),  new Vector2Int(-1, 0),  new Vector2Int(0, 0),   new Vector2Int(1, 0),   new Vector2Int(2, 0),   new Vector2Int(3, 0),    
                                    new Vector2Int(-3, -1), new Vector2Int(-2, -1), new Vector2Int(-1, -1), new Vector2Int(0, -1),  new Vector2Int(1, -1),  new Vector2Int(2, -1),  new Vector2Int(3, -1),   
                                    new Vector2Int(-3, -2), new Vector2Int(-2, -2), new Vector2Int(-1, -2), new Vector2Int(0, -2),  new Vector2Int(1, -2),  new Vector2Int(2, -2),  new Vector2Int(3, -2),   
                                    new Vector2Int(-3, -3), new Vector2Int(-2, -3), new Vector2Int(-1, -3), new Vector2Int(0, -3),  new Vector2Int(1, -3),  new Vector2Int(2, -3),  new Vector2Int(3, -3),   
 
            }
        },
        { SELECTION_TYPE.SQUARE9X9, new Vector2Int[]
            {
            new Vector2Int(-4, 4),  new Vector2Int(-3, 4),  new Vector2Int(-2, 4),  new Vector2Int(-1, 4),  new Vector2Int(0, 4),   new Vector2Int(1, 4),   new Vector2Int(2, 4),   new Vector2Int(3, 4),   new Vector2Int(4, 4),
            new Vector2Int(-4, 3),  new Vector2Int(-3, 3),  new Vector2Int(-2, 3),  new Vector2Int(-1, 3),  new Vector2Int(0, 3),   new Vector2Int(1, 3),   new Vector2Int(2, 3),   new Vector2Int(3, 3),   new Vector2Int(4, 3),
            new Vector2Int(-4, 2),  new Vector2Int(-3, 2),  new Vector2Int(-2, 2),  new Vector2Int(-1, 2),  new Vector2Int(0, 2),   new Vector2Int(1, 2),   new Vector2Int(2, 2),   new Vector2Int(3, 2),   new Vector2Int(4, 2),
            new Vector2Int(-4, 1),  new Vector2Int(-3, 1),  new Vector2Int(-2, 1),  new Vector2Int(-1, 1),  new Vector2Int(0, 1),   new Vector2Int(1, 1),   new Vector2Int(2, 1),   new Vector2Int(3, 1),   new Vector2Int(4, 1),
            new Vector2Int(-4, 0),  new Vector2Int(-3, 0),  new Vector2Int(-2, 0),  new Vector2Int(-1, 0),  new Vector2Int(0, 0),   new Vector2Int(1, 0),   new Vector2Int(2, 0),   new Vector2Int(3, 0),   new Vector2Int(4, 0),
            new Vector2Int(-4, -1), new Vector2Int(-3, -1), new Vector2Int(-2, -1), new Vector2Int(-1, -1), new Vector2Int(0, -1),  new Vector2Int(1, -1),  new Vector2Int(2, -1),  new Vector2Int(3, -1),  new Vector2Int(4, -1),
            new Vector2Int(-4, -2), new Vector2Int(-3, -2), new Vector2Int(-2, -2), new Vector2Int(-1, -2), new Vector2Int(0, -2),  new Vector2Int(1, -2),  new Vector2Int(2, -2),  new Vector2Int(3, -2),  new Vector2Int(4, -2),
            new Vector2Int(-4, -3), new Vector2Int(-3, -3), new Vector2Int(-2, -3), new Vector2Int(-1, -3), new Vector2Int(0, -3),  new Vector2Int(1, -3),  new Vector2Int(2, -3),  new Vector2Int(3, -3),  new Vector2Int(4, -3),
            new Vector2Int(-4, -4), new Vector2Int(-3, -4), new Vector2Int(-2, -4), new Vector2Int(-1, -4), new Vector2Int(0, -4),  new Vector2Int(1, -4),  new Vector2Int(2, -4),  new Vector2Int(3, -4),  new Vector2Int(4, -4)
            }
        },
        { SELECTION_TYPE.CIRCLE3X3, new Vector2Int[]
            {
             
             
             
                                                                                                            new Vector2Int(0, 1),    
                                                                                    new Vector2Int(-1, 0),  new Vector2Int(0, 0),   new Vector2Int(1, 0),    
                                                                                                            new Vector2Int(0, -1) 
 
 
 
            }
        },
        { SELECTION_TYPE.CIRCLE5X5, new Vector2Int[]
            {
             

                                                                                    new Vector2Int(-1, 2),  new Vector2Int(0, 2),   new Vector2Int(1, 2),     
                                                            new Vector2Int(-2, 1),  new Vector2Int(-1, 1),  new Vector2Int(0, 1),   new Vector2Int(1, 1),   new Vector2Int(2, 1),   
                                                            new Vector2Int(-2, 0),  new Vector2Int(-1, 0),  new Vector2Int(0, 0),   new Vector2Int(1, 0),   new Vector2Int(2, 0),   
                                                            new Vector2Int(-2, -1), new Vector2Int(-1, -1), new Vector2Int(0, -1),  new Vector2Int(1, -1),  new Vector2Int(2, -1),  
                                                                                    new Vector2Int(-1, -2), new Vector2Int(0, -2),  new Vector2Int(1, -2) 
             

            }
        },
        { SELECTION_TYPE.CIRCLE7X7, new Vector2Int[]
            {
             
                                                                                    new Vector2Int(-1, 3),  new Vector2Int(0, 3),   new Vector2Int(1, 3),
                                                            new Vector2Int(-2, 2),  new Vector2Int(-1, 2),  new Vector2Int(0, 2),   new Vector2Int(1, 2),   new Vector2Int(2, 2),
                                    new Vector2Int(-3, 1),  new Vector2Int(-2, 1),  new Vector2Int(-1, 1),  new Vector2Int(0, 1),   new Vector2Int(1, 1),   new Vector2Int(2, 1),   new Vector2Int(3, 1), 
                                    new Vector2Int(-3, 0),  new Vector2Int(-2, 0),  new Vector2Int(-1, 0),  new Vector2Int(0, 0),   new Vector2Int(1, 0),   new Vector2Int(2, 0),   new Vector2Int(3, 0), 
                                    new Vector2Int(-3, -1), new Vector2Int(-2, -1), new Vector2Int(-1, -1), new Vector2Int(0, -1),  new Vector2Int(1, -1),  new Vector2Int(2, -1),  new Vector2Int(3, -1),
                                                            new Vector2Int(-2, -2), new Vector2Int(-1, -2), new Vector2Int(0, -2),  new Vector2Int(1, -2),  new Vector2Int(2, -2),
                                                                                    new Vector2Int(-1, -3), new Vector2Int(0, -3),  new Vector2Int(1, -3)

            }
        },
        { SELECTION_TYPE.CIRCLE9X9, new Vector2Int[]
            {
                                                                                    new Vector2Int(-1, 4),  new Vector2Int(0, 4),   new Vector2Int(1, 4),   
                                    new Vector2Int(-3, 3),  new Vector2Int(-2, 3),  new Vector2Int(-1, 3),  new Vector2Int(0, 3),   new Vector2Int(1, 3),   new Vector2Int(2, 3),   new Vector2Int(3, 3),
                                    new Vector2Int(-3, 2),  new Vector2Int(-2, 2),  new Vector2Int(-1, 2),  new Vector2Int(0, 2),   new Vector2Int(1, 2),   new Vector2Int(2, 2),   new Vector2Int(3, 2),   
            new Vector2Int(-4, 1),  new Vector2Int(-3, 1),  new Vector2Int(-2, 1),  new Vector2Int(-1, 1),  new Vector2Int(0, 1),   new Vector2Int(1, 1),   new Vector2Int(2, 1),   new Vector2Int(3, 1),   new Vector2Int(4, 1),
            new Vector2Int(-4, 0),  new Vector2Int(-3, 0),  new Vector2Int(-2, 0),  new Vector2Int(-1, 0),  new Vector2Int(0, 0),   new Vector2Int(1, 0),   new Vector2Int(2, 0),   new Vector2Int(3, 0),   new Vector2Int(4, 0),
            new Vector2Int(-4, -1), new Vector2Int(-3, -1), new Vector2Int(-2, -1), new Vector2Int(-1, -1), new Vector2Int(0, -1),  new Vector2Int(1, -1),  new Vector2Int(2, -1),  new Vector2Int(3, -1),  new Vector2Int(4, -1),
                                    new Vector2Int(-3, -2), new Vector2Int(-2, -2), new Vector2Int(-1, -2), new Vector2Int(0, -2),  new Vector2Int(1, -2),  new Vector2Int(2, -2),  new Vector2Int(3, -2),  
                                    new Vector2Int(-3, -3), new Vector2Int(-2, -3), new Vector2Int(-1, -3), new Vector2Int(0, -3),  new Vector2Int(1, -3),  new Vector2Int(2, -3),  new Vector2Int(3, -3),  
                                                                                    new Vector2Int(-1, -4), new Vector2Int(0, -4),  new Vector2Int(1, -4) 
            }
        }
    };

    #endregion

    public enum SELECTION_TYPE {SINGLE, SQUARE3X3, SQUARE5X5, SQUARE7X7, SQUARE9X9, CIRCLE3X3, CIRCLE5X5, CIRCLE7X7, CIRCLE9X9, SELECTION_COUNT}
    [Header("Selector Variables")]
    public float m_selectorDistance = 2.5f;
    public float m_heightChange = 0.05f;
    public GameObject m_selectorPrefab = null;

    #region Reference Variables
    private GameObject m_followObject = null;
    private InGame_SceneController m_inGameSceneController = null;
    #endregion  

    public Node m_storedNode = null;
    public Node[] m_storedNodeGroup = new Node[0];

    private Queue<KeyValuePair<GameObject, MeshRenderer>> m_selectorsFree = new Queue<KeyValuePair<GameObject, MeshRenderer>>();
    private Queue<KeyValuePair<GameObject, MeshRenderer>> m_selectorsInUse = new Queue<KeyValuePair<GameObject, MeshRenderer>>();

    private SELECTION_TYPE m_selectionType = SELECTION_TYPE.SINGLE;

    public void Init(GameObject p_followObject)
    {
        m_followObject = p_followObject;

        m_inGameSceneController = (InGame_SceneController)MasterController.Instance.m_sceneController;

        //Reset Variables
        m_storedNode = null;
        m_storedNodeGroup = new Node[0];

        if (m_selectorPrefab == null)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            Debug.LogError(name + ": is missing its assigned selector prefab");
#endif
        }
        else
        { 
            for (int selectorChildIndex = 0; selectorChildIndex < MAX_SELECTOR_COUNT; selectorChildIndex++)
            {
                GameObject newSelectorChild = Instantiate(m_selectorPrefab, transform); //Set parent to this, only for cleanliness in inspector
                MeshRenderer newSelectorMesh = newSelectorChild.GetComponent<MeshRenderer>();

                if(newSelectorMesh == null)
                {
    #if UNITY_EDITOR || DEVELOPMENT_BUILD
                    Debug.LogError(name + ": assigned selector prefab has no mesh renderer, this is required");
    #endif

                    Destroy(newSelectorChild);
                }
                else
                {
                    m_selectorsFree.Enqueue(new KeyValuePair<GameObject, MeshRenderer>(newSelectorChild, newSelectorMesh));
                }
            }
        }
    }

    /// <summary> 
    /// Update the selcted node and node group
    /// </summary>
    public void UpdateNodeSelection()
    {
        if (m_followObject == null)
            return;

        //Get selected node
        //Try get raycast target
        Node currentSelected = null;
        if (Physics.Raycast(m_followObject.transform.position, m_followObject.transform.forward, out RaycastHit hit, m_selectorDistance, CommonLayers.m_terrainMask))
        {
            currentSelected = m_inGameSceneController.m_worldController.GetClosestNode(hit.point);
        }

        if(currentSelected == null) //Found no nodes
        {
            if(m_storedNode != null)//Previously was on one, disable and update sotred variables
            {
                m_storedNode = currentSelected;
                UpdateSelection();
            }
        }
        else if(m_storedNode != currentSelected)//Moved to new node
        {
            m_storedNode = currentSelected;
            UpdateSelection();
        }
    }

    /// <summary>
    /// Update the node group
    /// Determined based off current selection type
    /// m_storedNodeGroup should include the center node
    /// </summary>
    public void UpdateSelection()
    {
        //Disable all slectors in use
        while (m_selectorsInUse.Count > 0)
        {
            KeyValuePair<GameObject, MeshRenderer> currentSelector = m_selectorsInUse.Dequeue();
            currentSelector.Value.enabled = false;
            m_selectorsFree.Enqueue(currentSelector);
        }

        if (m_storedNode == null)
        {
            m_storedNodeGroup = new Node[0];
        }
        else
        {
            transform.position = m_storedNode.m_globalPosition;

            //Get group of nodes as needed
            Vector2Int[] offsets = m_selectionDirs[m_selectionType];
            List<Node> groupNodes = new List<Node>();

            for (int offsetIndex = 0; offsetIndex < offsets.Length; offsetIndex++)
            {
                Node offsetNode = m_inGameSceneController.m_worldController.GetNodeFromOffset(m_storedNode, offsets[offsetIndex]);

                if(offsetNode!= null && m_selectorsFree.Count > 0)
                {
                    //Setup in group
                    groupNodes.Add(offsetNode);

                    //Enable/Place selectors
                    KeyValuePair<GameObject, MeshRenderer> offsetSelector = m_selectorsFree.Dequeue();

                    offsetSelector.Key.transform.position = offsetNode.m_globalPosition;
                    offsetSelector.Value.enabled = true;

                    m_selectorsInUse.Enqueue(offsetSelector);
                }
            }

            m_storedNodeGroup = groupNodes.ToArray();
        }
    }

    /// <summary>
    /// Move to the next selection type
    /// </summary>
    /// <param name="p_stepDirection">Direction to move in</param>
    public void NextSelectionType(CommonEnums.STEP_DIRECTION p_stepDirection)
    {
        //Increase in right direction
        int currentSelection = (int)m_selectionType;
        if (p_stepDirection == CommonEnums.STEP_DIRECTION.FORWARD)
            currentSelection++;
        else
            currentSelection--;

        //Clamp and assign
        if (currentSelection == (int)SELECTION_TYPE.SELECTION_COUNT)
            currentSelection = 0;
        else if (currentSelection < 0)
            currentSelection = (int)SELECTION_TYPE.SELECTION_COUNT - 1;

        m_selectionType = (SELECTION_TYPE)currentSelection;

        UpdateSelection();
    }
}
