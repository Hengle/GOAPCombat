using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDrop : MonoBehaviour
{
    [SerializeField]
    GameObject Item;

    [SerializeField]
    float minTime;
    [SerializeField]
    float maxTime;
    public float modifier = 1; // > 1 = increased Drop rate (ie. Storms); <1 = decreased Drop Rate
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Drop(minTime, maxTime));
    }

    IEnumerator Drop(float min, float max)
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(min/modifier, max/modifier));
            Instantiate(Item, transform.position+new Vector3(0,0,0), Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
