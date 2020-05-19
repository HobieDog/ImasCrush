using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    const int width = 7;
    const int height = 7;

    const float create_delay = 0.2f;
    const float create_y = 0.8f;
    const float start_y = -2.9f;
    const float start_x = 2.4f;
    const float space = 0.8f;

    const float fade_delay = 0.001f;
    const float fade_value = 0.1f;

    enum CharacterType {Anna, Mirai, Miya, Serika, Shiho, Yuriko};

    public GameObject[] characters;
    GameObject[,] board;

    bool beStart = true;

    void Start()
    {
        board = new GameObject[height, width];

        if (beStart)
            Create();
    }

    void Update()
    {
        
    }

    void Create()
    {
        for(int i = 0;i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                int val = Random.Range(0, 6);
                Vector3 pos = new Vector3(j * space - start_x, i * create_y + start_y, 0.0f);

                board[i, j] = Instantiate(characters[val], pos, Quaternion.identity);
                board[i, j].GetComponent<CharacterBox>().SetArr(i, j);

                Color color = board[i, j].GetComponent<SpriteRenderer>().color;
                board[i, j].GetComponent<SpriteRenderer>().color = new Vector4(color.r, color.g, color.b, 0.0f);
            }
        }
        beStart = false;
        CheckOverlap();
        StartCoroutine("FadeIn");
    }

    
    //remove duplicated tile
    void CheckOverlap()
    {
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                bool beOverlap = false;

                if(0 < i && i < height - 1 && 0 < j && j < width - 1)
                {
                    if (board[i - 1, j].tag == board[i, j].tag && board[i + 1, j].tag == board[i, j].tag ||
                        board[i, j - 1].tag == board[i, j].tag && board[i, j + 1].tag == board[i, j].tag)
                    {
                        beOverlap = true;
                    }
                }
                else if ((i == 0 || i == height - 1) && 0 < j && j > width - 1)
                {
                    if (board[i, j - 1].tag == board[i, j].tag && board[i, j + 1].tag == board[i, j].tag)
                    {
                        beOverlap = true;
                    }
                }
                else if(0 < i && i < height - 1 && (j == 0 || j == width - 1))
                {
                    if (board[i - 1, j].tag == board[i, j].tag && board[i + 1, j].tag == board[i, j].tag)
                    {
                        beOverlap = true;
                    }
                }

                if (beOverlap)
                {
                    //Tile Remove
                    int val = CheckTagWithEnum(board[i, j].tag);
                    DestroyObject(board[i, j]);

                    //Change New Tile
                    Vector3 pos = new Vector3(j * space - start_x, i * create_y + start_y, 0.0f);
                    board[i, j] = Instantiate(characters[val], pos, Quaternion.identity);

                    Color color = board[i, j].GetComponent<SpriteRenderer>().color;
                    board[i, j].GetComponent<SpriteRenderer>().color = new Vector4(color.r, color.g, color.b, 0.0f);
                    board[i, j].GetComponent<CharacterBox>().SetArr(i, j);
                }
            }
        }
    }
    

    int CheckTagWithEnum(string str)
    {
        CharacterType characterType = (CharacterType)System.Enum.Parse(typeof(CharacterType), str);
        int randCharacter;

        do
        {
            randCharacter = Random.Range(0, 6);
        } while ((int)characterType == randCharacter);

        return randCharacter;
    }

    //Fade In Effect
    IEnumerator FadeIn()
    {
        while (board[6, 6].GetComponent<SpriteRenderer>().color.a != 1.0f)
        {
            for (int i = 0; i < height; ++i)
            {
                for (int j = 0; j < width; ++j)
                {
                    board[i, j].GetComponent<SpriteRenderer>().color += new Color(0.0f, 0.0f, 0.0f, fade_value);
                }
                yield return new WaitForSeconds(fade_delay);
            }
        }
    }

    public GameObject[,] GetCharacterTile()
    {
        return board;
    }

    //Check Tiles
    public bool CheckCharacter(int row, int column, int dirV, int dirH, string type)
    {
        Queue<GameObject> queueW = new Queue<GameObject>();
        Queue<GameObject> queueH = new Queue<GameObject>();
        bool L, R, U, D, result1, result2;
        result1 = result2 = L = R = U = D = false;

        queueW.Enqueue(board[row - dirV, column - dirH]);
        queueH.Enqueue(board[row - dirV, column - dirH]);

        // 맞는 짝을 상,하 탐색한다
        for (int i = 1; i < 7; ++i)
        {
            if (!R && column + i < 7 && board[row, column + i].tag == type)
                queueW.Enqueue(board[row, column + i]);
            else
                R = true;
            if (!L && column - i >= 0 && board[row, column - i].tag == type)
                queueW.Enqueue(board[row, column - i]);
            else
                L = true;
            if (!U && row + i < 7 && board[row + i, column].tag == type)
                queueH.Enqueue(board[row + i, column]);
            else
                U = true;
            if (!D && row - i >= 0 && board[row - i, column].tag == type)
                queueH.Enqueue(board[row - i, column]);
            else
                D = true;
        }

        Debug.Log("cnt1 : " + queueW.Count);
        Debug.Log("cnt2 : " + queueH.Count);

        if (queueW.Count >= 3)
        {
            while (queueW.Count > 0)
            {
                queueW.Dequeue().GetComponentInChildren<DestroyTile>().StartCoroutine("Destroy");
            }
            result1 = true;
        }

        if (queueH.Count >= 3)
        {
            while (queueH.Count > 0)
            {
                queueH.Dequeue().GetComponentInChildren<DestroyTile>().StartCoroutine("Destroy");
            }
            result2 = true;
        }
        return result1 || result2;
    }
}
