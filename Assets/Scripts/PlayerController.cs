using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
	public int healthIncrement;

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
	private Text txtCentralMessage;
	private Text txtBottomMessage;
    private AudioSource audSource;
    private AudioClip clip;
	private Stack<PathStep> path = new Stack<PathStep>();
	private bool updatePath = true;
	private int enemiesKilled;
	private int health;
	private GameObject healthBar;
	private GameObject healthBarRed;
	private GameObject healthBarGreen;
	private bool fastReturn;
	private bool winTheGame;
	private bool gameOver;

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
		aux = canvas.transform.Find("txtCentralMessage");
		txtCentralMessage = aux.GetComponent<Text>();
		aux = canvas.transform.Find("txtBottomMessage");
		txtBottomMessage = aux.GetComponent<Text>();

        audSource = this.GetComponent<AudioSource>();
		healthBar = transform.GetChild(2).gameObject;
		healthBarRed = healthBar.transform.GetChild(0).gameObject;
		healthBarGreen = healthBarRed.transform.GetChild (0).gameObject;
		health = initialHealth;
		UpdateHealthBar ();

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
		maze.SetDone (cNode);
        UpdateCanvas();
		txtBottomMessage.text = "Welcome, find the exit of the maze!";
		txtCentralMessage.text = "";
		fastReturn = false;
		winTheGame = false;
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
		if (other.tag == "Medkit")
		{
			health += healthIncrement;
			if(health > totalHealth) {
				health = totalHealth;
			}
			Destroy(other.gameObject);
			maze.SetDone(dNode);
			audSource.Stop();
			audSource.clip = maze.sounds[0];
			audSource.Play();
			UpdateHealthBar ();
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
			if (!fastReturn && !winTheGame) {
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
				health -= 1;
				if (other.tag == "Enemy") {
					Destroy (other.gameObject);
					enemyKilled ();
					if (nBullets == 0) {
						health -= 2;
					}
				}
				if (health < 0) {
					health = 0;
				}
				if (health == 0) {
					GameOver ();
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
			if (fastReturn) {
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
                        mark.transform.localScale = new Vector3(wPath, hPath, 0.1f);
                        mark.name = "Node" + cNode.ToString();
                        path.Push(new PathStep(cNode, mark));
                    }
                }
                cNode = dNode;
                updatePath = true;
				if (!maze.nodes[cNode].done) {
					if (maze.SetDone (cNode)) {
						UpdateCanvas ();
					}
				}
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

	private void GameOver() {
		txtCentralMessage.text = "Game Over!";
		txtBottomMessage.text = "Press 'R' to restart";
		gameOver = true;
	}

	private void WinTheGame() {
		txtCentralMessage.text = "Congratulations!";
		txtBottomMessage.text = "Press 'R' to restart";
		winTheGame = true;
	}

	private void RestartTheGame() {
		SceneManager.LoadScene (0);
	}

    private void Update()
    {
		if ((gameOver || winTheGame) && Input.GetKeyDown (KeyCode.R)) {
			RestartTheGame ();
		}

		if (gameOver) {
			return;
		}

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
			if (Input.GetKeyDown (KeyCode.F)) {
				if (maze.nodes[cNode].done && path.Count > 0) {
					fastReturn = true;
				}
			}
			if (!Input.GetKey (KeyCode.F) || !maze.nodes[cNode].done || path.Count == 0) {
				fastReturn = false;
			}

			if (Input.GetKeyDown(KeyCode.F10)) {
				if(nGoldenKeys == 0){
					nGoldenKeys = 1;
				}
				Vector3 pos = transform.position;
				pos.x = maze.nodes [31].x;
				pos.y = maze.nodes [31].y;
				dNode = 31;
				cNode = 31;
				transform.position = pos;
				UpdateCanvas ();
			}
				
			if (fastReturn)
            {
				bool stop = false;
				for (int i = 0; i < 4 && !stop; i++)
            	{
					if (maze.nodes [cNode].links [i] != -1) {
						if (maze.nodes [cNode].links [i] == path.Peek ().node) {
							if (!maze.nodes [maze.nodes [cNode].links [i]].done) {
								stop = !maze.SetDone (maze.nodes [cNode].links [i]);
							}
							if (!stop) {
								nDir = i;
							}
						} else {
							if (!maze.nodes [maze.nodes [cNode].links [i]].done) {
								stop = true;
								nDir = -1;
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
				if (maze.nodes[cNode].links[nDir] != -1) {
					if (maze.nodes [cNode].obstacles [nDir] == ' ') {
						dNode = maze.nodes [cNode].links [nDir];
						anim.SetBool ("PlayerIsWalking", true);
						if (!winTheGame) {
							txtBottomMessage.text = "";
						}
						audSource.clip = maze.sounds [1];

						if (!audSource.isPlaying) {
							audSource.Play ();
						}
					} 
					if (maze.nodes [cNode].obstacles [nDir] == 'W') {
						if (nBombs == 0) {
							txtBottomMessage.text = "You need a bomb to pass through this breakable wall.";
						} else {
							txtBottomMessage.text = "You can use a bomb with 'B' to pass through this breakable wall.";
						}
					}
					if (maze.nodes [cNode].obstacles [nDir] == 'D') {
						if (nKeys == 0) {
							txtBottomMessage.text = "You need a key to pass through this door.";
						} else {
							txtBottomMessage.text = "You can use a key with 'k' to pass through this door.";
						}
					}
					if (maze.nodes [cNode].obstacles [nDir] == 'G') {
						if (nGoldenKeys == 0) {
							txtBottomMessage.text = "You need a golden key to pass through this door.";
						} else {
							txtBottomMessage.text = "You can use a golden key with 'k' to pass through this door.";
						}
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
			nGoldenKeys--;
			UpdateCanvas();
            if (maze.nodes[dNode].obstacles[cDir] == 'G')
            {
				int oDir = (cDir + 2) % 4;
				maze.nodes [dNode].obstacles [cDir] = ' ';
				maze.nodes [maze.nodes [dNode].links [cDir]].obstacles [oDir] = ' ';
				maze.SetDone (maze.nodes [dNode].links [cDir]);
				if (maze.nodes [dNode].row<5) {
					WinTheGame ();
				}
            }
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
