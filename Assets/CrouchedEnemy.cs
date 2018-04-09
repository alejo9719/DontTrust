using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrouchedEnemy : MonoBehaviour
{
	[SerializeField] float m_AimDistance = 80f;
	public GameObject m_bulletPrefab;
	public float m_fireRate = 1f;

	private float m_nextFire = 0f;
	bool m_shoot = false;

	// Use this for initialization
	void Start () {
		
	}

	// Update is called once per frame
	void FixedUpdate () {
		RaycastHit hitInfo;

		Debug.DrawLine(transform.position + (Vector3.up * 4f), transform.position + (Vector3.up * 4f) + (Vector3.back * m_AimDistance)); //If in the editor, draw the raycast line

		if (Physics.Raycast(transform.position + (Vector3.up * 4f), Vector3.back, out hitInfo, m_AimDistance) && hitInfo.transform.tag == "Player")
		{
			m_shoot = true;
		}
		else
		{
			m_shoot = false;
		}

		if ( (Time.time > m_nextFire) && m_shoot ) //Fire time is up and the target is in shoot range
		{
			m_nextFire = Time.time + m_fireRate; //Set the next shot firing time
			GameObject bullet = Instantiate( m_bulletPrefab, new Vector3(transform.position.x-0.5f, transform.position.y+3.2f, transform.position.z-4.5f), 
				Quaternion.Euler(90f, 0f, 0f) ) as GameObject; //Instantiate new "prefab" object (bullet) on the specified position and rotation
			//bullet.name = "Bullet" + count++;
			bullet.GetComponent<Rigidbody>().AddForce(-Vector3.forward*55, ForceMode.Impulse); //Bullet is shot with an impulse force of magnitude 55
			Destroy(bullet, 2f); //Destroy the bullet 2 seconds later

		}
	}
}