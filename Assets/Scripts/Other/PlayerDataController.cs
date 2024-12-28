using System.Collections.Generic;
using UnityEngine;

public class PlayerDataController
{
    private const string PLAYER_DATA = "playerdata";
    public static int CurrentCoin { get; private set; }
    public static int CoinToUpgrade { get; private set; }
    public static float CurrentSpeed {  get; private set; }

    public static void InitPlayerData()
    {
        if (!PlayerPrefs.HasKey(PLAYER_DATA))
        {
            PlayerData playerData = new PlayerData();
            playerData.currentCoin = 100;
            playerData.currentSpeed = 2f;
            playerData.speedIncrease = 0.2f;
            playerData.coinToUpgradeSpeed = 50;
            playerData.coinIncrease = 25;
            CurrentSpeed = playerData.currentSpeed;
            CurrentCoin = playerData.currentCoin;
            CoinToUpgrade = playerData.coinToUpgradeSpeed;
            PlayerPrefs.SetString(PLAYER_DATA, JsonUtility.ToJson(playerData));
        }
        else
        {
            string data = PlayerPrefs.GetString(PLAYER_DATA);
            PlayerData playerData = JsonUtility.FromJson<PlayerData>(data);
            CurrentSpeed = playerData.currentSpeed;
            CurrentCoin = playerData.currentCoin;
            CoinToUpgrade = playerData.coinToUpgradeSpeed;
        }
    }



    public static void UpdateCoin(int addedCoin)
    {
        string data = PlayerPrefs.GetString(PLAYER_DATA);
        PlayerData playerData = JsonUtility.FromJson<PlayerData>(data);
        int newCoin = playerData.currentCoin + addedCoin;
        playerData.currentCoin = newCoin;
        CurrentCoin = newCoin;
        PlayerPrefs.SetString(PLAYER_DATA, JsonUtility.ToJson(playerData));
    }


    public static bool CanUpgradeSpeed()
    {
        string data = PlayerPrefs.GetString(PLAYER_DATA);
        PlayerData playerData = JsonUtility.FromJson<PlayerData>(data);
        return CurrentCoin >= playerData.coinToUpgradeSpeed;
    }


    public static void UpgradeSpeed()
    {
        string data = PlayerPrefs.GetString(PLAYER_DATA);
        PlayerData playerData = JsonUtility.FromJson<PlayerData>(data);

        playerData.currentSpeed += playerData.speedIncrease;
        CurrentSpeed = playerData.currentSpeed;

        playerData.currentCoin -= playerData.coinToUpgradeSpeed;
        CurrentCoin = playerData.currentCoin;

        playerData.coinToUpgradeSpeed += playerData.coinIncrease;
        CoinToUpgrade = playerData.coinToUpgradeSpeed;

        PlayerPrefs.SetString(PLAYER_DATA, JsonUtility.ToJson(playerData));
    }
}
