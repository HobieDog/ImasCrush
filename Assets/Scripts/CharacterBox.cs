using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBox : MonoBehaviour
{
    const float drag_range_split = 1.6f;
    const float drag_range = 0.8f;
    const float drag_speed = 0.2f;

    GameManager gameManager;
    SpriteRenderer spriteRenderer;
    BoxCollider2D boxCollider;
    Vector3 originPos;

    float horizontal = 0;
    float vertical = 0;
    int row, column;
    bool beClicked = false;
    bool fix = false;

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        boxCollider = GetComponent<BoxCollider2D>();
        originPos = transform.position;
    }

    void Update()
    {

    }

    private void OnMouseUp()
    {
        //좌우 이동
        if(horizontal != 0)
        {
            //절반이상 이동했다면
            if(originPos.x + drag_range / drag_range_split < transform.position.x || originPos.x - drag_range / drag_range_split > transform.position.x)
            {
                transform.position = gameManager.GetCharacterTile()[row, column + (int)(horizontal / horizontal)].GetComponent<CharacterBox>().GetOriginPos();
                gameManager.GetCharacterTile()[row, column + (int)(horizontal / horizontal)].transform.position = originPos;

                originPos = transform.position;
                gameManager.GetCharacterTile()[row, column + (int)(horizontal / horizontal)].GetComponent<CharacterBox>().SetOriginPos(
                   gameManager.GetCharacterTile()[row, column + (int)(horizontal / horizontal)].transform.position);

                // board 배열의 위치를 바꾼다.
                GameObject tempObj = gameManager.GetCharacterTile()[row, column];
                gameManager.GetCharacterTile()[row, column] = gameManager.GetCharacterTile()[row, column + (int)(horizontal / horizontal)];
                gameManager.GetCharacterTile()[row, column + (int)(horizontal / horizontal)] = tempObj;

                gameManager.GetCharacterTile()[row, column].GetComponent<CharacterBox>().SetArr(row, column - (int)(horizontal / horizontal));
                gameManager.GetCharacterTile()[row, column + (int)(horizontal / horizontal)].GetComponent<CharacterBox>().SetArr(row, column + (int)(horizontal / horizontal));
            }
            //아니라면
            else
            {
                transform.position = originPos;
                fix = false;
            }
        }

        else if(vertical != 0)
        {
            if(originPos.y + drag_range / drag_range_split < transform.position.y || originPos.y - drag_range / drag_range_split > transform.position.y)
            {
                transform.position = gameManager.GetCharacterTile()[row + (int)(vertical / vertical), column].GetComponent<CharacterBox>().GetOriginPos();
                gameManager.GetCharacterTile()[row + (int)(vertical / vertical), column].transform.position = originPos;

                originPos = transform.position;
                gameManager.GetCharacterTile()[row + (int)(vertical / vertical), column].GetComponent<CharacterBox>().SetOriginPos(
                   gameManager.GetCharacterTile()[row + (int)(vertical / vertical), column].transform.position);

                // board 배열의 위치를 바꾼다.
                GameObject tempObj = gameManager.GetCharacterTile()[row, column];
                gameManager.GetCharacterTile()[row, column] = gameManager.GetCharacterTile()[row + (int)(vertical / vertical), column];
                gameManager.GetCharacterTile()[row + (int)(vertical / vertical), column] = tempObj;

                gameManager.GetCharacterTile()[row, column].GetComponent<CharacterBox>().SetArr(row - (int)(vertical / vertical), column);
                gameManager.GetCharacterTile()[row + (int)(vertical / vertical), column].GetComponent<CharacterBox>().SetArr(row + (int)(vertical / vertical), column);
            }
            else
            {
                transform.position = originPos;
                fix = false;
            }
        }

        beClicked = false;
    }

    private void OnMouseDrag()
    {
        float dragHorizon = Input.GetAxis("Mouse X");
        float dragVertical = Input.GetAxis("Mouse Y");
        Vector3 pos = transform.position;
        int d_row = row;
        int d_column = column;

        //상하
        if (Mathf.Abs(dragVertical) > Mathf.Abs(dragHorizon) && !fix)
        {
            vertical = drag_speed;
            horizontal = 0;
            fix = true;
        }
        //좌우
        else if (Mathf.Abs(dragVertical) < Mathf.Abs(dragHorizon) && !fix)
        {
            horizontal = drag_speed;
            vertical = 0;
            fix = true;
        }
        Vector3 nowPos = transform.position;
        if (originPos.x + drag_range > nowPos.x + dragHorizon * horizontal && originPos.x - drag_range < nowPos.x + dragHorizon * horizontal &&
            originPos.y + drag_range > nowPos.y + dragVertical * vertical && originPos.y - drag_range < nowPos.y + dragVertical * vertical)
            transform.position += new Vector3(dragHorizon * horizontal, dragVertical * vertical, 0);
    }

    public void SetOriginPos(Vector3 originPos)
    {
        this.originPos = originPos;
    }

    public Vector3 GetOriginPos()
    {
        return originPos;
    }

    public void SetArr(int row, int column)
    {
        this.row = row;
        this.column = column;
    }

    public void GetArr(ref int row, ref int column)
    {
        row = this.row;
        column = this.column;
    }
}
