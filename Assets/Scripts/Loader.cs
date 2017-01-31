using System.Collections;
using System.Collections.Generic; 
using UnityEngine;

public class Loader : MonoBehaviour {

	public GameManager gameManager;
	public SoundManager soundManager;


	void Awake () //loads game
	{
		if (GameManager.instance == null)
			Instantiate(gameManager);
		if (SoundManager.instance == null)
			Instantiate(soundManager);
	}
}
