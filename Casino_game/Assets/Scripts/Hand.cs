using System.Collections.Generic;
using UnityEngine;

public class Hand
{
    public List<Card>  PlayerCards;
    public GameObject TexasGameController;

    public Hand() { }

    public void AddCard(Card card)
    {
        PlayerCards.Add(card);
    }
}
