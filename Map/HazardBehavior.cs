using UnityEngine;

public class HazardBehavior : MonoBehaviour
{
    [Header("Hazard Settings")]
    [SerializeField] private float damageAmount = 10f;
    [SerializeField] private float damageInterval = 1f; 

    private float nextDamageTime = 0f;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (Time.time >= nextDamageTime)
            {
                if (PlayerStats.Instance != null && !PlayerStats.Instance.IsImmune)
                {
                    PlayerStats.Instance.TakeDamage(damageAmount);

                    nextDamageTime = Time.time + damageInterval;
                }
            }
        }
    }
}