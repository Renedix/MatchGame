﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid : MonoBehaviour {

	public enum PieceType
	{
		NORMAL,
		COUNT,
	};

	[System.Serializable]
	public struct PiecePrefab
	{
		public PieceType type;
		public GameObject prefab;
	};

	public int xDim;
	public int yDim;

	public PiecePrefab[] piecePrefabs;
	public GameObject backgroundPrefab;

	private Dictionary<PieceType, GameObject> piecePrefabDict;
    private GameObject[,] gamePieces;

	// Use this for initialization
	void Start () {
		piecePrefabDict = new Dictionary<PieceType, GameObject> ();

		for (int i = 0; i < piecePrefabs.Length; i++) {
			if (!piecePrefabDict.ContainsKey (piecePrefabs [i].type)) {
				piecePrefabDict.Add (piecePrefabs [i].type, piecePrefabs [i].prefab);
			}
		}

        gamePieces = new GameObject[xDim, yDim];
        for (int x = 0; x < xDim; x++) {
			for (int y = 0; y < yDim; y++) {
                // Background
				GameObject background = (GameObject)Instantiate (backgroundPrefab, GetWorldPosition(x, y), Quaternion.identity);
				background.transform.parent = transform;

                // Game Piece
                gamePieces[x, y] = (GameObject)Instantiate(piecePrefabDict[PieceType.NORMAL], GetWorldPosition(x, y), Quaternion.identity);
                gamePieces[x, y].name = "Piece("+x+","+y+")";
                gamePieces[x, y].transform.parent = transform;
            }
		}
    }
	
	Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(this.transform.position.x-xDim/2.0f+x, this.transform.position.y + yDim / 2.0f - y, 0);
    }
}
