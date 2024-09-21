using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RobbieWagnerGames.TurnBasedCombat
{
    public enum GameMode
    {
        NONE,
        EXPLORATION,
        COMBAT,
        EVENT
    }
    public class GameManager : MonoBehaviour
    {
        public static Action OnGameModeChanged = () => { };
        private GameMode currentGameMode = GameMode.NONE;
        public GameMode CurrentGameMode
        {
            get 
            {
                return currentGameMode;
            }
            set
            {
                if (value == currentGameMode)
                    return;
                currentGameMode = value;

            }
        }

        public static GameManager Instance { get; private set; }
        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
                Instance = this;
        }
    }
}