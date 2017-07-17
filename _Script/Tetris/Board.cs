using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public GameObject pos;

    private int _boardHeight = 20;
    private int _boardWidth = 20;
    private bool[][] cubes;
    internal Cube currentCube;

    // Use this for initialization
    void Start()
    {
        Init();
        currentCube = InstantiateCube();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentCube == null)
        {
            currentCube = InstantiateCube();
        }
    }

    // 36 * 20 / 100 == 7.2 == 32 * scale / 100
    // scale == 22.5
    private void Init()
    {
        float bgScaleX = 36f / 32f * _boardWidth;
        float bgScaleY = 36f / 32f * _boardHeight;
        transform.localScale = new Vector3(bgScaleX, bgScaleY, 1);

        cubes = new bool[_boardWidth][];
        for (int i = 0; i < _boardWidth; i++)
        {
            cubes[i] = new bool[_boardHeight + 5];
        }

        // 缓冲区为false
    }

    public void SetBorderSize(int width, int height)
    {
        _boardWidth = width;
        _boardHeight = height;
    }

    // xIndex range [0,width - 1]
    // yIndex range [0,height - 1]
    public void GetArrayIndex(float x, float y, out float xIndex, out float yIndex)
    {
        xIndex = Mathf.Round((x + 18f / 100f) / 36f * 100f) + _boardWidth / 2f - 1;
        yIndex = Mathf.Round((y + 18f / 100f) / 36f * 100f) + _boardHeight / 2f - 1;
    }

    public void GetCurrentPivotIndex(out float xIndex, out float yIndex)
    {
        GetArrayIndex(currentCube.pivot.transform.position.x, currentCube.pivot.transform.position.y, out xIndex, out yIndex);
    }

    public Vector2 GetCurrentPivotIndex()
    {
        float xIndex, yIndex;
        GetArrayIndex(currentCube.pivot.transform.position.x, currentCube.pivot.transform.position.y, out xIndex, out yIndex);
        return new Vector2(xIndex, yIndex);
    }

    public void GetPos(int xIndex, int yIndex, out float x, out float y)
    {
        x = (xIndex + 1 - _boardWidth / 2f) / 100f * 36f - 18f / 100f;
        y = (yIndex + 1 - _boardHeight / 2f) / 100f * 36f - 18f / 100f;
    }

    public void GetPivotPos(int xIndex, int yIndex, out float x, out float y)
    {
        GetPos(9, 19, out x, out y);
        x -= 18f / 100f;
        y -= 18f / 100f;
    }

    public void GetStartPivotPos(int xIndex, int yIndex, out float x, out float y)
    {
        GetPivotPos(9, 19, out x, out y);
        y += 36f * 3f / 100f;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(Mathf.Round((pos.transform.position.x + 18f / 100f) / 36f * 100f) * 36f / 100f - 18f / 100f, 5f, 0),
            new Vector3(Mathf.Round((pos.transform.position.x + 18f / 100f) / 36f * 100f) * 36f / 100f - 18f / 100f, -5f, 0));

        Gizmos.DrawLine(new Vector3(5, Mathf.Round((pos.transform.position.y + 18f / 100f) / 36f * 100f) * 36f / 100f - 18f / 100f, 0),
            new Vector3(-5, Mathf.Round((pos.transform.position.y + 18f / 100f) / 36f * 100f) * 36f / 100f - 18f / 100f, 0));

        Gizmos.color = Color.black;
        for (int i = 0; i <= _boardHeight; i++)
        {
            Gizmos.DrawLine(new Vector3(Mathf.Round((-3.5f + 36f / 100f * i) / 36f * 100f) * 36f / 100f, 4f, 0),
            new Vector3(Mathf.Round((-3.5f + 36f / 100f * i) / 36f * 100f) * 36f / 100f, -4f, 0));
        }

        for (int i = 0; i <= _boardWidth; i++)
        {
            Gizmos.DrawLine(new Vector3(4f, Mathf.Round((-3.5f + 36f / 100f * i) / 36f * 100f) * 36f / 100f, 0),
                new Vector3(-4f, Mathf.Round((-3.5f + 36f / 100f * i) / 36f * 100f) * 36f / 100f, 0));
        }
    }

    public Cube InstantiateCube()
    {
        float x, y;
        GetStartPivotPos(9, 19, out x, out y);
        return (Instantiate(Resources.Load("Prefabs/RZCube"), new Vector3(x, y), Quaternion.identity) as GameObject).GetComponent<Cube>();
    }

    public void updateArray(List<Vector2> activeList, List<Vector2> deactiveList)
    {
        foreach (Vector2 v in activeList)
        {
            cubes[(int)v.x][(int)v.y] = true;
        }

        foreach (Vector2 v in deactiveList)
        {
            cubes[(int)v.x][(int)v.y] = false;
        }
    }

    public bool checkFall()
    {
        Vector2 currentPivotIndex = GetCurrentPivotIndex();

        switch (currentCube.type)
        {
            case TetrisCubeType.RZ:
                if ((int)currentPivotIndex.y > 21)
                    return true;

                if (currentCube.rotateRatio == 1)
                {
                    /**轴心在X, C为需要检测的方块
                     * oxoo
                     * oXxo
                     * oCxo
                     * ooCo
                     * */
                    print(currentPivotIndex);
                    print(cubes[(int)currentPivotIndex.x][(int)currentPivotIndex.y]);
                    return checkCollision(new Vector2(currentPivotIndex.x, currentPivotIndex.y - 1),
                        new Vector2(currentPivotIndex.x + 1, currentPivotIndex.y - 2));
                }
                else
                {
                    /**
                    * oooo
                    * oXxo
                    * xxCo
                    * CCoo
                    * */
                    return checkCollision(new Vector2(currentPivotIndex.x + 1, currentPivotIndex.y - 1),
                        new Vector2(currentPivotIndex.x, currentPivotIndex.y - 2),
                        new Vector2(currentPivotIndex.x - 1, currentPivotIndex.y - 2));
                }

            default: return false;
        }
    }

    public bool checkRotate()
    {

        return false;
    }

    public bool checkMove(bool toLeft)
    {
        Vector2 currentPivotIndex = GetCurrentPivotIndex();

        switch (currentCube.type)
        {
            case TetrisCubeType.RZ:
                if ((int)currentPivotIndex.y > 21)
                    return true;

                if (currentCube.rotateRatio == 1)
                {
                    /**轴心在X, L为toLeft需要检测的方块, 否则检测R
                     * LxRo
                     * LXxR
                     * oLxR
                     * */
                    if (toLeft)
                    {
                        return checkCollision(new Vector2(currentPivotIndex.x - 1, currentPivotIndex.y),
    new Vector2(currentPivotIndex.x - 1, currentPivotIndex.y + 1),
    new Vector2(currentPivotIndex.x, currentPivotIndex.y - 1));
                    }
                    else
                    {
                        return checkCollision(new Vector2(currentPivotIndex.x + 2, currentPivotIndex.y),
    new Vector2(currentPivotIndex.x + 2, currentPivotIndex.y - 1),
    new Vector2(currentPivotIndex.x + 1, currentPivotIndex.y + 1));
                    }

                }
                else
                {
                    /**
                    * ooooo
                    * oLXxR
                    * LxxRo
                    * ooooo
                    * */
                    if (toLeft)
                    {
                        return checkCollision(new Vector2(currentPivotIndex.x - 1, currentPivotIndex.y),
    new Vector2(currentPivotIndex.x - 2, currentPivotIndex.y - 1));
                    }
                    else
                    {
                        return checkCollision(new Vector2(currentPivotIndex.x + 2, currentPivotIndex.y),
    new Vector2(currentPivotIndex.x + 1, currentPivotIndex.y - 1));
                    }
                }

            default: return false;
        }
    }

    // 需要去掉重合方块
    private bool checkCollision(params Vector2[] checkList)
    {
        foreach (Vector2 v in checkList)
        {
            print(v.x + "," + v.y);
            if (v.x < 0 || v.x >= _boardWidth || v.y < 0 || cubes[(int)v.x][(int)v.y])
            {
                print("blocked");
                return false;
            }
        }
        return true;
    }

    public void updateFallCube()
    {
        switch (currentCube.type)
        {
            case TetrisCubeType.RZ:
                float xIndex, yIndex;
                GetArrayIndex(currentCube.pivot.transform.position.x, currentCube.pivot.transform.position.y, out xIndex, out yIndex);
                if (xIndex >= 0 && yIndex >= 0 && xIndex < _boardWidth && yIndex < _boardHeight)
                {
                    if (currentCube.rotateRatio == 1)
                    {
                        /**轴心在X, D为需要置false的方块
                         * oDoo
                         * oxDo
                         * oXxo
                         * ooxo
                         * */

                        cubes[(int)xIndex][(int)yIndex + 2] = false;
                        cubes[(int)xIndex + 1][(int)yIndex + 1] = false;

                        if (yIndex + 1 < _boardHeight)
                            cubes[(int)xIndex][(int)yIndex + 1] = true;

                        cubes[(int)xIndex][(int)yIndex] = true;
                        cubes[(int)xIndex + 1][(int)yIndex] = true;
                        cubes[(int)xIndex + 1][(int)yIndex - 1] = true;
                    }
                    else
                    {
                        /**
                         * oooo
                         * oXxo
                         * xxoo
                         * */
                        cubes[(int)xIndex][(int)yIndex] = true;
                        cubes[(int)xIndex + 1][(int)yIndex] = true;
                        cubes[(int)xIndex][(int)yIndex - 1] = true;
                        cubes[(int)xIndex - 1][(int)yIndex - 1] = true;
                    }
                }
                break;
        }
    }

    public void updateRotateCube()
    {
        switch (currentCube.type)
        {
            case TetrisCubeType.RZ:
                float xIndex, yIndex;
                GetArrayIndex(currentCube.pivot.transform.position.x, currentCube.pivot.transform.position.y, out xIndex, out yIndex);
                if (xIndex >= 0 && yIndex >= 0 && xIndex < _boardWidth && yIndex < _boardHeight)
                {
                    if (currentCube.rotateRatio == 1)
                    {
                        /**轴心在X, D为需要置false的方块
                         * oDoo
                         * oxDo
                         * oXxo
                         * ooxo
                         * */

                        cubes[(int)xIndex][(int)yIndex + 2] = false;
                        cubes[(int)xIndex + 1][(int)yIndex + 1] = false;

                        if (yIndex + 1 < _boardHeight)
                            cubes[(int)xIndex][(int)yIndex + 1] = true;

                        cubes[(int)xIndex][(int)yIndex] = true;
                        cubes[(int)xIndex + 1][(int)yIndex] = true;
                        cubes[(int)xIndex + 1][(int)yIndex - 1] = true;
                    }
                    else
                    {
                        /**
                         * oooo
                         * oXxo
                         * xxoo
                         * */
                        cubes[(int)xIndex][(int)yIndex] = true;
                        cubes[(int)xIndex + 1][(int)yIndex] = true;
                        cubes[(int)xIndex][(int)yIndex - 1] = true;
                        cubes[(int)xIndex - 1][(int)yIndex - 1] = true;
                    }
                }
                break;
        }
    }

    public void updateMov
        eCube(bool toLeft)
    {
        switch (currentCube.type)
        {
            case TetrisCubeType.RZ:
                float xIndex, yIndex;
                GetArrayIndex(currentCube.pivot.transform.position.x, currentCube.pivot.transform.position.y, out xIndex, out yIndex);
                if (xIndex >= 0 && yIndex >= 0 && xIndex < _boardWidth && yIndex < _boardHeight)
                {
                    if (currentCube.rotateRatio == 1)
                    {
                        /**轴心在X, L,R为需要置false的方块
                         * ooooo
                         * RxLoo
                         * RXxLo
                         * oRxLo
                         * */

                        if (toLeft)
                        {
                            cubes[(int)xIndex + 1][(int)yIndex + 1] = false;
                            cubes[(int)xIndex + 2][(int)yIndex] = false;
                            cubes[(int)xIndex + 2][(int)yIndex - 1] = false;
                        }
                        else
                        {
                            cubes[(int)xIndex + 1][(int)yIndex - 1] = false;
                            cubes[(int)xIndex - 1][(int)yIndex] = false;
                            cubes[(int)xIndex][(int)yIndex - 1] = false;
                        }

                        if (yIndex + 1 < _boardHeight)
                            cubes[(int)xIndex][(int)yIndex + 1] = true;

                        cubes[(int)xIndex][(int)yIndex] = true;
                        cubes[(int)xIndex + 1][(int)yIndex] = true;
                        cubes[(int)xIndex + 1][(int)yIndex - 1] = true;
                    }
                    else
                    {
                        /**
                         * ooooo
                         * oRXxL
                         * RxxLo
                         * */

                        if (toLeft)
                        {
                            cubes[(int)xIndex + 2][(int)yIndex] = false;
                            cubes[(int)xIndex + 1][(int)yIndex - 1] = false;
                        }
                        else
                        {
                            cubes[(int)xIndex - 1][(int)yIndex] = false;
                            cubes[(int)xIndex - 2][(int)yIndex - 1] = false;
                        }

                        cubes[(int)xIndex][(int)yIndex] = true;
                        cubes[(int)xIndex + 1][(int)yIndex] = true;
                        cubes[(int)xIndex][(int)yIndex - 1] = true;
                        cubes[(int)xIndex - 1][(int)yIndex - 1] = true;
                    }
                }
                break;
        }
    }
}
