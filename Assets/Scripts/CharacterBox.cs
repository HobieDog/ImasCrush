using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBox : MonoBehaviour
{
    const float drag_range_split = 1.6f;
    const float drag_range = 0.8f;
    const float drag_speed = 0.2f;

    const float drop_speed = 0.08f;
    const float drop_time = 0.01f;

    GameManager gameManager;
    SpriteRenderer spriteRenderer;
    BoxCollider2D boxCollider;
    Vector3 originPos;
    User user;

    int dirV = 0;
    int dirH = 0;
    int row, column;
    float vertical = 0;
    float horizontal = 0;
    bool beStart = false;
    bool beDrop = false;
    bool bomb = false;
    bool fix = false;

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        user = GameObject.Find("GameManager").GetComponent<User>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
        originPos = transform.position;
    }

    private void Update()
    {
        if (!beDrop && row > 0 && gameManager.GetCharacterTile()[row - 1, column] == null)
        {
            StartCoroutine("Drop");
        }
            
    }

    private void OnMouseUp()
    {
        bool bOne = false;
        bool bTwo = false;

        //상하
        if(vertical != 0)
        {
            if(originPos.y + drag_range / drag_range_split < transform.position.y || originPos.y - drag_range / drag_range_split > transform.position.y)
            {
                if (row + dirV < 0 || row + dirV > 6)
                {
                    transform.position = originPos;
                    return;
                }

                GameObject ChangeTile = gameManager.GetCharacterTile()[row + dirV, column];
                bTwo = gameManager.CheckCharacter(row + dirV, column, dirV, dirH, transform.tag);
                bOne = gameManager.CheckCharacter(row, column, -dirV, dirH, ChangeTile.tag);

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
                return;
            }
        }

        //좌우 이동
        else if (horizontal != 0)
        {
            //절반이상 이동했다면
            if (originPos.x + drag_range / drag_range_split < transform.position.x || originPos.x - drag_range / drag_range_split > transform.position.x)
            {
                if (column + dirH < 0 || column + dirH > 6)
                {
                    transform.position = originPos;
                    return;
                }

                GameObject ChangeTile = gameManager.GetCharacterTile()[row, column + dirH];
                bTwo = gameManager.CheckCharacter(row, column + dirH, dirV, dirH, transform.tag);
                bOne = gameManager.CheckCharacter(row, column, dirV, -dirH, ChangeTile.tag);

                if ((bOne || bTwo) == false)
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
        transform.position = originPos;
    }

    private void OnMouseDown()
    {
        if (bomb)
            Bomb();
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

    //Switch Tile
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

    //Drag Direction Check
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

    public bool IsDropping()
    {
        return beDrop;
    }

    IEnumerator Drop()
    {
        beDrop = true;

        if (!beStart)
        {
            yield return new WaitForSeconds(GameManager.time - 0.2f);
            beStart = true;
        }

        if (row > 0 && gameManager.GetCharacterTile()[row - 1, column] == null)
        {
            gameManager.GetCharacterTile()[row - 1, column] = gameManager.GetCharacterTile()[row, column];
            gameManager.GetCharacterTile()[row, column] = null;
            row--;

            while (originPos.y - transform.position.y < 0.8f)  //0.8f = GameManger.space
            {
                Vector3 movement = new Vector3(0, drop_speed, 0);
                transform.position -= movement;

                yield return new WaitForSeconds(drop_time);
            }
            Vector3 tempVec = new Vector3(originPos.x, originPos.y - 0.8f, originPos.z);
            originPos = tempVec;
        }
        gameManager.CheckCharacter(row, column, 0, 0, transform.tag);
        beDrop = false;

        transform.position = originPos;
        beStart = false;
    }

    public void SetColor(float r, float g, float b, float a)
    {
        Color color = new Color(r, g, b, a);
        GetComponent<SpriteRenderer>().color = color;
    }

    public void ActiveBomb()
    {
        bomb = true;
        SetSpriteBomb();
    }

    public void SetSpriteBomb()
    {
        spriteRenderer.sprite = Resources.Load<Sprite>("bomb");
    }

    void Bomb()
    {
        int cnt = 0;

        for (int i = 1; i < GameManager.width; ++i)
        {
            if (column + i < GameManager.width && gameManager.GetCharacterTile()[row, column + i])
            {
                cnt++;
                gameManager.GetCharacterTile()[row, column + i].GetComponentInChildren<DestroyTile>().StartCoroutine("Destroy");
            }
            if (column - i >= 0 && gameManager.GetCharacterTile()[row, column - i])
            {
                cnt++;
                gameManager.GetCharacterTile()[row, column - i].GetComponentInChildren<DestroyTile>().StartCoroutine("Destroy");
            }
            if (row + i < GameManager.height && gameManager.GetCharacterTile()[row + i, column])
            {
                cnt++;
                gameManager.GetCharacterTile()[row + i, column].GetComponentInChildren<DestroyTile>().StartCoroutine("Destroy");
            }
            if (row - i >= 0 && gameManager.GetCharacterTile()[row - i, column])
            {
                cnt++;
                gameManager.GetCharacterTile()[row - i, column].GetComponentInChildren<DestroyTile>().StartCoroutine("Destroy");
            }
        }
        user.AddScore(cnt);
        gameManager.GetCharacterTile()[row, column].GetComponentInChildren<DestroyTile>().StartCoroutine("Destroy");
    }
}
