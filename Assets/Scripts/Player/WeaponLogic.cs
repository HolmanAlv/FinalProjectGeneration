using UnityEngine;

public class WeaponLogic : MonoBehaviour
{
    [System.Serializable]
    public class WeaponData
    {
        public GameObject weaponObject;
        public int damage;
        public int costToUnlock;
    }

    [Header("Weapons")]
    [SerializeField] private WeaponData[] weapons;
    [SerializeField] private int currentWeaponIndex = -1;
    


    private Collider currentWeaponCollider;
    private PlayerEnergy playerEnergy;

    public int CurrentDamage
    {
        get
        {
            if (currentWeaponIndex < 0 || currentWeaponIndex >= weapons.Length)
            {
                Debug.LogWarning("No hay arma equipada, daño = 0");
                return 0;
            }

            return weapons[currentWeaponIndex].damage;
        }
    }

    private void Awake()
    {
        playerEnergy = GetComponent<PlayerEnergy>();
    }

    private void Start()
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i].weaponObject != null)
                weapons[i].weaponObject.SetActive(false);
        }

        // Equipar arma inicial automáticamente
        if (weapons.Length > 0)
        {
            EquipWeapon(0);
        }
    }
        

    private void EquipWeapon(int index)
    {
        if (index < 0 || index >= weapons.Length)
            return;

        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].weaponObject.SetActive(i == index);
        }

        currentWeaponIndex = index;
        currentWeaponCollider = weapons[currentWeaponIndex].weaponObject.GetComponent<Collider>();

        DisableWeaponCollider();

        Debug.Log("Arma equipada: " + weapons[currentWeaponIndex].weaponObject.name);
    }

    public void BuyNextWeaponUpgrade()
    {
        int nextIndex = currentWeaponIndex + 1;

        if (nextIndex >= weapons.Length)
        {
            Debug.Log("Ya tienes la mejor arma");
            return;
        }

        int cost = weapons[nextIndex].costToUnlock;

        if (playerEnergy == null)
        {
            Debug.LogWarning("No hay PlayerEnergy en el Player");
            return;
        }

        if (playerEnergy.currentEnergy < cost)
        {
            Debug.Log("No tienes suficiente energía");
            return;
        }

        playerEnergy.ConsumeEnergy(cost);
        EquipWeapon(nextIndex);
    }

    public void EnableWeaponCollider()
    {
        if (currentWeaponCollider != null)
            currentWeaponCollider.enabled = true;
    }

    public void DisableWeaponCollider()
    {
        if (currentWeaponCollider != null)
            currentWeaponCollider.enabled = false;
    }

    public bool CanUnlockWeapon(int weaponIndex)
    {
        if (weaponIndex < 0 || weaponIndex >= weapons.Length)
        {
            Debug.Log("Índice de arma inválido: " + weaponIndex);
            return false;
        }

        if (weaponIndex != currentWeaponIndex + 1)
        {
            Debug.Log("Orden incorrecto. Arma actual: " + currentWeaponIndex + " / Intentando desbloquear: " + weaponIndex);
            return false;
        }

        int cost = weapons[weaponIndex].costToUnlock;

        if (cost <= 0)
            return true;

        if (playerEnergy == null)
        {
            Debug.LogWarning("No hay PlayerEnergy en el Player");
            return false;
        }

        if (playerEnergy.currentEnergy < cost)
        {
            Debug.Log("Energía insuficiente. Tienes: " + playerEnergy.currentEnergy + " / Necesitas: " + cost);
            return false;
        }

        return true;
    }
    public int GetWeaponCost(int weaponIndex)
    {
        if (weaponIndex < 0 || weaponIndex >= weapons.Length)
        {
            Debug.LogWarning("Índice de arma inválido");
            return 0;
        }

    return weapons[weaponIndex].costToUnlock;
}

    public void UnlockWeapon(int weaponIndex)
    {
        if (!CanUnlockWeapon(weaponIndex))
        {
            Debug.Log("No se puede desbloquear esta arma");
            return;
        }

        int cost = weapons[weaponIndex].costToUnlock;

        if (cost > 0 && playerEnergy != null)
            playerEnergy.ConsumeEnergy(cost);

        EquipWeapon(weaponIndex);

        Debug.Log("Arma desbloqueada: " + weapons[weaponIndex].weaponObject.name);
    }
}
