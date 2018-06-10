using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour {

	private Rigidbody myRigidbody;
	private Vector3 velocity;

	void Start()
	{
		myRigidbody = GetComponent<Rigidbody> ();
	}

	public void Move(Vector3 _velocity)
	{
		velocity = _velocity;
	}

	public void LookAt(Vector3 lookPoint)
	{
		Vector3 heightCorrectedPoint = new Vector3 (lookPoint.x, transform.position.y, lookPoint.z);
		transform.LookAt (heightCorrectedPoint);
	}

	void FixedUpdate()
	{
		myRigidbody.velocity = velocity;
//		myRigidbody.MovePosition (transform.position + velocity * Time.fixedDeltaTime);
	}
}
