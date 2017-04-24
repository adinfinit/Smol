using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NinjaPosition : MonoBehaviour
{

	GameObject Player;
	GameObject Tilt;
	DashController Controller;

	void Start ()
	{
		Player = GameObject.FindGameObjectWithTag ("Player");
		Controller = Player.GetComponent<DashController> ();

		Tilt = transform.FindChild ("Camera Tilt").gameObject;
	}

	void Update ()
	{
		if (Player == null) {
			return;
		}

		transform.position = Player.transform.position;
		transform.rotation = Controller.forwardRotation ();

		if (Tilt != null) {
			float p = Controller.getViewTilt () / 20f;
			if (p > 0) {
				float py = Mathf.Lerp (4f, 6f, p);
				float rx = Mathf.Lerp (10f, -15f, p);
				Tilt.transform.localPosition = new Vector3 (0f, py, 0f);
				Tilt.transform.localRotation = Quaternion.Euler (new Vector3 (rx, 0f, 0f));
			} else {
				float py = Mathf.Lerp (4f, 2f, -p);
				float rx = Mathf.Lerp (10f, 90f, -p);
				Tilt.transform.localPosition = new Vector3 (0f, py, 0f);
				Tilt.transform.localRotation = Quaternion.Euler (new Vector3 (rx, 0f, 0f));
			}
		}
	}
}
