using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    public float spawnRate = 1;
    public int floorScale = 1;
    public GameObject myPrefab;
    public float timeElapsed = 0;
    public Transform[] foodTab;
    bool canSpawnFood = true;
    public bool foodIsSpawned = false;

    public WaveFunctionCollapse WFC;

    void Start()
    {
        WFC = GameObject.Find("WaveManager").GetComponent<WaveFunctionCollapse>();
    }

    // FixedUpdate is called once per physics frame
    void FixedUpdate()
    {
        if(WFC.terrainGenerated)
        {
            if (canSpawnFood)
            {
                foodTab = new Transform[100];
                // Spawn food at random locations at the start of the game
                for (int i = 0; i < 100; i++)
                {
                    SpawnFood(i);
                }

                canSpawnFood = false;
                foodIsSpawned=true;
            }
        }
        
        
    }

    void SpawnFood(int index)
    {
        int x = Random.Range(-100, 101) * floorScale;
        int z = Random.Range(-100, 101) * floorScale;
        GameObject food = Instantiate(myPrefab, new Vector3((float)x, 0.75f, (float)z), Quaternion.identity);
        foodTab[index] = food.transform;
    }
}