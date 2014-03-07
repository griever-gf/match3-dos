using UnityEngine;
using System.Collections;
using System;

public class SwitchButton : MonoBehaviour {

	public Sprite spriteOn;
	public Sprite spriteOff;
	public bool IsChecked;
	public enum SwitchType {RowBlock, ColumnBlock, MovementBlock};

	public SwitchType BlockType;
	//public int BlockNumber;
	//public MovementType BlockDirection;

	void Start()
	{
		IsChecked = false;
	}

	void OnMouseDown() //also works for touch input since unity 4.3, works on gui or collider only
	{
		IsChecked = !IsChecked;
		this.GetComponent<SpriteRenderer>().sprite = IsChecked ? spriteOn : spriteOff;
		//Debug.Log(GetComponentInChildren<TextMesh>().text);
		//Debug.Log(FindObjectOfType<JewelData>().Length.ToString());
		switch (BlockType)
		{
			case SwitchType.RowBlock:
				FindObjectOfType<JewelData>().BlockedRows[Convert.ToInt32(GetComponentInChildren<TextMesh>().text)-1] = IsChecked;
				break;
			case SwitchType.ColumnBlock:
				FindObjectOfType<JewelData>().BlockedColumns[Convert.ToInt32(GetComponentInChildren<TextMesh>().text)-1] = IsChecked;
				break;
			case SwitchType.MovementBlock:
			{
				switch (GetComponentInChildren<TextMesh>().text)
				{
					case "L":
						FindObjectOfType<JewelData>().BlockedMovements[0] = IsChecked;
						break;
					case "R":
						FindObjectOfType<JewelData>().BlockedMovements[1] = IsChecked;
						break;
					case "U":
						FindObjectOfType<JewelData>().BlockedMovements[2] = IsChecked;
						break;
					case "D":
						FindObjectOfType<JewelData>().BlockedMovements[3] = IsChecked;
						break;
			}
		}
			break;
		}
	}
}
