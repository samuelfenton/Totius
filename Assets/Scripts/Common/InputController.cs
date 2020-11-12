using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    public float m_mouseXSensitivity = 30.0f;
    public float m_mouseYSensitivity = 30.0f;

    public bool m_inverted = false;

    public float m_scrollDelay = 0.1f;
    private bool m_canScrollFlag = false;
    private int m_scrollVal = 0;

    public enum INPUT_KEY {SPRINT, DASH, BLOCK, LIGHT_ATTACK, HEAVY_ATTACK, INTERACT, MENU, KEY_COUNT };
    
    public enum INPUT_STATE { UP, DOWN, DOWNED };

    public enum INPUT_AXIS {FORWARD, HORIZONTAL, VERTICAL, LOOK_HORIZONTAL, LOOK_VERTICAL, AXIS_COUNT };

    //[HideInInspector]
    private INPUT_STATE[] m_keyVal = new INPUT_STATE[(int)INPUT_KEY.KEY_COUNT];
    //[HideInInspector]
    private float[] m_axisVal = new float[(int)INPUT_AXIS.AXIS_COUNT];

    private InputAction_Gameplay m_intput = null;

    private void Awake()
    {
        for (int i = 0; i < (int)INPUT_KEY.KEY_COUNT; i++)
        {
            m_keyVal[i] = INPUT_STATE.UP;
        }

        for (int i = 0; i < (int)INPUT_AXIS.AXIS_COUNT; i++)
        {
            m_axisVal[i] = 0.0f;
        }

        m_intput = new InputAction_Gameplay();

        m_canScrollFlag = true;
    }

    private void OnEnable()
    {
        m_intput.Player.Forward.Enable();
        m_intput.Player.Horizontal.Enable();
        m_intput.Player.Vertical.Enable();

        m_intput.Player.LookHorizontal.Enable();
        m_intput.Player.LookVertical.Enable();

        m_intput.Player.Scroll.Enable();

        m_intput.Player.Sprint.Enable();
        m_intput.Player.Block.Enable();
        m_intput.Player.LightAttack.Enable();
        m_intput.Player.HeavyAttack.Enable();
        m_intput.Player.Interact.Enable();
        m_intput.Player.Menu.Enable();
    }

    private void OnDisable()
    {
        m_intput.Player.Forward.Disable();
        m_intput.Player.Horizontal.Disable();
        m_intput.Player.Vertical.Disable();

        m_intput.Player.LookHorizontal.Disable();
        m_intput.Player.LookVertical.Disable();

        m_intput.Player.Scroll.Disable();

        m_intput.Player.Sprint.Disable();
        m_intput.Player.Block.Disable();
        m_intput.Player.LightAttack.Disable();
        m_intput.Player.HeavyAttack.Disable();
        m_intput.Player.Interact.Disable();
        m_intput.Player.Menu.Disable();
    }


    /// <summary>
    /// Update the inputs
    /// </summary>
    public void UpdateInput()
    {
        m_axisVal[(int)INPUT_AXIS.FORWARD] = m_intput.Player.Forward.ReadValue<float>();
        m_axisVal[(int)INPUT_AXIS.HORIZONTAL] = m_intput.Player.Horizontal.ReadValue<float>();
        m_axisVal[(int)INPUT_AXIS.VERTICAL] = m_intput.Player.Vertical.ReadValue<float>();

        m_axisVal[(int)INPUT_AXIS.LOOK_HORIZONTAL] = m_intput.Player.LookHorizontal.ReadValue<float>();
        m_axisVal[(int)INPUT_AXIS.LOOK_VERTICAL] = m_intput.Player.LookVertical.ReadValue<float>();

        m_keyVal[(int)INPUT_KEY.SPRINT] = DetermineInputState(m_intput.Player.Sprint.triggered, m_intput.Player.Sprint.ReadValue<float>());
        m_keyVal[(int)INPUT_KEY.BLOCK] = DetermineInputState(m_intput.Player.Block.triggered, m_intput.Player.Block.ReadValue<float>());
        m_keyVal[(int)INPUT_KEY.LIGHT_ATTACK] = DetermineInputState(m_intput.Player.LightAttack.triggered, m_intput.Player.LightAttack.ReadValue<float>());
        m_keyVal[(int)INPUT_KEY.HEAVY_ATTACK] = DetermineInputState(m_intput.Player.HeavyAttack.triggered, m_intput.Player.HeavyAttack.ReadValue<float>());
        m_keyVal[(int)INPUT_KEY.INTERACT] = DetermineInputState(m_intput.Player.Interact.triggered, m_intput.Player.Interact.ReadValue<float>());
        m_keyVal[(int)INPUT_KEY.MENU] = DetermineInputState(m_intput.Player.Menu.triggered, m_intput.Player.Menu.ReadValue<float>());

        UpdateScroll();
    }

    //Reset each input to Keystate up, and set axis values to 0.0f
    public void ResetInput()
    {
        for (int i = 0; i < (int)INPUT_KEY.KEY_COUNT; i++)
        {
            m_keyVal[i] = INPUT_STATE.UP;
        }

        for (int i = 0; i < (int)INPUT_AXIS.AXIS_COUNT; i++)
        {
            m_axisVal[i] = 0.0f;
        }
    }

    /// <summary>
    /// Determine the next state for a given key
    /// If trigger frame, always DOWNED
    /// If postive value and not trigger, then DOWN
    /// Default UP
    /// </summary>
    /// <param name="p_isTriggerFrame">All inputs will have trigger when input is in the same frame</param>
    /// <param name="p_currentVal">Current value for an input</param>
    /// <returns>Current input state based off above rules</returns>
    private INPUT_STATE DetermineInputState(bool p_isTriggerFrame, float p_currentVal)
    {
        if (p_isTriggerFrame)
            return INPUT_STATE.DOWNED;
        if (p_currentVal > 0.0f)
            return INPUT_STATE.DOWN;
        return INPUT_STATE.UP;
    }

    /// <summary>
    /// Simplify key state to a boolean
    /// </summary>
    /// <param name="p_input">Key to test against</param>
    /// <returns>true when down or downed</returns>
    public bool GetKeyBool(INPUT_KEY p_input)
    {
        if (p_input < INPUT_KEY.KEY_COUNT)
            return m_keyVal[(int)p_input] == INPUT_STATE.DOWN || m_keyVal[(int)p_input] == INPUT_STATE.DOWNED;
        return false;
    }

    /// <summary>
    /// Get Key state
    /// </summary>
    /// <param name="p_input">Axis to test against</param>
    /// <returns>Current state of key</returns>
    public INPUT_STATE GetKey(INPUT_KEY p_input)
    {
        if (p_input < INPUT_KEY.KEY_COUNT)
            return m_keyVal[(int)p_input];
        return INPUT_STATE.UP;
    }

    /// <summary>
    /// Simplyfy axis value to a boolean
    /// </summary>
    /// <param name="p_input">Axis to test against</param>
    /// <returns>true when axis value isnt 0.0f</returns>
    public bool GetAxisBool(INPUT_AXIS p_input)
    {
        if (p_input < INPUT_AXIS.AXIS_COUNT)
            return m_axisVal[(int)p_input] != 0.0f;
        return false;
    }

    /// <summary>
    /// Get Axis value
    /// </summary>
    /// <param name="p_input">Axis to test against</param>
    /// <returns>Value of Axis</returns>
    public float GetAxis(INPUT_AXIS p_input)
    {
        if(p_input == INPUT_AXIS.LOOK_VERTICAL)
        {
            if (m_inverted)
                return m_axisVal[(int)p_input] * -1;
            return m_axisVal[(int)p_input];
        }
        if (p_input < INPUT_AXIS.AXIS_COUNT)
            return m_axisVal[(int)p_input];
        return 0.0f;
    }

    /// <summary>
    /// Get zooming click
    /// This contains delays within
    /// </summary>
    /// <returns></returns>
    private void UpdateScroll()
    {
        float scrollVal = m_intput.Player.Scroll.ReadValue<float>();

        if (m_canScrollFlag && scrollVal != 0.0f) //Able to scroll
        {
            m_scrollVal = scrollVal >= 0.0f ? 1 : -1;

            m_canScrollFlag = false;

            StartCoroutine(ResetCanScrollFlag());
        }
        else //Unable to scorll, set default value
        {
            m_scrollVal = 0;
        }
    }

    private IEnumerator ResetCanScrollFlag()
    {
        yield return new WaitForSeconds(m_scrollDelay);

        m_canScrollFlag = true;
    }

    /// <summary>
    /// Get the current scroll value
    /// </summary>
    /// <returns>Scroll value</returns>
    public int GetScroll()
    {
        return m_scrollVal;
    }

    /// <summary>
    /// Get movement for all 3 dirs in a single vector3
    /// </summary>
    /// <returns>Vector3, x = horizontal, y = vertical, z = forward</returns>
    public Vector3 GetMovement()
    {
        return new Vector3(GetAxis(INPUT_AXIS.HORIZONTAL), GetAxis(INPUT_AXIS.VERTICAL), GetAxis(INPUT_AXIS.FORWARD));
    }

    /// <summary>
    /// Checks for any input that might be used
    /// All excluding and menu
    /// </summary>
    /// <returns>If any value is positive</returns>
    public bool AnyInput()
    {
        return GetAxisBool(INPUT_AXIS.FORWARD)  || GetAxisBool(INPUT_AXIS.HORIZONTAL) || GetAxisBool(INPUT_AXIS.VERTICAL) || 
            GetKeyBool(INPUT_KEY.SPRINT) || GetKeyBool(INPUT_KEY.DASH) || GetKeyBool(INPUT_KEY.BLOCK) || 
            GetKeyBool(INPUT_KEY.LIGHT_ATTACK) || GetKeyBool(INPUT_KEY.HEAVY_ATTACK) || GetKeyBool(INPUT_KEY.INTERACT);
    }
}
