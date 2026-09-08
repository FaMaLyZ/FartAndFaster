using UnityEngine;

public class SpinDummy : DummyPeople
{
    [SerializeField] private float spinSpeed = 440f;

    protected override void RotateTowards(Vector3 targetPosition)
    {
        transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime, Space.World);
    }
}
