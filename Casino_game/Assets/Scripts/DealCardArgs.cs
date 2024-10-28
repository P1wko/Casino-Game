using System;
using UnityEngine;

public class DealCardArgs : EventArgs
{
    public int PlayerId {  get; private set; }
    public Card Card { get; private set; }

    public DealCardArgs(int playerId, Card card)
    {
        PlayerId = playerId;
        Card = card;
    }
}
