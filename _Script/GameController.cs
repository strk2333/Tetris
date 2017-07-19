using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{

    public Board board;
    public int height;
    public int width;
    public bool debug;

    // Use this for initialization
    void Start()
    {
        height = 20;
        width = 20;
        board.SetBorderSize(width, height);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public int GetHeight()
    {
        return height;
    }

    public int GetWidth()
    {
        return width;
    }
}
