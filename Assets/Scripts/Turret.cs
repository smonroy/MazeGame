using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour {
    public int interval = 3;

    public GameObject arrow;
    // Use this for initialization
    void Start () {
        StartCoroutine(throwArrows());
    }

    IEnumerator throwArrows()
    {
        yield return new WaitForSeconds(interval);
        while (true)
        {
			Vector2 spawnPosition = new Vector2(this.transform.parent.position.x, this.transform.parent.position.y);
            Quaternion spawnRotation = this.transform.rotation;
            Instantiate(arrow, spawnPosition, spawnRotation);

            yield return new WaitForSeconds(interval);
        }
    }

}
