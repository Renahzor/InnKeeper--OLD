using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileMapEditor : MonoBehaviour
{

    public TerrainBuilder builder;
    public int[,] spriteIDs;
    public List<Sprite> editorSprites;
    public Image displaySprite;

    bool editMode = false;

    int currentSelectedSprite = 0;

    public Texture2D textureAtlas;

    void Start()
    {
        int xTotal = (builder.xTiles * 2) + 1;
        int yTotal = (builder.yTiles * 2) + 1;

        displaySprite.sprite = editorSprites[currentSelectedSprite];
        spriteIDs = new int[xTotal, yTotal];

        for (int x = 0; x < xTotal; x++)
            for (int y = 0; y < yTotal; y++)
            {
                spriteIDs[x, y] = builder.tileMap[x, y].GetComponent<TerrainTileScript>().spriteIndex;
            }

        Sprite[] sprites = Resources.LoadAll<Sprite>(textureAtlas.name);

        editorSprites = new List<Sprite>((Sprite[])sprites);
    }

    void Update()
    {
        if (Input.GetKeyDown("m"))
        {
            editMode = !editMode;
        }

        if(editMode && Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hit.collider != null && hit.collider.tag == "Terrain")
            {
                TerrainTileScript terrain = hit.collider.gameObject.GetComponent<TerrainTileScript>();
                ChangeTileSprite(terrain.xCoord, terrain.yCoord);
            }

        }
    }

    public void ChangeSelectedSprite(int change)
    {
        currentSelectedSprite += change;

        if (currentSelectedSprite >= editorSprites.Count)
        {
            currentSelectedSprite = 0;
        }

        if (currentSelectedSprite < 0)
        {
            currentSelectedSprite = editorSprites.Count - 1;
        }

        displaySprite.sprite = editorSprites[currentSelectedSprite];

    }

    public void ChangeTileSprite(int x, int y)
    {
        builder.tileMap[x, y].GetComponent<SpriteRenderer>().sprite = editorSprites[currentSelectedSprite];
        spriteIDs[x, y] = currentSelectedSprite;
    }
}

