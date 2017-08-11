using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowDestroy : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (gameObject.tag.Equals("Arrow"))
        {
            Destroy(this.gameObject);
        }
    }
}
