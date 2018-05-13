using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DontTrust.Characters.Main;
using DontTrust.Obstacles;

namespace DontTrust.Obstacles
{
	public class GreatAxe : ObstacleClass {

		[SerializeField] private sbyte m_Damage = 20; //MODIFICAR DE ACUERDO A DOC DE DISENIO
		[SerializeField] private AudioClip m_CutSound;
		private MainCharacter m_MCharacter;

		// Use this for initialization
		new void Start () {
			
		}
		
		// Update is called once per frame
		new void Update () {
			
		}

		public void OnCollisionEnter(Collision col) //This works on the parent even if it doesn't have a collider because it has the GameObject's Rigidbody.
		//If the parent doesn't have a Rigidbody or the child has it, the collision methods will only work for the object with the collider (child).
		{
			if (col.gameObject.CompareTag ("Player")) {
				m_MCharacter = col.gameObject.GetComponent<MainCharacter>(); //Get the MainCharacter component (class) of the player's gameObject
				m_MCharacter.TakeDamage(m_Damage); //Decrease player's health when colliding with it
				GetComponent<AudioSource>().PlayOneShot (m_CutSound); // Play blade slash sound
			}
		}
	}
}