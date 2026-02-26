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

    public TMP_Text tmp_maxStartUnit;
    public TMP_Text tmp_curentStartUnit;
    public RectTransform scrollViewContent_unitCard;
    
    public Button btn_startBattle;
    public GameObject panel_unitCardList;
    public GameObject panel_battleInfo;
    public GameObject tileMap_startingPoints;

    public GameObject m_closeCombatIcon;

    public GameObject m_unitCardPrefab;
    public GameObject m_unitObject;
    public Unit m_testUnit;
    public Sprite m_tempSprite;
    private PlayerData m_playerData;
    public Unit currentSelectedUnit;
    public UnitCard m_selectedUnitCard;
    private int m_maxStartUnitCount;
    private int m_currentStartUnitCount;

    public List<Vector3> m_movableTilePositions = new List<Vector3>();
    public List<MoveTarget> m_movableTiles = new List<MoveTarget>();
    public GameObject tile_moveTarget_true;
    public GameObject tile_attackTarget_true;
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
        btn_startBattle.onClick.AddListener(StartBattle);
        btn_turnOver.onClick.AddListener(SwitchTurnOwner);
        m_currentTurnOwner = eTurnOwner.Player;
        m_currentTurn = 1;
        tmp_turnOwner.text = "player";
        tmp_currentTurn.text = m_currentTurn.ToString();
        Debug.Log("<color=yellow>start battleMap Scene</color>");
    }

    // Update is called once per frame
    void Update()
    {
        
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
        currentSelectedUnit = currentUnit;
        Debug.Log("select unit : " + currentUnit.m_spriteRenderer.sprite.name);
        //Debug.Log(currentSelectedUnit);
    }

    public void SelectUnitCard(UnitCard unitCard)
    {
        m_selectedUnitCard = unitCard;

        if(unitCard != null)
        {
            Debug.Log("UnitCard is selected");
        }
        else
        {
            Debug.Log("Selected UnitCard is null");
        }
    }

    public void MoveUnit(MoveTarget currentMoveTarget)
    {
        
        currentSelectedUnit.gameObject.transform.position = currentMoveTarget.gameObject.transform.position;
        
        if(currentSelectedUnit.e_currentUnitState == eUnitState.CloseCombat)
        {
            currentSelectedUnit.e_currentUnitState = eUnitState.Default;

            int nOpponentUnitCount = 0;
            nOpponentUnitCount = currentSelectedUnit.m_closeCombatOpponents.Count;
            for (int i = 0; i < nOpponentUnitCount; i++)
            {
                currentSelectedUnit.m_closeCombatOpponents[i].m_closeCombatOpponents.Remove(currentSelectedUnit);
                if(currentSelectedUnit.m_closeCombatOpponents[i].m_closeCombatOpponents.Count == 0)
                {
                    currentSelectedUnit.m_closeCombatOpponents[i].e_currentUnitState = eUnitState.Default;
                }
            }

            currentSelectedUnit.m_closeCombatOpponents.Clear();
        }
        
        currentSelectedUnit = null;
        RemoveMoveTargetTiles();
        
    }

    public void AttackUnit(AttackTarget currentAttackTarget)
    {
        //Debug.Log("attack unit : " + currentAttackTarget.assignedUnit);
        currentAttackTarget.assignedUnit.stat_health -= currentSelectedUnit.stat_attack;
        Debug.Log(currentAttackTarget.assignedUnit + "-> hp : " + currentAttackTarget.assignedUnit.stat_health);

        currentAttackTarget.assignedUnit.m_closeCombatOpponents.Add(currentSelectedUnit);
        currentSelectedUnit.m_closeCombatOpponents.Add(currentAttackTarget.assignedUnit);
        AssignCloseCombatState(currentAttackTarget);

        bool isDead = false;
        isDead = currentAttackTarget.assignedUnit.DeadCheck();
        if(isDead == true)
        {
            currentSelectedUnit.e_currentUnitState = eUnitState.Default;
            Debug.Log("unit state changed : ->default");
        }
        

        currentSelectedUnit = null;

        RemoveAttackTargetTiles();
        ResetPositions();
    }

    public void AssignCloseCombatState(AttackTarget attackTarget)
    {
        currentSelectedUnit.isCloseCombat = true;
        currentSelectedUnit.e_currentUnitState = eUnitState.CloseCombat;
        attackTarget.assignedUnit.isCloseCombat = true;
        attackTarget.assignedUnit.e_currentUnitState = eUnitState.CloseCombat;
        MakeCloseCombatIcon(currentSelectedUnit, attackTarget);

        currentSelectedUnit.ChangeMoveRange();

        Debug.Log(attackTarget.assignedUnit + " is in closeCombatState");
    }

    public void MakeCloseCombatIcon(Unit damageCauser, AttackTarget target)
    {
        Vector3 pos_damageCauser = damageCauser.gameObject.transform.position;
        Vector3 pos_target = target.assignedUnit.gameObject.transform.position;
        Vector3 pos_middle = (pos_damageCauser + pos_target) / 2;

        Instantiate(m_closeCombatIcon, pos_middle, Quaternion.identity);

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


    //public void MakeMoveTargets(Unit selectedUnit, int nMoveLevel)
    //{
    //    //setup length
    //    int nLength = (nMoveLevel * 2) + 1;
    //    int nLengthX = nLength;
    //    int nLengthY = nLength;

    //    float fLengthX = (float)nLengthX;
    //    float fLengthY = (float)nLengthY;

    //    int nMiddleX = Mathf.CeilToInt(fLengthX / 2);
    //    int nMiddleY = Mathf.CeilToInt(fLengthY / 2);

    //    //int nSize = nLength * nLength;

    //    float targetPosX = selectedUnit.gameObject.transform.position.x - nMoveLevel;
    //    float targetPosY = selectedUnit.gameObject.transform.position.y + nMoveLevel;

    //    //create posistions
    //    int nCountX = 0;
    //    int nCountY = 0;
    //    GameObject tempGameObj;

    //    int nCount = 0;

    //    while (nCountY < nMiddleY)
    //    {
    //        while(nCountX < nLengthX)
    //        {
    //            Vector3 tempPos = new Vector3(targetPosX, targetPosY, 0f);
    //            m_movableTilePositions.Add(tempPos);

    //            tempGameObj = Instantiate(tile_moveTarget_true);
    //            tempGameObj.transform.position = tempPos;
    //            m_movableTiles.Add(tempGameObj.GetComponent<MoveTarget>());

    //            if(tempGameObj.transform.position == selectedUnit.gameObject.transform.position)
    //            {
    //                tempGameObj.SetActive(false);
    //            }

    //            if(nCountX < nMiddleX)
    //            {
    //                nCount = nCountX;
    //                tempGameObj.SetActive(false);
    //            }
    //            else if(nCountX == nMiddleX)
    //            {

    //            }
    //            else if(nCountX > nMiddleX)
    //            {

    //            }


    //            nCountX++;
    //            targetPosX++;
    //        }

    //        nCountX = 0;
    //        targetPosX = selectedUnit.gameObject.transform.position.x - nMoveLevel;

    //        nCountY++;
    //        targetPosY--;
    //    }
    //}

    public void MakeMoveTargets(Unit selectedUnit, int nMoveLevel)
    {
        // ���� ��ġ�� ���� ���ڶ�� ������ int�� ����
        Vector3 unitPos = selectedUnit.gameObject.transform.position;
        float cx = unitPos.x;
        float cy = unitPos.y;

        for (int dy = -nMoveLevel; dy <= nMoveLevel; dy++)
        {
            int dxLimit = nMoveLevel - Mathf.Abs(dy);

            for (int dx = -dxLimit; dx <= dxLimit; dx++)
            {
                float tx = cx + dx;
                float ty = cy + dy;

                Vector3 tempPos = new Vector3(tx, ty, 0f);

                m_movableTilePositions.Add(tempPos);

                GameObject tempGameObj = Instantiate(tile_moveTarget_true);
                tempGameObj.transform.position = tempPos;

                m_movableTiles.Add(tempGameObj.GetComponent<MoveTarget>());

                // �߽�ĭ�� ��Ȱ�� (float �� ��� dx/dy�� ����)
                if (dx == 0 && dy == 0)
                {
                    tempGameObj.SetActive(false);
                }
            }
        }
    }

    //public void InactiveMoveTargets()
    //{
    //    float fLength = (float)Math.Sqrt(m_movableTiles.Count);
    //    float fMiddle = (float)fLength / 2.0f;
    //    int nMiddleY = Mathf.CeilToInt(fMiddle);
    //    int nMiddleX = Mathf.CeilToInt(fMiddle);

    //    Debug.Log("moveTiles fLength / nMiddleX: " + fLength + "/" + nMiddleX);

    //    int nCount = 0;
    //    for(int i=0; i < nMiddleY; i++)
    //    {
    //        if(nCount < nMiddleX)
    //        {

    //        }
    //        else if(nCount == nMiddleX)
    //        {
                
    //        }
    //        else if(nCount > nMiddleX)
    //        {
                 
    //        }
    //    }
        

    //}

    public void RemoveMoveTargetTiles()
    {
        int nCount = m_movableTiles.Count;

        for(int i=0 ; i < nCount ; i++)
        {
            //Destroy(moveTargets[i]);
            Destroy(m_movableTiles[i].gameObject);
        }

        m_movableTiles.Clear();

        Debug.Log("RemoveMoveTargetTiles, m_movableTiles cleared..");
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

            MakeMoveTargets(selectedUnit, selectedUnit.stat_moveRange_modified);
        }
        else if(selectedUnit.currentControlMode == unitControlMode.Attack)
        {
            RemoveMoveTargetTiles();

            MakeAttackTargets(selectedUnit);
        }

    }

    public void ShowMovableArea(Unit selectedUnit)
    {
        ResetPositions();

        RemoveAttackTargetTiles();
        MakeMoveTargets(selectedUnit, selectedUnit.stat_moveRange);
    }

    public void ShowAttackableArea(Unit selectedUnit)
    {
        ResetPositions();

        RemoveMoveTargetTiles();
        MakeAttackTargets(selectedUnit);
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
    
    public void LoadUnitCard()
    {

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
            
            cardObj.GetComponent<UnitCard>().m_unitName = m_playerData.currentUnits[i].unitName;
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
            Debug.Log("GetPortraitByName Success..");
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
        //scrollViewContent_unitCard.gameObject.SetActive(false);
        panel_unitCardList.SetActive(false);
        panel_battleInfo.SetActive(false);
        tileMap_startingPoints.SetActive(false);
        Debug.Log("StartBattle");
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

}
