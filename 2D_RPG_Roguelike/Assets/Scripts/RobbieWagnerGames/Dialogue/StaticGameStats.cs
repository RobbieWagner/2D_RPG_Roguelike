using RobbieWagnerGames.StrategyCombat;
using RobbieWagnerGames.TurnBasedCombat;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RobbieWagnerGames
{
    public static class StaticGameStats
    {
        #region Asset File Paths
        //All file paths are local to the "Resources" folder.
        public static string combatActionFilePath = "CombatAction";
        public static string spritesFilePath = "Sprites/";
        public static string combatAnimatorFilePath = "Animation/CombatAnimation";
        public static string defaultCombatAnimatorFilePath = "Animation/CombatAnimation/Player/Player";
        public static string characterSpriteFilePath = "Sprites/Characters/";
        public static string backgroundSpriteFilePath = "Sprites/Backgrounds/";
        public static string headSpriteFilePath = "Sprites/Heads";
        public static string soundFilePath = "Sounds/";
        public static string dialogueMusicFilePath = "Sounds/Dialogue/Music/";
        public static string dialogueSoundEffectsFilePath = "Sounds/Dialogue/SoundEffects/";
        public static string dialogueSavePath = "Exploration/DialogueInteractions/";
        public static string combatMusicFilePath = "Sounds/Combat/Music/";
        public static string combatSoundEffectsFilePath = "Sounds/Combat/SoundEffects/";
        //TODO: find way to load scene in build!!
        public static string sceneFilePath = "Assets/Scenes/Combat/";
       
        public static string GetCombatActionResourcePath(CombatAction action)
        {
            return "Actions/" + (action.actionType == ActionType.NONE ? $"{action.name}" : $"{action.actionType}/{action.name}");
        }

        public static string GetItemResourcePath(GameItem item)
        {
            Type itemType = item.GetType();
            string itemFolder = "";
            switch(itemType.ToString())
            {
                case nameof(KeyItem):
                    itemFolder = "KEY/";
                    break;
                case nameof(CombatItem):
                    itemFolder = "COMBAT/" + ((CombatItem) item).combatItemType.ToString() + "/";
                    break;
                default:
                    itemFolder = "";
                    break;
            }

            return $"Items/{itemFolder}/{item.name}";
        }
        #endregion

        #region Save Data
        private static string persistentDataPath;
        public static string PersistentDataPath
        {
            get 
            {
                if (string.IsNullOrWhiteSpace(persistentDataPath))
                    persistentDataPath = Application.persistentDataPath; 
                return persistentDataPath; 
            }
            private set 
            { 
                persistentDataPath = value; 
            }
        }
        public static string partySavePath = "player_party.json";
        #endregion
    }
}