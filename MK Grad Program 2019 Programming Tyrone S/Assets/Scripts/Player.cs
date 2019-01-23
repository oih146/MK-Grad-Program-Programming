using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public static Player Instance { get; set; }

    ChildMe m_cameraChild;
    Rigidbody m_rigidbody;

    #region Movement Variables

    bool m_takingMovement;
    Vector3 m_movementDirection;
    public Vector3 MovementDirection
    {
        get { return m_movementDirection; }
        set {
            m_movementDirection = value;
            m_takingMovement = true;
        }
    }

    bool m_takingLook;
    Vector3 m_lookDirection;
    public Vector3 LookDirection
    {
        get { return m_lookDirection; }
        set {
            m_lookDirection = value;
            m_takingLook = true;
        }
    }

    [SerializeField]
    float m_movementSpeed;

    #endregion

    WeaponBase m_currentWeapon;
    public WeaponBase CurrentWeapon { get { return m_currentWeapon; } }

    List<WeaponBase> m_allweapons;

    private void Awake()
    {
        Instance = this;
        m_rigidbody = GetComponent<Rigidbody>();
    }

    // Use this for initialization
    void Start () {
        m_cameraChild = GetComponent<ChildMe>();
        m_allweapons = new List<WeaponBase>(GetComponentsInChildren<WeaponBase>(true));
	}
	
	// Update is called once per frame
	void Update () {

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        //If we're scrolling
        if (Mathf.Abs(scroll) >= 0.1f)
        {
            //Find out which way we're scrolling
            bool ascendingWeapList = scroll > 0 ? true : false;
            //Get a mutible version of our weapon type, if currentWeapon doesn't equal null
            //If it does, it most likely means we have no weapon
            int currentWeaponType = m_currentWeapon ? (int)CurrentWeapon.WeaponType : (int)PlayerWeaponType.None;
                
            //Go up one in the weapons list
            if (ascendingWeapList)
                currentWeaponType++;
            //Go down one in the weapons list
            else
                currentWeaponType--;

            //If we've gone to an invaild index, wrap around
            if (currentWeaponType == -1)
                currentWeaponType = (int)PlayerWeaponType.NumOfWeapons - 1;
            else if (currentWeaponType == (int)PlayerWeaponType.NumOfWeapons)
                currentWeaponType = 0;

            //If we're now selecting no weapon...
            if (currentWeaponType == (int)PlayerWeaponType.None)
            {
                //Set our weapon to null and then stop here
                SetCurrentWeapon(PlayerWeaponType.None);
                return;
            }

            //Flip through our weapons 
            foreach(WeaponBase wb in m_allweapons)
            {
                //Select the matching weapon type
                if (wb.WeaponType == (PlayerWeaponType)currentWeaponType && wb.HasWeapon)
                {
                    //Set it as our weapon
                    SetCurrentWeapon((PlayerWeaponType)currentWeaponType);
                    break;
                }
            }
        }
	}

    private void FixedUpdate()
    {
        m_cameraChild.SetPosition();

        //Only change our velocity if we detect input from the left stick
        if (m_takingMovement)
        {
            m_rigidbody.velocity = m_movementDirection * m_movementSpeed;
            //If there's no input from the right stick
            if (!m_takingLook)
                //Face the direction we're moving
                gameObject.transform.rotation = Quaternion.LookRotation(m_movementDirection);

        }
        else
        {
            //If we're not moving, slow our velocity 
            m_rigidbody.velocity *= 0.9f;
        }

        m_takingMovement = false;

        //Only change our rotation if we detect input from the right stick
        if(m_takingLook)
        {
            gameObject.transform.rotation = Quaternion.LookRotation(m_lookDirection);
            //If we have a weapon (currentWeapon != null), use it
            if(m_currentWeapon)
                m_currentWeapon.UseWeapon();
        }

        m_takingLook = false;
    }

    public void SetCurrentWeapon(PlayerWeaponType weaponType)
    {
        if(weaponType == PlayerWeaponType.None)
        {
            if (m_currentWeapon)
                m_currentWeapon.gameObject.SetActive(false);
            m_currentWeapon = null;
            return;
        }

        WeaponBase wb = GetWeapon(weaponType);
        if (wb)
        {
            if (m_currentWeapon)
                m_currentWeapon.gameObject.SetActive(false);
            m_currentWeapon = wb;
            m_currentWeapon.gameObject.SetActive(true);
            m_currentWeapon.OnEquip();
        }
    }

    public WeaponBase GetWeapon(PlayerWeaponType weaponType)
    {
        if(weaponType == PlayerWeaponType.None)
        {
            Debug.LogError("Searching for WeaponType.None.");
            return null;
        }

        foreach (WeaponBase wb in m_allweapons)
        {
            if (wb.WeaponType == weaponType)
            {
                return wb;
            }
        }

        Debug.LogError("Unknown weapon: " + weaponType.ToString());
        return null;
    }
}
