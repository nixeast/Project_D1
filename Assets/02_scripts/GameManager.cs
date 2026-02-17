using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance {get; private set;}
    public Unit currentSelectedUnit;
    Vector3[] movePositions = new Vector3[4];
    public GameObject tile_moveTarget_true;
    GameObject[] moveTargets = new GameObject[4];
    
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
        RemoveMoveTargetTiles();
        currentSelectedUnit = null;

        for(int i=0 ; i < 4 ; i++)
        {
            movePositions[i].x = 0f;
            movePositions[i].y = 0f;
            movePositions[i].z = 0f;
        }
        movePositions[0].x=1.0f;
        movePositions[1].x=-1.0f;
        movePositions[2].y=1.0f;
        movePositions[3].y=-1.0f;


    }

    public void MakeMoveTargets(Unit selectedUnit)
    {
        Debug.Log("make moveTargetTiles..");

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
}
