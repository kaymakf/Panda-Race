using UnityEngine;

public class CharacterController : MonoBehaviour {
    public float moveSpeed = 5f;
    public float jumpVelocity = 10f;

    [SerializeField] private LayerMask platformsLayerMask;
    private Rigidbody2D rigidbody2d;
    private BoxCollider2D boxCollider2d;

    private void Awake() {
        rigidbody2d = transform.GetComponent<Rigidbody2D>();
        boxCollider2d = transform.GetComponent<BoxCollider2D>();
    }

    private void Update() {
        if (IsGrounded() && Input.GetAxisRaw("Vertical") > 0)
            rigidbody2d.velocity = Vector2.up * jumpVelocity;

        HandleMovement();
        // todo:Set Animations
    }

    private bool IsGrounded() {
        RaycastHit2D raycastHit2d = Physics2D.BoxCast(boxCollider2d.bounds.center, boxCollider2d.bounds.size, 0f, Vector2.down, 1f, platformsLayerMask);
        return raycastHit2d.collider != null;
    }

    private void HandleMovement() {
        rigidbody2d.velocity = new Vector2(+moveSpeed, rigidbody2d.velocity.y);
    }
}