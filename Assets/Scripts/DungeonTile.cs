using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonTile : MonoBehaviour
{
    public float size = 5;
    [HideInInspector] public bool isOn;
    [HideInInspector] public int coordX;
    [HideInInspector] public int coordY;

    public GameObject wallUp, wallDown, wallLeft, wallRight, floor, ceiling;

    public void Init(int x, int y)
    {
        coordX = x;
        coordY = y;
        isOn = false;
    }
}
