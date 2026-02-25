using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Net.Http.Headers;
using System;
using TMPro;

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
    public GameObject tileMap_startingPoints;
    public GameObject m_unitCardPrefab;
    public GameObject m_unitObject;
    public Unit m_testUnit;
    public Sprite m_tempSprite;
    private PlayerData m_playerData;
    public Unit currentSelectedUnit;
    public UnitCard m_selectedUnitCard;
    private int m_maxStartUnitCount;
    private int m_currentStartUnitCount;
    public GameObject tile_moveTarget_true;
    public GameObject tile_attackTarget_true;

    Vector3[] movePositions = new Vector3[4];
    Vector3[] attackPositions = new Vector3[4];
    
    GameObject[] moveTargets = new GameObject[4];
    GameObject[] attackTargets = new GameObject[4];
    
    private List<UnitCard> m_unitCardList = new List<UnitCard>();

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
        SetGamePlayState(eGamePlayState.SetupBattleUnit);
        LoadUnitCard();
        tmp_maxStartUnit.text = m_maxStartUnitCount.ToString();
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
        Debug.Log("select unit..");
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

}
