using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using Unity.VisualScripting;

public class WeaponUnlockPickup : MonoBehaviour
{
    [SerializeField] private int weaponIndex;

    [Header("Textos")]
    [SerializeField] private GameObject textCanUnlock;
    //[SerializeField] private GameObject textNeedEnergy;
    [SerializeField] private TMP_Text energyText;

    private WeaponLogic weaponLogic;
    private bool playerInRange = false;
    
    private void Start()
    {
        if (energyText != null)
            energyText.gameObject.SetActive(false);

        HideTexts();
    }

    private void Update()
    {
        if (playerInRange && Keyboard.current.eKey.wasPressedThisFrame)
        {
            TryUnlockWeapon();
        }

        if (playerInRange)
        {
            UpdateTexts();
        }
    }

    private void TryUnlockWeapon()
    {
        if (weaponLogic != null && weaponLogic.CanUnlockWeapon(weaponIndex))
        {
            weaponLogic.UnlockWeapon(weaponIndex);

            HideTexts();
            gameObject.SetActive(false);
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
            playerInRange = true;

            // 🔥 ACTIVAR texto de energía
            if (energyText != null)
                energyText.gameObject.SetActive(true);

            UpdateTexts(); // para que muestre 3 / 5 por ejemplo
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            weaponLogic = null;

            // 🔥 DESACTIVAR texto
            if (energyText != null)
                energyText.gameObject.SetActive(false);

            HideTexts();
        }
    }

    private void UpdateTexts()
    {
        if (weaponLogic == null) return;

        bool canUnlock = weaponLogic.CanUnlockWeapon(weaponIndex);

        if (textCanUnlock != null)
            textCanUnlock.SetActive(canUnlock);


        if (!canUnlock && energyText != null)
        {
            PlayerEnergy playerEnergy = weaponLogic.GetComponent<PlayerEnergy>();

            if (playerEnergy != null)
            {
                int requiredEnergy = weaponLogic.GetWeaponCost(weaponIndex);
                energyText.text = playerEnergy.currentEnergy + " / " + requiredEnergy;
            }
        }
    }

    private void HideTexts()
    {
        if (textCanUnlock != null)
            textCanUnlock.SetActive(false);

        if (energyText != null)
            energyText.gameObject.SetActive(false);
    }
}