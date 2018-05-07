using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollingStone : MonoBehaviour {

	[SerializeField] private float m_InitialSpeed = 30;
	private Rigidbody m_Rigidbody;

	// Use this for initialization
	void Start () {
		m_Rigidbody = GetComponent<Rigidbody> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player") {
			m_Rigidbody.useGravity = true;
			m_Rigidbody.AddForce (Vector3.down*m_InitialSpeed, ForceMode.VelocityChange);
		}
	}
}
