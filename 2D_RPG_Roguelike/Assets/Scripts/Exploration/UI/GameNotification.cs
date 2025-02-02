using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RobbieWagnerGames.TurnBasedCombat
{
    public class GameNotification : MonoBehaviour
    {
        public RectTransform rectTransform;
        public TextMeshProUGUI displayText;
        public Image displaySprite;
        public Vector2 offScreenPos;
    }
}