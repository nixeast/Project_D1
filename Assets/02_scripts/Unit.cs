using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Unit : MonoBehaviour
{

    //public GameObject tile_moveTarget_true;
    Vector3 currentPosition;
    
    
    // Start is called before the first frame update
    void Start()
    {
        currentPosition = this.transform.position;
        

           
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnMouseDown()
    {
        Debug.Log("clicked");

        GameManager.instance.MakeMoveTargets(this);
        
        if(GameManager.instance==null)
        {
            Debug.Log("game manger null..");
        }

        if(GameManager.instance.currentSelectedUnit == null)
        {
            GameManager.instance.SelectUnit(this);
        }
        
    }


}
