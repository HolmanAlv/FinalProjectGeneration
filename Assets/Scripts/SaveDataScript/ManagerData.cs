using UnityEngine;
using System.IO;

public class ManagerData : MonoBehaviour
{
    public PlayerEnergy playerEnergy;
    public PlayerResources playerResources;
    public DayNiightManager dayNiightManager;

    public GameObject structure1;
    public StructureHealth structureHealth1;
    
    public GameObject structure2;
    public StructureHealth structureHealth2;
    
    public GameObject structure3;
    public StructureHealth structureHealth3;
    
    public GameObject structure4;
    public StructureHealth structureHealth4;

    public MainHealth mainHealth;

    private string saveFile;
    private DataGame dataGame = new DataGame();

    void Awake()
    {
        saveFile = Application.dataPath + "/dataGame.json";
        
        if (NewOrLoad.Instance != null && NewOrLoad.Instance.loadGame == true)
        {
            LoadDataGame();
        }
    }

    [ContextMenu("Cargar datos")]
    public void LoadDataGame()
    {
        if (File.Exists(saveFile))
        {
            string content = File.ReadAllText(saveFile);
            dataGame = JsonUtility.FromJson<DataGame>(content);

            playerEnergy.currentEnergy = dataGame.numberPower;
            playerResources.wood = dataGame.numberWood;
            playerResources.stone = dataGame.numberStone;
            dayNiightManager.completedNights = dataGame.numberDay;

            structure1.SetActive(dataGame.activeStructure1);
            structureHealth1.currentHealth = dataGame.lifeStructure1;
            
            structure2.SetActive(dataGame.activeStructure2);
            structureHealth2.currentHealth = dataGame.lifeStructure2;
            
            structure3.SetActive(dataGame.activeStructure3);
            structureHealth3.currentHealth = dataGame.lifeStructure3;

            structure4.SetActive(dataGame.activeStructure4);
            structureHealth4.currentHealth = dataGame.lifeStructure4;

            mainHealth.currentHealth = dataGame.currentMainHealth;

            Debug.Log("Datos cargados correctamente");
        }
        else
        {
            Debug.Log("No existe archivo de guardado");
        }
    }

    [ContextMenu("Guardar datos")]
    public void SaveDataGame()
    {
        dataGame.numberPower = playerEnergy.currentEnergy;
        dataGame.numberWood = playerResources.wood;
        dataGame.numberStone = playerResources.stone;
        dataGame.numberDay = dayNiightManager.completedNights;

        dataGame.activeStructure1 = structure1.activeSelf;
        dataGame.lifeStructure1 = structureHealth1.currentHealth;
        
        dataGame.activeStructure2 = structure2.activeSelf;
        dataGame.lifeStructure2 = structureHealth2.currentHealth;
        
        dataGame.activeStructure3 = structure3.activeSelf;
        dataGame.lifeStructure3 = structureHealth3.currentHealth;
        
        dataGame.activeStructure4 = structure4.activeSelf;
        dataGame.lifeStructure4 = structureHealth4.currentHealth;

        dataGame.currentMainHealth = mainHealth.currentHealth;

        string chainJSON = JsonUtility.ToJson(dataGame);
        File.WriteAllText(saveFile, chainJSON);
        Debug.Log("Partida guardada");
    }
}