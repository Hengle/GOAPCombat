using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField]
    Image[] InvSlots_img;
    int index = 0;
    [SerializeField]
    GameObject InvUI_Obj;
    [SerializeField]
    PInventory inv;
    

    bool isShowing;

    int selection = -1;

    // Start is called before the first frame update
    void Start()
    {
        InvUI_Obj.SetActive(false);
        UpdateUI();
    }

    void UpdateUI()
    {
        for (int i = 0; i < inv.StorageSize; i++)
        {
            if (i < inv.storage.Count)
            {
               
                InvSlots_img[i] = inv.storage[i].icon; // set slot icon
            }
            else
            {
                InvSlots_img[i] = inv.items[0].icon; //empty icon
            }
        }
        //InvSlots_txt[inv.StorageSize].text = inv.handR.handSlot.name;
        //InvSlots_txt[inv.StorageSize+1].text = inv.handL.handSlot.name;
        InvSlots_img[inv.StorageSize - 2] = inv.handR.handSlot.icon;
        InvSlots_img[inv.StorageSize - 1] = inv.handL.handSlot.icon;
    }

    public void Select(int index)
    {
        selection = index;
        for (int i = 0; i < InvSlots_img.Length; i++) // unhighlight all
        {
            InvSlots_img[i].transform.parent.gameObject.GetComponent<Image>().color = Color.white;
        }
        InvSlots_img[index].transform.parent.gameObject.GetComponent<Image>().color = Color.grey;// highlight selected
    }

    public void Equip(char LR) // Unequip or Equip to certain hand
    {
        
        if (selection == inv.StorageSize) // Unequip HandR
        {
            inv.Unequip(1);
        }else if (selection == inv.StorageSize + 1) // Unequip HandL
        {
            inv.Unequip(0);
        } else if (selection != -1) //regular inventory slot
        {
            inv.Equip(selection, LR);
        }


        UpdateUI(); //Update UI before leaving
    }

    public void Consume(){
        inv.Consume(selection);
        UpdateUI(); //Update UI before leaving
    }

    public void Drop()
    {
        inv.Drop(selection);
        UpdateUI(); //Update UI before leaving
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Inventory"))
        {
            if (isShowing)
            {
                PlayerController.inMenu = false; // let everyone know player is not in a menu
                InvUI_Obj.SetActive(false);
                Cursor.visible = false;
                Screen.lockCursor = true;
                isShowing = false;
            }
            else
            {
                PlayerController.inMenu = true; // let everyone know player is in a menu
                UpdateUI();
                InvUI_Obj.SetActive(true);
                Cursor.visible = true;
                Screen.lockCursor = false;
                isShowing = true;
            }
        }
    }
}
