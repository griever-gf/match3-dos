using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class JewelData : MonoBehaviour {

	public tk2dTileMap tilemap;

	public GameObject prefabJewel;
	public GameObject prefabBorder;

	private GameObject[,] spritesJewels;
	private GameObject Border;

	int SelectedSpriteIndexX, SelectedSpriteIndexY, PreviousSpriteIndexX, PreviousSpriteIndexY;
	bool IsFirstJewelInPair;

	const float MOVEMENT_DURATION = 1.0f;
	const float MOVEMENT_SPEED = 2.0f;

	// Use this for initialization
	void Start () {
		spritesJewels = new GameObject[tilemap.width, tilemap.height];
		IsFirstJewelInPair = true;
		CreateJewels();
	}

	void CreateJewels()
	{
		Vector3 tilesize = tilemap.data.tileSize;
		for (int i = 0; i < spritesJewels.GetLength(0); i++)
			for (int j = 0; j < spritesJewels.GetLength(1); j++)
			{
				spritesJewels[i,j] = Instantiate(prefabJewel, tilemap.GetTilePosition(i,j)+ tilesize*0.5f, transform.rotation) as GameObject;//as tk2dSprite;
			}
		int SpritesCount = spritesJewels[0,0].GetComponent<tk2dSprite>().Collection.Count;
		System.Random random = new System.Random();
		do 
		{
			for (int i = 0; i < spritesJewels.GetLength(0); i++)
				for (int j = 0; j < spritesJewels.GetLength(1); j++)
				{
					do
					{
						spritesJewels[i,j].GetComponent<tk2dSprite>().SetSprite(random.Next(0, SpritesCount));
					}
					while (IsMatch3(i, j));
				}
		}
		while (!IsAnywherePotentialMatch3());
	}

	public void SelectTile(GameObject go)
	{
		if (go.name.Contains("prefabJewel"))
		{
			SelectedSpriteIndexX = SelectedSpriteIndexY = -10;
			for (int i = 0; i < spritesJewels.GetLength(0); i++)
				for (int j = 0; j < spritesJewels.GetLength(1); j++)
					if (go.Equals(spritesJewels[i, j]))
					{
						SelectedSpriteIndexX = i;
						SelectedSpriteIndexY = j;
						i = spritesJewels.GetLength(0);
						break;
					}
			//Debug.Log ("Jewel coords: " + SelectedSpriteIndexX + ", " + SelectedSpriteIndexY);

			if (Border != null) Destroy(Border);
			if (IsFirstJewelInPair)
			{
				Border = Instantiate(prefabBorder, go.transform.position, go.transform.rotation) as GameObject;
			}
			else
			{
				if (IsNeighbors(SelectedSpriteIndexX, SelectedSpriteIndexY))
					StartCoroutine(TryToMatch(SelectedSpriteIndexX, SelectedSpriteIndexY, PreviousSpriteIndexX,PreviousSpriteIndexY));
			}
			IsFirstJewelInPair = !IsFirstJewelInPair;
			PreviousSpriteIndexX = SelectedSpriteIndexX;
			PreviousSpriteIndexY = SelectedSpriteIndexY;
		}
	}

	bool IsNeighbors(int CurrentIndexX, int CurrentIndexY)
	{
		return ((((CurrentIndexX - PreviousSpriteIndexX == 1)||(CurrentIndexX - PreviousSpriteIndexX == -1))&&(CurrentIndexY - PreviousSpriteIndexY == 0))||
				(((CurrentIndexY - PreviousSpriteIndexY == 1)||(CurrentIndexY - PreviousSpriteIndexY == -1))&&(CurrentIndexX - PreviousSpriteIndexX == 0)));
	}

	bool IsMatch3(int X, int Y)
	{
		int SpriteID = spritesJewels[X,Y].GetComponent<tk2dSprite>().spriteId;
		if (X>=2)
			if ((spritesJewels[X-1,Y].GetComponent<tk2dSprite>().spriteId == SpriteID)&&
        	    (spritesJewels[X-2,Y].GetComponent<tk2dSprite>().spriteId == SpriteID))
				return true;
		if (X<spritesJewels.GetLength(0)-2)
			if((spritesJewels[X+1,Y].GetComponent<tk2dSprite>().spriteId == SpriteID)&&
               (spritesJewels[X+2,Y].GetComponent<tk2dSprite>().spriteId == SpriteID))
				return true;
		if (Y>=2)
			if ((spritesJewels[X,Y-1].GetComponent<tk2dSprite>().spriteId == SpriteID)&&
        	 	(spritesJewels[X,Y-2].GetComponent<tk2dSprite>().spriteId == SpriteID))
				return true;
		if (Y<spritesJewels.GetLength(1)-2)
			if ((spritesJewels[X,Y+1].GetComponent<tk2dSprite>().spriteId == SpriteID)&&
      	 		(spritesJewels[X,Y+2].GetComponent<tk2dSprite>().spriteId == SpriteID))
				return true;
		if ((X>0)&&(X<spritesJewels.GetLength(0)-1))
		    if ((spritesJewels[X-1,Y].GetComponent<tk2dSprite>().spriteId == SpriteID)&&
		    	(spritesJewels[X+1,Y].GetComponent<tk2dSprite>().spriteId == SpriteID))
		    	return true;
		if ((Y>0)&&(Y<spritesJewels.GetLength(1)-1))
			if ((spritesJewels[X,Y-1].GetComponent<tk2dSprite>().spriteId == SpriteID)&&
			    (spritesJewels[X,Y+1].GetComponent<tk2dSprite>().spriteId == SpriteID))
				return true;
		return false;
	}

	bool IsAnywherePotentialMatch3()
	{
		int CurrentSpriteId;
		for (int i = 0; i < spritesJewels.GetLength(0); i++)
			for (int j = 0; j < spritesJewels.GetLength(1); j++)
			{
				CurrentSpriteId = spritesJewels[i,j].GetComponent<tk2dSprite>().spriteId;
				if ((i > 0)&&(i < spritesJewels.GetLength(0) - 1))
				{
					if ( CurrentSpriteId == spritesJewels[i-1,j].GetComponent<tk2dSprite>().spriteId)
					{
						if (j > 0)
							if (CurrentSpriteId == spritesJewels[i+1,j-1].GetComponent<tk2dSprite>().spriteId)
								return true;
						if (j < spritesJewels.GetLength(1) - 1)
							if (CurrentSpriteId == spritesJewels[i+1,j+1].GetComponent<tk2dSprite>().spriteId)
								return true;
					}
					if (CurrentSpriteId == spritesJewels[i+1,j].GetComponent<tk2dSprite>().spriteId)
					{
						if (j > 0)
							if (CurrentSpriteId == spritesJewels[i-1,j-1].GetComponent<tk2dSprite>().spriteId)
								return true;
						if (j < spritesJewels.GetLength(1) - 1)
							if (CurrentSpriteId == spritesJewels[i-1,j+1].GetComponent<tk2dSprite>().spriteId)
								return true;
					}
				}
				if ((j > 0)&&(j < spritesJewels.GetLength(0) - 1))
				{
					if (CurrentSpriteId == spritesJewels[i,j-1].GetComponent<tk2dSprite>().spriteId)
					{
						if (i > 0)
							if (CurrentSpriteId == spritesJewels[i-1,j+1].GetComponent<tk2dSprite>().spriteId)
								return true;
						if (i < spritesJewels.GetLength(0) - 1)
							if (CurrentSpriteId == spritesJewels[i+1,j+1].GetComponent<tk2dSprite>().spriteId)
								return true;
					}
					if (CurrentSpriteId == spritesJewels[i,j+1].GetComponent<tk2dSprite>().spriteId)
					{
						if (i > 0)
							if (CurrentSpriteId == spritesJewels[i-1,j-1].GetComponent<tk2dSprite>().spriteId)
								return true;
						if (i < spritesJewels.GetLength(0) - 1)
							if (CurrentSpriteId == spritesJewels[i+1,j-1].GetComponent<tk2dSprite>().spriteId)
								return true;
					}
				}
			}
		return false;
	}

	List<int[]> GetMatchedJewels(int x, int y)
	{
		int SpriteID = spritesJewels[x,y].GetComponent<tk2dSprite>().spriteId;
		List<int[]> matchedCoords = new List<int[]>();
		List<int[]> temp = new List<int[]>();
		temp.Add(new int[2]{x,y});
		for (int i = x+1; i < spritesJewels.GetLength(0); i++)
			if (spritesJewels[i,y].GetComponent<tk2dSprite>().spriteId==SpriteID)
				temp.Add(new int[2]{i,y});
			else
				break;
		for (int i = x-1; i >= 0; i--)
			if (spritesJewels[i,y].GetComponent<tk2dSprite>().spriteId==SpriteID)
				temp.Add(new int[2]{i,y});
			else
				break;

		if (temp.Count >= 3)
			matchedCoords.AddRange(temp);
		temp.Clear();

		for (int j = y+1; j < spritesJewels.GetLength(1); j++)
			if (spritesJewels[x,j].GetComponent<tk2dSprite>().spriteId==SpriteID)
				temp.Add(new int[2]{x,j});
			else
				break;
		for (int j = y-1; j >= 0; j--)
			if (spritesJewels[x,j].GetComponent<tk2dSprite>().spriteId==SpriteID)
				temp.Add(new int[2]{x,j});
			else
				break;
		if (temp.Count >= 2)
		{
			if (matchedCoords.Count == 0)
				temp.Add(new int[2]{x,y});
			matchedCoords.AddRange(temp);
		}
		return matchedCoords;
	}

	IEnumerator SwapJewels(int x1, int y1, int x2, int y2)
	{
		Vector3 position1 = spritesJewels[x1,y1].transform.position;
		Vector3 position2 = spritesJewels[x2,y2].transform.position;
		float elapsedTime = 0;
		
		while (elapsedTime < MOVEMENT_DURATION)
		{
			spritesJewels[x1,y1].transform.position = Vector3.Lerp(position1, position2, elapsedTime);
			spritesJewels[x2,y2].transform.position = Vector3.Lerp(position2, position1, elapsedTime);
			elapsedTime += Time.deltaTime * MOVEMENT_SPEED;
			yield return null;
		}
		
		spritesJewels[x1,y1].transform.position = position2;
		spritesJewels[x2,y2].transform.position = position1;
		
		GameObject tmp = spritesJewels[x1,y1];
		spritesJewels[x1,y1] = spritesJewels[x2,y2];
		spritesJewels[x2,y2] = tmp;
	}

	IEnumerator ShiftAndGenerateNewJewels()
	{
		List<int> BrokenColumns = new List<int>();
		List<int> BrokenColumnsShifts = new List<int>();
		List<int[]> JewelsForShift = new List<int[]>();
		for (int i = 0; i < spritesJewels.GetLength(0); i++)
			for (int j = 0; j < spritesJewels.GetLength(1); j++)
				if (!BrokenColumns.Contains(i))
					if (spritesJewels[i, j] == null)
					{
						BrokenColumns.Add(i);
						BrokenColumnsShifts.Add(1);
						for (int k = j+1; k < spritesJewels.GetLength(1); k++)
							if (spritesJewels[i, k] != null)
								JewelsForShift.Add(new int[2]{i, k});
							else
								BrokenColumnsShifts[BrokenColumnsShifts.Count-1]++;
					}

		Vector3 tilesize = tilemap.data.tileSize;
		List<Vector3> Positions = new List<Vector3>();
		List<Vector3> Destinations = new List<Vector3>();
		foreach (int[] coords in JewelsForShift)
		{
			Positions.Add(spritesJewels[coords[0],coords[1]].transform.position);
			Destinations.Add(tilemap.GetTilePosition(coords[0],coords[1]-BrokenColumnsShifts[BrokenColumns.IndexOf(coords[0])])+ tilesize*0.5f);
		}
		float elapsedTime = 0;
		while (elapsedTime < MOVEMENT_DURATION)
		{
			for (int i = 0; i < JewelsForShift.Count; i++)
			{
				spritesJewels[JewelsForShift[i][0],JewelsForShift[i][1]].transform.position =
					Vector3.Lerp(Positions[i], Destinations[i], elapsedTime);
			}
			elapsedTime += Time.deltaTime * MOVEMENT_SPEED;
			yield return null;
		}
		for (int i = 0; i < JewelsForShift.Count; i++)
			spritesJewels[JewelsForShift[i][0],JewelsForShift[i][1]].transform.position = Destinations[i];
	}

	IEnumerator TryToMatch(int x1, int y1, int x2, int y2)
	{
		yield return StartCoroutine(SwapJewels(x1, y1, x2, y2));

		List<int[]> MatchedJewelCoords = new List<int[]>();
		if (IsMatch3(x1, y1))
			MatchedJewelCoords.AddRange(GetMatchedJewels(x1, y1));
		if (IsMatch3(x2, y2))
			MatchedJewelCoords.AddRange(GetMatchedJewels(x2, y2));

		if (MatchedJewelCoords.Count > 0)
		{
			foreach (int[] coords in MatchedJewelCoords)
			{
				Destroy(spritesJewels[coords[0],coords[1]]);
				spritesJewels[coords[0],coords[1]] = null;
			}
			yield return StartCoroutine(ShiftAndGenerateNewJewels());
		}
		//else
			//yield return StartCoroutine(SwapJewels(x1, y1, x2, y2));
	}
}
