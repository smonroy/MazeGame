using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpen : MonoBehaviour
{
    PlayerController pc;

    // Use this for initialization
    void Start()
    {
        pc = transform.parent.GetComponent<PlayerController>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag.Equals("Door"))
        {
            if (pc.UseKey())
            {
                Destroy(other.gameObject);
            }
        }
        else if(other.gameObject.tag.Equals("GoldenDoor"))
        {
            if (pc.UseGoldenKey())
            {
                Destroy(other.gameObject);
            }
        }
    }
}
