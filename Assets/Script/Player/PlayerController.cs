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
    //[SerializeField] private Animator cameraShake;
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
    private bool isFlashlightOn = true;
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

        // if (SceneManager.GetActiveScene().name == MAIN_SCENE_NAME || SceneManager.GetActiveScene().name == lastscene)
        // {
        //     StartCoroutine(PlayWakeUpSequence());
        // }
        // else
        // {
        //     if (playerWakeUpAnimator != null) playerWakeUpAnimator.enabled = false;
        //     canMove = true;
        // }
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

    private float stopTimer = 0f;

    private void UpdateFootstepAudio(float speedX, float speedY, bool isRunning)
    {
        // Hitung kecepatan horizontal asli karakter (mengabaikan sumbu Y/gravitasi)
        Vector3 horizontalVelocity = new Vector3(characterController.velocity.x, 0, characterController.velocity.z);

        // Player hanya dianggap jalan jika kecepatan mendatarnya > 0.5 dan menempel tanah
        bool isMoving = horizontalVelocity.magnitude > 0.5f && characterController.isGrounded;

        if (!isMoving)
        {
            stopTimer += Time.deltaTime;
            // Hanya reset timer jika player benar-benar berhenti lebih dari 0.2 detik
            if (stopTimer > 0.2f)
            {
                footstepTimer = 0f;
            }
            return;
        }

        // Reset timer berhenti jika sedang bergerak
        stopTimer = 0f;

        float currentInterval = isRunning ? footstepIntervalRun : footstepIntervalWalk;

        // Hitung mundur jeda antar langkah
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
        // if (cameraShake != null)
        // {
        //     float speed = new Vector3(speedX, 0, speedY).magnitude;
        //     cameraShake.SetFloat("Speed", speed);
        // }
    }

    //     private IEnumerator PlayWakeUpSequence()
    //     {
    //         canMove = false;

    //         if (playerWakeUpAnimator != null)
    //         {
    //             playerWakeUpAnimator.enabled = true;
    //             if (wakeUpSound != null && wakeUpAudioSource != null)
    //             {
    //                 wakeUpAudioSource.PlayOneShot(wakeUpSound);
    //             }

    //             yield return new WaitForSeconds(playerWakeUpAnimator.GetCurrentAnimatorStateInfo(0).length);

    //             playerWakeUpAnimator.enabled = false;
    //         }

    //         canMove = true;
    //     }
}