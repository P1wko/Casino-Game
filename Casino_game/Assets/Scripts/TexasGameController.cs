using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.UI;

public class TexasGameController : MonoBehaviour
{
    public GameObject CardPrefab;
    public GameObject CardsOnTable;
    public List<TextMeshProUGUI> playersMoney;
    public List<TextMeshProUGUI> playerBets;
    public TextMeshProUGUI House;

    public float CardSpacing = 0;
    public Vector2 CardScale = Vector2.one;
    public Vector2 FirstCardPos = Vector2.one;
    public int smallBlind = 20;

    private int playersCalled = 0;
    private int houseMoney = 0;
    private TexasDeck texasDeck;
    private int CardsOnTableCount = 0;
    private List<Player> players = new List<Player>();
    private int largestBet = 0;
    private bool actionPerformed = false;

    private void Awake()
    {
        texasDeck = new TexasDeck();
        texasDeck.ShuffleDeck();
    }

    private void Start()
    {
        players.Add(new Player(1, "Krzysiek", 100));
        players.Add(new Player(2, "Grzesiek", 200));
        players.Add(new Player(3, "Marcin", 200));
        players.Add(new Player(4, "Mariusz", 200));

        StartCoroutine(GameStages());

        for(int i = 0; i < players.Count; i++)
        {
            playersMoney[i].text = players[i].money.ToString() + "$";
        }
    }

    private IEnumerator GameStages()
    {
        CheckIfPlayersHaveMoney();

        for (int i = 0; i < 2; i++)
        {
            DealCardsToPlayers();
        }

        PlaceBet(players[2], smallBlind, false);
        PlaceBet(players[3], smallBlind * 2, false);

        yield return StartCoroutine(PlacingBets());

        for (int i = 0; i < 3; i++)
        {
            DealCardOnTable();
        }

        yield return StartCoroutine(PlacingBets());

        DealCardOnTable();

        yield return StartCoroutine(PlacingBets());

        DealCardOnTable();

        yield return StartCoroutine(PlacingBets());
    }

    private IEnumerator PlacingBets()
    {
        foreach (Player player in players)
        {
            Debug.Log("wchodzi");
            if (player.isPassed) continue;
            if (player == players[0])
            {
                Debug.Log("czekam...");
                actionPerformed = false;
                while (!actionPerformed)
                {
                    yield return null;
                }
            }
            else
            {
                int decision = UnityEngine.Random.Range(0, 2);
                if (decision == 0) // Call
                {
                    int callAmount = largestBet - player.placedBet;
                    PlaceBet(player, callAmount, true);
                    Debug.Log($"AI Player {player.playerId} called with {callAmount}");
                }
                else // Pass
                {
                    player.isPassed = true;
                    Debug.Log($"AI Player {player.playerId} passed.");
                }

                yield return new WaitForSeconds(1);
            }
        }

        for (int i = 0; i < players.Count; i++)
        {
            houseMoney += players[i].placedBet;
            players[i].placedBet = 0;
            playerBets[i].text = players[i].placedBet.ToString() + "$";

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

        newSpriteObject.transform.localPosition =
            new Vector3(FirstCardPos.x + (CardSpacing * CardsOnTableCount), FirstCardPos.y, 0);
        newSpriteObject.transform.localScale = CardScale;
        CardsOnTableCount++;

        EventManager.DealCardInit(0, card);

        foreach (Player player in players)
        {
            player.AddCardToHand(card);
        }
    }

    public void DealCardsToPlayers()
    {
        foreach (Player player in players)
        {
            Card card = texasDeck.DrawRandomCard();
            player.AddCardToHand(card);

            EventManager.DealCardInit(player.playerId, card);
        }
    }

    public void PlaceBet(Player player, int bet, bool isCalling)
    {
        if (isCalling) playersCalled++;
        else playersCalled = 0;

        if (player.PlaceBet(bet))
        {
            if(player.placedBet > largestBet) largestBet = player.placedBet;
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
        PlaceBet(players[0], largestBet - players[0].placedBet, true);
        Debug.Log("Przycisk");
    }

    public void Pass()
    {
        players[0].isPassed = true;
        actionPerformed = true;
        Debug.Log("Przycisk");
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

            int handValue = EvaluateHand(player); // Implementuj t� metod�
            if (handValue > bestHandValue)
            {
                bestHandValue = handValue;
                bestPlayer = player;
            }
        }

        Debug.Log($"Winner is Player {bestPlayer.playerId} with hand value {bestHandValue}!");
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

        if (RoyalFlush(cardValues, cardsOnHand)) return 9;
        if (StraightFlush(cardValues, cardsOnHand)) return 8;
        if (FourOfAKind(cardValues)) return 7;
        if (FullHouse(cardValues)) return 6;
        if (Flush(suitValues)) return 5;
        if (Straight(cardValues)) return 4;
        if (ThreeOfAKind(cardValues)) return 3;
        if (TwoPair(cardValues)) return 2;
        if (Pair(cardValues)) return 1;
        // High card
        return 0;
    }

    private bool FourOfAKind(Dictionary<int,int> cardValues)
    {
        return cardValues.Values.Contains(4);
    }
    
    private bool ThreeOfAKind(Dictionary<int,int> cardValues)
    {
        return cardValues.Values.Contains(3);
    }
    
    private bool Pair(Dictionary<int,int> cardValues)
    {
        return cardValues.Values.Contains(2);
    }
    
    private bool TwoPair(Dictionary<int,int> cardValues)
    {
        return cardValues.Values.Count(value => value == 2) == 2;
    }
    
    private bool FullHouse(Dictionary<int,int> cardValues)
    {
        return cardValues.Values.Contains(3) && cardValues.Values.Contains(2);
    }
    
    private bool Flush(Dictionary<Suits,int> suitValues)
    {
        return suitValues.Values.Contains(5);
    }

    private bool Straight(Dictionary<int, int> cardValues)
    {
        var sortedKeys = cardValues.Keys.OrderBy(key => key).ToList();
        int l = 0;
        int r = 1;
        int counter = 1;

        for (int i = 1; i < sortedKeys.Count; i++)
        {
            if (sortedKeys[i] - sortedKeys[i - 1] == 1)
            {
                counter++;
                if (counter == 5) return true; // Strit znaleziony
            }
            else if (sortedKeys[i] != sortedKeys[i - 1]) // Jeśli różnice > 1, resetuj licznik
            {
                counter = 1;
            }
        }

        return false; // Brak strita
    }
    
    private bool StraightFlush(Dictionary<int, int> cardValues, List<Card> cardsOnHand)
    {
        var sortedKeys = cardValues.Keys.OrderBy(key => key).ToList();
        int l = 0;
        int r = 1;
        int counter = 1;

        for (int i = 1; i < sortedKeys.Count; i++)
        {
            if (sortedKeys[i] - sortedKeys[i - 1] == 1 && cardsOnHand[sortedKeys[l]].GetSuit() == cardsOnHand[sortedKeys[r]].GetSuit())
            {
                counter++;
                if (counter == 5) return true; // Strit znaleziony
            }
            else if (sortedKeys[i] != sortedKeys[i - 1]) // Jeśli różnice > 1, resetuj licznik
            {
                counter = 1;
            }
        }
        return false;
    }
    
    private bool RoyalFlush(Dictionary<int, int> cardValues, List<Card> cardsOnHand)
    {
        var sortedKeys = cardValues.Keys.OrderBy(key => key).ToList();
        int l = 10;
        int r = 11;
        int counter = 1;

        for (int i = 0; i < sortedKeys.Count; i++)
        {
            if (sortedKeys[r] - sortedKeys[l] == 1 && cardsOnHand[sortedKeys[l]].GetSuit() == cardsOnHand[sortedKeys[r]].GetSuit())
            {
                counter++;
                if (counter == 5) return true; // Strit znaleziony
            }
            else if (sortedKeys[i] != sortedKeys[i - 1]) // Jeśli różnice > 1, resetuj licznik
            {
                counter = 1;
            }
        }
        return false;
    }
    
}