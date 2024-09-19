using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;
using RobbieWagnerGames;

namespace RobbieWagnerGames
{
    public class DialogueSceneEvent : SceneEvent
    {
        [SerializeField] private TextAsset storyTextAsset;
        [SerializeField] private bool useSimpleDialogueManager;

        public override IEnumerator RunSceneEvent()
        {
            Story story = new Story(storyTextAsset.text);
            
            if(useSimpleDialogueManager)
                yield return SimpleDialogueManager.Instance?.EnterDialogueModeCo(story);
            else
                yield return DialogueManager.Instance.EnterDialogueModeCo(story);

            yield return StartCoroutine(base.RunSceneEvent());
            StopCoroutine(RunSceneEvent());
        }
    }
}