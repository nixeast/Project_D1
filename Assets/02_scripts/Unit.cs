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

    // Start is called before the first frame update
    void Start()
    {
        currentPosition = this.transform.position;
        currentControlMode = unitControlMode.None;

        stat_health = 50;
        stat_attack = 33;
        stat_defense = 0;
           
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnMouseDown()
    {
        if(GameManager.instance==null)
        {
            Debug.Log("game manger null..");
        }

        if (GameManager.instance.currentSelectedUnit == null)
        {
            currentControlMode = unitControlMode.Move;
            GameManager.instance.SelectUnit(this);
            GameManager.instance.MakeMoveTargets(this);
            Debug.Log("[mode] " + currentControlMode);
        }
        else if (GameManager.instance.currentSelectedUnit == this)
        {
            if(currentControlMode == unitControlMode.Move)
            {
                currentControlMode = unitControlMode.Attack;
                GameManager.instance.ChangeUnitControlMode(this);
            }
            else if(currentControlMode == unitControlMode.Attack)
            {
                currentControlMode = unitControlMode.Move;
                GameManager.instance.ChangeUnitControlMode(this);
            }
            
            Debug.Log("[mode] " + currentControlMode);
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
