using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IngameUiManager : MonoBehaviour
{
    public GameObject panel_openingScreen;
    public GameObject panel_tileInfo;
    public GameObject panel_unitcard;

    [Header("Mision Data InFo")]
    public int m_missionNumber;
    public GameObject panel_missionBrief;
    public TMP_Text text_missionName;
    public TMP_Text text_missionObjective;

    [Header("Uniy Deployment")]
    public GameObject panel_deployPhase;
    public GameObject prefab_btn_playerUnitCard;
    public List<GameObject> m_playerUnitCardList = new List<GameObject>();
    public RectTransform scrollViewContent_unitCommand;

    [Header("Data")]
    private PlayerDataManager m_playerDataManager;
    public PlayerData m_playerData;
    public UnitDataBase m_unitDatabase;

    public void Awake()
    {
        LoadDatas();
    }

    public void Start()
    {
        LoadMissionInfo();
        SetMissionBriefContent(m_missionNumber);
        
        InitPlayerUnitCardList();

    }
    
    public void InitPlayerUnitCardList()
    {
        m_playerUnitCardList.Clear();

        m_playerData = m_playerDataManager.GetPlayerData();
        int nPlayerUnitCount = m_playerData.m_currentUnits.Count;
        for (int i = 0; i < nPlayerUnitCount; i++)
        {
            GameObject newCard = Instantiate(prefab_btn_playerUnitCard, scrollViewContent_unitCommand);
            PlayerUnitCard tempUnitData = newCard.GetComponent<PlayerUnitCard>();
            int newOriginNumber = m_playerData.m_currentUnits[i].m_unitOriginalNumber;
            
            tempUnitData.m_unitOriginNumber = newOriginNumber;
            tempUnitData.text_unitNumber.text = tempUnitData.m_unitOriginNumber.ToString();
            tempUnitData.img_unitPortrait.sprite = m_unitDatabase.GetUnitPortrait(newOriginNumber);
            m_playerUnitCardList.Add(newCard);
        }


    }
 
    
    public void LoadDatas()
    {
        m_playerDataManager = PlayerDataManager.s_instance;
        m_unitDatabase = UnitDataBase.s_instance;
    }

    public void OnClickBattleStart()
    {
        panel_deployPhase.SetActive(false);
    }

    public void LoadMissionInfo()
    {
        if(GameRoot.s_instance == null)
        {
            return;
        }
        m_missionNumber = GameRoot.s_instance.GetStartMissionNumber();
    }

    public void SetMissionBriefContent(int nMissionNumber)
    {
        if (GameRoot.s_instance == null)
        {
            return;
        }

        MissionData newData;
        bool isSuccess = false;
        isSuccess = MissionDatabase.s_instance.m_missionDataDic.TryGetValue(nMissionNumber, out newData);
        if(isSuccess == true)
        {
            text_missionName.text = newData.m_missionName;
            text_missionObjective.text = newData.m_missionObjective;

        }
        else
        {
            Debug.Log("Load mission info from missionDatabase failed..");
        }
    }

    public void OnClickToContinue()
    {
        panel_openingScreen.SetActive(false);
    }

    public void OnClickCloseMissionBriefPanel()
    {
        panel_missionBrief.SetActive(false);
        panel_deployPhase.SetActive(true);
    }
}
