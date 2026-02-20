using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public GameObject panel_characterInfo;
    public RectTransform scrollViewContent_unitCard;
    public GameObject unitCardPrefab;
    public PlayerDataManager m_playerDataManager;
    public UnitPortraitDatabase m_unitPortraitDatabase;
    public UnitCard m_selectedUnitCard;

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
    [SerializeField] private ItemSlotButton[] m_slotButtons;

    private void Awake()
    {
        SubscribeSlotButton();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SubscribeSlotButton()
    {
        if (m_slotButtons == null)
        {
            Debug.Log("SubscribeSlotButton failed..");
            return;
        }

        for(int i = 0; i < m_slotButtons.Length; i++)
        {
            m_slotButtons[i].AddOnClicked(OnSlotButtonClicked);
            Debug.Log("SubscribeSlotButton success..");
        }

    }

    private void OnSlotButtonClicked(ItemSlotButton slotButton)
    {
        Debug.Log("OnSlotButtonClicked..");
        //m_playerDataManager.GenerateItem();
        ApplySpriteToSlot(slotButton);
        PlayerUnitDataUpdate();
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
            m_playerDataManager.GenerateItem();
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

        if(m_selectedUnitCard.m_unitSaveData.m_weapon.m_itemName == "longSword")
        {
            //slotButton.setIconImage(m_testSprite);
            btn_slotWeapon.GetComponent<ItemSlotButton>().setIconImage(m_testSprite);
        }
        else
        {
            btn_slotWeapon.GetComponent<ItemSlotButton>().setIconImage(null);
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
}
