using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    [SerializeField] UIManager m_uiManager;
    private PlayerData playerData;
    private string savePath;

    void Awake()
    {
        savePath = Path.Combine(Application.persistentDataPath,"playerData.json");

        playerData = new PlayerData();
        playerData.nGold = 0;
        
        playerData.m_storage = new StorageSaveData();
        playerData.m_storage.m_storageItem = new Item[10];
        playerData.m_storage.m_storageItem[0] = new Item();
        playerData.m_storage.m_storageItem[0].m_itemNumber = 0;
        playerData.m_storage.m_storageItem[0].InitItem();


        playerData.currentUnits = new UnitSaveData[4];

        playerData.currentUnits[0] = new UnitSaveData();
        playerData.currentUnits[0].unitName = "Dwarf_01";
        playerData.currentUnits[0].level = 1;
        playerData.currentUnits[0].upgrade = 1;

        playerData.currentUnits[0].m_weapon = null;
        playerData.currentUnits[0].m_armor = null;
        playerData.currentUnits[0].m_charm_01 = null;
        playerData.currentUnits[0].m_charm_02 = null;

        playerData.currentUnits[0].m_health = 10;
        playerData.currentUnits[0].m_attack = 10;
        playerData.currentUnits[0].m_defense = 10;
        playerData.currentUnits[0].m_morale = 10;

        playerData.currentUnits[1] = new UnitSaveData();
        playerData.currentUnits[1].unitName = "Dwarf_02";
        playerData.currentUnits[1].level = 2;
        playerData.currentUnits[1].upgrade = 2;

        playerData.currentUnits[1].m_weapon = null;
        playerData.currentUnits[1].m_armor = null;
        playerData.currentUnits[1].m_charm_01 = null;
        playerData.currentUnits[1].m_charm_02 = null;

        playerData.currentUnits[1].m_health = 11;
        playerData.currentUnits[1].m_attack = 11;
        playerData.currentUnits[1].m_defense = 11;
        playerData.currentUnits[1].m_morale = 11;

        playerData.currentUnits[2] = new UnitSaveData();
        playerData.currentUnits[2].unitName = "Dwarf_03";
        playerData.currentUnits[2].level = 3;
        playerData.currentUnits[2].upgrade = 3;

        playerData.currentUnits[2].m_weapon = null;
        playerData.currentUnits[2].m_armor = null;
        playerData.currentUnits[2].m_charm_01 = null;
        playerData.currentUnits[2].m_charm_02 = null;

        playerData.currentUnits[2].m_health = 12;
        playerData.currentUnits[2].m_attack = 12;
        playerData.currentUnits[2].m_defense = 12;
        playerData.currentUnits[2].m_morale = 12;

        playerData.currentUnits[3] = new UnitSaveData();
        playerData.currentUnits[3].unitName = "Dwarf_04";
        playerData.currentUnits[3].level = 4;
        playerData.currentUnits[3].upgrade = 4;

        playerData.currentUnits[3].m_weapon = null;
        playerData.currentUnits[3].m_armor = null;
        playerData.currentUnits[3].m_charm_01 = null;
        playerData.currentUnits[3].m_charm_02 = null;

        playerData.currentUnits[3].m_health = 13;
        playerData.currentUnits[3].m_attack = 13;
        playerData.currentUnits[3].m_defense = 13;
        playerData.currentUnits[3].m_morale = 13;

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
        // playerData.currentUnits[3].unitName = "mage";
        // playerData.currentUnits[3].level = 2;
        // playerData.currentUnits[3].upgrade = 2;

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

        m_uiManager.ClearStorageSlotButtons();
        m_uiManager.LoadStorageItem();

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

    public void GenerateItem()
    {
        playerData.currentUnits[0].m_weapon = new Item();
        playerData.currentUnits[0].m_weapon.m_itemName = "longSword";
        playerData.currentUnits[0].m_weapon.m_attackValue = 45;
    }

    public void DeleteItem()
    {
        playerData.currentUnits[0].m_weapon = null;
    }

    public void AddItemToStorageData(int storageSlotNumber)
    {
        playerData.m_storage.m_storageItem[storageSlotNumber] = new Item();
        playerData.m_storage.m_storageItem[storageSlotNumber].m_itemNumber = storageSlotNumber;
        playerData.m_storage.m_storageItem[storageSlotNumber].m_itemName = "test" + storageSlotNumber.ToString();
        playerData.m_storage.m_storageItem[storageSlotNumber].InitItem();

        //playerData.m_storage.m_storageItem[storageSlotNumber].m_itemNumber = storageSlotNumber;
        //playerData.m_storage.m_storageItem[storageSlotNumber].InitItem();
    }

    public void RemoveItemFromStorageData(int storageSlotNumber)
    {
        //playerData.m_storage.m_storageItem[storageSlotNumber].InitItem();
        playerData.m_storage.m_storageItem[storageSlotNumber].m_empty = true;

    }

}
