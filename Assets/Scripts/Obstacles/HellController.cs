using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DontTrust.Characters.Main;

namespace DontTrust.Obstacles
{
	public class HellController : ObstacleClass {

		private MainCharacter m_MCharacter;

		// Use this for initialization
		void Start () {
			
		}
		
		// Update is called once per frame
		void Update () {
			
		}

		void OnCollisionEnter(Collision colInfo)
		{
			if (colInfo.gameObject.CompareTag ("Player")) {
				m_MCharacter = colInfo.gameObject.GetComponent<MainCharacter> (); //Get main character script
				m_MCharacter.TakeDamage (6); //PROVISIONAL (REPRODUCIR SONIDO)
			}
		}

		void OnCollisionStay(Collision colInfo)
		{
			if (colInfo.gameObject.CompareTag ("Player")) {
				m_MCharacter = colInfo.gameObject.GetComponent<MainCharacter> ();
				m_MCharacter.TakeDamage (1); //PROVISIONAL (REPRODUCIR SONIDO)
			}
		}
	}

}