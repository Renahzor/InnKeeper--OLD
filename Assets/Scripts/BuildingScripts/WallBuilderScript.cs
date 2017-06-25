using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallBuilderScript : MonoBehaviour {

    [SerializeField]
    List<Sprite> floorSprites = new List<Sprite>();

    [SerializeField]
    TerrainBuilder terrainBuilder;

    int tileSize = 64;

    Sprite selectedSprite;
    Sprite tempStoredSprite = null;
    bool currentlyBuilding = false;

    Vector2 currentTile = new Vector2(0, 0);
    Vector2 storedTile = new Vector2(-500, -500);

    void Start()
    {
        selectedSprite = floorSprites[0];
        storedTile = currentTile;
        tempStoredSprite = RetrieveSprite(currentTile);
    }

    void Update()
    {
        var mousePos = Input.mousePosition;
        mousePos.z = 0;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);

        if (mousePos.x > 0.0f)
            mousePos.x += .32f;
        else mousePos.x -= .32f;

        if (mousePos.y > 0.0f)
            mousePos.y += .32f;
        else mousePos.y -= .32f;

        currentTile.x = (int)(mousePos.x / (tileSize / 100.0f));
        currentTile.y = (int)(mousePos.y / (tileSize / 100.0f));

        if (Input.GetKeyDown(KeyCode.K))
        {
            if (currentlyBuilding)
            {
                SetSprite(currentTile, tempStoredSprite);
            }

            ToggleBuildMode();
        }

        if (currentlyBuilding)
        {
            //Debug.Log(currentTile);
            if (currentTile == storedTile)
            {
                if (selectedSprite != RetrieveSprite(currentTile))
                    SetSprite(currentTile, selectedSprite);
            }

            else
            {
                SetSprite(storedTile, tempStoredSprite);
                tempStoredSprite = RetrieveSprite(currentTile);
                SetSprite(currentTile, selectedSprite);
                storedTile = currentTile;
            }

            if (Input.GetMouseButtonDown(0))
            {
                SetSprite(currentTile, selectedSprite);
                tempStoredSprite = selectedSprite;
            }
        }

    }

    public void ToggleBuildMode()
    {
        currentlyBuilding = !currentlyBuilding;
    }

    public void SetTileSelection(int tileIndex)
    {
        if (tileIndex < floorSprites.Count - 1)
            selectedSprite = floorSprites[tileIndex];

        else
            Debug.Log("Tile Index not found in floorSprites List");
    }

    private Sprite RetrieveSprite(Vector2 coords)
    {
        if (currentTile.x == -500 || currentTile.y == -500)
            return null;

        return terrainBuilder.tileMap[(int)coords.x + terrainBuilder.xTiles, (int)coords.y + terrainBuilder.yTiles].GetComponent<SpriteRenderer>().sprite;
    }

    private void SetSprite(Vector2 coords, Sprite sprite)
    {
        terrainBuilder.tileMap[(int)coords.x + terrainBuilder.xTiles, (int)coords.y + terrainBuilder.yTiles].GetComponent<SpriteRenderer>().sprite = sprite;
    }
}