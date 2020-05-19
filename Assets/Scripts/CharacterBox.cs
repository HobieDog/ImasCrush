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

    int dirV = 0;
    int dirH = 0;
    int row, column;
    bool beClicked = false;
    bool fix = false;
    float vertical = 0;
    float horizontal = 0;

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
        originPos = transform.position;
    }

    void Update()
    {

    }

    void Switching()
    {
        GameObject ChangeTile = gameManager.GetCharacterTile()[row + dirV, column + dirH];

        //Change Tile
        transform.position = ChangeTile.GetComponent<CharacterBox>().GetOriginPos();
        ChangeTile.transform.position = originPos;

        //Tiles OriginPos Reset
        originPos = transform.position;
        ChangeTile.GetComponent<CharacterBox>().SetOriginPos(ChangeTile.transform.position);

        //Change Board Array
        GameObject tempObj = gameManager.GetCharacterTile()[row, column];
        gameManager.GetCharacterTile()[row, column] = ChangeTile;
        gameManager.GetCharacterTile()[row + dirV, column + dirH] = tempObj;

        gameManager.GetCharacterTile()[row, column].GetComponent<CharacterBox>().SetArr(row, column);
        gameManager.GetCharacterTile()[row + dirV, column + dirH].GetComponent<CharacterBox>().SetArr(row + dirV, column + dirH);
    }

    private void OnMouseUp()
    {
        bool bOne = false;
        bool bTwo = false;

        //좌우 이동
        if (horizontal != 0)
        {
            //절반이상 이동했다면
            if(originPos.x + drag_range / drag_range_split < transform.position.x || originPos.x - drag_range / drag_range_split > transform.position.x)
            {
                GameObject ChangeTile = gameManager.GetCharacterTile()[row, column + dirH];

                bTwo = gameManager.CheckCharacter(row, column + dirH, dirV, dirH, transform.tag);
                bOne = gameManager.CheckCharacter(row, column, 0, 0, ChangeTile.tag);

                if((bOne || bTwo) == false)
                {
                    transform.position = originPos;
                    fix = false;
                    return;
                }
                Switching();
            }
            //아니라면
            else
            {
                transform.position = originPos;
                fix = false;
            }
        }

        //상하
        else if(vertical != 0)
        {
            if(originPos.y + drag_range / drag_range_split < transform.position.y || originPos.y - drag_range / drag_range_split > transform.position.y)
            {
                GameObject ChangeTile = gameManager.GetCharacterTile()[row + dirV, column];

                bTwo = gameManager.CheckCharacter(row + dirV, column, dirV, dirH, transform.tag);
                bOne = gameManager.CheckCharacter(row, column, 0, 0, ChangeTile.tag);

                if ((bOne || bTwo) == false)
                {
                    transform.position = originPos;
                    fix = false;
                    return;
                }
                Switching();
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
        int t_row = row;
        int t_column = column;

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

        DecideDirection(dragVertical, dragHorizon);

        Vector3 nowPos = transform.position;
        if (originPos.x + drag_range > nowPos.x + dragHorizon * horizontal && originPos.x - drag_range < nowPos.x + dragHorizon * horizontal &&
            originPos.y + drag_range > nowPos.y + dragVertical * vertical && originPos.y - drag_range < nowPos.y + dragVertical * vertical)
            transform.position += new Vector3(dragHorizon * horizontal, dragVertical * vertical, 0);
    }

    void DecideDirection(float dragV, float dragH)
    {
        if (vertical > 0)
        {
            if (dragV > 0)
                dirV = 1;
            else if (dragV < 0)
                dirV = -1;
            dirH = 0;
        }
        else
        {
            if (dragH > 0)
                dirH = 1;
            else if (dragH < 0)
                dirH = -1;
            dirV = 0;
        }
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
