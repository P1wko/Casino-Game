using System.Collections.Generic;
using UnityEngine;

public class Hand
{
    private List<Card> PlayerCards = new();
    public GameObject TexasGameController;

    public Hand() { }

    public void AddCard(Card card)
    {
        PlayerCards.Add(card);
    }

    public List<Card> GetCards()
    {
        return PlayerCards;
    }
}
