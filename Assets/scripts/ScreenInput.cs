using UnityEngine;
using System.Collections;

public class ScreenInput : MonoBehaviour {

	private float lastClickTime;
	private GameObject LastSelectedJewel;
	private bool IsLastBeginTouchWasJewel;
	
	void Update () {
		//touch input
		if(Input.touchCount > 0)
		{
			if ((Input.GetTouch(0).phase == TouchPhase.Began)||(Input.GetTouch(0).phase == TouchPhase.Stationary))
				if (Time.time > lastClickTime + 0.5f)
				{
					ProcessScreenContact(Input.GetTouch(0).position, true);
					lastClickTime = Time.time;
				}
			if (Input.GetTouch(0).phase == TouchPhase.Ended)
			{
					ProcessScreenContact(Input.GetTouch(0).position, false);
					lastClickTime = Time.time;
			}
		}
		else
		//mouse input
		if (Input.GetKey(KeyCode.Mouse0))
			if (Time.time > lastClickTime + 0.5f)
			{
				ProcessScreenContact(Input.mousePosition, true);
				lastClickTime = Time.time;
			}
	}

	void ProcessScreenContact(Vector2 ContactCoords, bool isBeginTouchPhase)
	{
		Ray ray = Camera.main.ScreenPointToRay(ContactCoords);
		RaycastHit hit = new RaycastHit();
		if(Physics.Raycast(ray, out hit))
		{
			if (hit.transform.name.Contains("prefabJewel"))
			{
				if (isBeginTouchPhase)
				{
					LastSelectedJewel = hit.transform.gameObject;
					IsLastBeginTouchWasJewel = true;
					GetComponent<JewelData>().SelectTile( hit.transform.gameObject);
				}
				else
				{
					if (LastSelectedJewel.Equals(hit.transform.gameObject))
						return;
				}
			}
			else
				if (isBeginTouchPhase)
					IsLastBeginTouchWasJewel = false;
		}
		else
			if (isBeginTouchPhase)
				IsLastBeginTouchWasJewel = false;
		if ((!isBeginTouchPhase)&&IsLastBeginTouchWasJewel)
			GetComponent<JewelData>().MoveJewelBySwipe(LastSelectedJewel, ContactCoords - (new Vector2(LastSelectedJewel.transform.position.x, LastSelectedJewel.transform.position.y)));
	}
}
