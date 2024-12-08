using DG.Tweening;
using RobbieWagnerGames.StrategyCombat;
using RobbieWagnerGames.TurnBasedCombat;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CombatExecutionUI : MonoBehaviour
{
    [SerializeField] protected Image displayedActionIcon;
    [SerializeField] protected TextMeshProUGUI displayedActionName;
    [SerializeField] protected RectTransform textBox;
    [SerializeField] protected Vector2 offScreenPosition;
    [SerializeField] protected Vector2 onScreenPosition;

    [SerializeField] protected TextMeshProUGUI effectTextPrefab;
    [SerializeField] protected Vector3 effectTextOffset;

    [SerializeField] protected Canvas worldSpaceCanvas;
    [SerializeField] protected Canvas overlayCanvas;

    public static CombatExecutionUI Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;

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

    public void DisplayEffectText(Unit effectedUnit, StatType stat, int delta)
    {
        StartCoroutine(DisplayEffectTextCo(effectedUnit, stat, delta));
    }

    public IEnumerator DisplayEffectTextCo(Unit effectedUnit, StatType stat, int delta)
    {
        TextMeshProUGUI text = Instantiate(effectTextPrefab, worldSpaceCanvas.transform);

        if (stat == StatType.NONE)
        {
            text.color = GameUIConfiguration.Instance.GetHPChangeColor(delta);
            text.text =  delta > 0 ? $"+{delta}" : delta.ToString();

            text.transform.position = effectedUnit.transform.position + effectTextOffset;
            yield return text.transform.DOJump(text.transform.position + Vector3.right * .25f, .25f, 1, .5f).WaitForCompletion();
            yield return new WaitForSeconds(1.5f);
        }
        else
        {
            text.color = Color.clear;
            text.text = delta.ToString();

            text.transform.position = effectedUnit.transform.position + effectTextOffset;
            text.DOColor(GameUIConfiguration.Instance.GetStatColor(stat), 1);
            yield return text.transform.DOMove(text.transform.position + Vector3.up * .2f, 1).WaitForCompletion();
            text.DOColor(Color.clear, 1);
            yield return text.transform.DOMove(text.transform.position + Vector3.up * .2f, 1).WaitForCompletion();
        }    

        Destroy(text.gameObject);
    }

    public void DisplayHPChange(int hpDifference, Unit unit)
    {
        StartCoroutine(DisplayEffectTextCo(unit, StatType.NONE, hpDifference));
    }
}
