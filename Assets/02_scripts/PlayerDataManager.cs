using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    private PlayerData playerData;
    private string savePath;

    void Awake()
    {
        savePath = Path.Combine(Application.persistentDataPath,"playerdata.json");

        playerData = new PlayerData();
        playerData.nGold = 0;
    }

    public PlayerData GetPlayerData()
    {
        return playerData;
    }

    public int GetGold()
    {
        return playerData.nGold;
    }

    public void SetGold(int value)
    {
        playerData.nGold = value;
    }

    public string GetSavePath()
    {
        return savePath;
    }

    public void SavePlayerData()
    {
        string json = JsonUtility.ToJson(playerData, true);
        File.WriteAllText(savePath,json);
        Debug.Log("[save] " + savePath + "\n" + json);
    }

    public void LoadPlayerData()
    {
        if(File.Exists(savePath) == false)
        {
            Debug.Log("[load] No save file : " + savePath);
        }

        string json = File.ReadAllText(savePath);
        playerData = JsonUtility.FromJson<PlayerData>(json);
        Debug.Log("[load]" + savePath + "\n" + json);

    }

    public void AddGold(int value)
    {
        playerData.nGold += value;
        Debug.Log("[Gold]" + playerData.nGold);
    }

}
