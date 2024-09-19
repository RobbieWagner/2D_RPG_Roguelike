using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RobbieWagnerGames.Minijam164
{
    public class ScreenCover : MonoBehaviour
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private Image screenCover;

        public static ScreenCover Instance { get; private set; }
        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
                Instance = this;
        }

        public IEnumerator FadeCoverIn(float time = 1f)
        {
            canvas.enabled = false;
            screenCover.color = Color.clear;
            canvas.enabled = true;
            yield return screenCover.DOColor(Color.black, time).SetEase(Ease.Linear).WaitForCompletion();
        }

        public IEnumerator FadeCoverOut(float time = 1f)
        {
            screenCover.color = Color.black;
            canvas.enabled = true;
            yield return screenCover.DOColor(Color.clear, time).SetEase(Ease.Linear).WaitForCompletion();
            canvas.enabled = false;
        }
    }
}