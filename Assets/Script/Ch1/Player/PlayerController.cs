using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterController), typeof(AudioSource))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkingSpeed = 7.5f;
    [SerializeField] private float runningSpeed = 11.5f;
    [SerializeField] private float gravity = 20.0f;

    [Header("Camera Settings")]
    public Camera playerCamera;
    [SerializeField] private float lookSpeed = 2.0f;
    [SerializeField] private float smoothSpeed = 5.0f;
    [SerializeField] private float lookXLimit = 45.0f;

    [Header("Flashlight Settings")]
    [SerializeField] private Light flashlight;
    [SerializeField] private AudioClip flashlightSound;

    [Header("Animation Settings")]
    [SerializeField] private Animator cameraShake;
    [SerializeField] private Animator playerWakeUpAnimator;
    [SerializeField] private AudioClip wakeUpSound;

    private CharacterController characterController;
    private AudioSource footstepAudioSource;
    private AudioSource flashlightAudioSource;
    private AudioSource wakeUpAudioSource;

    private Vector3 moveDirection = Vector3.zero;
    private float rotationX;
    private float targetRotationY;
    private bool isFlashlightOn;
    private bool canMove = true;

    public const string MAIN_SCENE_NAME = "CH1";
    public const string lastscene = "CH8";

    private void Awake()
    {
        InitializeComponents();
        InitializeCursor();
    }

    private void Start()
    {
        SetupInitialState();

        if (SceneManager.GetActiveScene().name == MAIN_SCENE_NAME || SceneManager.GetActiveScene().name == lastscene)
        {
            StartCoroutine(PlayWakeUpSequence());
        }
        else
        {
            playerWakeUpAnimator.enabled = false;
            footstepAudioSource.enabled = true;
            canMove = true;
        }
    }

    private void Update()
    {
        if (!canMove) return;

        HandleMovement();
        HandleFlashlight();
        HandleCameraRotation();
    }

    private void InitializeComponents()
    {
        characterController = GetComponent<CharacterController>();
        footstepAudioSource = GetComponent<AudioSource>();
        flashlightAudioSource = gameObject.AddComponent<AudioSource>();
        wakeUpAudioSource = gameObject.AddComponent<AudioSource>();
    }

    private void InitializeCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void SetupInitialState()
    {
        flashlight.enabled = isFlashlightOn;
        targetRotationY = transform.eulerAngles.y;
    }

    private void HandleMovement()
    {
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float speed = isRunning ? runningSpeed : walkingSpeed;

        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        float curSpeedX = canMove ? speed * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? speed * Input.GetAxis("Horizontal") : 0;

        UpdateFootstepAudio(curSpeedX, curSpeedY);
        UpdateCameraShake(curSpeedX, curSpeedY);

        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        characterController.Move(moveDirection * Time.deltaTime);
    }

    private void HandleFlashlight()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            isFlashlightOn = !isFlashlightOn;
            flashlight.enabled = isFlashlightOn;
            flashlightAudioSource.PlayOneShot(flashlightSound);
        }
    }

    private void HandleCameraRotation()
    {
        rotationX -= Input.GetAxis("Mouse Y") * lookSpeed;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);

        Quaternion cameraTargetRotation = Quaternion.Euler(rotationX, 0, 0);
        playerCamera.transform.localRotation = Quaternion.Slerp(
            playerCamera.transform.localRotation,
            cameraTargetRotation,
            Time.deltaTime * smoothSpeed
        );

        targetRotationY += Input.GetAxis("Mouse X") * lookSpeed;
        Quaternion playerTargetRotation = Quaternion.Euler(0, targetRotationY, 0);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            playerTargetRotation,
            Time.deltaTime * smoothSpeed
        );
    }

    private void UpdateFootstepAudio(float speedX, float speedY)
    {
        if ((speedX != 0 || speedY != 0) && !footstepAudioSource.isPlaying)
        {
            footstepAudioSource.Play();
        }
        else if (speedX == 0 && speedY == 0)
        {
            footstepAudioSource.Stop();
        }
    }

    private void UpdateCameraShake(float speedX, float speedY)
    {
        float speed = new Vector3(speedX, 0, speedY).magnitude;
        cameraShake.SetFloat("Speed", speed);
    }

    private IEnumerator PlayWakeUpSequence()
    {
        footstepAudioSource.enabled = false;
        canMove = false;

        playerWakeUpAnimator.enabled = true;
        wakeUpAudioSource.PlayOneShot(wakeUpSound);

        yield return new WaitForSeconds(playerWakeUpAnimator.GetCurrentAnimatorStateInfo(0).length);

        playerWakeUpAnimator.enabled = false;
        footstepAudioSource.enabled = true;
        canMove = true;
    }
}