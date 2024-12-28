using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class PlayerData 
{
    public int currentCoin = 100;
    public float currentSpeed = 5f;
    public float speedIncrease = 0.5f;
    public int coinToUpgradeSpeed = 50;
    public int coinIncrease = 25;
}
