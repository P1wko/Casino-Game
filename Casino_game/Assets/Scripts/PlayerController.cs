using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
	public GameObject CardPrefab;
	public GameObject CardsOnHand;
	private void Start()
	{
		EventManager.onDealCard += DrawCard;
	}

	private void DrawCard(int playerId, Card card)
	{
		GameObject newImageObject = Instantiate(CardPrefab);
		Image newImage = newImageObject.GetComponent<Image>();

		newImage.sprite = card.cardImage;
		newImageObject.transform.SetParent(CardsOnHand.transform, false);
	}
}
