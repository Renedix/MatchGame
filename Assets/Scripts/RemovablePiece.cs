using UnityEngine;
using System.Collections;

public class RemovablePiece : MonoBehaviour {

    public AnimationClip removeAnimation;
    public bool IsBeingRemoved = false;

    protected GamePiece gamePiece;

    void Awake()
    {
        this.gamePiece = GetComponent<GamePiece>();
    }

    public void ClearPiece()
    {
        IsBeingRemoved = true;
        StartCoroutine(RemovePieceCoroutine());
    }


    IEnumerator RemovePieceCoroutine()
    {
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.Play(removeAnimation.name);

            yield return new WaitForSeconds(removeAnimation.length);

            Destroy(gameObject);
        }
    }

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
