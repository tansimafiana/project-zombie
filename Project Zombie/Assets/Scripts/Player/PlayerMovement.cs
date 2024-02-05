using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    private CharacterController _controller;

    [SerializeField] private float _speed;
    [SerializeField] private float _gravity = 9.81f;
    [SerializeField] private Vector3 _velocity;
    [SerializeField] private float _jumpForce;

    [HideInInspector] public Transform camTransform = null;
    [SerializeField] private Transform modelTransform;

    public bool _isGrounded;
    [SerializeField] private Transform _groundCheckerTransform;
    [SerializeField] private float _sphereRadius = 0.1f;
    [SerializeField] private LayerMask _checkForGround;

    public Joystick joystick;
    private bool isPlayingMobile = false;
    [HideInInspector] public bool isAiming = false;

    [HideInInspector] public Animator animator;


    void Start() {
        _controller = GetComponent<CharacterController>();
        _velocity = new Vector3();

        MobileCheck.CheckForMobile += CheckMobile;

        animator = GetComponent<Animator>();
    }

    //public override void OnNetworkSpawn() {
    //    if (!IsOwner) Destroy(this);
    //}

    void Update() {
        if (!IsOwner)  return;

        // Movement Axis
        float angleY = (camTransform) ? camTransform.localEulerAngles.y * Mathf.Deg2Rad : 0f;       // Check in case we have no camera

        Vector3 move;
        if (isPlayingMobile) {      // If on Mobile...
            move = new Vector3(joystick.Vertical * Mathf.Sin(angleY) + joystick.Horizontal * Mathf.Cos(angleY),
                               0f,
                               joystick.Vertical * Mathf.Cos(angleY) + joystick.Horizontal * Mathf.Sin(-angleY));
        } else {                    // If on Desktop...
            move = new Vector3(Input.GetAxis("Vertical") * Mathf.Sin(angleY) + Input.GetAxis("Horizontal") * Mathf.Cos(angleY),
                               0f,
                               Input.GetAxis("Vertical") * Mathf.Cos(angleY) + Input.GetAxis("Horizontal") * Mathf.Sin(-angleY));
        }

        // Checking for ground
        _isGrounded = Physics.CheckSphere(_groundCheckerTransform.position, _sphereRadius, _checkForGround, QueryTriggerInteraction.Ignore);
        if (_isGrounded) {
            if (_velocity.y < 0f) {
                _velocity.y = 0f;
            }

            if (Input.GetButtonDown("Jump")) {
                Jump();
            }
        } else {
            _velocity.y -= _gravity * Time.deltaTime;
        }
        move.y = _velocity.y;

        // Moving the player
        _controller.Move(move * Time.deltaTime * _speed);

        // Turning the player's model to face forward
        RotateModelWhenMoving(angleY * Mathf.Rad2Deg, move);

        // Updating Animator
        if (move.x != 0f || move.z != 0f) {
            animator.SetBool("isRunning", true);
        } else {
            animator.SetBool("isRunning", false);
        }
    }

    public void Jump() {
        if (_isGrounded)
            _velocity.y += _jumpForce;
    }

    void RotateModelWhenMoving(float angleY, Vector3 moveAxis) {
        // If we're moving
        if ((moveAxis.x != 0f && moveAxis.z != 0f) || isAiming) {
            modelTransform.localEulerAngles = new Vector3(0f, angleY, 0f);
        }
    }

    void CheckMobile() {
        isPlayingMobile = true;

        foreach (GameObject obj in Object.FindObjectsOfType<GameObject>()) {
            if (obj.activeInHierarchy) {
                if (obj.TryGetComponent<Joystick>(out Joystick js)) {
                    //viewJoystick = obj.transform.Find("Viewing Joystick").GetComponent<Joystick>();
                    if (js.gameObject.name.Contains("Movement"))  { joystick = js; }
                }
            }
        }
    }
}
