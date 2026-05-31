using UnityEngine;

public class GhostEffect : MonoBehaviour
{
    private SpriteRenderer sr;
    private float alpha = 1f;
    [SerializeField] private float fadeSpeed = 3f; 

    public void Init(Sprite currentSprite, bool isFlipped, Vector3 pos, Quaternion rot)
    {
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = currentSprite;
        sr.flipX = isFlipped;
        transform.position = pos;
        transform.rotation = rot;

        sr.color = new Color(0f, 0.8f, 1f, 0.5f);
    }

    void Update()
    {
        alpha -= fadeSpeed * Time.deltaTime;
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, alpha);

        if (alpha <= 0)
        {
            Destroy(gameObject);
        }
    }
}