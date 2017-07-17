using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
    public TetrisCubeType type;
    public GameObject pivot;

    private float originFallGap;
    private float fallGapTime = .2f;
    private Board board;
    internal int rotateRatio = 1;
    private float countTime = 0f;
    // Use this for initialization
    void Start()
    {
        Init();
    }

    private void Init()
    {
        originFallGap = fallGapTime;
        board = GameObject.Find("Board").GetComponent<Board>();
        type = TetrisCubeType.RZ;
    }

    // Update is called once per frame
    void Update()
    {
        if (fallGapTime > 0f)
        {
            countTime += Time.deltaTime;

            // 每fallGapTime秒执行一次下落
            if (countTime > fallGapTime)
            {
                countTime = 0f;
                HandleFall();
            }

            if (Input.GetKey(KeyCode.DownArrow))
            {
                fallGapTime = originFallGap / 10f;
            }
            else
            {
                fallGapTime = originFallGap;
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                HandleRotate();
            }

            if (Input.GetKey(KeyCode.LeftArrow) && countTime > 0.1f)
            {
                HandleMove(true);
            }
        }
    }

    private void HandleFall()
    {
        if (board.checkFall())
        {
            Fall();
            board.updateFallCube();
        }
        else
        {
            StopCube();
        }
    }

    private void HandleRotate()
    {
        if (board.checkRotate())
        {
            Rotate();
            board.updateFallCube();
        }
        else
        {
            StopCube();
        }
    }

    private void HandleMove(bool toLeft)
    {
        if (board.checkMove(toLeft))
        {
            Move(toLeft);
        }
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
                rotateRatio = -rotateRatio;
                break;
        }
    }

    private void Move(bool toLeft)
    {
        if (toLeft)
        {
            transform.position += new Vector3(-0.36f, 0f);
        }
        else
        {
            transform.position += new Vector3(0.36f, 0f);
        }
    }

    public void StopCube()
    {
        originFallGap = -1f;
        board.currentCube = null;
    }
}
