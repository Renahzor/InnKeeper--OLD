using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainBuilder : MonoBehaviour {

    [SerializeField]
    GameObject terrainTilePrefab;
    [SerializeField]
    List<Sprite> baseSprites = new List<Sprite>();

    int tilesize = 64;

    public int xTiles;
    public int yTiles;
    public Transform terrainParent;

    public GameObject[,] tileMap;

    void Awake()
    {
        int xTotal = (xTiles * 2) + 1;
        int yTotal = (yTiles * 2) + 1;

        tileMap = new GameObject[xTotal, yTotal];

        for (int x = 0; x <= xTiles * 2; x++)
            for (int y = 0; y <= yTiles * 2; y++)
            {
                int index = Random.Range(0, baseSprites.Count);
                var tile = Instantiate(terrainTilePrefab);
                tile.GetComponent<SpriteRenderer>().sprite = baseSprites[index];
                tile.transform.localPosition = new Vector3((x - xTiles) * tilesize / 100.0f, (y - yTiles) * tilesize / 100.0f, 0);
                tile.transform.SetParent(terrainParent.transform, true);
                tile.GetComponent<TerrainTileScript>().SetCoords(x, y, index);
                tileMap[x, y] = tile;
            }
    }	
}
