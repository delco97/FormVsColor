using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Box : MonoBehaviour {
    [SerializeField] private int currentStatus = 0;
    [SerializeField] private BoxStatus[] allowedStatuses;
    [SerializeField] Animator animator;
    protected int row, col;

    private Dictionary<(BoxStatus CurrentState, BoxStatus NextState), string> transitionAnimations = new() {
        { (BoxStatus.EMPTY, BoxStatus.FORM_BLACK_CROSS), "form_black_cross_idle" },
        { (BoxStatus.EMPTY, BoxStatus.FORM_WHITE_CIRCLE), "form_white_circle_idle" },
        { (BoxStatus.EMPTY, BoxStatus.COLOR_BLACK_CIRCLE), "color_black_circle_idle" },
        { (BoxStatus.EMPTY, BoxStatus.COLOR_WHITE_CROSS), "color_white_cross_idle" },
        { (BoxStatus.FORM_BLACK_CROSS, BoxStatus.FORM_WHITE_CIRCLE), "form_flip_from_black_cross_to_white_circle" },
        { (BoxStatus.FORM_WHITE_CIRCLE, BoxStatus.FORM_BLACK_CROSS), "form_flip_from_white_circle_to_black_cross" },
        { (BoxStatus.COLOR_BLACK_CIRCLE, BoxStatus.COLOR_WHITE_CROSS), "color_flip_from_black_circle_to_white_cross" },
        { (BoxStatus.COLOR_WHITE_CROSS, BoxStatus.COLOR_BLACK_CIRCLE), "color_flip_from_white_cross_to_black_circle" },
    };

    public delegate void BoxClickedEventHandler(int row, int col);

    public event BoxClickedEventHandler OnBoxClicked;

    public virtual void OnClick() {
        OnBoxClicked?.Invoke(row, col);
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

        if (GetStatus() == nextStatus) return;
        animator.Play(GetAnimationName(GetStatus(), nextStatus));
        currentStatus = Array.IndexOf(allowedStatuses, nextStatus);
    }

    private string GetIdleAnimationName(BoxStatus status) {
        switch (status) {
            case BoxStatus.EMPTY:
                return "empty_idle";
            case BoxStatus.FORM_BLACK_CROSS:
                return "form_black_cross_idle";
            case BoxStatus.FORM_WHITE_CIRCLE:
                return "form_white_circle_idle";
            case BoxStatus.COLOR_WHITE_CROSS:
                return "color_white_cross_idle";
            case BoxStatus.COLOR_BLACK_CIRCLE:
                return "color_black_circle_idle";
            default:
                throw new ApplicationException("BoxStatus does not have an idle animation: " + status);
        }
    }

    private string GetAnimationName(BoxStatus from, BoxStatus to) {
        transitionAnimations.TryGetValue((from, to), out string animationName);
        return animationName == null ? GetIdleAnimationName(to) : animationName;
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