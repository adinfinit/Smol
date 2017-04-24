using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Rigidbody))]
[RequireComponent (typeof(CapsuleCollider))]
[RequireComponent (typeof(DashMouseLook))]
public class DashController : MonoBehaviour
{
	public float GravityScale = 10.0f;
	public float VelocityDampening = 0.9f;
	public float MoveSpeed = 15.0f;

	[Header ("Dash")]
	public float DashDistance = 10.0f;
	public float JumpDistance = 12.0f;

	public float DashDuration = 0.35f;
	public float FloatDuration = 0.15f;

	public float ForwardSpeedMultiplier = 1.5f;
	public int MaxDashCount = 3;
	public int MaxJumpCount = 2;

	[Header ("State")]

	CapsuleCollider Capsule;
	Rigidbody Body;
	Camera MainCamera;

	DashMouseLook MouseLook;
	DashParticleEmitter Callbacks;

	float RotationY = 0.0f;
	float ViewTilt = 0.0f;

	void Start ()
	{
		Capsule = GetComponent<CapsuleCollider> ();
		Body = GetComponent<Rigidbody> ();

		MouseLook = GetComponent<DashMouseLook> ();
		Callbacks = GetComponentInChildren<DashParticleEmitter> ();

		MainCamera = Camera.main;
	}

	Vector3 acceleration = Vector3.zero;
	Vector3 previousVelocity = Vector3.zero;

	public Vector3 getAcceleration ()
	{
		return acceleration;
	}

	void FixedUpdate ()
	{
		GroundCheck ();

		acceleration = Body.velocity - previousVelocity;
		FixedUpdateDash ();
		acceleration = Body.velocity - previousVelocity;
		previousVelocity = Body.velocity;
	}

	Vector3 Move = Vector3.zero;

	float Move_Left = 0.0f;
	float Move_Right = 0.0f;
	float Move_Forward = 0.0f;
	float Move_Back = 0.0f;
	float Move_Up = 0.0f;

	void Update ()
	{
		if (Input.GetKeyUp (KeyCode.Escape)) {
			SceneController.StaticLoadMenu ();
			return;
		}


		Move = Vector3.zero;

		TryMove (KeyCode.A, Vector3.left, ref Move_Left, ref DashesRemaining, DashDistance);
		TryMove (KeyCode.D, Vector3.right, ref Move_Right, ref DashesRemaining, DashDistance);
		TryMove (KeyCode.W, Vector3.forward, ref Move_Forward, ref DashesRemaining, DashDistance);
		TryMove (KeyCode.S, Vector3.back, ref Move_Back, ref DashesRemaining, DashDistance);
		TryMove (KeyCode.Space, Vector3.up, ref Move_Up, ref JumpsRemaining, JumpDistance);

		{
			RotationY += MouseLook.RotationDeltaX;
			ViewTilt = MouseLook.RotationY;
		}

		UpdateCamera ();

		#if UNITY_EDITOR
		if (Dashing) {
			foreach (var dash in ActiveDashes) {
				Debug.DrawRay (dash.Start, dash.Target - dash.Start, Color.red);
			}
		}

		Debug.DrawRay (transform.position, planeForward () * 2f, Color.blue);
		#endif
	}

	void TryMove (KeyCode key, Vector3 dir, ref float timer, ref int dashesRemaining, float dashDistance)
	{
		bool Walk = Input.GetKey (KeyCode.LeftShift) && (key != KeyCode.Space);

		if (key == KeyCode.W && Grounded) {
			if (Input.GetKey (KeyCode.W)) {
				if (!Walk) {
					Move += dir * ForwardSpeedMultiplier * (DashDistance / DashDuration);
				} else {
					timer += Time.fixedTime;
					Move += dir * Smoothstep (Mathf.Clamp01 (timer)) * MoveSpeed;
				}
			} else {
				timer = 0.0f;
			}
		} else {
			if (Input.GetKeyDown (key) && !Walk) {
				Dash (key, dir, ref dashesRemaining, dashDistance);
			} else if (Input.GetKey (key)) {
				timer += Time.fixedTime;
				Move += dir * Smoothstep (Mathf.Clamp01 (timer)) * MoveSpeed;
			} else {
				timer = 0.0f;
			}
		}

		if (Input.GetKeyUp (key)) {
			if (key != KeyCode.Space) {
				ActiveDashes.RemoveAll ((dash) => dash.Key == key);
			}
		}
	}

	#region Dashing

	List<ActiveDash> ActiveDashes = new List<ActiveDash> ();


	class ActiveDash
	{
		public KeyCode Key;
		public float Progress;
		public float Duration;
		public Vector3 Start;
		public Vector3 Target;
	}

	bool Falling = false;
	bool Dashing = false;
	float FloatingTimer = 0.0f;
	public int DashesRemaining = 0;
	public int JumpsRemaining = 0;

	public bool isDashing ()
	{
		return ActiveDashes.Count > 0;
	}

	public void ExternalDash (Vector3 globalDirection)
	{
		//int dashesRemaining = 1000;
		//float magnitude = globalDirection.magnitude;
		//globalDirection *= 1.0f / magnitude;
		// Vector3 localDirection = Body.transform.worldToLocalMatrix * ;
		// Dash (KeyCode.Pause, forwardInverseRotation () * localDirection, ref dashesRemaining, magnitude);

		AddDash (KeyCode.Pause, globalDirection);
		ResetDashCounters ();
	}

	void Dash (KeyCode key, Vector3 localDirection, ref int dashesRemaining, float dashDistance)
	{
		dashesRemaining--;
		if (dashesRemaining < 0) {
			return;
		}

		localDirection = localDirection * dashDistance / localDirection.magnitude;
		AddDash (key, forwardRotation () * localDirection);
		Callbacks.Dashed (localDirection);
	}

	void AddDash (KeyCode key, Vector3 globalDirection)
	{
		ActiveDash dash = new ActiveDash ();
		dash.Key = key;
		dash.Progress = 0.0f;
		dash.Duration = DashDuration;
		dash.Start = Body.position;
		dash.Target = dash.Start + globalDirection;
		ActiveDashes.Add (dash);

		Dashing = true;
		Falling |= !Grounded;
	}

	void ResetDashCounters ()
	{
		DashesRemaining = MaxDashCount;
		JumpsRemaining = MaxJumpCount;
	}

	void FixedUpdateDash ()
	{
		float dt = Time.fixedDeltaTime;

		Vector3 move = forwardRotation () * Move;
		move.y = 0f;

		if (ActiveDashes.Count > 0) {
			FloatingTimer = FloatDuration;
			List<ActiveDash> stillDashing = new List<ActiveDash> ();
			Body.velocity = move;
			foreach (var dash in ActiveDashes) {
				dash.Progress += dt;
				bool dashing = dash.Progress < dash.Duration;
				if (dashing) {
					stillDashing.Add (dash);
					float pv = SmoothstepPrim (dash.Progress / dash.Duration);
					Body.velocity += (dash.Target - dash.Start) * pv / dash.Duration;
				}
			}
			ActiveDashes = stillDashing;
		} else {
			FloatingTimer -= dt;
			if (FloatingTimer > 0) {
				Body.velocity = move;
			} else {
				Dashing = false;
				Body.velocity = new Vector3 (move.x, Body.velocity.y, move.z);
			}
		}
		Body.useGravity = !Dashing;

		if (Grounded) {// reset dashes and falling
			ResetDashCounters ();
			Falling = false;
		}
		if (!Grounded) {// faster falling
			Body.AddForce (Physics.gravity * (GravityScale - 1f) * Body.mass);
		}
		if (!Dashing && Grounded) { // dampen sliding
			Body.velocity.Scale (new Vector3 (VelocityDampening, 1.0f, VelocityDampening));
		}

		const float tiltScale = 0.005f;
		Vector3 tilt = Body.velocity * 0.7f;
		float tiltSmooth = 0.3f;

		if (Dashing) { // tilt the player
			acceleration = Body.velocity - previousVelocity;
			tilt = tilt * 0.5f + acceleration;
			tiltSmooth = 0.7f;
		}

		tilt *= tiltScale;
		tilt.x = Mathf.Clamp (tilt.x, -1f, 1f);
		tilt.y = Mathf.Clamp (tilt.y, -1f, 1f);
		tilt.z = Mathf.Clamp (tilt.z, -1f, 1f);
		tilt.y = 0;

		tilt = forwardInverseRotation () * tilt;
		Quaternion target = Quaternion.Euler (new Vector3 (tilt.z * 180f, RotationY, -tilt.x * 180f));
		Body.rotation = Quaternion.Lerp (Body.rotation, target, tiltSmooth);
	}

	#endregion


	#region Grounding

	bool WasGrounded = true;
	bool Grounded = true;

	const float shellOffset = 0.1f;

	Vector3 getGroundPoint (float groundCheckDistance)
	{
		return transform.position + Capsule.center + Vector3.down * (Capsule.height / 2f - Capsule.radius - groundCheckDistance / 2f);
	}

	bool GroundCast (out RaycastHit hitInfo, float groundCheckDistance)
	{
		Vector3 bottom = getGroundPoint (groundCheckDistance);

		#if UNITY_EDITOR
		Debug.DrawRay (bottom, Vector3.down * groundCheckDistance, Color.green);
		#endif

		return Physics.SphereCast (
			bottom, Capsule.radius * (1.0f - shellOffset), Vector3.down, out hitInfo,
			(Capsule.height / 2f) - Capsule.radius + groundCheckDistance, 
			Physics.AllLayers, QueryTriggerInteraction.Ignore);
	}

	void GroundCheck ()
	{
		RaycastHit hitInfo;
		if (GroundCast (out hitInfo, 0.01f)) {
			Grounded = true;
		} else {
			Grounded = false;
		}
		if (Grounded != WasGrounded) {
			if (Grounded) {
				Callbacks.Grounded ();
			} else {
				Callbacks.Ungrounded ();
			}
		}
		WasGrounded = Grounded;
	}

	private void StickToGround ()
	{
		RaycastHit hitInfo;
		if (GroundCast (out hitInfo, 0.5f)) {
			if (Mathf.Abs (Vector3.Angle (hitInfo.normal, Vector3.up)) < 85f) {
				Body.velocity = Vector3.ProjectOnPlane (Body.velocity, hitInfo.normal);
			}
		}
	}

	#endregion

	#region Camera

	[Header ("Camera")]
	public bool FollowEnabled = false;
	public bool LookAtEnabled = false;

	void UpdateCamera ()
	{
		Quaternion previousRotation = MainCamera.transform.rotation;

		if (FollowEnabled) {
			Vector3 target = transform.position - planeForward () * 10.0f + Vector3.up * 4.0f;
			MainCamera.transform.position = Vector3.Lerp (MainCamera.transform.position, target, 0.2f);
		}

		if (LookAtEnabled) {
			Vector3 lookAt = transform.position;
			lookAt.y += ViewTilt + 1f;
			MainCamera.transform.LookAt (lookAt);

			MainCamera.transform.rotation = Quaternion.Lerp (previousRotation, MainCamera.transform.rotation, 0.2f);
		}
	}

	public float getViewTilt ()
	{
		return ViewTilt;
	}

	#endregion

	#region Misc

	public Quaternion forwardRotation ()
	{
		return Quaternion.AngleAxis (RotationY, Vector3.up);
	}

	public Quaternion forwardInverseRotation ()
	{
		return Quaternion.AngleAxis (-RotationY, Vector3.up);
	}

	Vector3 planeForward ()
	{
		Vector3 forward = new Vector3 ();
		forward.x = Mathf.Sin (RotationY * Mathf.Deg2Rad);
		forward.z = Mathf.Cos (RotationY * Mathf.Deg2Rad);
		return forward;
	}


	public static float Smoothstep (float p)
	{
		float p2 = p * p;
		return p2 / (2.0f * (p2 - p) + 1.0f);
	}

	public static float SmoothstepPrim (float p)
	{
		float p2 = p * p;
		float d = 2 * p2 - 2 * p + 1;
		return 2 * (p - p2) / (d * d);
	}

	#endregion
}
