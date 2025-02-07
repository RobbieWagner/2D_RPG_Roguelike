using AYellowpaper.SerializedCollections;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace RobbieWagnerGames.StrategyCombat
{
    [Serializable]
    public enum UnitClass
    {
        Classless = 0, 
    }

    public class Unit : MonoBehaviour
    {
        [SerializeField] public string unitName;
        [SerializeField] protected UnitClass unitClass;
        [SerializeField] public UnitAnimator unitAnimator;

        #region Unit Stats
        [SerializeField][SerializedDictionary("Stat", "Info")] SerializedDictionary<StatType, UnitStat> stats = new SerializedDictionary<StatType, UnitStat>();
        public SerializedDictionary<StatType, UnitStat> Stats => stats;
        #endregion

        [HideInInspector] public bool isUnitFighting = true;

        [SerializeField] public List<CombatAction> unitActions;
        public bool hasActedThisTurn = false;
        [HideInInspector] public CombatAction currentAction;
        [HideInInspector] public List<Unit> selectedTargets;

        [SerializeField] public SpriteRenderer spriteRenderer;
        [HideInInspector] public int unitCombatPos = 0; // units position in combat. Determines order of options in UI for target selection

        public int maxHP;
        protected int hp;
        public int HP
        {
            get {return hp;}
            set 
            {
                if(value == hp) 
                {
                    OnHPLowered?.Invoke(0, this);
                    return;
                }

                int difference = Math.Abs(value - hp);
                if (value < hp) 
                {
                    OnHPLowered?.Invoke(difference * -1, this);
                }
                else
                {
                    OnHPRaised?.Invoke(difference, this);
                }

                hp = value;
                if(hp > maxHP) hp = maxHP;
                if(hp < 0) hp = 0;

                OnHPChanged?.Invoke(hp);
                if(hp == 0)
                {
                    //isUnitFighting = false;
                    OnUnitsHPReaches0?.Invoke(this);
                }

            }
        }

        public delegate void OnHPChangedDelegate(int hp);
        public event OnHPChangedDelegate OnHPChanged;

        public delegate void OnUnitsHPReaches0Delegate(Unit unit);
        public event OnUnitsHPReaches0Delegate OnUnitsHPReaches0;

        public delegate void OnHPRaisedDelegate(int hpDifference, Unit unit);
        public event OnHPRaisedDelegate OnHPRaised;

        public delegate void OnHPLoweredDelegate(int hpDifference, Unit unit);
        public event OnHPLoweredDelegate OnHPLowered;

        public delegate void OnStatChangedDelegate(Unit unit, StatType stat, int delta);
        public event OnStatChangedDelegate OnStatChanged;

        protected virtual void Awake()
        {
            if(spriteRenderer == null)
            {
                spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            }

            OnUnitsHPReaches0 += DownUnit;

            SetupUnit();
        }

        protected virtual void DownUnit(Unit unit)
        {
            StartCoroutine(DownUnitCo());
        }

        public virtual void SetupUnit()
        {
            InitializeStats();

            //InitializeMaxHP();
        }

        public virtual void InitializeStats()
        {
            foreach (StatType statType in Enum.GetValues(typeof(StatType)))
            {
                if (!stats.ContainsKey(statType))
                    stats.Add(statType, new UnitStat(statType));
                stats[statType].OnStatSet += (UnitStat stat) => { Debug.Log($"{stat.UnitStatType}: {stat.CurrentValue}/{stat.BaseValue}"); };
            }
        }

        public virtual void InitializeStats(List<UnitStat> newStats)
        {
            foreach(UnitStat stat in newStats)
            {
                if (stats.ContainsKey(stat.UnitStatType))
                    stats[stat.UnitStatType] = stat;
                else
                    stats.Add(stat.UnitStatType, stat);
            }
        }

        public void DealDamage(int amount)
        {
            if(amount > 0) HP -= amount;
        }

        public void Heal(int amount)
        {
            HP += Math.Clamp(amount, 0, int.MaxValue);
        }

        public void ChangeStatValue(StatType stat, int amount)
        {
            if (amount != stats[stat].CurrentValue)
            {
                int delta = amount - stats[stat].CurrentValue;
                stats[stat].CurrentValue = amount;

                OnStatChanged?.Invoke(this, stat, delta);
            }
        }

        public virtual IEnumerator DownUnitCo()
        {
            isUnitFighting = false;

            yield return null;
            if(spriteRenderer != null)
            {
                yield return spriteRenderer.DOColor(Color.clear, .3f).SetEase(Ease.Linear).WaitForCompletion();
                spriteRenderer.enabled = false;
            }

            StopCoroutine(DownUnitCo());
        }

        #region Action Selection
        public void SelectAction(int index)
        {
            currentAction = unitActions[index];
            OnActionSelected(this, currentAction);
        }

        public delegate void OnActionSelectedDelegate(Unit user, CombatAction action);
        public event OnActionSelectedDelegate OnActionSelected;

        public void UnselectAction()
        {
            currentAction = null;
        }

        public void SelectTargets(List<Unit> targets)
        {
            selectedTargets = targets;
            OnTargetSelected();
        }
        public delegate void OnTargetSelectedDelegate();
        public event OnTargetSelectedDelegate OnTargetSelected;
        #endregion

        #region action execution
        public void DodgeAttack()
        {
            OnAttackDodged("miss", this, Color.gray);
        }
        public delegate void OnAttackDodgedDelegate(string text, Unit unit, Color color);
        public event OnAttackDodgedDelegate OnAttackDodged;
        #endregion

        #region statGetters
        #region base stat getters
        public int GetDamageBoost() {return stats[StatType.BRAWN].CurrentValue;}
        //public int InitializeMaxHP()
        //{
        //    maxHP = unitBaseHP;// + stats[StatType.HEART].BaseValue;
        //    return maxHP;
        //}
        public int GetMaxHP() {return maxHP;}
        public int GetBaseStatValue(StatType stat)
        {
            return stats[stat].BaseValue;
        }
        public int GetInitiativeBoost() {return stats[StatType.AGILITY].CurrentValue;}
        public int GetAccuracyBoost() { return stats[StatType.FOCUS].CurrentValue; }
        public int GetBoonBoost() { return stats[StatType.HEART].CurrentValue; }

        #region derived stat getters;
        public int GetCritChance() { return (stats[StatType.BRAWN].CurrentValue + stats[StatType.PSYCH].CurrentValue / 2);}
        public int GetBaneBoost() {return (stats[StatType.AGILITY].CurrentValue + stats[StatType.FOCUS].CurrentValue / 2);}
        #endregion
        #endregion
        #endregion

        public string GetName()
        {
            if(unitName.Equals("^NAME^"))
            {
                List<char> allowList = new List<char>() {' ', '-', '\'', ',', '.'};
                string name = SaveDataManager.LoadString("name", "Lux");

                bool nameAllowed = true;
                foreach(char c in name)
                {
                    if(!char.IsLetterOrDigit(c) && !allowList.Contains(c))
                    {
                        nameAllowed = false;
                        break;
                    }
                }

                if(!nameAllowed)
                {
                    name = "Lux";
                    SaveDataManager.SaveString("name", "Lux");
                }

                return name; 
            }

            return unitName;
        }
    }
}