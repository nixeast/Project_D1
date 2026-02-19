using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class UIManager : MonoBehaviour
{

    public GameObject panel_characterInfo;
    public RectTransform scrollViewContent_unitCard;
    public GameObject unitCardPrefab;
    public PlayerDataManager currentPlayerDataManager;
    public UnitPortraitDatabase m_unitPortraitDatabase;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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

    public void ShowCharacterInfoPanel()
    {
        panel_characterInfo.gameObject.SetActive(true);
    }

    public void HideCharacterInfoPanel()
    {
        panel_characterInfo.gameObject.SetActive(false);
        //panel_characterInfo.SetActive(false);
    }

    public void RefreshUnitCard()
    {
        PlayerData playerData = currentPlayerDataManager.GetPlayerData();
        
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
            cardObj.GetComponent<UnitCard>().InitUnitCard(this, playerData.currentUnits[i].unitName);
            //cardObj.GetComponent<UnitCard>().m_unitName = playerData.currentUnits[i].unitName;
            cardObj.GetComponent<UnitCard>().m_playerUnitNumber = i;
            cardObj.GetComponent<UnitCard>().text_playerUnitNumber.text = i.ToString();

        }

    }

    public void SetItemToUnit()
    {

    }
}
