using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Flyover_Camera : Camera_Entity
{
    #region Movement Variables
    [Header("Movement Variables")]
    [Tooltip("m/s")]
    public float m_forwardSpeed = 10.0f;
    [Tooltip("m/s")]
    public float m_rightSpeed = 10.0f;
    [Tooltip("m/s")]
    public float m_upSpeed = 10.0f;
    [Tooltip("Modifies the previous speed by this amount")]
    public float m_sprintModifierSpeed = 4.0f;
    [Header("Camera Rot")]
    [Tooltip("degrees/s")]
    public float m_rotationSpeed = 90.0f;
    [Tooltip("Camera looking angle in degrees, up and down")]
    [Range(0.0f,90.0f)]
    public float m_maxCameraAngle = 80.0f;
    //Cumulative translation/rot changes

    private Rigidbody m_rigidBody = null;

    private Vector3 m_cumulativeTranslation = Vector3.zero;
    private Vector2 m_cumulativeRotation = Vector2.zero;
    #endregion

    #region Cell Traversal Variables
    [Header("Cell Traversal Variables")]
    [Tooltip("How far past the edge till traversal is considered, 0.0 means on edge, 1.0 means the next cells edge")]
    [Range(0.0f, 1.0f)]
    public float m_traversalRange = 0.5f;
    private Vector2Int m_previousCell = Vector2Int.zero;
    #endregion

    #region Stored Variables
    private InGame_SceneController m_inGameSceneController = null;
    private WorldController m_worldController = null;
    #endregion  

    /// <summary>
    /// Initialise the entity
    /// Note: Dont use start/awake on entities, this ensures correct load order
    /// </summary>
    public override void InitEntity()
    {
        base.InitEntity();

        m_rigidBody = GetComponent<Rigidbody>();

        m_inGameSceneController = (InGame_SceneController)MasterController.Instance.m_sceneController;
        m_worldController = m_inGameSceneController.m_worldController;
    }

    /// <summary>
    /// Update the entity, should be called from a scene controller during update
    /// Note: Dont use update on entities, this ensures when pausing game etc functions arent called
    /// </summary>
    public override void UpdateEntity()
    {
        base.UpdateEntity();

        UpdateCellTraversal();

        UpdateCumulativeInput();
    }

    /// <summary>
    /// Fixed update the entity, should be called from a scene controller during fixed update
    /// Note: Dont use fixed update on entities, this ensures when pausing game etc functions arent called
    /// </summary>
    public override void FixedUpdateEntity()
    {
        base.FixedUpdateEntity();

        ApplyCumulativeInput();
    }

    /// <summary>
    /// Get the culmative input from the player
    /// Done during update, where as apply transforms is done in fixed update
    /// All addtionas should be pre modified by transform and delta time
    /// </summary>
    private void UpdateCumulativeInput()
    {
        //Get translation
        Vector3 localMovement = MasterController.Instance.m_input.GetMovement();

        //Apply speed and time modifiers
        float sprintModifier = MasterController.Instance.m_input.GetKeyBool(InputController.INPUT_KEY.SPRINT) ? m_sprintModifierSpeed : 1.0f;

        localMovement.x *= m_rightSpeed * sprintModifier * Time.deltaTime;
        localMovement.y *= m_upSpeed * sprintModifier * Time.deltaTime;
        localMovement.z *= m_forwardSpeed * sprintModifier * Time.deltaTime;

        Vector3 globalMovement = transform.localToWorldMatrix * localMovement;

        m_cumulativeTranslation += globalMovement;

        //Get rotation
        m_cumulativeRotation.x += MasterController.Instance.m_input.GetAxis(InputController.INPUT_AXIS.LOOK_HORIZONTAL) * m_rotationSpeed * Time.deltaTime;
        m_cumulativeRotation.y -= MasterController.Instance.m_input.GetAxis(InputController.INPUT_AXIS.LOOK_VERTICAL) * m_rotationSpeed * Time.deltaTime; //Defualt using minus to fix incorrect inversion, then use inverted check within equation
    }

    /// <summary>
    /// Apply given cumulative input
    /// </summary>
    private void ApplyCumulativeInput()
    {
        //Apply cumulative translation
        m_rigidBody.MovePosition(transform.position + m_cumulativeTranslation);
        m_cumulativeTranslation = Vector3.zero;

        //Apply cumulative rotation
        Vector3 localRot = MOARMaths.GetSignedEulerAngles(transform.localRotation.eulerAngles);
        //Yaw - Left Right, infinite roation allowed
        localRot.y += m_cumulativeRotation.x;
        //Pitch - Up Down, clamped into -m_maxCameraAngle, m_maxCameraAngle
        localRot.x = Mathf.Clamp(localRot.x + m_cumulativeRotation.y, -m_maxCameraAngle, m_maxCameraAngle);
        transform.localRotation = Quaternion.Euler(localRot);

        m_cumulativeRotation = Vector2.zero;
    }

    /// <summary>
    /// Called every frame
    /// Update what cell we are in as needed
    /// </summary>
    private void UpdateCellTraversal()
    {
        if (!IsReadyForCellTraversal())
            return;

        Vector2Int currentCell = m_worldController.DetermineCell(transform.position);

        if(currentCell != m_previousCell) //Ensure moved to new cell
        {
            m_worldController.EnteredNewCell(currentCell);
            m_previousCell = currentCell;
        }
    }

    /// <summary>
    /// Have we moved far enough to consider cell traversal
    /// </summary>
    /// <returns></returns>
    private bool IsReadyForCellTraversal()
    {
        Vector2 previousCellCenter = Cell.GetCellCenter(m_previousCell);

        Vector2 distanceFromCenter = new Vector2(Mathf.Abs(transform.position.x - previousCellCenter.x), Mathf.Abs(transform.position.z - previousCellCenter.y));

        float largestDistance = distanceFromCenter.x > distanceFromCenter.y ? distanceFromCenter.x : distanceFromCenter.y;

        return largestDistance >= Cell.CELL_SIZE_HALF + Cell.CELL_SIZE_HALF * m_traversalRange; 
    }
}
