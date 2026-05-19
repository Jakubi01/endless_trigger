using UnityEngine;

namespace Items.Weapon
{
    public class Sniper : WeaponBase
    {
        protected override void Fire()
        {
            Debug.Log("Sniper Fired");
        }
    }
}