using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrouchedEnemy : MonoBehaviour {

	public GameObject prefab;
	public float fireRate;
	private float nextFire = 0f;
	// Use this for initialization
	void Start () {
		
	}

	// Update is called once per frame
	void FixedUpdate () {
		if (Time.time > nextFire)
		{
			nextFire = Time.time + fireRate;
			GameObject bullet = Instantiate( prefab, new Vector3(transform.position.x-0.5f, transform.position.y+3.2f, transform.position.z-6f), 
				Quaternion.Euler(90f, 0f, 0f) ) as GameObject; //Instantiate new "prefab" object (bullet) on the specified position and rotation
			//cube.name = "Cube" + count++;
			bullet.GetComponent<Rigidbody>().AddForce(-Vector3.forward*55, ForceMode.Impulse);
			Destroy(bullet, 2f); //Destroy the bullet 2 seconds later

		}
	}
}