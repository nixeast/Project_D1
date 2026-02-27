using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartingPointButton : MonoBehaviour
{
    [SerializeField] private SpriteRenderer m_spriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnMouseDown()
    {
        //Debug.Log("starting point clicked..");

        if(GameManager.instance.getCurrentStartUnitCount() < GameManager.instance.getMaxStartUnitCount())
        {
            MakeUnitByUnitCard();
        }
        
    }

    public void MakeUnitByUnitCard()
    {

        UnitCard card;
        Sprite unitPortrait;
        int playerUnitNumber;
        GameObject tempUnit;
        
        if(GameManager.instance.m_selectedUnitCard != null)
        {
            card = GameManager.instance.m_selectedUnitCard;
            unitPortrait = GameManager.instance.GetPortraitByName(card.m_unitName);
            playerUnitNumber = card.m_playerUnitNumber;
            tempUnit = Instantiate(GameManager.instance.m_unitObject, this.transform.position, Quaternion.identity);
            tempUnit.GetComponent<SpriteRenderer>().sprite = unitPortrait;

            tempUnit.GetComponent<Unit>().m_unitSaveData = card.m_unitSaveData;
            tempUnit.GetComponent<Unit>().m_name = card.m_unitSaveData.unitName;

            Debug.Log("unit name : " + tempUnit.GetComponent<Unit>().m_name);
            Debug.Log("created unit original num : " + tempUnit.GetComponent<Unit>().m_unitSaveData.m_unitOriginalNumber);

            card.isInBattleField = true;
            card.m_portraitSlot.color = Color.gray;
            GameManager.instance.AddCurrentStartUnitCount();
            GameManager.instance.UpdateStartUnitCount();
            
        }


    }
}
