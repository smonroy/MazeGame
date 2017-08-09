using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{

    public float velocity = 1f;
	public float returnVelocity = 1f;
    public float rotationVelocity = 1f;
    public int backStepsDead;
    public int bombIncrement;
    public int bulletIncrement;
	public GameObject pathMark;
	public AudioClip[] sounds; // 0 - collect, 1 - footsteps
	public int initialHealth;
	public int totalHealth;

    private GameObject pathGroup;
    private int cNode; // current node
    private int dNode; // destination node
    private int cDir; // direction of the player's face
    private float cAngle;
    private float nAngle;
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
    private Text txtDoneNodes;
    private Text txtEnemiesKilled;
    private AudioSource audSource;
    private AudioClip clip;
	private Stack<PathStep> path = new Stack<PathStep>();
	private bool updatePath = true;
	private int enemiesKilled;
	public int health;
	private GameObject healthBar;
	private GameObject healthBarRed;
	private GameObject healthBarGreen;

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
        aux = canvas.transform.Find("txtDoneNodes");
        txtDoneNodes = aux.GetComponent<Text>();
        aux = canvas.transform.Find("txtEnemiesKilled");
        txtEnemiesKilled = aux.GetComponent<Text>();

        audSource = this.GetComponent<AudioSource>();
		healthBar = transform.GetChild(2).gameObject;
		healthBarRed = healthBar.transform.GetChild(0).gameObject;
		healthBarGreen = healthBarRed.transform.GetChild (0).gameObject;

        maze = GameObject.Find("GameController").GetComponent<Maze>();
        anim = GetComponent<Animator>();
        anim.SetBool("PlayerIsWalking", false);
        cNode = maze.initialNode;
        dNode = cNode;
        cDir = 0;
        nBombs = 0;
        nBullets = 0;
        nKeys = 0;
        cAngle = 0;
        nAngle = 0;
        nGoldenKeys = 0;
		enemiesKilled = 0;
		health = initialHealth;
		maze.SetDone (cNode);
        UpdateCanvas();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Bomb")
        {
            nBombs += bombIncrement;
            Destroy(other.gameObject);
            maze.SetDone(dNode);
            audSource.Stop();
            audSource.clip = maze.sounds[0];
            audSource.Play();
        }
        if (other.tag == "Ammo")
        {
            nBullets += bulletIncrement;
            Destroy(other.gameObject);
            maze.SetDone(dNode);
            audSource.Stop();
            audSource.clip = maze.sounds[2];
            audSource.Play();
        }
        if (other.tag == "Key")
        {
            nKeys++;
            Destroy(other.gameObject);
            maze.SetDone(dNode);
            audSource.Stop();
            audSource.clip = maze.sounds[3];
            audSource.Play();
        }
        if (other.tag == "GoldenKey")
        {
            nGoldenKeys++;
            Destroy(other.gameObject);
            maze.SetDone(dNode);
            audSource.Stop();
            audSource.clip = maze.sounds[3];
            audSource.Play();
        }
        if (other.tag == "Arrow" || other.tag == "Enemy" || other.tag == "Explosion")
        {
			if (!Input.GetKey (KeyCode.R)) {
				for (int i = Mathf.Min (backStepsDead, path.Count); i > 1; i--) {
					Destroy (path.Pop ().mark);
				}
				Vector3 pos = transform.position;
				pos.x = maze.nodes [path.Peek ().node].x;
				pos.y = maze.nodes [path.Peek ().node].y;
				dNode = path.Peek ().node;
				cNode = path.Peek ().node;
				transform.position = pos;
				updatePath = true;
				Destroy (path.Pop ().mark);
				maze.SetDone (cNode);
				if (other.tag == "Enemy") {
					health -= 2;
					Destroy (other.gameObject);
					enemyKilled ();
				} else {
					health -= 1;
				}
				UpdateHealthBar ();
			}
		}
        else
        {
            if (other.tag == "GoldenKey" || other.tag == "Bomb" ||
                other.tag == "Ammo" || other.tag == "Key")
            {
                UpdateCanvas();
            }
        }
    }

    private void FixedUpdate()
    {
		if (cAngle != nAngle)
		{
			Vector3 ang = transform.eulerAngles;
			float angleDif = nAngle - ang.z;
			if (angleDif > 180f) {
				angleDif -= 360f;
			}
			if (angleDif < -180f) {
				angleDif += 360f;
			}
			ang.z += Mathf.Clamp (angleDif, -rotationVelocity, rotationVelocity);

			transform.eulerAngles = ang;
			ang.z = 180f;
			healthBar.transform.eulerAngles = ang;

			if (transform.eulerAngles.z == nAngle) {
				cAngle = nAngle;
			}
		}

        if (dNode != cNode)
        {
            Vector3 pos = transform.position;
			if (Input.GetKey (KeyCode.R)) {
				pos.x += Mathf.Clamp (maze.nodes [dNode].x - pos.x, -returnVelocity, returnVelocity);
				pos.y += Mathf.Clamp (maze.nodes [dNode].y - pos.y, -returnVelocity, returnVelocity);
			} else {
				pos.x += Mathf.Clamp (maze.nodes [dNode].x - pos.x, -velocity, velocity);
				pos.y += Mathf.Clamp (maze.nodes [dNode].y - pos.y, -velocity, velocity);
			}
            transform.position = pos;
            if (transform.position.x == maze.nodes[dNode].x && transform.position.y == maze.nodes[dNode].y)
            {
                if (updatePath)
                {
                    if (path.Count == 0 || path.Peek().node != dNode)
                    {
                        float xPath = (maze.nodes[cNode].x + maze.nodes[dNode].x) / 2f;
                        float yPath = (maze.nodes[cNode].y + maze.nodes[dNode].y) / 2f;
                        float wPath = Mathf.Abs(maze.nodes[cNode].x - maze.nodes[dNode].x) + 0.01f;
                        float hPath = Mathf.Abs(maze.nodes[cNode].y - maze.nodes[dNode].y) + 0.01f;
                        GameObject mark = Instantiate(pathMark, new Vector3(xPath, yPath, -1f), Quaternion.identity, pathGroup.transform);
                        mark.transform.localScale = new Vector3(wPath, hPath, 0f);
                        mark.name = "Node" + cNode.ToString();
                        path.Push(new PathStep(cNode, mark));
                    }
                }
                cNode = dNode;
                updatePath = true;
                if (maze.SetDone(cNode))
                {
                    UpdateCanvas();
                };
            }
            else
            {
                if (path.Count > 0 && path.Peek().node == dNode)
                {
                    Destroy(path.Pop().mark);
                    updatePath = false;
                }
            }
        }
    }

	private void UpdateHealthBar() {
		healthBarGreen.transform.localScale = new Vector3 (1f * health / totalHealth, 1, 1);
		Vector3 pos = healthBarGreen.transform.localPosition;
		pos.x = (0f + totalHealth - health) / totalHealth / 2f;
		healthBarGreen.transform.localPosition = pos;
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
            if (Input.GetKey(KeyCode.R))
            {
                if (maze.nodes[cNode].done)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        if (path.Count > 0)
                        {
                            if (maze.nodes[cNode].links[i] == path.Peek().node)
                            {
                                nDir = i;
                            }
                        }
                    }
                }
            }
            if (nDir == -1)
            {
                anim.SetBool("PlayerIsWalking", false);
                audSource.Stop();
            }
        }
        if (nDir != -1)
        {

            if (cDir == nDir && cAngle == nAngle)
            {
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
            else
            {
                //				transform.eulerAngles = new Vector3(0, 0, maze.getAngle(nDir));
                cDir = nDir;
                nAngle = maze.getAngle(nDir);
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

    public void enemyKilled()
    {
        enemiesKilled++;
        UpdateCanvas();
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
                maze.SetDone(maze.nodes[dNode].links[cDir]);
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
                maze.SetDone(maze.nodes[dNode].links[cDir]);
            }
            nGoldenKeys--;
            UpdateCanvas();
            return true;
        }
        return false;
    }

    public void UpdateCanvas()
    {
        txtAmmo.text = nBullets.ToString();
        txtBomb.text = nBombs.ToString();
        txtKey.text = nKeys.ToString();
        txtGoldenKey.text = nGoldenKeys.ToString();
        txtDoneNodes.text = maze.GetDoneNodes().ToString();
        txtEnemiesKilled.text = enemiesKilled.ToString();
    }
}

public class PathStep
{
    public GameObject mark;
    public int node;

    public PathStep(int n)
    {
        node = n;
    }

    public PathStep(int n, GameObject t)
    {
        node = n;
        mark = t;
    }

}
