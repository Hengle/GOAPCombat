using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Crafting : MonoBehaviour
{
    [SerializeField]
    PInventory inv;
    public GameObject CraftingUI;
    bool isShowing;
    [SerializeField]
    TextMeshProUGUI R_Stick_txt;
    [SerializeField]
    TextMeshProUGUI R_Rock_txt;
    [SerializeField]
    TextMeshProUGUI R_Vine_txt;
    [SerializeField]
    TextMeshProUGUI R_Iron_txt;

    [SerializeField]
    TextMeshProUGUI Rec_Fire_txt;
    [SerializeField]
    TextMeshProUGUI Rec_Axe_txt;
    [SerializeField]
    TextMeshProUGUI Rec_Pickaxe_txt;
    [SerializeField]
    TextMeshProUGUI Rec_Spear_txt;
    [SerializeField]
    TextMeshProUGUI Rec_Bow_txt;
    [SerializeField]
    TextMeshProUGUI Rec_Arrow_txt;

    int sticks, rocks, vines, iron;
    // Start is called before the first frame update
    void Start()
    {
        Screen.lockCursor = true;
        CraftingUI.SetActive(false);
    }

    public void UpdateResources() //checks resources available & updates UI
    {
        sticks = inv.CountResource(1);
        rocks = inv.CountResource(2);
        vines = inv.CountResource(3);
        iron = inv.CountResource(7);

        R_Stick_txt.text = "Sticks: " + sticks;
        R_Rock_txt.text = "Rocks: " + rocks;
        R_Vine_txt.text = "Vines: " + vines;
        R_Iron_txt.text = "Iron: " + iron;
    }

    void CheckRecipes() //check what the player is able to craft
    {
        if (sticks >= 3 && rocks >= 2) { Rec_Fire_txt.color = Color.green; } else { Rec_Fire_txt.color = Color.red; } // Camp Fire
        if (sticks >= 1 && rocks >= 1 && vines >=1) { Rec_Axe_txt.color = Color.green; } else { Rec_Axe_txt.color = Color.red; } // Axe
        if (sticks >= 1 && rocks >= 1) { Rec_Pickaxe_txt.color = Color.green; } else { Rec_Pickaxe_txt.color = Color.red; } // Pickaxe
        if (sticks >= 1 && iron >= 1) { Rec_Spear_txt.color = Color.green; } else { Rec_Spear_txt.color = Color.red; } // Spear
        if (sticks >= 1 && vines >= 3) { Rec_Bow_txt.color = Color.green; } else { Rec_Bow_txt.color = Color.red; } // Bow
        if (sticks >= 1 && rocks >= 1 && vines >= 1) { Rec_Arrow_txt.color = Color.green; } else { Rec_Arrow_txt.color = Color.red; } // Arrow


    }


    public void Craft_Fire() // Craft Camp fire; Requires 3 Sticks & 2 Rocks
    {
        UpdateResources();
        if (sticks >= 3 && rocks >= 2)
        {
            if (inv.AddToStorage(9) ==1) // ensure successfully added to storage before removing resources
            {
                inv.UseResource(1); // stick
                inv.UseResource(1);
                inv.UseResource(1);

                inv.UseResource(2); //rock
                inv.UseResource(2);
            }

            
        }
        UpdateResources();
        CheckRecipes();
    }

    public void Craft_Pickaxe() 
    {
        UpdateResources();
        if (sticks >= 1 && rocks >= 1)
        {
            if (inv.AddToStorage(5) == 1) // ensure successfully added to storage before removing resources
            {
                inv.UseResource(1); // stick

                inv.UseResource(2); //rock
                
            }


        }
        UpdateResources();
        CheckRecipes();
    }

    public void Craft_Axe() 
    {
        UpdateResources();
        if (sticks >= 1 && rocks >= 1 && vines >= 1)
        {
            if (inv.AddToStorage(6) == 1) // ensure successfully added to storage before removing resources
            {
                inv.UseResource(1); // stick
                inv.UseResource(2); //rock
                inv.UseResource(3); //vine

            }


        }
        UpdateResources();
        CheckRecipes();
    }

    public void Craft_Spear() 
    {
        UpdateResources();
        if (sticks >= 1 && iron >= 1)
        {
            if (inv.AddToStorage(8) == 1) // ensure successfully added to storage before removing resources
            {
                inv.UseResource(1); //stick
                inv.UseResource(7); //rock
                

            }


        }
        UpdateResources();
        CheckRecipes();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Craft"))
        {
            if (isShowing)
            {
                CraftingUI.SetActive(false);
                Cursor.visible = false;
                isShowing = false;
                Screen.lockCursor = true;
            }
            else
            {
                UpdateResources();
                CheckRecipes();
                CraftingUI.SetActive(true);
                Cursor.visible = true;
                isShowing = true;
                Screen.lockCursor = false;
            }
        }
    }
}
