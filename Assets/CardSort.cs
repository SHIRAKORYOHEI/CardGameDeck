using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardSort : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown dropdown;
    [SerializeField] private DeckManager deckManager;
    
    private void Start()
    {
        dropdown.onValueChanged.AddListener(OnDropdownChanged);
    }

    private void OnDropdownChanged(int index)
    {
        switch (index)
        {
            case 0:
                deckManager.SortByOriginalOrder();
                break;
            case 1:
                deckManager.SortByCostAscending();
                break;
            case 2:
                deckManager.SortByHPDescending();
                break;
        }
    }

    void For()
    {
        for (int i = 0; i < deckManager.cardList.Count; i++)
        {
            deckManager.cardList[i].card.transform.SetSiblingIndex(i);
        }
    }
}
