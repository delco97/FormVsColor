using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class BoardState {
    private BoxStatus[,] boxes;

    public BoardState(BoxStatus[,] boxes) {
        this.boxes = boxes;
    }

    public BoardState(int rows, int cols) {
        this.boxes = new BoxStatus[rows, cols];
        for (int i = 0; i < rows; i++) {
            for (int j = 0; j < cols; j++) {
                boxes[i, j] = BoxStatus.EMPTY;
            }
        }
    }

    public BoardState(BoardState other) {
        this.boxes = new BoxStatus[other.GetRows(), other.GetColumns()];
        for (int i = 0; i < other.GetRows(); i++) {
            for (int j = 0; j < other.GetColumns(); j++) {
                boxes[i, j] = other.GetBoxStatus(i, j);
            }
        }
    }

    public BoxStatus GetBoxStatus(int row, int col) {
        return boxes[row, col];
    }
    
    public void SetBoxStatus(int row, int col, BoxStatus status) {
        boxes[row, col] = status;
    }


    public int GetRows() {
        return boxes.GetLength(0);
    }

    public int GetColumns() {
        return boxes.GetLength(1);
    }
}