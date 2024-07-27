using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockController : MonoBehaviour
{
    public float block = 0;
    public float speed = 1f;
    float speedArrowLR = 0.05f; // toc do block khi nhan left right arrow
    float speedArrowD = 0.02f; // toc do block khi nhan down arrow
    float timeReplay = 0.1f; // thoi gian
    float timeLR = 0;
    float timeD = 0;
    float timeClick = 0;
    bool moveLR = false;
    bool moveD = false;

    public bool allowRotate = true;
    public bool rotateOnce = false;

    public AudioClip moveSouth;
    public AudioClip rotateSouth;
    public AudioClip fallSouth;
    private AudioSource south;

    private void Start()
    {
        south = GetComponent<AudioSource>();
    }
    private void southMove()
    {
        south.PlayOneShot(moveSouth);
    }
    private void southFall()
    {
        south.PlayOneShot(fallSouth);
    }
    private void southRotate()
    {
        south.PlayOneShot(rotateSouth);
    }
    // Update is called once per frame
    void Update()
    {
        CheckClick();
    }

    void MoveLeft()
    {
        southMove();
        if (moveLR)
        {
            if (timeClick < timeReplay)
            {
                timeClick += Time.deltaTime;
                return;
            }
            if (timeLR < speedArrowLR)
            {
                timeLR += Time.deltaTime;
                return;
            }
        }
        else
        {
            moveLR = true;
        }
        timeLR = 0;
        transform.position += new Vector3(-1.02f, 0, 0);
        if (!CheckCollision())
        {
            transform.position += new Vector3(1.02f, 0, 0);
        }
    }
    void MoveRight()
    {
        southMove();
        if (moveLR)
        {
            if (timeClick < timeReplay)
            {
                timeClick += Time.deltaTime;
                return;
            }
            if (timeLR < speedArrowLR)
            {
                timeLR += Time.deltaTime;
                return;
            }
        }
        else
        {
            moveLR = true;
        }
        timeLR = 0;
        transform.position += new Vector3(1.02f, 0, 0);
        if (!CheckCollision())
        {
            transform.position += new Vector3(-1.02f, 0, 0);
        }
    }
    //Quay
    void rotate()
    {
        if (allowRotate)
        {
            southRotate();
            if (rotateOnce)
            {
                if (transform.rotation.eulerAngles.z >= 90) transform.Rotate(0, 0, -90);
                else transform.Rotate(0, 0, 90);
            }
            else
            {
                transform.Rotate(0, 0, 90);
            }
        }

        if (!CheckCollision())
        {
            transform.Rotate(0, 0, -90);
        }
    }
    //Rơi xuống
    void drop()
    {
        if (moveD)
        {
            if (timeClick < timeReplay)
            {
                timeClick += Time.deltaTime;
                return;
            }
            if (timeD < speedArrowD)
            {
                timeD += Time.deltaTime;
                return;
            }
            southMove();
        }
        else
        {
            moveD = true;
        }
        timeD = 0;
        transform.position += new Vector3(0, -1.02f, 0);

        if (CheckCollision())
        {
            FindObjectOfType<BienController>().updateGrid(this);
        }
        else
        {
            southFall();
            transform.position += new Vector3(0, 1.02f, 0);
            FindObjectOfType<BienController>().deleteRow();
            if (FindObjectOfType<BienController>().checkGameOver(this)) { FindObjectOfType<BienController>().ActiveGameOver(); }
            enabled = false;
            FindObjectOfType<BienController>().createBlock();
        }
        block = Time.time;
    }

    void CheckClick()
    {
        if (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.DownArrow))
        {
            moveLR = false;
            moveD = false;
            timeLR = 0;
            timeD = 0;
            timeClick = 0;
        }
        if (Input.GetKey(KeyCode.LeftArrow)) MoveLeft();
        if (Input.GetKey(KeyCode.RightArrow)) MoveRight();
        if (Input.GetKeyDown(KeyCode.UpArrow)) rotate();
        if (Input.GetKey(KeyCode.DownArrow) || Time.time - block >= speed) drop();
    }
    public bool CheckCollision()
    {
        foreach (Transform block in transform)
        {
            Vector2 kt = FindObjectOfType<BienController>().Round(block.position);
            if (FindObjectOfType<BienController>().inGrid(kt) == false) return false;
            if (FindObjectOfType<BienController>().BlockExisted(kt) != null && FindObjectOfType<BienController>().BlockExisted(kt).parent != transform) return false;
        }
        return true;
    }
}
