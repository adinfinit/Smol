using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomBounce : MonoBehaviour
{

	private GameObject player;
	private DashController controller;
	private Rigidbody rb;

	public float magnitude = 10f;
	public float bounceMagnitude = 0.3f;

	float bounceTimer = 1.0f;
	float initialScale = 0.0f;

	// Use this for initialization
	void Start ()
	{
		player = GameObject.FindGameObjectWithTag ("Player");
		controller = player.GetComponent<DashController> ();
		rb = player.GetComponent<Rigidbody> ();       
		initialScale = transform.localScale.y;
	}

	void OnDrawGizmos ()
	{
		Vector3 origin = transform.position;
		SphereCollider scollider = GetComponent<SphereCollider> ();
		if (scollider != null) {
			Vector3 newpos = transform.localToWorldMatrix * scollider.center;
			origin = transform.position + newpos;
		}
		Vector3 target = origin + transform.up * magnitude;
		Gizmos.DrawLine (origin, target);
		Gizmos.DrawSphere (target, 0.3f);
		Vector3 offset = new Vector3 ();
		offset.y = target.y - origin.y;
		Gizmos.DrawLine (target, target - offset);
	}

	void Update ()
	{
		if (bounceTimer < 0.3f) {
			bounceTimer += Time.deltaTime;
			Vector3 t = transform.localScale;
			t.y = initialScale + bounceMagnitude * SmoothstepPrim (Mathf.Clamp01 (bounceTimer / 0.3f));
			transform.localScale = t;
			if (bounceTimer >= 0.3f) {
				t.y = initialScale;
				transform.localScale = t;	
			}
		}

		#if UNITY_EDITOR
		Debug.DrawRay (this.transform.position, transform.up * magnitude, Color.blue);
		#endif
	}

	void OnTriggerEnter (Collider other)
	{
		if (other.tag == "Player") {
			controller.ExternalDash (transform.up * magnitude);           
			bounceTimer = 0.0f;
		}
	}


	static float SmoothstepPrim (float p)
	{
		float p2 = p * p;
		float d = 2 * p2 - 2 * p + 1;
		return 2 * (p - p2) / (d * d);
	}
}
