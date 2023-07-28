using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WaveFunctionCollapse : MonoBehaviour
{
    private WFCTile[,,] grid;

    public GameObject cube;

    private int sizeX = 40, sizeY = 2, sizeZ = 40;

    //private int curX, curY;
    public int sizeNeighborhood = 1;

    public List<RoadTile> availableTiles;
    private int minP = 12;
    private WFCTile curTile;

    public GameObject tileEx;
    private List<WFCTile> tileToCheck;
    //private List<WFCTile> tileToAdd;

    //Sauce
    //https://www.uproomgames.com/dev-log/wave-function-collapse
    //https://www.fxhash.xyz/article/lessons-learned-from-implementing-%22wave-function-collapse%22

    //                 0      1        2        3        4         5        6        7     
    private RoadTile empty, roadNS, roadEW, roadCross, roadUpN, roadUpS, roadUpE, roadUpW;
    public GameObject roadNSGO, roadEWGO, roadUpNGO, roadCrossGO, roadUpSGO, roadUpEGO, roadUpWGO;

    // Start is called before the first frame update
    void Start()
    {
        availableTiles = new List<RoadTile>(); //add road tiles 

        empty = new RoadTile(0, null, sizeNeighborhood);
        roadNS = new RoadTile(1, roadNSGO, sizeNeighborhood);
        roadEW = new RoadTile(2, roadEWGO, sizeNeighborhood);
        roadCross = new RoadTile(3, roadCrossGO, sizeNeighborhood);
        roadUpN = new RoadTile(4, roadUpNGO, sizeNeighborhood);
        roadUpS = new RoadTile(5, roadUpSGO, sizeNeighborhood);
        roadUpE = new RoadTile(6, roadUpEGO, sizeNeighborhood);
        roadUpW = new RoadTile(7, roadUpWGO, sizeNeighborhood);

        availableTiles.Add(empty);
        availableTiles.Add(roadNS);
        availableTiles.Add(roadEW);
        availableTiles.Add(roadCross);
        availableTiles.Add(roadUpN);
        availableTiles.Add(roadUpE);
        availableTiles.Add(roadUpS);
        availableTiles.Add(roadUpW);

        fillConstraints();

        //tileToAdd = new List<WFCTile>();
        //tileToCheck = new List<WFCTile>();
        grid = new WFCTile[sizeX, sizeY, sizeZ];
        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                for (int z = 0; z < sizeZ; z++)
                {
                    grid[x, y, z] = new WFCTile(x, y, z);
                }
            }
        }

        curTile = grid[sizeX / 2, sizeY / 2, sizeZ / 2];
        //tileToCheck.Add(curTile);
    }

    // Update is called once per frame
    void Update()
    {
        //Créer des modèles de contraintes pour chaque tuile ???

        

        while (curTile != null)
        {
            if (curTile.tilePossible.Count() > 1)
            {
                int rand = Random.Range(0, curTile.tilePossible.Count());
                curTile.cell = curTile.tilePossible[rand];
                curTile.tilePossible.Clear();
            } else if(curTile.tilePossible.Count() == 1)
            {
                curTile.cell = curTile.tilePossible[0];
                curTile.tilePossible.Clear();
            }
            else
            {
                curTile.cell = 0;
            }
            //CollapseTileRandom(nextTile);

            //Instanciation de la tile
            if (curTile.cell > 0)
            {
                Instantiate(availableTiles[curTile.cell].roadGO,
                    new Vector3(curTile.posX, curTile.posY, curTile.posZ),
                    availableTiles[curTile.cell].roadGO.transform.rotation);
            }

            propagateTile(curTile);
            
            curTile = null;
            int nbPoss = 10;

            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    for (int z = 0; z < sizeZ; z++)
                    {
                        if (grid[x, y, z].tilePossible.Count() > 0 && grid[x, y, z].tilePossible.Count() <= nbPoss && grid[x, y, z].cell == -1)
                        {
                            nbPoss = grid[x, y, z].tilePossible.Count();
                            curTile = grid[x, y, z];
                        }
                    }
                }
            }
        }

        /*
    while (tileToCheck.Count() > 0)
    {
        curTile = tileToCheck[0];
        tileToCheck.Remove(curTile);
        //curTile = t;
        if(curTile.cell == -1)
            CollapseTileRandom(curTile);

        //Instanciation de la tile
        if (curTile.cell > 0)
            Instantiate(availableTiles[curTile.cell].roadGO,
                new Vector3(curTile.posX, curTile.posY, curTile.posZ),
                availableTiles[curTile.cell].roadGO.transform.rotation);
        
        //On cherche la prochaine tile avec la plus petite entropie
        minP = 12;
        curTile = null;
        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                for (int z = 0; z < sizeZ; z++)
                {
                    if (grid[x, y, z].cell == -1 && grid[x, y, z].tilePossible.Count < minP)
                    {
                        curTile = grid[x, y, z];
                        minP = curTile.tilePossible.Count;
                    }
                }
            }
        }
        //Debug.Log(minP);
    }
    //Debug.Log(tileToAdd.Count);
*/
        //tileToCheck.Clear();
        //tileToCheck.AddRange(tileToAdd.ToList());
        //tileToAdd.Clear();
    }

    //           0      1     2        3        4        5       6      7     
    //RoadTile empty,roadNS,roadEW,roadCross,roadUpN,roadUpE,roadUpS,roadUpW
    void fillConstraints()
    {
        int size = 13;
        int[,,] exGrid = new int [size, size, size];
        int x0, y0, z0;

        for (int i = 0; i < tileEx.transform.childCount; i++)
        {
            var ex = tileEx.transform.GetChild(i);

            x0 = (int) ex.transform.localPosition.x;
            y0 = (int) ex.transform.localPosition.y;
            z0 = (int) ex.transform.localPosition.z;

            if (x0 >= 0 && x0 < size && y0 >= 0 && y0 < size && z0 >= 0 && z0 < size)
            {
                int cell = 0;
                if (ex.gameObject.name.Contains("NS"))
                    cell = 1;
                else if (ex.gameObject.name.Contains("EW"))
                    cell = 2;
                else if (ex.gameObject.name.Contains("Cross"))
                    cell = 3;
                else if (ex.gameObject.name.Contains("UpN"))
                    cell = 4;
                else if (ex.gameObject.name.Contains("UpE"))
                    cell = 5;
                else if (ex.gameObject.name.Contains("UpS"))
                    cell = 6;
                else if (ex.gameObject.name.Contains("UpW"))
                    cell = 7;
                //Debug.Log(cell);
                exGrid[x0, y0, z0] = cell;
            }
        }
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                for (int z = 0; z < size; z++)
                {
                    RoadTile curRoadTile = availableTiles[exGrid[x, y, z]];

                    for (int a = -sizeNeighborhood; a <= sizeNeighborhood; a++)
                    {
                        for (int b = -sizeNeighborhood; b <= sizeNeighborhood; b++)
                        {
                            for (int c = -sizeNeighborhood; c <= sizeNeighborhood; c++)
                            {
                                if ((a != 0 || b != 0 || c != 0) && x + a >= 0 && x + a < size && y + b >= 0 &&
                                    y + b < size && z + c >= 0 && z + c < size
                                    && curRoadTile.contraintes.Length > 0)
                                {
                                    int cons = exGrid[x + a, y + b, z + c];
                                    if (!curRoadTile.contraintes[a + sizeNeighborhood, b + sizeNeighborhood,
                                            c + sizeNeighborhood].Contains(cons))
                                        curRoadTile.contraintes[a + sizeNeighborhood, b + sizeNeighborhood,
                                            c + sizeNeighborhood].Add(cons);
                                }
                            }
                        }
                    }
                }
            }
        }
        /*
        RoadTile EmptyTile = availableTiles[0];
        for (int a = -sizeNeighborhood; a <= sizeNeighborhood; a++)
        {
            for (int b = -sizeNeighborhood; b <= sizeNeighborhood; b++)
            {
                for (int c = -sizeNeighborhood; c <= sizeNeighborhood; c++)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        if (a != 0 || b != 0 || c != 0)
                        {
                            if (!EmptyTile.contraintes[a + sizeNeighborhood, b + sizeNeighborhood,
                                    c + sizeNeighborhood].Contains(i))
                                EmptyTile.contraintes[a + sizeNeighborhood, b + sizeNeighborhood,
                                    c + sizeNeighborhood].Add(i);
                        }
                    }
                }
            }
        }*/
    }

    void CollapseTile(WFCTile tile)
    {
        tile.cell = tile.tilePossible.First();
        tile.tilePossible.Clear();
        propagateTile(tile);
    }
    
    void CollapseTileRandom(WFCTile tile)
    {
        int[] prob = new int[8];
        for (int a = -sizeNeighborhood; a <= sizeNeighborhood; a++)
        {
            for (int b = -sizeNeighborhood; b <= sizeNeighborhood; b++)
            {
                for (int c = -sizeNeighborhood; c <= sizeNeighborhood; c++)
                {
                    if ((a != 0 || b != 0 || c != 0) && tile.posX + a >= 0 && tile.posX + a < sizeX &&
                        tile.posY + b >= 0 &&
                        tile.posY + b < sizeY && tile.posZ + c >= 0 && tile.posZ + c < sizeZ)
                    {
                        var lookCell = grid[tile.posX + a, tile.posY + b, tile.posZ + c];
                        if (lookCell.cell == -1)
                        {
                            foreach (var tp in lookCell.tilePossible)
                            {
                                var at = availableTiles[tp].contraintes[-a + sizeNeighborhood,
                                    -b + sizeNeighborhood, -c + sizeNeighborhood];
                                foreach (var t in at)
                                {
                                    if(tile.tilePossible.Contains(t))
                                        prob[t]++;
                                }
                            }
                        }
                        else
                        {
                            var at = availableTiles[lookCell.cell].contraintes[-a + sizeNeighborhood,
                                -b + sizeNeighborhood, -c + sizeNeighborhood];
                            foreach (var t in at)
                            {
                                if(tile.tilePossible.Contains(t))
                                    prob[t]++;
                            }
                        }
                    }
                }
            }
        }

        int rnd = Random.Range(0, prob.Sum());
        int tot = 0;
        int index = 0;
        for (int i = 0; i < 8; i++)
        {
            tot += prob[i];
            if (tot > rnd)
            {
                index = i;
                break;
            }
        }
        tile.cell = index;
        tile.tilePossible.Clear();
        
        //propagateTile(tile);
        
        //Debug.Log(tile.cell);
    }

    void propagateTile(WFCTile tile)
    {
        for (int a = -sizeNeighborhood; a <= sizeNeighborhood; a++)
        {
            for (int b = -sizeNeighborhood; b <= sizeNeighborhood; b++)
            {
                for (int c = -sizeNeighborhood; c <= sizeNeighborhood; c++)
                {
                    if ((a != 0 || b != 0 || c != 0) && tile.posX + a >= 0 && tile.posX + a < sizeX &&
                        tile.posY + b >= 0 &&
                        tile.posY + b < sizeY && tile.posZ + c >= 0 && tile.posZ + c < sizeZ
                        && tile.cell != -1)
                    {
                        var lookCell = grid[tile.posX + a, tile.posY + b, tile.posZ + c];
                        lookCell.Update(availableTiles[tile.cell].contraintes[a + sizeNeighborhood,
                            b + sizeNeighborhood, c + sizeNeighborhood]);
                        if (lookCell.tilePossible.Count == 1)
                        {
                            //lookCell.cell = lookCell.tilePossible[0];
                            Debug.Log("lookCell.cell: " + lookCell.cell);

                            propagateTile(lookCell);
                            //Instantiate(availableTiles[lookCell.cell].roadGO,
                            //    new Vector3(lookCell.posX, lookCell.posY, lookCell.posZ),
                            //    availableTiles[lookCell.cell].roadGO.transform.rotation);
                            //lookCell.tilePossible.Clear();

                        }
                    }
                }
            }
        }
    }
}
