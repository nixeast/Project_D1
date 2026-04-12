using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemySpawnTile : MonoBehaviour
{
    public GameObject prefab_enemyUnit;
    public Vector3 m_spawnPoint;
    public int m_createTurn;
    public Transform m_createParent;
    public GameManager m_gameManager;

    // Start is called before the first frame update
    void Start()
    {
        m_spawnPoint.x = transform.position.x;
        m_spawnPoint.y = transform.position.y;
        m_spawnPoint.z = -1.0f;
        m_gameManager = GameManager.instance;
        m_createParent = m_gameManager.m_stageManager.tileRoot_object;
        m_gameManager.m_enemySpawnTileList.Add(this);
    }

    public void CreateEnemy()
    {
        GameObject newEnemy = Instantiate(prefab_enemyUnit, m_spawnPoint, Quaternion.identity, m_createParent);
        //newUnit.transform.position = m_spawnPoint;
        Unit newUnit = newEnemy.GetComponent<Unit>();
        //m_gameManager.m_enemyUnits.Add(newUnit);
    }

    public void CreateCheck()
    {
        if(GameManager.instance.m_currentTurn != m_createTurn)
        {
            return;
        }

        Collider2D hit = null;
        Vector2 newPos = new Vector2();
        newPos.x = m_spawnPoint.x;
        newPos.y = m_spawnPoint.y;
        hit = Physics2D.OverlapPoint(newPos, LayerMask.GetMask("Unit"));
        
        if (hit == null)
        {
            CreateEnemy();
        }
        else
        {
            //m_gameManager.m_enemySpawnTileList.Remove(this);
            //Destroy(this.gameObject);
        }

    }

}
