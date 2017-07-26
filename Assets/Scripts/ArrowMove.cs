using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowMove : MonoBehaviour {
    public float speed = 5f;
    public GameObject turret;
    
    private Rigidbody2D rBody;

	// Use this for initialization
	void Start () {
        rBody = this.GetComponent<Rigidbody2D>();
        rBody.velocity = -this.transform.right * speed;
    }

    // Update is called once per frame
    void Update () {
		
	}
}
