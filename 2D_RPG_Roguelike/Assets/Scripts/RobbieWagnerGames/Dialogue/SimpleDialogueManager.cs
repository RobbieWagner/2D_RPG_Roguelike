using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using Ink.Runtime;

namespace RobbieWagnerGames
{
    public class SimpleDialogueManager : MonoBehaviour
    {

        [Header("General Dialogue Info")]
        [SerializeField] private Canvas dialogueCanvas;
        [SerializeField] private TextMeshProUGUI currentText;

        private Story currentStory;
        public IEnumerator dialogueCoroutine {get; private set;}
        private DialogueControls dialogueControls;

        private string currentSentenceText = "";
        //private bool sentenceTyping = false;
        private bool skipSentenceTyping = false;

        [SerializeField] HorizontalLayoutGroup textbox;

        [Header("Left Speaker")]
        [SerializeField] private LayoutElement leftSpeakerNamePlate;
        [SerializeField] private TextMeshProUGUI leftSpeakerName;

        [Header("Right Speaker")]
        [SerializeField] private LayoutElement rightSpeakerNamePlate;
        [SerializeField] private TextMeshProUGUI rightSpeakerName;

        [Header("Choices")]
        [SerializeField] private DialogueChoice choicePrefab;
        [SerializeField] private LayoutGroup choiceParent;
        private List<DialogueChoice> choices;
        private int currentChoice;
        public int CurrentChoice
        {
            get { return currentChoice; }
            set 
            {    
                if(choices.Count == 0) return;
                if(currentChoice >= 0 && currentChoice < choices.Count)choices[currentChoice].SetInactive();
                currentChoice = value;
                if(currentChoice < 0) currentChoice = choices.Count - 1;
                else if(currentChoice >= choices.Count) currentChoice = 0;
                choices[currentChoice].SetActive();
            }
        }

        [SerializeField] private Image continueIcon;
        private bool canContinue;
        public bool CanContinue
        {
            get { return canContinue; }
            set
            {
                if(value == canContinue) 
                    return;
                canContinue = value;

                if(canContinue) 
                    OnContinueDialogue?.Invoke();
                    //DisplayDialogueChoices();
            }
        }
        public delegate void ContinueDelegate();
        public event ContinueDelegate OnContinueDialogue;

        public static SimpleDialogueManager Instance {get; private set;}

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

            dialogueCanvas.enabled = false;
            CanContinue = false;
            dialogueControls = new DialogueControls();
            dialogueControls.Disable();
            dialogueControls.Dialogue.Select.performed += ReadNextSentence;
        }

        public void EnterDialogueMode(Story story)
        {
            StartCoroutine(EnterDialogueModeCo(story));
        }

        public IEnumerator EnterDialogueModeCo(TextAsset textAsset)
        {
            Story story = new Story(textAsset.text);
            yield return StartCoroutine(EnterDialogueModeCo(story));
        }

        public IEnumerator EnterDialogueModeCo(Story story)
        {
            if(dialogueCoroutine == null)
            {
                dialogueCoroutine = RunDialogue(story);
                dialogueControls.Enable();
                yield return StartCoroutine(dialogueCoroutine);
            }
        }

        #region core mechanics

        public IEnumerator RunDialogue(Story story)
        {
            if (story == null)
                throw new NullReferenceException();

            yield return null;

            currentStory = story;
            dialogueCanvas.enabled = true;

            ToggleLeftSpeaker(false);
            ToggleRightSpeaker(false);

            yield return StartCoroutine(ReadNextSentence());

            while(dialogueCoroutine != null)
            {
                yield return null;
            }

            StopCoroutine(RunDialogue(story));
        }

        public void ReadNextSentence(InputAction.CallbackContext context)
        {
            StartCoroutine(ReadNextSentence());
        }

        public IEnumerator ReadNextSentence()
        {
            CanContinue = false;
            RemoveChoiceGameObjects();
            choices = new List<DialogueChoice>();

            yield return null;
            if(currentStory.canContinue)
            {
                currentText.text = "";
                //sentenceTyping = true;

                currentSentenceText = ConfigureSentence(currentStory.Continue());

                ConfigureTextField();
                char nextChar;
                for(int i = 0; i < currentSentenceText.Length; i++)
                {
                    if(skipSentenceTyping) break;
                    nextChar = currentSentenceText[i];
                    if(nextChar == '<')
                    {
                        while(nextChar != '>' && i < currentSentenceText.Length)
                        {
                            currentText.text += nextChar;
                            i++;
                            nextChar = currentSentenceText[i];
                        } 
                    }

                    currentText.text += nextChar;
                    yield return new WaitForSeconds(.036f);
                }

                //Debug.Log(text);
                currentText.text = currentSentenceText;
                //sentenceTyping = false;
                skipSentenceTyping = false;
                CanContinue = true;
            }
            else
            {
                yield return StartCoroutine(EndDialogue());
            }
        }

        public IEnumerator EndDialogue()
        {
            yield return null;
            //Debug.Log("Dialogue ended");
            currentText.text = "";
            dialogueCanvas.enabled = false;
            dialogueCoroutine = null;
            currentStory = null;
            dialogueControls.Disable();
            OnEndDialogue?.Invoke();
            StopCoroutine(EndDialogue());
        }
        public delegate void EndDialogueDelegate();
        public event EndDialogueDelegate OnEndDialogue;
        #endregion

        #region Text and Text Field Configuration

        private string ConfigureSentence(string input)
        {
            string configuredText = input;

            List<char> allowList = new List<char>() {' ', '-', '\'', ',', '.'};
            string name = SaveDataManager.LoadString("name", "Lux");

            bool nameAllowed = true;
            foreach(char c in name)
            {
                if(!Char.IsLetterOrDigit(c) && !allowList.Contains(c))
                {
                    nameAllowed = false;
                    break;
                }
            }

            if(!nameAllowed)
            {
                name = "Lux";
                SaveDataManager.SaveString("name", "Lux");
            } 

            configuredText = configuredText.Replace("^NAME^", name);

            return configuredText;
        }

        private void ConfigureTextField()
        {
            List<string> tags = currentStory.currentTags;

            string speaker = "";
            bool placeSpeakerOnLeft = true;
            bool removeSpeakerOnLeft = false;
            bool removeSpeakerOnRight = true;

            foreach(string tag in tags)
            {
                //Debug.Log("tag");
                if(tag.ToUpper().Contains("SPEAKER"))
                {
                    speaker = tag.Substring(9).ToLower();
                    //Debug.Log(speaker);
                }
                else if(tag.ToUpper().Contains("PLACESPEAKERONRIGHT"))
                {
                    placeSpeakerOnLeft = false;
                }
                else if(tag.ToUpper().Contains("REMOVESPEAKERS"))
                {
                    removeSpeakerOnLeft = true;
                    removeSpeakerOnRight = true;
                }
                else if(tag.ToUpper().Contains("REMOVESPEAKERONLEFT"))
                {
                    removeSpeakerOnLeft = true;
                }
                else if(tag.ToUpper().Contains("REMOVESPEAKERONRIGHT"))
                {
                    removeSpeakerOnRight = true;
                }
            }

            if(!string.IsNullOrEmpty(speaker))
            {
                SetSpeaker(speaker, placeSpeakerOnLeft);
            }

            if (removeSpeakerOnLeft)
            {
                ToggleLeftSpeaker(false);
            }
            if (removeSpeakerOnRight)
            {
                ToggleRightSpeaker(false);
            }
            //use values to determine sentence outcome
        }

        private void SetSpeaker(string speaker, bool placeSpeakerOnLeft)
        {
            if(placeSpeakerOnLeft)
            {
                ToggleLeftSpeaker(true);
                speaker = char.ToUpper(speaker[0]) + speaker.Substring(1);
                leftSpeakerName.text = speaker;
                textbox.childAlignment = TextAnchor.MiddleLeft;
            }
            else
            {
                ToggleRightSpeaker(true);
                speaker = char.ToUpper(speaker[0]) + speaker.Substring(1);
                rightSpeakerName.text = speaker;
                textbox.childAlignment = TextAnchor.MiddleRight;
            }
        }

        private void ToggleLeftSpeaker(bool on)
        {
            leftSpeakerNamePlate?.gameObject.SetActive(on);
            leftSpeakerName?.gameObject.SetActive(on);
        }

        private void ToggleRightSpeaker(bool on)
        {
            rightSpeakerNamePlate?.gameObject.SetActive(on);
            rightSpeakerName?.gameObject.SetActive(on);
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
                List<Choice> choiceOptions = currentStory.currentChoices;

                for(int i = 0; i < choiceOptions.Count; i++)
                {
                    Choice choice = choiceOptions[i];
                    DialogueChoice choiceObject = Instantiate(choicePrefab, choiceParent.transform).GetComponent<DialogueChoice>();
                    choiceObject.choice = choice;
                    choiceObject.Initialize(choice);
                    choices.Add(choiceObject);
                }

                CurrentChoice = 0;
            }
            //else
            //{
            //    continueIcon.enabled = true;
            //}
        }

        private void OnNavigateDialogueMenu(InputValue inputValue)
        {
            if(DialogueHasChoices())
            {
                float value = inputValue.Get<float>();
                if(value > 0f) 
                {
                    CurrentChoice++;
                }
                else if(value < 0f)
                {
                    CurrentChoice--;
                }
            }
        }

        private void RemoveChoiceGameObjects()
        {
            if(choices != null)
            {
                foreach(DialogueChoice choice in choices)
                {
                    Destroy(choice.gameObject);
                }

                choices.Clear();
            }

            choices = null;
        }
        #endregion
    }
}