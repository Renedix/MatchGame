using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid : MonoBehaviour {

    Dictionary<PieceType, GameObject> piecePrefabDictionary;

    public enum PieceType
    {
        NORMAL,
        COUNT,
    };

    [System.Serializable]
    public struct PiecePrefab {
        public GameObject gameObject;
        public PieceType pieceType;
    };

    public int xDim;
    public int yDim;
    public PiecePrefab[] prefabArray;
    public GameObject backgroundPrefab;
    

	void Start () {
        piecePrefabDictionary = new Dictionary<PieceType, GameObject>();

        foreach (PiecePrefab piece in prefabArray)
        {
            if (!piecePrefabDictionary.ContainsKey(piece.pieceType))
            {
                piecePrefabDictionary.Add(piece.pieceType, piece.gameObject);
            }
        }

        for(int x = 0; x < xDim; x++)
        {
            for(int y = 0; y < yDim; y++)
            {
                GameObject background = (GameObject) Instantiate(backgroundPrefab,new Vector3(x,y,0),Quaternion.identity);
                background.transform.parent = transform;
            }
        }
	}
}
