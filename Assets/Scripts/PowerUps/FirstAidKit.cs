using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DontTrust.Characters.Main;

namespace DontTrust.Obstacles
{
	public class FirstAidKit : PowerUpClass {
		//Inherits from PowerUpClass

		[SerializeField] private sbyte m_RecoveryPoints=20;

		protected override void OnTriggerEnter(Collider other)
		{
			if (other.tag == "Player") {
				m_MainChar = other.GetComponent<MainCharacter>(); //Get the MainCharacter component (class) of the player's gameObject
				m_MainChar.ActivatePowerUp(m_PowerID, (float)m_RecoveryPoints);

				Destroy(this.gameObject); //Disappear power up
			}
		}
	}
}