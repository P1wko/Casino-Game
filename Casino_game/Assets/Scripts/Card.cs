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
    public int Value { get; set; }
    public Suits Suit { get; set; }

    public Card(int value, Suits suit)
    {
        Value = value;
        Suit = suit;
    }
}
