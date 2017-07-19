using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public GameObject pos;
    public GameController gameController;

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
        float bgScaleX = DataFormat.CubeSize / 32f * _boardWidth;
        float bgScaleY = DataFormat.CubeSize / 32f * _boardHeight;
        transform.localScale = new Vector3(bgScaleX, bgScaleY, 1);

        cubes = new bool[_boardWidth][];
        for (int i = 0; i < _boardWidth; i++)
        {
            cubes[i] = new bool[_boardHeight + 5];
        }

        // 缓冲区为false
    }

    public Cube InstantiateCube()
    {
        float x, y;
        GetStartPivotPos(9, 19, out x, out y);
        return (Instantiate(Resources.Load("Prefabs/RZCube"), new Vector3(x, y), Quaternion.identity) as GameObject).GetComponent<Cube>();
    }

    public void SetBorderSize(int width, int height)
    {
        _boardWidth = width;
        _boardHeight = height;
    }

    private void OnDrawGizmos()
    {

        //float x, y;
        //GetArrayIndex(pos.transform.position.x, pos.transform.position.y, out x, out y);
        //print(x + " " + y);

        Gizmos.color = Color.black;
        for (int i = 0; i <= _boardHeight; i++)
        {
            Gizmos.DrawLine(new Vector3(Mathf.Round((-3.2f + DataFormat.CubeSize / 100f * i) / DataFormat.CubeSize * 100f) * DataFormat.CubeSize / 100f, 3.2f, 0),
            new Vector3(Mathf.Round((-3.2f + DataFormat.CubeSize / 100f * i) / DataFormat.CubeSize * 100f) * DataFormat.CubeSize / 100f, -3.2f, 0));
        }

        for (int i = 0; i <= _boardWidth; i++)
        {
            Gizmos.DrawLine(new Vector3(3.2f, Mathf.Round((-3.2f + DataFormat.CubeSize / 100f * i) / DataFormat.CubeSize * 100f) * DataFormat.CubeSize / 100f, 0),
                new Vector3(-3.2f, Mathf.Round((-3.2f + DataFormat.CubeSize / 100f * i) / DataFormat.CubeSize * 100f) * DataFormat.CubeSize / 100f, 0));
        }

        if (gameController.debug)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(new Vector3(Mathf.Round((pos.transform.position.x + DataFormat.CubeSize / 2 / 100f) / DataFormat.CubeSize * 100f) * DataFormat.CubeSize / 100f - DataFormat.CubeSize / 2 / 100f, 4f, 0),
                new Vector3(Mathf.Round((pos.transform.position.x + DataFormat.CubeSize / 2 / 100f) / DataFormat.CubeSize * 100f) * DataFormat.CubeSize / 100f - DataFormat.CubeSize / 2 / 100f, -4f, 0));

            Gizmos.DrawLine(new Vector3(4, Mathf.Round((pos.transform.position.y + DataFormat.CubeSize / 2 / 100f) / DataFormat.CubeSize * 100f) * DataFormat.CubeSize / 100f - DataFormat.CubeSize / 2 / 100f, 0),
                new Vector3(-4, Mathf.Round((pos.transform.position.y + DataFormat.CubeSize / 2 / 100f) / DataFormat.CubeSize * 100f) * DataFormat.CubeSize / 100f - DataFormat.CubeSize / 2 / 100f, 0));


            if (Application.isPlaying)
            {
                float x1, y1;
                Gizmos.color = Color.white;

                for (int i = 0; i < _boardWidth; i++)
                {
                    for (int j = 0; j < _boardHeight; j++)
                    {
                        if (cubes[i][j])
                        {
                            GetPos(i, j, out x1, out y1);
                            Gizmos.DrawCube(new Vector2(x1, y1), Vector3.one / 5);
                        }
                    }
                }
            }
        }

    }

    #region Index Calc
    // xIndex range [0,width - 1]
    // yIndex range [0,height - 1]
    public void GetArrayIndex(float x, float y, out float xIndex, out float yIndex)
    {
        xIndex = Mathf.Round((x + DataFormat.CubeSize / 2f / 100f) / DataFormat.CubeSize * 100f) + _boardWidth / 2f - 1;
        yIndex = Mathf.Round((y + DataFormat.CubeSize / 2f / 100f) / DataFormat.CubeSize * 100f) + _boardHeight / 2f - 1;
    }

    public void GetCurrentPivotIndex(out float xIndex, out float yIndex)
    {
        GetArrayIndex(currentCube.pivot.transform.position.x, currentCube.pivot.transform.position.y, out xIndex, out yIndex);
    }

    public Vector2 GetCurrentPivotIndex()
    {
        float xIndex, yIndex;
        if (currentCube == null)
            return Vector2.zero;
        GetArrayIndex(currentCube.pivot.transform.position.x, currentCube.pivot.transform.position.y, out xIndex, out yIndex);
        return new Vector2(xIndex, yIndex);
    }

    public void GetPos(int xIndex, int yIndex, out float x, out float y)
    {
        x = (xIndex + 1 - _boardWidth / 2f) / 100f * DataFormat.CubeSize - DataFormat.CubeSize / 2f / 100f;
        y = (yIndex + 1 - _boardHeight / 2f) / 100f * DataFormat.CubeSize - DataFormat.CubeSize / 2f / 100f;
    }

    public void GetPivotPos(int xIndex, int yIndex, out float x, out float y)
    {
        GetPos(9, 19, out x, out y);
        x -= DataFormat.CubeSize / 2f / 100f;
        y -= DataFormat.CubeSize / 2f / 100f;
    }

    public void GetStartPivotPos(int xIndex, int yIndex, out float x, out float y)
    {
        GetPivotPos(9, 20, out x, out y);
        y += DataFormat.CubeSize * 3f / 100f;
    }
    #endregion

    #region check collision
    public bool checkFall()
    {
        Vector2 currentPivotIndex = GetCurrentPivotIndex();

        if ((int)currentPivotIndex.y > 21)
            return true;

        switch (currentCube.type)
        {
            case TetrisCubeType.RZ:
                return HandleRZCheck(currentPivotIndex);

            case TetrisCubeType.Z:
                return HandleZCheck(currentPivotIndex);

            case TetrisCubeType.RL:
                return HandleRLCheck(currentPivotIndex);

            case TetrisCubeType.L:
                return HandleLCheck(currentPivotIndex);

            case TetrisCubeType.T:
                return HandleTCheck(currentPivotIndex);

            case TetrisCubeType.I:
                return HandleICheck(currentPivotIndex);

            case TetrisCubeType.O:
                return HandleOCheck(currentPivotIndex);

            default: return false;
        }
    }

    public bool HandleRZCheck(Vector2 pivotPos)
    {
        if (currentCube.rotateRatio == 1)
        {
            /**轴心在X, C为需要检测的方块
             * oxoo
             * oXxo
             * oCxo
             * ooCo
             * */
            return checkCollision(new Vector2(pivotPos.x, pivotPos.y - 1),
                new Vector2(pivotPos.x + 1, pivotPos.y - 2));
        }
        else
        {
            /**
            * oooo
            * oXxo
            * xxCo
            * CCoo
            * */
            return checkCollision(new Vector2(pivotPos.x + 1, pivotPos.y - 1),
                new Vector2(pivotPos.x, pivotPos.y - 2),
                new Vector2(pivotPos.x - 1, pivotPos.y - 2));
        }
    }

    public bool HandleZCheck(Vector2 pivotPos)
    {
        if (currentCube.rotateRatio == 1)
        {
            /**轴心在X, C为需要检测的方块
             * ooxo
             * oXxo
             * oxCo
             * oCoo
             * */
            return checkCollision(new Vector2(pivotPos.x, pivotPos.y - 2),
                new Vector2(pivotPos.x + 1, pivotPos.y - 1));
        }
        else
        {
            /**
            * oooo
            * xxoo
            * CXxo
            * oCCo
            * */
            return checkCollision(new Vector2(pivotPos.x - 1, pivotPos.y),
                new Vector2(pivotPos.x, pivotPos.y - 1),
                new Vector2(pivotPos.x + 1, pivotPos.y - 1));
        }
    }

    public bool HandleRLCheck(Vector2 pivotPos)
    {
        switch (currentCube.rotateRatio)
        {
            case 0:
                /**轴心在X, C为需要检测的方块
                 * ooxo
                 * ooxo
                 * oxXo
                 * oCCo
                 * */

                return checkCollision(new Vector2(pivotPos.x, pivotPos.y - 1),
                    new Vector2(pivotPos.x - 1, pivotPos.y - 1));
            case 1:
                /**oooooo
                 * ooxooo
                 * ooXxxo
                 * ooCCCo
                 * */

                return checkCollision(new Vector2(pivotPos.x, pivotPos.y - 1),
                    new Vector2(pivotPos.x + 1, pivotPos.y - 1),
                    new Vector2(pivotPos.x + 2, pivotPos.y - 1));
            case 2:
                /**ooooo
                 * ooXxo
                 * ooxCo
                 * ooxoo
                 * ooCoo
                 * */

                return checkCollision(new Vector2(pivotPos.x, pivotPos.y - 3),
                    new Vector2(pivotPos.x + 1, pivotPos.y - 1));
            case 3:
                /**oooo
                 * oooo
                 * xxXo
                 * CCxo
                 * ooCo
                 * */

                return checkCollision(new Vector2(pivotPos.x - 2, pivotPos.y - 1),
                    new Vector2(pivotPos.x - 1, pivotPos.y - 1),
                    new Vector2(pivotPos.x, pivotPos.y - 2));

            default:
                Debug.LogException(new System.Exception("RL state Error!"));
                return false;
        }
    }

    public bool HandleLCheck(Vector2 pivotPos)
    {
        switch (currentCube.rotateRatio)
        {
            case 0:
                /**轴心在X, C为需要检测的方块
                 * oxoo
                 * oxoo
                 * oXxo
                 * oCCo
                 * */

                return checkCollision(new Vector2(pivotPos.x, pivotPos.y - 1),
                    new Vector2(pivotPos.x - 1, pivotPos.y - 1));
            case 1:
                /**oooooo
                 * ooxooo
                 * ooXxxo
                 * ooCCCo
                 * */

                return checkCollision(new Vector2(pivotPos.x, pivotPos.y - 1),
                    new Vector2(pivotPos.x + 1, pivotPos.y - 1),
                    new Vector2(pivotPos.x + 2, pivotPos.y - 1));
            case 2:
                /**ooooo
                 * ooXxo
                 * ooxCo
                 * ooxoo
                 * ooCoo
                 * */

                return checkCollision(new Vector2(pivotPos.x, pivotPos.y - 3),
                    new Vector2(pivotPos.x + 1, pivotPos.y - 1));
            case 3:
                /**oooo
                 * oooo
                 * xxXo
                 * CCxo
                 * ooCo
                 * */

                return checkCollision(new Vector2(pivotPos.x - 2, pivotPos.y - 1),
                    new Vector2(pivotPos.x - 1, pivotPos.y - 1),
                    new Vector2(pivotPos.x, pivotPos.y - 2));

            default:
                Debug.LogException(new System.Exception("RL state Error!"));
                return false;
        }
    }

    public bool HandleTCheck(Vector2 pivotPos)
    {

    }

    public bool HandleICheck(Vector2 pivotPos)
    {

    }

    public bool HandleOCheck(Vector2 pivotPos)
    {

    }

    public bool checkRotate()
    {
        Vector2 currentPivotIndex = GetCurrentPivotIndex();

        switch (currentCube.type)
        {
            case TetrisCubeType.RZ:

                if (currentCube.rotateRatio == 1)
                {
                    /**轴心在X, C为需要检测的方块
                     * oxCo
                     * CXxo
                     * ooxo
                     * */
                    return checkCollision(new Vector2(currentPivotIndex.x - 1, currentPivotIndex.y),
                        new Vector2(currentPivotIndex.x + 1, currentPivotIndex.y + 1));
                }
                else
                {
                    /**
                    * oCoo
                    * oXxo
                    * xxCo
                    * oooo
                    * */
                    return checkCollision(new Vector2(currentPivotIndex.x, currentPivotIndex.y + 1),
                        new Vector2(currentPivotIndex.x + 1, currentPivotIndex.y - 1));
                }

            default: return false;
        }
    }

    public bool checkMove(bool toLeft)
    {
        if (currentCube == null)
            return false;

        Vector2 currentPivotIndex = GetCurrentPivotIndex();

        switch (currentCube.type)
        {
            case TetrisCubeType.RZ:
                if ((int)currentPivotIndex.y > 21)
                    return true;

                if (currentCube.rotateRatio == 1)
                {
                    /**
                     * 轴心在X, L为toLeft需要检测的方块, 否则检测R
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
            //print(v.x + "," + v.y);
            if (v.x < 0 || v.x >= _boardWidth || v.y < 0 || cubes[(int)v.x][(int)v.y])
            {
                //print("blocked");
                return false;
            }
        }
        return true;
    }
    #endregion

    #region update cube
    public void updateFallCube()
    {
        switch (currentCube.type)
        {
            case TetrisCubeType.RZ:
                float xIndex, yIndex;
                GetArrayIndex(currentCube.pivot.transform.position.x, currentCube.pivot.transform.position.y, out xIndex, out yIndex);
                //if (xIndex >= 0 && yIndex >= 0 && xIndex < _boardWidth && yIndex < _boardHeight)
                if (xIndex >= 0 && yIndex >= 0 && xIndex < _boardWidth)
                {
                    if (currentCube.rotateRatio == 1)
                    {
                        /**
                         * 轴心在X, D为需要置false的方块
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
                         * oDDo
                         * DXxo
                         * xxoo
                         * */

                        cubes[(int)xIndex - 1][(int)yIndex] = false;
                        cubes[(int)xIndex][(int)yIndex + 1] = false;
                        cubes[(int)xIndex + 1][(int)yIndex + 1] = false;

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
                        /**
                         * 轴心在X, D为需要置false的方块
                         * oooo
                         * oxoo
                         * oXxo
                         * DDxo
                         * */

                        cubes[(int)xIndex][(int)yIndex - 1] = false;
                        cubes[(int)xIndex - 1][(int)yIndex - 1] = false;

                        if (yIndex + 1 < _boardHeight)
                            cubes[(int)xIndex][(int)yIndex + 1] = true;

                        cubes[(int)xIndex][(int)yIndex] = true;
                        cubes[(int)xIndex + 1][(int)yIndex] = true;
                        cubes[(int)xIndex + 1][(int)yIndex - 1] = true;
                    }
                    else
                    {
                        /**
                         * oDoo
                         * oXxo
                         * xxDo
                         * */

                        cubes[(int)xIndex][(int)yIndex + 1] = false;
                        cubes[(int)xIndex + 1][(int)yIndex - 1] = false;

                        cubes[(int)xIndex][(int)yIndex] = true;
                        cubes[(int)xIndex + 1][(int)yIndex] = true;
                        cubes[(int)xIndex][(int)yIndex - 1] = true;
                        cubes[(int)xIndex - 1][(int)yIndex - 1] = true;
                    }
                }
                break;
        }
    }

    public void updateMoveCube(bool toLeft)
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
                        /**
                         * 轴心在X, L,R为需要置false的方块
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
                            cubes[(int)xIndex - 1][(int)yIndex] = false;
                            cubes[(int)xIndex - 1][(int)yIndex + 1] = false;
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
    #endregion
}
