using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceStartingPosition : MonoBehaviour
{

	Vector3 StartPosition;

	void Start ()
	{
		StartPosition = transform.localPosition;
	}

	void Update ()
	{
		transform.localPosition = StartPosition;
	}
}
