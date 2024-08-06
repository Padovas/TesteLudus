using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using Unity.VisualScripting;

public class InterfaceContoller : MonoBehaviour
{
    private MouseItem mouseItem = new MouseItem();
    private int selectedSlotIndex = -1;
    private InventorySlot selectedSlot = null;

    public Canvas canvasStatus, canvasInventory;
    public PostProcessVolume ppVolume;
    public GameObject hintInventory, hintStatus;
    public TMP_Text txtVidasUI;
    public PlayerStats playerStats;

    [Header("Inventory")]
    public InventoryObject inventory;
    public GameObject slotsParent;
    public GameObject itemPrefab;

    [Header("Equipments")]
    public InventoryObject equipmentSlots;
    public GameObject equipmentSlotsParent;

    [Header("Informations - Inventory")]
    public GameObject emptyText;
    public GameObject infosParent;
    [Space(5)]
    public TMP_Text txtTitle;
    public TMP_Text txtDescription;
    public Button btnRemove, btnUse;
    public TMP_Text txtUseButton;
    [Space(5)]
    public TMP_Text txtChanges;

    [Header("Informations - Status")]
    public TMP_Text txtActualLife;
    public TMP_Text txtArmor;
    public TMP_Text txtAttack;
    public TMP_Text txtSpeed;

    private bool hasSomeMenuOpened = false;

    private void Start()
    {
        CloseInventory();
        CloseStatus();

        hintInventory.SetActive(true);
        hintStatus.SetActive(true);

        equipmentSlots.ResetList(4);
    }

    private void Update()
    {
        if (Input.GetButtonDown("Inventory"))
        {
            if (canvasInventory.isActiveAndEnabled)
            {
                CloseInventory();
            }
            else
            {
                if (!hasSomeMenuOpened)
                {
                    OpenInventory();
                }
            }
        }

        if (Input.GetButtonDown("Status") && !hasSomeMenuOpened)
        {
            OpenStatus();
        }

        if (Input.GetButtonUp("Status") && canvasStatus.isActiveAndEnabled)
        { 
            CloseStatus();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            if (playerStats.health > 1)
            {
                playerStats.health -= 1;
                txtVidasUI.text = playerStats.health + " / " + playerStats.maxHealth;
            }
        }
    }

    public void OpenInventory()
    {
        canvasInventory.enabled = true;
        ppVolume.enabled = true;
        hintInventory.SetActive(false);
        hasSomeMenuOpened = true;
        UpdateInventory(inventory, slotsParent, true);
        UpdateInventory(equipmentSlots, equipmentSlotsParent, true);
    }

    public void CloseInventory()
    {
        canvasInventory.enabled = false;
        ppVolume.enabled = false;
        hasSomeMenuOpened = false;
    }

    public void OpenStatus()
    {
        canvasStatus.enabled = true;
        ppVolume.enabled = true;
        hintStatus.SetActive(false);
        hasSomeMenuOpened = true;

        UpdateTxtStats();
    }

    public void CloseStatus()
    {
        canvasStatus.enabled = false;
        ppVolume.enabled = false;
        hasSomeMenuOpened = false;
    }

    public void UpdateInventory(InventoryObject _inv, GameObject _slotsParent, bool addTriggers = false)
    {
        for (int i = 0; i < _inv.Container.Count; i++)
        {

            if (_inv.Container[i].item != null)
            {
                if (_slotsParent.transform.GetChild(i).transform.childCount != 0)
                {
                    Destroy(_slotsParent.transform.GetChild(i).transform.GetChild(0).gameObject);
                }

                var newItem = Instantiate(itemPrefab, _slotsParent.transform.GetChild(i).transform);
                newItem.GetComponent<Image>().sprite = _inv.Container[i].item.icon;

                newItem.GetComponent<ItemInSlot>().item = _inv.Container[i].item;

                newItem.transform.GetChild(0).GetComponent<TMP_Text>().text = _inv.Container[i].item.isStackable ? _inv.Container[i].amount.ToString("n0") : "";

                if(addTriggers){
                    AddEvent(newItem, EventTriggerType.BeginDrag, delegate { OnDragStart(newItem); });
                    AddEvent(newItem, EventTriggerType.EndDrag, delegate { OnDragEnd(newItem); });
                    AddEvent(newItem, EventTriggerType.Drag, delegate { OnDragging(newItem); });
                    AddEvent(newItem, EventTriggerType.PointerClick, delegate { OnClicked(newItem); });
                    AddEvent(newItem, EventTriggerType.PointerEnter, delegate { OnEnter(newItem); });
                    AddEvent(newItem, EventTriggerType.PointerExit, delegate { OnExit(); });
                }
            }
            else
            {
                if (_slotsParent.transform.GetChild(i).transform.childCount != 0)
                {
                    Destroy(_slotsParent.transform.GetChild(i).transform.GetChild(0).gameObject);
                }
            }
        }
    }

    public void UpdateTxtStats()
    {
        txtActualLife.text = "Vida - " + playerStats.health + " / " + playerStats.maxHealth;
        txtArmor.text = "Armadura - " + (playerStats.baseArmor + playerStats.aditionalArmor);
        txtAttack.text = "Dano de Ataque - " + (playerStats.baseDamage + playerStats.aditionalDamage);
        txtSpeed.text = "Velocidade - " + (playerStats.baseSpeed + playerStats.aditionalSpeed);
    }

    public void UpdateStats()
    {
        playerStats.aditionalArmor = 0;
        playerStats.aditionalDamage = 0;
        playerStats.aditionalSpeed = 0;

        for (int i = 0; i < equipmentSlots.Container.Count; i++)
        {
            var localItem = equipmentSlots.Container[i].item;

            if (localItem != null)
            {
                if(localItem.changeProperty == ChangeProperty.improveArmor)
                {
                    playerStats.aditionalArmor += localItem.propertyValue;
                }
                if (localItem.changeProperty == ChangeProperty.improveAtkDamage)
                {
                    playerStats.aditionalDamage += localItem.propertyValue;
                }
                if (localItem.changeProperty == ChangeProperty.improveSpeed)
                {
                    playerStats.aditionalSpeed += localItem.propertyValue;
                }
            }
        }
    }

    private void AddEvent(GameObject obj, EventTriggerType type, UnityAction<BaseEventData> action)
    {
        EventTrigger trigger = obj.GetComponent<EventTrigger>();
        var eventTrigger = new EventTrigger.Entry();
        eventTrigger.eventID = type;
        eventTrigger.callback.AddListener(action);
        trigger.triggers.Add(eventTrigger);
    }

    public void OnSlotEnter(Slot _slot) 
    {
        mouseItem.indexHoverObj = _slot.slotIndex;
        mouseItem.hoveredSlot = _slot.invObjParent.Container[_slot.slotIndex];
        mouseItem.hoveredSlotType = _slot.slotType.ToString();
    }

    public void OnEnter(GameObject obj)
    {
        mouseItem.hoveredObj = obj;
        mouseItem.indexHoverObj = obj.transform.parent.GetComponent<Slot>().slotIndex;
        mouseItem.hoveredSlotType = obj.transform.parent.GetComponent<Slot>().slotType.ToString();

        if (obj.transform.GetChild(0) != null) {
            mouseItem.hoveredSlot = obj.transform.parent.GetComponent<Slot>().invObjParent.Container[mouseItem.indexHoverObj];
        }
    }

    public void OnExit()
    {
        mouseItem.hoveredObj = null;
        mouseItem.hoveredSlot = null;
        mouseItem.indexHoverObj = 0;
    }

    public void OnClicked(GameObject obj)
    {
        SelectItem(obj);
        selectedSlotIndex = obj.transform.parent.GetSiblingIndex();
    }

    public void OnDragStart(GameObject obj)
    {
        SelectItem(obj);

        var mouseObject = new GameObject();
        var rt = mouseObject.AddComponent<RectTransform>();
        var img = mouseObject.AddComponent<Image>();
        rt.sizeDelta = new Vector2(50,50);
        mouseObject.transform.SetParent(canvasInventory.transform);

        img.sprite = obj.GetComponent<Image>().sprite;
        Color tempcolor = img.color;
        tempcolor.a = 0.5f;
        img.color = tempcolor;
        img.raycastTarget = false;

        mouseItem.obj = mouseObject;
        mouseItem.slot = obj.transform.parent.GetComponent<Slot>().invObjParent.Container[obj.transform.parent.GetSiblingIndex()];
        mouseItem.indexObj = obj.transform.parent.GetSiblingIndex();
    }

    public void OnDragEnd(GameObject obj)
    {
        var droppedItem = obj.GetComponent<ItemInSlot>().item;

        if (mouseItem.hoveredSlot != null)
        {
            if (mouseItem.hoveredSlotType == "other")
            {
                inventory.MoveItem(mouseItem.slot, mouseItem.hoveredSlot);
            }
            else if (droppedItem.type.ToString() == mouseItem.hoveredSlotType)
            {
                inventory.MoveItem(mouseItem.slot, mouseItem.hoveredSlot);
            }
        }
        else
        {
            inventory.RemoveItem(mouseItem.indexObj);
        }

        UpdateInventory(inventory, slotsParent, true);
        UpdateInventory(equipmentSlots, equipmentSlotsParent, true);
        UpdateStats();
        UnselectItem();
        Destroy(mouseItem.obj);
    }

    public void OnDragging(GameObject obj)
    {
        if(mouseItem.obj != null)
            mouseItem.obj.GetComponent<RectTransform>().position = Input.mousePosition;
    }

    public void ButtonRemoveItem ()
    {
        if (selectedSlotIndex != -1)
        {
            inventory.RemoveItem(selectedSlotIndex);
            selectedSlotIndex = -1;
            UpdateInventory(inventory, slotsParent, true);
            UpdateInventory(equipmentSlots, equipmentSlotsParent, true);
            UnselectItem();
        }
    }

    private void SelectItem(GameObject _obj)
    {
        emptyText.SetActive(false);
        infosParent.SetActive(true);

        for (int i = 0; i < slotsParent.transform.childCount; i++)
        {
            slotsParent.transform.GetChild(i).GetComponent<Image>().color = Color.white;
        }

        _obj.transform.parent.gameObject.GetComponent<Image>().color = Color.yellow;

        txtTitle.text = _obj.GetComponent<ItemInSlot>().item.itemName;
        txtDescription.text = _obj.GetComponent<ItemInSlot>().item.description;

        if (_obj.GetComponent<ItemInSlot>().item.type != ItemType.generic)
        {
            btnUse.gameObject.SetActive(true);
            txtUseButton.text = _obj.GetComponent<ItemInSlot>().item.type == ItemType.consumable ? "usar" : "equipar";
        }
        else
        {
            btnUse.gameObject.SetActive(false);
        }

        if (_obj.GetComponent<ItemInSlot>().item.type != ItemType.generic )
        {
            txtChanges.gameObject.SetActive(true);
            txtChanges.text = _obj.GetComponent<ItemInSlot>().item.propertyName + " +" + _obj.GetComponent<ItemInSlot>().item.propertyValue;
        }
        else
        {
            txtChanges.gameObject.SetActive(false);
        }

        selectedSlot = _obj.transform.parent.gameObject.GetComponent<Slot>().invObjParent.Container[_obj.transform.parent.GetSiblingIndex()];
    }

    private void UnselectItem()
    {
        emptyText.SetActive(true);
        infosParent.SetActive(false);
        selectedSlot = null;
        selectedSlotIndex = -1;

        for (int i = 0; i < slotsParent.transform.childCount; i++)
        {
            slotsParent.transform.GetChild(i).GetComponent<Image>().color = Color.white;
        }

        for (int i = 0; i < equipmentSlotsParent.transform.childCount; i++)
        {
            equipmentSlotsParent.transform.GetChild(i).GetComponent<Image>().color = Color.white;
        }
    }

    public void UseItem()
    {
        for (int i = 0; i < equipmentSlots.Container.Count; i++)
        {
            Slot slot = equipmentSlotsParent.transform.GetChild(i).GetComponent<Slot>();

            if (selectedSlot.item.type == ItemType.boot && slot.slotType == SlotType.boot)
            {
                equipmentSlots.MoveItem(selectedSlot, equipmentSlots.Container[i]);
                break;
            }else if (selectedSlot.item.type == ItemType.helmet && slot.slotType == SlotType.helmet)
            {
                equipmentSlots.MoveItem(selectedSlot, equipmentSlots.Container[i]);
                break;
            }
            else if (selectedSlot.item.type == ItemType.armor && slot.slotType == SlotType.armor)
            {
                equipmentSlots.MoveItem(selectedSlot, equipmentSlots.Container[i]);
                break;
            }
            else if (selectedSlot.item.type == ItemType.weapon && slot.slotType == SlotType.weapon)
            {
                equipmentSlots.MoveItem(selectedSlot, equipmentSlots.Container[i]);
                break;
            }else if (selectedSlot.item.type == ItemType.consumable)
            {
                if (selectedSlot.item.changeProperty == ChangeProperty.heal)
                {
                    if (playerStats.health < playerStats.maxHealth)
                    {
                        playerStats.health += selectedSlot.item.propertyValue;

                        if (playerStats.health > playerStats.maxHealth)
                            playerStats.health = playerStats.maxHealth;

                        txtVidasUI.text = playerStats.health + " / " + playerStats.maxHealth;
                    }
                    else
                    {
                        Debug.Log("Você já está com a vida cheia!");
                        break;
                    }
                    
                }else if (selectedSlot.item.changeProperty == ChangeProperty.improveSpeed)
                {
                    playerStats.baseSpeed += selectedSlot.item.propertyValue;
                }
                
                inventory.ReduceAmount(selectedSlotIndex, 1);
                break;
            }
        }

        UpdateInventory(inventory, slotsParent, true);
        UpdateInventory(equipmentSlots, equipmentSlotsParent, true);
        UnselectItem();
    }
}

public class MouseItem
{
    public int indexObj, indexHoverObj;
    public string hoveredSlotType;
    public GameObject obj, hoveredObj;
    public InventorySlot slot, hoveredSlot;
}
