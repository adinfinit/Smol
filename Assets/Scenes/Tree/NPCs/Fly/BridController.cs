using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridController : MonoBehaviour {

    public bool isTriggered = false;
    public float speed = 10f;

    private Vector3 direction;

    private Rigidbody rb;

	// Use this for initialization
	void Start () {

        rb = this.gameObject.GetComponent<Rigidbody>();
		
	}
	
	// Update is called once per frame
	void Update () {

        if (isTriggered)
        {
            // do burd stuff 
            this.rb.position +=  Time.deltaTime * ((speed * direction) +
                new Vector3(4.1f * Mathf.Sin(3f * Time.time) + 1.3f * Mathf.Sin(1f * Time.time),
                            1.4f * Mathf.Sin(4.3f * Time.time),
                            4.25f * Mathf.Sin(Time.time * 0.1f) + 3.7f * Mathf.Sin(Time.time * 1.5f)));
            Vector3 newDir = Vector3.RotateTowards(transform.forward, (direction - new Vector3(0f, 0.85f, 0f)), 3f * Time.deltaTime, 0.0F);
            Debug.DrawRay(transform.position, newDir, Color.red);
            transform.rotation = Quaternion.LookRotation(newDir);

        }
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {

            isTriggered = true;
            rb.useGravity = false;

            direction = (this.rb.position - other.transform.position);
            direction = new Vector3(direction.x, 0f, direction.z).normalized;
            direction = new Vector3(direction.x, 1f, direction.z);
        }
    }
}
