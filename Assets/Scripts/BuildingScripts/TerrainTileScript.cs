using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainTileScript : MonoBehaviour {

    public int xCoord;
    public int yCoord;

    public GameObject topWall;
    public GameObject leftWall;
    public GameObject rightWall;
    public GameObject bottomWall;

    public int spriteIndex;

    public void SetCoords(int x, int y, int _spriteIndex)
    {
        xCoord = x;
        yCoord = y;

        spriteIndex = _spriteIndex;
        topWall.SetActive(false);
        leftWall.SetActive(false);
        rightWall.SetActive(false);
        bottomWall.SetActive(false);
    }
}
