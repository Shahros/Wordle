using UnityEngine;

public class Board : MonoBehaviour
{
    private static readonly KeyCode[] SUPPORTED_KEYS = new KeyCode[] {
        KeyCode.A, KeyCode.B, KeyCode.C, KeyCode.D, KeyCode.E, KeyCode.F,
        KeyCode.G, KeyCode.H, KeyCode.I, KeyCode.J, KeyCode.K, KeyCode.L,
        KeyCode.M, KeyCode.N, KeyCode.O, KeyCode.P, KeyCode.Q, KeyCode.R,
        KeyCode.S, KeyCode.T, KeyCode.U, KeyCode.V, KeyCode.W, KeyCode.X,
        KeyCode.Y, KeyCode.Z,
    };//array of keys which are allowed

    private Row[] rows;     //array all available rows
    private int rowIndex;
    private int columnIndex;

    private string[] solutions;     //pick random solution
    private string[] validWords;    //words allowed to enter
    private string word;

    [Header("Tiles")]
    public Tile.State emptyState;       //tile is empty
    public Tile.State occupiedState;    //there is a character in tile
    public Tile.State correctState;     //Correct letter in tile
    public Tile.State wrongSpotState;   //Correct letter but wrong place
    public Tile.State incorrectState;   //wrong letter

    [Header("UI")]
    public GameObject tryAgainButton;
    public GameObject newWordButton;
    public GameObject invalidWordText;

    private void Awake()
    {
        rows = GetComponentsInChildren<Row>();
    }

    private void Start()
    {
        LoadData();
        NewGame();
    }

    private void LoadData()
    {
        TextAsset textFile = Resources.Load("official_wordle_common") as TextAsset;
        solutions = textFile.text.Split("\n");      // read file and add all words from solution file and seperate it with \n newline.

        textFile = Resources.Load("official_wordle_all") as TextAsset;
        validWords = textFile.text.Split("\n");     // read file and add all allowed words from all file.
    }

    public void NewGame()   //create new game and set new random word
    {
        ClearBoard();
        SetRandomWord();

        enabled = true;
    }

    public void TryAgain()  //clear board but donot set new word
    {
        ClearBoard();

        enabled = true;
    }

    private void SetRandomWord()        // Set random word from solution list
    {
        word = solutions[Random.Range(0, solutions.Length)];
        word = word.ToLower().Trim();   //Convert all to lower case and just a precaution to trim extra spaces at the end.
    }

    private void Update()
    {
        Row currentRow = rows[rowIndex];        //temporary value of current row

        if (Input.GetKeyDown(KeyCode.Backspace))        //when player remove last letter using backspace
        {
            columnIndex = Mathf.Max(columnIndex - 1, 0);        //move back 1 tile

            currentRow.tiles[columnIndex].SetLetter('\0');      //set that letter to null
            currentRow.tiles[columnIndex].SetState(emptyState);

            invalidWordText.SetActive(false);
        }
        else if (columnIndex >= currentRow.tiles.Length)        // if player have filled all tiles in current row player can submit row
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                SubmitRow(currentRow);      //submit current row to check and change row
            }
        }
        else
        {
            for (int i = 0; i < SUPPORTED_KEYS.Length; i++)
            {
                if (Input.GetKeyDown(SUPPORTED_KEYS[i]))
                {
                    currentRow.tiles[columnIndex].SetLetter((char)SUPPORTED_KEYS[i]);       //convert pressed key to character and set it to the current tile
                    currentRow.tiles[columnIndex].SetState(occupiedState);          //set tile state to already filled

                    columnIndex++;      //move to next tile
                    break;
                }
            }
        }
    }

    private void SubmitRow(Row row)
    {
        if (!IsValidWord(row.Word))     //verify if word is valid or not
        {
            invalidWordText.SetActive(true);
            return;
        }

        string remaining = word;

        // check correct/incorrect letters first
        for (int i = 0; i < row.tiles.Length; i++)
        {
            Tile tile = row.tiles[i];       //current tile to check

            if (tile.letter == word[i])     //compare tile letter and letter at the same index of word
            {
                tile.SetState(correctState);

                remaining = remaining.Remove(i, 1);     //remove character if it is correct
                remaining = remaining.Insert(i, " ");   //keep string at same length
            }
            else if (!word.Contains(tile.letter))   //wrong letter
            {
                tile.SetState(incorrectState);
            }
        }

        // check wrong spots after
        for (int i = 0; i < row.tiles.Length; i++)
        {
            Tile tile = row.tiles[i];

            if (tile.state != correctState && tile.state != incorrectState) //if the tile is neither correct nor wrong
            {
                if (remaining.Contains(tile.letter))        //if it contains letter then it is in wrong spot
                {
                    tile.SetState(wrongSpotState);

                    int index = remaining.IndexOf(tile.letter);
                    remaining = remaining.Remove(index, 1);
                    remaining = remaining.Insert(index, " ");
                }
                else
                {
                    tile.SetState(incorrectState);      //there are not multiple same letters so it is incorrect
                }
            }
        }

        if (HasWon(row))
        {
            enabled = false;
        }

        rowIndex++;     //move to next row
        columnIndex = 0;    //start from first column

        if (rowIndex >= rows.Length)        //if player have exhausted all guess 
        {
            enabled = false;            //disable this script
        }
    }

    private bool IsValidWord(string word)
    {
        for (int i = 0; i < validWords.Length; i++) //check in valid word array
        {
            if (validWords[i] == word)
            {
                return true;
            }
        }

        return false;
    }

    private bool HasWon(Row row)
    {
        for (int i = 0; i < row.tiles.Length; i++)      //check all tile if all are correct player have won otherwise not
        {
            if (row.tiles[i].state != correctState)
            {
                return false;
            }
        }

        return true;
    }

    private void ClearBoard()       //set state of all tiles in each row to empty
    {
        for (int row = 0; row < rows.Length; row++)
        {
            for (int col = 0; col < rows[row].tiles.Length; col++)
            {
                rows[row].tiles[col].SetLetter('\0');
                rows[row].tiles[col].SetState(emptyState);
            }
        }

        rowIndex = 0;
        columnIndex = 0;
    }

    private void OnEnable()     //on start game disable these
    {
        tryAgainButton.SetActive(false);
        newWordButton.SetActive(false);
    }

    private void OnDisable()    //On game end show user options to do next
    {
        tryAgainButton.SetActive(true);
        newWordButton.SetActive(true);
    }
}
