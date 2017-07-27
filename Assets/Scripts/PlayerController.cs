﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public float velocity = 1f;
	public int bombIncrement;
	public int bulletIncrement;

	private int cNode; // current node
	private int dNode; // destination node
	private int checkPointNode; // checkPoint;
	private int cDir; // direction of the player's face
	private int nBombs;
	private int nBullets;
	private int nKeys;
	private int nGoldenKeys;
	private Maze maze;
	private Animator anim;

	// Use this for initialization
	void Start () {
		maze = GameObject.Find ("GameController").GetComponent<Maze> ();
		anim = GetComponent<Animator> ();
		cNode = maze.initialNode;
		dNode = cNode;
		checkPointNode = cNode;
		cDir = 0;
		nBombs = 0;
		nBullets = 0;
		nKeys = 0;
		nGoldenKeys = 0;
	}

	private void OnTriggerEnter2D(Collider2D other){
		if (other.tag == "Bomb") {
			nBombs += bombIncrement;
			Destroy (other.gameObject);
			checkPointNode = dNode;
		}
		if (other.tag == "Ammo") {
			nBullets += bulletIncrement;
			Destroy (other.gameObject);
			checkPointNode = dNode;
		}
		if (other.tag == "Key") {
			nKeys++;
			Destroy (other.gameObject);
			checkPointNode = dNode;
		}
		if (other.tag == "GoldenKey") {
			nGoldenKeys++;
			Destroy (other.gameObject);
			checkPointNode = dNode;
		}
		if (other.tag == "Arrow" || other.tag == "Enemy") {
			Vector3 pos = transform.position;
			pos.x = maze.nodes [checkPointNode].x;
			pos.y = maze.nodes [checkPointNode].y;
			dNode = checkPointNode;
			cNode = checkPointNode;
			transform.position = pos;
			checkPointNode = dNode;
		}
	}

	private void FixedUpdate(){
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

	private void Update () {
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
			if (nDir == -1) {
				anim.SetBool ("PlayerIsWalking", false);
			}
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
				anim.SetBool ("PlayerIsWalking", true);
			}
		}
	}
}
