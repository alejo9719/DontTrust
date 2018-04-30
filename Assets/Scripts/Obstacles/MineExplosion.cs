using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DontTrust.Characters.Main;

public class MineExplosion : MonoBehaviour {

	[SerializeField] private sbyte m_Damage = 10;

	public AudioClip Explosion; // Explosion sound

	private GameObject Mine; // Land Mine

	private MainCharacter m_MCharacter; // Game Character
	private Rigidbody m_Rigidbody; // Game Character's rigid body

	// Use this for initialization
	void Start () {
		GetComponent<ParticleSystem>().Stop (); // Stop the animation so it doesn't play until the soldier collides with it

	}
	
	// Update is called once per frame
	void OnTriggerEnter (Collider other) {
		if (other.tag == "Player") {
			GetComponent<ParticleSystem>().Play (); // Start the animation after the collision
			GetComponent<AudioSource>().PlayOneShot (Explosion); // Play sound for the explosion
			//Mine = GameObject.Find ("Mine"); // Find Game Object Mine
			Mine = transform.parent.gameObject;
			m_MCharacter = other.gameObject.GetComponent<MainCharacter>(); //Get the MainCharacter component (class) of the player's gameObject
			m_MCharacter.TakeDamage(m_Damage); //Decrease player's health when colliding with it
			Vector3 PushForce = new Vector3(0, 12f*2, -70);
			m_Rigidbody = other.gameObject.GetComponent<Rigidbody>(); //Get the Rigid Body of the MainCharacter component (class) of the player's gameObject
			m_Rigidbody.AddForce (PushForce, ForceMode.Impulse);

			ParticleSystem particles = GetComponent<ParticleSystem> ();
			float particlesDuration = particles.startLifetime + particles.duration;
			//Destroy (GetComponent<ParticleSystem>()); // End explosion animation
			//gameObject.SetActive(false); // End explosion animation
			//InvokeRepeating("deactivateMine", particles.duration, 0); //Delays the turning off of the wall collision flag in order to give the player some time to perform the wall jump.
			Mine.GetComponent<MeshRenderer>().enabled = false; //Hide Mesh after collision for the mine to disappear
			//Mine.SetActive(false);
		}
	}

	void deactivateMine(){
		Mine.SetActive(false);
	}
}
