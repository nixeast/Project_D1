using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTarget : MonoBehaviour
{
    public string enemyTag = "enemy";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(enemyTag))
        {
            //this.gameObject.SetActive(false);
        }
    }

    private void OnMouseDown()
    {
        GameManager.instance.MoveUnit(this);
    }

}
