using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

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

    public GameObject panel_characterInfo;
    public GameObject panel_storage;

    public UnitRecruitDataBase m_unitRecruitDatabase;
    public GameObject panel_recruit;
    public GameObject prefab_recruitCard;
    public RecruitCard m_selectedRecruitCard;
    public string m_selectedRecruitUnitName;
    public bool isRecruitUnitSelected;
    public List<RecruitCard> m_recruitCardList = new List<RecruitCard>();

    public RectTransform scrollViewContent_unitCard;
    public RectTransform scrollViewContent_storage;
    public RectTransform scrollViewContent_recruit;
    public GameObject unitCardPrefab;
    
    
    public GameObject storageSlotPrefab;
    public PlayerDataManager m_playerDataManager;
    public UnitPortraitDatabase m_unitPortraitDatabase;
    public UnitCard m_selectedUnitCard;
    public List<GameObject> m_currentUnitCardList;
    private StorageSlotButton m_currentSelectedStorageSlotBtn;

    public TMP_Text tmp_currentUnitName;
    public TMP_Text tmp_currentUnitHealth;
    public TMP_Text tmp_currentUnitAttack;
    public TMP_Text tmp_currentUnitDefense;
    public TMP_Text tmp_currentUnitMorale;

    public Button btn_slotWeapon;
    public Button btn_slotArmor;
    public Button btn_slotAccessory_01;
    public Button btn_slotAccessory_02;

    [SerializeField] private Sprite m_testSprite;
    [SerializeField] private ItemSlotButton[] m_unitItemSlotButtons;
    //[SerializeField] private StorageSlotButton[] m_StorageSlotButtons;
    [SerializeField] private List<StorageSlotButton> m_StorageSlotButtons;

    private int m_currentStorageItemCount = 1;

    public Doom[] m_doomList;

    public TMP_Text tmp_value_dwarfGold;
    public TMP_Text tmp_value_dwarfHonor;
    public TMP_Text tmp_value_forgedEssence;

    private void Awake()
    {
        SubscribeSlotButton();
    }

    // Start is called before the first frame update
    void Start()
    {
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
            m_playerDataManager.CreateUnit();
            RefreshUnitCard();

            isRecruitUnitSelected = false;

            int nGoldCost = m_selectedRecruitCard.m_goldCost * -1;
            m_playerDataManager.AddDwarfGold(nGoldCost);

            m_selectedRecruitCard.image_soldOut.gameObject.SetActive(true);

            Debug.Log("recruit success");
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

        }
        
        //match info by checking recruitID
        
            // recruitObj.m_unitName = i.ToString();
            // recruitObj.m_unitType = "warrior";
            // recruitObj.m_unitTrait = "trait_" + i.ToString();
            // recruitObj.m_goldCost = i;
            // recruitObj.m_honorCost = i;

            // recruitObj.text_unitName.text = recruitObj.m_unitName;
            // recruitObj.text_unitType.text = recruitObj.m_unitType;
            // recruitObj.text_unitTrait.text = recruitObj.m_unitTrait;
            // recruitObj.text_goldCost.text = recruitObj.m_goldCost.ToString();
            // recruitObj.text_honorCost.text = recruitObj.m_honorCost.ToString();

            // string path = "UnitPortraits/" + "unit_dwarf_01";
            // Sprite newSprite = Resources.Load<Sprite>(path);
            // recruitObj.btn_portrait_unit.image.sprite = newSprite;
            
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
            Debug.Log("SubscribeStorageSlotButton failed..");
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
            tmp_currentUnitMorale.text = m_selectedUnitCard.m_unitSaveData.m_morale.ToString();

            int attackValueResult;
            if(m_selectedUnitCard.m_unitSaveData.m_weapon != null)
            {
                attackValueResult = m_selectedUnitCard.m_unitSaveData.m_attack + m_selectedUnitCard.m_unitSaveData.m_weapon.m_attackValue;
            }
            else
            {
                attackValueResult = m_selectedUnitCard.m_unitSaveData.m_attack;
            }
            tmp_currentUnitAttack.text = attackValueResult.ToString();

            //Debug.Log("m_selectedUnitCard: " + m_selectedUnitCard.name);

            SetSpriteByItemName("longSword");
            
        }

    }

    public void SetSpriteByItemName(string itemName)
    {

        if(m_selectedUnitCard.m_unitSaveData.m_weapon == null)
        {
            btn_slotWeapon.GetComponent<ItemSlotButton>().setIconImage(null);
            return;
        }

        if (m_selectedUnitCard.m_unitSaveData.m_weapon.m_itemName == itemName)
        {
            //slotButton.setIconImage(m_testSprite);
            btn_slotWeapon.GetComponent<ItemSlotButton>().setIconImage(m_testSprite);
            Debug.Log("find " + itemName);
        }
        else
        {
            btn_slotWeapon.GetComponent<ItemSlotButton>().setIconImage(null);
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
