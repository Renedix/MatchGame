using UnityEngine;
using System.Collections;

public class MovablePiece : MonoBehaviour {

    GamePiece gamePiece;
    IEnumerator moveCoroutine;

    void Awake()
    {
        gamePiece = GetComponent<GamePiece>();
    }


    public void Move(int newX, int newY, float time)
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }

        moveCoroutine = moveRoutine(newX, newY, time);
        StartCoroutine(moveCoroutine);
    }

    private IEnumerator moveRoutine(int newX, int newY, float time)
    {
        Vector3 originalPosition = gamePiece.transform.position;
        Vector3 endPosition = gamePiece.GridRef.GetWorldPosition(newX, newY);

        gamePiece.X = newX;
        gamePiece.Y = newY;

        for(float t=0;t< 1*time;t+= Time.deltaTime)
        {
            gamePiece.transform.localPosition = Vector3.Lerp(originalPosition, endPosition, t / time);

            yield return 0;
        }

        gamePiece.transform.localPosition = endPosition;
    }

}
