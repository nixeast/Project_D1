using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartingPointButton : MonoBehaviour
{
    [SerializeField] private SpriteRenderer m_spriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnMouseDown()
    {
        //Debug.Log("starting point clicked..");
        Instantiate(GameManager.instance.m_testUnit, this.gameObject.transform.position, Quaternion.identity);
        m_spriteRenderer.sprite = null;
    }
}
