using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using RobbieWagnerGames.TurnBasedCombat;
using System;

namespace RobbieWagnerGames.UI
{
    public class PauseMenu : Menu
    {
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button controlsButton;
        [SerializeField] private Button saveButton;
        [SerializeField] private Button quitButton;

        [SerializeField] private Canvas settings;
        [SerializeField] private Canvas controls;

        [HideInInspector] public bool canPause;
        [HideInInspector] public bool paused;

        private List<string> pausedActionMaps;

        public static PauseMenu Instance {get; private set;}

        protected override void Awake()
        {
            if (Instance != null && Instance != this) 
            { 
                Destroy(gameObject); 
            } 
            else 
            { 
                Instance = this; 
            } 

            canPause = true;
            paused = false;

            pausedActionMaps = new List<string>();
        } 

        protected override void OnEnable()
        {
            base.OnEnable();

            paused = true;
            Time.timeScale = 0;

            resumeButton.onClick.AddListener(ResumeGame);
            settingsButton.onClick.AddListener(OpenSettings);
            //controlsButton.onClick.AddListener(OpenControls);
            saveButton.onClick.AddListener(SaveGame);
            quitButton.onClick.AddListener(QuitToMainMenu);

            thisCanvas.enabled = true;

            foreach (InputActionMap actionMap in InputManager.Instance.gameControls.asset.actionMaps)
            {
                if (actionMap.enabled && !actionMap.name.Equals(ActionMapName.UI.ToString(), StringComparison.CurrentCultureIgnoreCase))
                {
                    pausedActionMaps.Add(InputManager.Instance.actionMaps[actionMap.name].name);
                    InputManager.Instance.DisableActionMap(actionMap.name);
                }
            }

            OnGamePaused?.Invoke();
        }

        public delegate void OnGamePausedDelegate();
        public event OnGamePausedDelegate OnGamePaused;

        protected override void OnDisable()
        {
            base.OnDisable();

            if(!paused) 
            {
                Time.timeScale = 1;

                OnGameUnpaused?.Invoke();
            }

            resumeButton.onClick.RemoveListener(ResumeGame);
            settingsButton.onClick.RemoveListener(OpenSettings);
            //controlsButton.onClick.RemoveListener(OpenControls);
            saveButton.onClick.RemoveListener(SaveGame);
            quitButton.onClick.RemoveListener(QuitToMainMenu);

            foreach (string map in pausedActionMaps)
                InputManager.Instance.EnableActionMap(map);
            pausedActionMaps.Clear();

            thisCanvas.enabled = false;
        }

        public delegate void OnGameUnpausedDelegate();
        public event OnGameUnpausedDelegate OnGameUnpaused;

        public void ResumeGame()
        {
            paused = false;
            enabled = false;
        }

        private void OpenSettings()
        {
            StartCoroutine(SwapCanvases(thisCanvas, settings));
        }

        private void OpenControls()
        {
            StartCoroutine(SwapCanvases(thisCanvas, controls));
        }

        protected virtual void SaveGame()
        {
            GameManager.Instance.SaveGame();
        }

        protected virtual void OnSaveButtonComplete()
        {

        }

        private void QuitToMainMenu()
        {
            ToggleButtonInteractibility(false);

            StartCoroutine(QuitToMainMenuCo());
        }

        protected override void ToggleButtonInteractibility(bool toggleOn)
        {
            base.ToggleButtonInteractibility(toggleOn);

            resumeButton.interactable = toggleOn;
            settingsButton.interactable = toggleOn;
            //controlsButton.interactable = toggleOn;
            saveButton.interactable = toggleOn;
            quitButton.interactable = toggleOn;
        }

        private IEnumerator QuitToMainMenuCo()
        {
            yield return new WaitForSecondsRealtime(.1f);
            Time.timeScale = 1;
            SceneManager.LoadScene("MainMenu");

            StopCoroutine(QuitToMainMenuCo());
        }

        protected override IEnumerator SwapCanvases(Canvas active, Canvas next)
        {
            yield return StartCoroutine(base.SwapCanvases(active, next));

            StopCoroutine(SwapCanvases(active, next));
        }
    }
}