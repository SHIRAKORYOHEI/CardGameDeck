using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Card : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private DeckManager deckManager;
    public CardData cardData;
    Vector3 originalPosition;
    private Transform originalParent;
    [SerializeField] private Image backgroundImage;
    public Transform dragLayer;
    public bool isDraggable = true;

    
    void Start()
    {
        dragLayer = GameObject.FindGameObjectWithTag("DragLayer").transform;
        deckManager = FindObjectOfType<DeckManager>();
    }

    public int GetCost()
    {
        int cost = 0;
        var texts = GetComponentsInChildren<TextMeshProUGUI>();
        var costText = System.Array.Find(texts, x => x.gameObject.name == "CostText");

        if (costText != null)
        {
            var split = costText.text.Split(':');
            if (split.Length >= 2)
                int.TryParse(split[1], out cost);
            else
                Debug.LogWarning("CostText のフォーマットが違います: " + costText.text);
        }
        else
        {
            Debug.LogWarning("CostText が見つかりませんでした。");
        }
        return cost;
    }

    

    public void SetDisplay(CardData data)
    {
        cardData = data;

        var texts = GetComponentsInChildren<TextMeshProUGUI>();

        var costText = System.Array.Find(texts, x => x.gameObject.name == "CostText");
        var hpText = System.Array.Find(texts, x => x.gameObject.name == "HPText");

        if (costText != null) costText.text += data.cost.ToString();
        if (hpText != null) hpText.text += data.hp.ToString();
        
        SetColor(data.color);
    }

    private void SetColor(Color color)
    {
        if (backgroundImage != null)
        {
            backgroundImage.color = color;
        }
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        if(!isDraggable) return;
        
        originalPosition = transform.position;
        originalParent = transform.parent;
        
        transform.SetParent(dragLayer);
        transform.SetAsLastSibling();
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        if(!isDraggable) return;
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(!isDraggable) return;
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        
        bool placed = false;

        foreach (var hit in results)
        {
            //カードが手札にセットされた
            if (hit.gameObject.CompareTag("Set") && hit.gameObject.transform.childCount == 0)
            {
                transform.position = hit.gameObject.transform.position;
                transform.SetParent(hit.gameObject.transform);
                deckManager?.OnCardPlaced(this);
                placed = true;
            }
            //カードが手札から外された
            else if (hit.gameObject.CompareTag("CardStock"))
            {
                var cardStock = hit.gameObject.transform.Find("CardStock");
                if (cardStock != null)
                {
                    transform.position = cardStock.position;
                    transform.SetParent(cardStock);
                    placed = true;
                    deckManager?.OnCardRemoved(this);
                    break;
                }
            }
        }
        if (!placed)
        {
            transform.position = originalPosition;
            transform.SetParent(originalParent);
        }
    }
}
