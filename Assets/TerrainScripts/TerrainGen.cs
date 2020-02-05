using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGen : MonoBehaviour
{
    
    public int size = 50;
    [SerializeField]
    private float noiseScale = .08f;
    [SerializeField]
    GameObject Prefab;


    // Start is called before the first frame update
    void Start()
    {
        gen();
    }

    private void gen()
    {
        for (int x=0; x<size; x++)
        {
            for (int y=0; y<size; y++)
            {
                for (int z=0; z<size; z++)
                {
                    if (Perlin3d(x*noiseScale,y*noiseScale,z*noiseScale) >= .5)
                    {
                        Instantiate(Prefab, new Vector3(x, y, z), Quaternion.identity);
                    }
                }
            }
        }
    }

    public static float Perlin3d(float x, float y, float z)
    {
        float ab = Mathf.PerlinNoise(x, y);
        float bc = Mathf.PerlinNoise(y, z);
        float ac = Mathf.PerlinNoise(x, z);

        float ba = Mathf.PerlinNoise(y, x);
        float cb = Mathf.PerlinNoise(z, y);
        float ca = Mathf.PerlinNoise(z, x);

        float abc = ab + bc + ac + ba + cb + ca;
        return abc / 6f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
