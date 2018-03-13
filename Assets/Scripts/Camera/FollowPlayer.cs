using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DontTrust.Characters.Main;

public class FollowPlayer : MonoBehaviour {

	public GameObject Player;
	public Vector3 offset;
	public float ZoomOutSpeed;
	public float MaxZoomOut;
	private MainCharacter character;
	private Vector3 newPosition;

	void Start()
	{
		//offset = transform.position - Player.transform.position;
		//offset= new Vector3(3f, 1.2f, 0f);
		newPosition = Player.transform.position + offset;
	}

	void LateUpdate()
	{
		character= Player.GetComponent<MainCharacter>();

		if (!character.m_IsGrounded && (newPosition.x < Player.transform.position.x + MaxZoomOut) ) {
			newPosition.x += ZoomOutSpeed;
		}
		else if (newPosition.x >= Player.transform.position.x + offset.x) {
			newPosition.x -= ZoomOutSpeed * 2;
		}

		newPosition.y += Player.transform.position.y + offset.y - newPosition.y;
		newPosition.z += Player.transform.position.z + offset.z - newPosition.z;
		transform.position = newPosition;
	}
}
