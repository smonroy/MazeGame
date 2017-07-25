using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public float velocity = 1f;
	public int cNode; // current node
	public int dNode; // destination node
	public int cDir; // direction of the player's face

	private Maze maze;

	// Use this for initialization
	void Start () {
		maze = GameObject.Find ("GameController").GetComponent<Maze> ();
		cNode = maze.initialNode;
		dNode = cNode;
		cDir = 0;
	}

	void FixedUpdate(){
		Vector3 pos = transform.position;
		// make the move
		if (dNode != cNode) {
			pos.x += Mathf.Clamp (maze.nodes[dNode].x - pos.x, -velocity, velocity);
			pos.y += Mathf.Clamp (maze.nodes[dNode].y - pos.y, -velocity, velocity);
			transform.position = pos;
		}	
		if (transform.position.x == maze.nodes [dNode].x && transform.position.y == maze.nodes [dNode].y) {
			cNode = dNode;
		}
	}

	void Update () {
		int nDir = -1;
		if (cNode == dNode) {
			if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
				nDir = 0;
			if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
				nDir = 1;
			if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
				nDir = 2;
			if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
				nDir = 3;
		}
		if (nDir != -1) {
			if (nDir != cDir) {
				float newAngle = 0;
				switch (nDir) {
				case 0:
					newAngle = 0;
					break;
				case 1:
					newAngle = 270;
					break;
				case 2:
					newAngle = 180;
					break;
				case 3:
					newAngle = 90;
					break;
				}
				transform.eulerAngles = new Vector3 (0, 0, newAngle);
				cDir = nDir;
			}

			if (maze.nodes [cNode].links [nDir] != -1 && maze.nodes [cNode].obstacles [nDir] == ' ') {
				dNode = maze.nodes [cNode].links [nDir];
			}
		}
	}
}
