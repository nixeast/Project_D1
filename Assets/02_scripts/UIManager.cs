using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting.Antlr3.Runtime.Tree;

public enum eDataUpdateType
{
    Default = 0,
    Add = 1,
    Remove = 2,
}

public enum eDoomType
{
    PlagueOfContamination = 0,
    RiftStoneMutation = 1,
    SubterraneanTunnels = 2,
    SoulHarvest = 3,
    GearsOfCalamity = 4,
}

public class UIManager : MonoBehaviour
{
    public PlayerDataManager m_playerDataManager;

    [Header("Mision Data InFo")]
    public MissionDatabase m_missionDatabase;
    public int m_currentSelectedMissionNumber;
    public TMP_Text text_missionType;
    public TMP_Text text_missionTypeDesc;
    public TMP_Text text_missionName;
    public TMP_Text text_missionObjectiveTitle;
    public TMP_Text text_missionObjective;
    public Image img_missionDoomIcon;
    public TMP_Text text_missionDoomName;
    public RectTransform scrollViewContent_missionEnemyList;
    public List<GameObject> m_missionEnemyBtnList = new List<GameObject>();
    public GameObject prefab_enemyUnitCard;

    [Header("Unit Data Information")]
    public UnitDataBase m_unitDataBase;
    public TMP_Text tmp_currentUnitName;
    public TMP_Text tmp_currentUnitHealth;
    public TMP_Text tmp_currentUnitAttack;
    public TMP_Text tmp_currentUnitDefense;
    //public TMP_Text tmp_currentUnitMorale;

    public TMP_Text tmp_currentUnitHit;
    public TMP_Text tmp_currentUnitEvade;
    public TMP_Text tmp_currentUnitAp;

    public Image img_currentUnit_trait_01;
    public Image img_currentUnit_trait_02;
    public Image img_currentUnit_passive_01;
    public Image img_currentUnit_passive_02;
    public Image img_currentUnit_passive_03;
    public Image img_currentUnit_passive_04;
    public Image img_currentUnit_active_01;
    public Image img_currentUnit_active_02;
    public Image img_currentUnit_active_03;

    public Button btn_currentUnit_slotWeapon;
    public Button btn_currentUnit_slotArmor;
    public Button btn_currentUnit_slotAccessory_01;
    //public Button btn_currentUnit_slotAccessory_02;
    [SerializeField] private Sprite m_testSprite;
    public Image img_portrait;

    [Header("Unit Recruit")]
    public UnitRecruitDataBase m_unitRecruitDatabase;
    public GameObject panel_recruit;
    public GameObject prefab_recruitCard;
    public RecruitCard m_selectedRecruitCard;
    public string m_selectedRecruitUnitName;
    public bool isRecruitUnitSelected;
    public List<RecruitCard> m_recruitCardList = new List<RecruitCard>();
    public RectTransform scrollViewContent_recruit;

    [Header("Unit Card")]
    public GameObject panel_characterInfo;
    public RectTransform scrollViewContent_unitCard;
    public GameObject unitCardPrefab;
    public UnitCard m_selectedUnitCard;
    public List<GameObject> m_currentUnitCardList;
    public UnitPortraitDatabase m_unitPortraitDatabase;

    [Header("Storage")]
    public GameObject panel_storage;
    public RectTransform scrollViewContent_storage;
    public GameObject storageSlotPrefab;
    private StorageSlotButton m_currentSelectedStorageSlotBtn;
    private int m_currentStorageItemCount = 1;
    [SerializeField] private ItemSlotButton[] m_unitItemSlotButtons;
    //[SerializeField] private StorageSlotButton[] m_StorageSlotButtons;
    [SerializeField] private List<StorageSlotButton> m_StorageSlotButtons;

    [Header("Player Resources")]
    public TMP_Text tmp_value_dwarfGold;
    public TMP_Text tmp_value_dwarfHonor;
    public TMP_Text tmp_value_forgedEssence;

    [Header("Doom List")]
    public Doom[] m_doomList;

    private void Awake()
    {
        SubscribeSlotButton();
    }

    // Start is called before the first frame update
    void Start()
    {
        m_playerDataManager.CreateStarterUnits();
        LoadStorageItem();
        SubscribeStorageSlotButton();
        updateDwarfResourcesText();
        
        CreateRecruitCard();
        MakeRecruitID();
        RefreshUnitCard();

        //ShowCurrentUnitList();
        //TestDoomList();
        //TestDwarfResource();
    }

    public void OnClickMissionIcon(int nMissionNumber)
    {
        m_currentSelectedMissionNumber = nMissionNumber;
        GameRoot.s_instance.SetMissionNumber(nMissionNumber);
        ClearEnemyUnitCards();
        RefreshMissionInfoPanel();
    }

    public void ClearEnemyUnitCards()
    {
        int nCount = m_missionEnemyBtnList.Count;
        for(int i=0; i < nCount; i++)
        {
            Destroy(m_missionEnemyBtnList[i].gameObject);
        }
        m_missionEnemyBtnList.Clear();
    }

    public void RefreshMissionInfoPanel()
    {
        MissionData tempData = new MissionData();
        m_missionDatabase.m_missionDataDic.TryGetValue(m_currentSelectedMissionNumber, out tempData);
        text_missionType.text = tempData.m_missionTheme;
        text_missionTypeDesc.text = tempData.m_missionTheme;
        text_missionName.text = tempData.m_missionName;
        text_missionObjectiveTitle.text = tempData.m_missionName;
        text_missionObjective.text = tempData.m_missionName;
        text_missionDoomName.text = tempData.m_doomName;
        string path_doomIcon = tempData.m_doomIcon_path;
        img_missionDoomIcon.sprite = Resources.Load<Sprite>(path_doomIcon);

        int nEnemyCount = tempData.m_enemyTotalCount;
        for(int i=0; i<nEnemyCount; i++)
        {
            GameObject newEnemyCard = Instantiate<GameObject>(prefab_enemyUnitCard, scrollViewContent_missionEnemyList);
            int nTempID = int.Parse(tempData.m_enemyList_ID[i]);
            //Debug.Log(nTempID);

            EnemyUnitCard tempEnemyUnitCard = newEnemyCard.GetComponent<EnemyUnitCard>();
            tempEnemyUnitCard.m_enemyUnitID = int.Parse(tempData.m_enemyList_ID[i]);
            tempEnemyUnitCard.text_unitName.text = tempData.m_enemyList_ID[i];

            UnitData tempUnitData;
            m_unitDataBase.m_unitDataDic.TryGetValue(nTempID, out tempUnitData);
            
            string path_portrait = tempUnitData.m_PortraitPath;
            tempEnemyUnitCard.img_enemyPortrait.sprite = Resources.Load<Sprite>(path_portrait);
            
            m_missionEnemyBtnList.Add(newEnemyCard);
        }
        
        //enemyCard.transform.SetParent(scrollViewContent_missionEnemyList, false);
}
    
    public void ShowCurrentUnitList()
    {
        int nLength = m_playerDataManager.GetPlayerData().m_currentUnits.Count;
        for (int i = 0; i < nLength; i ++)
        {
            int nTemp;
            nTemp =  m_playerDataManager.GetPlayerData().m_currentUnits[i].m_unitOriginalNumber;
            Debug.Log("playerUnit0" + (i + 1).ToString() + " originalNumber : " + nTemp);
        }
    }
    public void TestDoomList()
    {
        m_doomList[0].IncreaseDoomValue(eDoomType.PlagueOfContamination, 76);
        m_doomList[0].UpdateDoom();
        
    }
    
    public void TestDwarfResource()
    {
        m_playerDataManager.AddDwarfHonor(200);
        
    }
    
    public void RecruitSelectedUnit()
    {
        bool bCostCheck = RecruitCostCheck();
        bool bSelectionCheck = RecruitSelectionCheck();

        if (bCostCheck == true && bSelectionCheck == true)
        {
            m_playerDataManager.CreateUnit(m_selectedRecruitCard);
            RefreshUnitCard();

            isRecruitUnitSelected = false;
            int nGoldCost = m_selectedRecruitCard.m_goldCost * -1;
            m_playerDataManager.AddDwarfGold(nGoldCost);
            m_selectedRecruitCard.image_soldOut.gameObject.SetActive(true);
            //Debug.Log("recruit success");
        }
    }

    public bool RecruitCostCheck()
    {
        int nPlayerGold = m_playerDataManager.GetPlayerData().nDwarfGold;
        int nRecruitGoldCost = m_selectedRecruitCard.m_goldCost;

        if (nPlayerGold >= nRecruitGoldCost)
        {
            return true;
        }
        else
        {
            Debug.Log("not enough gold");
            return false;
        }    
    }

    public bool RecruitSelectionCheck()
    {
        if (isRecruitUnitSelected == true)
        {
            return true;
        }
        else
        {
            Debug.Log("no selected recruitCard");
            return false;
        }
    }

    public void ResetRecruitCardList()
    {
        DeleteAllRecruitCards();
        CreateRecruitCard();
        MakeRecruitID();
    }

    public void DeleteAllRecruitCards()
    {
        if(m_recruitCardList.Count > 0)
        {
            int nCount = m_recruitCardList.Count;
            for(int i = 0; i < nCount; i++)
            {
                Destroy(m_recruitCardList[i].gameObject);
            }
            m_recruitCardList.Clear();
        }
    }

    public void MakeRecruitID()
    {
        int nData1 = 25;
        int nData2 = 25;
        int nData3 = 25;
        int nData4 = 25;

        int[] arrResult = new int[3];

        int nCount = 3;
        for (int i = 0; i < nCount; i++)
        {
            arrResult[i] = new int();
            int nRandNum = 0;
            nRandNum = Random.Range(0, 100);
            
            if(nRandNum < nData1)
            {
                arrResult[i] = 0;
            }
            else if(nRandNum < nData1+nData2)
            {
                arrResult[i] = 1;
            }
            else if(nRandNum < nData1+nData2+nData3)
            {
                arrResult[i] = 2;
            }
            else if(nRandNum < nData1+nData2+nData3+nData4)
            {
                arrResult[i] = 3;
            }
        }

        AssignRecruitCardInfo(arrResult[0], arrResult[1] + 10, arrResult[2] + 20);

    }

    public void AssignRecruitCardInfo(int recruit_01, int recruit_02, int recruit_03)
    {
        int[] nRecruitSlots = new int[3];
        
        for (int i = 0; i < 3; i++)
        {
            nRecruitSlots[i] = new int();
        }

        nRecruitSlots[0] = recruit_01;
        nRecruitSlots[1] = recruit_02;
        nRecruitSlots[2] = recruit_03;

        for (int i = 0; i < 3; i++)
        {
            UnitRecruitData tempData = m_unitRecruitDatabase.GetRecruitData(nRecruitSlots[i]);
            
            m_recruitCardList[i].m_recruitID = tempData.m_recruitID;
            m_recruitCardList[i].m_unitName = tempData.m_unitName;
            m_recruitCardList[i].m_unitType = tempData.m_unitType;
            m_recruitCardList[i].m_goldCost = tempData.m_cost_gold;
            m_recruitCardList[i].m_honorCost = tempData.m_cost_honor;
            m_recruitCardList[i].m_unitTrait = tempData.m_basicTraitName;
            m_recruitCardList[i].btn_portrait_unit.image.sprite = tempData.m_portrait_image;
            m_recruitCardList[i].image_icon_trait.sprite = tempData.m_basicTrait_image;

            m_recruitCardList[i].UpdateUI();

        }
        
            
    }

    public void CreateRecruitCard()
    {
        for (int i = 0; i < 3; i++)
        {
            GameObject cardObj = Instantiate(prefab_recruitCard);
            cardObj.transform.SetParent(scrollViewContent_recruit, false);
            RecruitCard recruitObj = cardObj.GetComponent<RecruitCard>();

            recruitObj.m_uiManager = this;
            m_recruitCardList.Add(recruitObj);
        }
        

    }
    
    public void ShowRecruitPanel()
    {
        panel_recruit.SetActive(true);
    }
    
    public void HideRecruitPanel()
    {
        panel_recruit.SetActive(false);
    }

    public void updateDwarfResourcesText()
    {
        tmp_value_dwarfGold.text = m_playerDataManager.GetPlayerData().nDwarfGold.ToString();
        tmp_value_dwarfHonor.text = m_playerDataManager.GetPlayerData().nDwarfHonor.ToString();
        tmp_value_forgedEssence.text = m_playerDataManager.GetPlayerData().nForgedEssence.ToString();
    }

    private void SubscribeSlotButton()
    {
        if (m_unitItemSlotButtons == null)
        {
            Debug.Log("SubscribeSlotButton failed..");
            return;
        }

        for(int i = 0; i < m_unitItemSlotButtons.Length; i++)
        {
            m_unitItemSlotButtons[i].AddOnClicked(OnSlotButtonClicked);
            //Debug.Log("SubscribeSlotButton success..");
        }
    }

    public void SubscribeStorageSlotButton()
    {
        if(m_StorageSlotButtons == null)
        {
            //Debug.Log("SubscribeStorageSlotButton failed..");
            return;
        }

        int nLength;
        nLength = m_StorageSlotButtons.Count;

        for (int i = 0; i < nLength; i++)
        {
            m_StorageSlotButtons[i].AddOnClicked(OnStorageSlotButtonClicked);
            //Debug.Log("SubscribeStorageSlotButton success..");
        }
    }

    private void OnStorageSlotButtonClicked(StorageSlotButton storageSlotButton)
    {
        Debug.Log("OnStorageSlotButtonClicked..");
        //m_unitItemSlotButtons[0].gameObject.SetActive(false);
        Color tempColor = m_unitItemSlotButtons[0].getButtonImage().color;
        tempColor.a = 0.3f;
        m_unitItemSlotButtons[0].getButtonImage().color = tempColor;
        m_currentSelectedStorageSlotBtn = storageSlotButton;
    }

    private void OnSlotButtonClicked(ItemSlotButton slotButton)
    {
        if(m_currentSelectedStorageSlotBtn != null)
        {
            //ApplySpriteToSlot(slotButton);
            if(slotButton.getIconSprite() != null)
            {
                m_playerDataManager.DeleteItem();
                slotButton.resetIconImage();
            }
            else
            {
                m_playerDataManager.GenerateItem(m_currentSelectedStorageSlotBtn);
                slotButton.setIconImage(m_testSprite);

                m_playerDataManager.RemoveItemFromStorageData(m_currentSelectedStorageSlotBtn.m_storageSlotNumber);
                Destroy(m_currentSelectedStorageSlotBtn.gameObject);
                m_currentSelectedStorageSlotBtn = null;
            }

            RefreshUnitStats();
            PlayerUnitDataUpdate();

            Debug.Log("copy storage item to unit");
            
            Debug.Log("clear currentSelectedStorageSlot");
            
            Color tempColor = m_unitItemSlotButtons[0].getButtonImage().color;
            tempColor.a = 1.0f;
            m_unitItemSlotButtons[0].getButtonImage().color = tempColor;
                
        }

        //test assign item to unitSlot
        // ApplySpriteToSlot(slotButton);
        // PlayerUnitDataUpdate();
    }

    public void AddStorageButtonToList(StorageSlotButton storageSlotBtn)
    {
        m_StorageSlotButtons.Add(storageSlotBtn);
        //Debug.Log("AddStorageButtonToList..");

    }

    public void RemoveStorageButtonFromList()
    {
        int nLastIndex;
        nLastIndex = m_StorageSlotButtons.Count - 1;
        GameObject removeTarget;
        removeTarget = m_StorageSlotButtons[nLastIndex].gameObject;

        m_StorageSlotButtons.RemoveAt(nLastIndex);
        Destroy(removeTarget);
        
    }

    private void ApplySpriteToSlot(ItemSlotButton slotButton)
    {
        if (slotButton.getIconSprite() != null)
        {
            m_playerDataManager.DeleteItem();
            slotButton.resetIconImage();
        }
        else if (slotButton.getIconSprite() == null)
        {
            m_playerDataManager.GenerateItem(m_currentSelectedStorageSlotBtn);
            slotButton.setIconImage(m_testSprite);
        }
        RefreshUnitStats();
    }

    private void PlayerUnitDataUpdate()
    {
        RefreshUnitStats();
    }


    public void gotoBattleMapScene()
    {
        Debug.Log("btn clicked");
        SceneManager.LoadScene("sc_01_battleMap");
    }

    public void gotoLobbyScene()
    {
        Debug.Log("btn clicked");
        SceneManager.LoadScene("sc_00_lobby");
    }

    public void ShowCharacterInfoPanel(UnitCard selectedUnitCard)
    {
        m_selectedUnitCard = selectedUnitCard;
        panel_characterInfo.gameObject.SetActive(true);
        tmp_currentUnitName.text = m_selectedUnitCard.m_unitName;
        RefreshUnitStats();
    }

    public void RefreshUnitStats()
    {
        if(m_selectedUnitCard != null)
        {
            tmp_currentUnitHealth.text = m_selectedUnitCard.m_unitSaveData.m_health.ToString();
            tmp_currentUnitDefense.text = m_selectedUnitCard.m_unitSaveData.m_defense.ToString();
            //tmp_currentUnitMorale.text = m_selectedUnitCard.m_unitSaveData.m_morale.ToString();
            tmp_currentUnitHit.text = m_selectedUnitCard.m_unitSaveData.m_unitOriginData.m_stat_HIT.ToString();
            tmp_currentUnitEvade.text = m_selectedUnitCard.m_unitSaveData.m_unitOriginData.m_stat_EVA.ToString();
            tmp_currentUnitAp.text = m_selectedUnitCard.m_unitSaveData.m_unitOriginData.m_stat_AP.ToString();

            int attackValueResult = m_selectedUnitCard.m_unitSaveData.m_attack;
            tmp_currentUnitAttack.text = attackValueResult.ToString();

            string tempPath_portrait = m_selectedUnitCard.m_unitSaveData.m_unitOriginData.m_PortraitPath;
            string tempPath_trait_01 = m_selectedUnitCard.m_unitSaveData.m_unitOriginData.m_trait_01_path;
            string tempPath_trait_02 = m_selectedUnitCard.m_unitSaveData.m_unitOriginData.m_trait_02_path;
            string tempPath_passive_01 = m_selectedUnitCard.m_unitSaveData.m_unitOriginData.m_passiveSkill_01_path;
            string tempPath_passive_02 = m_selectedUnitCard.m_unitSaveData.m_unitOriginData.m_passiveSkill_02_path;
            string tempPath_passive_03 = m_selectedUnitCard.m_unitSaveData.m_unitOriginData.m_passiveSkill_03_path;
            string tempPath_passive_04 = m_selectedUnitCard.m_unitSaveData.m_unitOriginData.m_passiveSkill_04_path;
            string tempPath_active_01 = m_selectedUnitCard.m_unitSaveData.m_unitOriginData.m_activeSkill_01_path;
            string tempPath_active_02 = m_selectedUnitCard.m_unitSaveData.m_unitOriginData.m_activeSkill_02_path;
            string tempPath_active_03 = m_selectedUnitCard.m_unitSaveData.m_unitOriginData.m_activeSkill_03_path;

            img_portrait.sprite = Resources.Load<Sprite>(tempPath_portrait);
            img_currentUnit_trait_01.sprite = Resources.Load<Sprite>(tempPath_trait_01);
            img_currentUnit_trait_02.sprite = Resources.Load<Sprite>(tempPath_trait_02);
            img_currentUnit_passive_01.sprite = Resources.Load<Sprite>(tempPath_passive_01);
            img_currentUnit_passive_02.sprite = Resources.Load<Sprite>(tempPath_passive_02);
            img_currentUnit_passive_03.sprite = Resources.Load<Sprite>(tempPath_passive_03);
            img_currentUnit_passive_04.sprite = Resources.Load<Sprite>(tempPath_passive_04);
            img_currentUnit_active_01.sprite = Resources.Load<Sprite>(tempPath_active_01);
            img_currentUnit_active_02.sprite = Resources.Load<Sprite>(tempPath_active_02);
            img_currentUnit_active_03.sprite = Resources.Load<Sprite>(tempPath_active_03);

            string tempPath_weapon = m_selectedUnitCard.m_unitSaveData.m_unitOriginData.m_equip_weapon_path;
            string tempPath_armor = m_selectedUnitCard.m_unitSaveData.m_unitOriginData.m_equip_armor_path;
            string tempPath_accessary = m_selectedUnitCard.m_unitSaveData.m_unitOriginData.m_equip_accessary_path;

            btn_currentUnit_slotWeapon.gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>(tempPath_weapon);
            btn_currentUnit_slotArmor.gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>(tempPath_armor);
            btn_currentUnit_slotAccessory_01.gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>(tempPath_accessary);

            // SetSpriteByItemName("longSword");

        }

    }

    public void SetSpriteByItemName(string itemName)
    {

        if(m_selectedUnitCard.m_unitSaveData.m_weapon == null)
        {
            //Debug.Log(m_selectedUnitCard.m_unitSaveData.m_weapon);
            btn_currentUnit_slotWeapon.GetComponent<ItemSlotButton>().setIconImage(null);
            return;
        }

        if (m_selectedUnitCard.m_unitSaveData.m_weapon.m_itemName == itemName)
        {
            btn_currentUnit_slotWeapon.GetComponent<ItemSlotButton>().setIconImage(m_testSprite);
            Debug.Log("find " + itemName);
        }
        else
        {
            btn_currentUnit_slotWeapon.GetComponent<ItemSlotButton>().setIconImage(null);
            Debug.Log("can't find " + itemName);
        }
    }

    public void HideCharacterInfoPanel()
    {
        panel_characterInfo.gameObject.SetActive(false);
        //panel_characterInfo.SetActive(false);
    }

    public void RefreshUnitCard()
    {
        PlayerData playerData = LoadUnitCradInfo();
        int unitCardCount = 0;
        unitCardCount = CountUnitCardLength(playerData);

        ClearUnitCards();
        CreateUnitCards(playerData, unitCardCount);
    }
    
    public void CreateUnitCards(PlayerData playerData, int unitCardCount)
    {
        for(int i=0; i<unitCardCount; i++)
        {
            GameObject cardObj = Instantiate(unitCardPrefab);
            m_currentUnitCardList.Add(cardObj);
            cardObj.transform.SetParent(scrollViewContent_unitCard, false);
            cardObj.GetComponent<UnitCard>().InitUnitCard(this, playerData.m_currentUnits[i].unitName, playerData.m_currentUnits[i]);
            //cardObj.GetComponent<UnitCard>().m_unitName = playerData.currentUnits[i].unitName;
            cardObj.GetComponent<UnitCard>().m_playerUnitNumber = i;
            cardObj.GetComponent<UnitCard>().text_playerUnitNumber.text = i.ToString();
        }
    }
    
    public void ClearUnitCards()
    {
        int nListCount = 0;
        nListCount = m_currentUnitCardList.Count;
        for (int i = 0; i < nListCount; i++)
        {
            Destroy(m_currentUnitCardList[i].gameObject);
        }
        m_currentUnitCardList.Clear();
    }
    
    public int CountUnitCardLength(PlayerData playerData)
    {
        if(playerData != null)
        {
            return playerData.m_currentUnits.Count;
        }
        else
        {
            return 0;
        }
    }
    
    public PlayerData LoadUnitCradInfo()
    {
        PlayerData playerData = m_playerDataManager.GetPlayerData();

        if(playerData != null)
        {
            
            return playerData;
        }
        else
        {
            Debug.Log("playerData is null..");
            return null;
        }
    }

    public void SetItemToUnit(int slotNumber)
    {

    }

    public void CreateStorageItem()
    {
        if(m_currentStorageItemCount < m_playerDataManager.GetPlayerData().m_storage.m_storageItem.Length)
        {

            Debug.Log("CreateStorageItem");

            GameObject itemObj = Instantiate(storageSlotPrefab);
            itemObj.transform.SetParent(scrollViewContent_storage,false);
            itemObj.GetComponent<StorageSlotButton>().m_storageSlotNumber = m_currentStorageItemCount;
            itemObj.GetComponent<StorageSlotButton>().m_storageSlotName = m_currentStorageItemCount.ToString();
            itemObj.GetComponent<StorageSlotButton>().InitStorageSlotButton(m_currentStorageItemCount);
            AddStorageButtonToList(itemObj.GetComponent<StorageSlotButton>());
            SubscribeStorageSlotButton();

            UpdateStorageSaveData(m_currentStorageItemCount, eDataUpdateType.Add);
            Debug.Log("Create storageSlots: " + m_currentStorageItemCount);
            m_currentStorageItemCount++;
        }
    }
    public void UpdateStorageSaveData(int slotNumnber, eDataUpdateType type)
    {
        if(type == eDataUpdateType.Add)
        {
            m_playerDataManager.AddItemToStorageData(slotNumnber);
        }
        else if(type == eDataUpdateType.Remove)
        {
            int nLastIndex = slotNumnber - 1;
            m_playerDataManager.RemoveItemFromStorageData(nLastIndex);
        }

    }

    public void ClearStorageSlotButtons()
    {
        
        int nLength = m_StorageSlotButtons.Count;
        for (int i = 0; i < nLength; i++)
        {
            Destroy(m_StorageSlotButtons[i].gameObject);
        }
        Debug.Log("storage cleared..");
        m_StorageSlotButtons.Clear();
        Debug.Log("m_StorageSlotButtons count: " + m_StorageSlotButtons.Count);
    }

    public void LoadStorageItem()
    {
        int nStorageLength;
        int nCount = 0;
        //int nStorageSlotNumber = 0;
        nStorageLength = m_playerDataManager.GetPlayerData().m_storage.m_storageItem.Length;
        //Debug.Log("LoadStorageItem length: " + nStorageLength);
        if(nStorageLength > 0)
        {
            for(int i=0; i < nStorageLength; i++)
            {
                if(m_playerDataManager.GetPlayerData().m_storage.m_storageItem[i] != null && m_playerDataManager.GetPlayerData().m_storage.m_storageItem[i].m_empty == false)
                {
                    GameObject itemObj = Instantiate(storageSlotPrefab);
                    itemObj.transform.SetParent(scrollViewContent_storage, false);
                    itemObj.GetComponent<StorageSlotButton>().m_storageSlotNumber = m_playerDataManager.GetPlayerData().m_storage.m_storageItem[i].m_itemNumber;
                    itemObj.GetComponent<StorageSlotButton>().m_storageSlotName = m_playerDataManager.GetPlayerData().m_storage.m_storageItem[i].m_itemName;
                    itemObj.GetComponent<StorageSlotButton>().InitStorageSlotButton(m_playerDataManager.GetPlayerData().m_storage.m_storageItem[i].m_itemNumber);
                    AddStorageButtonToList(itemObj.GetComponent<StorageSlotButton>());
                    //nStorageSlotNumber++;
                    nCount++;
                    //Debug.Log("slot Num: " + i);
                }
            }
            m_currentStorageItemCount = nCount;
            //Debug.Log("Create storageSlots: " + m_currentStorageItemCount);
        }
    }

    public void RefreshStorageItem()
    {

    }

    public void DeleteStorageItem()
    {
        if(m_currentStorageItemCount > 0)
        {
            Debug.Log("DeleteStorageItem");
            UpdateStorageSaveData(m_currentStorageItemCount, eDataUpdateType.Remove);
            RemoveStorageButtonFromList();
            m_currentStorageItemCount--;
        }
        else
        {
            Debug.Log("no storage item..");
        }
    }

    public void HideStoragePanel()
    {
        panel_storage.SetActive(false);
    }

    public void ShowStoragePanel()
    {
        panel_storage.SetActive(true);
        //LoadStorageItem();
    }

    public void PrintStorageSlotInfo()
    {

    }
}
