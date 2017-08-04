using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombExplosion : MonoBehaviour {

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "BreakableWall" || other.tag == "Bomb")
        {
            Destroy(other.gameObject);
        }
    }
}
