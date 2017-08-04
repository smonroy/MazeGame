using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombScript : MonoBehaviour {
    public int intervalExplosion = 3;
    public GameObject explosionAnimation;

    void Start () {
        StartCoroutine(interval());
    }

    IEnumerator interval()
    {
        yield return new WaitForSeconds(intervalExplosion);

        Destroy(this.gameObject);

        Vector3 spawnPosition = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);
        Instantiate(explosionAnimation, spawnPosition, Quaternion.identity);
    }
}
