using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DontTrust.Obstacles
{
	public class ObstacleClass : MonoBehaviour { //Parent class for obstacles

		// Use this for initialization
		protected virtual void Start () {
			
		}
		
		// Update is called once per frame
		protected virtual void Update () {
			
		}

		public virtual void Deactivate () {
			//Deactivate object
			gameObject.SetActive(false);
		}

		public virtual void Respawn () {
			gameObject.SetActive(true);
			//Debug.Log ("Obstacle respawn");
		}
	}
}