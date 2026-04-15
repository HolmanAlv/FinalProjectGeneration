using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove2 : MonoBehaviour
{
    
    public float speed;
    public float rotationSpeed;
    private Vector3 forward, right;
    public Camera cam;
    
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private Animator animator;
    

    private InputAction moveAction;
    private InputAction AttackAction;

    void Awake()
    {
        moveAction = playerInput.actions["Move"];
        AttackAction = playerInput.actions["Attack"];
            
        
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

    // Update is called once per frame
    void Update()
    {
        Vector2 input = moveAction.ReadValue<Vector2>();
        Vector3 direction = right * input.x + forward * input.y;

        if (direction.magnitude > 0.1f)
        {
            //para evitar que vaya mas rapido o mas lento segun el frame rate
            transform.position += direction * speed * Time.deltaTime;

            Quaternion targetarotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetarotation, rotationSpeed * Time.deltaTime);
        }

        Animations(input);
        Attack();
        

    }

    private void Animations(Vector2 input)
    {
        if (input.sqrMagnitude < 0.01f)
        {
            // No hay input → Idle
            animator.SetFloat("MoveAmount", 0f, 0.1f, Time.deltaTime);
        }
        else
        {
            // Hay input → Run
            animator.SetFloat("MoveAmount", 1f, 0.1f, Time.deltaTime);
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
