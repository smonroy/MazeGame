using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

	public float velocity;

	private Maze maze;
	private int hNode; // home node
	private int pNode; // pivote node;
	private int cNode; // current node;
	private int dNode; // destination node;
	private int cDir; // current direction;

	// Use this for initialization
	void Start () {
		maze = GameObject.Find ("GameController").GetComponent<Maze> ();
		hNode = maze.nodes.FindIndex(x => x.x == this.transform.position.x && x.y == this.transform.position.y);
		cNode = hNode;
		dNode = hNode;
		pNode = hNode;
		cDir = -1;
	}
		
	// Update is called once per frame
	void FixedUpdate () {
		if (dNode != cNode) {
			Vector3 pos = transform.position;
			pos.x += Mathf.Clamp (maze.nodes [dNode].x - pos.x, -velocity, velocity);
			pos.y += Mathf.Clamp (maze.nodes [dNode].y - pos.y, -velocity, velocity);
			transform.position = pos;
		}
		if (transform.position.x == maze.nodes [dNode].x && transform.position.y == maze.nodes [dNode].y) {
			int nDir = 0;
			cNode = dNode;
			if (cNode != hNode && cNode != pNode) {
				dNode = pNode;
				nDir = cDir + 2;
				if (nDir > 3) {
					nDir -= 4;
				}
			} else {
				float topRand = 0f;
				for (int i = 0; i < 4; i++) {
					if (maze.nodes [cNode].links [i] != -1 && maze.nodes[cNode].obstacles[i] == ' ') {
						float cRand = Random.Range (1, 10);
						if (cRand > topRand) {
							topRand = cRand;
							nDir = i;
							dNode = maze.nodes [cNode].links [nDir];
						}
					}
				}
				if (cNode == hNode) {
					pNode = dNode; // change the pivot node
				}
			}
			if (nDir != cDir) {
				float newAngle = 0;
				switch (nDir) {
				case 0:
					newAngle = 180;
					break;
				case 1:
					newAngle = 90;
					break;
				case 2:
					newAngle = 0;
					break;
				case 3:
					newAngle = 270;
					break;
				}
				this.transform.eulerAngles = new Vector3 (0, 0, newAngle);
				cDir = nDir;
			}
		}
	}
}
