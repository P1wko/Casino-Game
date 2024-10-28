using System;
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
	private void Awake()
	{
		//TexasDeck = new TexasDeck();
		//TexasDeck.ShuffleDeck();
	}

	private void Start()
	{
		EventManager.DealCardInit(1, new Card(1, Suits.Diamonds));
		EventManager.DealCardInit(1, new Card(1, Suits.Hearts));

		DealCardOnTable();
		DealCardOnTable();
		DealCardOnTable();
		DealCardOnTable();
		DealCardOnTable();
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
	}
}
