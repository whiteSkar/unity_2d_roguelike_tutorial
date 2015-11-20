using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour
{
    [Serializable]
    public class Count
    {
        public int minimum;
        public int maximum;
        
        public Count(int min, int max)
        {
            minimum = min;
            maximum = max;
        }
    }
    
    
    public int rows = 8;
    public int columns = 8;
    public Count wallCount = new Count(5, 9);
    public Count foodCount = new Count(1, 5);
    public GameObject exit;
    public GameObject[] floorTiles;
    public GameObject[] wallTiles;
    public GameObject[] foodTiles;
    public GameObject[] enemyTiles;
    public GameObject[] outerWallTiles;
    
    
    private Transform boardHolder;
    private List<Vector3> gridPositions = new List<Vector3>();
    
    
    public void SetupScene(int level)
    {
        BoardSetup();
        InitializeList();
        LayoutObjectAtRandom(wallTiles, wallCount.minimum, wallCount.maximum);
        LayoutObjectAtRandom(foodTiles, foodCount.minimum, foodCount.maximum);
        
        int enemyCount = (int)Mathf.Log(level, 2f);
        LayoutObjectAtRandom(enemyTiles, enemyCount, enemyCount);
        
        Instantiate(exit, new Vector3(columns - 1, rows - 1, 0f), Quaternion.identity);        
    }
    
    private void InitializeList()
    {
        gridPositions.Clear();
        for (int row = 1; row < rows - 1; row++)
        {
            for (int col = 1; col < columns - 1; col++)
            {
                gridPositions.Add(new Vector3(col, row, 0f));
            }
        }
    }
    
    private void BoardSetup()
    {
        boardHolder = new GameObject("Board").transform;
        for (int row = -1; row < rows + 1; row++)
        {
            for (int col = -1; col < columns + 1; col++)
            {
                GameObject toInstantiate;
                if (row == -1 || row == rows || col == -1 || col == columns)
                    toInstantiate = outerWallTiles[Random.Range(0, outerWallTiles.Length)];
                else
                    toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];
                
                GameObject instance = Instantiate(toInstantiate, new Vector3(col, row, 0f), Quaternion.identity) as GameObject;
                instance.transform.SetParent(boardHolder);
            }
        }
    }
    
    private Vector3 RandomPosition()
    {
        int randomIndex = Random.Range(0, gridPositions.Count);
        Vector3 randomPosition = gridPositions[randomIndex];
        gridPositions.RemoveAt(randomIndex);
        
        return randomPosition;
    }
    
    private void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum)
    {
        int objectCount = Random.Range(minimum, maximum + 1);
        for (int i = 0; i < objectCount; i++)
        {
            Vector3 randomPosition = RandomPosition();
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];
            Instantiate(tileChoice, randomPosition, Quaternion.identity);
        }
    }
}
