using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DontTrust.GameManager
{
	public class Mechanics : MonoBehaviour {

		[SerializeField] private GameObject m_InitialCheckpoint;
		private GameObject m_LastCheckpoint;
		private GameObject m_Player;

		// Use this for initialization
		void Start () {
			m_LastCheckpoint = m_InitialCheckpoint;
			m_Player = GameObject.FindWithTag ("Player");
		}
		
		// Update is called once per frame
		void Update () {
			
		}

		public void SetCheckpoint (GameObject checkpoint)
		{
			m_LastCheckpoint = checkpoint;
		}

		public void LoadCheckpoint()
		{
			m_Player.transform.position = m_LastCheckpoint.GetComponent<Checkpoint>().getPosition();
		}
	}
}