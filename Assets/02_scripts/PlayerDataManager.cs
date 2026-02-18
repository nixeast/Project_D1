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
        savePath = Path.Combine(Application.persistentDataPath,"playerData.json");

        playerData = new PlayerData();
        playerData.nGold = 0;
        playerData.currentUnits = new UnitSaveData[4];

        playerData.currentUnits[0] = new UnitSaveData();
        playerData.currentUnits[0].unitName = "knight1";
        playerData.currentUnits[0].level = 1;
        playerData.currentUnits[0].upgrade = 1;

        playerData.currentUnits[1] = new UnitSaveData();
        playerData.currentUnits[1].unitName = "knight2";
        playerData.currentUnits[1].level = 2;
        playerData.currentUnits[1].upgrade = 2;

        playerData.currentUnits[2] = new UnitSaveData();
        playerData.currentUnits[2].unitName = "knight3";
        playerData.currentUnits[2].level = 3;
        playerData.currentUnits[2].upgrade = 3;

        playerData.currentUnits[3] = new UnitSaveData();
        playerData.currentUnits[3].unitName = "knight4";
        playerData.currentUnits[3].level = 4;
        playerData.currentUnits[3].upgrade = 4;

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
        //playerData.currentUnits[3] = new UnitSaveData();
        playerData.currentUnits[3].unitName = "mage";
        playerData.currentUnits[3].level = 2;
        playerData.currentUnits[3].upgrade = 2;

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

    public void DeletePlayerData()
    {
        if (File.Exists(savePath) != false)
        {
            File.Delete(savePath);
            Debug.Log("save file deleted " + savePath);
        }

    }

    public void AddGold(int value)
    {
        playerData.nGold += value;
        Debug.Log("[Gold]" + playerData.nGold);
    }

    public void ShowData()
    {
        Debug.Log("[Current Gold] : " + playerData.nGold);
        Debug.Log("[Current Unit length] : " + playerData.currentUnits.Length);
        Debug.Log("[Current Unit 1] : " + playerData.currentUnits[0].unitName);
        Debug.Log("[Current Unit 2] : " + playerData.currentUnits[1].unitName);
        Debug.Log("[Current Unit 3] : " + playerData.currentUnits[2].unitName);
        Debug.Log("[Current Unit 4] : " + playerData.currentUnits[3].unitName);
    }

}
