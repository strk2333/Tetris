using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour {
    public TetrisCubeType type;

    private float originFallGap = 1f;
    private float fallGapTime = 1f;
    private Board board;
    private int spriteIndex;
    internal int rotateRatio = 1;
    private float countTime = 0f;
    // Use this for initialization
    void Start () {
        Init();
	}
	
	// Update is called once per frame
	void Update () {
        if (fallGapTime > 0f)
        {
            countTime += Time.deltaTime;

            if (countTime > fallGapTime)
            {
                countTime = 0f;
                HandleFall();
            }

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                fallGapTime /= 3f;
            }
            else
            {
                fallGapTime = originFallGap;
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                HandleRotate();
            }
        }
    }

    private void HandleFall()
    {
        Fall();
        
        board.updateCube();
    }

    private void HandleRotate()
    {
        Rotate();
        board.updateCube();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == "BottomCheck")
        {
            fallGapTime = -1f;
            board.currentCube = null;
        }
    }

    private void Init()
    {
        board = GameObject.Find("Board").GetComponent<Board>();
        spriteIndex = 0;
        type = TetrisCubeType.RZ;
    }

    private void Fall()
    {
        transform.position += new Vector3(0f, -0.36f);
    }

    private void Rotate()
    {
        switch (type)
        {
            case TetrisCubeType.RZ:
                transform.Rotate(new Vector3(0, 0, 90 * rotateRatio));
                rotateRatio *= -1;
                break;
        }
    }

    private void GetPivotCubeIndex(out float xIndex, out float yIndex)
    {
        board.GetArrayIndex(transform.position.x, transform.position.y, out xIndex, out yIndex);
    }

    //private void getBottomIndex(out float xIndex, out float yIndex)
    //{

    //}

    private void checkBottom()
    {

    }
}
