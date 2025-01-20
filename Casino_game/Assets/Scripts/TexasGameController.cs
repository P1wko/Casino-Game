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

	private int playersCalled = 0;
	private TexasDeck texasDeck;
	private int CardsOnTableCount = 0;
	private List<Player> players = new List<Player>();
	private int largestBet = 0;
	private bool actionPerformed = false;
	private void Awake()
	{
		texasDeck = new TexasDeck();
		texasDeck.ShuffleDeck();
	}

	private void Start()
	{
		players.Add(new Player ( 1, "Krzysiek", 100 ));
		players.Add(new Player ( 2, "Grzesiek", 200 ));
		players.Add(new Player ( 3, "Marcin", 200 ));
		players.Add(new Player ( 4, "Mariusz", 200 ));

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

		//PlaceBet(players[0], smallBlind);
		//PlaceBet(players[1], smallBlind * 2);

		yield return StartCoroutine(PlacingBets());

		for(int i = 0; i < 3;i++)
		{
			DealCardOnTable();
		}

		yield return StartCoroutine(PlacingBets());

		DealCardOnTable();

		yield return StartCoroutine(PlacingBets());

		DealCardOnTable();

		yield return StartCoroutine(PlacingBets());
	}

	private IEnumerator PlacingBets()
	{
		foreach (Player player in players)
		{
			Debug.Log("wchodzi");
			if (player.isPassed) continue;
			if (player == players[0])
			{
				Debug.Log("czekam...");
				actionPerformed = false;
				while (!actionPerformed)
				{
					yield return null;
				}

			}
			else
			{
				int decision = UnityEngine.Random.Range(0, 2); // AI mo�e spasowa� lub wyr�wna� zak�ad
				if (decision == 0) // Call
				{
					int callAmount = largestBet - player.placedBet;
					PlaceBet(player, callAmount, true);
					Debug.Log($"AI Player {player.playerId} called with {callAmount}");
				}
				else // Pass
				{
					player.isPassed = true;
					Debug.Log($"AI Player {player.playerId} passed.");
				}

				yield return new WaitForSeconds(1); // Ma�e op�nienie dla AI
			}
		}
	}

    public void DealCardOnTable()
	{
		playersCalled = 0;
		Card card = texasDeck.DrawRandomCard();

		GameObject newSpriteObject = Instantiate(CardPrefab);
		SpriteRenderer newSprite = newSpriteObject.GetComponent<SpriteRenderer>();

		newSprite.sprite = card.cardImage;
		newSpriteObject.transform.SetParent(CardsOnTable.transform, false);
		
		newSpriteObject.transform.localPosition = new Vector3(FirstCardPos.x, FirstCardPos.y + (CardSpacing * CardsOnTableCount), 0);
		newSpriteObject.transform.localScale = CardScale;
		CardsOnTableCount++;

		EventManager.DealCardInit(0, card);

		foreach (Player player in players)
		{
			player.AddCardToHand(card);
		}
	}

	public void DealCardsToPlayers()
	{
		foreach(Player player in players)
		{
			Card card = texasDeck.DrawRandomCard();
			player.AddCardToHand(card);

			EventManager.DealCardInit(player.playerId, card);
		}
	}

	public void PlaceBet(Player player, int bet, bool isCalling)
	{
		if (isCalling) playersCalled++;
		else playersCalled = 0;

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

	public void Call()
	{
		PlaceBet(players[0], largestBet - players[0].placedBet, true);
		Debug.Log("Przycisk");
	}

	public void Pass()
	{
		players[0].isPassed = true;
		actionPerformed = true;
		Debug.Log("Przycisk");
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

	private void DetermineWinner()
	{
		Player bestPlayer = null;
		int bestHandValue = 0;

		foreach (Player player in players)
		{
			if (player.isPassed) continue;

			/*int handValue = EvaluateHand(player); // Implementuj t� metod�
			if (handValue > bestHandValue)
			{
				bestHandValue = handValue;
				bestPlayer = player;
			}*/
		}

		Debug.Log($"Winner is Player {bestPlayer.playerId} with hand value {bestHandValue}!");
	}

	/*private EvaluateHand(Player player)
	{ 
		List<Card> cardsOnHand = player.GetHand().GetCards();

		foreach (Card cards in cardsOnHand)
		{
			
		}
		
	} */

}
