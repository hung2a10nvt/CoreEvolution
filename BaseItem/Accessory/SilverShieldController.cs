using UnityEngine;

public class SilverShieldController : AccessoryController
{
    private float cooldownTimer = 0f;
    private float immunityDuration = 3f;
    private float cooldownDuration = 50f;

    private void Update()
    {
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
            return; 
        }

        if (player.CurrentHealth <= player.CurrentMaxHealth * 0.25f && player.CurrentHealth > 0)
        {
            ActivateShield();
        }
    }

    private void ActivateShield()
    {
        player.AddImmunity(immunityDuration);
        cooldownTimer = cooldownDuration; 
        Debug.Log("SILVER SHIELD");
    }
}
