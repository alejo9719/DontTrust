using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DontTrust.Characters.Main;

public class EnergyDrink : MonoBehaviour {
	[SerializeField] private sbyte m_PowerID=1;

	private MainCharacter m_MainChar;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player") {
			m_MainChar = other.GetComponent<MainCharacter>(); //Get the MainCharacter component (class) of the player's gameObject
			m_MainChar.ActivatePowerUp(m_PowerID);

			Destroy(this.gameObject); //Disappear energy drink
		}
	}
}
