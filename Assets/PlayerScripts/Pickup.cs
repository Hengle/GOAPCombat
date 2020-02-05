using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    [SerializeField]
    float checkRadius =3;
    [SerializeField]
    PInventory inv;
    private Animator _animator;
    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    GameObject checkForItems()
    {
        Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, checkRadius);
        for (int i=0; i< hitColliders.Length; i++)
        {
            if (hitColliders[i].gameObject.tag == "Item")
            {
                return hitColliders[i].gameObject;
            }
        }
        return null;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Pickup"))
        {
            GameObject item = checkForItems();
            if (item)
            {
                if(inv.storage.Count < 10)
                {
                    _animator.SetTrigger("Gather");
                    inv.AddToStorage(item.GetComponent<item>().id);
                    Destroy(item);
                }
            }
        }
    }
}
