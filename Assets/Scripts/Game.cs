using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    //grid 
    public static int gridWidth = 10;
    public static int gridHeight = 20;

    //Score
    public int oneLineScore = 40;
    public int twoLineScore = 100;
    public int threeLineScore = 400;
    public int fourLineScore = 1000;

    // number of full line
    private int numberOfLineThisTurn = 0;

    //the text of score, correspond to the text in the canvas
    public Text hud_score;

    //the score variable that users now get
    public static int currentScore = 0;

    // the entity of grid
    public static Transform[,] grid = new Transform[gridWidth, gridHeight];

    //sound player
    public AudioSource audiosource;
    //when lines are cleared, the sound will be played
    public AudioClip clearLineSound;

    // Start is called before the first frame update
    void Start()
    {
        //spawn the first tetromino
        SpawnNextTetromino();
        //init the sound player, GetComponent means get the component in the inspector
        audiosource = GetComponent<AudioSource>();
    }

    void Update()
    {
        //to update users' ectra source
        UpdateScore();  

        //update the score shown in the scene
        UpdateUI();
    }

    public void UpdateUI()
    {
        // match the text component and the score variable
        hud_score.text = currentScore.ToString();
        
    }

    public void UpdateScore()
    {
        if(numberOfLineThisTurn > 0)
        {
            //call method according to the number of full lines
            if(numberOfLineThisTurn == 1)
            {

                ClearOneLine();

            }else if(numberOfLineThisTurn == 2)
            {

                ClearTwoLine();

            }else if(numberOfLineThisTurn == 3)
            {

                ClearThreeLine();

            }else if(numberOfLineThisTurn == 4)
            {

                ClearFourLine();    
                
            }
            numberOfLineThisTurn = 0;
            PlayLineCleardSound();
        }
    }

    //add the corresponding score in each function
    public void ClearOneLine()
    {
        currentScore += oneLineScore;
    }

    public void ClearTwoLine()
    {
        currentScore += twoLineScore;
    }

    public void ClearThreeLine()
    {
        currentScore += threeLineScore;
    }

    public void ClearFourLine()
    {
        currentScore += fourLineScore;
    }

    public void PlayLineCleardSound()
    {
        audiosource.PlayOneShot(clearLineSound);
    }

    //border check
    public bool CheckIfAboveGrid(Tetromino tetromino)
    {
        for(int x = 0; x < gridWidth; ++x)
        {
            foreach(Transform mino in tetromino.transform)
            {
                Vector2 pos = mino.position;
                if(pos.y > gridHeight - 1)
                {
                    return true;
                }
            }
        }
        return false;
    }

    // check if line is filled of position y
    public bool IsFullAtRoW(int y)
    {
        for (int x = 0; x < gridWidth; ++x)
        {
            // not full in this line
            if (grid[x, y] == null)
            {
                return false;
            }
        }
        numberOfLineThisTurn++;
        return true;

    }

    // destroy all the minos in the line of y
    public void DestroyMinoAtRow(int y)
    {
        for (int x = 0; x < gridWidth; ++x)
        {
            Destroy(grid[x, y].gameObject);
            grid[x, y] = null;
        }
    }

    //move all the object down above y
    public void MoveRowDown(int y)
    {
        for (int x = 0; x < gridWidth; ++x)
        {
            if(grid[x,y] != null)
            {
                // get object down, but position is not changed
                grid[x, y - 1] = grid[x, y];
                //set original pos to null 
                grid[x, y] = null;
                //change the position
                grid[x, y - 1].position += new Vector3(0, -1, 0);
            }
        }
    }
    public void MoveAllRowsDown(int y)
    {
        //for all objects above y, move them down
        for (int i = y; i < gridHeight; ++i)
        {
            MoveRowDown(i);
        }
    }

    public void DeleteRow()
    {
        // for all vertical position,check if there are full rows, if there are, destroy them and move them all down
        for(int y = 0; y < gridHeight; ++y)
        {
            if(IsFullAtRoW(y))
            {
                DestroyMinoAtRow(y);
                MoveAllRowsDown(y + 1);
                // y move been moved down,so y = y - 1
                y--;    
            }
        }

    }

    public void UpdateGrid(Tetromino tetromino)
    {
        //not clear now
        for (int y = 0; y < gridHeight; ++y )
        {
            for(int x = 0; x < gridWidth; ++x)
            {
                if(grid[x,y] != null)
                {
                    if(grid[x,y].parent == tetromino.transform)
                    {
                        grid[x, y] = null;
                    }
                }
            }
        }

        // set the grid with the minos in transform
        foreach(Transform mino in tetromino.transform)
        {
            Vector2 pos = GetInteger(mino.position);
            if(pos.y < gridHeight)
            {
                grid[(int)pos.x, (int)pos.y] = mino;
            }
        }
    }

    //return back the object of the position in the grid
    public Transform GetTransformAtGridPosition(Vector2 pos)
    {
        //illegal
        if(pos.y > gridHeight - 1)
        {
            return null;
        }
        else
        {
            return grid[(int)pos.x, (int)pos.y];
        }
    }

    public void SpawnNextTetromino()
    {
        //instantiate prefabs from the prefabs folder
            GameObject nextTetromino = Instantiate(Resources.Load(GetRandomTetromino(), typeof(GameObject))
                , new Vector3(5.0f, 20.0f,0.0f), Quaternion.identity) as GameObject;
  
    }

    //check the boundary
    public bool CheckEdgePosition(Vector3 pos)
    {
        return ((int)pos.x >= 0 && (int)pos.x < gridWidth && (int)pos.y >= 0);
    }

    //return the integer of the pos
    public Vector2 GetInteger(Vector2 pos)
    {
        return new Vector2(Mathf.Round(pos.x), Mathf.Round(pos.y));
    }

    //randomly produce the name of the prefabs, then return to instantiate the object
     string GetRandomTetromino()
    {
        int randomTetromino = Random.Range(1, 8);
        string tetrominoName = "Prefabs/J";

        switch(randomTetromino)
        {
            case 1:
                tetrominoName = "Prefabs/J";
                break;
            case 2:
                tetrominoName = "Prefabs/L";
                break;
            case 3:
                tetrominoName = "Prefabs/Long";
                break;
            case 4:
                tetrominoName = "Prefabs/S";
                break;
            case 5:
                tetrominoName = "Prefabs/Square";
                break;
            case 6:
                tetrominoName = "Prefabs/T";
                break;
            case 7:
                tetrominoName = "Prefabs/Z";
                break;
            
        }
        //Debug.LogError(tetrominoName);
        return tetrominoName;
    }

    //call the GameOver Scene
    public void Gameover()
    {
        Application.LoadLevel("GameOver");
    }


}
