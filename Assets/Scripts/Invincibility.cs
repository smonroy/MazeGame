using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Invincibility : MonoBehaviour
{
    bool invincible;
    public float invincibleLength = 2;

    public bool Invincible
    {
        get
        {
            return invincible;
        }
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Blink(float waitTime)
    {
        float endTime = Time.time + waitTime;

        while (Time.time < endTime)
        {
            gameObject.GetComponent<Renderer>().enabled = false;
            yield return new WaitForSeconds(0.1f);
            gameObject.GetComponent<Renderer>().enabled = true;
            yield return new WaitForSeconds(0.1f);
        }
    }

    public IEnumerator InvincibleTimer()
    {
        invincible = true;
        StartCoroutine(Blink(invincibleLength));
        yield return new WaitForSeconds(invincibleLength);
        invincible = false;
    }
}
