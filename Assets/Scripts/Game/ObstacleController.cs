using UnityEngine;

public class ObstacleController : MonoBehaviour {
    private Rigidbody2D rb;

    void Start() {
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    void Update() {
        if(rb.bodyType != RigidbodyType2D.Static && rb.velocity.y >= 0) {
            rb.velocity = new Vector2(0, 0);
            rb.bodyType = RigidbodyType2D.Static;
        }
    }
}
