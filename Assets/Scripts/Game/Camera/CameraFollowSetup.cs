using UnityEngine;

//Set up for CameraFollow, it will follow the transform with zoom 
public class CameraFollowSetup : MonoBehaviour {

    [SerializeField] private CameraFollow cameraFollow;
    public Transform followTransform;
    public float zoom;

    private void Start() {
        if (followTransform == null) {
            Debug.LogError("followTransform is null! Intended?");
            cameraFollow.Setup(() => Vector3.zero, () => zoom);
        }
        else {
            cameraFollow.Setup(() => followTransform.position, () => zoom);
        }
    }
}