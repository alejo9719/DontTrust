using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnCollisionEnter (Collision col)
	{
		if (!col.gameObject.CompareTag ("Enemy")) {
			Destroy(this.gameObject, 0.2f);
		}
	}
}
