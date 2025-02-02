using DG.Tweening;
using RobbieWagnerGames.Utilities;
using System.Collections;
using UnityEngine;

namespace RobbieWagnerGames.TurnBasedCombat
{
    public class NotificationsUI
        : MonoBehaviourSingleton<NotificationsUI>
    {
        [SerializeField] private Canvas notificationsCanvas;
        [SerializeField] private GameNotification notificationCardPrefab;

        public IEnumerator DisplayNotification(string text, Sprite icon)
        {
            GameNotification notification = Instantiate(notificationCardPrefab, notificationsCanvas.transform);

            notification.displayText.text = text;
            
            if(icon != null)
            {
                notification.displaySprite.gameObject.SetActive(true);
                notification.displaySprite.sprite = icon;
            }
            else
                notification.displaySprite.gameObject.SetActive(false);

            Vector2 position = new Vector2(notification.rectTransform.position.x, notification.rectTransform.position.y);

            notification.rectTransform.anchoredPosition = notification.offScreenPos;

            yield return notification.rectTransform.DOAnchorPos(new Vector2(8, -8), .5f).SetEase(Ease.Linear).WaitForCompletion();
            yield return new WaitForSeconds(2f);
            yield return notification.rectTransform.DOAnchorPos(notification.offScreenPos, .5f).SetEase(Ease.Linear).WaitForCompletion();
        }
    }
}