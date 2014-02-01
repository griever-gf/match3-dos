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
				Debug.Log(spritesJewels[i,j].GetComponent<tk2dSprite>().spriteId);
				//Debug.Log (spritesJewels[i,j].name);
				//spritesJewels[i,j].Collection.Id;
				//spritesJewels[i,j].SetSprite("Jewel_5");
			}
		int SpritesCount = spritesJewels[0,0].GetComponent<tk2dSprite>().Collection.Count;
		System.Random random = new System.Random();

		for (int i = 0; i < spritesJewels.GetLength(0); i++)
			for (int j = 0; j < spritesJewels.GetLength(1); j++)
				spritesJewels[i,j].GetComponent<tk2dSprite>().SetSprite(random.Next(0, SpritesCount));
		//Debug.Log("Sprite Variants: " + sp.Collection.Count);
		//for (int i = 0; i < spritesJewels.GetLength(0); i++)
			//for (int j = 0; j < spritesJewels.GetLength(1); j++)
				//j = 1;
				//spritesJewels[i,j].SetSprite("Jewel_5");
	}
}
