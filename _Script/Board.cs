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
    private List<Vector2> cubesToEnable;
    private List<Vector2> cubesToDisable;

    private List<Vector2> debugV1;
    private List<Vector2> debugV2;

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

        cubesToEnable = new List<Vector2>();
        cubesToDisable = new List<Vector2>();
        debugV1 = new List<Vector2>();
        debugV2 = new List<Vector2>();
    }

    public Cube InstantiateCube()
    {
        float x, y;
        GetStartPivotPos(9, 19, out x, out y);
        return (Instantiate(Resources.Load("Prefabs/OCube"), new Vector3(x, y), Quaternion.identity) as GameObject).GetComponent<Cube>();
    }

    public void SetBorderSize(int width, int height)
    {
        _boardWidth = width;
        _boardHeight = height;
    }

    private void OnDrawGizmos()
    {
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

                if (currentCube != null)
                {
                    Vector2 tmp = Vector2.zero;
                    float delta = 0.16f;
                    switch (currentCube.rotateRatio)
                    {
                        // fix this
                        case 0:
                            tmp = new Vector2(currentCube.pivot.transform.position.x - delta, currentCube.pivot.transform.position.y - delta);
                            break;
                        case 1:
                            tmp = new Vector2(currentCube.pivot.transform.position.x - delta, currentCube.pivot.transform.position.y + delta);
                            break;
                        case 2:
                            tmp = new Vector2(currentCube.pivot.transform.position.x + delta, currentCube.pivot.transform.position.y + delta);
                            break;
                        case 3:
                            tmp = new Vector2(currentCube.pivot.transform.position.x + delta, currentCube.pivot.transform.position.y - delta);
                            break;
                    }

                    Gizmos.DrawSphere(new Vector2(tmp.x, tmp.y), 0.1f);

                    //Gizmos.color = Color.red;
                    //foreach (Vector2 v in debugV1)
                    //{
                    //    GetPos((int)v.x, (int)v.y, out x1, out y1);
                    //    Gizmos.DrawSphere(new Vector2(x1, y1), 0.05f);
                    //}
                    //Gizmos.color = Color.blue;
                    //foreach (Vector2 v in debugV2)
                    //{
                    //    GetPos((int)v.x, (int)v.y, out x1, out y1);
                    //    Gizmos.DrawSphere(new Vector2(x1, y1), 0.05f);
                    //}
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

    #region check & update
    // changeType 1-fall 2-move 3-rotate
    public bool checkChange(int changeType, bool toLeft = false)
    {
        if (currentCube == null)
        {
            return false;
        }
        return HandleCheck(GetOnta(), changeType, toLeft);
    }

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

    public void updateCube()
    {
        updateState(cubesToEnable.ToArray(), cubesToDisable.ToArray());
    }

    private bool HandleCheck(Vector2[] onta, int changeType, bool toLeft = false)
    {
        if (checkCollision(GetCollisionCheckCube(GetOnta(), changeType, toLeft)))
            return true;
        else
        {
            cubesToEnable.Clear();
            cubesToDisable.Clear();
            return false;
        }
    }

    // record currentCube info to array onta.
    // func for data initial, properly the biggest func:P
    // optimise it if you can.(it really needs, i kindof have some thoughts.)
    private Vector2[] GetOnta()
    {
        Vector2[] onta = new Vector2[4];
        Vector2 pivotIndex = GetCurrentPivotIndex();
        int ratio = currentCube.rotateRatio;
        switch (currentCube.type)
        {
            case TetrisCubeType.RZ:
                /**
                 * oxoo oooo xooo oxxo
                 * oXxo oXxo xXoo xXoo
                 * ooxo xxoo oxoo oooo
                 * */
                switch (ratio)
                {
                    case 0:
                        onta[0] = new Vector2(pivotIndex.x, pivotIndex.y);
                        onta[1] = new Vector2(pivotIndex.x, pivotIndex.y + 1);
                        onta[2] = new Vector2(pivotIndex.x + 1, pivotIndex.y);
                        onta[3] = new Vector2(pivotIndex.x + 1, pivotIndex.y - 1);
                        break;
                    case 1:
                        onta[0] = new Vector2(pivotIndex.x, pivotIndex.y);
                        onta[1] = new Vector2(pivotIndex.x + 1, pivotIndex.y);
                        onta[2] = new Vector2(pivotIndex.x, pivotIndex.y - 1);
                        onta[3] = new Vector2(pivotIndex.x - 1, pivotIndex.y - 1);
                        break;
                    case 2:
                        onta[0] = new Vector2(pivotIndex.x, pivotIndex.y);
                        onta[1] = new Vector2(pivotIndex.x - 1, pivotIndex.y);
                        onta[2] = new Vector2(pivotIndex.x - 1, pivotIndex.y + 1);
                        onta[3] = new Vector2(pivotIndex.x, pivotIndex.y - 1);
                        break;
                    case 3:
                        onta[0] = new Vector2(pivotIndex.x, pivotIndex.y);
                        onta[1] = new Vector2(pivotIndex.x - 1, pivotIndex.y);
                        onta[2] = new Vector2(pivotIndex.x, pivotIndex.y + 1);
                        onta[3] = new Vector2(pivotIndex.x + 1, pivotIndex.y + 1);
                        break;
                    default:
                        Debug.LogException(new System.Exception("the ratio is not valid"));
                        break;
                }
                break;
            case TetrisCubeType.Z:
                /**
                 * ooxo oooo oxoo xxoo
                 * oXxo xXoo xXoo oXxo
                 * oxoo oxxo xooo oooo
                 * */
                switch (ratio)
                {
                    case 0:
                        onta[0] = new Vector2(pivotIndex.x, pivotIndex.y);
                        onta[1] = new Vector2(pivotIndex.x, pivotIndex.y - 1);
                        onta[2] = new Vector2(pivotIndex.x + 1, pivotIndex.y);
                        onta[3] = new Vector2(pivotIndex.x + 1, pivotIndex.y + 1);
                        break;
                    case 1:
                        onta[0] = new Vector2(pivotIndex.x, pivotIndex.y);
                        onta[1] = new Vector2(pivotIndex.x - 1, pivotIndex.y);
                        onta[2] = new Vector2(pivotIndex.x, pivotIndex.y - 1);
                        onta[3] = new Vector2(pivotIndex.x + 1, pivotIndex.y - 1);
                        break;
                    case 2:
                        onta[0] = new Vector2(pivotIndex.x, pivotIndex.y);
                        onta[1] = new Vector2(pivotIndex.x - 1, pivotIndex.y);
                        onta[2] = new Vector2(pivotIndex.x - 1, pivotIndex.y - 1);
                        onta[3] = new Vector2(pivotIndex.x, pivotIndex.y + 1);
                        break;
                    case 3:
                        onta[0] = new Vector2(pivotIndex.x, pivotIndex.y);
                        onta[1] = new Vector2(pivotIndex.x + 1, pivotIndex.y);
                        onta[2] = new Vector2(pivotIndex.x, pivotIndex.y + 1);
                        onta[3] = new Vector2(pivotIndex.x - 1, pivotIndex.y + 1);
                        break;
                    default:
                        Debug.LogException(new System.Exception("the ratio is not valid"));
                        break;
                }
                break;

            case TetrisCubeType.RL:
                /**
                 * oxxo oooo oxoo xooo
                 * oXoo xXxo oXoo xXxo
                 * oxoo ooxo xxoo oooo
                 * */
                switch (ratio)
                {
                    case 0:
                        onta[0] = new Vector2(pivotIndex.x, pivotIndex.y);
                        onta[1] = new Vector2(pivotIndex.x, pivotIndex.y - 1);
                        onta[2] = new Vector2(pivotIndex.x, pivotIndex.y + 1);
                        onta[3] = new Vector2(pivotIndex.x + 1, pivotIndex.y + 1);
                        break;
                    case 1:
                        onta[0] = new Vector2(pivotIndex.x, pivotIndex.y);
                        onta[1] = new Vector2(pivotIndex.x - 1, pivotIndex.y);
                        onta[2] = new Vector2(pivotIndex.x + 1, pivotIndex.y);
                        onta[3] = new Vector2(pivotIndex.x + 1, pivotIndex.y - 1);
                        break;
                    case 2:
                        onta[0] = new Vector2(pivotIndex.x, pivotIndex.y);
                        onta[1] = new Vector2(pivotIndex.x, pivotIndex.y - 1);
                        onta[2] = new Vector2(pivotIndex.x, pivotIndex.y + 1);
                        onta[3] = new Vector2(pivotIndex.x - 1, pivotIndex.y - 1);
                        break;
                    case 3:
                        onta[0] = new Vector2(pivotIndex.x, pivotIndex.y);
                        onta[1] = new Vector2(pivotIndex.x + 1, pivotIndex.y);
                        onta[2] = new Vector2(pivotIndex.x - 1, pivotIndex.y);
                        onta[3] = new Vector2(pivotIndex.x - 1, pivotIndex.y + 1);
                        break;
                    default:
                        Debug.LogException(new System.Exception("the ratio is not valid"));
                        break;
                }
                break;

            case TetrisCubeType.L:
                /**
                 * oxoo oooo xxoo ooxo
                 * oXoo xXxo oXoo xXxo
                 * oxxo xooo oxoo oooo
                 * */
                switch (ratio)
                {
                    case 0:
                        onta[0] = new Vector2(pivotIndex.x, pivotIndex.y);
                        onta[1] = new Vector2(pivotIndex.x, pivotIndex.y - 1);
                        onta[2] = new Vector2(pivotIndex.x, pivotIndex.y + 1);
                        onta[3] = new Vector2(pivotIndex.x + 1, pivotIndex.y - 1);
                        break;
                    case 1:
                        onta[0] = new Vector2(pivotIndex.x, pivotIndex.y);
                        onta[1] = new Vector2(pivotIndex.x + 1, pivotIndex.y);
                        onta[2] = new Vector2(pivotIndex.x - 1, pivotIndex.y);
                        onta[3] = new Vector2(pivotIndex.x - 1, pivotIndex.y - 1);
                        break;
                    case 2:
                        onta[0] = new Vector2(pivotIndex.x, pivotIndex.y);
                        onta[1] = new Vector2(pivotIndex.x, pivotIndex.y - 1);
                        onta[2] = new Vector2(pivotIndex.x, pivotIndex.y + 1);
                        onta[3] = new Vector2(pivotIndex.x - 1, pivotIndex.y + 1);
                        break;
                    case 3:
                        onta[0] = new Vector2(pivotIndex.x, pivotIndex.y);
                        onta[1] = new Vector2(pivotIndex.x - 1, pivotIndex.y);
                        onta[2] = new Vector2(pivotIndex.x + 1, pivotIndex.y);
                        onta[3] = new Vector2(pivotIndex.x + 1, pivotIndex.y + 1);
                        break;
                    default:
                        Debug.LogException(new System.Exception("the ratio is not valid"));
                        break;
                }
                break;

            case TetrisCubeType.T:
                /**
                 * oxoo oooo oxoo oxoo
                 * oXxo xXxo xXoo xXxo
                 * oxoo oxoo oxoo oooo
                 * */
                switch (ratio)
                {
                    case 0:
                        onta[0] = new Vector2(pivotIndex.x, pivotIndex.y);
                        onta[1] = new Vector2(pivotIndex.x, pivotIndex.y - 1);
                        onta[2] = new Vector2(pivotIndex.x, pivotIndex.y + 1);
                        onta[3] = new Vector2(pivotIndex.x + 1, pivotIndex.y);
                        break;
                    case 1:
                        onta[0] = new Vector2(pivotIndex.x, pivotIndex.y);
                        onta[1] = new Vector2(pivotIndex.x - 1, pivotIndex.y);
                        onta[2] = new Vector2(pivotIndex.x + 1, pivotIndex.y);
                        onta[3] = new Vector2(pivotIndex.x, pivotIndex.y - 1);
                        break;
                    case 2:
                        onta[0] = new Vector2(pivotIndex.x, pivotIndex.y);
                        onta[1] = new Vector2(pivotIndex.x, pivotIndex.y - 1);
                        onta[2] = new Vector2(pivotIndex.x, pivotIndex.y + 1);
                        onta[3] = new Vector2(pivotIndex.x - 1, pivotIndex.y);
                        break;
                    case 3:
                        onta[0] = new Vector2(pivotIndex.x, pivotIndex.y);
                        onta[1] = new Vector2(pivotIndex.x + 1, pivotIndex.y);
                        onta[2] = new Vector2(pivotIndex.x - 1, pivotIndex.y);
                        onta[3] = new Vector2(pivotIndex.x, pivotIndex.y + 1);
                        break;
                    default:
                        Debug.LogException(new System.Exception("the ratio is not valid"));
                        break;
                }
                break;

            case TetrisCubeType.I:
                /**pivot: X's right bottom
                 * ooxo oooo oxoo oooo
                 * ooXo oooo oxoo xXxx
                 * ooxo xxXx oXoo oooo
                 * ooxo oooo oxoo oooo
                 * */
                switch (ratio)
                {
                    case 0:
                        onta[0] = new Vector2(pivotIndex.x, pivotIndex.y);
                        onta[1] = new Vector2(pivotIndex.x, pivotIndex.y + 1);
                        onta[2] = new Vector2(pivotIndex.x, pivotIndex.y - 1);
                        onta[3] = new Vector2(pivotIndex.x, pivotIndex.y - 2);
                        break;
                    case 1:
                        onta[0] = new Vector2(pivotIndex.x, pivotIndex.y);
                        onta[1] = new Vector2(pivotIndex.x - 2, pivotIndex.y);
                        onta[2] = new Vector2(pivotIndex.x - 1, pivotIndex.y);
                        onta[3] = new Vector2(pivotIndex.x + 1, pivotIndex.y);
                        break;
                    case 2:
                        onta[0] = new Vector2(pivotIndex.x, pivotIndex.y);
                        onta[1] = new Vector2(pivotIndex.x, pivotIndex.y + 1);
                        onta[2] = new Vector2(pivotIndex.x, pivotIndex.y + 2);
                        onta[3] = new Vector2(pivotIndex.x, pivotIndex.y - 1);
                        break;
                    case 3:
                        onta[0] = new Vector2(pivotIndex.x, pivotIndex.y);
                        onta[1] = new Vector2(pivotIndex.x + 1, pivotIndex.y);
                        onta[2] = new Vector2(pivotIndex.x + 2, pivotIndex.y);
                        onta[3] = new Vector2(pivotIndex.x - 1, pivotIndex.y);
                        break;
                    default:
                        Debug.LogException(new System.Exception("the ratio is not valid"));
                        break;
                }
                break;

            case TetrisCubeType.O:
                /**pivot: X's right bottom
                 * xX
                 * xx
                 * */
                onta[0] = new Vector2(pivotIndex.x, pivotIndex.y);
                onta[1] = new Vector2(pivotIndex.x - 1, pivotIndex.y);
                onta[2] = new Vector2(pivotIndex.x, pivotIndex.y - 1);
                onta[3] = new Vector2(pivotIndex.x - 1, pivotIndex.y - 1);
                break;
            default:
                Debug.LogException(new System.Exception("the TetrisType is not valid"));
                break;
        }
        return onta;
    }

    // changeType: 1 - fall 2 - move 3 - rotate
    // onta: 本体 (from greece)
    private Vector2[] GetCollisionCheckCube(Vector2[] onta, int changeType, bool toLeft = false)
    {
        Vector2[] afterTrans = new Vector2[onta.Length];
        switch (changeType)
        {
            case 1:
                for (int i = 0; i < onta.Length; i++)
                    afterTrans[i].Set(onta[i].x, onta[i].y - 1);

                break;
            case 2:
                if (toLeft)
                {
                    for (int i = 0; i < onta.Length; i++)
                        afterTrans[i].Set(onta[i].x - 1, onta[i].y);
                }
                else
                {
                    for (int i = 0; i < onta.Length; i++)
                        afterTrans[i].Set(onta[i].x + 1, onta[i].y);
                }

                break;
            case 3:
                if (currentCube.type == TetrisCubeType.O)
                    return null;

                for (int i = 0; i < onta.Length; i++)
                    afterTrans[i] = GetRotatePoint(onta[i], GetCurrentPivotIndex());

                debugV1.AddRange(onta);
                debugV2.AddRange(afterTrans);
                break;
            default:
                break;
        }

        return SplitOverlayCube(onta, afterTrans);
    }

    // clockwise rotate 90°
    // rotate: (y, -x)
    private Vector2 GetRotatePoint(Vector2 origin, Vector2 pivot)
    {
        if (currentCube.type == TetrisCubeType.I)
        {
            float delta = 0.01f * DataFormat.CubeSize / 2f;
            switch (currentCube.rotateRatio)
            {
                // fix this
                case 0:
                    pivot = new Vector2(pivot.x - delta, pivot.y - delta);
                    break;
                case 1:
                    pivot = new Vector2(pivot.x - delta, pivot.y + delta);
                    break;
                case 2:
                    pivot = new Vector2(pivot.x + delta, pivot.y + delta);
                    Vector2 refPos2 = origin - pivot;
                    Vector2 tmp = pivot + new Vector2(refPos2.y, -refPos2.x);
                    return new Vector2(tmp.x, tmp.y + 1);
                case 3:
                    pivot = new Vector2(pivot.x + delta, pivot.y - delta);
                    Vector2 refPos3 = origin - pivot;
                    Vector2 tmp3 = pivot + new Vector2(refPos3.y, -refPos3.x);
                    return new Vector2(tmp3.x + 1, tmp3.y);
            }
        }
        Vector2 refPos = origin - pivot;
        return pivot + new Vector2(refPos.y, -refPos.x);
    }

    // 去除重合, 获得要增加(result)和删除(cubesToDisable)状态的方块
    private Vector2[] SplitOverlayCube(Vector2[] onta, Vector2[] afterTran)
    {
        cubesToEnable.Clear();
        cubesToDisable.Clear();
        cubesToEnable.AddRange(afterTran);
        cubesToDisable.AddRange(onta);

        for (int i = 0; i < afterTran.Length; i++)
        {
            for (int j = 0; j < onta.Length; j++)
            {
                if (afterTran[i] == onta[j])
                {
                    cubesToEnable.Remove(afterTran[i]);
                    cubesToDisable.Remove(afterTran[i]);
                    break;
                }
            }
        }
        return cubesToEnable.ToArray();
    }

    private void updateState(Vector2[] add, Vector2[] del)
    {
        foreach (Vector2 v in del)
        {
            cubes[(int)v.x][(int)v.y] = false;
        }

        foreach (Vector2 v in add)
        {
            cubes[(int)v.x][(int)v.y] = true;
        }
    }

    private bool isCollided()
    {
        return false;
    }
    #endregion
}