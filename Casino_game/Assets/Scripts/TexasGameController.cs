using System;
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

	private TexasDeck TexasDeck;
	private int CardsOnTableCount = 0;
	private List<Player> players = new List<Player>();
	private void Awake()
	{
		//TexasDeck = new TexasDeck();
		//TexasDeck.ShuffleDeck();
	}

	private void Start()
	{
		players.Add(new Player ( 1, "Krzysiek", 100 ));
		players.Add(new Player ( 2, "Grzesiek", 200 ));

		DealCardOnTable();
		DealCardOnTable();
		DealCardOnTable();
		DealCardOnTable();
		DealCardOnTable();
	}

    private void Update()
    {
        
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
            Card card = new Card(1, Suits.Spades);
			player.AddCardToHand(card);
		}
	}

}
