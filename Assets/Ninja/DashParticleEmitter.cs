using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashParticleEmitter : MonoBehaviour
{
	ParticleSystem Base;
	ParticleSystem Steam;

	void Start ()
	{
		Base = transform.FindChild ("Base").GetComponent<ParticleSystem> ();
		Steam = transform.FindChild ("Steam").GetComponent<ParticleSystem> ();
	}

	void Update ()
	{
		
	}

	public void Dashed (Vector3 direction)
	{
		if (direction.y > 0) {
			Steam.Play ();
		}
	}

	public void Grounded ()
	{
		Base.Play ();
	}

	public void Ungrounded ()
	{
		Base.Play ();
	}
}
