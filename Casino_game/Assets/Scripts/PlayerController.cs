using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour 
{
	public GameObject CardPrefab;
	public GameObject CardsOnHand;

	private int PlayerId = 1;
	private Hand Hand;
	private void Start()
	{
		Hand = new();
		EventManager.onDealCard += CardDealed;
	}
    private void CardDealed(int playerId, Card card)
    {
        if (playerId == PlayerId)
        {
            GameObject newImageObject = Instantiate(CardPrefab);
            Image newImage = newImageObject.GetComponent<Image>();

            newImage.sprite = card.cardImage;
            newImageObject.transform.SetParent(CardsOnHand.transform, false);

            Hand.AddCard(card);
        }
        else if (playerId == 0)
        {
            Hand.AddCard(card);
        }
    }
}
