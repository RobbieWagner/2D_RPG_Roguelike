using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RobbieWagnerGames.StrategyCombat.Units
{
    [System.Serializable]
    public class SerializableAlly
    {
        public string UnitName;
        public int HP;
        public int MaxHP;

        List<UnitStat> stats = new List<UnitStat>();

        public List<string> actionFilePaths;
        //public string animatorFilePath;
        public string headSpriteRelativePath;
        public string dialogueSpriteRelativePath;

        //public string mentality = MentalityType.FINE.ToString();

        //TODO: add refs for unitanimator.

        public SerializableAlly(Unit unit)
        {
            UnitName = unit.unitName;
            HP = unit.HP;
            MaxHP = unit.maxHP;

            stats = unit.Stats.Select(x => x.Value).ToList();

            actionFilePaths = unit.unitActions.Select(a => StaticGameStats.GetCombatActionResourcePath(a)).ToList();
            //animatorFilePath = unit.GetAnimatorResourcePath();

            //Debug.Log($"new serializable unit: {ToString()}");
        }

        public SerializableAlly() {}

        public void InitializeAlly(ref Ally allyInstance)
        {
            allyInstance.InitializeStats(stats);
            allyInstance.unitName = UnitName;
            //allyInstance.InitializeMaxHP();
            allyInstance.HP = HP;

            allyInstance.unitActions = LoadActions();
        }

        public List<CombatAction> LoadActions()
        {
            List<CombatAction> actions = new List<CombatAction>();

            foreach(string file in actionFilePaths)
                actions.Add(Resources.Load<CombatAction>(file));

            return actions;
        }

        public override string ToString()
        {
            string statsString = JsonUtility.ToJson(stats);

            return $"-----\n{UnitName}:\nHP:{HP}\n{statsString}\nActions:{string.Join(",", actionFilePaths)}\n-----";
        }
    }
}