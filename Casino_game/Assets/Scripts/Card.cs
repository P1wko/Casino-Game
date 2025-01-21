using UnityEngine;
[System.Serializable]

public enum Suits
{
    Hearts,
    Diamonds,
    Clubs,
    Spades
}
public class Card
{
    private int Value { get; set; }
    private Suits Suit { get; set; }
    public Sprite cardImage {  get; set; }

    public Card(int value, Suits suit)
    {
        Value = value;
        Suit = suit;
        cardImage = LoadImage();
    }

    private Sprite LoadImage()
    {
        string imageName = $"{Value}_{Suit}";
        return Resources.Load<Sprite>(imageName);
    }

    public int GetValue()
    {
        return Value;
    }

    public Suits GetSuit()
    {
        return Suit;
    }
}
