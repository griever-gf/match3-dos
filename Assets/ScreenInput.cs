using UnityEngine;
using System.Collections;

public class ScreenInput : MonoBehaviour {

	bool moving = false;
	GameObject TouchedObject;
	public float lastShotTime;
	public GameObject prefabBorder;
	private GameObject Border;

	// Update is called once per frame
	void Update () {
		if(Input.touchCount == 1)
		{ 
			// touch on screen
			if(Input.GetTouch(0).phase == TouchPhase.Began)
			{
				Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
				RaycastHit hit = new RaycastHit();
				moving = Physics.Raycast (ray, out hit, 1000);
				if(moving)
				{
					TouchedObject = hit.transform.gameObject;
					Debug.Log("Touch Detected on : " + TouchedObject.name);
				}
				
			}
			// release touch/dragging
			if((Input.GetTouch(0).phase == TouchPhase.Ended || Input.GetTouch(0).phase == TouchPhase.Canceled) && TouchedObject != null)
			{
				moving = false;
				Debug.Log("Touch Released from : " + TouchedObject.name);
			}
		}
		//mouse input
		if (Input.GetKey(KeyCode.Mouse0))
			if (Time.time > lastShotTime +1.0f)
			{
				//Debug.Log("LMB!");
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit = new RaycastHit();
				if(Physics.Raycast(ray, out hit))
				{
					TouchedObject = hit.transform.gameObject;
					Debug.Log("Mouse Click Detected on : " + TouchedObject.name);
					if (Border != null) Destroy(Border);
					Border = Instantiate(prefabBorder, TouchedObject.transform.position, TouchedObject.transform.rotation) as GameObject;
				}
				lastShotTime = Time.time;
			}
	}
}
