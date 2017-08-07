﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maze : MonoBehaviour {

	// public variables
	public GameObject wallsGroup;
	public GameObject fogGroup;
	public GameObject objectGroup;
	public GameObject doneGroup;
	public GameObject wallBlock;
	public GameObject fogObject;
	public GameObject[] mazeObjects;
    public AudioClip[] sounds; // 0 - collect, 1 - footsteps
	public float centerX = 10;
	public float centerY = 10;
	public float squareSize;
	public float wallWidth; // factor of size
	public int initialNode;
	public List<Node> nodes = new List<Node>();

	// private variables
	private int[,] map;
	private float initialX;
	private float initialY;
	private float width;
	private float height;
	private float sizeX;
	private float sizeY;
	private List<Wall> walls = new List<Wall>();
	private int doneNodes;

	// Use this for initialization
	void Start () {
		InitializeLevel(1);
		width = squareSize * map.GetLength (1);
		height = squareSize * map.GetLength (0);
		initialX = centerX - (width / 2);
		initialY = centerY - (height / 2);
		sizeX = width / (map.GetLength(1));
		sizeY = height / (map.GetLength(0));
		doneNodes = 0;

		// logic construction
		AddWalls();
		AddNodes();
		ConnectNodes();

		// phisycal construction
		CreateWalls();
		CreateFog();
		CreateObject();
	}


	private void CreateWalls(){
		foreach (Wall wall in walls) {
			GameObject newWall = Instantiate(wallBlock, new Vector3 (wall.x, wall.y, 0f), Quaternion.identity, wallsGroup.transform);
			newWall.transform.localScale = new Vector3(wall.width, wall.height, 0.1f);
		}
	}

	private void CreateFog() {
		for (int i = 0; i < 2500; i++) {
			Instantiate(fogObject, new Vector3 (Random.Range(initialX, initialX + width), Random.Range(initialY, initialY + height), -1f), Quaternion.identity, fogGroup.transform);
		}
	}

	private void CreateObject(){
		foreach (Node node in nodes) {
			switch (node.initialObject) {
				case 'I':
					Instantiate(mazeObjects[1], new Vector3 (node.x, node.y, -1f), Quaternion.identity);
					break;
				case 'K':
					Instantiate(mazeObjects[2], new Vector3 (node.x, node.y, 0f), Quaternion.identity, objectGroup.transform);
					break;
				case 'B':
					Instantiate(mazeObjects[3], new Vector3 (node.x, node.y, 0f), Quaternion.identity, objectGroup.transform);
					break;
				case 'A':
					Instantiate(mazeObjects[4], new Vector3 (node.x, node.y, 0f), Quaternion.identity, objectGroup.transform);
					break;
				case 'G':
					Instantiate(mazeObjects[5], new Vector3 (node.x, node.y, 0f), Quaternion.identity, objectGroup.transform);
					break;
				case 'E':
					Instantiate(mazeObjects[9], new Vector3 (node.x, node.y, 0f), Quaternion.identity, objectGroup.transform);
					break;
				case 'T':
					int angle = 0;
					for (int i = 0; i < 4; i++)
					{
						if (node.links [i] != -1) {
							angle = 270 - (i * 90);
							break;
						}
					}
					Quaternion rot = Quaternion.AngleAxis(angle, new Vector3(0,0,1));
					Instantiate(mazeObjects[10], new Vector3 (node.x, node.y, 0f), rot, objectGroup.transform); 
					break;
			}
			for (int dir = 0; dir < 2; dir++) {

				float widthFactor;
				if (node.obstacles [dir] == 'W') {
					widthFactor = 1f;
				} else {
					widthFactor = 0.4f;
				}
				GameObject newObstacle;
				float oWidth, oHeight, oX, oY;
				if (dir == 0) {
					oWidth = squareSize * 2f;
					oHeight = sizeX * wallWidth * widthFactor;
					oX = node.x;
					oY = node.y + squareSize;
				} else {
					oWidth = sizeY * wallWidth * widthFactor;
					oHeight = squareSize * 2f;
					oX = node.x + squareSize;
					oY = node.y;
				}
					
				switch (node.obstacles [dir]) {
					case 'W':
						newObstacle = Instantiate (mazeObjects [6], new Vector3 (oX, oY, 0f), Quaternion.identity, objectGroup.transform);
						newObstacle.transform.localScale = new Vector3 (oWidth, oHeight, 0.1f);
						break;
					case 'D':
						newObstacle = Instantiate (mazeObjects [7], new Vector3 (oX, oY, 0f), Quaternion.identity, objectGroup.transform);
						newObstacle.transform.localScale = new Vector3 (oWidth, oHeight, 0.1f);
						break;
					case 'G':
						newObstacle = Instantiate (mazeObjects [8], new Vector3 (oX, oY, 0f), Quaternion.identity, objectGroup.transform);
						newObstacle.transform.localScale = new Vector3 (oWidth, oHeight, 0.1f);
						break;
				}
			}
		}
	}

	private void AddNodes(){
		for(int i=1; i<map.GetLength(0); i=i+2){ // step by 2 because we are skiping the internal walls
			for(int j=1; j<map.GetLength(1); j=j+2){ // step by 2 because we are skiping the internal walls
				if(map[i,j] != 1) {	// not unbreakeble wall
					if(MapNumberToChar(map [i, j]) == 'I') {
						initialNode = nodes.Count;
					}
					nodes.Add(new Node(j, i, initialX+(j*sizeX), -(initialY+(i*sizeY)), MapNumberToChar(map [i, j])));
				}
			}
		}
	}

	private void ConnectNodes(){
		foreach (Node node in nodes){
			if(map[node.row-1, node.col] != 1 && node.row > 1) { // not unbreakeable wall 
				node.links[0] = nodes.FindIndex(x => x.col == node.col && x.row == node.row-2);
				node.obstacles[0] = MapNumberToChar(map[node.row-1, node.col]);
			}
			if(map[node.row, node.col+1] != 1) { // not unbreakeable wall 
				node.links[1] = nodes.FindIndex(x => x.col == node.col+2 && x.row == node.row);
				node.obstacles[1] = MapNumberToChar(map[node.row, node.col+1]);
			}
			if(map[node.row+1, node.col] != 1) { // not unbreakeable wall 
				node.links[2] = nodes.FindIndex(x => x.col == node.col && x.row == node.row+2);
				node.obstacles[2] = MapNumberToChar(map[node.row+1, node.col]);
			}
			if(map[node.row, node.col-1] != 1) { // not unbreakeable wall 
				node.links[3] = nodes.FindIndex(x => x.col == node.col-2 && x.row == node.row);
				node.obstacles[3] = MapNumberToChar(map[node.row, node.col-1]);
			}
		}
	}

	private void AddWalls(){
		
		// Horizontals walls
		for(int i=0; i<map.GetLength(0); i++){
			bool nextToPath = false;

			// start of the wall
			int j1 = -1; // without wall

			// end of the wall
			int j2 = 0;
			for(int j=0; j<map.GetLength(1); j++){
				if(map[i,j] == 1) { // wall
					if(j1 == -1){
						j1 = j;
					}
					if(!nextToPath) {
						if(i>0){
							if(map[i-1,j] != 1)
								nextToPath = true;
						}
					}
					if(!nextToPath) {
						if(i<map.GetLength(0)-1){
							if(map[i+1,j] != 1)
								nextToPath = true;
						}
					}
				}
				else {	// not wall
					if(j1 != -1) {
						if(j1 < j2 && nextToPath){
							float x = initialX+((j1 + j2) / 2.0f * sizeX);
							walls.Add(new Wall(x,-(initialY+(i*sizeY)),sizeX*(j2-j1)+(sizeY*wallWidth),sizeY*wallWidth));
						}
						j1 = -1;
						nextToPath = false;
					}
				}
				j2 = j;
			}
			if(j1 != -1) {
				if(j1 < j2  && nextToPath){
					float x = initialX+((j1 + j2) / 2.0f * sizeX);
					walls.Add(new Wall(x,-(initialY+(i*sizeY)),sizeX*(j2-j1)+(sizeY*wallWidth),sizeY*wallWidth));
				}
			}
		}

		// verticals
		for(int j=0; j<map.GetLength(1); j++){
			bool nextToPath = false;
			int i1 = -1; // without wall
			int i2 = 0;
			for(int i=0; i<map.GetLength(0); i++){
				if(map[i,j] == 1) { // wall
					if(i1 == -1){
						i1 = i;
					}
					if(!nextToPath) {
						if(j>0){	
							if(map[i,j-1] != 1)
								nextToPath = true;
						}
					}
					if(!nextToPath) {
						if(j<map.GetLength(1)-1){
							if(map[i,j+1] != 1)
								nextToPath = true;
						}
					}
				}
				else {	// not wall
					if(i1 != -1) {
						if(i1 < i2 && nextToPath){
							float y = initialY+((i1 + i2) / 2.0f * sizeY);
							walls.Add(new Wall((initialX+(j*sizeX)),-y,sizeX*wallWidth, sizeY*(i2-i1)+(sizeX*wallWidth)));
						}
						i1 = -1;
						nextToPath = false;
					}
				}
				i2 = i;
			}
			if(i1 != -1) {
				if(i1 < i2  && nextToPath){
					float y = initialY+((i1 + i2) / 2.0f * sizeY);
					walls.Add(new Wall((initialX+(j*sizeX)),-y,sizeX*wallWidth, sizeY*(i2-i1)+(sizeX*wallWidth)));
				}
			}
		}
	}

	public float getAngle(int dir) {
		switch (dir)
		{
			case 0:
				return 0f;
			case 1:
				return 270f;
			case 2:
				return 180f;
			case 3:
				return 90f;
			default:
				return 0f;
		}
	}

	private char MapNumberToChar(int i){
		switch (i) {
			// main nodes
			case 44: return 'K';
			case 10: return 'G';
			case 03: return 'E';
			case 06: return 'A';
			case 33: return 'B';
			case 29: return 'T';
			case 07: return 'I';
			// inter-nodes
			case 23: return 'D';
			case 53: return 'G';
			case 43: return 'W';
			default: return ' ';
		}
	}

	public bool SetDone(int node, int fromNode = -1) {
//		int onlyWay = -1;
		int nPaths = 0;

		for (int i = 0; i < 4; i++) {
			if (nodes [node].links [i] != -1) {
				if (nodes [node].links [i] == fromNode) {
					nPaths++;
				} else {
					if (nodes [node].obstacles [i] == ' ' && nodes [nodes [node].links [i]].done == false && nodes [nodes [node].links [i]].cObject == ' ' ) {
						SetDone (nodes [node].links [i], node);
					}
					if (nodes [nodes [node].links [i]].done == false) {
						nPaths++;
//						if (nodes [nodes [node].links [i]].cObject == ' ' && nodes [node].obstacles [i] == ' ') {
//							onlyWay = i;
//						}
					}
				}
			}
		}
		if (nPaths <= 1) {
			nodes[node].cObject = ' ';
			nodes [node].done = true;
			doneNodes++;
			Instantiate (mazeObjects [0], new Vector3 (nodes [node].x, nodes [node].y, 0), Quaternion.identity, doneGroup.transform);
//			if (onlyWay != -1){
//				SetDone (nodes [node].links [onlyWay]);
//			}
			return true;
		}
		return false;
	}

	public int GetDoneNodes(){
		return doneNodes;
	}

	private void InitializeLevel(int level) {

		// 0  = empty path in the maze
		// 1  = unbreakeble walls
		// 3  = path with an enemy
		// 6  = path with ammo
		// 7  = initial play position
		// 10 = path with a golden key
		// 23 = path with a door
		// 29 = path with trap
		// 33 = path with bomb reload
		// 43 = path with a breakeable wall
		// 44 = path with a key
		// 53 = path with a golden door

		// temporaly values
		// 15 = Tu 
		// 48 = Tr

		if (level == 1) {
			int[,] tmpMap = new int[,] {
				{1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,53,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
				{1,0,0,0,1,0,43,0,0,0,0,0,1,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,29,1},
				{1,0,1,0,1,0,1,1,1,1,1,0,1,0,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,0,1},
				{1,0,1,0,1,0,1,0,0,0,0,0,1,0,1,0,0,3,0,0,1,33,0,0,0,0,0,29,1,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
				{1,0,1,0,1,0,1,0,1,1,1,1,1,0,1,0,1,1,1,0,1,1,1,0,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,0,1},
				{1,0,1,3,1,0,1,0,0,0,0,0,0,0,1,0,1,6,1,3,0,0,0,0,0,3,0,0,1,0,1,3,0,0,0,0,0,0,0,0,1,0,1},
				{1,0,1,0,1,0,1,1,1,1,1,1,1,0,1,0,1,0,1,1,1,1,1,1,1,1,1,1,1,0,1,0,1,1,1,1,1,1,1,0,1,0,1},
				{1,0,1,0,1,0,0,0,0,0,0,29,1,3,1,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,1,44,0,0,0,0,0,0,1,0,1},
				{1,0,1,0,1,0,1,1,1,1,1,1,1,0,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,0,1},
				{1,0,1,0,1,0,1,0,0,0,0,0,1,0,1,3,0,0,0,0,0,0,1,0,0,0,0,6,1,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
				{1,0,1,0,1,0,1,0,1,1,1,0,1,0,1,1,1,1,1,1,1,0,1,0,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1},
				{1,0,1,0,1,3,1,0,1,6,1,0,0,0,0,0,0,0,0,0,1,0,1,0,0,3,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
				{1,0,1,0,1,0,1,0,1,0,1,1,1,1,1,1,1,1,1,0,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,0,1},
				{1,0,1,0,0,0,1,0,1,0,0,0,0,3,0,0,0,0,0,0,1,0,1,0,0,0,0,0,1,0,0,0,1,0,1,0,1,0,0,3,1,0,1},
				{1,0,1,1,1,1,1,23,1,1,1,1,1,1,1,1,1,23,1,0,1,0,1,0,1,0,1,0,1,0,1,0,1,0,1,0,1,0,1,0,1,0,1},
				{1,0,0,0,0,0,0,0,0,0,0,0,0,3,1,3,0,0,1,0,1,0,1,0,1,0,1,0,1,0,1,0,1,0,0,0,1,0,1,0,1,0,1},
				{1,0,1,1,1,1,1,1,1,1,1,1,1,0,1,0,1,1,1,0,1,0,1,0,1,0,1,0,1,0,1,0,1,23,1,1,1,0,1,0,1,0,1},
				{1,0,1,0,0,0,0,0,0,6,0,6,1,0,1,0,1,33,1,0,1,0,1,0,1,0,1,0,1,0,1,0,0,0,0,0,0,0,1,0,1,0,1},
				{1,0,1,0,1,1,1,1,1,1,1,1,1,0,1,0,1,0,1,0,1,0,1,0,1,0,1,0,1,0,1,1,1,1,1,1,1,1,1,0,1,0,1},
				{1,0,1,0,0,3,0,0,0,0,0,0,0,0,1,0,0,0,1,0,1,0,1,0,1,0,1,3,1,0,0,0,0,3,0,0,0,0,1,0,1,0,1},
				{1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,0,1,0,1,0,1,0,1,1,1,1,1,1,1,1,1,0,1,0,1,0,1},
				{1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,1,0,1,0,1,0,1,0,1,0,0,0,0,0,0,3,1,0,1,0,1,0,1},
				{1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,0,1,0,1,0,1,0,1,0,1,0,1,1,1,1,1,0,1,0,1,0,1,0,1},
				{1,0,0,0,1,0,0,0,0,3,0,0,0,0,0,0,1,0,1,0,1,44,1,0,1,0,1,0,1,0,1,33,0,44,0,0,1,0,1,0,1,0,1},
				{1,0,1,0,1,0,1,1,1,1,1,1,1,1,1,0,1,0,1,0,1,1,1,0,1,0,1,0,1,0,1,1,1,1,1,1,1,0,1,0,1,0,1},
				{1,0,1,0,1,0,1,29,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,1,0,1,0,1,3,0,0,0,0,0,0,0,0,1,0,1,6,1},
				{1,0,1,0,1,0,1,1,1,1,1,1,1,1,1,0,1,0,1,1,1,1,1,1,1,0,1,0,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1},
				{1,0,1,0,1,0,0,0,1,6,0,0,0,0,1,0,1,0,1,0,0,0,0,0,1,0,1,0,0,0,0,3,0,0,0,0,0,0,0,0,0,29,1},
				{1,0,1,0,1,1,1,0,1,1,1,1,1,0,1,0,1,0,1,0,1,1,1,0,1,0,1,1,1,1,1,1,1,0,1,1,1,1,1,0,1,1,1},
				{1,0,1,0,0,0,0,0,1,0,0,0,1,0,1,0,1,3,1,0,0,29,1,0,43,0,1,0,0,0,0,0,0,0,0,0,0,0,1,29,1,0,1},
				{1,23,1,1,1,1,1,1,1,0,1,1,1,0,1,0,1,0,1,0,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1},
				{1,0,1,44,0,0,0,0,0,3,1,0,0,0,1,0,1,0,1,3,1,33,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
				{1,0,1,1,1,1,1,1,1,0,1,0,1,1,1,0,1,0,1,0,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,0,1},
				{1,0,43,0,0,3,0,0,1,0,1,0,0,0,0,3,1,0,1,0,0,0,0,0,0,3,0,0,0,0,0,3,0,0,1,0,0,0,0,0,1,0,1},
				{1,0,1,1,1,1,1,0,1,43,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,0,1,0,1,1,1,1,1,0,1},
				{1,0,1,0,0,6,0,0,1,0,0,0,1,0,0,0,1,6,1,0,0,0,1,0,1,0,1,0,0,0,0,0,1,6,1,0,0,0,0,0,1,0,1},
				{1,0,1,0,1,1,1,1,1,0,1,0,1,0,1,0,1,1,1,0,1,0,1,0,1,0,1,0,1,1,1,0,1,1,1,1,1,1,1,0,1,23,1},
				{1,0,1,44,1,0,0,0,0,0,1,0,1,0,1,0,1,0,1,3,1,0,1,0,1,29,1,0,1,0,1,0,1,0,1,33,0,0,0,0,1,0,1},
				{1,0,1,1,1,0,1,1,1,0,1,0,1,0,1,0,1,0,1,0,1,0,1,0,1,1,1,0,1,0,1,0,1,0,1,1,1,1,1,1,1,0,1},
				{1,3,1,0,0,0,1,33,1,0,1,0,0,0,1,0,1,0,1,0,1,0,0,0,1,0,0,0,1,0,1,0,0,3,1,0,0,3,0,0,1,0,1},
				{1,0,1,1,1,1,1,0,1,0,1,1,1,0,1,0,1,0,1,0,1,0,1,0,1,0,1,1,1,0,1,0,1,0,1,0,1,0,1,1,1,0,1},
				{1,0,0,0,0,0,1,0,1,0,0,0,1,0,1,0,1,0,1,0,1,3,1,0,1,0,0,0,0,0,1,0,1,0,1,0,1,0,1,29,1,0,1},
				{1,1,1,0,1,0,1,0,1,1,1,1,1,1,1,0,1,0,1,0,1,0,1,43,1,1,1,1,1,0,1,0,1,0,1,0,1,0,1,0,1,0,1},
				{1,6,1,0,1,0,1,0,1,0,0,0,0,0,0,0,0,0,1,6,1,0,1,0,0,0,0,0,1,0,1,0,1,0,0,0,1,0,1,0,1,0,1},
				{1,0,1,0,1,0,1,0,1,1,1,1,1,1,1,1,1,0,1,1,1,0,1,0,1,0,1,1,1,0,1,0,1,1,1,1,1,0,1,0,1,0,1},
				{1,0,1,0,1,0,1,0,0,3,0,0,0,0,0,0,0,0,0,0,0,0,1,0,1,0,1,0,0,0,1,0,1,0,0,0,0,0,1,0,1,3,1},
				{1,0,1,0,1,0,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,0,1,0,1,0,1,0,1,0,1,0,1,0,1,1,1,1,1,0,1,0,1},
				{1,0,1,0,1,0,0,0,1,0,1,0,1,0,0,3,0,0,0,29,1,0,1,0,1,0,1,0,1,29,1,29,1,33,1,0,0,0,0,0,1,0,1},
				{1,0,1,0,1,1,1,1,1,0,1,0,1,0,1,1,1,0,1,1,1,1,1,0,1,0,1,23,1,1,1,1,1,1,1,1,1,0,1,43,1,0,1},
				{1,3,1,0,0,0,1,0,0,0,1,29,1,0,0,6,1,0,0,0,0,0,0,0,1,0,1,0,0,0,1,6,0,0,0,0,0,3,1,0,0,0,1},
				{1,0,1,0,1,43,1,0,1,23,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,0,1,0,1,0,1,1,1,1,1,1,1,0,1,1,1,1,1},
				{1,0,0,0,1,0,0,3,1,0,1,0,1,0,0,3,1,0,0,0,0,0,1,0,1,0,1,0,1,0,0,0,1,0,0,0,0,0,1,0,0,0,1},
				{1,0,1,0,1,1,1,0,1,0,1,0,1,0,1,0,1,0,1,1,1,0,1,0,1,0,1,0,1,0,1,1,1,23,1,1,1,1,1,43,1,0,1},
				{1,0,1,3,1,0,1,0,1,0,1,44,0,0,1,0,0,0,0,29,1,0,1,0,1,0,1,0,1,0,1,0,1,0,0,0,0,0,0,0,1,3,1},
				{1,0,1,0,1,0,1,1,1,0,1,1,1,1,1,1,1,0,1,1,1,0,1,1,1,0,1,0,1,0,1,0,1,1,1,1,1,1,1,1,1,0,1},
				{1,0,1,0,0,0,1,0,1,0,0,0,1,0,0,0,1,0,1,0,0,0,0,0,1,0,0,0,1,0,23,0,1,33,0,3,0,0,0,0,1,0,1},
				{1,1,1,1,1,0,1,0,1,1,1,0,1,0,1,0,1,0,1,0,1,1,1,0,1,1,1,0,1,0,1,0,1,1,1,1,1,1,1,0,1,0,1},
				{1,3,0,0,0,0,1,0,0,0,0,0,0,0,1,0,1,0,1,0,0,0,1,0,1,44,1,0,1,0,1,0,0,0,0,0,0,0,0,3,1,0,1},
				{1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,0,1,1,1,1,1,0,1,0,1,1,1,0,1,1,1,1,1,1,1,0,1,0,1,0,1},
				{1,44,0,0,1,6,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,1,0,0,0,0,3,1,6,0,0,0,0,0,3,1,44,1,10,1},
				{1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,53,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
				{1,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,1,0,0,0,0,0,1,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,1},
				{1,0,1,1,1,0,1,1,1,1,1,1,1,0,1,0,1,0,1,1,1,0,1,0,1,1,1,1,1,0,1,0,1,1,1,1,1,23,1,1,1,0,1},
				{1,0,1,6,1,0,0,0,0,0,0,0,0,0,1,0,1,0,1,0,1,0,0,0,1,0,0,0,1,0,0,0,1,0,0,0,1,0,1,0,1,0,1},
				{1,0,1,0,1,1,1,1,1,1,1,1,1,1,1,0,1,0,1,0,1,1,1,1,1,1,1,0,1,1,1,1,1,0,1,0,1,0,1,0,1,1,1},
				{1,0,1,0,0,0,1,0,0,0,0,0,1,0,0,0,1,0,1,0,0,0,1,0,0,0,0,0,1,0,0,10,1,0,1,0,0,0,1,0,0,0,1},
				{1,0,1,1,1,0,1,0,1,1,1,0,1,0,1,0,1,0,1,0,1,0,1,0,1,1,1,0,1,0,1,1,1,0,1,1,1,1,1,1,1,0,1},
				{1,0,0,0,1,0,0,0,1,0,0,0,1,0,1,0,1,0,1,0,1,0,0,0,1,0,0,0,1,0,0,0,1,0,0,0,1,0,0,0,0,3,1},
				{1,1,1,0,1,1,1,1,1,0,1,1,1,0,1,0,1,0,1,0,1,1,1,1,1,0,1,1,1,1,1,0,1,1,1,0,1,1,1,1,1,0,1},
				{1,0,1,0,1,0,0,0,0,0,1,0,0,0,1,0,1,0,1,0,0,0,1,0,0,0,0,0,1,0,0,0,1,0,0,0,1,0,0,0,1,0,1},
				{1,0,1,0,1,0,1,1,1,1,1,23,1,1,1,1,1,0,1,0,1,0,1,1,1,1,1,0,1,0,1,0,1,0,1,1,1,0,1,0,1,0,1},
				{1,0,0,0,1,0,0,0,0,3,1,0,0,0,0,0,0,0,1,0,1,0,0,0,0,0,1,0,0,0,1,0,1,0,0,0,0,0,1,0,1,0,1},
				{1,0,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,0,1,1,1,1,1,0,1,1,1,1,1,0,1,0,1},
				{1,0,1,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,1,0,1,3,1,0,0,0,1,0,0,0,1,0,0,0,1,0,1},
				{1,0,1,1,1,0,1,0,1,0,1,1,1,1,1,0,1,0,1,1,1,1,1,0,1,0,1,1,1,0,1,0,1,1,1,1,1,0,1,1,1,0,1},
				{1,0,0,0,1,0,1,0,1,0,0,0,1,0,0,0,1,0,1,0,0,0,1,0,1,0,1,0,0,0,1,0,0,0,1,0,0,0,1,0,1,0,1},
				{1,1,1,0,1,0,1,0,1,1,1,0,1,0,1,1,1,1,1,0,1,0,1,0,1,0,1,0,1,1,1,1,1,0,1,0,1,1,1,0,1,0,1},
				{1,0,1,0,0,0,1,0,1,0,0,0,1,0,1,0,0,0,0,0,1,0,1,0,43,0,1,0,0,0,0,0,1,0,0,0,2,0,0,0,0,0,1},
				{1,0,1,1,1,1,1,0,1,0,1,1,1,0,1,0,1,1,1,1,1,0,1,0,1,1,1,1,1,0,1,0,1,1,1,1,1,0,1,1,1,1,1},
				{1,0,1,0,0,0,0,0,1,0,0,0,1,0,1,0,1,0,0,0,1,0,1,0,1,0,0,0,1,0,1,0,1,0,0,0,1,0,0,0,1,3,1},
				{1,0,1,0,1,1,1,1,1,1,1,0,1,0,1,0,1,0,1,0,1,0,1,1,1,0,1,0,1,0,1,0,1,0,1,0,1,1,1,0,1,0,1},
				{1,0,1,0,0,0,1,0,0,0,1,0,1,0,1,0,0,0,1,0,1,3,0,0,0,0,1,0,1,0,1,0,1,0,1,0,0,0,1,0,1,0,1},
				{1,0,1,1,1,0,1,0,1,0,1,1,1,0,1,1,1,1,1,0,1,1,1,1,1,2,1,0,1,1,1,0,1,0,1,1,1,0,1,0,1,0,1},
				{1,0,0,0,1,0,0,0,1,0,23,0,43,0,0,0,0,2,1,0,1,0,0,0,0,0,1,0,0,0,1,0,1,0,0,0,1,0,0,0,1,0,1},
				{1,0,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,0,1,0,1,1,1,1,1,1,1,1,1,0,1,0,1,1,1,0,1,1,1,1,1,0,1},
				{1,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,1,0,1,0,0,0,1,29,0,0,0,0,0,0,1,0,0,0,1,0,1,44,1,0,43,0,1},
				{1,0,1,1,1,1,1,1,1,1,1,1,1,0,1,0,1,0,1,1,1,0,1,1,1,1,1,0,1,1,1,1,1,0,1,0,1,0,1,0,1,0,1},
				{1,0,0,0,1,29,0,0,0,0,1,0,0,0,1,0,0,0,0,0,1,0,0,0,0,0,1,0,0,0,0,0,0,0,1,0,1,0,0,0,1,0,1},
				{1,1,1,0,1,1,1,0,1,0,1,0,1,1,1,1,1,1,1,43,1,1,1,1,1,0,1,0,1,1,1,1,1,1,1,0,1,0,1,1,1,0,1},
				{1,0,1,0,0,0,0,0,1,0,1,0,0,0,0,0,1,0,1,0,1,0,0,0,1,0,1,0,1,0,0,0,0,0,1,0,1,0,1,0,0,0,1},
				{1,0,1,1,1,1,1,1,1,0,1,0,1,1,1,0,1,0,1,0,1,0,1,0,1,0,1,0,1,0,1,1,1,0,1,0,1,1,1,0,1,0,1},
				{1,0,1,0,0,0,0,0,1,0,1,0,1,0,1,0,1,0,0,0,0,0,1,6,1,0,1,0,1,0,1,33,1,0,1,0,0,0,0,0,1,0,1},
				{1,0,1,0,1,1,1,0,1,0,1,0,1,0,1,0,1,1,1,1,1,1,1,1,1,0,1,1,1,0,1,0,1,0,1,1,1,1,1,1,1,1,1},
				{1,0,0,0,1,0,1,0,0,0,1,0,0,0,1,0,1,0,0,0,0,0,0,0,1,3,0,0,0,0,1,0,1,0,0,0,0,0,0,0,0,0,1},
				{1,0,1,1,1,0,1,1,1,1,1,0,1,1,1,0,1,0,1,1,1,1,1,0,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,0,1},
				{1,0,1,3,0,0,0,0,1,0,0,0,1,0,0,0,1,0,1,3,0,0,0,0,1,29,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,1},
				{1,0,1,1,1,0,1,0,1,1,1,1,1,0,1,0,1,0,1,0,1,1,1,0,1,1,1,1,1,1,1,0,1,1,1,0,1,1,1,0,1,0,1},
				{1,0,0,0,1,0,1,0,0,0,0,0,1,0,1,0,1,0,1,0,1,0,0,0,0,0,1,0,0,0,1,0,0,0,1,0,1,0,1,0,1,0,1},
				{1,1,1,0,1,0,1,1,1,1,1,0,1,0,1,1,1,0,1,0,1,1,1,1,1,0,1,0,1,0,1,1,1,1,1,0,1,0,1,0,1,0,1},
				{1,0,1,0,0,0,1,0,0,0,1,0,1,0,0,0,0,0,1,0,1,0,0,0,1,0,0,0,1,0,0,0,0,0,0,0,1,0,1,0,1,0,1},
				{1,0,1,1,1,1,1,0,1,0,1,2,1,1,1,1,1,1,1,0,1,0,1,0,1,0,1,1,1,1,1,1,1,1,1,1,1,0,1,0,1,0,1},
				{1,0,0,0,0,0,1,0,1,0,1,0,0,0,1,0,0,0,0,0,1,0,1,0,1,0,1,0,0,0,0,0,0,0,1,0,0,0,0,0,1,0,1},
				{1,0,1,1,1,0,1,0,1,0,1,1,1,0,1,0,1,1,1,1,1,0,1,0,1,1,1,0,1,1,1,1,1,0,1,1,1,1,1,1,1,23,1},
				{1,0,1,0,1,0,0,0,1,0,1,0,0,0,1,0,0,0,1,0,0,0,1,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,2,0,1},
				{1,0,1,0,1,1,1,1,1,0,1,0,1,1,1,0,1,0,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,0,1},
				{1,0,1,0,0,0,0,0,1,0,1,0,1,0,0,0,1,0,0,0,1,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,1,0,1},
				{1,0,1,0,1,1,1,0,1,0,1,0,1,1,1,0,1,1,1,1,1,0,1,1,1,1,1,0,1,0,1,1,1,0,1,1,1,1,1,1,1,0,1},
				{1,0,1,0,0,0,1,0,0,0,1,3,0,0,1,0,0,0,0,0,1,0,1,0,0,0,0,0,0,0,1,0,0,0,1,0,0,0,0,0,0,0,1},
				{1,0,1,1,1,0,1,1,1,1,1,1,1,0,1,1,1,0,1,1,1,0,1,0,1,1,1,1,1,1,1,1,1,1,1,0,1,0,1,1,1,1,1},
				{1,0,1,0,1,0,1,0,0,44,1,0,1,0,0,0,1,0,1,0,0,0,1,0,1,0,0,0,0,0,0,0,0,0,1,0,1,0,1,0,0,3,1},
				{1,0,1,0,1,0,1,0,1,0,1,0,1,1,1,0,1,1,1,0,1,1,1,0,1,0,1,1,1,1,1,1,1,0,1,0,1,1,1,0,1,0,1},
				{1,0,1,0,1,0,0,0,1,0,1,0,0,0,1,0,0,0,0,0,1,0,0,0,1,0,1,0,0,0,0,0,1,0,1,0,1,0,0,0,1,0,1},
				{1,0,1,0,1,1,1,1,1,0,1,0,1,0,1,1,1,1,1,0,1,1,1,1,1,0,1,1,1,1,1,0,1,0,1,0,1,0,1,1,1,0,1},
				{1,0,1,0,0,0,0,0,1,0,0,0,1,0,1,0,0,0,0,0,1,0,0,0,1,0,0,0,0,0,1,0,0,0,1,0,0,0,0,0,1,0,1},
				{1,0,1,0,1,0,1,1,1,1,1,1,1,0,1,0,1,1,1,1,1,0,1,0,1,1,1,1,1,0,1,0,1,1,1,1,1,1,1,1,1,0,1},
				{1,0,1,0,1,0,0,0,0,0,0,0,0,0,1,0,0,0,1,0,1,0,1,0,0,0,0,0,0,0,1,0,0,0,1,0,0,0,1,0,0,0,1},
				{1,0,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,0,1,0,1,1,1,1,1,1,1,1,1,1,1,0,1,0,1,0,1,0,1,1,1},
				{1,0,1,3,0,6,1,0,0,0,0,0,1,0,0,0,1,0,1,0,1,0,1,0,0,0,0,0,0,0,0,0,1,0,0,0,1,0,1,0,0,0,1},
				{1,0,1,1,1,1,1,0,1,1,1,0,1,0,1,0,1,0,1,0,1,0,1,0,1,0,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,0,1},
				{1,3,0,0,0,0,0,0,0,0,1,0,0,0,1,0,0,0,0,0,1,0,1,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
				{1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,53,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
				{1,0,0,6,1,0,0,0,0,29,1,6,0,0,0,0,0,29,1,29,1,0,0,0,1,6,1,0,0,0,0,0,0,3,0,0,0,6,1,0,0,0,1},
				{1,0,1,1,1,0,1,0,1,1,1,1,1,0,1,0,1,1,1,0,1,1,1,0,1,0,1,0,1,1,1,1,1,1,1,0,1,1,1,0,1,0,1},
				{1,0,0,0,0,3,1,3,0,0,0,3,0,0,1,3,0,0,0,0,0,3,0,0,1,0,1,0,1,0,23,0,0,0,0,0,0,6,1,3,1,6,1},
				{1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,0,1,0,1,1,1,1,1,1,1,1,1,0,1,1,1},
				{1,3,1,29,0,0,0,0,0,0,0,3,0,6,1,0,0,0,0,0,0,0,0,0,1,0,1,0,1,0,0,0,0,29,1,33,1,0,0,0,0,0,1},
				{1,0,1,1,1,0,1,1,1,1,1,0,1,1,1,0,1,0,1,1,1,1,1,0,1,0,1,0,1,0,1,1,1,1,1,0,1,23,1,1,1,0,1},
				{1,0,0,0,1,0,0,0,0,3,1,0,0,0,0,0,1,0,0,0,1,6,0,0,1,0,43,0,1,0,0,3,0,29,1,0,0,0,0,44,1,0,1},
				{1,1,1,0,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,0,1,0,1,0,1,1,1,1,1,1,1,1,1,1,1,0,1},
				{1,0,0,0,1,44,0,0,0,0,0,0,1,0,0,0,0,3,1,0,0,0,0,0,1,0,1,0,1,0,0,0,0,29,1,3,0,0,0,0,0,0,1},
				{1,0,1,1,1,1,1,1,1,1,1,0,1,0,1,1,1,0,1,1,1,1,1,0,1,0,1,0,1,0,1,1,1,1,1,0,1,1,1,1,1,0,1},
				{1,0,0,0,23,0,0,0,0,0,1,0,1,0,0,0,1,0,0,0,1,29,0,0,0,0,1,3,1,0,1,29,1,0,0,0,1,6,0,0,0,0,1},
				{1,1,1,1,1,1,1,1,1,0,1,0,1,1,1,0,1,1,1,0,1,1,1,1,1,1,1,0,1,0,1,0,1,0,1,0,1,1,1,1,1,1,1},
				{1,0,0,3,0,0,1,29,1,0,1,6,1,0,0,0,1,29,1,0,0,0,0,0,1,0,0,0,1,0,1,0,1,0,1,0,1,0,0,3,0,0,1},
				{1,0,1,1,1,0,1,0,1,0,1,1,1,0,1,1,1,0,1,1,1,1,1,0,1,0,1,0,1,0,1,0,1,0,1,0,1,0,1,1,1,0,1},
				{1,0,1,6,0,0,1,0,1,3,0,0,0,0,1,0,0,0,0,0,0,0,1,0,0,0,1,29,1,0,0,0,1,3,1,0,1,0,0,0,1,0,1},
				{1,0,1,1,1,0,1,0,1,1,1,1,1,1,1,0,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,0,1,0,1,0,1,1,1,0,1,0,1},
				{1,0,0,0,1,0,0,0,1,0,0,0,0,0,0,0,1,0,0,0,1,6,1,0,0,0,0,0,0,0,1,0,1,0,1,0,0,0,1,0,1,0,1},
				{1,0,1,0,1,1,1,0,1,0,1,1,1,0,1,1,1,0,1,0,1,1,1,0,1,1,1,1,1,0,1,0,1,0,1,1,1,0,1,0,1,0,1},
				{1,0,1,0,0,3,1,0,0,0,1,3,0,0,1,0,0,0,1,3,0,0,1,0,1,6,0,0,1,0,0,3,1,0,0,44,1,3,1,0,1,0,1},
				{1,0,1,1,1,0,1,1,1,1,1,0,1,1,1,0,1,1,1,1,1,0,1,0,1,1,1,0,1,1,1,1,1,1,1,1,1,0,1,0,1,0,1},
				{1,0,0,33,1,0,1,0,0,0,1,0,0,0,1,0,0,0,1,29,1,0,1,3,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,1,0,1},
				{1,1,1,1,1,0,1,0,1,0,1,1,1,0,1,1,1,0,1,0,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,0,1,0,1},
				{1,0,0,0,0,0,1,3,1,0,0,0,1,0,1,6,1,0,0,0,1,0,0,0,0,0,0,0,1,0,0,0,0,29,1,0,0,0,1,29,1,0,1},
				{1,0,1,1,1,0,1,0,1,1,1,0,1,0,1,0,1,1,1,0,1,1,1,1,1,1,1,0,1,0,1,0,1,1,1,1,1,0,1,1,1,0,1},
				{1,0,1,6,0,0,1,0,0,0,1,0,1,3,0,0,0,44,1,0,0,0,0,0,1,0,0,0,1,0,1,0,0,3,0,0,1,0,0,0,1,0,1},
				{1,43,1,1,1,1,1,1,1,0,1,0,1,1,1,1,1,1,1,1,1,1,1,0,1,0,1,1,1,0,1,1,1,1,1,0,1,1,1,0,1,43,1},
				{1,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,1,0,0,0,0,0,1,3,1,3,0,0,1,0,1,3,0,0,1,0,0,6,1,0,0,0,1},
				{1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,0,1,0,1,0,1,0,1,1,1,0,1,0,1,0,1,0,1,0,1,1,1,1,1,0,1},
				{1,0,1,29,0,0,0,0,0,0,0,0,0,0,1,0,0,0,1,0,1,33,1,0,1,33,1,0,0,0,1,0,1,0,1,0,1,0,0,0,0,0,1},
				{1,0,1,1,1,1,1,0,1,1,1,1,1,0,1,1,1,1,1,0,1,1,1,0,1,0,1,1,1,1,1,0,1,0,1,0,1,0,1,1,1,1,1},
				{1,0,0,0,0,0,1,0,1,0,0,3,0,0,1,0,0,0,1,0,0,0,1,0,0,0,0,0,1,0,0,0,1,0,1,0,0,0,1,0,0,0,1},
				{1,1,1,1,1,0,1,0,1,0,1,1,1,0,1,0,1,0,1,1,1,0,1,1,1,1,1,0,1,0,1,1,1,0,1,1,1,1,1,0,1,0,1},
				{1,0,0,0,0,0,1,33,1,0,0,0,1,0,0,0,1,0,0,0,1,3,1,0,0,0,1,0,0,0,1,6,1,0,1,0,0,0,0,0,1,0,1},
				{1,0,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,0,1,0,1,0,1,0,1,1,1,1,1,0,1,0,1,0,1,1,1,0,1,0,1},
				{1,3,1,0,0,0,0,0,0,0,0,0,1,0,0,0,1,0,0,0,1,0,0,0,1,0,0,0,23,0,0,0,1,0,1,3,0,0,1,0,1,6,1},
				{1,0,1,0,1,1,1,1,1,1,1,1,1,0,1,0,1,43,1,1,1,1,1,1,1,1,1,0,1,1,1,0,1,0,1,1,1,0,1,0,1,1,1},
				{1,0,1,0,0,0,0,0,1,6,0,0,0,0,1,0,1,0,1,29,0,0,0,0,0,3,1,0,1,0,0,0,1,0,0,0,1,0,1,0,0,0,1},
				{1,0,1,0,1,1,1,0,1,1,1,0,1,1,1,0,1,0,1,1,1,0,1,1,1,0,1,0,1,0,1,1,1,1,1,0,1,0,1,1,1,43,1},
				{1,0,1,0,0,6,1,0,0,0,1,3,0,44,1,0,0,0,0,0,0,0,1,33,0,0,1,0,1,0,1,0,0,0,1,0,1,0,0,44,1,0,1},
				{1,0,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,0,1,0,1,0,1,0,1,0,1,1,1,0,1},
				{1,0,0,0,23,0,0,0,0,0,1,0,0,0,0,0,1,0,0,0,0,3,0,0,0,0,1,0,1,3,1,0,1,0,0,0,1,29,1,0,23,0,1},
				{1,1,1,43,1,1,1,1,1,1,1,0,1,1,1,0,1,0,1,1,1,1,1,1,1,0,1,0,1,0,1,0,1,1,1,1,1,1,1,0,1,0,1},
				{1,33,1,0,1,0,0,0,0,0,0,0,1,6,0,0,1,0,0,0,1,0,0,0,1,0,0,0,1,0,1,0,1,3,0,0,0,0,0,0,1,0,1},
				{1,0,1,0,1,0,1,1,1,1,1,1,1,1,1,0,1,1,1,0,1,0,1,0,1,1,1,1,1,0,1,0,1,1,1,0,1,1,1,1,1,0,1},
				{1,0,0,0,1,0,0,3,0,0,0,0,0,0,1,0,0,0,0,0,1,33,1,0,0,0,0,0,0,0,1,0,0,0,0,0,1,0,0,6,1,0,1},
				{1,0,1,1,1,1,1,1,1,1,1,1,1,0,1,2,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,0,1},
				{1,0,1,0,0,0,0,3,0,0,0,0,1,0,1,29,1,29,2,0,0,0,0,0,1,0,0,0,1,0,0,0,0,0,0,29,1,0,0,0,1,0,1},
				{1,0,1,0,1,1,1,1,1,1,1,0,1,0,1,1,1,1,1,0,1,1,1,0,1,1,1,0,1,0,1,1,1,0,1,1,1,0,1,0,1,0,1},
				{1,0,0,0,1,6,0,0,0,0,1,0,1,0,0,0,0,0,1,0,0,0,1,0,0,0,1,0,0,0,1,44,0,0,1,0,0,0,1,0,0,0,1},
				{1,1,1,1,1,1,1,1,1,43,1,0,1,1,1,1,1,0,1,1,1,0,1,1,1,0,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1},
				{1,0,43,0,43,0,43,0,0,0,1,0,1,29,1,0,0,0,0,0,1,0,1,29,1,0,0,0,23,0,0,0,0,0,1,0,1,0,0,0,0,0,1},
				{1,0,1,1,1,1,1,1,1,43,1,0,1,0,1,0,1,1,1,0,1,0,1,0,1,1,1,1,1,1,1,1,1,0,1,0,1,0,1,1,1,0,1},
				{1,0,0,3,1,10,0,0,0,0,1,3,0,0,1,0,1,6,0,0,1,0,0,0,1,0,0,0,0,0,0,0,1,0,1,0,1,0,0,33,1,0,1},
				{1,0,1,0,1,1,1,1,1,1,1,1,1,0,1,0,1,1,1,1,1,1,1,0,1,0,1,1,1,1,1,0,1,0,1,0,1,1,1,1,1,0,1},
				{1,0,1,0,0,0,0,0,1,0,0,0,23,0,1,0,1,0,0,0,0,0,0,0,1,0,0,0,0,33,1,0,1,0,1,0,0,0,0,0,0,0,1},
				{1,0,1,1,1,1,1,0,1,0,1,1,1,1,1,0,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,0,1,1,1,1,1,1,1,0,1},
				{1,0,0,0,1,0,0,0,1,0,0,0,1,33,1,0,1,0,1,0,0,0,1,0,0,0,0,0,0,0,1,0,1,0,0,0,1,0,0,0,0,0,1},
				{1,1,1,0,1,0,1,1,1,1,1,0,1,0,1,0,1,0,1,0,1,0,1,0,1,1,1,1,1,0,1,0,1,1,1,0,1,0,1,1,1,1,1},
				{1,44,0,0,1,0,0,0,0,3,0,0,0,0,1,0,43,0,0,0,1,7,0,0,1,29,0,0,0,0,0,0,0,6,1,0,0,0,0,0,0,29,1},
				{1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
			};
			map = tmpMap;
		}
	}
} // end fo the class Maze

public class Wall {
	public float x;
	public float y;
	public float width;
	public float height;

	public Wall(float x1, float y1, float w, float h){
		width = w;
		height = h;
		x = x1;
		y = y1;
	}
}

public class Node {
	// coordinates in the original map
	public int col;
	public int row;

	// coordinates in the screen
	public float x;
	public float y;

	public char initialObject;
	public char cObject;
	public bool done;

	// links to others nodes
	// 0 = up, 1 = right, 2 = down, 3 = left
	public int[] links;

	// objects that can block the path between nodes
	// D = door, G = Golden door, W = breakeble wall, P = Player
	public char[] obstacles;

    public Node(int c, int r, float x1, float y1, char o){
		col = c;
		row = r;
		x = x1;
		y = y1;
		links = new int[] {-1, -1, -1, -1};
		obstacles = new char[] {' ', ' ', ' ', ' '};
		initialObject = o;
		cObject = ' ';
		if (o == 'B' || o == 'K' || o == 'G'|| o == 'A') {
			cObject = o;
		}
		done = false;
	}
}
