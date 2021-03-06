﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class JewelData : MonoBehaviour {
	
	public VirtualTilemap DummyTilemap;

	public GameObject prefabJewel;
	public GameObject prefabBorder;

	public GameObject[,] spritesJewels;
	private GameObject Border;

	int SelectedSpriteIndexX, SelectedSpriteIndexY, PreviousSpriteIndexX, PreviousSpriteIndexY;
	bool IsLastSelectedJewelBlocked;
	bool IsBusy;

	public AudioClip clipMove;
	public AudioClip clipMoveBack;
	public AudioClip clipMatch;

	const float MOVEMENT_DURATION = 0.2f;

	public bool[] BlockedRows, BlockedColumns, BlockedMovements;
	
	void Start () {
		spritesJewels = new GameObject[DummyTilemap.width, DummyTilemap.height];
		//IsFirstJewelInPair = true;
		IsBusy = false;
		BlockedRows = new bool[spritesJewels.GetLength(0)];
		BlockedColumns = new bool[spritesJewels.GetLength(1)];
		BlockedMovements = new bool[4];
		CreateJewels();
	}
	
	void CreateJewels() // initializing jewel field
	{
		Vector3 tilesize = DummyTilemap.data.tileSize;
		List<int[]> JewelsForGeneration = new List<int[]>();
		for (int i = 0; i < spritesJewels.GetLength(0); i++)
			for (int j = 0; j < spritesJewels.GetLength(1); j++)
			{
				spritesJewels[i,j] = Instantiate(prefabJewel, DummyTilemap.GetTilePosition(i,j)+ tilesize*0.5f, transform.rotation) as GameObject;
				JewelsForGeneration.Add(new int[]{i, j});
			}
		FillRandomJewels(JewelsForGeneration);
	}

	void FillRandomJewels(List<int[]> Jewels)
	{
		System.Random random = new System.Random();
		int SpritesCount = spritesJewels[0,0].GetComponent<VirtualSprite2D>().SpriteImages.Length;
		do {
			foreach (int[] coords in Jewels)
			{
				do
				{
					spritesJewels[coords[0],coords[1]].GetComponent<VirtualSprite2D>().SetSprite(random.Next(0, SpritesCount));
				}
				while (IsMatch3(coords[0], coords[1]));
			}
		} while (!IsAnywherePotentialMatch3());
	}

	bool IsNeighbors(int CurrentIndexX, int CurrentIndexY)
	{
		return ((((CurrentIndexX - PreviousSpriteIndexX == 1)||(CurrentIndexX - PreviousSpriteIndexX == -1))&&(CurrentIndexY - PreviousSpriteIndexY == 0))||
				(((CurrentIndexY - PreviousSpriteIndexY == 1)||(CurrentIndexY - PreviousSpriteIndexY == -1))&&(CurrentIndexX - PreviousSpriteIndexX == 0)));
	}

	bool IsMatch3(int X, int Y)
	{
		int SpriteID = spritesJewels[X,Y].GetComponent<VirtualSprite2D>().spriteId();
		if (X>=2)
			if ((spritesJewels[X-1,Y].GetComponent<VirtualSprite2D>().spriteId() == SpriteID)&&
			    (spritesJewels[X-2,Y].GetComponent<VirtualSprite2D>().spriteId() == SpriteID))
				return true;
		if (X<spritesJewels.GetLength(0)-2)
			if((spritesJewels[X+1,Y].GetComponent<VirtualSprite2D>().spriteId()== SpriteID)&&
			   (spritesJewels[X+2,Y].GetComponent<VirtualSprite2D>().spriteId() == SpriteID))
				return true;
		if (Y>=2)
			if ((spritesJewels[X,Y-1].GetComponent<VirtualSprite2D>().spriteId() == SpriteID)&&
			    (spritesJewels[X,Y-2].GetComponent<VirtualSprite2D>().spriteId() == SpriteID))
				return true;
		if (Y<spritesJewels.GetLength(1)-2)
			if ((spritesJewels[X,Y+1].GetComponent<VirtualSprite2D>().spriteId() == SpriteID)&&
			    (spritesJewels[X,Y+2].GetComponent<VirtualSprite2D>().spriteId() == SpriteID))
				return true;
		if ((X>0)&&(X<spritesJewels.GetLength(0)-1))
			if ((spritesJewels[X-1,Y].GetComponent<VirtualSprite2D>().spriteId() == SpriteID)&&
			    (spritesJewels[X+1,Y].GetComponent<VirtualSprite2D>().spriteId() == SpriteID))
		    	return true;
		if ((Y>0)&&(Y<spritesJewels.GetLength(1)-1))
			if ((spritesJewels[X,Y-1].GetComponent<VirtualSprite2D>().spriteId() == SpriteID)&&
			    (spritesJewels[X,Y+1].GetComponent<VirtualSprite2D>().spriteId() == SpriteID))
				return true;
		return false;
	}
	
	bool IsAnywherePotentialMatch3() //check the entire field - is somewhere possible match?
	{
		int CurrentSpriteId;
		for (int i = 0; i < spritesJewels.GetLength(0); i++)
			for (int j = 0; j < spritesJewels.GetLength(1); j++)
			{
			CurrentSpriteId = spritesJewels[i,j].GetComponent<VirtualSprite2D>().spriteId();
				if ((i > 0)&&(i < spritesJewels.GetLength(0) - 1))
				{
					if ( CurrentSpriteId == spritesJewels[i-1,j].GetComponent<VirtualSprite2D>().spriteId())
					{
						if (j > 0)
							if (CurrentSpriteId == spritesJewels[i+1,j-1].GetComponent<VirtualSprite2D>().spriteId())
								return true;
						if (j < spritesJewels.GetLength(1) - 1)
							if (CurrentSpriteId == spritesJewels[i+1,j+1].GetComponent<VirtualSprite2D>().spriteId())
								return true;
					}
					if (CurrentSpriteId == spritesJewels[i+1,j].GetComponent<VirtualSprite2D>().spriteId())
					{
						if (j > 0)
							if (CurrentSpriteId == spritesJewels[i-1,j-1].GetComponent<VirtualSprite2D>().spriteId())
								return true;
						if (j < spritesJewels.GetLength(1) - 1)
							if (CurrentSpriteId == spritesJewels[i-1,j+1].GetComponent<VirtualSprite2D>().spriteId())
								return true;
					}
				}
				if ((j > 0)&&(j < spritesJewels.GetLength(0) - 1))
				{
					if (CurrentSpriteId == spritesJewels[i,j-1].GetComponent<VirtualSprite2D>().spriteId())
					{
						if (i > 0)
							if (CurrentSpriteId == spritesJewels[i-1,j+1].GetComponent<VirtualSprite2D>().spriteId())
								return true;
						if (i < spritesJewels.GetLength(0) - 1)
							if (CurrentSpriteId == spritesJewels[i+1,j+1].GetComponent<VirtualSprite2D>().spriteId())
								return true;
					}
					if (CurrentSpriteId == spritesJewels[i,j+1].GetComponent<VirtualSprite2D>().spriteId())
					{
						if (i > 0)
							if (CurrentSpriteId == spritesJewels[i-1,j-1].GetComponent<VirtualSprite2D>().spriteId())
								return true;
						if (i < spritesJewels.GetLength(0) - 1)
							if (CurrentSpriteId == spritesJewels[i+1,j-1].GetComponent<VirtualSprite2D>().spriteId())
								return true;
					}
				}
			}
		return false;
	}

	public void SelectTile(GameObject go)
	{
		if (!IsBusy)
		{
			SelectedSpriteIndexX = SelectedSpriteIndexY = -100;
			for (int i = 0; i < spritesJewels.GetLength(0); i++)
				for (int j = 0; j < spritesJewels.GetLength(1); j++)
					if (go.Equals(spritesJewels[i, j]))
					{
						IsLastSelectedJewelBlocked = true;
						if (BlockedColumns[i]||BlockedRows[j]) //checking for blocked rows / columns
							return;
						if (BlockedMovements[0]) //if left is blocked
							if ((i-PreviousSpriteIndexX==-1)&&(j==PreviousSpriteIndexY))
						    	return;
						if (BlockedMovements[1]) //if right is blocked
							if ((i-PreviousSpriteIndexX==1)&&(j==PreviousSpriteIndexY))
								return;
						if (BlockedMovements[2]) //if up is blocked
							if ((j-PreviousSpriteIndexY==1)&&(i==PreviousSpriteIndexX))
								return;
						if (BlockedMovements[3]) //if down is blocked
							if ((j-PreviousSpriteIndexY==-1)&&(i==PreviousSpriteIndexX))
								return;
						IsLastSelectedJewelBlocked = false;
						SelectedSpriteIndexX = i;
						SelectedSpriteIndexY = j;
						i = spritesJewels.GetLength(0);
						break;
					}		
			if (Border != null) Destroy(Border);

			if (IsNeighbors(SelectedSpriteIndexX, SelectedSpriteIndexY))
			{
				StartCoroutine(TryToMatch(SelectedSpriteIndexX, SelectedSpriteIndexY, PreviousSpriteIndexX,PreviousSpriteIndexY));
			}
			else
			{
				PreviousSpriteIndexX = SelectedSpriteIndexX;
				PreviousSpriteIndexY = SelectedSpriteIndexY;
				Border = Instantiate(prefabBorder, go.transform.position, go.transform.rotation) as GameObject;
			}
		}
	}

	public void MoveJewelBySwipe(GameObject go, Vector2 direction)
	{
		if ((!IsBusy)&&(!IsLastSelectedJewelBlocked))
		{
			if (Math.Abs(direction.x)>Math.Abs(direction.y)) //if horizontal swipe
			{
				if (direction.x > 0) //if right
				{
					if (BlockedMovements[1]) return;
					if (PreviousSpriteIndexX+1< spritesJewels.GetLength(0))
					{
						if (BlockedColumns[PreviousSpriteIndexX+1]) return;
						SelectedSpriteIndexX = PreviousSpriteIndexX + 1;
					}
					else
						return;
				}
				else //if left
				{
					if (BlockedMovements[0]) return;
					if (PreviousSpriteIndexX-1 >= 0)
					{
						if (BlockedColumns[PreviousSpriteIndexX-1]) return;
						SelectedSpriteIndexX = PreviousSpriteIndexX - 1;
					}
					else
						return;
				}
				SelectedSpriteIndexY = PreviousSpriteIndexY;
			}
			else //if vertical
			{
				if (direction.y > 0) //if up
				{
					if (BlockedMovements[2]) return;
					if (PreviousSpriteIndexY+1< spritesJewels.GetLength(1))
					{
						if (BlockedRows[PreviousSpriteIndexY+1]) return;
						SelectedSpriteIndexY = PreviousSpriteIndexY + 1;
					}
					else
						return;
				}
				else //if down
				{
					if (BlockedMovements[3]) return;
					if (PreviousSpriteIndexY-1 >= 0)
					{
						if (BlockedRows[PreviousSpriteIndexY-1]) return;
						SelectedSpriteIndexY = PreviousSpriteIndexY - 1;
					}
					else
						return;
				}
				SelectedSpriteIndexX = PreviousSpriteIndexX;
			}
			if (Border != null) Destroy(Border);
			StartCoroutine(TryToMatch(SelectedSpriteIndexX, SelectedSpriteIndexY, PreviousSpriteIndexX,PreviousSpriteIndexY));
		}
	}
	
	IEnumerator TryToMatch(int x1, int y1, int x2, int y2) //execute the swap, then process if matches and swap back if not
	{
		IsBusy = true;
		PreviousSpriteIndexX = PreviousSpriteIndexY = -100;
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
		Vector3 tilesize = DummyTilemap.data.tileSize;
		foreach (int[] coords in JewelsForGenerate)
			spritesJewels[coords[0],coords[1]] = Instantiate(prefabJewel,
			                                                 DummyTilemap.GetTilePosition(coords[0],coords[1])+ tilesize*0.5f+ new Vector3(0,tilesize.y*coords[2]),
			                                                 transform.rotation) as GameObject;
		FillRandomJewels(JewelsForGenerate);
		
		//sprites movement
		List<Vector3> Goals = new List<Vector3>();
		foreach (int[] coords in JewelsForShift)
			Goals.Add(DummyTilemap.GetTilePosition(coords[0],coords[1]) + tilesize*0.5f);
		yield return StartCoroutine(MoveJewels(JewelsForShift, Goals));

		//recursive check after shift
		if (CheckForMatchesAndDestroyIfMatch())
			yield return(StartCoroutine(ShiftAndGenerateNewJewels()));
	}
	
	List<int[]> GetMatchedJewels(int x, int y) //Get a list of jewels around a point what makes a match (3, at least)
	{
		int SpriteID = spritesJewels[x,y].GetComponent<VirtualSprite2D>().spriteId();
		List<int[]> matchedCoords = new List<int[]>();
		List<int[]> temp = new List<int[]>();
		temp.Add(new int[2]{x,y});
		for (int i = x+1; i < spritesJewels.GetLength(0); i++)
			if (spritesJewels[i,y].GetComponent<VirtualSprite2D>().spriteId()==SpriteID)
				temp.Add(new int[2]{i,y});
		else
			break;
		for (int i = x-1; i >= 0; i--)
			if (spritesJewels[i,y].GetComponent<VirtualSprite2D>().spriteId()==SpriteID)
				temp.Add(new int[2]{i,y});
		else
			break;
		
		if (temp.Count >= 3)
			matchedCoords.AddRange(temp);
		temp.Clear();
		
		for (int j = y+1; j < spritesJewels.GetLength(1); j++)
			if (spritesJewels[x,j].GetComponent<VirtualSprite2D>().spriteId()==SpriteID)
				temp.Add(new int[2]{x,j});
		else
			break;
		for (int j = y-1; j >= 0; j--)
			if (spritesJewels[x,j].GetComponent<VirtualSprite2D>().spriteId()==SpriteID)
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
