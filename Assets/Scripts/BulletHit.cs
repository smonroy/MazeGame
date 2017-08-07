using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHit : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.tag.Equals("Player"))
        {
            Destroy(gameObject);
            if(other.gameObject.tag.Equals("Enemy"))
            {
                Destroy(other.gameObject);
				PlayerController pc = GameObject.FindWithTag ("Player").GetComponent<PlayerController> ();
				pc.enemyKilled ();
            }
        }
    }
}
