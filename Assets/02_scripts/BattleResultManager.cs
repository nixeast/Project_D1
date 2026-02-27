using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class BattleResultManager : MonoBehaviour
{
    public TMP_Text tmp_winLose;
    public Button btn_continue;
    public RectTransform scrollViewContent_playerUnitSlot;
    public Button prefab_resultUnitSlot;

    // Start is called before the first frame update
    void Start()
    {
        btn_continue.onClick.AddListener(gotoLobbyScene);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void MakeWinResult()
    {
        tmp_winLose.text = "win";
        RefreshResultUnitSlots();
    }
    
    public void MakeLoseResult()
    {
        tmp_winLose.text = "lose";
        RefreshResultUnitSlots();
    }
    
    public void RefreshResultUnitSlots()
    {
        int nbtnSlotCount = GameManager.instance.m_playerUnits.Count;
        for(int i=0; i<nbtnSlotCount; i++)
        {
            Button unitSlot = Instantiate(prefab_resultUnitSlot);
            unitSlot.transform.SetParent(scrollViewContent_playerUnitSlot, false);
            unitSlot.GetComponentInChildren<TMP_Text>().text = GameManager.instance.m_playerUnits[i].m_unitSaveData.m_unitOriginalNumber.ToString();

            //cardObj.GetComponent<UnitCard>().InitUnitCard(this, m_playerData.currentUnits[i].unitName, m_playerData.currentUnits[i]);

            // cardObj.GetComponent<UnitCard>().m_unitName = m_playerData.currentUnits[i].unitName;
            // string tempUnitName = cardObj.GetComponent<UnitCard>().m_unitName;
            // cardObj.GetComponent<UnitCard>().m_playerUnitNumber = i;
            // cardObj.GetComponent<UnitCard>().text_playerUnitNumber.text = i.ToString();
            // Sprite tempSprite = m_unitPortraitDatabase.GetPortraitSprite(tempUnitName);
            // cardObj.GetComponent<UnitCard>().m_portraitSlot.sprite = tempSprite;
            // cardObj.GetComponent<UnitCard>().SetGameManager(this);
            // cardObj.GetComponent<UnitCard>().InitUnitCardSelectButton();
            // m_unitCardList.Add(cardObj.GetComponent<UnitCard>());

        }
        
    }
    
    public void gotoLobbyScene()
    {
        Debug.Log("btn clicked");
        SceneManager.LoadScene("sc_00_lobby");
    }
    
    public void CheckCombatUnitInUnitSaveData()
    {
        
    }
    
}
