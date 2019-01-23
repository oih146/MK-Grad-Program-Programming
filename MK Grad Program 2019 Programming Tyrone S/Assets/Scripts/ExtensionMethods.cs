using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{

    public static PlayerWeaponType GetRandomWeaponType(this PlayerWeaponType weapType)
    {
        return (PlayerWeaponType)Random.Range(0, (int)PlayerWeaponType.NumOfWeapons);
    }

    public static bool IsAmmoUsingWeapon(this PlayerWeaponType weapType)
    {
        return weapType == PlayerWeaponType.MachineGun || weapType == PlayerWeaponType.Pistol ||
            weapType == PlayerWeaponType.RocketLauncher;
    }

    public static Vector3 RandomPointInBox(this BoxCollider box)
    {
        return new Vector3(
            Random.Range(box.bounds.min.x, box.bounds.max.x),
            Random.Range(box.bounds.min.y, box.bounds.max.y),
            Random.Range(box.bounds.min.z, box.bounds.max.z));
    }

}
