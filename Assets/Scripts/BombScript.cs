using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombScript : MonoBehaviour {
    public int intervalExplosion = 3;
    public GameObject explosionAnimation;

	private int node;
	private Maze maze;

    void Start () {
		maze = GameObject.Find("GameController").GetComponent<Maze>();
		StartCoroutine(interval());
    }

    IEnumerator interval()
    {
        yield return new WaitForSeconds(intervalExplosion);

        Destroy(this.gameObject);

        Vector3 spawnPosition = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);
        Instantiate(explosionAnimation, spawnPosition, Quaternion.identity);
		for (int cDir = 0; cDir < 4; cDir++)
		{
			if (maze.nodes[node].obstacles[cDir] == 'W') {
				int oDir = (cDir + 2) % 4;
				maze.nodes[node].obstacles[cDir] = ' ';
				maze.nodes[maze.nodes[node].links[cDir]].obstacles[oDir] = ' ';
				maze.SetDone (maze.nodes [node].links [cDir]);
			}
		}
    }

	public void setNode (int n)
	{
		node = n;
	}

}
