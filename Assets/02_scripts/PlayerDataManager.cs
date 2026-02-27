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

        CreateStarterStorage();
        CreateStarterUnits();

    }
    
    public void CreateStarterStorage()
    {
        playerData.m_storage = new StorageSaveData();
        playerData.m_storage.m_storageItem = new Item[10];
        playerData.m_storage.m_storageItem[0] = new Item();
        playerData.m_storage.m_storageItem[0].m_itemNumber = 0;
        playerData.m_storage.m_storageItem[0].m_itemName = "longSword";
        playerData.m_storage.m_storageItem[0].InitItem();
    }
    
    public void CreateStarterUnits()
    {
        int nCount = 4;
        
        playerData.currentUnits = new UnitSaveData[nCount];

        for (int i = 0; i < nCount; i++)
        {
            int nUnitSpriteNumber = i + 1;
            int nUnitLevelNumber = i + 1;
            int nUnitUpgradeNumber = i + 1;
            
            playerData.currentUnits[i] = new UnitSaveData();
            playerData.currentUnits[i].unitName = "Dwarf_0" + nUnitSpriteNumber.ToString();
            playerData.currentUnits[i].level = nUnitLevelNumber;
            playerData.currentUnits[i].upgrade = nUnitUpgradeNumber;
            playerData.currentUnits[i].m_unitOriginalNumber = i;

            playerData.currentUnits[i].m_weapon = null;
            playerData.currentUnits[i].m_armor = null;
            playerData.currentUnits[i].m_charm_01 = null;
            playerData.currentUnits[i].m_charm_02 = null;

            playerData.currentUnits[i].m_health = 10;
            playerData.currentUnits[i].m_attack = 10;
            playerData.currentUnits[i].m_defense = 10;
            playerData.currentUnits[i].m_morale = 10;
        }

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
        m_uiManager.SubscribeStorageSlotButton();

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

    public void GenerateItem(StorageSlotButton currentStorageSlot)
    {
        playerData.currentUnits[0].m_weapon = new Item();
        playerData.currentUnits[0].m_weapon.m_itemName = currentStorageSlot.m_storageSlotName;
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
        playerData.m_storage.m_storageItem[storageSlotNumber].m_itemName = "";

    }

}
