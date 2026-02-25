using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public enum unitControlMode
{
    None = 0,
    Move = 1,
    Attack = 2,

}
public class Unit : MonoBehaviour
{

    //public GameObject tile_moveTarget_true;
    Vector3 currentPosition;
    public unitControlMode currentControlMode;
    public int stat_health;
    public int stat_attack;
    public int stat_defense;
    public int stat_moveRange;
    public int stat_moveRange_modified;
    public SpriteRenderer m_spriteRenderer;
    public bool isCloseCombat;

    // Start is called before the first frame update
    void Start()
    {
        currentPosition = this.transform.position;
        currentControlMode = unitControlMode.None;
        stat_moveRange_modified = stat_moveRange;

        stat_health = 50;
        stat_attack = 33;
        stat_defense = 0;
        stat_moveRange = 2;
           
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeMoveRange()
    {
        //isCloseCombat = true;
        stat_moveRange_modified = stat_moveRange / 2;
    }

    private void OnMouseDown()
    {
        if (GameManager.instance.currentSelectedUnit == null)
        {
            SelectThisUnit();
        }
        else if (GameManager.instance.currentSelectedUnit == this)
        {
            ControlModeChange();
            //Debug.Log("[mode] " + currentControlMode);
        }
        
    }
    public void SelectThisUnit()
    {
        currentControlMode = unitControlMode.Move;
        GameManager.instance.SelectUnit(this);

        if(isCloseCombat == false)
        {
            GameManager.instance.MakeMoveTargets(this, stat_moveRange);
        }
        else
        {
            GameManager.instance.MakeMoveTargets(this, stat_moveRange_modified);
        }
        
        //GameManager.instance.InactiveMoveTargets();

        //Debug.Log("[mode] " + currentControlMode);
    }

    public void ControlModeChange()
    {
        if (currentControlMode == unitControlMode.Move)
        {
            currentControlMode = unitControlMode.Attack;
            //GameManager.instance.ChangeUnitControlMode(this);
            GameManager.instance.ShowAttackableArea(this);
            
        }
        else if (currentControlMode == unitControlMode.Attack)
        {
            currentControlMode = unitControlMode.Move;
            //GameManager.instance.ChangeUnitControlMode(this);
            GameManager.instance.ShowMovableArea(this);

        }
    }

    public void DeadCheck()
    {
        if(stat_health <= 0)
        {
            Debug.Log(this.gameObject + " is dead..");
            Destroy(this.gameObject);
        }
    }

}
