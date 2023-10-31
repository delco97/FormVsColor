using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayer {
    public bool IsAI();
    public GameState Play(GameState gameState);
    public bool HasMoved();
}