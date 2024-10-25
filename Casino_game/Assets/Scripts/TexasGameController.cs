using System.Collections.Generic;
using UnityEngine;

public class TexasGameController : MonoBehaviour
{
	private TexasDeck TexasDeck;
	private void Awake()
	{
		TexasDeck = new TexasDeck();
		TexasDeck.ShuffleDeck();
	}

}
