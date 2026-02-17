using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance {get; private set;}
    public Unit currentSelectedUnit;
    
    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
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
    }
}
