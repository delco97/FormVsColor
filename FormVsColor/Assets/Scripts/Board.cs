using System;
using JetBrains.Annotations;
using UnityEngine;

public class Board: MonoBehaviour {
    private GameObject[,] boxes;
    [SerializeField] private GameObject container, box;
    private int nRows, nCols;
    private float cellHeight, cellWidth;
    public delegate void BoxClickedEventHandler(int row, int col);
    public event BoxClickedEventHandler OnBoxClicked;

    private GameObject GenerateBoardBox(BoxStatus status, int row, int col) {
        GameObject go = Instantiate(box, container.transform);
        go.name = $"Box ({row}, {col})"; // Set name of the box
        
        Rect rect = ((RectTransform)go.transform).rect;
        rect.width = cellWidth;
        rect.height = cellHeight;
        
        Box boxScript = go.GetComponent<Box>();
        boxScript.Initialize(row, col);
        boxScript.OnBoxClicked += HandleBoxClicked;
        
        row = nRows - row - 1;
        float boardWidth = ((RectTransform)container.transform).rect.width;
        float boardHeight = ((RectTransform)container.transform).rect.height;
        go.transform.localPosition = new Vector2((col * cellWidth) - (boardWidth / 2) + cellWidth / 2,
            (row * cellHeight) - (boardHeight / 2) + cellHeight / 2);
        
        return go;
    }
    
    private BoardState GetStartingState() {
        return new BoardState(nRows, nCols);
    }

    public void SetState(BoardState state) {
        state = state ?? GetStartingState();
        nRows = state.GetRows();
        nCols = state.GetColumns();
        cellWidth = ((RectTransform)container.transform).rect.width / nCols;
        cellHeight = ((RectTransform)container.transform).rect.height / nRows;
        boxes = new GameObject[state.GetRows(), state.GetColumns()];
        for (int i = 0; i < state.GetRows(); i++) {
            for (int j = 0; j < state.GetColumns(); j++) {
                BoxStatus boxType = state.GetBoxStatus(i, j);
                box = GenerateBoardBox(boxType, i, j);
                box.GetComponent<Box>().SetStatus(boxType);
                boxes[i, j] = box;
            }
        }
    }    
    
    public bool IsBoardEmpty() {
        return boxes == null || boxes.Length == 0;
    }
    
    public bool AreCoordinatesValid(int row, int col) {
        return row >= 0 && row < nRows && col >= 0 && col < nCols;
    }    
    
    [CanBeNull] private GameObject GetBoardBox(int row, int col) {
        if (IsBoardEmpty()) return null;
        if(!AreCoordinatesValid(row, col)) throw new ApplicationException("Invalid coordinates");
        return boxes[row, col].gameObject;
    }    
    
    public void HandleBoxClicked(int row, int col) {
        OnBoxClicked?.Invoke(row, col);
    }

    public BoxStatus GetBoxStatus(int row, int col) {
        return boxes[row, col].GetComponent<Box>().GetStatus();
    }
    
    public void SetBoxStatus(BoxStatus status, int row, int col) {
        boxes[row, col].GetComponent<Box>().SetStatus(status);
    }

    public BoardState GetCurrentState() {
        BoxStatus[,] boxStates = new BoxStatus[nRows, nCols];
        for(int i = 0; i < nRows; i++) {
            for (int j = 0; j < nCols; j++) {
                boxStates[i, j] = GetBoxStatus(i, j);
            }
        }
        return new BoardState(boxStates);
    }

}