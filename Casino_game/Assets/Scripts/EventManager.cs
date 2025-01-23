using UnityEngine;

public class EventManager : MonoBehaviour
{
	public delegate void DealCardAction(int playerId, Card card, float rotation);
	public static event DealCardAction onDealCard;

	public static void DealCardInit(int playerId, Card card, float rotation)
	{
		if (onDealCard != null)
		{
			onDealCard(playerId, card, rotation);
		}
	}
}
