using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponUnlockPickup : MonoBehaviour
{
    [SerializeField] private int weaponIndex;
    [SerializeField] private GameObject textInteract;
    [SerializeField] private GameObject weaponPickupObject; // El objeto del bate en la escena

    private WeaponLogic weaponLogic;
    private bool playerInRange = false;
    private PlayerInput playerInput;

    private void Start()
    {
        if (textInteract != null)
            textInteract.SetActive(false);
    }

    private void Update()
    {
        // Detectar presión de E cuando el jugador está en rango
        if (playerInRange && playerInput != null)
        {
            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                TryUnlockWeapon();
            }
        }
    }

    private void TryUnlockWeapon()
    {
        if (weaponLogic != null && weaponLogic.CanUnlockWeapon(weaponIndex))
        {
            weaponLogic.UnlockWeapon(weaponIndex);

            if (textInteract != null)
                textInteract.SetActive(false);

            gameObject.SetActive(false); // ← suficiente

        }
        else
        {
            Debug.Log("No puedes desbloquear esta arma todavía");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            weaponLogic = other.GetComponent<WeaponLogic>();
            playerInput = other.GetComponent<PlayerInput>();

            if (weaponLogic != null)
            {
                if (weaponLogic.CanUnlockWeapon(weaponIndex))
                {
                    if (textInteract != null)
                        textInteract.SetActive(true);
                    playerInRange = true;
                }
                else
                {
                    Debug.Log("No puedes desbloquear esta arma todavía");
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (textInteract != null)
                textInteract.SetActive(false);
            playerInRange = false;
            playerInput = null;
        }
    }
}