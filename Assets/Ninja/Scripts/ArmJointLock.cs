using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(CharacterJoint))]
[RequireComponent (typeof(Rigidbody))]
public class ArmJointLock : MonoBehaviour
{
	CharacterJoint Joint;
	Rigidbody Body;

	/*
	Vector3 BaseRotation = Vector3.zero;

	bool LimitRotation = true;
	Vector3 RotationEulerMin = new Vector3 (-20, -5, -20);
	Vector3 RotationEulerMax = new Vector3 (20, 5, 20);
*/

	void Start ()
	{
		Joint = GetComponent<CharacterJoint> ();	
		Body = GetComponent<Rigidbody> ();

		// BaseRotation = transform.localRotation.eulerAngles;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Joint == null || Body == null) {
			return;
		}

		Body.MovePosition (Joint.connectedBody.position + Joint.connectedAnchor);

		/*
		Vector3 min = BaseRotation + RotationEulerMin;
		Vector3 max = BaseRotation + RotationEulerMax;

		Vector3 rotation = transform.localRotation.eulerAngles;

		rotation.x = Mathf.Clamp (rotation.x, min.x, max.x);
		rotation.y = Mathf.Clamp (rotation.y, min.y, max.y);
		rotation.z = Mathf.Clamp (rotation.z, min.z, max.z);

		transform.localRotation = Quaternion.Euler (rotation);
		*/
	}
}
