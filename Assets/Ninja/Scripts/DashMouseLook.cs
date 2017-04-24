using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashMouseLook : MonoBehaviour
{
	public float XSensitivity = 10f;
	public float YSensitivity = 2f;

	public bool ClampVerticalRotation = true;
	public float MinimumY = -10f;
	public float MaximumY = 20f;

	public bool LockCursor = true;
	bool CursorLocked = true;
		
	[Header ("State")]
	public float RotationY = 0.0f;
	public float RotationDeltaY = 0.0f;

	public float RotationX = 0.0f;
	public float RotationDeltaX = 0.0f;

	void Update ()
	{
		UpdateCursorLock ();
	}

	public void UpdateCursorLock ()
	{
		if (LockCursor) {
			InternalLockUpdate ();
		}

		RotationDeltaX = Input.GetAxis ("Mouse X") * XSensitivity;
		RotationDeltaY = Input.GetAxis ("Mouse Y") * YSensitivity;

		RotationX += RotationDeltaX;

		RotationY += RotationDeltaY;
		RotationY = Mathf.Clamp (RotationY, MinimumY, MaximumY);
	}

	void InternalLockUpdate ()
	{
		if (Input.GetKeyUp (KeyCode.Escape)) {
			CursorLocked = false;
		} else if (Input.GetMouseButtonUp (0)) {
			CursorLocked = true;
		}

		if (CursorLocked) {
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		} else {
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}
	}

	void OnDisable ()
	{
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
	}

	void OnDestroy ()
	{
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
	}
}
