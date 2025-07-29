using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardDataList", menuName = "Card Game Deck List")]
public class CardDataList : ScriptableObject
{
    public List<CardData> cards;
}