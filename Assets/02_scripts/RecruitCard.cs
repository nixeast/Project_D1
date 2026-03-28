using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RecruitCard : MonoBehaviour
{
    public string m_unitName;
    public string m_unitType;
    public string m_unitTrait;
    public int m_goldCost;
    public int m_honorCost;
    public int m_recruitID;

    public TMP_Text text_unitName;
    public TMP_Text text_unitType;
    public TMP_Text text_unitTrait;
    public TMP_Text text_goldCost;
    public TMP_Text text_honorCost;

    //public Image image_portrait_unit;
    public Image image_icon_trait;
    public Button btn_portrait_unit;
    public UIManager m_uiManager;
    public Image image_soldOut;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void AssignSelectedUnit()
    {
        m_uiManager.isRecruitUnitSelected = true;
        m_uiManager.m_selectedRecruitUnitName = m_unitName;
        m_uiManager.m_selectedRecruitCard = this;
    }
}
