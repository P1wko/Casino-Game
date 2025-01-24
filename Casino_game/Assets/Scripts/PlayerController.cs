using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour 
{
	public GameObject CardPrefab;
	public GameObject CardsOnHand;
	public GameObject Comp1Cards;
	public GameObject Comp2Cards;
	public GameObject Comp3Cards;
    private void Start()
	{
		EventManager.onDealCard += CardDealed;
	}
    private void CardDealed(int playerId, Card card, float rotation)
    {
        switch (playerId)
        {
            case 1:
                {
                    GameObject newImageObject = Instantiate(CardPrefab);
                    Image newImage = newImageObject.GetComponent<Image>();

                    newImage.sprite = card.cardImage;
                    newImageObject.transform.SetParent(CardsOnHand.transform, false);
                    newImageObject.transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotation);
                    newImageObject.name = "Player0Card"+rotation.ToString();
                }
                break;
            case 2:
                {
					GameObject newImageObject = Instantiate(CardPrefab);
					Image newImage = newImageObject.GetComponent<Image>();

                    //newImage.sprite = card.cardImage;
                    newImage.sprite = Resources.Load<Sprite>("reverse");
					newImageObject.transform.SetParent(Comp1Cards.transform, false);
                    newImageObject.transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotation - 90.0f);
                    newImageObject.name = "Player1Card" + rotation.ToString();
                }
                break;
            case 3:
                {
					GameObject newImageObject = Instantiate(CardPrefab);
					Image newImage = newImageObject.GetComponent<Image>();

                    //newImage.sprite = card.cardImage;
                    newImage.sprite = Resources.Load<Sprite>("reverse");
                    newImageObject.transform.SetParent(Comp2Cards.transform, false);
                    newImageObject.transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotation - 180.0f);
                    newImageObject.name = "Player2Card" + rotation.ToString();
                }
                break;
            case 4:
                {
					GameObject newImageObject = Instantiate(CardPrefab);
					Image newImage = newImageObject.GetComponent<Image>();

                    //newImage.sprite = card.cardImage;
                    newImage.sprite = Resources.Load<Sprite>("reverse");
                    newImageObject.transform.SetParent(Comp3Cards.transform, false);
                    newImageObject.transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotation - 270.0f);
                    newImageObject.name = "Player3Card" + rotation.ToString();
                }
                break;
            default:
                break;
        }
    }
}
