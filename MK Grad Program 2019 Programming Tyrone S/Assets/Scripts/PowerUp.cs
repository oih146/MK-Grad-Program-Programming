using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour {

    [SerializeField]
    float m_respawnTime;
    float m_timer;

    [SerializeField]
    PlayerWeaponType m_weaponToGive;

    Collider m_collider;
    MeshRenderer m_mesh;

    private void Start()
    {
        m_mesh = GetComponent<MeshRenderer>();
        m_collider = GetComponent<Collider>();
        enabled = false;
    }

    //Could create another object to keep track of this
    //Needs to be a bit more specialised
    private void Update()
    {
        m_timer -= Time.deltaTime;
        if(m_timer <= 0)
        {
            m_mesh.enabled = true;
            m_collider.enabled = true;
            enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //If the player runs into us
        if(other.tag == "Player")
        {
            Player player = other.GetComponent<Player>();

            //Unlock the specified weapon for them
            player.GetWeapon(m_weaponToGive).HasWeapon = true;

            //If it uses ammo, refill their ammo
            if(m_weaponToGive.IsAmmoUsingWeapon())
            {
                AmmoWeapon ammoWeap = (AmmoWeapon)player.GetWeapon(m_weaponToGive);
                ammoWeap.RestockAmmo();
            }

            //Set it as their current weapon, only if they currently don't have a weapon or their current weapon has no ammo
            if (player.CurrentWeapon == null ||
                (player.CurrentWeapon.WeaponType.IsAmmoUsingWeapon() && ((AmmoWeapon)player.CurrentWeapon).HasAmmo)
                )
            {
                player.SetCurrentWeapon(m_weaponToGive);
            }

            //Despawn the powerup and start the timer
            m_timer = m_respawnTime;
            enabled = true;
            m_mesh.enabled = false;
            m_collider.enabled = false;
        }
    }
}
