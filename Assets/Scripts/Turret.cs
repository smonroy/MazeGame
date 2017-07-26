using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour {
    public int interval = 3;
    //public GameObject arrowLeft;
    //public GameObject arrowRight;
    //public GameObject arrowUp;
    //public GameObject arrowDown;

    public GameObject arrow;
    // Use this for initialization
    void Start () {
        StartCoroutine(throwArrows());
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator throwArrows()
    {
        yield return new WaitForSeconds(interval);
        while (true)
        {
            Vector2 spawnPosition = new Vector2(this.transform.position.x, this.transform.position.y);
            Quaternion spawnRotation = this.transform.rotation;
            Instantiate(arrow, spawnPosition, spawnRotation);

            yield return new WaitForSeconds(interval);
        }
    }

}
