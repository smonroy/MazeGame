using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    public GameObject bulletPrefab;
    private Transform shootLoc;
    public float bulletSpeed;
    PlayerController pc;

    // Use this for initialization
    void Start()
    {
        pc = GetComponent<PlayerController>();
        shootLoc = transform.GetChild(0);
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("ShootOffset: " + shootLoc.position);
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if (pc.UseAmmo())
            {
                GameObject bullet = Instantiate(bulletPrefab, shootLoc.position, Quaternion.identity);
                bullet.GetComponent<Rigidbody2D>().velocity = transform.up * bulletSpeed;
            }
        }
    }
}
