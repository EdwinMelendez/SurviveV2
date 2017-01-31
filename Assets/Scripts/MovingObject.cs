using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public abstract class MovingObject : MonoBehaviour 
{

	public float moveTime = 0.01f; //quicker move time
	public LayerMask blockingLayer;


	private BoxCollider2D boxCollider;
	private Rigidbody2D rb2D;
	private float inverseMoveTime;


	// Use this for initialization
	protected virtual void Start () 
	{
		boxCollider = GetComponent<BoxCollider2D>();
		rb2D = GetComponent<Rigidbody2D>();
		inverseMoveTime = 1f / moveTime;
			
	}

	protected bool Move (int xDir, int yDir, out RaycastHit2D hit) //moves you one space
	{
		Vector2 start = transform.position;

		Vector2 end = start + new Vector2 (xDir, yDir);

		boxCollider.enabled = false;

		hit = Physics2D.Linecast (start, end, blockingLayer);

		boxCollider.enabled = true;

		if(hit.transform == null)
		{
			StartCoroutine (SmoothMovement (end));
			return true;
		}

		return false;
	}

	protected IEnumerator SmoothMovement (Vector3 end) //if not hitting anything it smoothly moves you to next space
	{
		yield return null;
		float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

		while(sqrRemainingDistance > float.Epsilon)
		{
			Vector3 newPosition = Vector3.MoveTowards (rb2D.position, end, inverseMoveTime * Time.deltaTime);
			rb2D.MovePosition(newPosition);
			sqrRemainingDistance = (transform.position - end).sqrMagnitude;
			yield return null;
		}
	}

	protected virtual void AttemptMove (int xDir, int yDir) //attempt to move 
		//where T : Component
		{
			RaycastHit2D hit;
			bool canMove = Move (xDir, yDir, out hit);

			if(hit.transform == null)
			{
				return;
			}
			///T hitComponent = hit.transform.GetComponent<T>();

			if(!canMove) 
			{
			//&& hitComponent != null)
				OnCantMove(hit.transform);
			}
		}

	protected abstract void OnCantMove(Transform t);
		//where T : Component;

		}
	

