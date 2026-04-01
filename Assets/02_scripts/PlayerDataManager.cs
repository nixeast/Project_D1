using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager s_instance;

    [SerializeField] UIManager m_uiManager;
    private PlayerData playerData;
    private string savePath;
    public int m_maxUnitNumber;
    public UnitDataBase m_unitDataBase;

    public void Awake()
    {
        MakeSingletonPattern();

        savePath = Path.Combine(Application.persistentDataPath,"playerData.json");

        playerData = new PlayerData();
        InintDwarfResourcesData();
        CreateStarterStorage();

        m_maxUnitNumber = 20;
        //CreateStarterUnits();

    }

    public void MakeSingletonPattern()
    {
        if(s_instance != null && s_instance != this)
        {
            Destroy(gameObject);
            return;
        }
        s_instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void InintDwarfResourcesData()
    {
        playerData.nDwarfGold = 3;
        playerData.nDwarfHonor = 100;
        playerData.nForgedEssence = 100;
    }

    public void AddDwarfGold(int nGold)
    {
        playerData.nDwarfGold += nGold;
        m_uiManager.updateDwarfResourcesText();
    }

    public void AddDwarfHonor(int nHonor)
    {
        playerData.nDwarfHonor += nHonor;
        m_uiManager.updateDwarfResourcesText();
    }

    public void AddForgedEssence(int nForgedEssence)
    {
        playerData.nForgedEssence += nForgedEssence;
        m_uiManager.updateDwarfResourcesText();
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
    
    public void CreateUnit(RecruitCard selectedRecruitCard)
    {
        //int nCurrentUnitsLength = 0;
        //nCurrentUnitsLength = playerData.m_currentUnits.Count;

        UnitSaveData newData = new UnitSaveData();
        newData.m_unitOriginData = new UnitData();

        int nUnitID = selectedRecruitCard.m_recruitID;
        bool isOutSuccess = m_unitDataBase.m_unitDataDic.TryGetValue(nUnitID, out newData.m_unitOriginData);

        if (isOutSuccess == true)
        {
            newData.unitName = newData.m_unitOriginData.m_UnitName;
            newData.level = newData.m_unitOriginData.m_level;
            newData.upgrade = 0;
            newData.m_unitOriginalNumber = newData.m_unitOriginData.m_UnitID;

            newData.m_weapon = new Item();
            newData.m_weapon.m_itemName = "longSword";
            newData.m_armor = null;
            newData.m_charm_01 = null;
            newData.m_charm_02 = null;

            newData.m_health = newData.m_unitOriginData.m_stat_HP;
            newData.m_attack = newData.m_unitOriginData.m_stat_ATK;
            newData.m_defense = newData.m_unitOriginData.m_stat_DEF;
            newData.m_morale = 1;

            playerData.m_currentUnits.Add(newData);

        }
        else
        {
            Debug.Log("tryGetValue from m_unitDataDic failed..");
        }

        //int nUnitSpriteNumber = 9;
        //int nUnitLevelNumber = 9;
        //int nUnitUpgradeNumber = 9;

        //newData.unitName = "Dwarf_0" + nUnitSpriteNumber.ToString();
        //newData.level = nUnitLevelNumber;
        //newData.upgrade = nUnitUpgradeNumber;
        //newData.m_unitOriginalNumber = 9;

        //newData.m_weapon = null;
        //newData.m_armor = null;
        //newData.m_charm_01 = null;
        //newData.m_charm_02 = null;

        //newData.m_health = 13;
        //newData.m_attack = 13;
        //newData.m_defense = 13;
        //newData.m_morale = 13;
    }
    
    public void InitUnitArray()
    {
        //playerData.currentUnits = new UnitSaveData[m_maxUnitNumber];

    }
    
    public void CreateStarterUnits()
    {
        int nCount = 4;

        for (int i = 0; i < nCount; i++)
        {
            UnitSaveData newData = new UnitSaveData();
            newData.m_unitOriginData = new UnitData();
            //UnitData tempData;

            bool isOutSuccess = m_unitDataBase.m_unitDataDic.TryGetValue(i, out newData.m_unitOriginData);
            
            if(isOutSuccess == true)
            {
                newData.unitName = newData.m_unitOriginData.m_UnitName;
                newData.level = newData.m_unitOriginData.m_level;
                newData.upgrade = 0;
                newData.m_unitOriginalNumber = newData.m_unitOriginData.m_UnitID;
                
                newData.m_weapon = new Item();
                newData.m_weapon.m_itemName = "longSword";
                newData.m_armor = null;
                newData.m_charm_01 = null;
                newData.m_charm_02 = null;

                newData.m_health = newData.m_unitOriginData.m_stat_HP;
                newData.m_attack = newData.m_unitOriginData.m_stat_ATK;
                newData.m_defense = newData.m_unitOriginData.m_stat_DEF;
                newData.m_morale = 1;
            
                playerData.m_currentUnits.Add(newData);
            
            }
            else
            {
                Debug.Log("tryGetValue from m_unitDataDic failed..");
            }
        }
    }

    public PlayerData GetPlayerData()
    {
        return playerData;
    }

    public int GetGold()
    {
        return playerData.nDwarfGold;
    }

    public void SetGold(int value)
    {
        playerData.nDwarfGold = value;
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
        playerData.nDwarfGold += value;
        Debug.Log("[Gold]" + playerData.nDwarfGold);
    }

    public void ShowData()
    {
        Debug.Log("[Current Gold] : " + playerData.nDwarfGold);
        Debug.Log("[Current Unit length] : " + playerData.m_currentUnits.Count);
        // Debug.Log("[Current Unit 1] : " + playerData.currentUnits[0].unitName);
        // Debug.Log("[Current Unit 2] : " + playerData.currentUnits[1].unitName);
        // Debug.Log("[Current Unit 3] : " + playerData.currentUnits[2].unitName);
        // Debug.Log("[Current Unit 4] : " + playerData.currentUnits[3].unitName);
    }

    public void GenerateItem(StorageSlotButton currentStorageSlot)
    {
        // playerData.currentUnits[0].m_weapon = new Item();
        // playerData.currentUnits[0].m_weapon.m_itemName = currentStorageSlot.m_storageSlotName;
        // playerData.currentUnits[0].m_weapon.m_attackValue = 45;
    }

    public void DeleteItem()
    {
        //playerData.currentUnits[0].m_weapon = null;
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
