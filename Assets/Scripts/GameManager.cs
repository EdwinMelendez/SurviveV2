
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public class GameManager : MonoBehaviour //most of the code here is from https://www.youtube.com/playlist?list=PLX2vGYjWbI0SKsNH5Rkpxvxr1dPE0Lw8F
{
	public float levelStartDelay = 2f; //Variables
	public float turnDelay = 0.2f;
	public int playerFoodPoints = 100;
	public int knifeCount = 1; //new
	public static GameManager instance = null;


	[HideInInspector] public bool playersTurn = true; 

	public BoardManager boardScript;	
	 Text levelText;
	 Text knifeText; //new
     GameObject levelImage;
	 public int level = 1;
	 List<Enemy> enemies;
	 bool enemiesMoving;
	 bool doingSetup = true;

	// Use this for initialization
	void Awake () //makes sure there is only one instance of the game manager, otherwise destroys the copies
	{
		if (instance == null)
			instance = this;

		else if (instance != this)
			Destroy(gameObject);
		
		

		DontDestroyOnLoad(gameObject);
		enemies = new List<Enemy>();
		boardScript = GetComponent<BoardManager>();
		InitGame();
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
	static public void CallbackInitialization()
	{
	 	SceneManager.sceneLoaded += OnSceneLoaded;
	 	
	}

	static private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1) //loads each new level
	{
		instance.level++;
		instance.InitGame();

	}
	 
	public void GetKnife(GameObject gameObject) //this code doesn't quite work but is meant to accumulate when you grab knife object
	{
		if (knifeCount < 3)
		{
			knifeCount++;
			knifeText.text = knifeCount.ToString(); //this code is derived from https://github.com/clcreations/2DRogueLike
		}

		gameObject.SetActive(false);
	}

	public bool UseKnife(Enemy enemy) //use to attack and kill enemy
	{
		if (knifeCount > 0 && enemy.alive)
		{
			knifeCount--;

			knifeText.text = knifeCount.ToString();
			return true;
		}
		return false;
	}

	//void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
	//{
	//	level++;
	//	InitGame();
	//}

	//void OnEnable()
	//{
	//	SceneManager.sceneLoaded += OnLevelFinishedLoading;
	//}

	//void OnDisable()
	//{
	//	SceneManager.sceneLoaded -= OnLevelFinishedLoading;
	//}

	void InitGame() //sets up game board each level and resets everything 
	{
		doingSetup = true;

		levelImage = GameObject.Find("LevelImage");
		levelText = GameObject.Find("LevelText").GetComponent<Text>();
		knifeText = GameObject.Find("knifeText").GetComponent<Text>();
		levelText.text = "Day " + level;
		knifeText.text = knifeCount.ToString();
		levelImage.SetActive(true);
		Invoke("HideLevelImage", levelStartDelay);

		enemies.Clear();
		boardScript.SetupScene(level);
	}

	private void HideLevelImage() //hides the level image
	{
		levelImage.SetActive(false);
		doingSetup = false;
	}


	
	// Update is called once per frame
	void Update () 
	{
		

		if (playersTurn ||doingSetup || enemiesMoving) //calls this once per frame
														//if any of these are true starts coroutine
			return;
		



		StartCoroutine(MoveEnemies ());
	}

	public void AddEnemyToList (Enemy script)		//adds new enemies to game board
	{
		enemies.Add (script);

	}

	public void GameOver() //game over screen
	{
		levelText.text = "After " + level + " days, you starved.";
		levelImage.SetActive(true);
		enabled = false;
	}

	IEnumerator MoveEnemies() //moves enemies
	{

		
		//yield return null;
	 	enemiesMoving = true;
	 	yield return new WaitForSeconds(turnDelay);

	 	//if (enemies.Count == 0)
	 	//{
	 	//	yield return new WaitForSeconds(turnDelay);
	 	//}

	 	//for (int i = 0; i < enemies.Count; i++)
	 	//{
	 	//	enemies[i].MoveEnemy();
	 	//	yield return new WaitForSeconds(enemies[i].moveTime);
	 	//}

	 	foreach(Enemy enemy in enemies)
	 	{
	 	enemy.MoveEnemy();
	 	yield return new WaitForSeconds(enemy.moveTime);
	 	}



	 	playersTurn = true;

	 	enemiesMoving = false;
	}
}

