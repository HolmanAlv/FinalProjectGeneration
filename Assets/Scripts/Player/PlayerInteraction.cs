using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    private InteractableResources currentResource;



    private void Update()
    {
        if (currentResource != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            currentResource.Interact(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        InteractableResources resource = other.GetComponent<InteractableResources>();

        if (resource != null)
        {
            currentResource = resource;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        InteractableResources resource = other.GetComponent<InteractableResources>();

        if (resource != null && resource == currentResource)
        {
            currentResource = null;
        }
    }
}