using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class Enemy : MovingObject //most of the code here is derived from https://www.youtube.com/playlist?list=PLX2vGYjWbI0SKsNH5Rkpxvxr1dPE0Lw8F
{

	public int playerDamage;
	public AudioClip enemeyAttack1;
	public AudioClip enemyAttack2; // setting up variables
	public float turnDelay = 10f;

	public bool alive = true; //new
	public int eHp = 1;

	 Animator animator;
	 Transform target;
	bool skipMove;

	List<Vector2> goodDirs = new List<Vector2>(); 
    List<Vector2> badDirs = new List<Vector2>();




	protected override void Start () 
	{

		GameManager.instance.AddEnemyToList(this);
		animator = GetComponent<Animator>();
		target = GameObject.FindGameObjectWithTag ("Player").transform;

		base.Start();
	}


	protected override void AttemptMove (int xDir, int yDir) //this is derived from https://github.com/clcreations/2DRogueLike
	{

		
		goodDirs.Clear(); //this is for better enemy ai movement
         badDirs.Clear();
         if (xDir == -1 && yDir == -1){
             goodDirs.Add(Vector2.down);
             goodDirs.Add(Vector2.left);
             badDirs.Add(Vector2.up);
             badDirs.Add(Vector2.right);
         }else if (xDir == 0 && yDir == -1){
             goodDirs.Add(Vector2.down);
             badDirs.Add(Vector2.left);
             badDirs.Add(Vector2.right);
         }else if (xDir == -1 && yDir == 0){
             goodDirs.Add(Vector2.left);
             badDirs.Add(Vector2.down);
             badDirs.Add(Vector2.up);
         }else if (xDir == 1 && yDir == 1){
             goodDirs.Add(Vector2.up);
             goodDirs.Add(Vector2.right);
             badDirs.Add(Vector2.left);
             badDirs.Add(Vector2.down);
         }else if (xDir == 1 && yDir == 0){
             goodDirs.Add(Vector2.right);
             badDirs.Add(Vector2.up);
             badDirs.Add(Vector2.down);
         }else if (xDir == 0 && yDir == 1){
             goodDirs.Add(Vector2.up);
             badDirs.Add(Vector2.up);
             badDirs.Add(Vector2.right);
         }else if (xDir == 1 && yDir == -1){
             goodDirs.Add(Vector2.right);
             goodDirs.Add(Vector2.down);
             badDirs.Add(Vector2.up);
             badDirs.Add(Vector2.left);
         }else if (xDir == -1 && yDir == 1){
             goodDirs.Add(Vector2.up);
             goodDirs.Add(Vector2.left);
             badDirs.Add(Vector2.right);
             badDirs.Add(Vector2.down);
         }
         FindBestMove();
		//base.AttemptMove(xDir, yDir);


	}

	public int Comparison(float a, float b){
         if (a > b) return 1;
         if (a < b) return -1;
         return 0;
         //if (a - b < float.Epsilon) return 1;
         //if (a - b > float.Epsilon) return -1;
         //return 0;
      }

	void FindBestMove(){ //https://github.com/clcreations/2DRogueLike
         for(int i = 0; i < goodDirs.Count; i++){
            Vector2 temp = goodDirs[i];
             int randomIndex = Random.Range(i, goodDirs.Count);
             goodDirs[i] = goodDirs[randomIndex];
             goodDirs[randomIndex] = temp;
         }
         for(int i = 0; i < badDirs.Count; i++){ //determines best move for enemy
             Vector2 temp = badDirs[i];
             int randomIndex = Random.Range(i, badDirs.Count);
             badDirs[i] = badDirs[randomIndex];
             badDirs[randomIndex] = temp;
         }
         foreach(Vector2 dir in goodDirs){
             if (CheckMove(dir)) return;
         }
         foreach(Vector2 dir in badDirs){
             if (CheckMove(dir)) return;
          }
     }

	public void MoveEnemy()
	{  if (skipMove) //determines if skip move while player moved
		{
			skipMove = false;


			return;
		}
		else if (!alive) //I added a little here although I'm not sure if it works. Runs the die() if alive is false
		{
			die();
			//skipMove = true;
			return;
		}
			
			int newX = Comparison(target.position.x, transform.position.x);
	        int newY = Comparison(target.position.y, transform.position.y);
            AttemptMove(newX, newY);//int xDir = 0;
		//int yDir = 0;

		//if (Mathf.Abs (target.position.x - transform.position.x) < float.Epsilon)
		//	yDir = target.position.y > transform.position.y ? 1 : -1;
		//else
		//	xDir = target.position.x > transform.position.x ? 1 : -1;

	skipMove = true;

	//AttemptMove <Player> (xDir, yDir);
	}

	public void die() //this area needs work. I tried to trigger an animation which flashes but then resets enemyIdle animation
	{


	 alive = false; //is meant to turn off enemy and hide them in another game object layer
	 gameObject.SetActive(false);
	//animator.SetTrigger("enemyDie");
	gameObject.layer = 0;
		//Enemy.DestroyObject(this);
		//yield return new WaitForSeconds(turnDelay);
		//GameManager.instance.AddEnemyToList(this);

	//	if (alive)
	//	{
	//	StartCoroutine(dead());
	//	}
	}

	//IEnumerator dead ()
	//{
	//	alive = false;
	//	yield return new WaitForSeconds(turnDelay);
	//	alive = true;
	//}

	bool CheckMove(Vector2 checking) //checks move
	{
		


		Vector2 start = transform.position;
		Vector2 end = start + checking;
		RaycastHit2D hit;

		GetComponent<BoxCollider2D>().enabled = false;
		hit = Physics2D.Linecast(start, end, blockingLayer);
		GetComponent<BoxCollider2D>().enabled = true;

		if (hit.transform == null)
		{
			Debug.Log("Active? "+gameObject.activeInHierarchy); //this was my biggest issue in the code 
																//anytime i killed an enemy it would register as not active thus breaking the game
			 //return new WaitForSeconds(turnDelay);			
			 //i added in this next line to prevent the game from breaking
			 //but it reactivates the enemy after a few seconds. 
			 //this is a work around for now but still allows for passable levels in case the player gets trapped.e
			gameObject.SetActive(true);


			StartCoroutine(SmoothMovement(end));
			return true;



		}
		else if (hit.transform.GetComponent<Player>())
		{
			OnCantMove(hit.transform);
			return true;
		}
		return false;
	}

	protected override void OnCantMove (Transform t) //player loses food when hit by enemy if enemy cant move in direction of player
	{
		Player player = t.GetComponent<Player>(); //esentially when the enemy runs into player it attacks
		//hitPlayer.LoseFood(playerDamage);
		if (player)
		{
		animator.SetTrigger("enemyAttack");
		SoundManager.instance.RandomizeSfx(enemeyAttack1,enemyAttack2);
		player.LoseFood(playerDamage);


		}

	}
}
