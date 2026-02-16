using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Unit : MonoBehaviour
{

    public GameObject tile_moveTarget_true;
    Vector3 currentPosition;
    Vector3[] movePosittions = new Vector3[4];
    
    // Start is called before the first frame update
    void Start()
    {
        currentPosition = this.transform.position;
        
        movePosittions[0] = currentPosition + new Vector3(1.0f,0f,0f); 
        movePosittions[1] = currentPosition + new Vector3(-1.0f,0f,0f); 
        movePosittions[2] = currentPosition + new Vector3(0f,1.0f,0f); 
        movePosittions[3] = currentPosition + new Vector3(0f,-1.0f,0f); 
           
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnMouseDown()
    {
        Debug.Log("clicked");

        for(int i=0 ; i < 4 ; i++)
        {
            Instantiate(tile_moveTarget_true,movePosittions[i],Quaternion.identity);
        }
        
        
    }


}
