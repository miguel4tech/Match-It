using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgGrid : MonoBehaviour
{
    public int xDim;
    public int yDim;

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

    public PiecePrefab[] piecePrefabs;
    public GameObject backgroundPrefabs;


    private Dictionary<PieceType, GameObject> piecePrefabDict;
    // Start is called before the first frame update
    void Start()
    {
        piecePrefabDict = new Dictionary<PieceType, GameObject>();

        for (int i = 0; i < piecePrefabs.Length; i++)
        {
            if (!piecePrefabDict.ContainsKey(piecePrefabs[i].type))
            {
                piecePrefabDict.Add(piecePrefabs[i].type, piecePrefabs[i].prefab);
            }
        }

        for (int x = 0; x < xDim; x++)
        {
            for (int y = 0; y < yDim; y++)
            {
                GameObject background = (GameObject)Instantiate(backgroundPrefabs, new Vector2(x, y), Quaternion.identity);
                background.transform.parent = transform;
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
