using UnityEngine;
using UnityEngine.InputSystem;

public class ClickDamageSystem : MonoBehaviour
{
    public float damage = 10f;
    public Camera cam;

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();

            Ray ray = cam.ScreenPointToRay(mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                StructureHealth structure = hit.collider.GetComponent<StructureHealth>();

                if (structure != null)
                {
                    structure.TakeDamage(damage);
                }
            }
        }
    }
}