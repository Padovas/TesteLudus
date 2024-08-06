using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerStats : MonoBehaviour
{
    public int health, maxHealth, baseArmor, aditionalArmor, baseSpeed, aditionalSpeed, baseDamage, aditionalDamage, slotsQuantity;
    public InventoryObject inventory;

    private void Start()
    {
        inventory.ResetList(slotsQuantity);
    }

    private void OnTriggerEnter2D(Collider2D other)
    { 
        if(other.gameObject.tag == "Item")
        {
            if (inventory.GetEmptySlotsCount(other.GetComponent<Item>().item) > 0)
            {
                inventory.AddItem(other.GetComponent<Item>().item, 1);
                Destroy(other.gameObject);
            }
            else
            {
                Debug.Log("Inventário Cheio!");
            }
            
        }
    }
}
