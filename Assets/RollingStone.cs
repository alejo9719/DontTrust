using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DontTrust.Characters.Main;
using DontTrust.GameManager;

public class RollingStone : MonoBehaviour {

	[SerializeField] private float m_InitialSpeed = 30;
	[SerializeField] private sbyte m_DamageMultiplier = 3; //MODIFICAR DE ACUERDO A DOC DE DISENIO
	[SerializeField] private sbyte m_MaxDamage = 50; //MODIFICAR DE ACUERDO A DOC DE DISENIO
	[SerializeField] private AudioClip m_RollSound;
	private MainCharacter m_MCharacter;
	private Rigidbody m_Rigidbody;
	private GameObject m_GameManager;
	private Mechanics m_ManagerMechanics;
	private bool m_MakeDamage;

	private Vector3 m_InitialPosition;
	private Quaternion m_InitialRotation;

	// Use this for initialization
	void Start () {
		m_Rigidbody = GetComponent<Rigidbody> ();
		m_GameManager = GameObject.FindWithTag ("GameController");
		m_ManagerMechanics = m_GameManager.GetComponent<Mechanics> ();
		m_MakeDamage = true;
		m_InitialPosition = m_Rigidbody.transform.position;
		m_InitialRotation = m_Rigidbody.transform.rotation;

		m_ManagerMechanics.AddRespawnableObstacle (gameObject); //Adds the object to the list of gameObjects to be reactivated on respawn
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

	public void OnCollisionEnter(Collision col)
	{
		if (col.gameObject.CompareTag ("Player") && m_MakeDamage) {
			m_MCharacter = col.gameObject.GetComponent<MainCharacter>(); //Get the MainCharacter component (class) of the player's gameObject
			sbyte damage = (sbyte)(int)Mathf.Abs(m_Rigidbody.velocity.magnitude*m_DamageMultiplier); //Damage will be dealt according to velocity of the stone
			if (damage >= m_MaxDamage || damage <0)
				damage = m_MaxDamage;
			m_MCharacter.TakeDamage(damage); //Decrease player's health when both collide
			m_MakeDamage = false; //Don't continue damaging the character
			//Deactivate object
			InvokeRepeating("Deactivate", 2f, 0); //Delays the gameobject deactivation.
			//GetComponent<AudioSource>().PlayOneShot (m_CutSound); // Play blade slash sound
		}
	}

	void Deactivate(){
		//Reset original state
		m_Rigidbody.useGravity = false;
		m_MakeDamage = true;
		m_Rigidbody.transform.position = m_InitialPosition;
		m_Rigidbody.transform.rotation = m_InitialRotation;
		m_Rigidbody.velocity = Vector3.zero;
		m_Rigidbody.angularVelocity = Vector3.zero;
		//Deactivate object
		gameObject.SetActive(false);
	}
}
