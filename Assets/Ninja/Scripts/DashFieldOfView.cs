using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashFieldOfView : MonoBehaviour
{

	GameObject player;
	Rigidbody body;
	DashController controller;
	Camera targetCamera;

	void Start ()
	{
		player = GameObject.FindGameObjectWithTag ("Player");
		body = player.GetComponent<Rigidbody> ();
		controller = player.GetComponent<DashController> ();
		targetCamera = Camera.main;
	}

	void Update ()
	{
		if (player == null) {
			return;
		}

		Vector3 localVelocity = controller.forwardInverseRotation () * body.velocity;
		float magnitude = Mathf.Clamp01 (Mathf.Abs (localVelocity.z) * 0.01f);
		targetCamera.fieldOfView = Mathf.Lerp (targetCamera.fieldOfView, 60 + magnitude * 7f, 0.1f * Time.deltaTime);
	}
}
