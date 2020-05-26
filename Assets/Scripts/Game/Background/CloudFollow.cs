using System.Collections;
using System.Linq;
using UnityEngine;

public class CloudFollow : MonoBehaviour {
    public float speed;
    public Camera Camera;
    public GameObject GameplayContainer;
    public bool RespawnChildren = false;
    private Transform cameraTransform;

    private Rigidbody2D targetRigidbody;
    private Transform[] children;
    private float cameraSize;
    private Vector3 newPosition = new Vector3();
    private float y = 0, z = 0;

    void Start() {
        children = GetComponentsInChildren<Transform>().Where(ch => ch.gameObject != gameObject).ToArray<Transform>();
        cameraSize = Camera.GetComponent<CameraFollowSetup>().zoom;
        cameraTransform = Camera.GetComponent<Transform>();
        speed /= 100;
        targetRigidbody = Camera.GetComponent<CameraFollowSetup>().followTransform.GetComponent<Rigidbody2D>();
        StartCoroutine(SetTarget());
        y = transform.position.y;
        z = transform.position.z;
    }

    void Update() {
        newPosition.Set(transform.position.x + speed * targetRigidbody.velocity.x / 10, y, z);
        transform.position = newPosition;

        if (RespawnChildren) {
            for (int i = 0; i < children.Length; i++) {
                if (children[i].position.x < cameraTransform.position.x - cameraSize * 4)
                    children[i].position = new Vector3(cameraTransform.position.x + cameraSize * 4, children[i].position.y, children[i].position.z);
                else if (children[i].position.x > cameraTransform.position.x + cameraSize * 4)
                    children[i].position = new Vector3(cameraTransform.position.x - cameraSize * 4, children[i].position.y, children[i].position.z);
            }
        }
    }

    private IEnumerator SetTarget() {
        yield return new WaitUntil(() => GlobalModel.MyCharacter != -1);
        targetRigidbody = GameplayContainer.transform.Find(GlobalModel.CharacterName(GlobalModel.MyCharacter)).GetComponent<Rigidbody2D>();
    }
}
