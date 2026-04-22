using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutoutObject : MonoBehaviour
{
    [SerializeField]
    private Transform targetObject;
    
    [SerializeField]
    private LayerMask wallMask;
    
    private Camera mainCamera;
    private List<Renderer> lastSeenRenderers = new List<Renderer>();

    private Vector3 targetOffset = new Vector3(-3.4f, 6f, -3.4f);
    private Vector2 cutoutPos = new Vector2(0.5f, 0.55f);
    private float cutoutSize = 0.45f;
    private float falloffSize = 0.6f;
    private float sphereCastRadius = 4f;

    private void Awake()
    {
        if (mainCamera == null) mainCamera = Camera.main;
    }
    
    private void Update()
{
    if (targetObject == null || mainCamera == null) return;

    Vector3 targetPosition = targetObject.position + targetOffset;

    Vector3 rayOrigin = mainCamera.transform.position;
    Vector3 direction = targetPosition - rayOrigin;
    float distance = direction.magnitude;
    
    RaycastHit[] hitObjects = Physics.SphereCastAll(
        rayOrigin, 
        sphereCastRadius, 
        direction.normalized, 
        distance, 
        wallMask,
        QueryTriggerInteraction.Ignore
    );
    
    System.Array.Sort(hitObjects, (a, b) => a.distance.CompareTo(b.distance));
    
    List<Renderer> currentlyHitRenderers = new List<Renderer>();
    
    for (int i = 0; i < hitObjects.Length; i++)
    {
        Renderer renderer = hitObjects[i].transform.GetComponent<Renderer>();
        if (renderer != null)
        {
            currentlyHitRenderers.Add(renderer);
            
            Material[] materials = renderer.materials;
            for (int n = 0; n < materials.Length; n++)
            {
                materials[n].SetVector("_CutoutPos", cutoutPos);
                materials[n].SetFloat("_CutoutSize", cutoutSize);
                materials[n].SetFloat("_FalloffSize", falloffSize);
            }
        }
    }

    foreach (Renderer renderer in lastSeenRenderers)
    {
        if (renderer != null && !currentlyHitRenderers.Contains(renderer))
        {
            Material[] materials = renderer.materials;
            for (int n = 0; n < materials.Length; n++)
            {
                materials[n].SetFloat("_CutoutSize", 0f);
            }
        }
    }
    lastSeenRenderers = currentlyHitRenderers;
}
}