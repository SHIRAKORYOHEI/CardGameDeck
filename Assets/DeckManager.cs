using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class DeckManager : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] public TextMeshProUGUI totalCostText;
    [SerializeField] private GameObject costOverText;
    [SerializeField] private GameObject confirmText;
    
    
    public CardDataList cardDataList;
    public List<(GameObject card, CardData data)> cardList = new List<(GameObject, CardData)>(); 
    private readonly List<(GameObject card, CardData data)> originalOrder = new List<(GameObject, CardData)>();
    
    public int count = 10;
    public Transform canvas;
    private int currentTotalCost = 0;

    void Start()
    {
        costOverText.SetActive(false);
        confirmText.SetActive(false);
        if (cardDataList == null || cardDataList.cards == null || cardDataList.cards.Count == 0)
        {
            Debug.LogError("CardDataListが正しく設定されていないか、中身が空です。");
            return;
        }
        DeckInstantiate();
    }

    void DeckInstantiate()
    {
        for (int i = 0; i < count; i++)
        {
            GameObject card = Instantiate(cardPrefab, canvas);
            card.transform.SetAsLastSibling();

            int randIndex = Random.Range(0, cardDataList.cards.Count);
            CardData cardData = cardDataList.cards[randIndex];

            Card cardScript = card.GetComponent<Card>();
            cardScript.SetDisplay(cardData);

            cardList.Add((card, cardData));
            originalOrder.Add((card, cardData));
        }
    }
    
    public void OnCardPlaced(Card card)
    {
        currentTotalCost += card.GetCost();
        UpdateTotalCostText();
    }

    public void OnCardRemoved(Card card)
    {
        currentTotalCost -= card.GetCost();
        UpdateTotalCostText();
    }

    private void UpdateTotalCostText()
    {
        if (totalCostText != null)
            totalCostText.text = $"TotalCost: {currentTotalCost}";
        totalCostText.color = currentTotalCost >= 10 ? Color.red : Color.white;
    }
    
    //ソート処理
    public void SortByOriginalOrder()
    {
        cardList = new List<(GameObject, CardData data)>(originalOrder);
        for (int i = 0; i < cardList.Count; i++)
            cardList[i].card.transform.SetSiblingIndex(i);
    }
    
    public void SortByCostAscending()
    {
        cardList = cardList.OrderBy(p => p.data.cost).ToList(); 
        for (int i = 0; i < cardList.Count; i++) 
            cardList[i].card.transform.SetSiblingIndex(i);
    }
    
    public void SortByHPDescending()
    {
        cardList = cardList.OrderByDescending(pair => pair.data.hp).ToList();
        for (int i = 0; i < cardList.Count; i++)
            cardList[i].card.transform.SetSiblingIndex(i);
    }

    private void LockAllCards()
    {
        foreach (var (cardObj, _) in cardList)
        {
            var card = cardObj.GetComponent<Card>();
            card.isDraggable = false;
        }
    }

    public void ConfirmCardSelection()
    {
        if (currentTotalCost < 10)
        {
            LockAllCards();
            costOverText.SetActive(false);
            confirmText.SetActive(true);
        }
        else
            costOverText.SetActive(true);
            
    }
}