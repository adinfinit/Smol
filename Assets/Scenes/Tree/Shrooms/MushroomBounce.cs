using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomBounce : MonoBehaviour
{

	private GameObject player;
	private DashController controller;
	private Rigidbody rb;

	public float magnitude = 10f;

	// Use this for initialization
	void Start ()
	{
		player = GameObject.FindGameObjectWithTag ("Player");
		controller = player.GetComponent<DashController> ();
		rb = player.GetComponent<Rigidbody> ();       
	}

	void Update ()
	{
		Debug.DrawRay (this.transform.position, Vector3.up * 10f, Color.red);
		Debug.DrawRay (this.transform.position, transform.rotation * Vector3.up * 3f, Color.black);
		Debug.DrawRay (this.transform.position, transform.up * 3f, Color.blue);
	}

	void OnTriggerEnter (Collider other)
	{
		if (other.tag == "Player") {
			controller.ExternalDash (transform.up * magnitude);           
		}
	}
}
