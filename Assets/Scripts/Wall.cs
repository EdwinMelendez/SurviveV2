using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour {

	public AudioClip chopSound1; 
	public AudioClip chopSound2;
	public Sprite dmgSprite;
	public Sprite food;
	public int hp = 3;
	public int ehp = 1;


	private SpriteRenderer spriteRenderer;

	// Use this for initialization
	void Awake () 
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
	}
	
	public void DamageWall (int loss) //allows walls to be destroyed
	{
		SoundManager.instance.RandomizeSfx(chopSound1, chopSound2);
		 spriteRenderer.sprite = dmgSprite;
		 hp -= loss;
		 if (hp <= 0)
		 	gameObject.SetActive(false); 
	}

	public void DamageEnemy (int loss) //I added this to be able to damage enemies and kill them
	{
		SoundManager.instance.RandomizeSfx(chopSound1, chopSound2);
		spriteRenderer.sprite = food;
		ehp -= loss;
		if (ehp <= 0)
		gameObject.SetActive(false);
	}
}

