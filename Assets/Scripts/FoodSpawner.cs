using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    public float spawnRate = 1;
    public float floorScale = .25f;
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
        float x = Random.Range(0, 160) * floorScale;
        float z = Random.Range(0, 160) * floorScale;

        RaycastHit hit;

       
       
            
           
            
            
            Vector3 rayStart = new Vector3(x,50,z);
            if (Physics.Raycast(rayStart,Vector3.down, out hit))
            {
                // Draw a line representing the raycast in the scene view for debugging purposes
                Debug.DrawRay(rayStart, Vector3.down, Color.red);
                GameObject food = Instantiate(myPrefab,new Vector3(hit.point.x,hit.point.y + 0.25f,hit.point.z), Quaternion.identity);
                foodTab[index] = food.transform;

            }
            
        






        
    }
}