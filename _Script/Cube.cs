using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
    public TetrisCubeType type;
    public GameObject pivot;

    internal int rotateRatio = 0;

    private bool toRotate, left, right, quickFall;
    private float originFallGap;
    private float fallGapTime = 0.5f;
    private Board board;
    private float countTime = 0f;
    // private float movePressCount = 0f;
    // Use this for initialization
    void Start()
    {
        Init();
    }

    private void Init()
    {
        originFallGap = fallGapTime;
        board = GameObject.Find("Board").GetComponent<Board>();
    }

    private void FixedUpdate()
    {
        quickFall = Input.GetKey(KeyCode.DownArrow);

        toRotate = Input.GetKeyDown(KeyCode.Space);

        left = Input.GetKey(KeyCode.LeftArrow);

        right = Input.GetKey(KeyCode.RightArrow);
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

            if (quickFall)
                fallGapTime = originFallGap / 10f;
            else
                fallGapTime = originFallGap;

            if (toRotate)
                HandleRotate();

            if (left)
                HandleMove(true);

            if (right)
                HandleMove(false);
        }
    }

    private void HandleFall()
    {
        if (board.checkChange(1))
        {
            Fall();
            board.updateCube();
        }
        else
        {
            StopCube();
        }
    }

    private void HandleRotate()
    {
        if (board.checkChange(3))
        {
            Rotate();
            board.updateCube();
        }
    }

    private void HandleMove(bool toLeft)
    {
        if (board.checkChange(2, toLeft))
        {
            Move(toLeft);
            board.updateCube();
        }
    }

    private void Fall()
    {
        transform.position += new Vector3(0f, -0.01f * DataFormat.CubeSize);
    }

    private void Rotate()
    {
        if (type != TetrisCubeType.O)
        {

            if (type == TetrisCubeType.I)
            {
                float delta = DataFormat.CubeSize / 200f;
                Vector2 tmp = pivot.transform.position;
                switch (rotateRatio)
                {
                    case 0:
                        tmp = new Vector2(tmp.x - delta, tmp.y - delta);
                        break;
                    case 1:
                        tmp = new Vector2(tmp.x - delta, tmp.y + delta);
                        break;
                    case 2:
                        tmp = new Vector2(tmp.x + delta, tmp.y + delta);
                        break;
                    case 3:
                        tmp = new Vector2(tmp.x + delta, tmp.y - delta);
                        break;
                }
                transform.RotateAround(tmp, Vector3.forward, -90);
            }else
            {
                transform.RotateAround(pivot.transform.position, Vector3.forward, -90);
            }
            rotateRatio = ++rotateRatio % 4;
        }
    }

    private void Move(bool toLeft)
    {
        if (toLeft)
        {
            transform.position += new Vector3(-0.01f * DataFormat.CubeSize, 0f);
        }
        else
        {
            transform.position += new Vector3(0.01f * DataFormat.CubeSize, 0f);
        }
    }

    public void StopCube()
    {
        fallGapTime = -1f;
        originFallGap = -1f;
        board.currentCube = null;
    }
}
