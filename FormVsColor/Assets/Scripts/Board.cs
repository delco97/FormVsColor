using System;
using JetBrains.Annotations;
using UnityEngine;

public class Board: MonoBehaviour {
    private GameObject[,] boxes;
    [SerializeField] private GameObject container, box;
    private int boardSize;
    private float cellHeight, cellWidth;
    public delegate void BoxClickedEventHandler(int row, int col);
    public event BoxClickedEventHandler OnBoxClicked;

    private GameObject GenerateBoardBox(int row, int col) {
        GameObject go = Instantiate(box, container.transform);
        go.name = $"Box ({row}, {col})";
        
        Rect rect = ((RectTransform)go.transform).rect;
        rect.width = cellWidth;
        rect.height = cellHeight;
        
        Box boxScript = go.GetComponent<Box>();
        boxScript.Initialize(row, col);
        boxScript.OnBoxClicked += HandleBoxClicked;
        
        row = boardSize - row - 1;
        float boardWidth = ((RectTransform)container.transform).rect.width;
        float boardHeight = ((RectTransform)container.transform).rect.height;
        go.transform.localPosition = new Vector2((col * cellWidth) - (boardWidth / 2) + cellWidth / 2,
            (row * cellHeight) - (boardHeight / 2) + cellHeight / 2);
        
        return go;
    }
    
    private BoardState GetStartingState() {
        return new BoardState(boardSize);
    }

    public void SetState(BoardState state) {
        state = state ?? GetStartingState();
        boardSize = state.GetSize();
        if(boxes == null || boardSize != boxes.GetLength(0) || boardSize != boxes.GetLength(1)) {
            boxes = new GameObject[boardSize, boardSize];
        }
        cellWidth = ((RectTransform)container.transform).rect.width / boardSize;
        cellHeight = ((RectTransform)container.transform).rect.height / boardSize;
        for (int row = 0; row < state.GetSize(); row++) {
            for (int col = 0; col < state.GetSize(); col++) {
                BoxStatus boxType = state.GetBoxStatus(row, col);
                if (boxes[row, col] == null) {
                    boxes[row, col] = GenerateBoardBox(row, col);
                }
                boxes[row, col].GetComponent<Box>().SetStatus(boxType);
            }
        }
    }    
    
    public bool IsBoardEmpty() {
        return boxes == null || boxes.Length == 0;
    }
    
    public bool AreCoordinatesValid(int row, int col) {
        return row >= 0 && row < boardSize && col >= 0 && col < boardSize;
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
        BoxStatus[,] boxStates = new BoxStatus[boardSize, boardSize];
        for(int i = 0; i < boardSize; i++) {
            for (int j = 0; j < boardSize; j++) {
                boxStates[i, j] = GetBoxStatus(i, j);
            }
        }
        return new BoardState(boxStates);
    }

}