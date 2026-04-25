using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    private InteractableResources currentResource;
    private StructureHealth currentStructure;



    private void Update()
{
    if (Keyboard.current.eKey.wasPressedThisFrame)
    {
        // Recolección
        if (currentResource != null)
        {
            currentResource.Interact(gameObject);
        }

        // Reparación
        if (currentStructure != null)
        {
            Debug.Log("opprimiendo la tecla e");
            currentStructure.TryRepair();
        }
    }
}

    private void OnTriggerEnter(Collider other)
    {
        InteractableResources resource = other.GetComponent<InteractableResources>();
        if (resource != null)
        {
            currentResource = resource;
        }

        StructureHealth structure = other.GetComponent<StructureHealth>();
        if (structure != null)
        {
            currentStructure = structure;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        InteractableResources resource = other.GetComponent<InteractableResources>();

        if (resource != null && resource == currentResource)
        {
            currentResource = null;
        }


        StructureHealth structure = other.GetComponent<StructureHealth>();
        if (structure != null && structure == currentStructure)
        {
            currentStructure = null;
            //Debug.Log("Salió del rango de la estructura: " + other.name);
        }
    }
}