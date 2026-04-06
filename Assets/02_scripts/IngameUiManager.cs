using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum ePhaseType
{
    Default = 0,
    UnitDeplyment = 1,
    Battle = 2,
}

public class IngameUiManager : MonoBehaviour
{
    public GameObject panel_openingScreen;
    public GameObject panel_tileInfo;
    public GameObject panel_unitcard;
    public ePhaseType m_currentPhase;
    public GameManager m_gameManager;

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
    public GameObject obj_DeployArea;
    public int m_selectedDeployUnitNum;
    public GameObject m_selectedPlayerUnitCard;

    [Header("Combat")]
    public GameObject panel_combatExpect;
    public Button btn_combatExpect_Confirm;
    public Image img_unit_player;
    public Image img_unit_enemy;
    public TMP_Text text_unitName_palyer;
    public TMP_Text text_unitName_enemy;
    public TMP_Text text_attackName_palyer;
    public TMP_Text text_attackName_enemy;
    public TMP_Text text_unitinfo_player_hp;
    public TMP_Text text_unitinfo_player_atk;
    public TMP_Text text_unitinfo_player_def;
    public TMP_Text text_unitinfo_player_hit;
    public TMP_Text text_unitinfo_player_eva;
    public TMP_Text text_unitinfo_player_ap;
    public TMP_Text text_unitinfo_enemy_hp;
    public TMP_Text text_unitinfo_enemy_atk;
    public TMP_Text text_unitinfo_enemy_def;
    public TMP_Text text_unitinfo_enemy_hit;
    public TMP_Text text_unitinfo_enemy_eva;
    public TMP_Text text_unitinfo_enemy_ap;
    public TMP_Text text_hp_playerUnit;
    public TMP_Text text_hp_enemyUnit;
    public TMP_Text text_combat_playerUnit_attack;
    public TMP_Text text_combat_playerUnit_accuracy;
    public TMP_Text text_combat_playerUnit_critical;
    public TMP_Text text_combat_enemyUnit_attack;
    public TMP_Text text_combat_enemyUnit_accuracy;
    public TMP_Text text_combat_enemyUnit_critical;
    public Image img_hpBar_playerUnit;
    public Image img_hpBar_enemyUnit;
    public Image img_atkCondition_playerUnit;
    public Image img_atkCondition_enemyUnit;
    public Unit m_currentCombatAttacker;
    public Unit m_currentCombatDefender;


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
        m_gameManager = GameManager.instance;
        LoadMissionInfo();
        SetMissionBriefContent(m_missionNumber);
        
        InitPlayerUnitCardList();
        SetCurrentPhase(ePhaseType.UnitDeplyment);
    }

    public void OnClickConfirmCombatExpect()
    {
        Debug.Log(m_currentCombatAttacker.m_name);
        Debug.Log(m_currentCombatDefender.m_name);
        StartCoroutine(m_gameManager.StartCombatSequence(m_currentCombatAttacker, m_currentCombatDefender));
        
        //panel_combatExpect.SetActive(false);
    }

    public void UpdateCombatExpectInfo(Unit attacker, Unit defender)
    {
        Unit leftUnit;
        Unit rightUnit;

        leftUnit = attacker;
        rightUnit = defender;
        m_currentCombatAttacker = attacker;
        m_currentCombatDefender = defender;

        if (attacker.gameObject.tag == "enemy")
        {
            leftUnit = defender;
            rightUnit = attacker;
        }

        UnitData newData;
        int nLeftUnitID = leftUnit.m_unitID;
        m_unitDatabase.m_unitDataDic.TryGetValue(nLeftUnitID, out newData);

        img_unit_player.sprite = Resources.Load<Sprite>(newData.m_PortraitPath);
        text_unitName_palyer.text = leftUnit.m_name.ToString();
        text_attackName_palyer.text = "normal attack";
        text_unitinfo_player_hp.text = leftUnit.m_stat_hp.ToString();
        text_unitinfo_player_atk.text = leftUnit.m_stat_atk.ToString();
        text_unitinfo_player_def.text = leftUnit.m_stat_def.ToString();
        text_unitinfo_player_hit.text = leftUnit.m_stat_hit.ToString();
        text_unitinfo_player_eva.text = leftUnit.m_stat_eva.ToString();
        text_unitinfo_player_ap.text = leftUnit.m_stat_ap.ToString();
        text_hp_playerUnit.text = leftUnit.m_stat_hp.ToString();
        text_combat_playerUnit_attack.text = leftUnit.m_stat_atk.ToString();
        text_combat_playerUnit_accuracy.text = leftUnit.m_stat_hit.ToString();
        text_combat_playerUnit_critical.text = leftUnit.m_stat_hit.ToString();

        int nRightUnitID = rightUnit.m_unitID;
        m_unitDatabase.m_unitDataDic.TryGetValue(nRightUnitID, out newData);
        img_unit_enemy.sprite = Resources.Load<Sprite>(newData.m_PortraitPath);
        text_unitName_enemy.text = rightUnit.m_name.ToString();
        text_attackName_enemy.text = "verminkin attack";
        text_unitinfo_enemy_hp.text = rightUnit.m_stat_hp.ToString();
        text_unitinfo_enemy_atk.text = rightUnit.m_stat_atk.ToString();
        text_unitinfo_enemy_def.text = rightUnit.m_stat_def.ToString();
        text_unitinfo_enemy_hit.text = rightUnit.m_stat_hit.ToString();
        text_unitinfo_enemy_eva.text = rightUnit.m_stat_eva.ToString();
        text_unitinfo_enemy_ap.text = rightUnit.m_stat_ap.ToString();
        text_hp_enemyUnit.text = rightUnit.m_stat_hp.ToString();
        text_combat_enemyUnit_attack.text = rightUnit.m_stat_atk.ToString();
        text_combat_enemyUnit_accuracy.text = rightUnit.m_stat_hit.ToString();
        text_combat_enemyUnit_critical.text = rightUnit.m_stat_hit.ToString();


    }

    public void OnClickDeployUnitCard(int nUnitNumber)
    {
        if (m_currentPhase == ePhaseType.UnitDeplyment)
        {
            m_selectedDeployUnitNum = nUnitNumber;
            ShowDeploymentArea();
            Debug.Log("selected unit id :" + nUnitNumber);

        }
    }

    private void OnClickDeployUnitCard_01(GameObject clickedUnitObj)
    {
        //Debug.Log("클릭된 유닛 버튼 이름: " + clickedUnitObj.name);
        m_selectedPlayerUnitCard = clickedUnitObj;

    }

    public void ShowDeploymentArea()
    {
        obj_DeployArea.SetActive(true);
    }
    
    public void SetCurrentPhase(ePhaseType phaseType)
    {
        m_currentPhase = phaseType;
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

            tempUnitData.btn_cardButton.onClick.AddListener(() => OnClickDeployUnitCard(newOriginNumber));
            tempUnitData.btn_cardButton.onClick.AddListener(() => OnClickDeployUnitCard_01(tempUnitData.btn_cardButton.gameObject));
            //tempUnitData.btn_cardButton.onClick.AddListener(ShowDeploymentArea);

            //m_selectedPlayerUnitCard = newCard;
            m_playerUnitCardList.Add(newCard);
        }
    }
    
    public void LoadDatas()
    {
        if(PlayerDataManager.s_instance)
        {
            m_playerDataManager = PlayerDataManager.s_instance;
        }
        else
        {
            Debug.Log("can't find playerDataManager");
        }

        m_unitDatabase = UnitDataBase.s_instance;
    }

    public void OnClickBattleStart()
    {
        panel_deployPhase.SetActive(false);
        SetCurrentPhase(ePhaseType.Battle);
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
