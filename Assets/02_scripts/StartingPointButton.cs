using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartingPointButton : MonoBehaviour
{
    [SerializeField] private SpriteRenderer m_spriteRenderer;
    public IngameUiManager m_ingameUiManager;
    public GameObject obj_tilemap_object;

    private void OnMouseDown()
    {
        Debug.Log("starting point clicked..");
        // if(GameManager.instance.getCurrentStartUnitCount() < GameManager.instance.getMaxStartUnitCount())
        // {
        //     MakeUnitByUnitCard();
        // }
        
        //MakeUnitByUnitCard();
        
        if(this.gameObject.tag == "area")
        {
            SetUnitcardOnBattleMap();
        }
        
    }
    
    public void SetUnitcardOnBattleMap()
    {
        GameObject newUnit = Instantiate(GameManager.instance.m_unitObject, this.gameObject.transform.position, Quaternion.identity, obj_tilemap_object.transform);

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
