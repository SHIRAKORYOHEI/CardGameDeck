using UnityEngine;

[CreateAssetMenu(fileName = "CardData", menuName = "Card Game Deck")]
public class CardData : ScriptableObject
{
    public int cost;
    public int hp;
    public Color color;
}