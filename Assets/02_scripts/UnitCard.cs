using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitCard : MonoBehaviour
{
    [SerializeField] private Button m_infoButton;
    [SerializeField] private Button m_selectButton;
    
    private UIManager m_uiManager;
    private GameManager m_gameManager;
    public UnitSaveData m_unitSaveData;

    private Sprite m_portrait;
    public string m_unitName;
    public Image m_portraitSlot;
    public int m_playerUnitNumber;
    public TMP_Text text_playerUnitNumber;
    public bool isSelected;
    public bool isInBattleField;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitUnitCardSelectButton()
    {
        m_selectButton.onClick.AddListener(OnSelectButtonClicked);
    }

    public void SetGameManager(GameManager gameManager)
    {
        m_gameManager = gameManager;
    }

    public void InitUnitCard(UIManager uiManager, string tempUnitName, UnitSaveData currentUnitSaveData)
    {
        m_uiManager = uiManager;
        m_infoButton.onClick.AddListener(OnInfoButtonClicked);
        m_unitName = tempUnitName;
        m_portrait = m_uiManager.m_unitPortraitDatabase.GetPortraitSprite(m_unitName);
        m_portraitSlot.sprite = m_portrait;
        m_unitSaveData = currentUnitSaveData;

    }

    public void OnInfoButtonClicked()
    {
        m_uiManager.ShowCharacterInfoPanel(this);
    }

    public void OnSelectButtonClicked()
    {
        //m_uiManager.ShowCharacterInfoPanel(this);
        
        if(isSelected == false)
        {
            m_gameManager.ResetAllUnitCardHighlight();
            m_portraitSlot.color = Color.green;
            isSelected = true;
            m_gameManager.SelectUnitCard(this);
        }
        else
        {
            m_portraitSlot.color = Color.white;
            isSelected = false;
            m_gameManager.SelectUnitCard(null);
        }
    }
}
