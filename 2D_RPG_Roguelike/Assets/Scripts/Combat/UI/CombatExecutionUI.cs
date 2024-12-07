using DG.Tweening;
using RobbieWagnerGames.StrategyCombat;
using RobbieWagnerGames.TurnBasedCombat;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CombatExecutionUI : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private Image displayedActionIcon;
    [SerializeField] private TextMeshProUGUI displayedActionName;
    [SerializeField] private RectTransform textBox;
    [SerializeField] private Vector2 offScreenPosition;
    [SerializeField] private Vector2 onScreenPosition;

    private void Awake()
    {
        textBox.anchoredPosition = offScreenPosition;
    }

    public virtual IEnumerator DisplayExecutingAction(Sprite actionIcon, string text, float flyInTime = .25f, float displaySeconds = 1f, bool awaitDisplayTime = false)
    {
        textBox.anchoredPosition = offScreenPosition;
        textBox.gameObject.SetActive(true);
        displayedActionIcon.sprite = actionIcon;
        displayedActionName.text = text;
        yield return StartCoroutine(MoveTextBox(onScreenPosition, flyInTime));

        if (awaitDisplayTime)
            yield return StartCoroutine(MoveTextBox(offScreenPosition, flyInTime, displaySeconds));
        else
            StartCoroutine(MoveTextBox(offScreenPosition, flyInTime, displaySeconds));
    }

    public virtual IEnumerator DisplayExecutingAction(CombatAction action, float flyInTime = .25f, float displaySeconds = 1f, bool awaitDisplayTime = false)
    {
        textBox.anchoredPosition = offScreenPosition;
        textBox.gameObject.SetActive(true);
        displayedActionIcon.sprite = action.actionIcon;
        displayedActionName.text = action.actionName;
        yield return StartCoroutine(MoveTextBox(onScreenPosition, flyInTime));

        if (awaitDisplayTime)
            yield return StartCoroutine(MoveTextBox(offScreenPosition, flyInTime, displaySeconds));
        else
            StartCoroutine(MoveTextBox(offScreenPosition, flyInTime, displaySeconds));
    }

    public virtual IEnumerator DisplayConsumingAction(CombatItem item, float flyInTime = .25f, float displaySeconds = 1f, bool awaitDisplayTime = false)
    {
        textBox.anchoredPosition = offScreenPosition;
        textBox.gameObject.SetActive(true);
        displayedActionIcon.sprite = item.itemIcon;
        displayedActionName.text = item.itemName;
        yield return StartCoroutine(MoveTextBox(onScreenPosition, flyInTime));

        if (awaitDisplayTime)
            yield return StartCoroutine(MoveTextBox(offScreenPosition, flyInTime, displaySeconds));
        else
            StartCoroutine(MoveTextBox(offScreenPosition, flyInTime, displaySeconds));
    }

    public IEnumerator MoveTextBox(Vector2 position, float seconds, float delayStart = -1f)
    {
        if(delayStart > 0)
            yield return new WaitForSeconds(delayStart);
        yield return textBox.DOAnchorPos(position, seconds).WaitForCompletion();
    }
}
