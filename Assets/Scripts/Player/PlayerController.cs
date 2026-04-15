using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal.Internal;
using UnityEngine.VFX;

public class PlayerController : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform visual;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Animator animator;

    public float speed; 
   
    [Header("Rotación")]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float rotationSpeed = 12f;

    private InputAction moveAction;
    private InputAction AttackAction;

    private Vector3 currentMoveDirection;


    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];//
        AttackAction = playerInput.actions["Attack"];

    }


    void Start()
    {
  
         
    }

    // Update is called once per frame
    void Update()
    {
        
        Move();
        rotateMouse();
        UpdateAnimations();
        Attack();

    }

    private void Move()
    {
        //leemos el input del teclado
        Vector2 input = moveAction.ReadValue<Vector2>();

        // tomamos las direcciones de la camara
        Vector3 camForward = mainCamera.transform.forward;
        Vector3 camRight = mainCamera.transform.right;

        // Quitamos la parte vertical para que el movimiento sea sobre el suelo
        camForward.y = 0f;
        camRight.y = 0f;
       

        camForward.Normalize();
        camRight.Normalize();
        //dejamos el ventro con longitud 1, porque solo lo usaremos como dirección, no como fuerza


         // direccion final
        Vector3 moveDirection = camForward * input.y + camRight * input.x;

         // Evita que en diagonal corra más rápido
        if (moveDirection.magnitude > 1f)
        {
            moveDirection.Normalize();
        }
        currentMoveDirection = moveDirection;
            

           
        transform.position += moveDirection * speed * Time.deltaTime;
    }


    private void rotateMouse()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Ray ray = mainCamera.ScreenPointToRay(mousePosition); // convertimos a rayo

        if(Physics.Raycast(ray, out RaycastHit hit, 100f, groundMask))
        {
            Vector3 targetPoint= hit.point; 
            Vector3 direction = targetPoint -transform.position; 
            direction.y = 0;

            if (direction.sqrMagnitude < 0.001f)
            return;

            // creamos la rotación
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            visual.rotation = Quaternion.Slerp( visual.rotation, targetRotation, rotationSpeed * Time.deltaTime); 

        }
    }

    
    private void UpdateAnimations()
    {
        // 1. Si no hay movimiento → Idle
        if (currentMoveDirection.sqrMagnitude < 0.001f)
        {
            animator.SetFloat("MoveSigned", 0f, 0.1f, Time.deltaTime);
            return;
        }

        // 2. Dirección hacia donde mira el personaje
        Vector3 forward = visual.forward;
        forward.y = 0f;
        forward.Normalize();

        // 3. Dirección de movimiento
        Vector3 moveDir = currentMoveDirection;
        moveDir.y = 0f;
        moveDir.Normalize();

        // 4. Comparación clave
        float dot = Vector3.Dot(forward, moveDir);

        // 5. Decisión de animación
        if (dot > 0.2f)
        {
            // Avanza
            animator.SetFloat("MoveSigned", 1f, 0.1f, Time.deltaTime);
        }
        else if (dot < -0.2f)
        {
            // Retrocede
            animator.SetFloat("MoveSigned", -1f, 0.1f, Time.deltaTime);
        }
        else
        {
            // Movimiento lateral → por ahora lo tratamos como forward
            animator.SetFloat("MoveSigned", 1f, 0.1f, Time.deltaTime);
        }
    }

        private void Attack ()
    {
        if (AttackAction.WasPressedThisFrame())
        {
            animator.SetTrigger("Attack");
        }
    }

}
