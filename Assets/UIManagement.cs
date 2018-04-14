using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DontTrust.Characters.Main;

public class UIManagement : MonoBehaviour {

	[SerializeField] private Image m_HealthBar;
	private GameObject m_Player;
	private MainCharacter m_Character;

	// Use this for initialization
	void Start () {
		m_Player = GameObject.FindGameObjectWithTag ("Player"); //Find the main character's game object
		m_Character= m_Player.GetComponent<MainCharacter>();
	}
	
	// Update is called once per frame
	void Update () {
		UpdateBars ();
	}

	void UpdateBars()
	{
		m_HealthBar.fillAmount = (float)m_Character.m_Health/100f; //Map health to a number between 0 and 1 (fillAmount scale)
	}
}
