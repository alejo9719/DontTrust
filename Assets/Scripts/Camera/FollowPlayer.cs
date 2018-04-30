using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DontTrust.Characters.Main;

public class FollowPlayer : MonoBehaviour {

	public GameObject m_Player;
	public Vector3 offset;
	public float ZoomOutSpeed;
	public float MaxZoomOut;
	public bool m_CameraRotation;
	public Vector3 m_RotationOffset;
	public float m_MaxHorizontalDistance=8f;
	public float m_MaxVerticalDistance=2.5f;
	private MainCharacter character;
	private Vector3 newPosition;
	private Vector3 cameraSpeed;
	public float cameraAcceleration=0.2f;

	void Start()
	{
		//offset = transform.position - m_Player.transform.position;
		//offset= new Vector3(3f, 1.2f, 0f);
		newPosition = m_Player.transform.position + offset;
	}

	void FixedUpdate() //Has to be FixedUpdate since it moves the camera depending on positions and velocities
	{
		character = m_Player.GetComponent<MainCharacter>();
		newPosition = transform.position;

		if (!character.m_IsGrounded && (newPosition.x < m_Player.transform.position.x + MaxZoomOut)) {
			newPosition.x += ZoomOutSpeed;
		} else if (newPosition.x > m_Player.transform.position.x + offset.x) {
			newPosition.x -= ZoomOutSpeed * 2;
		}
		else {
			newPosition.x = m_Player.transform.position.x + offset.x;
		}

		if (!m_CameraRotation) {
			newPosition.y = m_Player.transform.position.y + offset.y;
			newPosition.z = m_Player.transform.position.z + offset.z;
			transform.position = newPosition;
		} 
		else {
			cameraSpeed.z = cameraAcceleration/10 * (m_Player.transform.position.z + offset.z - (transform.position.z));
			cameraSpeed.y = cameraAcceleration/10 * (m_Player.transform.position.y + offset.y - (transform.position.y));

			newPosition.z += cameraSpeed.z;
			newPosition.y += cameraSpeed.y;

			if ((m_Player.transform.position.z + offset.z - (newPosition.z)) > m_MaxHorizontalDistance) {
				newPosition.z = m_Player.transform.position.z + offset.z - m_MaxHorizontalDistance;
			} else if ((m_Player.transform.position.z + offset.z - (newPosition.z)) < -m_MaxHorizontalDistance) {
				newPosition.z = m_Player.transform.position.z + offset.z + m_MaxHorizontalDistance;
			}

			if ((m_Player.transform.position.y + offset.y - (newPosition.y)) > m_MaxVerticalDistance) {
				newPosition.y = m_Player.transform.position.y + offset.y - m_MaxVerticalDistance;
			} else if ((m_Player.transform.position.y + offset.y - (newPosition.y)) < -m_MaxVerticalDistance) {
				newPosition.y = m_Player.transform.position.y + offset.y + m_MaxVerticalDistance;
			}

			transform.position = newPosition;
			transform.LookAt (m_Player.transform.position+m_RotationOffset);
		}
	}
}
