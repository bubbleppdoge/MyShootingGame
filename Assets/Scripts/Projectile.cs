using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

	public LayerMask collisionMask;
	public Color trailColor;
	float speed = 10;
	float damage = 1;
	float lifeTime = 3f;
	float skinWidth = 0.1f;

	void Start()
	{
		Destroy (gameObject, lifeTime);

		Collider[] initialCollisions = Physics.OverlapSphere (transform.position, 0.1f, collisionMask);
		if (initialCollisions.Length > 0) {
			OnHitObject (initialCollisions [0], transform.position);
		}

		GetComponent<TrailRenderer> ().material.SetColor ("_TintColor", trailColor);
	}

	public void SetSpeed(float newSpeed)
	{
		speed = newSpeed;
	}

	void Update()
	{
		float moveDistance = speed * Time.deltaTime;
		CheckCollision (moveDistance);
		transform.Translate (Vector3.forward * moveDistance);
	}

	void CheckCollision(float moveDistance)
	{
		Ray ray = new Ray (transform.position, transform.forward);
		RaycastHit hit;

		if (Physics.Raycast (ray, out hit, moveDistance + skinWidth, collisionMask, QueryTriggerInteraction.Collide)) {
			OnHitObject (hit.collider, hit.point);
		}
	}

	void OnHitObject(Collider c, Vector3 hitPoint)
	{
		IDamageable damageableObject = c.GetComponent<IDamageable> ();
		if (damageableObject != null) {
			damageableObject.TakeHit (damage, hitPoint, transform.forward);
		}
		Destroy (gameObject);
	}
}
