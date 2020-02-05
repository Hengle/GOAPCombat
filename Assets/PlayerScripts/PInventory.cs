using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PInventory : MonoBehaviour
{
    [System.Serializable]
    public struct item
    {
        public string name;
        public GameObject model;
        public GameObject Spawnable;
        public Image icon;
        public int id;
        public bool resource; // item is resource used in crafting
        [Header("Consumable Options")]
        public bool consumable; // item can be consumed
        public float hunger;
        public float thirst;
        [Header("Equippable Options")]
        public bool equippable; //weather or not this item is equippable
        public bool TwoHands; //need 2 hands?
        public bool canMine;
        public float attack; // damage per second
        public float defense; // blocked damage per second
        public string animTrigger; // the animation variable that should be triggered on action
        public int randAnim; // # of animations to pick from: 0 if only 1
        public AudioClip sfx;
        [Header("Crafting Options")]
        public bool isCraftable;
        public int[] recipe;
    };

    [System.Serializable]
    public struct Hand{
        public Transform hand;
        public item handSlot;
        public bool isEmpty;
        public GameObject obj;
    }

    public item[] items;
    [SerializeField]
    public Hand handR; // Primary hand
    [SerializeField]
    public Hand handL; // Secondary hand

    //[SerializeField]
    //public Hand hand;

    public int StorageSize = 10;
    public List <item> storage = new List <item>();
    [SerializeField]
    Health health;

    // Start is called before the first frame update
    void Start()
    {
        //ITEM(S) EQUIP ON START
        handR.isEmpty = false; // is hand empty at start?
        handL.isEmpty = false; // is hand empty at start?
        handR.handSlot = items[15]; // 15 = the item ID to spawn in right hand. 
        handL.handSlot = items[15]; // 15 = the item ID to spawn in left hand. 
        //hand.isEmpty = false;
        //hand.handSlot = items[0];
        Update_Hands();
    }

    public bool UseResource(int id) // use resource with item id
    {
        if (CountResource(id) > 0)
        {
            storage.Remove(storage[findItem(id)]);
            return true;
        }
        else
        {
            return false;
        }
    }

    int findItem (int id) // returns storage index
    {
        for (int i = 0; i < storage.Count; i++)
        {
            if (storage[i].id == id)
            {
                return i;
            }
        }
        return -1;
    }

    public int CountResource(int id) // returns number of resources of id in inventory
    {
        int count = 0;
        for (int i = 0; i < storage.Count; i++)
        {
            if (storage[i].id == id)
            {
                count++;
            }
        }
        return count;
    
    }

    public int AddToStorage(int id) //add item to storage
    {
        if (storage.Count < StorageSize)
        {
            storage.Add(items[id]);
            return 1;
        }
        else
        {
            Debug.Log("Storage Full");
            return -1;
        }
    }

    public void Equip(int ind, char LR) // equip the item at index of inventory in specified hand
    {
        if (storage[ind].equippable) //check if can be equipped
        {
            if (storage[ind].TwoHands) // needs two hands
            {
                Unequip(0); // unequip Right hand
                Unequip(1); // unequip Left hand
                if (LR == 'R')// hold in Right hand
                {
                    handR.handSlot = items[storage[ind].id];
                    handR.isEmpty = false;
                    handL.isEmpty = false;
                }
                else  //hold in left hand
                {
                    handL.handSlot = items[storage[ind].id];
                    handR.isEmpty = false;
                    handL.isEmpty = false;
                }
                storage.Remove(storage[ind]);
            }
            else //does not need two hands
            {
                if (LR == 'R')// equip in Right hand
                {
                    Unequip(0); // unequip Right hand
                    handR.handSlot = items[storage[ind].id];
                    handR.isEmpty = false;
                }
                else  //equip in left hand
                {
                    Unequip(1); // unequip Left hand
                    handL.handSlot = items[storage[ind].id];
                    handL.isEmpty = false;
                }
            }

        }
        Update_Hands();
    }

    public void Unequip(int h) //take item from hand and put into inventory (h=0:HandR; h=1:HandL)
    {
        
        if (h == 0)
        {
            if (!handR.isEmpty)// ensure hand is not empty
            {
                if (AddToStorage(handR.handSlot.id) == 1) //ensure was able to add to inventory
                {
                    handR.isEmpty = true;
                    handR.handSlot = items[0]; //item[0] is the empty item
                }
                else
                {
                    Debug.Log("Inventory Full");
                }
            }
        }else if(h == 1)
        {
            if (!handL.isEmpty)// ensure hand is not empty
            {
                if (AddToStorage(handL.handSlot.id) == 1) //ensure was able to add to inventory
                {
                    handL.isEmpty = true;
                    handL.handSlot = items[0]; //item[0] is the empty item
                }
                else
                {
                    Debug.Log("Inventory Full");
                }
            }
        }


        Update_Hands();
    }

    public void Consume(int ind){ // consume a consumable item
        if (storage[ind].consumable){ // make sure item is consumable
            
            float Hgr = storage[ind].hunger;
            float Thrst = storage[ind].thirst;
            health.hunger += Hgr;
            health.hydration += Thrst;
            if (health.hunger > 100){
                health.hunger = 100;
            }
            if (health.hydration > 100){
                health.hydration = 100;
            }
            health.update_Txt();
            storage.Remove(storage[ind]);// remove item from inventory
        }
    }

    public void Drop(int ind) // drop an item at the specified inventory slot
    {
        Instantiate(storage[ind].Spawnable, transform.position + new Vector3(0, 1, 0), Quaternion.identity);
        storage.Remove(storage[ind]);// remove item from inventory
    }

    void Update_Hands(){ // Add or remove object to players hand. Called when item is equipped or unequipped
        
        if(!handR.isEmpty){ //if hand is not empty and no item is in hand, spawn item in hand
            handR.obj = Instantiate(handR.handSlot.Spawnable);
            handR.obj.GetComponent<Rigidbody>().isKinematic = true; // set the object's rigidbody to kinematic
            handR.obj.transform.position = handR.hand.transform.position;
            handR.obj.transform.parent = handR.hand.transform;
            handR.obj.tag = "Equip"; //give spawn item the 'Equip' tag (used for detecting when hitting stuff)
            handR.obj.transform.localRotation = handR.handSlot.Spawnable.transform.rotation;
        }
        else {
            Destroy(handR.obj);
        }
        if(!handL.isEmpty)
        { //if hand is not empty and no item is in hand, spawn item in hand
            handL.obj = Instantiate(handL.handSlot.Spawnable);
            handL.obj.GetComponent<Rigidbody>().isKinematic = true; // set the object's rigidbody to kinematic
            handL.obj.transform.position = handL.hand.transform.position;
            handL.obj.transform.parent = handL.hand.transform;
            handL.obj.tag = "Equip"; //give spawn item the 'Equip' tag (used for detecting when hitting stuff)
            handL.obj.transform.localRotation = handL.handSlot.Spawnable.transform.rotation;
        }
        else
        {
            Destroy(handL.obj);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
