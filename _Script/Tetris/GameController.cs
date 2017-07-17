using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{

    public Board board;

    // Use this for initialization
    void Start()
    {
        board.SetBorderSize(20, 20);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
