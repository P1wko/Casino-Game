using System;
using UnityEngine;

public class Player
{
    private Hand hand;
    private int playerId;
    private string playerName;
    private int money;
    private Boolean isPassed = false;

    public Player(int playerId, string playerName, int money)
    {
        this.hand = new Hand();
        this.playerId = playerId;
        this.playerName = playerName;
        this.money = money;
    }

    public void AddCardToHand(Card card)
    {
        hand.AddCard(card);
    }

}
