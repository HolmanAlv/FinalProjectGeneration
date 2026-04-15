using System;
using NUnit.Framework;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove2 : MonoBehaviour
{
    
    public float speed;
    public float rotationSpeed;
    public float attackDuration = 1f;
    [SerializeField] private Transform visual;
    

    private Vector3 forward, right;
    public Camera cam;

    private Vector3 lastMoveDirection;
    
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private Animator animator;
    [SerializeField] private LayerMask groundMask;
    

    private InputAction moveAction;
    private InputAction attackAction;

    [SerializeField] private bool isAttacking;
    private bool enemyInRange;

    void Awake()
    {
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

    // Update is called once per frame
    void Update()
    {
        if(isAttacking)
        return;

        if (attackAction.WasPressedThisFrame())
    {
        Debug.Log("Se presionó Attack");

        if (enemyInRange)
        {
            Debug.Log("Hay enemigo en rango, inicia ataque");
            StartAttack();
            return;
        }
        else
        {
            Debug.Log("No hay enemigo en rango");
        }
    }

    if (!isAttacking)
        {
            Vector2 input = moveAction.ReadValue<Vector2>();
            Vector3 direction = right * input.x + forward * input.y;

            if (direction.magnitude > 1f)
            direction.Normalize();

            if (direction.magnitude > 0.1f)
            {
                //para evitar que vaya mas rapido o mas lento segun el frame rate
                transform.position += direction * speed * Time.deltaTime;

                Quaternion targetRotation = Quaternion.LookRotation(direction);
                visual.rotation = Quaternion.Slerp(visual.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                lastMoveDirection = direction;
        
            }

            Animations(input);
        }

            
        
    
    }

    private void Movement()
    {
        
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

    private void StartAttack ()
    {
        
        RotateToMouse();
        isAttacking = true;

        animator.SetFloat("MoveAmount", 0f);
        animator.SetTrigger("Attack");

        Invoke(nameof(EndAttack), attackDuration);
        
    }

    private void RotateToMouse()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Ray ray = cam.ScreenPointToRay(mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundMask))
        {
            Debug.Log("Click en suelo: " + hit.point);

            Vector3 targetPoint = hit.point;
            Vector3 direction = targetPoint - transform.position;
            direction.y = 0f;

            if (direction.sqrMagnitude < 0.001f)
                return;

            Quaternion targetRotation = Quaternion.LookRotation(direction);

            // Giro instantáneo para probar
            visual.rotation = targetRotation;

            Debug.Log("Giró hacia: " + direction);
        }
        else
        {
            Debug.Log("El raycast no golpeó el suelo");
        }
    }

    private void EndAttack()
    {
        isAttacking = false;


        
    }

    public void SetEnemyInRange(bool value)
    {
        enemyInRange = value;
    }

}
