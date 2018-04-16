using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DontTrust.Characters.Main;

public class BulletController : MonoBehaviour
{
	[SerializeField] private sbyte m_Damage = 10; //MODIFICAR DE ACUERDO A DOC DE DISENIO

	private MainCharacter m_MCharacter;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void OnCollisionEnter (Collision col)
	{
		if (!col.gameObject.CompareTag ("Enemy")) {
			Destroy(this.gameObject, 0.2f);
		}

		if (col.gameObject.CompareTag ("Player")) {
			m_MCharacter = col.gameObject.GetComponent<MainCharacter>(); //Get the MainCharacter component (class) of the player's gameObject
			m_MCharacter.TakeDamage(m_Damage); //Decrease player's health when colliding with it
		}
	}
}
