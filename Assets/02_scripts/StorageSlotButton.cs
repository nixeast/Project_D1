using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class StorageSlotButton : MonoBehaviour
{
    public TMP_Text tmp_slotName;
    public int m_storageSlotNumber;
    public string m_storageSlotName;

    [SerializeField] private Button m_button;

    private Action<StorageSlotButton> m_onClicked;

    private void Awake()
    {
        if(m_button != null)
        {
            m_button.onClick.AddListener(OnUnityButtonClicked);
        }
    }
    private void OnDestroy()
    {
        if(m_button != null)
        {
            m_button.onClick.RemoveListener(OnUnityButtonClicked);
        }
        m_onClicked = null;
    }
    public void InitStorageSlotButton(int slotNumber)
    {
        tmp_slotName.text = slotNumber.ToString();
    }

    public void OnUnityButtonClicked()
    {
        if (m_onClicked != null)
        {
            m_onClicked(this);
            Debug.Log("m_storageSlotNumber: " + m_storageSlotNumber);
        }
        else if(m_onClicked == null)
        {
            Debug.Log("m_onClicked is null");
        }
    }

    public void AddOnClicked(Action<StorageSlotButton> listener)
    {
        m_onClicked = listener;
    }

    public void RemoveOnClicked(Action<StorageSlotButton> listener)
    {
        m_onClicked -= listener;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
}
