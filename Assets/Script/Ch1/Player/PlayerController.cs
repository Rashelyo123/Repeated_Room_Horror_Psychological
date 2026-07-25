using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using FMODUnity;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkingSpeed = 7.5f;
    [SerializeField] private float runningSpeed = 11.5f;
    [SerializeField] private float gravity = 20.0f;

    [Header("FMOD Event References")]
    // Menggunakan EventReference bawaan FMOD modern
    public EventReference footstepEvent;
    [SerializeField] private float footstepIntervalWalk = 0.5f;
    [SerializeField] private float footstepIntervalRun = 0.3f;

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
    private AudioSource flashlightAudioSource;
    private AudioSource wakeUpAudioSource;

    private float footstepTimer;
    private bool wasMovingLastFrame = false; // Mencegah spamming audio saat tombol ditahan

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
            if (playerWakeUpAnimator != null) playerWakeUpAnimator.enabled = false;
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
        if (flashlight != null) flashlight.enabled = isFlashlightOn;
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

        UpdateFootstepAudio(curSpeedX, curSpeedY, isRunning);
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
            if (flashlight != null) flashlight.enabled = isFlashlightOn;

            // Aman dari NullReferenceException jika AudioClip belum dimasukkan
            if (flashlightSound != null && flashlightAudioSource != null)
            {
                flashlightAudioSource.PlayOneShot(flashlightSound);
            }
        }
    }

    private void HandleCameraRotation()
    {
        rotationX -= Input.GetAxis("Mouse Y") * lookSpeed;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);

        Quaternion cameraTargetRotation = Quaternion.Euler(rotationX, 0, 0);
        if (playerCamera != null)
        {
            playerCamera.transform.localRotation = Quaternion.Slerp(
                playerCamera.transform.localRotation,
                cameraTargetRotation,
                Time.deltaTime * smoothSpeed
            );
        }

        targetRotationY += Input.GetAxis("Mouse X") * lookSpeed;
        Quaternion playerTargetRotation = Quaternion.Euler(0, targetRotationY, 0);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            playerTargetRotation,
            Time.deltaTime * smoothSpeed
        );
    }

    private void UpdateFootstepAudio(float speedX, float speedY, bool isRunning)
    {
        // Deteksi pergerakan yang stabil dengan threshold > 0.1f
        bool isMoving = (Mathf.Abs(speedX) > 0.1f || Mathf.Abs(speedY) > 0.1f) && characterController.isGrounded;

        if (!isMoving)
        {
            wasMovingLastFrame = false;
            footstepTimer = 0f;
            return;
        }

        float currentInterval = isRunning ? footstepIntervalRun : footstepIntervalWalk;

        // Jika baru mulai melangkah (frame pertama)
        if (!wasMovingLastFrame)
        {
            if (!footstepEvent.IsNull)
            {
                RuntimeManager.PlayOneShotAttached(footstepEvent, gameObject);
            }
            footstepTimer = currentInterval;
            wasMovingLastFrame = true;
            return;
        }

        // Hitung mundur timer sesuai interval jalan/lari saat W ditahan
        footstepTimer -= Time.deltaTime;
        if (footstepTimer <= 0f)
        {
            if (!footstepEvent.IsNull)
            {
                RuntimeManager.PlayOneShotAttached(footstepEvent, gameObject);
            }
            footstepTimer = currentInterval;
        }
    }

    private void UpdateCameraShake(float speedX, float speedY)
    {
        if (cameraShake != null)
        {
            float speed = new Vector3(speedX, 0, speedY).magnitude;
            cameraShake.SetFloat("Speed", speed);
        }
    }

    private IEnumerator PlayWakeUpSequence()
    {
        canMove = false;

        if (playerWakeUpAnimator != null)
        {
            playerWakeUpAnimator.enabled = true;
            if (wakeUpSound != null && wakeUpAudioSource != null)
            {
                wakeUpAudioSource.PlayOneShot(wakeUpSound);
            }

            yield return new WaitForSeconds(playerWakeUpAnimator.GetCurrentAnimatorStateInfo(0).length);

            playerWakeUpAnimator.enabled = false;
        }

        canMove = true;
    }
}