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

        cubes = new bool[_boardHeight][];
        for (int i = 0; i < _boardHeight; i++)
        {
            cubes[i] = new bool[_boardWidth];
        }
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

        //   Gizmos.color = Color.blue;
        //   Gizmos.DrawLine(new Vector3(Mathf.Round((pos.transform.position.x) / 36f * 100f) * 36f / 100f + 36 / 100f, 4f, 0),
        //new Vector3(Mathf.Round((pos.transform.position.x) / 36f * 100f) * 36f / 100f + 36f / 100f, -4f, 0));

        Gizmos.DrawLine(new Vector3(5, Mathf.Round((pos.transform.position.y + 18f / 100f) / 36f * 100f) * 36f / 100f - 18f / 100f, 0),
            new Vector3(-5, Mathf.Round((pos.transform.position.y + 18f / 100f) / 36f * 100f) * 36f / 100f - 18f / 100f, 0));
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

    public void updateCube()
    {
        switch (currentCube.type)
        {
            case TetrisCubeType.RZ:
                float xIndex, yIndex;
                GetArrayIndex(currentCube.transform.position.x, currentCube.transform.position.y, out xIndex, out yIndex);
                if (currentCube.rotateRatio == 1)
                {
                    /**所有轴心都在X左下角
                     * oxoo
                     * oXxo
                     * ooxo
                     * */
                    cubes[(int) xIndex][(int) yIndex] = true;
                }
                else
                {
                    /**
                     * oooo
                     * oXxo
                     * xxoo
                     * */
                    cubes[(int) xIndex][(int) yIndex] = true;
                }
                break;
        }
    }
}
