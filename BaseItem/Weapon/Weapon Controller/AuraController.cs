using UnityEngine;

public class AuraWeaponController : WeaponController
{
    protected override void Attack()
    {
        SpawnWeapon(null); 
        currentCooldown = data.CooldownDuration;
    }
}
