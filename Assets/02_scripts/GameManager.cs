using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public enum eGamePlayState
{
    Default = 0,
    SetupBattleUnit = 1,
    Battle = 2,
    BattleResult = 3,
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance {get; private set;}
    public Unit currentSelectedUnit;
    public Unit m_testUnit;

    public GameObject tile_moveTarget_true;
    public GameObject tile_attackTarget_true;

    public eGamePlayState m_currentGameState = default;
    public PlayerDataManager m_playerDataManager;
    private PlayerData m_playerData;
    private string m_savePath;

    public GameObject m_unitCardPrefab;
    public RectTransform scrollViewContent_unitCard;

    Vector3[] movePositions = new Vector3[4];
    Vector3[] attackPositions = new Vector3[4];
    
    GameObject[] moveTargets = new GameObject[4];
    GameObject[] attackTargets = new GameObject[4];

    private void Awake()
    {
        instance = this;
        
        m_savePath = Path.Combine(Application.persistentDataPath,"m_playerData.json");
        m_playerData = new PlayerData();
        m_playerData.nGold = 0;
        initUnitSaveData();
    }

    // Start is called before the first frame update
    void Start()
    {
        movePositions[0] = new Vector3(1.0f,0f,0f); 
        movePositions[1] = new Vector3(-1.0f,0f,0f); 
        movePositions[2] = new Vector3(0f,1.0f,0f); 
        movePositions[3] = new Vector3(0f,-1.0f,0f);

        attackPositions[0] = new Vector3(1.0f, 0f, 0f);
        attackPositions[1] = new Vector3(-1.0f, 0f, 0f);
        attackPositions[2] = new Vector3(0f, 1.0f, 0f);
        attackPositions[3] = new Vector3(0f, -1.0f, 0f);

        SetGamePlayState(eGamePlayState.SetupBattleUnit);
        RefreshUnitCard();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void initUnitSaveData()
    {
        m_playerData.currentUnits = new UnitSaveData[4];

        m_playerData.currentUnits[0] = new UnitSaveData();
        m_playerData.currentUnits[0].unitName = "Dwarf_01";
        m_playerData.currentUnits[0].level = 1;
        m_playerData.currentUnits[0].upgrade = 1;

        m_playerData.currentUnits[0].m_weapon = null;
        m_playerData.currentUnits[0].m_armor = null;
        m_playerData.currentUnits[0].m_charm_01 = null;
        m_playerData.currentUnits[0].m_charm_02 = null;

        m_playerData.currentUnits[0].m_health = 10;
        m_playerData.currentUnits[0].m_attack = 10;
        m_playerData.currentUnits[0].m_defense = 10;
        m_playerData.currentUnits[0].m_morale = 10;

        m_playerData.currentUnits[1] = new UnitSaveData();
        m_playerData.currentUnits[1].unitName = "Dwarf_02";
        m_playerData.currentUnits[1].level = 2;
        m_playerData.currentUnits[1].upgrade = 2;

        m_playerData.currentUnits[1].m_weapon = null;
        m_playerData.currentUnits[1].m_armor = null;
        m_playerData.currentUnits[1].m_charm_01 = null;
        m_playerData.currentUnits[1].m_charm_02 = null;

        m_playerData.currentUnits[1].m_health = 11;
        m_playerData.currentUnits[1].m_attack = 11;
        m_playerData.currentUnits[1].m_defense = 11;
        m_playerData.currentUnits[1].m_morale = 11;

        m_playerData.currentUnits[2] = new UnitSaveData();
        m_playerData.currentUnits[2].unitName = "Dwarf_03";
        m_playerData.currentUnits[2].level = 3;
        m_playerData.currentUnits[2].upgrade = 3;

        m_playerData.currentUnits[2].m_weapon = null;
        m_playerData.currentUnits[2].m_armor = null;
        m_playerData.currentUnits[2].m_charm_01 = null;
        m_playerData.currentUnits[2].m_charm_02 = null;

        m_playerData.currentUnits[2].m_health = 12;
        m_playerData.currentUnits[2].m_attack = 12;
        m_playerData.currentUnits[2].m_defense = 12;
        m_playerData.currentUnits[2].m_morale = 12;

        m_playerData.currentUnits[3] = new UnitSaveData();
        m_playerData.currentUnits[3].unitName = "Dwarf_04";
        m_playerData.currentUnits[3].level = 4;
        m_playerData.currentUnits[3].upgrade = 4;

        m_playerData.currentUnits[3].m_weapon = null;
        m_playerData.currentUnits[3].m_armor = null;
        m_playerData.currentUnits[3].m_charm_01 = null;
        m_playerData.currentUnits[3].m_charm_02 = null;

        m_playerData.currentUnits[3].m_health = 13;
        m_playerData.currentUnits[3].m_attack = 13;
        m_playerData.currentUnits[3].m_defense = 13;
        m_playerData.currentUnits[3].m_morale = 13;

        Debug.Log("init unitSaveData..");
    }
    
    public void SetGamePlayState(eGamePlayState state)
    {
        m_currentGameState = state;
    }

    public void SelectUnit(Unit currentUnit)
    {
        currentSelectedUnit = currentUnit;
        Debug.Log("select unit..");
        //Debug.Log(currentSelectedUnit);
    }

    public void MoveUnit(MoveTarget currentMoveTarget)
    {
        Debug.Log("move unit..");
        currentSelectedUnit.gameObject.transform.position = currentMoveTarget.gameObject.transform.position;
        
        currentSelectedUnit = null;
        RemoveMoveTargetTiles();
        ResetPositions();
    }

    public void AttackUnit(AttackTarget currentAttackTarget)
    {
        Debug.Log("attack unit : " + currentAttackTarget.assignedUnit);
        currentAttackTarget.assignedUnit.stat_health -= currentSelectedUnit.stat_attack;
        currentAttackTarget.assignedUnit.DeadCheck();
        Debug.Log("enemy hp : " + currentAttackTarget.assignedUnit.stat_health);

        currentSelectedUnit = null;
        RemoveAttackTargetTiles();
        //RemoveMoveTargetTiles();
        ResetPositions();
    }

    public void ResetPositions()
    {
        for (int i = 0; i < 4; i++)
        {
            movePositions[i].x = 0f;
            movePositions[i].y = 0f;
            movePositions[i].z = 0f;

            attackPositions[i].x = 0f;
            attackPositions[i].y = 0f;
            attackPositions[i].z = 0f;

        }

        movePositions[0].x = 1.0f;
        movePositions[1].x = -1.0f;
        movePositions[2].y = 1.0f;
        movePositions[3].y = -1.0f;

        attackPositions[0].x = 1.0f;
        attackPositions[1].x = -1.0f;
        attackPositions[2].y = 1.0f;
        attackPositions[3].y = -1.0f;

    }


    public void MakeMoveTargets(Unit selectedUnit)
    {
        //Debug.Log("make moveTargetTiles..");

        for(int i=0 ; i < 4 ; i++)
        {
            movePositions[i] = movePositions[i] + selectedUnit.gameObject.transform.position;
        }

        for(int i=0 ; i < 4 ; i++)
        {
            moveTargets[i] = Instantiate(tile_moveTarget_true,movePositions[i],Quaternion.identity);
        }

    }

    public void RemoveMoveTargetTiles()
    {
        for(int i=0 ; i < 4 ; i++)
        {
            Destroy(moveTargets[i]);
        }
    }

    public void RemoveAttackTargetTiles()
    {
        for (int i = 0; i < 4; i++)
        {
            Destroy(attackTargets[i]);
        }
    }

    public void ChangeUnitControlMode(Unit selectedUnit)
    {
        ResetPositions();

        if (selectedUnit.currentControlMode == unitControlMode.Move)
        {
            RemoveAttackTargetTiles();

            MakeMoveTargets(selectedUnit);
        }
        else if(selectedUnit.currentControlMode == unitControlMode.Attack)
        {
            RemoveMoveTargetTiles();

            MakeAttackTargets(selectedUnit);
        }

    }

    public void MakeAttackTargets(Unit selectedUnit)
    {

        for (int i = 0; i < 4; i++)
        {
            attackPositions[i] = attackPositions[i] + selectedUnit.gameObject.transform.position;
        }

        for (int i = 0; i < 4; i++)
        {
            attackTargets[i] = Instantiate(tile_attackTarget_true, attackPositions[i], Quaternion.identity);
        }

    }
    
    public void RefreshUnitCard()
    {
        //PlayerData m_playerData = m_playerDataManager.GetPlayerData();

        int unitCardCount = 0;

        if(m_playerData != null)
        {
            unitCardCount = m_playerData.currentUnits.Length;
            Debug.Log("unitCardCount: " + unitCardCount);
        }
        else
        {
            Debug.Log("m_playerData is null..");
        }

        for(int i=0; i<unitCardCount; i++)
        {
            GameObject cardObj = Instantiate(m_unitCardPrefab);
            cardObj.transform.SetParent(scrollViewContent_unitCard, false);
            
            //cardObj.GetComponent<UnitCard>().InitUnitCard(this, m_playerData.currentUnits[i].unitName, m_playerData.currentUnits[i]);
            
            cardObj.GetComponent<UnitCard>().m_playerUnitNumber = i;
            cardObj.GetComponent<UnitCard>().text_playerUnitNumber.text = i.ToString();

        }

    }

}
