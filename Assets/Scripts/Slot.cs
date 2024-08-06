using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour
{
    public int slotIndex;
    public SlotType slotType;
    public InventoryObject invObjParent;

    private void Start()
    {
        slotIndex = transform.GetSiblingIndex();
    }
}

public enum SlotType
{
    helmet,
    armor,
    boot,
    weapon,
    other
}
