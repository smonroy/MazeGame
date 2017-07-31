using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    public GameObject bulletPrefab;
    private Transform shootLoc;
    public float bulletSpeed;
    PlayerController pc;
    GameObject key;
    Collider2D keyColl;
    Renderer keyRend;

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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (pc.UseAmmo())
            {
                GameObject bullet = Instantiate(bulletPrefab, shootLoc.position, Quaternion.identity);
                bullet.GetComponent<Rigidbody2D>().velocity = transform.up * bulletSpeed;
            }
        }
        else if (Input.GetKey(KeyCode.K))
        {
            if (pc.TestKey())
            {
                keyColl.enabled = true;
                keyRend.enabled = true;
            }
        }
        else if(keyColl.enabled)
        {
            key.GetComponent<Collider2D>().enabled = false;
            key.GetComponent<Renderer>().enabled = false;
        }
    }
}
