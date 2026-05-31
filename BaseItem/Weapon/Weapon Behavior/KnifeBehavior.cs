using UnityEngine;

public class KnifeBehavior : ProjectileBehavior
{
    [SerializeField] private float rotationSpeed = 600f;

    protected override void Update()
    {
        base.Update();
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }
}