using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BienController : MonoBehaviour
{
    public static int width_grid = 10;
    public static int height_grid = 15;
    public static Transform[,] grid = new Transform[width_grid, height_grid];
    private GameObject blockCurrent, blockNext;
    private bool startGame = false;
    private Vector3 displayBlock = new Vector3(5.06f, 17.35f, -0.02465344f);
    private Vector3 displayNextBlock = new Vector3(13.24f, 12.28f, -0.02465344f);
    public int delete1Row = 100;
    public int delete2Row = 300;
    public int delete3Row = 800;
    public int delete4Row = 1500;
    public Text displayScore;
    public Text displayLevel;
    public Text displayLines;
    private int numberRowDeleted = 0;
    private int level = 0;
    private int lines = 0;
    private int score = 0;
    private int upLevel = 15;
    private void UpdateUI()
    {
        displayScore.text = score.ToString();
        displayLevel.text = level.ToString();
        displayLines.text = lines.ToString();
    }
    private void UpdateDeleted()
    {
        
        if(numberRowDeleted > 0)
        {
            Debug.LogWarning(numberRowDeleted);
            switch (numberRowDeleted) 
            {
                case 1: 
                    {
                        score += delete1Row;
                        lines += numberRowDeleted;
                        break;
                    } 
                case 2:
                    {
                        score += delete2Row;
                        lines += numberRowDeleted;
                        break;
                    }
                case 3:
                    {
                        score += delete3Row;
                        lines += numberRowDeleted;
                        break;
                    }
                case 4:
                    {
                        score += delete4Row;
                        lines += numberRowDeleted;
                        break;
                    }
                default: break;
            }
        }
        numberRowDeleted = 0;
    }
    private void UpdateLevel()
    {
       if(lines > upLevel)
        {
            level = lines / upLevel;
            FindObjectOfType<BlockController>().speed = 1.0f - level*0.05f;
        }
    }
    public bool inGrid(Vector2 kt)
    {
        return ((int)kt.x >= 0 && (int)kt.x < width_grid && (int)kt.y > 0);
    }
    private void Start()
    {
        createBlock();
    }
    void Update()
    {
        UpdateUI();
        UpdateDeleted();
        UpdateLevel();
    }
    public Vector2 Round(Vector2 vt)
    {
        return new Vector2(Mathf.Round(vt.x), Mathf.Round(vt.y));
    }
    string createRandomBlock()
    {
        int numberRandom = Random.Range(1,8);
        string blockRandom = "Block/Block_O";
        switch (numberRandom)
        {
            case 1:
                blockRandom = "Block/Block_O"; break;
            case 2:
                blockRandom = "Block/Block_I"; break;
            case 3:
                blockRandom = "Block/Block_T"; break;
            case 4:
                blockRandom = "Block/Block_J"; break;
            case 5:
                blockRandom = "Block/Block_L"; break;
            case 6:
                blockRandom = "Block/Block_Z"; break;
            case 7:
                blockRandom = "Block/Block_S"; break;
            default:
                blockRandom = "Block/Block_O"; break;
        }
        return blockRandom;
    }
    public void createBlock()
    {
        if (!startGame)
        {
            startGame = true;
            blockCurrent = (GameObject)Instantiate(Resources.Load(createRandomBlock(), typeof(GameObject)), displayBlock, Quaternion.identity);
            blockNext = (GameObject)Instantiate(Resources.Load(createRandomBlock(), typeof(GameObject)), displayNextBlock, Quaternion.identity);
            blockNext.GetComponent<BlockController>().enabled = false;
        }
        else
        {
            blockNext.transform.localPosition = displayBlock;
            blockCurrent = blockNext;
            blockNext.GetComponent<BlockController>().enabled = true;
            blockNext = (GameObject)Instantiate(Resources.Load(createRandomBlock(), typeof(GameObject)), displayNextBlock, Quaternion.identity);
            blockNext.GetComponent<BlockController>().enabled = false;
        }
    }
    public Transform BlockExisted(Vector2 kt)
    {
        if (kt.y >= height_grid - 1) return null;
        return grid[(int)kt.x, (int)kt.y];
    }
    public void updateGrid(BlockController block)
    {
        for(int y = 0; y < height_grid; ++y)
        {
            for (int x = 0; x < width_grid; ++x)
            {
                if (grid[x, y] != null) if (grid[x, y].parent == block.transform) grid[x, y] = null;
            }
        }
        foreach(Transform kt in block.transform)
        {
            Vector2 vt = Round(kt.position);
            if (vt.y < height_grid) grid[(int)vt.x, (int)vt.y] = kt;
        }
    }
    // delte row full
    public bool rowFull(int y)
    {
        for(int x = 0; x < width_grid; ++x)
        {
            if (grid[x, y] == null) return false;
        }
        numberRowDeleted++;
        return true;
    }
    public void mergeRow (int y)
    {
        for(int x = 0;x<width_grid; ++x)
        {
            if (grid[x,y] != null) 
            {
                grid[x, y - 1] = grid[x, y];
                grid[x, y] = null;
                grid[x, y - 1].position += new Vector3(0, -1.02f, 0);
            }
        }
    }
    public void mergeAllRow(int y)
    {
        for(int i = y; i < height_grid; ++i)
        {
            mergeRow(i);
        }
    }
    public void deleteRowFull(int y)
    {
        for(int x = 0; x<width_grid; ++x)
        {
            Destroy(grid[x, y].gameObject);
            grid[x, y] = null;
        }
    }
    public void deleteRow()
    {
        for(int y = 0; y<height_grid; ++y)
        {
            if (rowFull(y))
            {
                deleteRowFull(y);
                mergeAllRow(y+1);
                --y;
            }
        }
    }
    public bool checkGameOver(BlockController blocks)
    {
        for(int x = 0; x < width_grid; ++x)
        {
            foreach(Transform kt in blocks.transform)
            {
                Vector2 block = Round(kt.position);
                if(block.y >= height_grid)
                {
                    return true;
                }
            }
        }
        return false;
    }
    public void ActiveGameOver()
    {
        Application.LoadLevel("GameOver");
    }
}
