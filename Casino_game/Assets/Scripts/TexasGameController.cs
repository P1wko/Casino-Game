using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class TexasGameController : MonoBehaviour
{
    public GameObject CardPrefab;
    public GameObject CardsOnTable;
    public List<TextMeshProUGUI> playersMoney;
    public List<TextMeshProUGUI> playersName;
    public List<TextMeshProUGUI> playersEvents;
    public List<TextMeshPro> playerBets;
    public List<HorizontalLayoutGroup> playersHands;
    public TextMeshPro House;
    public TextMeshPro WinnerText;
    public TextMeshProUGUI BetValue;
    public SpriteRenderer WinnerTextBackground;

    public float CardSpacing = 0;
    public Vector2 CardScale = Vector2.one;
    public Vector2 FirstCardPos = Vector2.one;
    public int smallBlind = 20;

    private int betValue = 0;
    private int playersCalled = 0;
    private int houseMoney = 0;
    private TexasDeck texasDeck;
    private int CardsOnTableCount = 0;
    private List<Player> players = new List<Player>();
    private int largestBet = 0;
    private bool actionPerformed = false;
    private bool yourTurn = false;
    private int cardOnTableIndex = 1;
    private int roundsCount = 0;
    private int lastWinnerId = -1;

    private void Awake()
    {
        texasDeck = new TexasDeck();
        texasDeck.ShuffleDeck();
    }

    private void Start()
    {
        players.Add(new Player(1, "Krzysiek", 500));
        players.Add(new Player(2, "Grzesiek", 500));
        players.Add(new Player(3, "Marcin", 500));
        players.Add(new Player(4, "Mieszko I", 500));

        StartCoroutine(getHumanPlayersName(1));

        for(int i = 1; i < players.Count; i++)
        {
            playersName[i].text = players[i].playerName;
        }
            
        StartCoroutine(GameStages());
    }

    private IEnumerator GameStages()
    {
        while (true)
        {
            ResetTable();
            roundsCount ++;
			for (int i = 0; i < players.Count; i++)
			{
				playersMoney[i].text = players[i].money.ToString() + "$";
			}

			yield return new WaitForSeconds(1f);

            CheckIfPlayersHaveMoney();

            yield return StartCoroutine(DealCardsToPlayers(12.0f));
            yield return StartCoroutine(DealCardsToPlayers(0.0f));

            PlaceBet(players[(roundsCount + 1)%4], smallBlind, false);
            PlaceBet(players[(roundsCount + 2)%4], smallBlind * 2, false);
            betValue = largestBet + 10;
            BetValue.text = betValue.ToString() + "$";

            yield return StartCoroutine(PlacingBets());
            largestBet = 0;
            betValue = largestBet + 10;
            BetValue.text = betValue.ToString() + "$";

            for (int i = 0; i < 3; i++)
            {
                DealCardOnTable();
                if (i < 2) yield return new WaitForSeconds(0.3f);
            }

            yield return StartCoroutine(PlacingBets());
            largestBet = 0;
            betValue = largestBet + 10;
            BetValue.text = betValue.ToString() + "$";

            DealCardOnTable();

            yield return StartCoroutine(PlacingBets());
            largestBet = 0;
            betValue = largestBet + 10;
            BetValue.text = betValue.ToString() + "$";

            DealCardOnTable();

            yield return StartCoroutine(PlacingBets());
            largestBet = 0;
            betValue = largestBet + 10;
            BetValue.text = betValue.ToString() + "$";

            DetermineWinner();

            yield return new WaitForSeconds(5f);
        }

    }

    private IEnumerator PlacingBets()
    {
        int playersInGame = 0;
        foreach (Player player in players) {
            if (!player.isPassed) playersInGame++;
        }
        while (playersCalled < playersInGame )
        {
            for(int i = roundsCount; i < players.Count + roundsCount; i++)
            {
                playersInGame = 0;
				foreach (Player playerCount in players)
				{
					if (!playerCount.isPassed) playersInGame++;
				}
                Debug.Log("playersingame: " + playersInGame + ". PlayersCalled: " + playersCalled);
				Player player = players[(i-1)%4];
                if (player.isPassed) continue;
                if (playersCalled == playersInGame) continue;
                

                yield return StartCoroutine(AnimateHand(player.playerId-1, 1));

                if (player == players[0])
                {
                    yourTurn = true;

                    Debug.Log("czekam...");
                    actionPerformed = false;
                    while (!actionPerformed)
                    {
                        yield return null;
                    }

                    yourTurn = false;
                }
                else
                {
                    yield return new WaitForSeconds(2);

                    int decision = UnityEngine.Random.Range(0, 10);
                    if (decision > 1) // Call
                    {
                        int callAmount = largestBet - player.placedBet;
                        PlaceBet(player, callAmount, true);
                        Debug.Log($"AI Player {player.playerId - 1} called with {callAmount}");
                        StartCoroutine(showEvent(player.playerId - 1, "CALL!"));
                    }
                    else // Pass
                    {
                        player.isPassed = true;
                        Debug.Log($"AI Player {player.playerId - 1} passed.");
                        playerBets[player.playerId - 1].text = "PASS!";
                        StartCoroutine(showEvent(player.playerId - 1, "PASS!"));
                    }
                }

                yield return StartCoroutine(AnimateHand(player.playerId - 1, -1));

            }
        }

        for (int i = 0; i < players.Count; i++)
        {
            houseMoney += players[i].placedBet;
            players[i].placedBet = 0;
            if (!players[i].isPassed)
            {
                playerBets[i].text = players[i].placedBet.ToString() + "$";
            }

            House.text = houseMoney.ToString() + "$";
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
        newSpriteObject.name = "CardOnTable" + cardOnTableIndex;
        cardOnTableIndex++;

        newSpriteObject.transform.localPosition =
            new Vector3(FirstCardPos.x + (CardSpacing * CardsOnTableCount), FirstCardPos.y, 0);
        newSpriteObject.transform.localScale = CardScale;
        CardsOnTableCount++;

        EventManager.DealCardInit(0, card, 0.0f);

        foreach (Player player in players)
        {
            player.AddCardToHand(card);
        }
    }

    public IEnumerator DealCardsToPlayers(float rotation)
    {
        foreach (Player player in players)
        {
            Card card = texasDeck.DrawRandomCard();
            player.AddCardToHand(card);

            EventManager.DealCardInit(player.playerId, card, rotation);

            yield return new WaitForSeconds(0.3f);
        }
    }

    public void PlaceBet(Player player, int bet, bool isCalling)
    {
        if (isCalling) playersCalled++;
        else playersCalled = 0;

        if (player.PlaceBet(bet))
        {
            if (player.placedBet > largestBet) largestBet = player.placedBet;
            playersMoney[player.playerId - 1].text = player.money.ToString() + "$";
            playerBets[player.playerId - 1].text = player.placedBet.ToString() + "$";
            actionPerformed = true;
        }
        else
        {
            Debug.Log("Not enough money");
        }
    }

    public void Call()
    {
        if (yourTurn)
        {
            PlaceBet(players[0], largestBet - players[0].placedBet, true);
            Debug.Log("Przycisk");
            StartCoroutine(showEvent(0, "CALL!"));
            yourTurn = false;
        }
    }

    public void Pass()
    {
        if(yourTurn)
        {
            players[0].isPassed = true;
            actionPerformed = true;
            Debug.Log("Przycisk");
            playerBets[0].text = "PASS!";
            StartCoroutine(showEvent(0, "PASS!"));
            yourTurn = false;
        }

    }

    public void CheckIfPlayersHaveMoney()
    {
        foreach (Player player in players)
        {
            if (player.money < (smallBlind * 2))
            {
                players.Remove(player);
            }
        }
    }

    public bool IfEveryoneCalled()
    {
        foreach (Player player in players)
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

            player.pointsOnHand = EvaluateHand(player); // Implementuj t� metod�
            if (player.pointsOnHand > bestHandValue)
            {
                bestHandValue = player.pointsOnHand;
                bestPlayer = player;
            }
        }

        string pokerHand = "HIGH CARD";
        if (bestHandValue >= 100)
        {
            pokerHand = "ONE PAIR";
        }
        if (bestHandValue >= 200)
        {
            pokerHand = "TWO PAIR";
        }
        if (bestHandValue >= 300)
        {
            pokerHand = "THREE OF A KIND";
        }
        if (bestHandValue >= 400)
        {
            pokerHand = "STRAIGHT";
        }
        if (bestHandValue >= 500)
        {
            pokerHand = "FLUSH";
        }
        if (bestHandValue >= 600)
        {
            pokerHand = "FULL HOUSE";
        }
        if (bestHandValue >= 700)
        {
            pokerHand = "FOUR OF A KIND";
        }
        if (bestHandValue >= 800)
        {
            pokerHand = "STRAIGHT FLUSH";
        }
        if (bestHandValue >= 900)
        {
            pokerHand = "ROYAL FLUSH";
        }

        WinnerTextBackground.sortingOrder = 4;
        WinnerText.text=$"{bestPlayer.playerName.ToUpper()} WINS WITH {pokerHand}!";
        StartCoroutine(AnimateHand(bestPlayer.playerId-1,1));
        RevealCards(bestPlayer.playerId - 1);

        players[bestPlayer.playerId - 1].money += houseMoney;

        lastWinnerId = bestPlayer.playerId - 1;
    }

    private int EvaluateHand(Player player)
    {
        List<Card> cardsOnHand = player.GetHand().GetCards();
        Dictionary<int, int> cardValues = new Dictionary<int, int>();
        Dictionary<Suits, int> suitValues = new Dictionary<Suits, int>();

        foreach (Card card in cardsOnHand)
        {
            if (!cardValues.Keys.Contains(card.GetValue()))
            {
                if (card.GetValue() == 1)
                {
                    cardValues.Add(1, 1);
                    cardValues.Add(14, 1);
                }
                else
                {
                    cardValues.Add(card.GetValue(), 1);
                }

            }
            else
            {
                if(card.GetValue() == 1)
                {
                    cardValues[14]++;
                }
                cardValues[card.GetValue()]++;
            }

            if (!suitValues.Keys.Contains(card.GetSuit()))
            {
                suitValues.Add(card.GetSuit(), 1);
            }
            else
            {
                suitValues[card.GetSuit()]++;
            }
        }

        cardValues = cardValues.OrderByDescending(x => x.Key).ToDictionary(x => x.Key, x => x.Value);

        int points = 0;
        if ((points = RoyalFlush(cardsOnHand)) > 0) return points;
        if ((points = StraightFlush(cardsOnHand)) > 0) return points;
        if ((points = FourOfAKind(cardValues)) > 0) return points;
        if ((points = FullHouse(cardValues)) > 0) return points;
        if ((points = Flush(suitValues)) > 0) return points;
        if ((points = Straight(cardValues)) > 0) return points;
        if ((points = ThreeOfAKind(cardValues)) > 0) return points;
        if ((points = TwoPair(cardValues)) > 0) return points;
        if ((points = Pair(cardValues)) > 0) return points;

        points = cardValues.Keys.Max();
        Debug.Log(points);
        return points; // High card or no combination
    }

    private int FourOfAKind(Dictionary<int, int> cardValues)
    {
        if (cardValues.Values.Contains(4))
        {
            var points = cardValues.FirstOrDefault(x => x.Value == 4);
            Debug.Log(points);
            return points.Key * points.Value + 700;
        }
        return 0;
    }

    private int ThreeOfAKind(Dictionary<int, int> cardValues)
    {
        if (cardValues.Values.Contains(3))
        {
            var points = cardValues.FirstOrDefault(x => x.Value == 3);
            Debug.Log(points);
            return points.Key * points.Value + 300;
        }
        return 0; }

    private int Pair(Dictionary<int, int> cardValues)
    {
        if (cardValues.Values.Contains(2))
        {
            var points = cardValues.FirstOrDefault(x => x.Value == 2);
            Debug.Log(points);

            return points.Key * points.Value + 100;
        }
        return 0; }

    private int TwoPair(Dictionary<int, int> cardValues)
    {
        if (cardValues.Values.Count(value => value == 2) == 2)
        {
            var points = cardValues.Where(x => x.Value == 2).OrderByDescending(x => x.Key).Take(2).ToList();
            if (points.Count == 2 && points.Any(p => p.Key == 1)) return 0;
            Debug.Log(points);
            return points.Sum(x => x.Key * 2) + 200;
        }
        return 0;
    }

    private int FullHouse(Dictionary<int, int> cardValues)
    {
        if (cardValues.Values.Contains(3) && cardValues.Values.Contains(2))
        {
            
            var points = cardValues.FirstOrDefault(x => x.Value == 3);
            var pts = points.Key * points.Value;

            points = cardValues.FirstOrDefault(x => x.Value == 2);
            pts += points.Key * points.Value + 600;

            return pts;
        }
        return 0;
    }

    private int Flush(Dictionary<Suits, int> suitValues)
    {
        return suitValues.Values.Contains(5) ? 500 : 0;
    }

    private int Straight(Dictionary<int, int> cardValues)
    {
        var sortedValues = cardValues.Keys.Distinct().OrderByDescending(key => key).ToList();
        var points = sortedValues[0];
        int counter = 1;

        for (int i = 1; i < sortedValues.Count; i++)
        {
            if (sortedValues[i - 1] - sortedValues[i] == 1)
            {
                points += sortedValues[i];
                counter++;
                if (counter == 5)
                {
                    points += 400;
                    return points;
                }
            }
            else
            {
                points = sortedValues[i];
                counter = 1; // Reset liczby, jeśli nie są kolejne
            }
        }

        return 0;
    }

    private int StraightFlush(List<Card> cardsOnHand)
    {
        var groupedBySuit = cardsOnHand.GroupBy(card => card.GetSuit());
        foreach (var suitGroup in groupedBySuit)
        {

            var sortedValues = suitGroup.Select(card => card.GetValue()).Distinct().ToList();

            if (sortedValues.Contains(1))
            {
                sortedValues.Add(14);
            }

            sortedValues = sortedValues.OrderByDescending(v => v).ToList();

            var points = sortedValues[0];
            int counter = 1;

            for (int i = 1; i < sortedValues.Count; i++)
            {
                if (sortedValues[i - 1] - sortedValues[i] == 1)
                {
                    points += sortedValues[i];
                    counter++;
                    if (counter == 5)
                    {
                        points += 800;
                        return points;
                    }
                }
                else
                {
                    points = sortedValues[i];
                    counter = 1; // Reset liczby, jeśli nie są kolejne
                }
            }
        }
        return 0;
    }

    private int RoyalFlush(List<Card> cardsOnHand)
    {
        var groupedBySuit = cardsOnHand.GroupBy(card => card.GetSuit());
        foreach (var suitGroup in groupedBySuit)
        {
            var sortedValues = suitGroup.Select(card => card.GetValue()).OrderBy(v => v).ToList();

            if (sortedValues.Contains(1))
            {
                sortedValues.Add(14);
            }

            sortedValues = sortedValues.OrderBy(v => v).ToList();

            if (new HashSet<int> { 10, 11, 12, 13, 14 }.IsSubsetOf(sortedValues))
            {
                return 960;
            }
        }
        return 0;
    }

    public void RaiseBet(){
        if (players[0].money > betValue)
        {
            betValue += 10;
            BetValue.text = betValue.ToString() + "$";
        }
    }

    public void LowerBet(){
        if (largestBet + 10 < betValue)
        {
            betValue -= 10;
            BetValue.text = betValue.ToString() + "$";
        }
    }

    public void PlaceBetButton()
    {
        if (yourTurn)
        {
            PlaceBet(players[0], betValue, false);
            yourTurn = false;
        }
    }

    private IEnumerator AnimateHand(int id, int way)
    {
        Vector3 newPosition = playersHands[id].transform.position;
        Vector3 offset = new Vector3(0, 0, 0);
        switch (id)
        {
            case 0:
                offset.y = 1 * way;
                break;
            case 1:
                offset.x = 1 * way;
                break;
            case 2:
                offset.y = -1 * way;
                break;
            case 3:
                offset.x = -1 * way;
                break;
            default:
                break;
        }
        for (int i = 0; i < 30; i++)
        {
            newPosition += offset;
            playersHands[id].transform.position = newPosition;
            yield return new WaitForSeconds(0.005f);
        }
    }

    private IEnumerator getHumanPlayersName(int id)
    {
        string url = "http://localhost/szwindel/getUser.php?id=";
        url += id.ToString();
        players[0].playerName = "Loading name";
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();
            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("There was an error getting the high score: " + webRequest.error);
                players[0].playerName = "not found";
            }
            else
            {
                Debug.Log(webRequest.downloadHandler.text);
                if ((!string.IsNullOrEmpty(webRequest.downloadHandler.text) && webRequest.downloadHandler.text[0] == '<') ||
                    string.IsNullOrEmpty(webRequest.downloadHandler.text))
                {
                    players[0].playerName = "not found";
                }
                else
                {
                    players[0].playerName = webRequest.downloadHandler.text;
                }
            }
        }
    }

    private void RevealCards(int playerID)
    {
        GameObject card1 = GameObject.Find("Player" + (playerID).ToString() + "Card12");
        Image card1image = card1.GetComponent<Image>();
        card1image.sprite = players[playerID].GetHand().GetCards()[0].cardImage;
        GameObject card2 = GameObject.Find("Player" + (playerID).ToString() + "Card0");
        Image card2image = card2.GetComponent<Image>();
        card2image.sprite = players[playerID].GetHand().GetCards()[1].cardImage;
    }

    private IEnumerator showEvent(int playerID, string text)
    {
        playersEvents[playerID].text = text;
        Color color = playersEvents[playerID].color;
        color.a = 1.0f;
        playersEvents[playerID].color = color;
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < 30; i++)
        {
            color.a -= 0.1f;
            playersEvents[playerID].color = color;
            yield return new WaitForSeconds(0.05f);
        }
    }

    private void ResetTable()
    {
	//public GameObject CardPrefab;
	//public GameObject CardsOnTable;
	//public List<TextMeshProUGUI> playersMoney;
	//public List<TextMeshProUGUI> playersName;
	//public List<TextMeshProUGUI> playersEvents;
	//public List<TextMeshPro> playerBets;
	//public List<HorizontalLayoutGroup> playersHands;
	//public TextMeshPro House;
	//public TextMeshPro WinnerText;
	//public TextMeshProUGUI BetValue;
	//public SpriteRenderer WinnerTextBackground;

	//public float CardSpacing = 0;
	//public Vector2 CardScale = Vector2.one;
	//public Vector2 FirstCardPos = Vector2.one;
	//public int smallBlind = 20;

	//private int betValue = 0;
	//private int playersCalled = 0;
	//private int houseMoney = 0;
	//private TexasDeck texasDeck;
	//private int CardsOnTableCount = 0;
	//private List<Player> players = new List<Player>();
	//private int largestBet = 0;
	//private bool actionPerformed = false;
	//private bool yourTurn = false;
	    CardsOnTableCount = 0;
        House.text = string.Empty;
        WinnerText.text = string.Empty;
        largestBet = 0;
        WinnerTextBackground.sortingOrder = 0;
        houseMoney = 0;
        playersCalled = 0;
        cardOnTableIndex = 1;
        yourTurn = false;
        actionPerformed = false;
        Destroy(GameObject.Find("Player0Card0"));
        Destroy(GameObject.Find("Player0Card12"));
        Destroy(GameObject.Find("Player1Card0"));
        Destroy(GameObject.Find("Player1Card12"));
        Destroy(GameObject.Find("Player2Card0"));
        Destroy(GameObject.Find("Player2Card12"));
        Destroy(GameObject.Find("Player3Card0"));
        Destroy(GameObject.Find("Player3Card12"));
        for (int i = 1;i < 6;i++)
        {
			Destroy(GameObject.Find("CardOnTable" + i));
		}
        foreach (Player player in players)
        {
            player.isPassed = false;
            player.isActionPerformed = false;
            player.hand = new();
        }

        foreach(TextMeshPro playerBet in playerBets)
        {
            playerBet.text = string.Empty;
        }
        texasDeck = new();
        texasDeck.ShuffleDeck();
        if (lastWinnerId != -1)
        {
            StartCoroutine(AnimateHand(lastWinnerId,-1));
        }
    }
}