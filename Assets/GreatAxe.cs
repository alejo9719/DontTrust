using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DontTrust.Characters.Main;

public class GreatAxe : MonoBehaviour {

	[SerializeField] private sbyte m_Damage=15; //MODIFICAR DE ACUERDO A DOC DE DISENIO
	private MainCharacter m_Character;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void OnCollisionEnter(Collision col) //This works on the parent even as it doesn't have a collider because it has the GameObject's Rigidbody.
	//If the parent doesn't have a Rigidbody or the child has it, the collision methods will only work for the object with the collider (child).
	{
		if (col.gameObject.CompareTag ("Player")) {
			m_Character = col.gameObject.GetComponent<MainCharacter>(); //Get the MainCharacter component (class) of the player's gameObject
			m_Character.m_Health -= m_Damage; //Decrease player's health when colliding with it
		}
	}
}
