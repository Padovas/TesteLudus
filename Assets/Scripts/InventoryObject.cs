using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerInventory", menuName = "Inventory System/Create New Inventory")]
public class InventoryObject : ScriptableObject
{
    public List<InventorySlot> Container = new List<InventorySlot>();
    public void AddItem(ItemObject _item, int _amount)
    {
        bool hasItem = false;
        List<int> emptySlots = new List<int>(0);

        for (int i = 0; i < Container.Count; i++)
        {
            if (Container[i].item == _item && _item.isStackable)
            {
                Container[i].AddAmout(_amount);
                hasItem = true;
            }

            if (Container[i].item == null)
            {
                emptySlots.Add(i);
            }
        }

        if (!hasItem)
        {
            if (emptySlots.Count > 0)
            {
                Container[emptySlots[0]].item = _item;
                Container[emptySlots[0]].amount = _amount;
            }
        }
    }

    public void MoveItem(InventorySlot item1, InventorySlot item2)
    {
        InventorySlot temp = new InventorySlot(item2.item, item2.amount);
        item2.UpdateSlot(item1.item, item1.amount);
        item1.UpdateSlot(temp.item, temp.amount);
    }

    public void ResetList(int size)
    {
        Container.Clear();

        for (int i = 0; i < size; i++)
        {
            Container.Add(new InventorySlot(null, 0));
        }

    }

    public void RemoveItem(int _index)
    {
        Container[_index].UpdateSlot(null, 0);
    }

    public void ReduceAmount(int _index, int value)
    {
        if (Container[_index].amount > 1)
        {
            Container[_index].RemoveAmount(value);
        }
        else
        {
            RemoveItem(_index);
        }
    }

    public int GetEmptySlotsCount(ItemObject _item)
    {
        int qntEmptySlots = 0;

        for (int i = 0; i < Container.Count; i++)
        {
            if (Container[i].item == null ^ (Container[i].item == _item && Container[i].item.isStackable && Container[i].amount < Container[i].item.maxStack))
            {
                qntEmptySlots++;
            }
        }

        return qntEmptySlots;
    }
}

    [System.Serializable]
public class InventorySlot
{
    public ItemObject item;
    public int amount;

    public InventorySlot(ItemObject item, int amount)
    {
        this.item = item;
        this.amount = amount;
    }

    public void UpdateSlot(ItemObject item, int amount)
    {
        this.item = item;
        this.amount = amount;
    }

    public void AddAmout(int value)
    {
        amount += value;
    }

    public void RemoveAmount(int value){ 
        amount -= value; 

    }
}