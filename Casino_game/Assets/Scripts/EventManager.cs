using UnityEngine;

public class EventManager : MonoBehaviour
{
	public delegate void DealCardAction(int playerId, Card card);
	public static event DealCardAction onDealCard;

	public static void DealCardInit(int playerId, Card card)
	{
		if (onDealCard != null)
		{
			onDealCard(playerId, card);
		}
	}
}
