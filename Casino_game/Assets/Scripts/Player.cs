using System;
using UnityEngine;

public class Player
{
    private Hand hand;
    public int playerId { get; private set; }
    private string playerName;
    public int money { get; private set; }
    public int placedBet { get; private set; } = 0;
    public Boolean isPassed = false;

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

    public Boolean PlaceBet(int betToPlace)
    {
        if(money - betToPlace > 0)
        {
            placedBet = betToPlace;
            return true;
        }
        else
        {
            return false;
        }
    }

}
