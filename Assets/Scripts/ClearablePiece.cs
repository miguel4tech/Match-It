using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearablePiece : MonoBehaviour
{
    public AnimationClip clearAnimation;
    private bool isBeingCleared;
    protected GamePiece piece;

    public bool IsBeingCleared{
        get { return isBeingCleared;}
    }
    //Being called before the Start, at the very beginning
    void Awake()
    {
        piece = GetComponent<GamePiece>();
    }
    public void Clear()
    {
        isBeingCleared = true;
        StartCoroutine (ClearCoroutine());
    }
    private IEnumerator ClearCoroutine()
    {
        Animator animator = GetComponent<Animator>();
        if(animator)
        {
            animator.Play (clearAnimation.name);
            yield return new WaitForSeconds (clearAnimation.length);

            Destroy (gameObject); //After playing animation, destroys the game objects
        }
    }
}
