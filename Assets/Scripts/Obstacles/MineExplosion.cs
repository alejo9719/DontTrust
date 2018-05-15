using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DontTrust.Characters.Main;
using DontTrust.GameManager;
using DontTrust.Obstacles;

namespace DontTrust.Obstacles
{
	public class MineExplosion : ObstacleClass {

		[SerializeField] private sbyte m_Damage = 10;
		[SerializeField] private float m_PushMultiplier = 30f;

		public AudioClip m_Explosion; // Explosion sound

		private GameObject m_Parent; // Land Mine

		private MainCharacter m_MCharacter; // Game Character
		private Rigidbody m_Rigidbody; // Game Character's rigid body
		private GameObject m_GameManager;
		private Mechanics m_ManagerMechanics;

		// Use this for initialization
		new void Start () {
			GetComponent<ParticleSystem>().Stop (); // Stop the animation so it doesn't play until the soldier collides with it
			m_GameManager = GameObject.FindWithTag ("GameController");
			m_ManagerMechanics = m_GameManager.GetComponent<Mechanics> ();
			m_Parent = transform.parent.gameObject;

			m_ManagerMechanics.AddRespawnableObstacle (gameObject); //Adds the parent object to the list of gameObjects to be reactivated on respawn
		}
		
		// Update is called once per frame
		void OnTriggerEnter (Collider other) {
			if (other.tag == "Player") {
				GetComponent<ParticleSystem>().Play (); // Start the animation after the collision
				GetComponent<AudioSource>().PlayOneShot (m_Explosion); // Play sound for the explosion

				m_MCharacter = other.gameObject.GetComponent<MainCharacter>(); //Get the MainCharacter component (class) of the player's gameObject
				m_MCharacter.TakeDamage(m_Damage); //Decrease player's health when colliding with it
				Vector3 PushForce = ((other.transform.position - transform.position) + Vector3.up) * m_PushMultiplier; //Apply knockback force to player in the opposite direction (vector substraction)
																														//add an upwards force to give it a better effect.
				m_Rigidbody = other.gameObject.GetComponent<Rigidbody>(); //Get the Rigid Body of the MainCharacter component (class) of the player's gameObject
				m_Rigidbody.AddForce (PushForce, ForceMode.Impulse);

				ParticleSystem particles = GetComponent<ParticleSystem> (); //Get explosion particle system

				InvokeRepeating("Deactivate", particles.main.startLifetime.constant, 0); //Delays the mine gameobject deactivation until the particle system finishes playing.
				//Destroy (GetComponent<ParticleSystem>()); // End explosion animation
				//Mine.GetComponent<MeshRenderer>().enabled = false; //Hide Mesh after collision for the mine to disappear
			}
		}

		public override void Deactivate(){ //Deactivate (disappear) mine
			//Deactivate parent gameobject
			m_Parent.SetActive(false);
		}

		public override void Respawn () { //Restart the mine state
			m_Parent.SetActive(true);
			//Debug.Log ("Mine respawn");
		}
	}
}