using System;
using UnityEngine;
using UnityEngine.UI;

public class Box : MonoBehaviour {
    [SerializeField] private int currentStatus = 0;
    [SerializeField] private BoxStatus[] allowedStatuses;
    [SerializeField] Image image;
    [SerializeField] Sprite empty, fromBlackCross, formWhiteCircle, colorWhiteCross, colorBlackCircle;
    protected int row, col;
    public delegate void BoxClickedEventHandler(int row, int col);
    public event BoxClickedEventHandler OnBoxClicked;

    public virtual void OnClick()
    {
        OnBoxClicked?.Invoke(row, col);
    }
    
    private Sprite GetSpriteFromStatus(BoxStatus status) {
        switch (status) {
            case BoxStatus.EMPTY:
                return empty;
            case BoxStatus.FORM_BLACK_CROSS:
                return fromBlackCross;
            case BoxStatus.FORM_WHITE_CIRCLE:
                return formWhiteCircle;
            case BoxStatus.COLOR_WHITE_CROSS:
                return colorWhiteCross;
            case BoxStatus.COLOR_BLACK_CIRCLE:
                return colorBlackCircle;
            default:
                throw new ApplicationException("Invalid BoxStatus");
        }
    }    

    public void Initialize(int row, int col) {
        this.row = row;
        this.col = col;
        SetStatus(GetStatus());
    }

    public BoxStatus GetStatus() {
        return allowedStatuses[currentStatus];
    }

    public void SetStatus(BoxStatus nextStatus) {
        if (Array.IndexOf(allowedStatuses, nextStatus) == -1) {
            throw new ArgumentException("nextStatus is not in allowedStatuses");
        }
        if (nextStatus != GetStatus()) {
            Sprite boxSprite = GetSpriteFromStatus(nextStatus);
            currentStatus = Array.IndexOf(allowedStatuses, nextStatus);
            image.sprite = boxSprite;
        }
    }
    
    public BoxStatus NextStatus() {
        int nextStatusIndex = (currentStatus + 1) % allowedStatuses.Length;
        BoxStatus nextStatus = allowedStatuses[nextStatusIndex];
        SetStatus(nextStatus);
        return nextStatus;
    }
}