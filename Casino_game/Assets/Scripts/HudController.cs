using UnityEngine;
using UnityEngine.UI;

public class HudController : MonoBehaviour
{
	public GameObject CardPrefab;
	public GameObject CardsOnHand;

	private void Start()
	{
		EventManager.onDealCard += CardDealed;
	}
	private void CardDealed(int playerId, Card card)
	{
		if(playerId == 1)
		{
			GameObject newImageObject = Instantiate(CardPrefab);
			Image newImage = newImageObject.GetComponent<Image>();

			newImage.sprite = card.cardImage;
			newImageObject.transform.SetParent(CardsOnHand.transform, false);
		}
	}
}
