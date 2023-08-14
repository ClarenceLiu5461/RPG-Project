using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Keep track of equipment . Has functions for adding and removeing items.*/

public class EquipmentManager : MonoBehaviour
{
    #region Singleton
    public static EquipmentManager instance;

    void Awake()
    {
        instance = this;
    }
    #endregion

    public SkinnedMeshRenderer targetMesh;
    Equipment[] currentEquipment; //Items player current have equipped
    SkinnedMeshRenderer[] currentMeshes;

    //Callback for when an item is equipped/unequipped
    public delegate void OnEquipmentChanged(Equipment newItem, Equipment oldItem);
    public OnEquipmentChanged onEquipmentChanged;

    Inventory inventory; //Reference to our inventory

    void Start()
    {
       inventory = Inventory.instance; //Get a reference to our inventory
       
       //Initialize currentEquipment based on number of equipment slots 
       int numSlots = System.Enum.GetNames(typeof(EquipmentSlot)).Length;
       currentEquipment = new Equipment[numSlots];
       currentMeshes = new SkinnedMeshRenderer[numSlots]; 
    }

    //Equip a new item
    public void Equip(Equipment newItem)
    {
        //Find out what slot the item fits in
        int slotIndex = (int)newItem.equipslot;

        Equipment oldItem = null;

        //If there was already an item in the slot
        //make sure tp put it back in the inventory
        if (currentEquipment[slotIndex] != null)
        {
            oldItem = currentEquipment[slotIndex];
            inventory.Add(oldItem);
        }

        //An item has been equipped so we trigger the callback
        if (onEquipmentChanged != null)
        {
            onEquipmentChanged.Invoke(newItem, oldItem);
        }

        //Insert the item into the slot
        currentEquipment[slotIndex] = newItem;
        SkinnedMeshRenderer newMesh = Instantiate<SkinnedMeshRenderer>(newItem.mesh);
        newMesh.transform.parent = targetMesh.transform;

        newMesh.bones = targetMesh.bones;
        newMesh.rootBone = targetMesh.rootBone;
        currentMeshes[slotIndex] = newMesh;
    }

    //Unequip an item with a particular index
    public void Unequip(int slotIndex)
    {
        //Only do this if an item is there
        if (currentEquipment[slotIndex] != null)
        {
            if (currentMeshes[slotIndex] != null)
            {
                Destroy(currentMeshes[slotIndex].gameObject);
            }
            //Add the item to the inventory
            Equipment oldItem = currentEquipment[slotIndex];
            inventory.Add(oldItem);

            //Remove the item from equipment array
            currentEquipment[slotIndex] = null;

            //Equipment has been removed so we trigger the callback
            if (onEquipmentChanged != null)
            {
                onEquipmentChanged.Invoke(null, oldItem);
            }
        }
    }

    public void UnequipAll()
    {
        for (int i = 0; i <currentEquipment.Length; i++)
        {
            Unequip(i);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            UnequipAll();
        }
    }
}
