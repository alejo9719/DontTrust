using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DontTrust.Characters.Main;

namespace DontTrust.Obstacles
{
	public class LavaController : ObstacleClass {

		private CapsuleCollider m_PlayerCollider;

		// Use this for initialization
		protected override void Start () {
			
		}
		
		// Update is called once per frame
		protected override void Update () {
			
		}

		void OnTriggerEnter(Collider other)
		{
			if (other.tag == "Player") { //Player entered the lava
				other.attachedRigidbody.AddForce(Vector3.down*5, ForceMode.Impulse); //Accelerate the player down
				m_PlayerCollider = other.GetComponent<CapsuleCollider>(); //Get the player's capsule collider
				m_PlayerCollider.enabled = false; //Disable collider
				StartCoroutine(ReEnableCollider()); //Collider must be re-enabled when player has passed the lava
			}
		}

		IEnumerator ReEnableCollider()
		{
			yield return new WaitForSeconds(0.8f); //Wait for player to pass through the lava to re-enable collider
			m_PlayerCollider.enabled = true; //Re-enable collider
		}
	}

}