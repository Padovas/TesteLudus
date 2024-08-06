using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newItem", menuName = "Inventory System/Create New Item")]
public class ItemObject : ScriptableObject
{
    public Sprite icon;
    public string itemName;
    public string description;
    [Space(5)]
    [Header("Stack")]
    public bool isStackable = false;
    public float maxStack = 100;
    [Space(5)]
    public ItemType type;
    [Space(5)]
    [Header("Modifier Attribute")]
    public ChangeProperty changeProperty;
    public string propertyName;
    public int propertyValue;
}

public enum ItemType
{
    helmet,
    armor,
    boot,
    weapon,
    consumable,
    generic
}

public enum ChangeProperty
{
    heal,
    improveMaxHealth,
    improveArmor,
    improveSpeed,
    improveAtkDamage
}