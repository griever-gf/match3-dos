using UnityEngine;
using System.Collections;
using System;

public class Filler : MonoBehaviour {

	public tk2dTileMap tilemap;

	public GameObject prefabJewel;

	private GameObject[,] spritesJewels;

	// Use this for initialization
	void Start () {
		spritesJewels = new GameObject[tilemap.width, tilemap.height];
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
		bool IsMatch3;

		for (int i = 0; i < spritesJewels.GetLength(0); i++)
			for (int j = 0; j < spritesJewels.GetLength(1); j++)
			{
				do
				{
					NewSpriteID = random.Next(0, SpritesCount);
					IsMatch3 = false;
					if (i>=2) IsMatch3 			   = IsMatch3 || ((spritesJewels[i-1,j].GetComponent<tk2dSprite>().spriteId == NewSpriteID)||
				                                        		  (spritesJewels[i-2,j].GetComponent<tk2dSprite>().spriteId == NewSpriteID));
					if (i<SpritesCount-2) IsMatch3 = IsMatch3 || ((spritesJewels[i+1,j].GetComponent<tk2dSprite>().spriteId == NewSpriteID)||
				                                              	  (spritesJewels[i+2,j].GetComponent<tk2dSprite>().spriteId == NewSpriteID));
					if (j>=2) IsMatch3 			   = IsMatch3 || ((spritesJewels[i,j-1].GetComponent<tk2dSprite>().spriteId == NewSpriteID)||
					                                        	  (spritesJewels[i,j-2].GetComponent<tk2dSprite>().spriteId == NewSpriteID));
					if (j<SpritesCount-2) IsMatch3 = IsMatch3 || ((spritesJewels[i,j+1].GetComponent<tk2dSprite>().spriteId == NewSpriteID)||
					                                              (spritesJewels[i,j+2].GetComponent<tk2dSprite>().spriteId == NewSpriteID));
				}
				while (IsMatch3);
				spritesJewels[i,j].GetComponent<tk2dSprite>().SetSprite(NewSpriteID);
			}
	}
}
