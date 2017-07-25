using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
	// Public variables
	public float velocity;
	public float topLimit;
	public float buttonLimit;

	private GameObject player;

	// Use this for initialization
	void Start () {
		}

	void FixedUpdate() {
		if (player == null) {
			GameObject p = GameObject.FindGameObjectWithTag("Player");
			if (p != null) {
				player = p;
			}
		} else {
			if (player.transform.position.y > this.transform.position.y + topLimit || 
				player.transform.position.y < this.transform.position.y - buttonLimit) {

				Vector3 pos = transform.position;
				// make the move
				pos.y += Mathf.Clamp (player.transform.position.y - pos.y, -velocity, velocity);
				transform.position = pos;
			}	
		}
	}
}
