using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(GunController))]
public class Player : LivingEntity {

	public float moveSpeed = 5f;

	public Crosshairs crossHairs;

	Camera viewCamera;
	PlayerController controller;
	GunController gunController;

	void Awake()
	{
		viewCamera = Camera.main;
		gunController = GetComponent<GunController> ();
		controller = GetComponent<PlayerController> ();
		FindObjectOfType<Spawner> ().OnNewWave += OnNewWave;
	}

	protected override void Start()
	{
		base.Start ();

	}

	void OnNewWave(int waveNumber)
	{
		health = startingHealth;
		gunController.EquipGun (waveNumber - 1);
	}

	void Update()
	{
		Vector3 moveInput = new Vector3 (Input.GetAxisRaw ("Horizontal"), 0f, Input.GetAxisRaw ("Vertical"));
		Vector3 moveVelocity = moveInput.normalized * moveSpeed;
		controller.Move (moveVelocity);

		Ray ray = viewCamera.ScreenPointToRay (Input.mousePosition);
		Plane groundPlane = new Plane (Vector3.up, Vector3.up * gunController.GunHeight);
		float rayDistance;
		if (groundPlane.Raycast (ray, out rayDistance)) {
			Vector3 point = ray.GetPoint (rayDistance);
			controller.LookAt (point);
			crossHairs.transform.position = point;
			crossHairs.DetectTargets (ray);
			if((new Vector2(point.x, point.z) - new Vector2(transform.position.x, transform.position.z)).sqrMagnitude > 1f)
				gunController.Aim (point);
		}

		if (Input.GetMouseButton (0)) {
			gunController.OnTriggerHold ();
		}

		if (Input.GetMouseButtonUp (0)) {
			gunController.OnTriggerRelease ();
		}

		if (Input.GetKeyDown (KeyCode.R))
			gunController.Reload ();
	}

	public override void Die ()
	{
		AudioManager.instance.PlaySound ("Player Death", transform.position);
		base.Die ();
	}
}
