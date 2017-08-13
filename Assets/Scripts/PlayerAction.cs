using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float bulletSpeed;
    public GameObject bombSprite;

    private Transform shootLoc;
    private PlayerController pc;
    private GameObject key;
    private Collider2D keyColl;
    private Renderer keyRend;
    private Maze maze;

    // Use this for initialization
    void Start()
    {
        maze = GameObject.Find("GameController").GetComponent<Maze>();
        pc = GetComponent<PlayerController>();
        shootLoc = transform.GetChild(0);
        key = transform.GetChild(1).gameObject;
        keyColl = key.GetComponent<Collider2D>();
        keyRend = key.GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("ShootOffset: " + shootLoc.position);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (pc.UseAmmo())
            {
                GameObject bullet = Instantiate(bulletPrefab, shootLoc.position, Quaternion.identity);
                bullet.GetComponent<Rigidbody2D>().velocity = transform.up * bulletSpeed;
            }
        }
        else if (Input.GetKeyDown(KeyCode.B)) 
        {
			if (pc.TestBomb())
            {
				if (pc.onBombSpot())
                {
                    GameObject bomb = Instantiate(bombSprite, transform.position, Quaternion.identity);
                    bomb.GetComponent<BombScript>().setNode(pc.GetNode());
                    pc.UseBomb();
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.K)) // Use key
        {
			if (pc.TestGoldenKey() || pc.TestKey())
            {
                keyColl.enabled = true;
                keyRend.enabled = true;
			} else {
				pc.setMessage("You are lack of keys, you need to collect a key");
			}
        }
        else if (Input.GetKeyUp(KeyCode.K))
        {
            key.GetComponent<Collider2D>().enabled = false;
            key.GetComponent<Renderer>().enabled = false;
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
			pc.ToggleZoom ();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            maze.ToggleDoneNodesMarks();
        }

    }
}
