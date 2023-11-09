using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Box : MonoBehaviour {
    [SerializeField] private int currentStatus = 0;
    [SerializeField] private BoxStatus[] allowedStatuses;
    [SerializeField] Sprite empty, fromBlackCross, formWhiteCircle, colorWhiteCross, colorBlackCircle;
    [SerializeField] Animator animator;
    [SerializeField] private SpriteRenderer renderer;
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
            currentStatus = Array.IndexOf(allowedStatuses, nextStatus);
            HandleTransactionAnimation(GetStatus(), nextStatus);
        }
    }
    
    private string GetIdleAnimationName(BoxStatus status) {
        switch (status) {
            case BoxStatus.EMPTY:
                return "empty_idle";
            case BoxStatus.FORM_BLACK_CROSS:
                return "form_white_circle_idle";
            case BoxStatus.FORM_WHITE_CIRCLE:
                return "form_white_circle_idle";
            case BoxStatus.COLOR_WHITE_CROSS:
                return "color_white_cross_idle";
            case BoxStatus.COLOR_BLACK_CIRCLE:
                return "color_black_circle_idle";
            default:
                throw new ApplicationException("Invalid BoxStatus");
        }
    }
    
    private void HandleTransactionAnimation(BoxStatus from, BoxStatus to) {
        animator.Play(GetIdleAnimationName(to));
        renderer.sprite = formWhiteCircle;
    }

    public BoxStatus NextStatus() {
        int nextStatusIndex = (currentStatus + 1) % allowedStatuses.Length;
        BoxStatus nextStatus = allowedStatuses[nextStatusIndex];
        SetStatus(nextStatus);
        return nextStatus;
    }

    public void SetEmpty() {
        SetStatus(BoxStatus.EMPTY);
    }
    
    public void SetFormBlackCross() {
        SetStatus(BoxStatus.FORM_BLACK_CROSS);
    }
    
    public void SetFormWhiteCircle() {
        SetStatus(BoxStatus.FORM_WHITE_CIRCLE);
    }
    
    public void SetColorWhiteCross() {
        SetStatus(BoxStatus.COLOR_WHITE_CROSS);
    }
    
    public void SetColorBlackCircle() {
        SetStatus(BoxStatus.COLOR_BLACK_CIRCLE);
    }        

}