using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MovingObject {

	public float restartLevelDelay = 1f;
	public int wallDamage = 1;
	public int enemyDamage = 1;
	public int pointsPerFood = 10;
	public int pointPerSoda = 20;
	public int pointsPerKnife = 1;

	public Text foodText;
	public Text knifeText;
	public AudioClip moveSound1;
	public AudioClip moveSound2;
	public AudioClip eatSound1;
	public AudioClip eatSound2;
	public AudioClip drinkSound1;
	public AudioClip drinkSound2;
	public AudioClip gameOverSound;

	private Animator animator;
	private int food;
	private int knife;
	
	// Use this for initialization
	protected override void Start () //keeps track of food count
	{
		animator = GetComponent<Animator>();

			food = GameManager.instance.playerFoodPoints;

			foodText.text = "Food: " + food;

			base.Start();	
	}

	private void OnDisable()
	{
		GameManager.instance.playerFoodPoints = food;
	}
	
	// Update is called once per frame
	void Update ()  //sets up players turn every frame
	{
		if (!GameManager.instance.playersTurn) return;

		int horizontal = 0;
		int vertical = 0;

		horizontal = (int) (Input.GetAxisRaw("Horizontal"));
		vertical = (int) (Input.GetAxisRaw("Vertical"));

		if (horizontal != 0)
			{
				vertical =0;	
			}

		if (horizontal != 0 || vertical != 0)
			{
				AttemptMove (horizontal, vertical);
			}
	}

	protected override void AttemptMove(int xDir, int yDir) //every step loses one food, main mechanic of the game
	{
		food--;
		foodText.text = "Food: " + food;

		base.AttemptMove(xDir, yDir);

		RaycastHit2D hit;

		if (Move (xDir, yDir, out hit))
		{
			SoundManager.instance.RandomizeSfx(moveSound1, moveSound2); //move sounds are randomized
		}
		CheckIfGameOver();

		GameManager.instance.playersTurn = false;
	}

	//protected override void OnCantMove <T> (T component)
	//{
	//	Wall hitWall = component as Wall;
	//	hitWall.DamageWall(wallDamage);
	//	animator.SetTrigger("playerChop");
	//}

	protected override void OnCantMove (Transform t) //if you run into an enemy or a wall
	{
	Debug.Log("Running into " +t);
		Wall wall = t.GetComponent<Wall>();
		Enemy enemy = t.GetComponent<Enemy>();


		if (wall) //damages wall
		{
			Debug.Log("Running into Wall");
			wall.DamageWall(wallDamage);
			animator.SetTrigger("playerChop");
		}
		else if(enemy) //I edited this a bit so that enemies that take damage die
		{
			Debug.Log("Running into Enemy");
			//wall.DamageEnemy(enemyDamage);

			if (GameManager.instance.UseKnife(enemy))
			{
			animator.SetTrigger("playerChop");	
			enemy.die();

			}
		}
	
	}

	private void OnTriggerEnter2D (Collider2D other)// lets you pick up food items
	{
		if (other.tag == "Exit")
		{
			Invoke("Restart", restartLevelDelay);
			enabled = false;
		}
		else if (other.tag == "Food")
		{
			food += pointsPerFood;
			foodText.text = "+" + pointsPerFood + " Food: " + food;
			SoundManager.instance.RandomizeSfx(eatSound1,eatSound2);
			other.gameObject.SetActive(false);
		}
		else if (other.tag == "Soda")
		{
		food += pointPerSoda;
		foodText.text = "+" + pointPerSoda + " Food: " + food;
		SoundManager.instance.RandomizeSfx(drinkSound1, drinkSound2);
		other.gameObject.SetActive(false);
		}
		else if (other.tag == "Knife") //this doesn't really work and I'm not sure why
		{
		//knife += pointsPerKnife;
		//knifeText.text = "+" + pointsPerKnife + " Knife: " + knife;
		GameManager.instance.GetKnife(other.gameObject);

		}
	}



	private void Restart()
	{
		SceneManager.LoadScene (SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
	}

	public void LoseFood (int loss) //everytime you lose food it checks if game is over
	{
		animator.SetTrigger("playerHit");
		food -= loss;
		foodText.text = "-" + loss + " Food: " + food;
		CheckIfGameOver();
	}

	private void CheckIfGameOver() //if food is lower than 0 then game is over
	{
		if (food <= 0)
		{
		SoundManager.instance.PlaySingle(gameOverSound);
		SoundManager.instance.musicSource.Stop();
		GameManager.instance.GameOver();
		}
	}
}
