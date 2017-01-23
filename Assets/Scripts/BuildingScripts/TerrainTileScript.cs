using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainTileScript : MonoBehaviour {

    public int xCoord;
    public int yCoord;

    public int spriteIndex;

    public void SetCoords(int x, int y, int _spriteIndex)
    {
        xCoord = x;
        yCoord = y;

        spriteIndex = _spriteIndex;
    }
}
