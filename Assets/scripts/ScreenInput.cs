using UnityEngine;
using System.Collections;

public class ScreenInput : MonoBehaviour {

	private float lastClickTime;
	
	void Update () {
		//touch input
		if(Input.touchCount > 0)
		{
			if ((Input.GetTouch(0).phase == TouchPhase.Began)||(Input.GetTouch(0).phase == TouchPhase.Stationary))
				if (Time.time > lastClickTime + 0.5f)
				{
					ProcessScreenContact(Input.GetTouch(0).position);
					lastClickTime = Time.time;
				}
			if (Input.GetTouch(0).phase == TouchPhase.Ended)
			{
				
			}
		}
		//mouse input
		if (Input.GetKey(KeyCode.Mouse0))
			if (Time.time > lastClickTime + 0.5f)
			{
				ProcessScreenContact(Input.mousePosition);
				lastClickTime = Time.time;
			}
	}

	void ProcessScreenContact(Vector3 ContactCoords)
	{
		Ray ray = Camera.main.ScreenPointToRay(ContactCoords);
		RaycastHit hit = new RaycastHit();
		if(Physics.Raycast(ray, out hit))
		{
			if (hit.transform.name.Contains("prefabJewel"))
				GetComponent<JewelData>().SelectTile( hit.transform.gameObject);
		}
	}
}
