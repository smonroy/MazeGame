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

    // Use this for initialization
    void Start()
    {
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
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.J))
        {
            if (pc.UseAmmo())
            {
                GameObject bullet = Instantiate(bulletPrefab, shootLoc.position, Quaternion.identity);
                bullet.GetComponent<Rigidbody2D>().velocity = transform.up * bulletSpeed;
            }
        }
		else if (Input.GetKey(KeyCode.L) || Input.GetKey(KeyCode.B)) // Lay bomb
        {
            if (pc.onBombSpot())
            {
                if (pc.TestBomb())
                {
                    GameObject bomb = Instantiate(bombSprite, transform.position, Quaternion.identity);
					bomb.GetComponent<BombScript> ().setNode(pc.GetNode ());
                }
            }
        }
        else if (Input.GetKey(KeyCode.K)) // Use key
        {
            if (pc.TestKey() || pc.TestGoldenKey())
            {
                keyColl.enabled = true;
                keyRend.enabled = true;
            }
        }
        else if (keyColl.enabled)
        {
            key.GetComponent<Collider2D>().enabled = false;
            key.GetComponent<Renderer>().enabled = false;
        }
    }
}
