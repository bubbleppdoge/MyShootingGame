﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour {

	public enum FireMode {Auto, Burst, Single}
	public FireMode fireMode;

	public Transform[] projectileSpawn;
	public Projectile projectile;
	public float msBetweenShots = 100f;
	public float muzzleVelocity = 35f;
	public int burstCount;
	public int projectilesPerMag;
	public float reloadTime = 0.3f;

	[Header("Recoil")]
	public Vector2 kickMinMax = new Vector2(0.05f, 0.2f);
	public Vector2 recoilAngleMinMax = new Vector2(3f, 5f);
	public float recoilMoveSettleTime = 0.1f;
	public float recoilRotationSettleTime = 0.1f;

	[Header("Effect")]
	public Transform shell;
	public Transform shellEjection;
	public AudioClip shootAudio;
	public AudioClip reloadAudio;

	MuzzleFlash muzzleFlash;
	float nextShotTime;

	bool triggerReleasedSinceLastShot;
	int shotsRemainingInBurst;
	int projectilesRemainingInMag;
	bool isReload;

	Vector3 recoilSmoothDampVelocity;
	float recoilRotSmoothDampVelocity;
	float recoilAngle;

	void Start()
	{
		muzzleFlash = GetComponent<MuzzleFlash> ();
		shotsRemainingInBurst = burstCount;
		projectilesRemainingInMag = projectilesPerMag;
	}

	void LateUpdate()
	{
		transform.localPosition = Vector3.SmoothDamp (transform.localPosition, Vector3.zero, ref recoilSmoothDampVelocity, recoilMoveSettleTime);
		recoilAngle = Mathf.SmoothDamp (recoilAngle, 0, ref recoilRotSmoothDampVelocity, recoilRotationSettleTime);
		transform.localEulerAngles = transform.localEulerAngles + Vector3.right * -recoilAngle;

		if (!isReload && projectilesRemainingInMag == 0)
			Reload ();
	}

	void Shoot()
	{
		if (!isReload && Time.time > nextShotTime && projectilesRemainingInMag > 0) {

			if (fireMode == FireMode.Burst) {
				if (shotsRemainingInBurst == 0)
					return;
				shotsRemainingInBurst--;
			} else if (fireMode == FireMode.Single) {
				if (!triggerReleasedSinceLastShot)
					return;
			}

			for (int i = 0; i < projectileSpawn.Length; i++) {
				if (projectilesRemainingInMag == 0)
					break;
				projectilesRemainingInMag--;
				nextShotTime = Time.time + msBetweenShots / 1000;
				Projectile newProjectile = Instantiate (projectile, projectileSpawn[i].position, projectileSpawn[i].rotation) as Projectile;
				newProjectile.SetSpeed (muzzleVelocity);
			}

			Instantiate (shell, shellEjection.position, shellEjection.rotation);
			muzzleFlash.Activate ();
			transform.localPosition -= Vector3.forward * Random.Range(kickMinMax.x, kickMinMax.y);
			recoilAngle += Random.Range(recoilAngleMinMax.x, recoilAngleMinMax.y);
			recoilAngle = Mathf.Clamp (recoilAngle ,0, 30);

			AudioManager.instance.PlaySound (shootAudio, transform.position);
		}
	}

	public void Reload()
	{
		if (!isReload && projectilesRemainingInMag != projectilesPerMag) {
			StartCoroutine (AnimateReload ());	
			AudioManager.instance.PlaySound (reloadAudio, transform.position);

		}
	}

	IEnumerator AnimateReload()
	{
		isReload = true;
		yield return new WaitForSeconds (0.2f);

		float reloadSpeed = 1f / reloadTime;
		float percent = 0;
		Vector3 initialRot = transform.localEulerAngles;
		float maxReloadAngle = 30;

		while (percent < 1) {
			percent += Time.deltaTime * reloadSpeed;
			float interpolation = (-Mathf.Pow (percent, 2) + percent) * 4;
			float reloadAngle = Mathf.Lerp (0, maxReloadAngle, interpolation);
			transform.localEulerAngles = initialRot + Vector3.left * reloadAngle;

			yield return null;
		}
		isReload = false;
		projectilesRemainingInMag = projectilesPerMag;
	}

	public void Aim(Vector3 aimPoint)
	{
		if (!isReload) {
			transform.LookAt (aimPoint);
		}
	}
		
	public void OnTriggerHold()
	{
		Shoot ();
		triggerReleasedSinceLastShot = false;
	}

	public void OnTriggerRelease()
	{
		triggerReleasedSinceLastShot = true;
		shotsRemainingInBurst = burstCount;
	}
}