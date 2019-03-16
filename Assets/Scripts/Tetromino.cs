using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tetromino : MonoBehaviour
{

    
    float fall = 0; // time record of last movement
    public float fallspeed = 1; // the fall speed

    //the limitation property of tetromino to rotate
    public bool allowrotation = true; 
    public bool limitrotation = false;

    // the higher speed users use to fall, the high score they will get
    public int individualScore = 100;   //the full extra score  
    private float individualScoreTime;  //the timer, like the contain, once overflow, operate something

    //sound effects for tetromino
    public AudioClip moveSound;     //moveSound effect, correspond to the component in the inspector
    public AudioClip rotateSound;   //rotateSound effect, correspond to the component in the inspector
    public AudioClip landSound;     //landSound effect, correspond to the component in the inspector

    private AudioSource audioSource;    //Sound Player, to play sound above


    // Start is called before the first frame update
    void Start()
    {

        audioSource = GetComponent<AudioSource>();  // get entity of component in the inspector in the initiation

    }

    // Update is called once per frame
    void Update()
    {
        CheckUserInput();
        UpdateIndividualScore();
    }

    public void UpdateIndividualScore()
    {
        //count the extra score that the users can get
        if(individualScoreTime < 1) //cut the period into every unit
        {
            individualScoreTime += Time.deltaTime;  //plus deltaTime every update

        }
        else
        {
            individualScoreTime = 0;    //if individualScoreTime >= 1,means user use more than unit time
            individualScore = Mathf.Max(individualScore - 10, 0); //then minus 10 score, and keep updating
        }
    }

    //check user's input function
    void CheckUserInput()
    {

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {

            transform.position += new Vector3(1, 0, 0);

            if (CheckIfValid())     //Check if after moving. the object is still within border
            {
                //if it is, update the grid in Game
                //FindObjectOfType means find object of the whole scene, differ from GetComponent<>
                FindObjectOfType<Game>().UpdateGrid(this);
                //once update,means valid movement,then play the sound
                playMoveSound();                            
            }
            else
            {
                //if not, get back to the original position(eyes cannot discover)
                transform.position -= new Vector3(1, 0, 0); 
            }

        }
        else if(Input.GetKeyDown(KeyCode.LeftArrow))
        {

            transform.position += new Vector3(-1, 0, 0);
            if (CheckIfValid())
            {
                FindObjectOfType<Game>().UpdateGrid(this);
                playMoveSound();
            }
            else
            {
                transform.position -= new Vector3(-1, 0, 0);
            }

        }
        else if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            //here to limit the rotation of some objects that can just rotate in 180 degree, and be back
            if(allowrotation)
            {
                if(limitrotation)
                {
                    if(transform.rotation.eulerAngles.z >= 90)
                    {
                        transform.Rotate(0, 0, -90);
                    }
                    else
                    {
                        transform.Rotate(0, 0, 90);
                    }
                }
                else
                {
                    transform.Rotate(0, 0, 90);
                }

                if (CheckIfValid())
                {
                    FindObjectOfType<Game>().UpdateGrid(this);
                    playRotateSound();
                }
                else
                {
                    if (limitrotation)
                    {
                        if (transform.eulerAngles.z >= 90)
                        {
                            transform.Rotate(0, 0, -90);
                        }
                        else
                        {
                            transform.Rotate(0, 0, 90);
                        }
                    }
                    else
                    {
                        transform.Rotate(0, 0, -90);
                    }
                }

            }
            

        }
        //keep object keeping being down with and without pressing button
        //use fall to record the last movement,and when Time.time grows,Time.time grows
        //finally when their gap is lager than fallspeed, the falling sentence is been executed
        else if(Input.GetKeyDown(KeyCode.DownArrow) || Time.time - fall >= fallspeed)
        {
            transform.position += new Vector3(0, -1, 0);

            // to record the last moving time
            fall = Time.time;   

            if (CheckIfValid())
            {
                FindObjectOfType<Game>().UpdateGrid(this);  
                if(Input.GetKeyDown(KeyCode.DownArrow))
                {
                    playMoveSound();
                }
            }
            else
            {
                transform.position -= new Vector3(0, -1, 0);

                //check if there is some rows filled, if they are, delete them
                FindObjectOfType<Game>().DeleteRow();   

                //check if there is some objects beyond Height - 1
                if(FindObjectOfType<Game>().CheckIfAboveGrid(this))
                {
                    FindObjectOfType<Game>().Gameover();
                }

                //freeze the objects not to move
                enabled = false;

                playLandSound();

                //spawn the next tetromino
                FindObjectOfType<Game>().SpawnNextTetromino();

                // get the extra score added(currentScore is a static variable)
                Game.currentScore += individualScore;

                
            }

        }
    }

    //audio played function
    void playMoveSound()
    {
        audioSource.PlayOneShot(moveSound);
    }

    void playLandSound()
    {
        audioSource.PlayOneShot(landSound);
    }

    void playRotateSound()
    {
        audioSource.PlayOneShot(rotateSound);
    }

    //check if the objects are beyond edge
    bool CheckIfValid()
    {
        foreach (Transform mino in transform)
        {
            //get the position (x,y) from float to integer
            Vector2 pos = (FindObjectOfType<Game>().GetInteger(mino.position));

            //check the edge situation
            if (FindObjectOfType<Game>().CheckEdgePosition(pos) == false)
            {
                return false;
            }

            //if there is transform in grid[x,y], and it does not match with the parent of the grid[x,y] 
            if (FindObjectOfType<Game>().GetTransformAtGridPosition(pos) != null &&
                FindObjectOfType<Game>().GetTransformAtGridPosition(pos).parent != transform)
            {
                return false;
            }

        }
        return true;

    }
}
