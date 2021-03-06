﻿

using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour { 

	[Serializable]
	public class Count
	{
		public int minimum;
		public int maximum;

		public Count (int min, int max)
		{
			minimum = min;
			maximum = max;
		}
	}

	public int columns = 14; //setting up variables and arrays for game objects to be placed
	public int rows = 8;
	public Count wallCount = new Count (8,13); 
	public Count foodCount = new Count (1,5);
	public Count knifeCount = new Count(0,1);//new
	public GameObject exit;
	public GameObject[] floorTiles;
	public GameObject[] wallTiles;
	public GameObject[] foodTiles;
	public GameObject[] enemyTiles;
	public GameObject[] outerWallTiles;
	public GameObject[] knifeTiles; //new

	private Transform boardHolder;
	private List <Vector3> gridPositions = new List<Vector3>();

	void InitialiseList()
	{
		gridPositions.Clear();

		for (int x = 1; x < columns - 1; x++)
		{
			for (int y = 1; y < rows - 1; y++)
			{
				gridPositions.Add(new Vector3(x,y,0f));
			}
		}
	}
	 
	void BoardSetup() //sets up board, makes sure boarder is lined with impassable objects 
	{
		boardHolder = new GameObject ("Board").transform;

		for (int x = -1; x < columns + 1; x++)
		{
			for (int y = -1; y < rows + 1; y++)
			{
				GameObject toInstatiate;

				if (x == -1 || x == columns || y == -1 || y == rows)
				{
					toInstatiate = outerWallTiles[Random.Range (0, outerWallTiles.Length)];
				}
				else 
				{
					toInstatiate = floorTiles[Random.Range (0, floorTiles.Length)];
				}
				GameObject instance = Instantiate(toInstatiate, new Vector3 (x,y,0f), Quaternion.identity) as GameObject;

				instance.transform.SetParent(boardHolder);
			}
		}
	}

	Vector3 RandomPosition() //randomizes placement of game objects
	{
		int randomIndex = Random.Range(0, gridPositions.Count);
		Vector3 randomPosition = gridPositions[randomIndex];
		gridPositions.RemoveAt(randomIndex);
		return randomPosition;
	}


	void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum) //method for laying out tiles
	{
		int objectCount = Random.Range (minimum, maximum);

		for (int i = 0; i < objectCount; i++)
		{
			Vector3 randomPosition = RandomPosition();
		
		Debug.Log("wall gen");
		 GameObject tileChoice = tileArray[Random.Range (0, tileArray.Length)];
			Instantiate (tileChoice, randomPosition, Quaternion.identity);
		}
	}

	public void SetupScene (int level) //setting up the scene
	{
		BoardSetup();
		InitialiseList();
		LayoutObjectAtRandom(wallTiles, wallCount.minimum, wallCount.maximum);
		LayoutObjectAtRandom(foodTiles, foodCount.minimum, foodCount.maximum);
		LayoutObjectAtRandom(knifeTiles, knifeCount.minimum, knifeCount.maximum); //new
		int enemyCount = (int)Mathf.Log(level, 2f);
		LayoutObjectAtRandom(enemyTiles, enemyCount, enemyCount);
		Instantiate(exit, new Vector3(columns - 1, rows - 1, 0f), Quaternion.identity);
	}
}
