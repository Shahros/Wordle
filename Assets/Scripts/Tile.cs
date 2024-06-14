using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    [System.Serializable]
    public class State
    {
        public Color fillColor;     //for the image of tile
        public Color outlineColor;  //for the outline of tile
    }

    public State state { get; private set; }
    public char letter { get; private set; }

    private Image fill;
    private Outline outline;
    private TextMeshProUGUI text;

    private void Awake()
    {
        //Initialize each tile and assign values
        fill = GetComponent<Image>();
        outline = GetComponent<Outline>();
        text = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void SetLetter(char letter)      //Set letter to the tile and assign to its text component
    {
        this.letter = letter; //this is the letter this tile holds
        text.text = letter.ToString();
    }

    public void SetState(State state)   //change color on the base of the state
    {
        this.state = state;
        fill.color = state.fillColor;
        outline.effectColor = state.outlineColor;
    }

}
