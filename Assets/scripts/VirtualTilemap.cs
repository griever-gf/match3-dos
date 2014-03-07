using UnityEngine;
using System.Collections;


public class VirtualTilemap : MonoBehaviour {
	public int width = 8;
	public int height = 8;

	public datatype data;
	public struct datatype
	{
		public Vector3 tileSize;
	}

	private float tileWidth, tileHeight;

	void Awake()
	{
		tileWidth = GetComponent<SpriteRenderer>().renderer.bounds.size.x / width;
		tileHeight = GetComponent<SpriteRenderer>().renderer.bounds.size.y / height;
		data.tileSize = new Vector3(tileWidth, tileHeight);
	}

	public Vector3 GetTilePosition(int i, int j)
	{
		return (new Vector3(GetComponent<SpriteRenderer>().transform.position.x + tileWidth*(i-(float)width/2),
		                    GetComponent<SpriteRenderer>().transform.position.y + tileHeight*(j-(float)height/2)));
	}
}
