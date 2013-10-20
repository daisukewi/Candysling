using UnityEngine;
using System.Collections;

public class RockBehaviour : MonoBehaviour {
	
	private Camera cam;
	
	// Use this for initialization
	void Start () {
		cam = gameObject.transform.parent.gameObject.GetComponent<Camera> ();
	}
	
	// Update is called once per frame
	void Update () {
		Rigidbody rig = (Rigidbody)GetComponent(typeof(Rigidbody));
		//rig.velocity
	}
}
