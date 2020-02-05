using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mineable : MonoBehaviour
{
    public GameObject Resource;
    public int Drops = 2; // resources to drop when mined
    public float Hits = 5; // hits before mined?
    public float currentHits; // hits left before mined
    public bool DestroyOnMine = false;

    [SerializeField]
    AudioSource source; // sfx audio source
    [SerializeField]
    AudioClip[] PA_sfx; //pickaxe sfx

    // Start is called before the first frame update
    void Start()
    {
        currentHits = Hits;
    }


    public IEnumerator hit(Vector3 point)
    {
        yield return new WaitForSeconds(1.0f); // wait until after the animation hits the surface
        source.clip = PA_sfx[Random.Range(0, PA_sfx.Length - 1)];
        source.Play();
        currentHits--;
        if (currentHits <= 0) //instantiate resource(s)
        {
            for (int i = 0; i < Drops; i++)
            {
                Instantiate(Resource, point + new Vector3(0, 0, 0), Quaternion.identity);
            }
            if (DestroyOnMine)
            {
                Destroy(this.gameObject); // Destroy this gameobject
            }
            else
            {
                currentHits = Hits; //reset hits
            }
        }
    }


    // Update is called once per frame
    void Update()
    {
    
    }
}
