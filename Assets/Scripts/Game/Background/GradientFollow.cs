using UnityEngine;

public class GradientFollow : MonoBehaviour
{
    public Transform target;
    public float xOffset = 0;
    private Vector3 newPosition = new Vector3();
    private float y = 0, z = 0;

    void Start() {
        y = transform.position.y;
        z = transform.position.z;
    }

    void FixedUpdate() {
        newPosition.Set(target.position.x + xOffset, y, z);
        transform.position = newPosition;
    }
}
