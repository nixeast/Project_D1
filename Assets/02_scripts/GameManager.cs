using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Net.Http.Headers;
using System;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

public enum eTurnOwner
{
    Default = 0,
    Player = 1,
    Enemy = 2,
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
    public UnitPortraitDatabase m_unitPortraitDatabase;
    public eGamePlayState m_currentGameState = default;
    public IngameUiManager m_ingameUiManager;

    [Header("UnitCommand")]
    public GameObject tile_moveTarget_true;
    public GameObject tile_moveTarget_false;
    public GameObject tile_attackTarget;
    public List<GameObject> m_currentMoveTiles = new List<GameObject>();
    public List<GameObject> m_currentAttackTiles = new List<GameObject>();

    [Header("Etc")]
    public BattleResultManager m_battleResultManager;

    public TMP_Text tmp_maxStartUnit;
    public TMP_Text tmp_curentStartUnit;
    public RectTransform scrollViewContent_unitCard;
    
    //public Button btn_startBattle;
    public GameObject panel_unitCardList;
    public GameObject panel_battleInfo;
    public GameObject tileMap_startingPoints;

    public GameObject m_closeCombatIcon;

    public GameObject m_unitCardPrefab;
    public GameObject m_unitObject;
    public Unit m_testUnit;
    public Sprite m_tempSprite;
    private PlayerData m_playerData;
    public Unit m_currentSelectedUnit;
    public UnitCard m_selectedUnitCard;
    private int m_maxStartUnitCount;
    private int m_currentStartUnitCount;

    public List<Vector3> m_movableTilePositions = new List<Vector3>();
    Vector3[] movePositions = new Vector3[4];
    GameObject[] moveTargets = new GameObject[4];
    Vector3[] attackPositions = new Vector3[4];
    GameObject[] attackTargets = new GameObject[4];

    private List<UnitCard> m_unitCardList = new List<UnitCard>();

    private eTurnOwner m_currentTurnOwner;
    public Button btn_turnOver;
    public GameObject panel_currentTurn;
    public TMP_Text tmp_currentTurn;
    public TMP_Text tmp_turnOwner;
    private int m_currentTurn;

    public List<Unit> m_playerUnits = new List<Unit>();
    public List<Unit> m_enemyUnits = new List<Unit>();

    [SerializeField] GameObject panel_battleResult;


    private void Awake()
    {
        instance = this;
        LoadGameRoot();
        m_maxStartUnitCount = 4;

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
        tmp_maxStartUnit.text = m_maxStartUnitCount.ToString();
        //btn_startBattle.onClick.AddListener(StartBattle);
        btn_turnOver.onClick.AddListener(SwitchTurnOwner);
        m_currentTurnOwner = eTurnOwner.Player;
        m_currentTurn = 1;
        tmp_turnOwner.text = "player";
        tmp_currentTurn.text = m_currentTurn.ToString();
        Debug.Log("<color=yellow>start battleMap Scene</color>");
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
        m_currentSelectedUnit.m_currentControlMode = eUnitControlMode.MoveEnd;
        
        RemoveMoveTargetTiles();
        MakeAttackTargets(m_currentSelectedUnit);

    }

    public void AttackUnit(Unit currentAttackTarget)
    {
        //currentAttackTarget.stat_hp -= currentSelectedUnit.stat_atk;
        //Debug.Log(currentAttackTarget + "-> hp : " + currentAttackTarget.stat_hp);

        m_ingameUiManager.panel_combatExpect.SetActive(true);

        RemoveAttackTargetTiles();
        m_ingameUiManager.panel_combatExpect.SetActive(true);
        m_ingameUiManager.UpdateCombatExpectInfo(m_currentSelectedUnit, currentAttackTarget);
        
    }

    public IEnumerator StartCombatSequence(Unit attacker, Unit defender)
    {
        yield return new WaitForSeconds(1.0f);

        if (attacker.m_canAttack == true)
        {
            defender.m_stat_hp -= attacker.m_stat_atk;
            m_ingameUiManager.UpdateCombatExpectInfo(attacker, defender);
        }

        yield return new WaitForSeconds(1.0f);

        if (defender.m_canAttack == true)
        {
            if(defender.m_stat_hp > 0)
            {
                attacker.m_stat_hp -= defender.m_stat_atk;
                m_ingameUiManager.UpdateCombatExpectInfo(attacker, defender);
            }
        }

        yield return new WaitForSeconds(1.0f);
        m_ingameUiManager.panel_combatExpect.SetActive(false);

        EndCombatPahse(attacker, defender);
    }

    public void EndCombatPahse(Unit attacker, Unit defender)
    {
        attacker.DeadCheck(attacker);
        defender.DeadCheck(defender);
    }

    public void OnClickEndTurn()
    {
        m_currentTurn++;

        int nCount = m_playerUnits.Count;
        for(int i = 0; i < nCount; i++)
        {
            m_playerUnits[i].m_isMoved = false;
        }

        nCount = m_enemyUnits.Count;
        for (int i = 0; i < nCount; i++)
        {
            m_enemyUnits[i].m_isMoved = false;
        }

        Debug.Log("turn[" + m_currentTurn + "] Start!");
    }

    public void AssignCloseCombatState(AttackTarget attackTarget)
    {
        m_currentSelectedUnit.isCloseCombat = true;
        m_currentSelectedUnit.e_currentUnitState = eUnitState.CloseCombat;
        attackTarget.assignedUnit.isCloseCombat = true;
        attackTarget.assignedUnit.e_currentUnitState = eUnitState.CloseCombat;
        MakeCloseCombatIcon(m_currentSelectedUnit, attackTarget);

        m_currentSelectedUnit.ChangeMoveRange();

        Debug.Log(attackTarget.assignedUnit + " is in closeCombatState");
    }

    public void MakeCloseCombatIcon(Unit damageCauser, AttackTarget target)
    {
        Vector3 pos_damageCauser = damageCauser.gameObject.transform.position;
        Vector3 pos_target = target.assignedUnit.gameObject.transform.position;
        Vector3 pos_middle = (pos_damageCauser + pos_target) / 2;

        Instantiate(m_closeCombatIcon, pos_middle, Quaternion.identity);

    }
    public void MakeAttackTargets(Unit selectedUnit)
    {
        int nMeleAttackRange = 1;
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
    }

    public void MakeMoveTargets(Unit selectedUnit, int nUnitAp)
    {
        Vector3 unitPos = selectedUnit.gameObject.transform.position;
        float cx = unitPos.x;
        float cy = unitPos.y;

        int unitLayerMask = LayerMask.GetMask("Unit");

        for (int dy = -nUnitAp; dy <= nUnitAp; dy++)
        {
            int dxLimit = nUnitAp - Mathf.Abs(dy);

            for (int dx = -dxLimit; dx <= dxLimit; dx++)
            {
                float tx = cx + dx;
                float ty = cy + dy;

                Vector3 tempPos = new Vector3(tx, ty, -1.0f);
                //m_movableTilePositions.Add(tempPos);

                Vector3 targetTilePos = tempPos;
                bool hasUnit = Physics2D.OverlapPoint(targetTilePos, unitLayerMask);

                GameObject tempGameObj;

                if (hasUnit == false)
                {
                    tempGameObj = Instantiate(tile_moveTarget_true);
                    tempGameObj.transform.position = tempPos;
                    m_currentMoveTiles.Add(tempGameObj);


                }
                else if (hasUnit == true)
                {
                    if (dx == 0 && dy == 0)
                    {
                        tempGameObj = Instantiate(tile_moveTarget_true);
                        tempGameObj.transform.position = tempPos;
                        m_currentMoveTiles.Add(tempGameObj);
                        //tempGameObj.gameObject.SetActive(false);
                    }
                    else
                    {
                        tempGameObj = Instantiate(tile_moveTarget_false);
                        tempGameObj.transform.position = tempPos;
                        m_currentMoveTiles.Add(tempGameObj);
                    }
                }

            }
        }
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
            Destroy(m_currentAttackTiles[i]);
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
        m_tempSprite = m_unitPortraitDatabase.GetPortraitSprite(unitName);

        if(m_tempSprite == null)
        {
            Debug.Log("GetPortraitByName Failed..");
        }
        else
        {
            Debug.Log("unitName: " + unitName);
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
        Debug.Log("player unit list");

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
            tmp_turnOwner.text = "enemy";
            m_currentTurnOwner = eTurnOwner.Enemy;
        }
        else if(m_currentTurnOwner == eTurnOwner.Enemy)
        {
            m_currentTurn++;
            tmp_currentTurn.text = m_currentTurn.ToString();
            tmp_turnOwner.text = "player";
            m_currentTurnOwner = eTurnOwner.Player;
        }

        Debug.Log(m_currentTurnOwner);

    }

    public void BattleWinCheck()
    {
        if(m_enemyUnits.Count == 0)
        {
            panel_battleResult.SetActive(true);
            m_battleResultManager.MakeWinResult();
            Debug.Log("player win..");
        }
    }

    public void BattleLoseCheck()
    {
        if (m_playerUnits.Count == 0)
        {
            panel_battleResult.SetActive(true);
            m_battleResultManager.MakeLoseResult();
            Debug.Log("enemy win..");
        }
    }

}
