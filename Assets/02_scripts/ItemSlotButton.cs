using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class ItemSlotButton : MonoBehaviour
{
    [SerializeField] private int m_slotIndex;
    [SerializeField] private Image m_iconImage;
    [SerializeField] private Button m_button;

    private Action<ItemSlotButton> m_onClicked;

    private void Awake()
    {
        if(m_button != null)
        {
            m_button.onClick.AddListener(OnUnityButtonClicked);
            Debug.Log("m_button.onClicked.AddListener completed..");
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

    private void OnUnityButtonClicked()
    {
        if(m_onClicked != null)
        {
            m_onClicked(this);
            Debug.Log("m_onClicked(this)");
        }
    }

    public void AddOnClicked(Action<ItemSlotButton> listener)
    {
        m_onClicked += listener;
    }

    public void RemoveOnClicked(Action<ItemSlotButton> listener)
    {
        m_onClicked -= listener;
    }

    public int GetSlotIndex()
    {
        return m_slotIndex;
    }

    public void setIconImage(Sprite iconImage)
    {
        m_iconImage.sprite = iconImage;
    }

    public void resetIconImage()
    {
        m_iconImage.sprite = null;
    }

    public Sprite getIconSprite()
    {
        return m_iconImage.sprite;
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
