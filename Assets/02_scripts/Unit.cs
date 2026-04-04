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
    public eUnitControlMode m_currentControlMode;
    public SpriteRenderer m_spriteRenderer;
    public bool isCloseCombat;
    public eUnitState e_currentUnitState = eUnitState.Default;
    public List<Unit> m_closeCombatOpponents = new List<Unit>();
    public UnitSaveData m_unitSaveData;

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

    [Header("Unit Command")]
    public GameManager m_gameManager;

    public void Awake()
    {
        m_unitDatabase = GameObject.FindObjectOfType<UnitDataBase>();
        m_gameManager = GameObject.FindObjectOfType<GameManager>();
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
        //isCloseCombat = true;
        m_stat_moveRange_modified = m_stat_moveRange / 2;
    }

    private void OnMouseDown()
    {
        //Debug.Log(m_name + "clicked");

        //SelectThisUnit();
        m_gameManager.SelectUnit(this);

        if (m_isMoved == false)
        {
            m_currentControlMode = eUnitControlMode.Move;
            m_gameManager.MakeMoveTargets(this, m_stat_ap);
        }
        else
        {
            if(m_currentControlMode == eUnitControlMode.MoveEnd)
            {
                m_currentControlMode = eUnitControlMode.Default;
                m_gameManager.RemoveAttackTargetTiles();
                //m_gameManager.MakeAttackTargets(this);
            }
        }

        //m_gameManager.MakeAttackTargets(this, stat_ap);

        //if (GameManager.instance.currentSelectedUnit == null)
        //{
        //    SelectThisUnit();
        //}
        //else if (GameManager.instance.currentSelectedUnit == this)
        //{
        //    ControlModeChange();
        //}

    }
    public void SelectThisUnit()
    {
        
        
        
        //if(e_currentUnitState == eUnitState.Default)
        //{
        //    GameManager.instance.MakeMoveTargets(this, stat_moveRange);
        //}
        //else if(e_currentUnitState == eUnitState.CloseCombat)
        //{
        //    GameManager.instance.MakeMoveTargets(this, stat_moveRange/2);
        //}

    }

    //public void ControlModeChange()
    //{
    //    if (currentControlMode == unitControlMode.Move)
    //    {
    //        currentControlMode = unitControlMode.Attack;
    //        //GameManager.instance.ChangeUnitControlMode(this);
    //        GameManager.instance.ShowAttackableArea(this);
            
    //    }
    //    else if (currentControlMode == unitControlMode.Attack)
    //    {
    //        currentControlMode = unitControlMode.Move;
    //        //GameManager.instance.ChangeUnitControlMode(this);
    //        GameManager.instance.ShowMovableArea(this);

    //    }
    //}

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
            
            Destroy(this.gameObject);
            Debug.Log(this.gameObject + " is dead..");
            return true;
        }

        return false;
    }

}
