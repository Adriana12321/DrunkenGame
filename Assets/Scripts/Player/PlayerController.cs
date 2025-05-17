using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public float walkingSpeed = 7.5f;
    public float runningSpeed = 11.5f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;

    public CinemachineBrain mainCamera;
    public CinemachineVirtualCamera fpsCamera;
    public CinemachineVirtualCamera thirdPersonCamera;

    public float lookSpeed = 2.0f;
    public float lookXLimit = 45.0f;

    private CharacterController characterController;
    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;

    [HideInInspector]
    public bool canMove = true;

    public static PlayerController Instance;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    private static readonly int MoveX = Animator.StringToHash("MoveX");
    private static readonly int MoveY = Animator.StringToHash("MoveY");

    void Start()
    {
        thirdPersonCamera.enabled = false;
        fpsCamera.enabled = true;

        Instance = this;
        characterController = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Vertical");
        float curSpeedX = canMove ? (isRunning ? runningSpeed : walkingSpeed) * inputY : 0;
        float curSpeedY = canMove ? (isRunning ? runningSpeed : walkingSpeed) * inputX : 0;

        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpSpeed;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        characterController.Move(moveDirection * Time.deltaTime);

        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            fpsCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }

        UpdateAnimator(inputX, inputY, isRunning);
    }

    private void UpdateAnimator(float inputX, float inputY, bool isRunning)
    {
        if (animator == null) return;

        float blendSpeed =1f;
        animator.SetFloat(MoveX, inputX * blendSpeed);
        animator.SetFloat(MoveY, inputY * blendSpeed);
    }

    public void SwitchCamera()
    {
        thirdPersonCamera.gameObject.SetActive(!thirdPersonCamera.gameObject.activeSelf);
        fpsCamera.gameObject.SetActive(!fpsCamera.gameObject.activeSelf);
    }
}
