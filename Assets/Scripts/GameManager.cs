using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Vector3[,] board;
    public int row, col;

    // Start is called before the first frame update
    void Start()
    {
        GenerateBoard(row,col);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    

    GameObject[,] GenerateBoard(int row, int col)
    {
        GameObject[,] board = new GameObject[row,col];

        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                
                GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                obj.transform.position = new Vector3(i,0,j);

                board[i, j] = obj;

                if((i+j)%2 == 0)
                {
                    obj.GetComponent<MeshRenderer>().material.color = Color.black; 
                }

            }

        }

        return board;

    }

    void WFC_Maker()
    {
        
    }
}
