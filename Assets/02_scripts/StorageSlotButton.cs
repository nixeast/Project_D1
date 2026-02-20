using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StorageSlotButton : MonoBehaviour
{
    public TMP_Text tmp_slotName;

    public int m_storageSlotNumber;
    public string m_storageSlotName;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitStorageSlotButton(int slotNumber)
    {
        tmp_slotName.text = slotNumber.ToString();
    }
}
