using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadTile
{
    private int tileId;
    public GameObject roadGO;
    //public List<int> contrainteN;
    //public List<int> contrainteE;
    //public List<int> contrainteS;
    //public List<int> contrainteW;
    public List<int>[,,] contraintes;

    public RoadTile(int id, GameObject prefab, int sNei)
    {
        //int[] allVal = new[] {0, 1, 2, 3, 4, 5, 6, 7};
        tileId = id;
        roadGO = prefab;
        contraintes = new List<int>[2*sNei+1,2*sNei+1,2*sNei+1];
        for (int x = 0; x < 2*sNei+1; x++)
        {
            for (int y = 0; y < 2*sNei+1; y++)
            {
                for (int z = 0; z < 2*sNei+1; z++)
                {
                    contraintes[x, y, z] = new List<int>();
                }
            }
        }
    }
}
