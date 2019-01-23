using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InputMaster : MonoBehaviour {

    Camera m_activeCamera;
    GraphicRaycaster m_raycaster;
    EventSystem m_eventSystem;
    Player m_playerMovement;

    [SerializeField]
    InputArea m_leftStick;
    int m_leftStickTouchNum = -1;
    bool m_stillTouchingLeft;


    [SerializeField]
    InputArea m_rightStick;
    int m_rightStickTouchNum = -1;
    bool m_stillTouchingRight;

    bool m_inputOnLastFrame;

    [SerializeField]
    bool m_simulateMovementWithWASD;

    private void Awake()
    {
        m_raycaster = GetComponent<GraphicRaycaster>();
    }

    private void Start()
    {
        m_playerMovement = Player.Instance;
        m_activeCamera = Camera.main;
        m_eventSystem = EventSystem.current;
    }

    private void Update()
    {
        //In case some decides to build to iphone or android for testing
#if UNITY_IOS || UNITY_ANDROID
        Debug.Log(Input.touchCount);
        m_stillTouchingLeft = false;
        m_stillTouchingRight = false;

        foreach (Touch touch in Input.touches)
        {
            if ((m_leftStickTouchNum == -1 || touch.fingerId == m_leftStickTouchNum) ||
                (m_rightStickTouchNum == -1 || touch.fingerId == m_rightStickTouchNum))
            {
                GetInput(touch);
            }
        }
        if (!m_stillTouchingLeft)
            m_leftStick.HasTakenInput = false;
        if (!m_stillTouchingRight)
            m_rightStick.HasTakenInput = false;

        if (!m_leftStick.HasTakenInput)
        {
            //If there was no input on the last frame, the left stick
            m_leftStick.ResetInput();
            m_leftStickTouchNum = -1;
        }
        
        if(!m_rightStick.HasTakenInput)
        {
            //If there was no input on the last frame, the left stick
            m_rightStick.ResetInput();
            m_rightStickTouchNum = -1;
        }
#else
        if (Input.GetMouseButton(0))
        {
            GetInput();
            m_inputOnLastFrame = true;
        }
        else if (m_inputOnLastFrame)
        {
            //If there was no input on the last frame, reset both sticks
            m_leftStick.ResetInput();
            m_leftStickTouchNum = -1;
            m_rightStick.ResetInput();
            m_rightStickTouchNum = -1;

            m_inputOnLastFrame = false;
        }


        //if we're using WASD
        if(m_simulateMovementWithWASD)
        {
            Vector3 WASDMove = Vector3.zero;
            bool detectedXInput = true;
            bool detectedYInput = true;

            //Don't allow the player to move in opposite directions
            if (Input.GetKey(KeyCode.A))
                WASDMove.x = -5f;
            else if (Input.GetKey(KeyCode.D))
                WASDMove.x = 5f;
            else
                detectedXInput = false;

            if (Input.GetKey(KeyCode.W))
                WASDMove.z = 5f;
            else if (Input.GetKey(KeyCode.S))
                WASDMove.z = -5f;
            else
                detectedYInput = false;

            //If we haven't detected input, don't give the player any
            if(detectedXInput || detectedYInput)
                m_playerMovement.MovementDirection = WASDMove * 5f;
        }
#endif
    }

    public void GetInput(Touch touch)
    {
        PointerEventData pointerData = new PointerEventData(null);
        pointerData.position = touch.position;

        List<RaycastResult> rayResults = new List<RaycastResult>();

        m_raycaster.Raycast(pointerData, rayResults);

        foreach (RaycastResult rr in rayResults)
        {

            //If we're touching the left stick and that the touch isn't already linked to the right stick...
            if (rr.gameObject.transform.position == m_leftStick.gameObject.transform.position && 
                touch.fingerId != m_rightStickTouchNum)
            {
                //If we've previously gotten input, it doesn't matter if we're no longer inside the stick area
                if (m_leftStick.HasTakenInput || (Vector2)m_leftStick.RestrictArea.ClosestPoint(touch.position) == touch.position)
                {
                    //Assign the finger id to the left stick
                    m_leftStickTouchNum = touch.fingerId;

                    m_leftStick.HasTakenInput = true;
                    m_stillTouchingLeft = true;

                    m_playerMovement.MovementDirection = m_leftStick.HandleInput(touch.position);
                }
            }

            //Same as the left stick... only for the right stick
            if (rr.gameObject.transform.position == m_rightStick.gameObject.transform.position &&
                touch.fingerId != m_leftStickTouchNum)
            {
                if (m_rightStick.HasTakenInput || (Vector2)m_rightStick.RestrictArea.ClosestPoint(touch.position) == touch.position)
                {
                    m_rightStickTouchNum = touch.fingerId;

                    m_rightStick.HasTakenInput = true;
                    m_stillTouchingRight = true;

                    m_playerMovement.LookDirection = m_rightStick.HandleInput(touch.position);
                }
            }
            

        }

    }

    public void GetInput()
    {
        PointerEventData pointerData = new PointerEventData(null);
        pointerData.position = Input.mousePosition;

        List<RaycastResult> rayResults = new List<RaycastResult>();

        m_raycaster.Raycast(pointerData, rayResults);

        foreach (RaycastResult rr in rayResults)
        {
            //If we're not using the WASD keys
            if (!m_simulateMovementWithWASD)
            {
                //If the object we're touching is the left stick and the right stick isn't already getting input from the mouse
                if (rr.gameObject.transform.position == m_leftStick.gameObject.transform.position && m_rightStickTouchNum != 0)
                {
                    //If we've already taken input previously, or our mouse is over the leftstick's restrict area
                    if (m_leftStick.HasTakenInput || m_leftStick.RestrictArea.ClosestPoint(Input.mousePosition) == Input.mousePosition)
                    {
                        //Get input

                        //Assign the left stick to the mouse
                        m_leftStickTouchNum = 0;
                        m_leftStick.HasTakenInput = true;
                        m_playerMovement.MovementDirection = m_leftStick.HandleInput(Input.mousePosition);
                    }
                    else
                        m_leftStick.HasTakenInput = false;
                }
            }

            //Same as above, except for the right stick
            if (rr.gameObject.transform.position == m_rightStick.gameObject.transform.position && m_leftStickTouchNum != 0)
            {
                if (m_rightStick.HasTakenInput || m_rightStick.RestrictArea.ClosestPoint(Input.mousePosition) == Input.mousePosition)
                {
                    m_rightStickTouchNum = 0;
                    m_rightStick.HasTakenInput = true;
                    m_playerMovement.LookDirection = m_rightStick.HandleInput(Input.mousePosition);
                }
                else
                    m_rightStick.HasTakenInput = false;
            }
        }
    }
}
