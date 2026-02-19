using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitCard : MonoBehaviour
{
    [SerializeField]
    private Button m_infoButton;
    private UIManager m_uiManager;
    private Sprite m_portrait;
    public string m_unitName;
    public Image m_portraitSlot;
    public int m_playerUnitNumber;
    public TMP_Text text_playerUnitNumber;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitUnitCard(UIManager uiManager, string tempUnitName)
    {
        m_uiManager = uiManager;
        m_infoButton.onClick.AddListener(OnInfoButtonClicked);
        m_unitName = tempUnitName;
        m_portrait = m_uiManager.m_unitPortraitDatabase.GetPortraitSprite(m_unitName);
        m_portraitSlot.sprite = m_portrait;
        
    }

    public void OnInfoButtonClicked()
    {
        m_uiManager.ShowCharacterInfoPanel();
    }
}
