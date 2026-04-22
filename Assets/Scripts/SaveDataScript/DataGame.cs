using System;

[System.Serializable]
public class DataGame
{
    public int numberPower;
    public int numberWood;
    public int numberStone;
    public int numberDay;
    public int numberNight;

    public bool activeStructure1;
    public bool activeStructure2;
    public bool activeStructure3;
    public bool activeStructure4;
    
    public float lifeStructure1;
    public float lifeStructure2;
    public float lifeStructure3;
    public float lifeStructure4;

    public float currentMainHealth;
}