using Unity.Netcode;
using UnityEngine;
using Cinemachine;

public class CameraWork : NetworkBehaviour
{
    private Camera mainCam;
    [SerializeField] private Transform cinemachineCam;
    [SerializeField] private Transform camTransform;
    [SerializeField] private Transform anchorTransform;
    [SerializeField] private float radius;
    [SerializeField] private float dir = -1f;
    public float sensitivity = 1f;
    [SerializeField] float swipeThreshold = 2f;

    float deltaX;
    float deltaY;
    int camSwipeID = 0;

    private Touch initTouch;

    public Joystick viewJoystick;

    void Start() {
        if (!IsOwner)  return;

        /*if (camTransform == null) {
            Debug.Log("[CameraWork] 'camTransform' is NULL, switching to MainCam");
            camTransform = Camera.main.transform;
            mainCam = Camera.main;
        }
        if (anchorTransform == null) {
            Debug.Log("[CameraWork 'anchorTransform' is NULL");
        }
        foreach (GameObject obj in Object.FindObjectsOfType<GameObject>()) {
            if (obj.activeInHierarchy) {
                *//*
                if (obj.TryGetComponent<Canvas>(out Canvas canvas)) {
                    viewJoystick = obj.transform.Find("Viewing Joystick").GetComponent<Joystick>();
                } *//*
                if (obj.TryGetComponent<CinemachineFreeLook>(out CinemachineFreeLook cine)) {
                    thirdPersonCam = cine;
                    cine.Follow = anchorTransform;
                    cine.LookAt = anchorTransform;
                }
            }
        }
        
        //if (SystemInfo.deviceType == DeviceType.Handheld) {
        //    CinemachineCore.GetInputAxis = GetAxisCustom;
        //}
        */
        //Debug.LogAssertion("CAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAMEEEERAAAAAAAA");
        camTransform = Instantiate(camTransform);       // We replace out PREFAB with the instantiated camera
        camTransform.tag = "MainCamera";
        mainCam = camTransform.GetComponent<Camera>();

        CinemachineFreeLook cine = Instantiate(cinemachineCam).GetComponent<CinemachineFreeLook>();
        cine.Follow = anchorTransform;
        cine.LookAt = anchorTransform;

        GetComponent<PlayerMovement>().camTransform = camTransform;
        GetComponent<ThrowBehaviour>().t_cam        = camTransform;


        initTouch = new Touch();

        MobileCheck.CheckForMobile += CheckMobile;
    }

    void Update() {
        if (!IsOwner)  return;

        // Looping through each touch on the screen
        foreach (Touch touch in Input.touches) {
            if (touch.phase == TouchPhase.Began) {
                initTouch = touch;
                camSwipeID = (touch.position.x >= mainCam.pixelWidth * 0.5f) ? touch.fingerId : -1;      // Can only swipe if the initial touch is on the right-half of the screen.
            }
            else if (touch.phase == TouchPhase.Moved && camSwipeID == touch.fingerId) {
                // Swiping
                float newDeltaX = initTouch.position.x - touch.position.x;
                float newDeltaY = initTouch.position.y - touch.position.y;

                deltaX = (Mathf.Abs(newDeltaX - deltaX) > swipeThreshold) ? newDeltaX : 0f;
                deltaY = (Mathf.Abs(newDeltaY - deltaY) > swipeThreshold) ? newDeltaY : 0f;

                initTouch = touch;
            }
            else if (touch.phase == TouchPhase.Ended) {
                initTouch = new Touch();

                deltaX = 0f;
                deltaY = 0f;
            }
        }

        float angleY = camTransform.localEulerAngles.y * Mathf.Deg2Rad;
        float xPos   = radius * Mathf.Cos(angleY);
        float zPos   = radius * Mathf.Sin(-angleY);

        anchorTransform.localPosition = new Vector3(xPos, anchorTransform.localPosition.y, zPos);
    }

    public float GetAxisCustom(string axisName) {
        if (axisName == "Mouse X") {
            //return viewJoystick.Horizontal * sensitivity;
            return deltaX * sensitivity * dir;
        }
        else if (axisName == "Mouse Y") {
            //return viewJoystick.Vertical * sensitivity;
            return deltaY * sensitivity * dir;
        }

        return 0;
    }

    void CheckMobile() {
        CinemachineCore.GetInputAxis = GetAxisCustom;

        foreach (GameObject obj in Object.FindObjectsOfType<GameObject>()) {
            if (obj.activeInHierarchy) {
                if (obj.TryGetComponent<Joystick>(out Joystick js)) {
                    //viewJoystick = obj.transform.Find("Viewing Joystick").GetComponent<Joystick>();
                    if (js.gameObject.name.Contains("Viewing"))  { viewJoystick = js; }
                }
            }
        }
    }
}
