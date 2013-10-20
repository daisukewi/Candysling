using UnityEngine;
using System.Collections;

public class TouchControl : MonoBehaviour
{
	private BoxCollider boxCollider;
	private bool shutting = false;
	private Plane ArrowPlane;
	
	public GameObject Candy;
	public float ShootForce = 400.0f;
	public Texture2D ArrowTexure;

	// Use this for initialization
	void Start ()
	{
		boxCollider = (BoxCollider)GetComponent(typeof(BoxCollider));
	}
	
	// Update is called once per frame
	void Update ()
	{
		GameObject CandyInstance;
		
		if (Candy == null)
			return;
		
		if (Input.GetMouseButtonDown (0)) {
			
			Vector3 intersectVect = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			intersectVect.z = transform.position.z;
			
			//Draw here the arrow
			
			if (!shutting && boxCollider.bounds.Contains (intersectVect)) {
				shutting = true;
			}
		}
		
		if (Input.GetMouseButtonUp (0) && shutting) {
			
			CandyInstance = Instantiate (Candy, transform.position, transform.rotation) as GameObject;
			Physics.IgnoreCollision (CandyInstance.collider, collider);
			
			Vector3 intersectVect = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			intersectVect.z = transform.position.z;
			
			((Rigidbody)CandyInstance.GetComponent (typeof(Rigidbody))).AddForce ((transform.position - intersectVect) * ShootForce);
			shutting = false;
		}
	}
}
