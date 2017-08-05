using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{

    public float velocity = 1f;
	public int backStepsDead;
    public int bombIncrement;
    public int bulletIncrement;
	public GameObject pathMark;

	private GameObject pathGroup;
	private int cNode; // current node
	private int dNode; // destination node
    private int cDir; // direction of the player's face
    private int nBombs;
    private int nBullets;
    private int nKeys;
    private int nGoldenKeys;
    private Maze maze;
    private Animator anim;

    private GameObject canvas;
    private Transform aux;
    private Text txtAmmo;
    private Text txtBomb;
    private Text txtKey;
    private Text txtGoldenKey;
    private AudioSource audSource;
    private AudioClip clip;
	private Stack<PathStep> path = new Stack<PathStep>();
	private bool updatePath = true;

    // Use this for initialization
    void Start()
    {
		pathGroup = GameObject.Find("PathGroup");
        canvas = GameObject.FindGameObjectWithTag("Canvas");
        aux = canvas.transform.Find("txtAmmo");
        txtAmmo = aux.GetComponent<Text>();
        aux = canvas.transform.Find("txtBomb");
        txtBomb = aux.GetComponent<Text>();
        aux = canvas.transform.Find("txtKey");
        txtKey = aux.GetComponent<Text>();
        aux = canvas.transform.Find("txtGoldenKey");
        txtGoldenKey = aux.GetComponent<Text>();

        audSource = this.GetComponent<AudioSource>();

        maze = GameObject.Find("GameController").GetComponent<Maze>();
        anim = GetComponent<Animator>();
        cNode = maze.initialNode;
        dNode = cNode;
        cDir = 0;
        nBombs = 0;
        nBullets = 0;
        nKeys = 0;
        nGoldenKeys = 0;
        UpdateCanvas();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Bomb")
        {
            nBombs += bombIncrement;
            Destroy(other.gameObject);
        }
        if (other.tag == "Ammo")
        {
            nBullets += bulletIncrement;
            Destroy(other.gameObject);
        }
        if (other.tag == "Key")
        {
            nKeys++;
            Destroy(other.gameObject);
        }
        if (other.tag == "GoldenKey")
        {
            nGoldenKeys++;
            Destroy(other.gameObject);
        }
        if (other.tag == "Arrow" || other.tag == "Enemy")
        {
			for (int i = Mathf.Min(backStepsDead, path.Count); i > 1; i--) {
				Destroy (path.Pop ().mark);
			}
            Vector3 pos = transform.position;
			pos.x = maze.nodes[path.Peek().node].x;
			pos.y = maze.nodes[path.Peek().node].y;
			dNode = path.Peek().node;
			cNode = path.Peek().node;
            transform.position = pos;
			updatePath = true;
			Destroy (path.Pop ().mark);
        }
        else
        {
            if (other.tag == "GoldenKey" || other.tag == "Bomb" ||
                other.tag == "Ammo" || other.tag == "Key")
            {
                UpdateCanvas();
                audSource.Stop();
                audSource.clip = maze.sounds[0];
                audSource.Play();
            }
        }
    }

    private void FixedUpdate()
    {
        Vector3 pos = transform.position;
        // make the move
        if (dNode != cNode)
        {
            pos.x += Mathf.Clamp(maze.nodes[dNode].x - pos.x, -velocity, velocity);
            pos.y += Mathf.Clamp(maze.nodes[dNode].y - pos.y, -velocity, velocity);
            transform.position = pos;
			if (transform.position.x == maze.nodes [dNode].x && transform.position.y == maze.nodes [dNode].y) {
				if (updatePath) {
					if (path.Count == 0 || path.Peek ().node != dNode) {
						float xPath = (maze.nodes [cNode].x + maze.nodes [dNode].x) / 2f;
						float yPath = (maze.nodes [cNode].y + maze.nodes [dNode].y) / 2f;
						float wPath = Mathf.Abs (maze.nodes [cNode].x - maze.nodes [dNode].x) + 0.01f;
						float hPath = Mathf.Abs (maze.nodes [cNode].y - maze.nodes [dNode].y) + 0.01f;
						GameObject mark = Instantiate (pathMark, new Vector3 (xPath, yPath, 0f), Quaternion.identity, pathGroup.transform);
						mark.transform.localScale = new Vector3 (wPath, hPath, 0f);
						mark.name = "Node" + cNode.ToString ();
						path.Push (new PathStep (cNode, mark));
					}
				}
				cNode = dNode;
				updatePath = true;
			} else {
				if (path.Count > 0 && path.Peek ().node == dNode) {
					Destroy (path.Pop ().mark);
					updatePath = false;
				}
			}
		} 
    }

    private void Update()
    {
        int nDir = -1;
        if (cNode == dNode)
        {
            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
                nDir = 0;
            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
                nDir = 1;
            if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
                nDir = 2;
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
                nDir = 3;
            if (nDir == -1)
            {
                anim.SetBool("PlayerIsWalking", false);
                audSource.Stop();
            }
        }
        if (nDir != -1)
        {
            if (nDir != cDir)
            {
                float newAngle = 0;
                switch (nDir)
                {
                    case 0:
                        newAngle = 0;
                        break;
                    case 1:
                        newAngle = 270;
                        break;
                    case 2:
                        newAngle = 180;
                        break;
                    case 3:
                        newAngle = 90;
                        break;
                }

                transform.eulerAngles = new Vector3(0, 0, newAngle);
                cDir = nDir;
            }

            if (maze.nodes[cNode].links[nDir] != -1 && maze.nodes[cNode].obstacles[nDir] == ' ')
            {
                dNode = maze.nodes[cNode].links[nDir];
                anim.SetBool("PlayerIsWalking", true);

                audSource.clip = maze.sounds[1];

                if (!audSource.isPlaying)
                {
                    audSource.Play();
                }
            }
        }
    }

    public bool UseAmmo()
    {
        if (nBullets > 0)
        {
            nBullets--;
            UpdateCanvas();
            return true;
        }
        return false;
    }

    public bool TestBomb()
    {
        if (nBombs > 0)
        {
            return true;
        }
        return false;
    }

	public int GetNode()
	{
		return dNode;
	}

    public bool onBombSpot()
    {
        return (maze.nodes[dNode].obstacles[0] == 'W' || maze.nodes[dNode].obstacles[1] == 'W' ||
            maze.nodes[dNode].obstacles[2] == 'W' || maze.nodes[dNode].obstacles[3] == 'W');
    }

    public void UseBomb()
    {
        nBombs--;
        UpdateCanvas();
    }

    public bool TestKey()
    {
        if (nKeys > 0)
        {
            return true;
        }
        return false;
    }

    public bool UseKey()
    {
        if (nKeys > 0)
        {
            if (maze.nodes[dNode].obstacles[cDir] == 'D')
            {
                int oDir = (cDir + 2) % 4;
                maze.nodes[dNode].obstacles[cDir] = ' ';
                maze.nodes[maze.nodes[dNode].links[cDir]].obstacles[oDir] = ' ';
            }
            nKeys--;
            UpdateCanvas();
            return true;
        }
        return false;
    }

    public bool TestGoldenKey()
    {
        if (nGoldenKeys > 0)
        {
            return true;
        }
        return false;
    }

    public bool UseGoldenKey()
    {
        if (nGoldenKeys > 0)
        {
            if (maze.nodes[dNode].obstacles[cDir] == 'G')
            {
                int oDir = (cDir + 2) % 4;
                maze.nodes[dNode].obstacles[cDir] = ' ';
                maze.nodes[maze.nodes[dNode].links[cDir]].obstacles[oDir] = ' ';
            }
            nGoldenKeys--;
            UpdateCanvas();
            return true;
        }
        return false;
    }

    private void UpdateCanvas()
    {
        txtAmmo.text = nBullets.ToString();
        txtBomb.text = nBombs.ToString();
        txtKey.text = nKeys.ToString();
        txtGoldenKey.text = nGoldenKeys.ToString();
    }
}

public class PathStep
{
	public GameObject mark;
	public int node;

	public PathStep(int n) {
		node = n;
	}

	public PathStep(int n, GameObject t) {
		node = n;
		mark = t;
	}

}
