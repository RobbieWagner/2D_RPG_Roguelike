using RobbieWagnerGames.StrategyCombat;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Serialization;

namespace RobbieWagnerGames.TurnBasedCombat
{
    public class GameItem : ScriptableObject
    {
        [Header("General Info")]
        public string itemName;
        public Sprite itemIcon;
        public string filePath;
        [TextArea]
        public string description;
        public bool reusable = false;

        private static readonly string gameItemsResourcesPath = "Assets/Resources/";

        [ContextMenu(nameof(WriteLocalFilePath))] 
        private void WriteLocalFilePath()
        {
            // Find all assets of type GameItem in the Resources folder
            string[] guids = AssetDatabase.FindAssets("t:GameItem", new[] { gameItemsResourcesPath });

            foreach (string guid in guids)
            {
                // Get the asset's path
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);

                // Load the GameItem object
                GameItem gameItem = AssetDatabase.LoadAssetAtPath<GameItem>(assetPath);

                assetPath = Regex.Match(assetPath, @"(?<=Assets\/Resources\/)(.*?)(?=\.asset)").Groups[0].Value;

                if (gameItem != null)
                {
                    // Update the filePath property
                    gameItem.filePath = assetPath;

                    // Mark the object as dirty to save changes
                    EditorUtility.SetDirty(gameItem);

                    Debug.Log($"Updated file path for {gameItem.itemName} to {assetPath}");
                }
            }

            // Save all modified assets
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}