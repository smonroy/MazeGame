using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
	// Public variables
	public float velocity;
	public float verticalLimit;
	public float horizontalLimit;

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
			Vector3 pos = transform.position;
			if (Mathf.Abs(player.transform.position.y-this.transform.position.y) > verticalLimit) {
				pos.y += Mathf.Clamp (player.transform.position.y - pos.y, -velocity, velocity);
			}	
			if (Mathf.Abs(player.transform.position.y-this.transform.position.y) > 2f*verticalLimit) {
				pos.y += Mathf.Clamp (player.transform.position.y - pos.y, -10f*velocity, 10f*velocity);
			}	
			if (Mathf.Abs(player.transform.position.x-this.transform.position.x) > horizontalLimit) {
				pos.x += Mathf.Clamp (player.transform.position.x - pos.x, -velocity, velocity);
			}	
			if (Mathf.Abs(player.transform.position.x-this.transform.position.x) > 2f*horizontalLimit) {
				pos.x += Mathf.Clamp (player.transform.position.x - pos.x, -2f*velocity, 2f*velocity);
			}	
			transform.position = pos;
		}
	}
}
