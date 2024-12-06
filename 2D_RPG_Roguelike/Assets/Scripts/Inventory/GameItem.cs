using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace RobbieWagnerGames.TurnBasedCombat
{

    public class GameItem : ScriptableObject
    {
        [Header("General Info")]
        [FormerlySerializedAs("name")]
        public string itemName;
        public Sprite icon;

        
    }
}