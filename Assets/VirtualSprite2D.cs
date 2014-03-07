using UnityEngine;
using System.Collections;

public class VirtualSprite2D : MonoBehaviour {

	public Sprite[] SpriteImages;

	public void SetSprite(int idx)
	{
		GetComponent<SpriteRenderer>().sprite = SpriteImages[idx];
	}

	public int spriteId()
	{
		for (int i = 0; i < SpriteImages.Length; i++)
		{
			if (SpriteImages[i].Equals(GetComponent<SpriteRenderer>().sprite))
				return i;
		}
		return -1;
	}
}
