using UnityEngine;

public class MeleeController : WeaponController
{
    private WeaponBehavior _permanentMelee;

    protected override void Start()
    {
        base.Start(); 

        if (data != null && data.Prefab != null)
        {
            _permanentMelee = SpawnWeapon(null);
        }
    }

    protected override void Update() { }
    protected override void Attack() { }
}