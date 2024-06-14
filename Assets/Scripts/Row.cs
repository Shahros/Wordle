using UnityEngine;

public class Row : MonoBehaviour
{
    public Tile[] tiles { get; private set; }       //array of tiles on each row

    public string Word  //create word from all letters in each tile in single row
    {
        get
        {
            string word = "";

            for (int i = 0; i < tiles.Length; i++)
            {
                word += tiles[i].letter;
            }

            return word;
        }
    }

    private void Awake()
    {
        tiles = GetComponentsInChildren<Tile>();    //Initialize all tiles
    }

}
