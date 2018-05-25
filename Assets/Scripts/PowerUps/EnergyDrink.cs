using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DontTrust.Characters.Main;

namespace DontTrust.Obstacles
{
	public class EnergyDrink : PowerUpClass {
		//Inherits from PowerUpClass

		[SerializeField] private float m_Duration=2f;

		protected override void OnTriggerEnter(Collider other)
		{
			if (other.tag == "Player") {
				m_MainChar = other.GetComponent<MainCharacter>(); //Get the MainCharacter component (class) of the player's gameObject
				m_MainChar.ActivatePowerUp(m_PowerID, m_Duration);

				Destroy(this.gameObject); //Disappear energy drink
			}
		}
	}
}