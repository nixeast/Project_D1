using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartingPointButton : MonoBehaviour
{
    [SerializeField] private SpriteRenderer m_spriteRenderer;
    public IngameUiManager m_ingameUiManager;
    public GameManager m_gameManager;
    public GameObject obj_tilemapRoot_object;

    public void Start()
    {
        m_gameManager = GameManager.instance;
    }

    private void OnMouseDown()
    {
        if(this.gameObject.tag == "area")
        {
            CreateUnitcardOnBattleMap();
            m_ingameUiManager.obj_DeployArea.gameObject.SetActive(false);
        }
    }
    
    public void CreateUnitcardOnBattleMap()
    {
        int unitID = m_ingameUiManager.m_selectedDeployUnitNum;
        GameObject newUnit;
        Vector3 newPos = this.transform.position;
        newPos.z = -1.0f;
        newUnit = Instantiate(m_gameManager.m_unitObject, newPos, Quaternion.identity, obj_tilemapRoot_object.transform);
        
        Sprite sp = m_ingameUiManager.m_unitDatabase.GetUnitIconSprite(unitID);

        Unit tempUnit = newUnit.GetComponent<Unit>();
        tempUnit.m_spriteRenderer.sprite = sp;
        tempUnit.m_unitID = unitID;
        tempUnit.SetUnitstats(unitID);

        m_ingameUiManager.m_selectedPlayerUnitCard.gameObject.GetComponent<Button>().enabled = false;
        Color newColor = m_ingameUiManager.m_selectedPlayerUnitCard.GetComponent<PlayerUnitCard>().img_unitPortrait.color;
        newColor.a = 0.1f;
        m_ingameUiManager.m_selectedPlayerUnitCard.GetComponent<PlayerUnitCard>().img_unitPortrait.color = newColor;

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
