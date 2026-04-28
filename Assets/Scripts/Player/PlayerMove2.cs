using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove2 : MonoBehaviour
{
    public float speed;
    public float rotationSpeed;
    public float attackDuration = 1f;

    [SerializeField] private Transform visual;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private Animator animator;
    [SerializeField] private LayerMask groundMask;

    public Camera cam;

    private CharacterController characterController;

    [Header("Gravity")]
    [SerializeField] private float gravity = -20f;
    [SerializeField] private float verticalVelocity;

    private Vector3 forward;
    private Vector3 right;
    private Vector3 lastMoveDirection;

    private InputAction moveAction;
    private InputAction attackAction;

    [SerializeField] private bool isAttacking;
    private bool enemyInRange;

    [Header("Footsteps")]
    [SerializeField] private AudioSource footstepSource;
    [SerializeField] private AudioClip[] footstepClips;
    [SerializeField] private float stepInterval = 0.35f;
    [SerializeField] private float footstepVolume = 0.7f;

    [Header("Weapon")]
    [SerializeField] private WeaponLogic weaponLogic;

    //[SerializeField] private Collider weaponCollider;
    [SerializeField] private float attackHitDelay = 0.2f;
    [SerializeField] private float attackHitDuration = 0.25f;

    






    private float stepTimer;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();

        moveAction = playerInput.actions["Move"];
        attackAction = playerInput.actions["Attack"];
    }

    void Start()
    {

        forward = cam.transform.forward;
        forward.y = 0;
        forward = Vector3.Normalize(forward);

        right = cam.transform.right;
        right.y = 0;
        right = Vector3.Normalize(right);


    }

    void Update()
    {
        if (isAttacking)
            return;

        if (attackAction.WasPressedThisFrame())
        {
            Debug.Log("Se presionó Attack");

            if (enemyInRange)
            {
                //Debug.Log("Hay enemigo en rango, inicia ataque");
                StartAttack();
                return;
            }
            else
            {
                //Debug.Log("No hay enemigo en rango");
            }
        }

        Vector2 input = moveAction.ReadValue<Vector2>();

        HandleFootsteps(input);

        Vector3 direction = right * input.x + forward * input.y;

        if (direction.magnitude > 1f)
            direction.Normalize();

        // Gravedad para que el CharacterController se mantenga pegado al suelo
        if (characterController.isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f;
        }

        verticalVelocity += gravity * Time.deltaTime;

        Vector3 finalMovement = direction * speed;
        finalMovement.y = verticalVelocity;

        characterController.Move(finalMovement * Time.deltaTime);

        if (direction.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            visual.rotation = Quaternion.Slerp(
                visual.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );

            lastMoveDirection = direction;
        }

        Animations(input);
    }
    

    private void Animations(Vector2 input)
    {
        if (input.sqrMagnitude < 0.01f)
        {
            animator.SetFloat("MoveAmount", 0f, 0.1f, Time.deltaTime);
        }
        else
        {
            animator.SetFloat("MoveAmount", 1f, 0.1f, Time.deltaTime);
        }
    }

    private void StartAttack()
    {
        RotateToMouse();
        isAttacking = true;

        animator.SetFloat("MoveAmount", 0f);
        animator.SetTrigger("Attack");


        // Activa el daño 0.2 segundos después de iniciar la animación
        Invoke(nameof(EnableWeaponCollider), attackHitDelay);

        // Lo desactiva después de la ventana de golpe
        Invoke(nameof(DisableWeaponCollider), attackHitDelay + attackHitDuration);

        // Termina el ataque completo
        Invoke(nameof(EndAttack), attackDuration);

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX("PlayerAttack");
        }
    }

    private void RotateToMouse()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Ray ray = cam.ScreenPointToRay(mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundMask))
        {
            Vector3 targetPoint = hit.point;
            Vector3 direction = targetPoint - transform.position;
            direction.y = 0f;

            if (direction.sqrMagnitude < 0.001f)
                return;

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            visual.rotation = targetRotation;
        }
        else
        {
            Debug.Log("El raycast no golpeó el suelo");
        }
    }

    public void EnableWeaponCollider()
    {
        if (weaponLogic != null)
        {
            weaponLogic.EnableWeaponCollider();
            Debug.Log("Collider del arma ACTIVADO");
        }
    }

    public void DisableWeaponCollider()
    {
        if (weaponLogic != null)
        {
            weaponLogic.DisableWeaponCollider();
            Debug.Log("Collider del arma DESACTIVADO");
        }
    }

    private void EndAttack()
    {
        DisableWeaponCollider();
        isAttacking = false;
    }

    public void SetEnemyInRange(bool value)
    {
        enemyInRange = value;
    }

    private void HandleFootsteps(Vector2 input)
    {
        if (isAttacking)
            return;

        if (input.sqrMagnitude < 0.01f)
        {
            stepTimer = 0f;
            return;
        }

        stepTimer += Time.deltaTime;

        if (stepTimer >= stepInterval)
        {
            PlayFootstep();
            stepTimer = 0f;
        }
    }

    private void PlayFootstep()
    {
        if (footstepClips == null || footstepClips.Length == 0)
            return;

        if (footstepSource == null)
            return;

        int index = Random.Range(0, footstepClips.Length);
        AudioClip clip = footstepClips[index];

        footstepSource.pitch = Random.Range(0.95f, 1.05f);
        footstepSource.PlayOneShot(clip, footstepVolume);
    }
}