using TMPro;
using UnityEngine;


public class PlayerResources : MonoBehaviour
{
    public int wood = 0;
    public int stone = 0;

    public UIManager uIManager;

    public void Start()
    {
        uIManager.UpdateWood(wood);
        uIManager.UpdateStone(stone);
    }

    public void AddWood(int amount)
    {
        AudioManager.Instance.PlaySFX("collect_wood");
        wood += amount;
        uIManager.UpdateWood(wood);
        Debug.Log("cantidad de madera:" + wood);
    }

    public void AddStone(int amount)
    {
        AudioManager.Instance.PlaySFX("collect_stone");
        stone += amount;
        uIManager.UpdateStone(stone);

        Debug.Log("cantidad de piedra" + stone);
    }

    public bool ConsumeResources(int woodAmount, int stoneAmount)
    {
        if(wood < woodAmount ||stone < stoneAmount)
        return false;

        wood -= woodAmount;
        stone-= stoneAmount;

        uIManager.UpdateWood(wood);
        uIManager.UpdateStone(stone);

        return true;
    }








}
