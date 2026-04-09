using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public enum eUnitControlMode
{
    Default = 0,
    Move = 1,
    MoveEnd = 2,
    Attack = 3,
}

public enum eUnitState
{
    Default = 0,
    CloseCombat = 1,
    Escape = 2,
    
}
public class Unit : MonoBehaviour
{
    //public GameObject tile_moveTarget_true;
    Vector3 currentPosition;
    public GameManager m_gameManager;
    public IngameUiManager m_ingameUiManager;

    [Header("Unit Stat")]
    public int m_unitID;
    public string m_name;
    public int m_stat_hp;
    public int m_stat_atk;
    public int m_stat_def;
    public int m_stat_hit;
    public int m_stat_eva;
    public int m_stat_ap;
    public int m_stat_moveRange;
    public int m_stat_moveRange_modified;
    public UnitDataBase m_unitDatabase;
    public bool m_isMoved = false;
    public bool m_canAttack = true;
    public int m_stat_attackRange;

    [Header("Unit Command")]
    public GameObject m_currentTargetUnit;
    public eUnitControlMode m_currentControlMode;

    [Header("Unit Data")]
    public UnitSaveData m_unitSaveData;
    public GameObject m_myUnitCard;
    public SpriteRenderer m_spriteRenderer;
    public bool isCloseCombat;
    public eUnitState e_currentUnitState = eUnitState.Default;
    public List<Unit> m_closeCombatOpponents = new List<Unit>();

    public void Awake()
    {
        m_unitDatabase = GameObject.FindObjectOfType<UnitDataBase>();
        m_gameManager = GameObject.FindObjectOfType<GameManager>();
        m_ingameUiManager = m_gameManager.m_ingameUiManager;
    }

    // Start is called before the first frame update
    void Start()
    {
        currentPosition = this.transform.position;
        m_currentControlMode = eUnitControlMode.Default;

        AssignUnitToUnitList();
        if(this.gameObject.tag == "enemy")
        {
            SetUnitstats(m_unitID);
        }
    }

    public void AssignUnitToUnitList()
    {
        if (this.gameObject.tag == "enemy")
        {
            AssignUnitToEnemyList();
        }
        else if (this.gameObject.tag == "Player")
        {
            AssignUnitToPlayerUnitList();
        }
    }

    public void SetUnitstats(int nUnitID)
    {
        UnitData newData;
        if (m_unitDatabase.m_unitDataDic.TryGetValue(nUnitID, out newData) == true)
        {
            //m_name = newData.m_UnitName;
            m_name = newData.m_unitType;
            m_stat_hp = newData.m_stat_HP;
            m_stat_atk = newData.m_stat_ATK;
            m_stat_def = newData.m_stat_DEF;
            m_stat_ap = newData.m_stat_AP;
            m_stat_attackRange = newData.m_stat_attackRange;
        }else
        {
            Debug.Log("no match unitID with unitData");
        }

    }
    
    public void AssignUnitToEnemyList()
    {
        GameManager.instance.m_enemyUnits.Add(this);
    }
    
    public void AssignUnitToPlayerUnitList()
    {
        if(GameManager.instance.m_playerUnits.Contains(this) == false)
        {
            GameManager.instance.m_playerUnits.Add(this);
        }
    }

    public void ChangeMoveRange()
    {
        m_stat_moveRange_modified = m_stat_moveRange / 2;
    }

    public void OnMouseDown()
    {
        if(m_gameManager.m_currentGameState == eGamePlayState.SetupBattleUnit)
        {
            //Debug.Log("this is setup state");
            //return to original state > player unit card
            this.m_myUnitCard.GetComponent<PlayerUnitCard>().btn_cardButton.enabled = true;

            Color tempColor;
            tempColor = this.m_myUnitCard.GetComponent<PlayerUnitCard>().img_unitPortrait.color;
            tempColor.a = 1.0f;
            this.m_myUnitCard.GetComponent<PlayerUnitCard>().img_unitPortrait.color = tempColor;
            this.m_myUnitCard.GetComponent<PlayerUnitCard>().m_currentDeployedUnit = null;

            //destroy this unit
            m_gameManager.m_playerUnits.Remove(this);
            Destroy(this.gameObject);

            return;
        }

        m_gameManager.SelectUnit(this);
        
        if(this.gameObject.tag != m_gameManager.m_currentTurnOwner.ToString())
        {
            Debug.Log("turn owner missmatch");
            return;
        }

        if (m_isMoved == false)
        {
            m_currentControlMode = eUnitControlMode.Move;
            m_gameManager.MakeMoveTargets(this, m_stat_ap);
            m_ingameUiManager.UpdateUnitControlState(this);
        }
        else if(m_isMoved == true)
        {
            if (m_currentControlMode == eUnitControlMode.Attack)
            {
                m_currentControlMode = eUnitControlMode.Default;
                m_gameManager.RemoveAttackTargetTiles();
                m_ingameUiManager.UpdateUnitControlState(this);
            }
        }
    }

    public bool DeadCheck(Unit targetUnit)
    {
        if(m_stat_hp <= 0)
        {
            if(this.gameObject.tag == "enemy")
            {
                GameManager.instance.m_enemyUnits.Remove(this);
            }
            else if(this.gameObject.tag == "Player")
            {
                GameManager.instance.m_playerUnits.Remove(this);
            }

            Debug.Log(targetUnit.m_name + " is dead..");
            Destroy(this.gameObject);
            return true;
        }

        return false;
    }

}
