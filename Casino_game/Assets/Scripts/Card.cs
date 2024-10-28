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
}
