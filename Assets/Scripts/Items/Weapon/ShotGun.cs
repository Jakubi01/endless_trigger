using UnityEngine;

namespace Items.Weapon
{
    public class ShotGun : WeaponBase
    {
        protected override void Fire()
        {
            Debug.Log("Shotgun Fired");
        }
    }
}