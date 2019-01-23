using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoWeapon : WeaponBase {

    public bool HasAmmo
    {
        get
        {
            return m_clipAmmo > 0 || m_totalAmmo > 0;
        }
    }

    [SerializeField]
    BulletPool m_objectPool;

    [SerializeField]
    Transform m_fireAid;

    [SerializeField]
    int m_clipCapacity;
    int m_clipAmmo;
    [SerializeField]
    int m_ammoCapacity;
    int m_totalAmmo;

    [SerializeField]
    float m_fireVelocity;

    [SerializeField]
    protected float m_reloadTime;
    protected float m_timerReload;
    bool m_reloading;

    float m_lastCountOfTime;

    protected override void VariableSetup()
    {
        base.VariableSetup();
        m_timerReload = m_reloadTime;
        RestockAmmo();
        Reload();
    }

    public override void OnEquip()
    {
        if ((m_reloading && m_timerReload - (Time.time - m_lastCountOfTime) >= 0) || !m_reloading)
            return;
        Reload();
    }

    public override void UseWeapon()
    {
        //If we're not reloading
        if (!m_reloading)
        {
            //Reduce the shot interval timer
            m_shotIntervalTimer -= Time.time - m_lastCountOfTime;
            //Switching weapons stalls the reload timer, remembering the time we allows us to maintain
            //reload time

            //If the shot interval timer reaches 0 (or less) and we have ammo in our clip
            if (m_shotIntervalTimer <= 0 && m_clipAmmo > 0)
            {
                //Fire!
                Projectile proj = m_objectPool.GetFreeObject();
                proj.gameObject.SetActive(true);
                proj.gameObject.transform.position = m_fireAid.position;

                //Mostly for non-symmetrical objects, make them face the right way 
                proj.gameObject.transform.rotation = Quaternion.LookRotation(m_fireAid.forward);
                proj.GetComponent<Rigidbody>().velocity = m_fireAid.forward * m_fireVelocity;

                //Reduce the clip ammo
                m_clipAmmo--;
                //If we've run out of ammo, and have ammo left, start reloading
                if (m_clipAmmo == 0 && m_totalAmmo > 0)
                    m_reloading = true;
                //If we don't have ammo left
                else if (m_totalAmmo <= 0 && m_clipAmmo == 0)
                {
                    //Get rid of the gun? Make clicking sounds?
                }
                m_shotIntervalTimer = m_shotInterval;
            }
        } else
        {
            m_timerReload -= Time.time - m_lastCountOfTime;
            if(m_timerReload <= 0)
            {
                //Reload!
                m_reloading = false;
                Reload();

                m_timerReload = m_reloadTime;
            }
        }
        m_lastCountOfTime = Time.time;
    }

    protected void Reload()
    {
        //Find out how much ammo we have left in the clip first
        int amountToGive = m_clipCapacity - m_clipAmmo;
        //If have 'that' amount of more...
        if (m_totalAmmo >= amountToGive)
        {
            //Take it straight from the ammo count...
            m_totalAmmo -= amountToGive;
            //And give it to the clip
            m_clipAmmo += amountToGive;
        }
        //If we don't have enough ammo to give
        else
        {
            //Give what we have
            m_clipAmmo += m_totalAmmo;
            m_totalAmmo = 0;
        }
    }

    public void RestockAmmo()
    {
        m_totalAmmo = m_ammoCapacity;
    }

}
