using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour {

    [SerializeField]
    PlayerWeaponType m_weaponType;
    public PlayerWeaponType WeaponType { get { return m_weaponType; } }

    [SerializeField]
    [Tooltip("If true, player has this weapon available to them")]
    bool m_hasWeapon;
    public bool HasWeapon { get { return m_hasWeapon; } set { m_hasWeapon = value; } }

    [SerializeField]
    protected float m_shotInterval;
    protected float m_shotIntervalTimer;

    private void Awake()
    {
        VariableSetup();
    }

    protected virtual void VariableSetup()
    {
        m_shotIntervalTimer = m_shotInterval;
    }

    public abstract void OnEquip();

    public virtual void UseWeapon()
    {
        m_shotIntervalTimer -= Time.deltaTime;

        if (m_shotIntervalTimer <= 0)
        {
            //Swing Weapon
            
            m_shotIntervalTimer = m_shotInterval;
        }
    }
}
