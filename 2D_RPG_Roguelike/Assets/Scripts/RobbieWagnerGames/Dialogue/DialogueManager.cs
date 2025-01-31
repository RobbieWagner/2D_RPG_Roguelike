using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using Ink.Runtime;
using System.Text.RegularExpressions;
using RobbieWagnerGames.TurnBasedCombat;
using System.Linq;

namespace RobbieWagnerGames
{
    public partial class DialogueManager : MonoBehaviour
    {
        [Header("General")]
        [SerializeField] private bool canvasEnabledOnStart;

        private Story currentStory;
        public IEnumerator dialogueCoroutine {get; private set;}

        private string currentSentenceText = "";
        private bool sentenceTyping = false;
        private bool skipSentenceTyping = false;
        private bool currentSpeakerIsOnLeft = true;

        [SerializeField] private AudioSource dialogueSound;

        private const string PLAYER_NAME_PATH = "/Player/name";

        [Header("Choices")]
        [SerializeField] private Button choicePrefab;
        [SerializeField] private LayoutGroup choiceParent;
        private List<Button> choiceButtons = new List<Button>();

        private bool canContinue;
        public bool CanContinue
        {
            get { return canContinue; }
            set
            {
                if(value == canContinue) return;
                canContinue = value;

                if(canContinue) DisplayDialogueChoices();
            }
        }

        public static DialogueManager Instance {get; private set;}

        private void Awake()
        {
            if (Instance != null && Instance != this) 
            { 
                Destroy(gameObject); 
            } 
            else 
            { 
                Instance = this; 
            }

            DisableSpeakerVisuals();
            dialogueCanvas.enabled = canvasEnabledOnStart;
            CanContinue = false;
            continueIcon.enabled = false;
            InputManager.Instance.gameControls.DIALOGUE.Select.performed += OnNextDialogueLine;
        }

        public void EnterDialogueMode(Story story)
        {
            StartCoroutine(EnterDialogueModeCo(story));
        }

        public IEnumerator EnterDialogueModeCo(Story story)
        {
            if(dialogueCoroutine == null)
            {
                ConfigureDialogueControls();
                dialogueCoroutine = RunDialogue(story);
                yield return StartCoroutine(dialogueCoroutine);
            }

            StopCoroutine(EnterDialogueModeCo(story));
        }

        private void ConfigureDialogueControls()
        {
            InputManager.Instance.EnableActionMap(ActionMapName.DIALOGUE.ToString());
        }

        #region core mechanics
        private IEnumerator RunDialogue(Story story)
        {
            yield return null;

            currentStory = story;
            dialogueCanvas.enabled = true;
            currentSpeakerIsOnLeft = true;

            ToggleSpeaker(leftSpeakerNamePlate, leftSpeakerName, true);
            ToggleSpeaker(rightSpeakerNamePlate, rightSpeakerName, true);

            yield return StartCoroutine(ReadNextSentence());

            while(dialogueCoroutine != null)
            {
                yield return null;
            }

            StopCoroutine(RunDialogue(story));
        }

        public IEnumerator EndDialogue()
        {
            yield return null;
            currentText.text = "";
            dialogueCanvas.enabled = false;
            dialogueCoroutine = null;
            currentStory = null;
            StopCoroutine(EndDialogue());
        }
        #endregion

        #region choices

        private bool DialogueHasChoices()
        {
            if(currentStory == null) return false;
            return currentStory.currentChoices.Count > 0;
        }

        private void DisplayDialogueChoices()
        {
            if(DialogueHasChoices())
            {
                List<Choice> choices = currentStory.currentChoices;

                for(int i = 0; i < choices.Count; i++)
                {
                    Choice choice = choices[i];
                    Button newButton = Instantiate(choicePrefab, choiceParent.transform);
                    newButton.onClick.AddListener(() => OnChoiceButtonConfirmed(choice.index));
                    newButton.GetComponentInChildren<TextMeshProUGUI>().text = choice.text;
                    choiceButtons.Add(newButton);
                }

                for (int i = 0; i < choiceButtons.Count; i++)
                {
                    Button button = choiceButtons[i];

                    Navigation navigation = new Navigation
                    {
                        mode = Navigation.Mode.Explicit,
                        selectOnUp = i == choiceButtons.Count - 1 ? choiceButtons[0] : choiceButtons[i + 1],
                        selectOnDown = i == 0 ? choiceButtons[choiceButtons.Count - 1] : choiceButtons[i - 1]
                    };

                    button.navigation = navigation;
                }

                EventSystemManager.Instance.SetSelectedGameObject(choiceButtons.First().gameObject);
            }
            else
            {
                continueIcon.enabled = true;
            }
        }

        private void OnChoiceButtonConfirmed(int index)
        {
            currentStory.ChooseChoiceIndex(index);
            continueIcon.enabled = false;

            StartCoroutine(ReadNextSentence());
        }

        private void RemoveChoiceGameObjects()
        {
            if(choiceButtons != null)
            {
                foreach(Button button in choiceButtons)
                {
                    Destroy(button.gameObject);
                }

                choiceButtons.Clear();
            }
        }
        #endregion
    }
}