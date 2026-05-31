using UnityEngine;

public class PlayerGhostTrail : MonoBehaviour
{
    [Header("Settings")]
    public GameObject ghostPrefab; 
    public float ghostInterval = 0.05f; 
    private float ghostTimer;

    [Header("References")]
    private PlayerController playerMove;
    private SpriteRenderer playerSR;

    void Start()
    {
        playerMove = GetComponent<PlayerController>();
        playerSR = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (playerMove.isDashing)
        {
            if (ghostTimer <= 0)
            {
                SpawnGhost();
                ghostTimer = ghostInterval;
            }
            else
            {
                ghostTimer -= Time.deltaTime;
            }
        }
    }

    void SpawnGhost()
    {
        GameObject ghost = Instantiate(ghostPrefab);
        GhostEffect effect = ghost.GetComponent<GhostEffect>();

        effect.Init(playerSR.sprite, playerSR.flipX, transform.position, transform.rotation);
    }
}