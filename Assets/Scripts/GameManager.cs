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

    public const float time = 0.2f;

    enum CharacterType {Anna, Mirai, Miya, Serika, Shiho, Yuriko};

    public GameObject[] characters;
    GameObject[,] board;
    User user;

    bool beStart = true;

    bool beRefill = false;
    bool beDrop = false;

    void Start()
    {
        user = GetComponent<User>();
        board = new GameObject[height, width];

        if (beStart)
            Create();
    }

    void Update()
    {
        if (!beRefill)
            StartCoroutine("Refill");
    }

    public bool IsDropping()
    {
        return beDrop;
    }

    //Create Tile
    void Create()
    {
        for(int i = 0;i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                CreateCharacterTile(j, i, true);
            }
        }
        beStart = false;
        CheckOverlap();
        StartCoroutine("FadeIn");
    }

    void CreateCharacterTile(int x, int y, bool alphaReset)
    {
        int val = Random.Range(0, 6);
        Vector3 pos = new Vector3(x * space - start_x, y * create_y + start_y, 0.0f);

        board[y, x] = Instantiate(characters[val], pos, Quaternion.identity);
        board[y, x].GetComponent<CharacterBox>().SetArr(y, x);

        if (alphaReset)
        {
            Color color = board[y, x].GetComponent<SpriteRenderer>().color;
            board[y, x].GetComponent<SpriteRenderer>().color = new Vector4(color.r, color.g, color.b, 0.0f);
        }
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
                    if (board[i, j])
                        board[i, j].GetComponent<SpriteRenderer>().color += new Color(0.0f, 0.0f, 0.0f, fade_value);
                }
                yield return new WaitForSeconds(fade_delay);
            }
        }
    }

    //Overlap tile Check
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
                else if ((i == 0 || i == height - 1) && 0 < j && j < width - 1)
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
                    //Check Tag
                    int val = CheckTagWithEnum(board[i, j].tag, j, i);

                    //Change New Tile
                    Vector3 pos = new Vector3(j * space - start_x, i * create_y + start_y, 0.0f);
                    board[i, j] = Instantiate(characters[val], pos, Quaternion.identity);

                    //New Tile Alpha Reset
                    Color color = board[i, j].GetComponent<SpriteRenderer>().color;
                    board[i, j].GetComponent<SpriteRenderer>().color = new Vector4(color.r, color.g, color.b, 0.0f);
                    board[i, j].GetComponent<CharacterBox>().SetArr(i, j);
                }
            }
        }
    }
    
    //Tile Tag Check
    int CheckTagWithEnum(string str, int x, int y)
    {
        CharacterType characterType = (CharacterType)System.Enum.Parse(typeof(CharacterType), str);
        int[] cntCharacter = new int[6];
        int randCharacter;

        for (int i = -2; i < 2; i++)
        {
            for (int j = -2; j < 2; j++)
            {
                int tx = x + j, ty = y + i;

                if (tx >= 0 && tx < 7 && ty >= 0 && ty < 7)
                {
                    int index = (int)System.Enum.Parse(typeof(CharacterType), board[ty, tx].tag);
                    ++cntCharacter[index];
                }
            }
        }

        do
        {
            randCharacter = Random.Range(0, 6);
        } while (cntCharacter[randCharacter] > 0);

        return randCharacter;
    }

    public GameObject[,] GetCharacterTile()
    {
        return board;
    }

    //Check Tile & Destroy Tile
    public bool CheckCharacter(int row, int column, int dirV, int dirH, string type)
    {
        Queue<GameObject> queueW = new Queue<GameObject>();
        Queue<GameObject> queueH = new Queue<GameObject>();
        bool L, R, U, D, result1, result2;
        result1 = result2 = L = R = U = D = false;

        if (row + (dirV * -1) >= 0 && row + (dirV * -1) < height &&
            column + (dirH * -1) >= 0 && column + (dirH * -1) < width)
        {
            if (board[row + (dirV * -1), column + (dirH * -1)] == null)
                return false;
        }

        queueW.Enqueue(board[row + (dirV * -1), column + (dirH * -1)]);
        queueH.Enqueue(board[row + (dirV * -1), column + (dirH * -1)]);

        int pivotRow = row + (dirV * -1);
        int pivotColumn = column + (dirH * -1);

        // 맞는 짝을 상,하 탐색한다
        for (int i = 1; i < 7; ++i)
        {
            if (column + i != pivotColumn)
                if (!R && column + i < 7 && board[row, column + i]
                    && !board[row, column + i].GetComponent<CharacterBox>().IsDropping() && board[row, column + i].tag == type)
                    queueW.Enqueue(board[row, column + i]);
                else
                    R = true;
            else
                R = true;

            if (column - i != pivotColumn)
                if (!L && column - i >= 0 && board[row, column - i]
                    && !board[row, column - i].GetComponent<CharacterBox>().IsDropping() && board[row, column - i].tag == type)
                    queueW.Enqueue(board[row, column - i]);
                else
                    L = true;
            else
                L = true;

            if (row + i != pivotRow)
                if (!U && row + i < 7 && board[row + i, column]
                    && !board[row + i, column].GetComponent<CharacterBox>().IsDropping() && board[row + i, column].tag == type)
                    queueH.Enqueue(board[row + i, column]);
                else
                    U = true;
            else
                U = true;

            if (row - i != pivotRow)
                if (!D && row - i >= 0 && board[row - i, column]
                    && !board[row - i, column].GetComponent<CharacterBox>().IsDropping() && board[row - i, column].tag == type)
                    queueH.Enqueue(board[row - i, column]);
                else
                    D = true;
            else
                D = true;
        }

        if (queueW.Count >= 3)
        {
            //Score Up
            user.AddScore(queueW.Count);

            while (queueW.Count > 0)
            {
                queueW.Dequeue().GetComponentInChildren<DestroyTile>().StartCoroutine("Destroy");
            }
            result1 = true;
        }

        if (queueH.Count >= 3)
        {
            //if tile destroyed result1
            if (result1)
                queueH.Dequeue();

            //Score Up
            user.AddScore(queueH.Count);

            while (queueH.Count > 0)
            {
                queueH.Dequeue().GetComponentInChildren<DestroyTile>().StartCoroutine("Destroy");
            }
            result2 = true;
        }
        return result1 || result2;
    }
    
    IEnumerator Refill()
    {
        beRefill = true;
        bool beEmpty = false;

        while (true)
        {
            yield return new WaitForSeconds(time);
            for (int i = 0; i < 7; ++i)
            {
                //If Find Empty Space
                if (board[6, i] == null)
                {
                    // Create New Tile
                    beEmpty = true;
                    CreateCharacterTile(i, 6, false);
                }
            }
            // If Don't Find Empty Space
            if (!beEmpty)
            {
                beRefill = false;
                beDrop = false;

                for (int i = 0; i < width; ++i)
                {
                    //Check Overlap Tile
                    CheckCharacter(6, i, 0, 0, board[6, i].tag);
                }
                break;
            }
            beEmpty = false;
        }
    }
    
}
