using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DontTrust.GameManager;

public class Checkpoint : MonoBehaviour {

	private Vector3 m_Position;
	private GameObject m_GameManager;
	private Mechanics m_ManagerMechanics;

	// Use this for initialization
	void Start () {
		m_GameManager = GameObject.FindWithTag ("GameController");
		m_ManagerMechanics = m_GameManager.GetComponent<Mechanics> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider col)
	{
		if (col.gameObject.CompareTag ("Player")) {
			m_Position = col.gameObject.transform.position;
			m_ManagerMechanics.SetCheckpoint(gameObject);
		}
	}

	public Vector3 getPosition()
	{
		return m_Position;
	}
}
