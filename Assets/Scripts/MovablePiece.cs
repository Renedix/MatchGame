using UnityEngine;
using System.Collections;

public class MovablePiece : MonoBehaviour {

    GamePiece gamePiece;

    void Awake()
    {
        gamePiece = GetComponent<GamePiece>();
    }


    public void Move(int newX, int newY)
    {
        gamePiece.X = newX;
        gamePiece.Y = newY;

        gamePiece.transform.localPosition = gamePiece.GridRef.GetWorldPosition(newX, newY);
    }

}
