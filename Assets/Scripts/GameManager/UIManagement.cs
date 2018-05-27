﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DontTrust.Characters.Main;
using TMPro; //TextMeshPro

namespace DontTrust.GameManager
{
	public class UIManagement : MonoBehaviour {

		[SerializeField] private Image m_HealthBar;
		[SerializeField] private TMP_Text m_Lifes;
		[SerializeField] private TMP_Text m_TimeCounter;
		private GameObject m_Player;
		private MainCharacter m_Character;
		private Mechanics m_Mechanics;

		// Use this for initialization
		void Start () {
			m_Player = GameObject.FindGameObjectWithTag ("Player"); //Find the main character's game object
			m_Character = m_Player.GetComponent<MainCharacter>();
			m_Mechanics = GetComponent<Mechanics> ();
		}
		
		// Update is called once per frame
		void Update () {
			UpdateBars();
			UpdateTexts();
		}

		void UpdateBars()
		{
			m_HealthBar.fillAmount = (float)m_Character.GetHealth()/100f; //Map health to a number between 0 and 1 (fillAmount scale)
		}

		void UpdateTexts()
		{
			m_Lifes.SetText(m_Character.GetLifes().ToString()); //Update lifes number
			m_TimeCounter.SetText (m_Mechanics.GetTime().ToString()); //Update time counter
		}
	}
}