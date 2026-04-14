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

    public float speed;
   
    [Header("Rotación")]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float rotationSpeed = 12f;

    private InputAction moveAction;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];//
    }


    void Start()
    {
  
         
    }

    // Update is called once per frame
    void Update()
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
            

            // Movimiento en espacio mundo
        transform.position += moveDirection * speed * Time.deltaTime;

        rotateMouse();

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
}
