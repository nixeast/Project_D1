using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTarget : MonoBehaviour
{
    public Unit assignedUnit;
    public string enemyTag = "enemy";

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(enemyTag) || other.CompareTag("Player"))
        {
            Debug.Log("attack target detected..");
            //Destroy(gameObject);
            //this.gameObject.SetActive(false);
            
            assignedUnit = other.gameObject.GetComponent<Unit>();
            if(assignedUnit != null)
            {
                Debug.Log("attack target: unit component checked..");
            }
            else
            {
                Debug.Log("attack target: null");
            }
        }
    }

    private void OnMouseDown()
    {
        GameManager.instance.AttackUnit(this);

    }
}
