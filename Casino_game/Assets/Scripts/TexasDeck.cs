using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public class TexasDeck
{
    public List<Card> deck = new();

	public TexasDeck()
	{
		foreach (Suits suit in Enum.GetValues(typeof(Suits)))
		{
			for (int i = 1; i < 14; i++)
			{
				deck.Add(new Card(i, suit));
			}
		}
	}

	public void ShuffleDeck()
	{
		Random random = new Random();
		for (int i = deck.Count - 1; i > 0; --i )
		{
			int k = random.Next(i + 1);
			(deck[k], deck[i]) = (deck[i], deck[k]);
		}
	}

	public Card DrawRandomCard()
	{
		Random rand = new Random();
		Card card = deck[rand.Next(deck.Count)];
		deck.Remove(card);
		return card;
	}
}
