using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorBuilderScript : MonoBehaviour {

    [SerializeField]
    List<Sprite> floorSprites = new List<Sprite>();

    [SerializeField]
    TerrainBuilder terrainBuilder;

    int tileSize = 64;

    Sprite selectedSprite;
    GameObject selectedFloor;
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
                if (selectedFloor.GetComponent<BuildableObject>().buildCost > Player.Instance.playerGold)
                {
                    GameMaster.Instance.SendGameMessage("Not Enough gold to build.\nRequires " + selectedFloor.GetComponent<BuildableObject>().buildCost + " Gold");
                }

                else if (selectedSprite != tempStoredSprite)
                {
                    GameMaster.Instance.SendGameMessage("-" + selectedFloor.GetComponent<BuildableObject>().buildCost + "g");
                    SetSprite(currentTile, selectedSprite);
                    tempStoredSprite = selectedSprite;
                    Player.Instance.playerGold -= selectedFloor.GetComponent<BuildableObject>().buildCost;
                    Player.Instance.UpdateGoldDisplay();
                    WallPlacement(currentTile, RetrieveSprite(currentTile));
                }
            }

            if (Input.GetMouseButtonDown(1))
            {
                SetSprite(storedTile, tempStoredSprite);
                ToggleBuildMode();
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

    //Find sprite of tile at coords
    private Sprite RetrieveSprite(Vector2 coords)
    {
        if (currentTile.x == -500 || currentTile.y == -500)
            return null;

        return terrainBuilder.tileMap[(int)coords.x + terrainBuilder.xTiles, (int)coords.y + terrainBuilder.yTiles].GetComponent<SpriteRenderer>().sprite;
    }

    //Changes the target tile sprite
    private void SetSprite(Vector2 coords, Sprite sprite)
    {
        terrainBuilder.tileMap[(int)coords.x + terrainBuilder.xTiles, (int)coords.y + terrainBuilder.yTiles].GetComponent<SpriteRenderer>().sprite = sprite;
    }

    //Method to begin floor building mode
    public void SetupBuilder(GameObject selectedFloorObject)
    {
        //currentlyBuilding = false;
        currentlyBuilding = true;
        selectedSprite = selectedFloorObject.GetComponent<SpriteRenderer>().sprite;
        selectedFloor = selectedFloorObject;
    }

    //Method to place walls along exterior of building
    //Ugly,find a way to refactor? 
    void WallPlacement(Vector2 coords, Sprite sSprite)
    {
        if (RetrieveSprite(new Vector2(coords.x, (coords.y - 1.0f))) != sSprite)
        {
            terrainBuilder.tileMap[(int)coords.x + terrainBuilder.xTiles, (int)coords.y + terrainBuilder.yTiles].GetComponent<TerrainTileScript>().bottomWall.SetActive(true);
        }
        else
        {
            terrainBuilder.tileMap[(int)coords.x + terrainBuilder.xTiles, (int)(coords.y - 1.0f) + terrainBuilder.yTiles].GetComponent<TerrainTileScript>().topWall.SetActive(false);
        }
          
        if (RetrieveSprite(new Vector2(coords.x, (coords.y + 1.0f))) != sSprite)
        {
            terrainBuilder.tileMap[(int)coords.x + terrainBuilder.xTiles, (int)coords.y + terrainBuilder.yTiles].GetComponent<TerrainTileScript>().topWall.SetActive(true);
        }
        else
        {
            terrainBuilder.tileMap[(int)coords.x + terrainBuilder.xTiles, (int)(coords.y + 1.0f) + terrainBuilder.yTiles].GetComponent<TerrainTileScript>().bottomWall.SetActive(false);
        }


        if (RetrieveSprite(new Vector2((coords.x - 1.0f), coords.y)) != sSprite)
        {
            terrainBuilder.tileMap[(int)coords.x + terrainBuilder.xTiles, (int)coords.y + terrainBuilder.yTiles].GetComponent<TerrainTileScript>().leftWall.SetActive(true);
        }
        else
        {
            terrainBuilder.tileMap[(int)(coords.x - 1.0f) + terrainBuilder.xTiles, (int)(coords.y) + terrainBuilder.yTiles].GetComponent<TerrainTileScript>().rightWall.SetActive(false);
        }

        if (RetrieveSprite(new Vector2((coords.x + 1.0f), coords.y)) != sSprite)
        {
            terrainBuilder.tileMap[(int)coords.x + terrainBuilder.xTiles, (int)coords.y + terrainBuilder.yTiles].GetComponent<TerrainTileScript>().rightWall.SetActive(true);
        }
        else
        {
            terrainBuilder.tileMap[(int)(coords.x + 1.0f) + terrainBuilder.xTiles, (int)(coords.y) + terrainBuilder.yTiles].GetComponent<TerrainTileScript>().leftWall.SetActive(false);
        }
    }


}