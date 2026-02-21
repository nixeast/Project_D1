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
public class UIManager : MonoBehaviour
{

    public GameObject panel_characterInfo;
    public GameObject panel_storage;
    public RectTransform scrollViewContent_unitCard;
    public RectTransform scrollViewContent_storage;
    public GameObject unitCardPrefab;
    public GameObject storageSlotPrefab;
    public PlayerDataManager m_playerDataManager;
    public UnitPortraitDatabase m_unitPortraitDatabase;
    public UnitCard m_selectedUnitCard;
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

    private void Awake()
    {
        SubscribeSlotButton();
        
    }

    // Start is called before the first frame update
    void Start()
    {
        LoadStorageItem();
        SubscribeStorageSlotButton();
    }

    // Update is called once per frame
    void Update()
    {
        
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
            Debug.Log("SubscribeSlotButton success..");
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
            Debug.Log("SubscribeStorageSlotButton success..");
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
        Debug.Log("AddStorageButtonToList..");
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
        PlayerData playerData = m_playerDataManager.GetPlayerData();
        
        int unitCardCount = 0;

        if(playerData != null)
        {
            unitCardCount = playerData.currentUnits.Length;
            Debug.Log("unitCardCount: " + unitCardCount);
        }
        else
        {
            Debug.Log("playerData is null..");
        }

        for(int i=0; i<unitCardCount; i++)
        {
            GameObject cardObj = Instantiate(unitCardPrefab);
            cardObj.transform.SetParent(scrollViewContent_unitCard, false);
            cardObj.GetComponent<UnitCard>().InitUnitCard(this, playerData.currentUnits[i].unitName, playerData.currentUnits[i]);
            //cardObj.GetComponent<UnitCard>().m_unitName = playerData.currentUnits[i].unitName;
            cardObj.GetComponent<UnitCard>().m_playerUnitNumber = i;
            cardObj.GetComponent<UnitCard>().text_playerUnitNumber.text = i.ToString();

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
        Debug.Log("LoadStorageItem length: " + nStorageLength);
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
                    Debug.Log("slot Num: " + i);
                }
            }
            m_currentStorageItemCount = nCount;
            Debug.Log("Create storageSlots: " + m_currentStorageItemCount);
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
