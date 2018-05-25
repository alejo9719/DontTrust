using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DontTrust.Characters.Main;
using DontTrust.GameManager;

namespace DontTrust.Obstacles
{
	public class RollingStone : ObstacleClass {

		[SerializeField] private float m_InitialSpeed = 30;
		[SerializeField] private float m_DamageMultiplier = 1.5f; //MODIFICAR DE ACUERDO A DOC DE DISENIO
		[SerializeField] private sbyte m_MaxDamage = 50; //MODIFICAR DE ACUERDO A DOC DE DISENIO
		[SerializeField] private AudioClip m_RollSound;
		[Range(0f, 1f)][SerializeField] float m_SoundVolume = 0.7f;
		private MainCharacter m_MCharacter; //PONER EN CLASE PADRE
		private Rigidbody m_Rigidbody;
		private GameObject m_GameManager;
		private Mechanics m_ManagerMechanics;
		private bool m_MakeDamage;
		private bool m_IsGrounded;
		private AudioSource m_Audio;

		private Vector3 m_InitialPosition;
		private Quaternion m_InitialRotation;

		// Use this for initialization
		protected override void Start () {
			m_Rigidbody = GetComponent<Rigidbody> ();
			m_GameManager = GameObject.FindWithTag ("GameController");
			m_ManagerMechanics = m_GameManager.GetComponent<Mechanics> ();
			m_MakeDamage = true;
			m_InitialPosition = m_Rigidbody.transform.position;
			m_InitialRotation = m_Rigidbody.transform.rotation;
			m_Audio = GetComponent<AudioSource> ();
			m_Audio.clip = m_RollSound;
			m_Audio.loop = true;
			m_Audio.Play ();

			m_ManagerMechanics.AddRespawnableObstacle (gameObject); //Adds the object to the list of gameObjects to be reactivated on respawn
		}
		
		// Update is called once per frame
		protected override void Update () {
			
		}

		void FixedUpdate() {
			CheckGroundStatus();

			m_Audio.volume = m_Rigidbody.velocity.magnitude * m_SoundVolume/50f;

			//GetComponent<AudioSource>().Play (m_RollSound, m_Rigidbody.velocity.magnitude/100f); // Play rolling sound
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
				sbyte damage = (sbyte)(int)(m_Rigidbody.velocity.magnitude*m_DamageMultiplier); //Damage will be dealt according to velocity of the stone //DEBE SER RELATIVO A LA DIRECCION RESPECTO A JUGADOR
				//Debug.Log(m_Rigidbody.velocity.magnitude);
				if (damage >= m_MaxDamage || damage <0) //The damage cannot be greater than the maximum or negative (sometimes caused by type overflow)
					damage = m_MaxDamage;
				m_MCharacter.TakeDamage(damage); //Decrease player's health when both collide
				m_MakeDamage = false; //Don't continue damaging the character
			}
		}

		public override void Respawn () { //Restart the stone state
			//Reset original state
			m_Rigidbody.useGravity = false;
			m_MakeDamage = true;
			m_Rigidbody.transform.position = m_InitialPosition;
			m_Rigidbody.transform.rotation = m_InitialRotation;
			m_Rigidbody.velocity = Vector3.zero;
			m_Rigidbody.angularVelocity = Vector3.zero;
			gameObject.SetActive(true);
			//Debug.Log ("Stone respawn");
		}

		void CheckGroundStatus() //Checks if the character is on ground.
		{
			RaycastHit hitInfo;
			#if UNITY_EDITOR
			// Helper to visualise the ground check rays in the scene view
			Debug.DrawLine(transform.position, transform.position + (Vector3.down * 12)); //If in the editor, draw the raycast line
			#endif

			// 0.1f is a small offset to start the ray from inside the character
			// it is also good to note that the transform position in the sample assets is at the base of the character
			if (Physics.Raycast(transform.position, Vector3.down, out hitInfo, 12)) //Ray from the center of the character
			{
				if(!m_IsGrounded) //Just landed
					m_Audio.Play(); //Start playing rolling audio
				
				m_IsGrounded = true;
			}
			else
			{
				if(!m_IsGrounded) //Stops touching the floor
					m_Audio.Stop ();
				
				m_IsGrounded = false;
			}
		}
	}
}