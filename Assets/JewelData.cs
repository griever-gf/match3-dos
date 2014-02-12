using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class JewelData : MonoBehaviour {

	public tk2dTileMap tilemap;

	public GameObject prefabJewel;
	public GameObject prefabBorder;

	private GameObject[,] spritesJewels;
	private GameObject Border;

	int SelectedSpriteIndexX, SelectedSpriteIndexY, PreviousSpriteIndexX, PreviousSpriteIndexY;
	bool IsFirstJewelInPair;
	bool IsBusy;

	public AudioClip clipMove;
	public AudioClip clipMoveBack;
	public AudioClip clipMatch;

	const float MOVEMENT_DURATION = 0.2f;

	void Start () {
		spritesJewels = new GameObject[tilemap.width, tilemap.height];
		IsFirstJewelInPair = true;
		IsBusy = false;
		CreateJewels();
	}
	
	void CreateJewels() //initializing jewel field
	{
		Vector3 tilesize = tilemap.data.tileSize;
		for (int i = 0; i < spritesJewels.GetLength(0); i++)
			for (int j = 0; j < spritesJewels.GetLength(1); j++)
			{
				spritesJewels[i,j] = Instantiate(prefabJewel, tilemap.GetTilePosition(i,j)+ tilesize*0.5f, transform.rotation) as GameObject;
			}
		int SpritesCount = spritesJewels[0,0].GetComponent<tk2dSprite>().Collection.Count;
		System.Random random = new System.Random();
		do {
			for (int i = 0; i < spritesJewels.GetLength(0); i++)
				for (int j = 0; j < spritesJewels.GetLength(1); j++)
				{
					do {
						spritesJewels[i,j].GetComponent<tk2dSprite>().SetSprite(random.Next(0, SpritesCount));
					} while (IsMatch3(i, j));
				}
		} while (!IsAnywherePotentialMatch3());
	}

	public void SelectTile(GameObject go)
	{
		if (go.name.Contains("prefabJewel")&&(!IsBusy))
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
	
	bool IsAnywherePotentialMatch3() //check the entire field - is anywhere match is possible?
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
	
	IEnumerator TryToMatch(int x1, int y1, int x2, int y2) //execute the swap, then process if matches and swap back if not
	{
		IsBusy = true;
		audio.PlayOneShot(clipMove);
		yield return StartCoroutine(SwapJewels(x1, y1, x2, y2)); //wait unitil swap

		if (CheckForMatchesAndDestroyIfMatch())
		{
			yield return(StartCoroutine(ShiftAndGenerateNewJewels()));
		}
		else
		{
			audio.PlayOneShot(clipMoveBack);
			yield return StartCoroutine(SwapJewels(x1, y1, x2, y2)); //swap back if no matches
		}
		IsBusy = false;
	}

	IEnumerator SwapJewels(int x1, int y1, int x2, int y2)
	{
		List<int[]> Jewels = new List<int[]>();
		Jewels.Add(new int[]{x1, y1});
		Jewels.Add(new int[]{x2, y2});
		List<Vector3> Goals = new List<Vector3>();
		Goals.Add(spritesJewels[x2,y2].transform.position);
		Goals.Add(spritesJewels[x1,y1].transform.position);

		yield return StartCoroutine(MoveJewels(Jewels, Goals));
		
		GameObject tmp = spritesJewels[x1,y1];
		spritesJewels[x1,y1] = spritesJewels[x2,y2];
		spritesJewels[x2,y2] = tmp;
	}

	IEnumerator MoveJewels(List<int[]> JewelsForMove, List<Vector3> Destinations)
	{
		List<Vector3> Positions = new List<Vector3>();
		foreach (int[] coords in JewelsForMove)
		{
			Positions.Add(spritesJewels[coords[0],coords[1]].transform.position);
		}

		float movementFraction = 0;
		float startTime = Time.time;
		float currentTime = Time.time;
		
		while ((currentTime - startTime) < MOVEMENT_DURATION)
		{
			for (int i = 0; i < JewelsForMove.Count; i++)
			{
				spritesJewels[JewelsForMove[i][0],JewelsForMove[i][1]].transform.position =
					Vector3.Lerp(Positions[i], Destinations[i], movementFraction);
			}
			currentTime = Time.time;
			movementFraction = (currentTime - startTime) / MOVEMENT_DURATION;
			yield return null;
		}
		
		for (int i = 0; i < JewelsForMove.Count; i++)
			spritesJewels[JewelsForMove[i][0],JewelsForMove[i][1]].transform.position = Destinations[i];

	}

	bool CheckForMatchesAndDestroyIfMatch()
	{
		List<int[]> MatchedJewelCoords = new List<int[]>();
		for (int i = 0; i < spritesJewels.GetLength(0); i++)
			for (int j = 0; j < spritesJewels.GetLength(1); j++)
				if (IsMatch3(i, j))
					MatchedJewelCoords.AddRange(GetMatchedJewels(i, j));
		if (MatchedJewelCoords.Count > 0)
		{
			MatchedJewelCoords = MatchedJewelCoords.Distinct().ToList(); //remove duplicates
			audio.PlayOneShot(clipMatch);
			foreach (int[] coords in MatchedJewelCoords)
			{
				Destroy(spritesJewels[coords[0],coords[1]]);
				spritesJewels[coords[0],coords[1]] = null;
			}
			return true;
		}
		else
			return false;
	}
	
	IEnumerator ShiftAndGenerateNewJewels() //shifting jewels after destroy by match, generating new ones, checking for match again
	{
		//shift jewels
		List<int> BrokenColumns = new List<int>();
		List<int[]> JewelsForShift = new List<int[]>();
		List<int[]> JewelsForGenerate = new List<int[]>();
		for (int i = 0; i < spritesJewels.GetLength(0); i++)
			for (int j = 0; j < spritesJewels.GetLength(1); j++)
				if (!BrokenColumns.Contains(i))
					if (spritesJewels[i, j] == null)
				{
					BrokenColumns.Add(i);
					int shift = 0;
					int k;
					bool IsGapChecked = false;
					for (k = j; k < spritesJewels.GetLength(1); k++)
					{
						if (spritesJewels[i, k] == null)
						{
							if (!IsGapChecked)
							{
								int shift_gap = 1;
								while (k+shift_gap < spritesJewels.GetLength(1))
								{
									if (spritesJewels[i, k+shift_gap] == null)
										shift_gap++;
									else
										break;
								}
								shift += shift_gap;
								IsGapChecked = true;
							}
						}
						else
							IsGapChecked = false;
						
						JewelsForShift.Add(new int[2]{i, k});
					}
					int m = j;
					for (k = j; k < spritesJewels.GetLength(1)-shift; k++)
					{
						do{
							m++;
						}while (spritesJewels[i, m]==null);
						spritesJewels[i, k] = spritesJewels[i, m];
					}
					
					for (k = spritesJewels.GetLength(1)-shift; k < spritesJewels.GetLength(1); k++)
					{
						spritesJewels[i, k] = null;
						JewelsForGenerate.Add(new int[3]{i, k, shift});
					}
				}
		
		//new jewels generation
		Vector3 tilesize = tilemap.data.tileSize;
		foreach (int[] coords in JewelsForGenerate)
		{
			spritesJewels[coords[0],coords[1]] = Instantiate(prefabJewel,
			                                                 tilemap.GetTilePosition(coords[0],coords[1])+ tilesize*0.5f+ new Vector3(0,tilesize.y*coords[2]),
			                                                 transform.rotation) as GameObject;
		}
		System.Random random = new System.Random();
		int SpritesCount = spritesJewels[0,0].GetComponent<tk2dSprite>().Collection.Count;
		do {
			foreach (int[] coords in JewelsForGenerate)
			{
				do {
					spritesJewels[coords[0],coords[1]].GetComponent<tk2dSprite>().SetSprite(random.Next(0, SpritesCount));
				} while (IsMatch3(coords[0], coords[1]));
			}
		} while (!IsAnywherePotentialMatch3());
		
		//sprites movement
		List<Vector3> Goals = new List<Vector3>();
		foreach (int[] coords in JewelsForShift)
			Goals.Add(tilemap.GetTilePosition(coords[0],coords[1]) + tilesize*0.5f);
		yield return StartCoroutine(MoveJewels(JewelsForShift, Goals));

		//recursive check after shift
		if (CheckForMatchesAndDestroyIfMatch())
			yield return(StartCoroutine(ShiftAndGenerateNewJewels()));
	}
	
	List<int[]> GetMatchedJewels(int x, int y) //Get a list of jewels around a point what makes a match (3, at least)
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
}
