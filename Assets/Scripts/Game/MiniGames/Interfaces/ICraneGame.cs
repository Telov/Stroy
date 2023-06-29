using System;
using Game.Tasks;
using UnityEngine;

namespace Game.MiniGames.Interfaces
{
    public interface ICraneGame
    {
        void StartGame(Action<float, float> callback, ITutorialService tutorialService);
        void StartGame(Action<float, float> callback);
        void StopGame();
    }
}
