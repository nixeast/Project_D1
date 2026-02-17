using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance {get; private set;}
    public Unit currentSelectedUnit;

    public GameObject tile_moveTarget_true;
    public GameObject tile_attackTarget_true;

    Vector3[] movePositions = new Vector3[4];
    Vector3[] attackPositions = new Vector3[4];
    
    GameObject[] moveTargets = new GameObject[4];
    GameObject[] attackTargets = new GameObject[4];

    private void Awake()
    {
        instance = this;
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

    }

    // Update is called once per frame
    void Update()
    {
        
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

}
