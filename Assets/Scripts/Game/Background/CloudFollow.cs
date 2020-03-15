using System.Linq;
using UnityEngine;

public class CloudFollow : MonoBehaviour {
    public float speed;
    public Camera camera;
    private Transform cameraTransform;

    private Transform[] children;
    private float cameraSize;
    private Vector3 newPosition = new Vector3();
    private float y = 0, z = 0;

    void Start() {
        children = GetComponentsInChildren<Transform>().Where(ch => ch.gameObject != gameObject).ToArray<Transform>();
        cameraSize = camera.GetComponent<CameraFollowSetup>().zoom;
        cameraTransform = camera.GetComponent<Transform>();
        speed /= 100;
        y = transform.position.y;
        z = transform.position.z;
    }

    void Update() {
        //if (transform.position.x < cameraTransform.position.x) {
            newPosition.Set(transform.position.x + speed, y, z);
            transform.position = newPosition;
        //}

        for (int i = 0; i < children.Length; i++) {
            if (children[i].position.x < cameraTransform.position.x - cameraSize * 2)
                children[i].position = new Vector3(cameraTransform.position.x + cameraSize * 2, children[i].position.y, children[i].position.z);
            else if(children[i].position.x > cameraTransform.position.x + cameraSize * 2)
                children[i].position = new Vector3(cameraTransform.position.x - cameraSize * 2, children[i].position.y, children[i].position.z);
        }
    }
}
