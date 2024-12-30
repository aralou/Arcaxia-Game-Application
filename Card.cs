using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public int id; // Unique identifier for the card
    public Sprite frontImage; // The card's front image
    public Sprite backImage; // The card's back image
    private Image cardImage;
    private bool isFlipped = false;

    private void Awake()
    {
        cardImage = GetComponent<Image>();
    }

    public void SetCard(int cardId, Sprite frontSprite)
    {
        id = cardId;
        frontImage = frontSprite;
        cardImage.sprite = backImage; // Show the back image initially
    }

    public void OnCardClicked()
    {
        if (isFlipped || L2GameManager.Instance.IsProcessing) return;

        FlipCard(true);
        L2GameManager.Instance.CardRevealed(this);
    }

    public void FlipCard(bool showFront)
    {
        isFlipped = showFront;
        cardImage.sprite = showFront ? frontImage : backImage;
    }
}