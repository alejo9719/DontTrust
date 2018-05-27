using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DontTrust.Obstacles;
using DontTrust.Characters.Main;

namespace DontTrust.GameManager
{
	public class Mechanics : MonoBehaviour {

		[SerializeField] private GameObject m_InitialCheckpoint;
		[SerializeField] private int m_LevelTime = 240;

		private GameObject m_LastCheckpoint;
		private GameObject m_Player;
		private MainCharacter m_Character;
		private List<GameObject> m_RespawnableObstacles = new List<GameObject>();
		private int m_OrigLevelTime;

		// Use this for initialization
		void Start () {
			m_LastCheckpoint = m_InitialCheckpoint;
			m_Player = GameObject.FindWithTag ("Player");
			m_Character = m_Player.GetComponent<MainCharacter> ();

			m_OrigLevelTime = m_LevelTime;
			InvokeRepeating ("TimeUpdate", 1f, 1f); //Call time update function after 1 second and repeat every 1 second
		}
		
		// Update is called once per frame
		void Update () {
			
		}

		private void TimeUpdate() //Update level time
		{
			m_LevelTime -= 1; //Decrease time by 1 second
			if (m_LevelTime <= 0) { //Time's up
				m_LevelTime = 0;
				//MOSTRAR TEXTO POR UN TIEMPO CORTO
				m_LevelTime = m_OrigLevelTime;
				m_Character.Die ();
			}
		}

		public int GetTime()
		{
			return m_LevelTime;
		}

		public void SetCheckpoint (GameObject checkpoint)
		{
			m_LastCheckpoint = checkpoint;
		}

		public void LoadCheckpoint()
		{
			m_Player.transform.position = m_LastCheckpoint.GetComponent<Checkpoint>().getPosition();

			foreach (GameObject obstacle in m_RespawnableObstacles) { //Reactivate obstacles
				//Debug.Log(obstacle.name);
				obstacle.GetComponent<ObstacleClass>().Respawn();
			}
		}

		public void RestartLevel() //DEBE SER RestartGame Y REINICIAR TODO EL JUEGO SI HAY GAMEOVER
		{
			m_LastCheckpoint = m_InitialCheckpoint;
			LoadCheckpoint ();
			//REAPARECER POWERUPS
		}

		public void AddRespawnableObstacle(GameObject obstacle)
		{
			m_RespawnableObstacles.Add (obstacle);
		}
	}
}