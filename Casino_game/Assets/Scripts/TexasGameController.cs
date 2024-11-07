using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TexasGameController : MonoBehaviour
{
	public GameObject CardPrefab;
	public GameObject CardsOnTable;

	public float CardSpacing = 0;
	public Vector2 CardScale = Vector2.one;
	public Vector2 FirstCardPos = Vector2.one;
	public int smallBlind = 20;

	private TexasDeck TexasDeck;
	private int CardsOnTableCount = 0;
	private List<Player> players = new List<Player>();
	private int largestBet = 0;
	private bool actionPerformed = false;
	private void Awake()
	{
		//TexasDeck = new TexasDeck();
		//TexasDeck.ShuffleDeck();
	}

	private void Start()
	{
		players.Add(new Player ( 1, "Krzysiek", 100 ));
		players.Add(new Player ( 2, "Grzesiek", 200 ));

		StartCoroutine(GameStages());

		//DealCardOnTable();
		//DealCardOnTable();
		//DealCardOnTable();
		//DealCardOnTable();
		//DealCardOnTable();
	}

	private IEnumerator GameStages()
	{
		CheckIfPlayersHaveMoney();

		for (int i = 0; i < 2; i++)
		{
            DealCardsToPlayers();
        }

		PlaceBet(players[0], smallBlind);
		PlaceBet(players[1], smallBlind * 2);

		yield return StartCoroutine(PlacingBets());

		for(int i = 0; i < 3;i++)
		{
			DealCardOnTable();
		}
	}

	private IEnumerator PlacingBets()
	{
		while (!IfEveryoneCalled())
		{
			foreach (Player player in players)
			{
				Debug.Log("wchodzi");
				actionPerformed = false;
				if(player.placedBet != largestBet && !player.isPassed)
				{
					while (!actionPerformed)
					{
						yield return null;
					}
					actionPerformed = false;
				}
			}
		}
	}

    public void DealCardOnTable()
	{
		//Card card = TexasDeck.DrawRandomCard();
		Card card = new Card(1, Suits.Diamonds);

		GameObject newSpriteObject = Instantiate(CardPrefab);
		SpriteRenderer newSprite = newSpriteObject.GetComponent<SpriteRenderer>();

		newSprite.sprite = card.cardImage;
		newSpriteObject.transform.SetParent(CardsOnTable.transform, false);

		newSpriteObject.transform.localPosition = new Vector3(FirstCardPos.x, FirstCardPos.y + (CardSpacing * CardsOnTableCount), 0);
		newSpriteObject.transform.localScale = CardScale;
		CardsOnTableCount++;

		EventManager.DealCardInit(0, card);
	}

	public void DealCardsToPlayers()
	{
		foreach(Player player in players)
		{
            Card card = new Card(1, Suits.Diamonds);
			player.AddCardToHand(card);

			EventManager.DealCardInit(player.playerId, card);
		}
	}

	public void PlaceBet(Player player, int bet)
	{
		if (player.PlaceBet(bet))
		{
			largestBet = bet;
			actionPerformed = true;
		}
		else
		{
			Debug.Log("Not enough money");
		}
	}

	public void Call(Player player)
	{
		PlaceBet(player, largestBet - player.placedBet);
	}

	public void Pass(Player player)
	{
		player.isPassed = true;
		actionPerformed = true;
	}

	public void CheckIfPlayersHaveMoney()
	{
		foreach (Player player in players)
		{
			if(player.money < (smallBlind * 2))
			{
				players.Remove(player);
			}
		}
	}

	public bool IfEveryoneCalled()
	{
		foreach( Player player in players)
		{
			if (player.placedBet != largestBet) return false;
		}
		return true;
	}

}
