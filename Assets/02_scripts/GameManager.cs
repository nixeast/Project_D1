using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Net.Http.Headers;
using System;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using Unity.VisualScripting.Dependencies.Sqlite;

public enum eTurnOwner
{
    Default = 0,
    Player = 1,
    enemy = 2,
}
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
    [SerializeField] private GameRoot m_gameRoot;
    public IngameUiManager m_ingameUiManager;
    public BattleResultManager m_battleResultManager;
    
    [Header("Data")]
    private PlayerData m_playerData;
    public UnitPortraitDatabase m_unitPortraitDatabase;

    [Header("Stage")]
    public StageManager m_stageManager;
    public eGamePlayState m_currentGameState = default;
    private int m_maxStartUnitCount;
    private int m_currentStartUnitCount;
    public int m_mapSizeX;
    public int m_mapSizeY;
    public int[,] distanceMap;
    public TMP_Text tmp_maxStartUnit;
    public TMP_Text tmp_curentStartUnit;
    public List<Unit> m_playerUnits = new List<Unit>();
    public List<Unit> m_enemyUnits = new List<Unit>();
    public int m_currentPlayerUnitCount;
    public int m_currentEnemyUnitCount;
    private ImissionCondition m_currentMissionCondition;
    public List<EnemySpawnTile> m_enemySpawnTileList = new List<EnemySpawnTile>();

    [Header("Turn")]
    public TMP_Text tmp_turnOwner;
    public int m_currentTurn;
    public eTurnOwner m_currentTurnOwner;
    public Button btn_turnOver;
    public GameObject panel_currentTurn;

    [Header("UnitCommand")]
    public GameObject tile_moveTarget_true;
    public GameObject tile_moveTarget_false;
    public GameObject tile_attackTarget;
    public List<GameObject> m_currentMoveTiles = new List<GameObject>();
    public List<GameObject> m_currentAttackTiles = new List<GameObject>();
    public RectTransform scrollViewContent_unitCard;
    public GameObject panel_unitCardList;
    private List<UnitCard> m_unitCardList = new List<UnitCard>();
    public GameObject tileMap_startingPoints;
    public GameObject m_closeCombatIcon;
    public GameObject m_unitCardPrefab;
    public GameObject m_unitObject;
    public List<Vector3> m_movableTilePositions = new List<Vector3>();
    Vector3[] movePositions = new Vector3[4];
    GameObject[] moveTargets = new GameObject[4];
    Vector3[] attackPositions = new Vector3[4];
    GameObject[] attackTargets = new GameObject[4];
    public Unit m_currentSelectedUnit;
    public UnitCard m_selectedUnitCard;

    [Header("Battle")]
    public GameObject panel_battleInfo;
    public GameObject panel_battleResult;
    public bool isEndCombatSequence = false;

    private void Awake()
    {
        instance = this;
        LoadGameRoot();
        m_maxStartUnitCount = 4;
        m_stageManager.GenerateStage();

        //m_savePath = Path.Combine(Application.persistentDataPath,"m_playerData.json");
        //m_playerData = new PlayerData();
        //m_playerData.nGold = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        MakeMovablePositions();
        MakeAttackablePositions();
        SetGamePlayState(eGamePlayState.SetupBattleUnit);
        LoadUnitCard();
        InitTurnInfo();
        tmp_maxStartUnit.text = m_maxStartUnitCount.ToString();
        m_mapSizeX = 19;
        m_mapSizeY = 11;
        distanceMap = new int[m_mapSizeX, m_mapSizeY];

        InitMissionCondition();
        Debug.Log("<color=yellow>start battleMap Scene</color>");
    }

    public int CheckAliveEnemyUnitCount()
    {
        int nCount = m_enemyUnits.Count;
        int nData = 0;
        for (int i = 0; i < nCount; i++)
        {
            if (m_enemyUnits[i].IsUnityNull() == false)
            {
                nData++;
                Debug.Log("alive number: " + i);
            }
        }
        Debug.Log("alive enemy count: " + nData);
        return nData;
    }

    public void InitMissionCondition()
    {
        int nCurrentMissionNumber = GameRoot.s_instance.GetStartMissionNumber();
        MissionData newData;
        MissionDatabase.s_instance.m_missionDataDic.TryGetValue(nCurrentMissionNumber, out newData);
        string missionConditionName = newData.m_missionType.ToString();
        if(missionConditionName == "Defense")
        {
            m_currentMissionCondition = new DefenseCondition();

        }
    }

    public void StopGamePlay()
    {

    }

    public void WinLoseCheck()
    {
        m_currentMissionCondition.CheckVictory();
        m_currentMissionCondition.CheckDefeat();
    }

    public void InitTurnInfo()
    {
        m_currentTurn = 1;
        m_currentTurnOwner = eTurnOwner.Player;
        tmp_turnOwner.text = "player";
        m_ingameUiManager.UpdateTurnUI();
    }

    public void UpdateLeftUnitCount()
    {
        UpdateLeftPlayerUnitCount();
        UpdateLeftEnemyUnitCount();
    }

    public void UpdateLeftPlayerUnitCount()
    {
        int nCount = m_playerUnits.Count;
        int nData = 0;

        for(int i = 0; i < nCount; i++)
        {
            if (m_playerUnits[i].m_isDead == false) nData++;
        }

        m_currentPlayerUnitCount = nData;
        //Debug.Log("m_currentPlayerUnitCount: " + m_currentPlayerUnitCount);

    }

    public void UpdateLeftEnemyUnitCount()
    {
        int nCount = m_enemyUnits.Count;
        int nData = 0;

        for (int i = 0; i < nCount; i++)
        {
            if (m_enemyUnits[i].m_isDead == false) nData++;
        }

        m_currentEnemyUnitCount = nData;
    }

    private void LoadGameRoot()
    {
        m_gameRoot = GameRoot.s_instance;

        if (m_gameRoot != null)
        {
            m_playerData = m_gameRoot.GetPlayerData();
        }
        else
        {
            Debug.Log("<color=red>no gameRoot..</color>");
        }
    }

    public void MakeMovablePositions()
    {
        movePositions[0] = new Vector3(1.0f, 0f, 0f);
        movePositions[1] = new Vector3(-1.0f, 0f, 0f);
        movePositions[2] = new Vector3(0f, 1.0f, 0f);
        movePositions[3] = new Vector3(0f, -1.0f, 0f);


    }

    public void MakeAttackablePositions()
    {
        attackPositions[0] = new Vector3(1.0f, 0f, 0f);
        attackPositions[1] = new Vector3(-1.0f, 0f, 0f);
        attackPositions[2] = new Vector3(0f, 1.0f, 0f);
        attackPositions[3] = new Vector3(0f, -1.0f, 0f);
    }

    

    public void SetGamePlayState(eGamePlayState state)
    {
        m_currentGameState = state;
    }

    public void SelectUnit(Unit currentUnit)
    {
        m_currentSelectedUnit = currentUnit;
    }

    public void SelectUnitCard(UnitCard unitCard)
    {
        m_selectedUnitCard = unitCard;

        if(unitCard != null)
        {
            //Debug.Log("UnitCard is selected");
        }
        else
        {
            //Debug.Log("Selected UnitCard is null");
        }
    }

    public void MoveUnit(MoveTarget currentMoveTarget)
    {
        Vector3 newPos;
        newPos.x = currentMoveTarget.gameObject.transform.position.x;
        newPos.y = currentMoveTarget.gameObject.transform.position.y;
        newPos.z = -1.0f;

        m_currentSelectedUnit.gameObject.transform.position = newPos;
        m_currentSelectedUnit.m_isMoved = true;
        //m_currentSelectedUnit.m_currentControlMode = eUnitControlMode.MoveEnd;
        m_currentSelectedUnit.m_currentControlMode = eUnitControlMode.Attack;


        RemoveMoveTargetTiles();
        MakeAttackTargets(m_currentSelectedUnit);
        m_ingameUiManager.UpdateUnitControlState(m_currentSelectedUnit);

    }

    public void AttackUnit(Unit currentAttackTarget)
    {
        RemoveAttackTargetTiles();
        isEndCombatSequence = false;
        m_ingameUiManager.panel_combatExpect.SetActive(true);
        m_ingameUiManager.UpdateCombatExpectInfo(m_currentSelectedUnit, currentAttackTarget);
        MakeAttackTargets(currentAttackTarget);
        CheckCounterAttack(currentAttackTarget);
        m_ingameUiManager.UpdateCombatExpectInfo(m_currentSelectedUnit, currentAttackTarget);
    }

    public void CheckCounterAttack(Unit attackTarget)
    {
        attackTarget.m_canAttack = false;
        int nAtkTileCount = m_currentAttackTiles.Count;

        for (int i = 0; i < nAtkTileCount; i++)
        {
            if (m_currentAttackTiles[i].GetComponent<AttackTarget>().m_assignedUnit != null)
            {
                if (m_currentAttackTiles[i].GetComponent<AttackTarget>().m_assignedUnit.gameObject == m_currentSelectedUnit.gameObject)
                {
                    attackTarget.m_canAttack = true;
                    //Debug.Log("defender: attacker in my range");
                }
            }
        }
    }

    public bool CheckAttackHit(Unit attackUnit)
    {
        //Debug.Log("start check attack hit");
        int nAttackChance = attackUnit.m_currentAttackChance;
        int nResult = UnityEngine.Random.Range(1, 101);
        //Debug.Log("chance: " + nAttackChance + " / nResult: " + nResult);

        if (nAttackChance >= 100)
        {
            return true;
        }
        
        if(nResult <= nAttackChance)
        {
            return true;
        }

        return false;
    }

    public IEnumerator StartCombatSequence(Unit attacker, Unit defender)
    {
        yield return new WaitForSeconds(1.0f);

        if (attacker.m_canAttack == true)
        {
            bool isHit = CheckAttackHit(attacker);
            if (isHit == true)
            {
                defender.m_stat_hp -= attacker.m_stat_atk;
                m_ingameUiManager.UpdateCombatExpectInfo(attacker, defender);

            }
        }

        yield return new WaitForSeconds(0.5f);

        if (defender.m_canAttack == true)
        {
            if(defender.m_stat_hp > 0)
            {
                bool isHit = CheckAttackHit(defender);
                if (isHit == true)
                {
                    attacker.m_stat_hp -= defender.m_stat_atk;
                    m_ingameUiManager.UpdateCombatExpectInfo(attacker, defender);
                }
            }
        }

        //yield return new WaitForSeconds(0.5f);

    }

    public void ExitCombatSequence(Unit attacker, Unit defender)
    {
        RemoveAttackTargetTiles();
        m_ingameUiManager.panel_combatExpect.SetActive(false);
        CombatUnitDeadCheck(attacker, defender);
        isEndCombatSequence = true;

        UpdateLeftUnitCount();
        WinLoseCheck();
    }

    public void CombatUnitDeadCheck(Unit attacker, Unit defender)
    {
        attacker.DeadCheck(attacker);
        defender.DeadCheck(defender);
    }

    public void IncreaseTurn()
    {
        if (m_currentTurnOwner == eTurnOwner.enemy)
        {
            m_currentTurn++;
            m_ingameUiManager.UpdateTurnUI();
        }
    }

    public void ResetIsMoved()
    {
        if (m_currentTurnOwner == eTurnOwner.Player)
        {
            int nCount = m_playerUnits.Count;
            for (int i = 0; i < nCount; i++)
            {
                m_playerUnits[i].m_isMoved = false;
            }
        }
        else if (m_currentTurnOwner == eTurnOwner.enemy)
        {
            int nCount = m_enemyUnits.Count;
            for (int i = 0; i < nCount; i++)
            {
                m_enemyUnits[i].m_isMoved = false;
            }
            //StartCoroutine(CommandEenmyUnits());
        }
    }

    public void CheckRespawnTiles()
    {
        if (m_currentTurnOwner == eTurnOwner.enemy)
        {
            List<EnemySpawnTile> removeList = new List<EnemySpawnTile>();
            int nSpawnTileCount = m_enemySpawnTileList.Count;
            
            for (int i = 0; i < nSpawnTileCount; i++)
            {
                if (m_enemySpawnTileList[i].m_createTurn == m_currentTurn)
                {
                    m_enemySpawnTileList[i].CreateCheck();
                    m_enemySpawnTileList[i].gameObject.SetActive(false);
                    
                }
            }


        }
    }
    
    public void RemoveEnemySpawnTile(EnemySpawnTile spawnTile)
    {
        //m_enemySpawnTileList.Remove(spawnTile);
        Destroy(spawnTile.gameObject);
    }

    public void OnClickEndTurn()
    {
        IncreaseTurn();
        ResetIsMoved();
        SwitchTurnOwner();
        CheckRespawnTiles();
        if (m_currentTurnOwner == eTurnOwner.enemy)
        {
            StartCoroutine(CommandEenmyUnits());
        }

    }

    public Unit AssisgnAttackTarget(Unit _mainUnit)
    {
        Debug.Log("searching cloest player unit..");

        List<Unit> tempUnitList = new List<Unit>();

        Unit resultUnit = null;
        int nCount = m_playerUnits.Count;
        for (int i = 0; i < nCount; i++)
        {
            if (m_playerUnits[i].m_isDead == false)
            {
                Unit targetUnit = m_playerUnits[i];
                tempUnitList.Add(targetUnit);
                
            }
        }

        if (tempUnitList.Count > 0)
        {
            int nAliveUnitCount = tempUnitList.Count;
            //int nUnitNumber = 0;
            float fDistToClosestPlayerUnit = Vector3.Distance(tempUnitList[0].transform.position, _mainUnit.transform.position);
            resultUnit = tempUnitList[0];
            for (int i =0; i < nAliveUnitCount; i++)
            {
                float tempDist = Vector3.Distance(tempUnitList[i].transform.position, _mainUnit.transform.position);
                if(tempDist < fDistToClosestPlayerUnit)
                {
                    fDistToClosestPlayerUnit = tempDist;
                    resultUnit = tempUnitList[i];
                }
            }
            Debug.Log("cloest player unit: "+ resultUnit.m_name);
            return resultUnit;
        }

        return resultUnit;
    }
    
    public void AssignTargetUnit(Unit mainUnit)
    {
        m_ingameUiManager.text_selectedUnitName.text = mainUnit.m_name;
        int nUnitID = mainUnit.m_unitID;
        m_ingameUiManager.img_selectedUnitPortrait.sprite = m_ingameUiManager.m_unitDatabase.GetUnitPortrait(nUnitID);
        m_ingameUiManager.img_selectedUnitPortrait.gameObject.SetActive(true);

        Unit tempUnit = AssisgnAttackTarget(mainUnit);
        //Unit targetUnit = null;
        //int nCount = m_playerUnits.Count;
        //for(int i=0;i<nCount;i++)
        //{
        //    if(m_playerUnits[i].m_isDead == false)
        //    {
        //        targetUnit = m_playerUnits[i];
        //        i = nCount;
        //    }
        //}

        if (tempUnit != null)
        {
            mainUnit.m_currentTargetUnit = tempUnit.gameObject;

        }
        else
        {
            Debug.Log("no target unit exist");
        }
    }

    public int CommandEnemyToFindMovePos(Unit mainUnit)
    {
        mainUnit.OnMouseDown();
        int nMoveTileCount = m_currentMoveTiles.Count;
        float fDistance;
        fDistance = Vector3.Distance(m_currentMoveTiles[0].transform.position, mainUnit.transform.position);
        
        int nCloseMoveTileNumber = 0;
        for (int j = 0; j < nMoveTileCount; j++)
        {
            if(mainUnit.m_currentTargetUnit == null)
            {
                return nCloseMoveTileNumber;
            }

            float newDistance = Vector3.Distance(m_currentMoveTiles[j].transform.position, mainUnit.m_currentTargetUnit.transform.position);
            if (newDistance < fDistance)
            {
                if (m_currentMoveTiles[j].GetComponent<MoveTarget>() == true)
                {
                    fDistance = newDistance;
                    nCloseMoveTileNumber = j;
                }

            }
        }
        return nCloseMoveTileNumber;
    }

    public void CommandEnemyToMoveToPosition(int nCloseTileNumber)
    {
        if (m_currentMoveTiles[nCloseTileNumber].GetComponent<MoveTarget>() == true)
        {
            m_currentMoveTiles[nCloseTileNumber].GetComponent<MoveTarget>().OnMouseDown();
        }
        else
        {
            Debug.Log("no move target detected");
            RemoveMoveTargetTiles();
        }
    }

    public void CommandEnemyToCheckAttackTarget()
    {
        int nAttackTargetCount = m_currentAttackTiles.Count;
        List<GameObject> tempTargetList = new List<GameObject>();
        tempTargetList.Clear();

        for (int j = 0; j < nAttackTargetCount; j++)
        {
            AttackTarget tempTarget = m_currentAttackTiles[j].GetComponent<AttackTarget>();
            if (tempTarget.m_assignedUnit == true)
            {
                if (tempTarget.m_assignedUnit.tag == "Player")
                {
                    tempTargetList.Add(m_currentAttackTiles[j]);
                }
            }
        }

        if (tempTargetList.Count == 0)
        {
            RemoveAttackTargetTiles();
            isEndCombatSequence = true;
        }
        else
        {
            tempTargetList[0].GetComponent<AttackTarget>().OnMouseDown();
            //m_ingameUiManager.OnClickConfirmCombatExpect();
            RemoveAttackTargetTiles();
        }
    }

    public IEnumerator CommandEenmyUnits()
    {
        if (m_currentTurnOwner == eTurnOwner.enemy)
        {
            int nCount = m_enemyUnits.Count;
            for (int i = 0; i < nCount; i++)
            {
                if (m_enemyUnits[i] != null)
                {
                    AssignTargetUnit(m_enemyUnits[i]);
                    int nCloseTileNumber = CommandEnemyToFindMovePos(m_enemyUnits[i]);
                    yield return new WaitForSeconds(1.0f);
                    CommandEnemyToMoveToPosition(nCloseTileNumber);
                    yield return new WaitForSeconds(1.0f);
                    CommandEnemyToCheckAttackTarget();
                    yield return new WaitUntil(() => isEndCombatSequence == true);
                }
                else
                {
                    Debug.Log("dead but in list found: enemy[" + i + "]");
                }

                UpdateLeftPlayerUnitCount();
                if(m_currentPlayerUnitCount <= 0)
                {
                    WinLoseCheck();
                    yield break;
                }
            }
        }

        yield return new WaitForSeconds(1.0f);
        OnClickEndTurn();
    }

    public void AssignCloseCombatState(AttackTarget attackTarget)
    {
        m_currentSelectedUnit.isCloseCombat = true;
        m_currentSelectedUnit.e_currentUnitState = eUnitState.CloseCombat;
        attackTarget.m_assignedUnit.isCloseCombat = true;
        attackTarget.m_assignedUnit.e_currentUnitState = eUnitState.CloseCombat;
        MakeCloseCombatIcon(m_currentSelectedUnit, attackTarget);

        m_currentSelectedUnit.ChangeMoveRange();

        Debug.Log(attackTarget.m_assignedUnit + " is in closeCombatState");
    }

    public void MakeCloseCombatIcon(Unit damageCauser, AttackTarget target)
    {
        Vector3 pos_damageCauser = damageCauser.gameObject.transform.position;
        Vector3 pos_target = target.m_assignedUnit.gameObject.transform.position;
        Vector3 pos_middle = (pos_damageCauser + pos_target) / 2;

        Instantiate(m_closeCombatIcon, pos_middle, Quaternion.identity);

    }
    public void MakeAttackTargets(Unit selectedUnit)
    {
        int nMeleAttackRange = 1;
        nMeleAttackRange = selectedUnit.m_stat_attackRange;
        //Debug.Log("atk range: "+selectedUnit.m_stat_attackRange);

        Vector3 unitPos = selectedUnit.gameObject.transform.position;
        int minColumn = nMeleAttackRange * -1;
        int maxColumn = nMeleAttackRange;

        int nDrawRange = 0;
        for (int i = minColumn; i < 0; i++)
        {

            int nStartRange = nDrawRange * -1;
            int nEndRange = nDrawRange;
            for (int j = nStartRange; j <= nEndRange; j++)
            {
                Vector3 drawPos = unitPos + new Vector3(i, j, -1.0f);
                GameObject tempGameObj = Instantiate(tile_attackTarget);
                tempGameObj.transform.position = drawPos;
                m_currentAttackTiles.Add(tempGameObj);
            }

            nDrawRange++;
        }

        nDrawRange = nMeleAttackRange;
        for (int i = 0; i < 1; i++)
        {
            int nStartRange = nDrawRange * -1;
            int nEndRange = nDrawRange;
            for (int j = nStartRange; j <= nEndRange; j++)
            {
                Vector3 drawPos = unitPos + new Vector3(i, j, -1.0f);
                GameObject tempGameObj = Instantiate(tile_attackTarget);
                tempGameObj.transform.position = drawPos;
                m_currentAttackTiles.Add(tempGameObj);
                if (i == 0 && j == 0)
                {
                    tempGameObj.SetActive(false);
                }
            }
        }

        nDrawRange = nMeleAttackRange - 1;
        for (int i = 1; i <= maxColumn; i++)
        {
            int nStartRange = nDrawRange * -1;
            int nEndRange = nDrawRange;
            for (int j = nStartRange; j <= nEndRange; j++)
            {
                Vector3 drawPos = unitPos + new Vector3(i, j, -1.0f);
                GameObject tempGameObj = Instantiate(tile_attackTarget);
                tempGameObj.transform.position = drawPos;
                m_currentAttackTiles.Add(tempGameObj);
            }
            nDrawRange--;
        }

        int nAtkTileCount = m_currentAttackTiles.Count;
        //Debug.Log("nAtkTileCount: " + nAtkTileCount);
        for (int i = 0; i < nAtkTileCount; i++)
        {
            Collider2D hit = null;

            Vector2 newPos = new Vector2();
            newPos.x = m_currentAttackTiles[i].transform.position.x;
            newPos.y = m_currentAttackTiles[i].transform.position.y;
            hit = Physics2D.OverlapPoint(newPos, LayerMask.GetMask("Unit"));

            if (hit != null && hit.gameObject != selectedUnit.gameObject)
            {
                m_currentAttackTiles[i].GetComponent<AttackTarget>().m_assignedUnit = hit.gameObject.GetComponent<Unit>();
                //Debug.Log("find enemy with Attack overlapPoint");
            }
        }
    }

    public void FindMovableArea(int startX, int startY, int movePower)
    {
        // 1. [�ʱ�ȭ] ��� Ÿ���� �Ÿ��� -1(�̹湮)�� ä��ϴ�.
        for (int x = 0; x < m_mapSizeX; x++)
        {
            for (int y = 0; y < m_mapSizeY; y++)
            {
                distanceMap[x, y] = -1;
            }
        }

        distanceMap[startX, startY] = 0;

        for (int currentStep = 0; currentStep < movePower; currentStep++)
        {
            for (int x = 0; x < m_mapSizeX; x++)
            {
                for (int y = 0; y < m_mapSizeY; y++)
                {
                    if (distanceMap[x, y] == currentStep)
                    {
                        CheckNeighborTile(x, y + 1, currentStep + 1);
                        CheckNeighborTile(x, y - 1, currentStep + 1);
                        CheckNeighborTile(x - 1, y, currentStep + 1);
                        CheckNeighborTile(x + 1, y, currentStep + 1);
                    }
                }
            }
        }

        for (int i = 0; i < m_mapSizeX; i++)
        {
            for (int j = 0; j < m_mapSizeY; j++)
            {
                int unitLayerMask = LayerMask.GetMask("Unit");

                if (distanceMap[i, j] != -1 && distanceMap[i, j] <= movePower)
                {
                    Vector3 tempPos = new Vector3();
                    tempPos.x = i - 9;
                    tempPos.y = j - 5;
                    tempPos.z = -1.0f;

                    bool hasUnit = Physics2D.OverlapPoint(tempPos, unitLayerMask);

                    if (hasUnit == false)
                    {
                        GameObject newTile = Instantiate(tile_moveTarget_true);
                        newTile.transform.position = tempPos;
                        m_currentMoveTiles.Add(newTile);
                    }
                    else if(hasUnit == true)
                    {
                        if(distanceMap[i, j] == 0)
                        {
                            GameObject newTile = Instantiate(tile_moveTarget_true);
                            newTile.transform.position = tempPos;
                            m_currentMoveTiles.Add(newTile);
                        }
                        else
                        {
                            GameObject newTile = Instantiate(tile_moveTarget_false);
                            newTile.transform.position = tempPos;
                            m_currentMoveTiles.Add(newTile);
                        }
                    }
                }
            }
        }

    }

    void CheckNeighborTile(int targetX, int targetY, int nextStepValue)
    {
        if (targetX < 0 || targetX >= m_mapSizeX || targetY < 0 || targetY >= m_mapSizeY)
        {
            return;
        }

        if (distanceMap[targetX, targetY] != -1)
        {
            return;
        }

        if (CheckIfEnemyExists(targetX, targetY) == true)
        {
            return;
        }

        if (CheckIfObstacle(targetX, targetY) == true)
        {
            return;
        }

        distanceMap[targetX, targetY] = nextStepValue;
    }

    bool CheckIfEnemyExists(int x, int y)
    {
        Vector2 currentPos = new Vector2(x, y);
        List<Vector2> tempEnemyPositionList = new List<Vector2>();

        int nCounterForceCount;
        if (m_currentTurnOwner == eTurnOwner.Player)
        {
            nCounterForceCount = m_enemyUnits.Count;

            for (int i = 0; i < nCounterForceCount; i++)
            {
                if (m_enemyUnits[i].m_isDead == false)
                {
                    Vector2 tempPos;
                    tempPos.x = m_enemyUnits[i].gameObject.transform.position.x + 9.0f;
                    tempPos.y = m_enemyUnits[i].gameObject.transform.position.y + 5.0f;
                    //tempEnemyPositionList.Add(tempPos);
                    if (tempPos.x == currentPos.x && tempPos.y == currentPos.y)
                    {
                        //Debug.Log("find enemy unit in movement range");
                        return true;
                    }

                }
            }
        }
        else
        {
            nCounterForceCount = m_playerUnits.Count;

            

            for (int i = 0; i < nCounterForceCount; i++)
            {
                if (m_playerUnits[i].m_isDead == false)
                {
                    Vector2 tempPos;
                    tempPos.x = m_playerUnits[i].gameObject.transform.position.x + 9.0f;
                    tempPos.y = m_playerUnits[i].gameObject.transform.position.y + 5.0f;

                    if (tempPos.x == currentPos.x && tempPos.y == currentPos.y)
                    {
                        //Debug.Log("find player unit in movement range");
                        return true;
                    }

                }

            }
        }
        
        

        return false;
    }

    bool CheckIfObstacle(int x, int y)
    {
        return false;
    }

    public void MakeMoveTargets(Unit selectedUnit, int nUnitAp)
    {
        Vector3 unitPos = selectedUnit.gameObject.transform.position;
        float cx = unitPos.x;
        float cy = unitPos.y;

        int ix = Mathf.FloorToInt(cx) + 9;
        int iy = Mathf.FloorToInt(cy) + 5;
        FindMovableArea(ix, iy, nUnitAp);

    }

    public void RemoveMoveTargetTiles()
    {
        int nCount = m_currentMoveTiles.Count;

        for(int i=0 ; i < nCount ; i++)
        {
            Destroy(m_currentMoveTiles[i].gameObject);
        }
        m_currentMoveTiles.Clear();
    }

    public void RemoveAttackTargetTiles()
    {
        int nCount = m_currentAttackTiles.Count;
        
        for (int i = 0; i < nCount; i++)
        {
            Destroy(m_currentAttackTiles[i].gameObject);
        }
        m_currentAttackTiles.Clear();
    }


    public void ShowMovableArea(Unit selectedUnit)
    {
        RemoveAttackTargetTiles();
        MakeMoveTargets(selectedUnit, selectedUnit.m_stat_moveRange);
    }

    public void ShowAttackableArea(Unit selectedUnit)
    {
        RemoveMoveTargetTiles();
    }
    
    public void LoadUnitCard()
    {

        int unitCardCount = 0;

        if(m_playerData != null)
        {
            unitCardCount = m_playerData.m_currentUnits.Count;
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
            
            cardObj.GetComponent<UnitCard>().m_unitName = m_playerData.m_currentUnits[i].unitName;
            cardObj.GetComponent<UnitCard>().m_unitSaveData = m_playerData.m_currentUnits[i];

            string tempUnitName = cardObj.GetComponent<UnitCard>().m_unitName;
            cardObj.GetComponent<UnitCard>().m_playerUnitNumber = i;
            cardObj.GetComponent<UnitCard>().text_playerUnitNumber.text = i.ToString();
            Sprite tempSprite = m_unitPortraitDatabase.GetPortraitSprite(tempUnitName);
            cardObj.GetComponent<UnitCard>().m_portraitSlot.sprite = tempSprite;
            cardObj.GetComponent<UnitCard>().SetGameManager(this);
            cardObj.GetComponent<UnitCard>().InitUnitCardSelectButton();
            m_unitCardList.Add(cardObj.GetComponent<UnitCard>());

        }

    }

    public Sprite GetPortraitByName(string unitName)
    {
        Sprite m_tempSprite = m_unitPortraitDatabase.GetPortraitSprite(unitName);

        if(m_tempSprite == null)
        {
            Debug.Log("GetPortraitByName Failed..");
        }
        else
        {
            //Debug.Log("unitName: " + unitName);
        }

        return m_tempSprite;
    }

    public void ResetAllUnitCardHighlight()
    {
        int nCount = m_unitCardList.Count;
        for(int i=0; i < nCount; i++)
        {
            if(m_unitCardList[i].isInBattleField == false)
            {
                m_unitCardList[i].m_portraitSlot.color = Color.white;
            }
            
            m_unitCardList[i].isSelected = false;
        }
    }

    public void AddCurrentStartUnitCount()
    {
        m_currentStartUnitCount++;
    }
    public void SubstractCurrentStartUnitCount()
    {
        m_currentStartUnitCount--;
    }

    public void UpdateStartUnitCount()
    {
        tmp_curentStartUnit.text = m_currentStartUnitCount.ToString();
    }

    public int getCurrentStartUnitCount()
    {
        return m_currentStartUnitCount;
    }

    public int getMaxStartUnitCount()
    {
        return m_maxStartUnitCount;
    }

    public void StartBattle()
    {
        m_currentGameState = eGamePlayState.Battle;

        panel_unitCardList.SetActive(false);
        panel_battleInfo.SetActive(false);
        tileMap_startingPoints.SetActive(false);
        
        Debug.Log("StartBattle");
        //Debug.Log("player unit list");

        int nLength = m_playerUnits.Count;
        int nCount = 0;
        while(nCount < nLength)
        {
            Debug.Log(m_playerUnits[nCount].m_name);
            nCount++;
        }

    }

    public void SwitchTurnOwner()
    {
        if(m_currentTurnOwner == eTurnOwner.Player) 
        {
            m_currentTurnOwner = eTurnOwner.enemy;
            //Debug.Log("enemy turn started");
        }
        else if (m_currentTurnOwner == eTurnOwner.enemy)
        {
            m_currentTurnOwner = eTurnOwner.Player;
            //Debug.Log("player turn started");
        }
    }

    public void BattleWin()
    {
        panel_battleResult.SetActive(true);
        m_battleResultManager.MakeWinResult();
        Debug.Log("player win..");
    }

    public void BattleLose()
    {
        panel_battleResult.SetActive(true);
        m_battleResultManager.MakeLoseResult();
        Debug.Log("enemy win..");
    }

}
