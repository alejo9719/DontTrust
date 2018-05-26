using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DontTrust.Characters.Main;

namespace DontTrust.Obstacles
{
	public class PowerUpClass : MonoBehaviour {
		[SerializeField] protected sbyte m_PowerID;

		protected MainCharacter m_MainChar;

		// Use this for initialization
		protected virtual void Start () {

		}

		// Update is called once per frame
		protected virtual void Update () {

		}

		protected virtual void OnTriggerEnter(Collider other)
		{
			if (other.tag == "Player") {
				m_MainChar = other.GetComponent<MainCharacter>(); //Get the MainCharacter component (class) of the player's gameObject
				m_MainChar.ActivatePowerUp(m_PowerID, 0f);

				Destroy(this.gameObject); //Disappear power up
			}
		}
	}
}
