using UnityEngine;
using System.Collections;
using System;

public class JewelData : MonoBehaviour {

	public tk2dTileMap tilemap;

	public GameObject prefabJewel;
	public GameObject prefabBorder;

	private GameObject[,] spritesJewels;
	private GameObject Border;

	int PreviousSpriteIndexX, PreviousSpriteIndexY;
	bool IsFirstJewelInPair;

	// Use this for initialization
	void Start () {
		spritesJewels = new GameObject[tilemap.width, tilemap.height];
		IsFirstJewelInPair = true;
		CreateJewels();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void CreateJewels()
	{
		Vector3 tilesize = tilemap.data.tileSize;
		for (int i = 0; i < spritesJewels.GetLength(0); i++)
			for (int j = 0; j < spritesJewels.GetLength(1); j++)
			{
				spritesJewels[i,j] = Instantiate(prefabJewel, tilemap.GetTilePosition(i,j)+ tilesize*0.5f, transform.rotation) as GameObject;//as tk2dSprite;
				//Debug.Log(spritesJewels[i,j].GetComponent<tk2dSprite>().spriteId);
			}
		int SpritesCount = spritesJewels[0,0].GetComponent<tk2dSprite>().Collection.Count;
		System.Random random = new System.Random();
		int NewSpriteID;

		for (int i = 0; i < spritesJewels.GetLength(0); i++)
			for (int j = 0; j < spritesJewels.GetLength(1); j++)
			{
				do
				{
					NewSpriteID = random.Next(0, SpritesCount);
				}
				while (IsMatch3(i, j, NewSpriteID));
				spritesJewels[i,j].GetComponent<tk2dSprite>().SetSprite(NewSpriteID);
			}
	}

	public void SelectTile(GameObject go)
	{
		if (go.name.Contains("prefabJewel"))
		{
			int SelectedSpriteIndexX = -10;
			int SelectedSpriteIndexY = -10;
			for (int i = 0; i < spritesJewels.GetLength(0); i++)
				for (int j = 0; j < spritesJewels.GetLength(1); j++)
					if (go.Equals(spritesJewels[i, j]))
					{
						SelectedSpriteIndexX = i;
						SelectedSpriteIndexY = j;
						//Debug.Log("Sprite #: "+ i + "," + j);
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
				{
					Debug.Log("Neighbors!");
				}
				else
					Debug.Log("Not Neighbors!");
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

	bool IsMatch3(int X, int Y, int SpriteID)
	{
		int SpritesCount = spritesJewels[0,0].GetComponent<tk2dSprite>().Collection.Count;
		bool result = false;
		if (X>=2) result				= result || ((spritesJewels[X-1,Y].GetComponent<tk2dSprite>().spriteId == SpriteID)||
		                                        	 (spritesJewels[X-2,Y].GetComponent<tk2dSprite>().spriteId == SpriteID));
		if (X<SpritesCount-2) result 	= result || ((spritesJewels[X+1,Y].GetComponent<tk2dSprite>().spriteId == SpriteID)||
		                                             (spritesJewels[X+2,Y].GetComponent<tk2dSprite>().spriteId == SpriteID));
		if (Y>=2) result 			   	= result || ((spritesJewels[X,Y-1].GetComponent<tk2dSprite>().spriteId == SpriteID)||
		                                        	 (spritesJewels[X,Y-2].GetComponent<tk2dSprite>().spriteId == SpriteID));
		if (Y<SpritesCount-2) result 	= result || ((spritesJewels[X,Y+1].GetComponent<tk2dSprite>().spriteId == SpriteID)||
	                                              	 (spritesJewels[X,Y+2].GetComponent<tk2dSprite>().spriteId == SpriteID));
		return result;
	}
}
