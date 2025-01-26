using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RobbieWagnerGames;

namespace RobbieWagnerGames.StrategyCombat.Units
{
    public class Ally : Unit
    {

        public string headSpriteRelativePath;
        public string dialogueSpriteRelativePath;

        [HideInInspector] public int currentActionIndex;

        protected override void Awake()
        {
            base.Awake();
            HP = maxHP;
            //MP = maxMP;

        }

        public override IEnumerator DownUnitCo()
        {
            isUnitFighting = false;

            yield return null;
            if(spriteRenderer != null)
            {
                Color color = spriteRenderer.color;
                yield return spriteRenderer.DOColor(new Color(1, 1, 1, .2f), .1f).SetEase(Ease.Linear).WaitForCompletion();
            }

            StopCoroutine(DownUnitCo());
        }
    }
}
