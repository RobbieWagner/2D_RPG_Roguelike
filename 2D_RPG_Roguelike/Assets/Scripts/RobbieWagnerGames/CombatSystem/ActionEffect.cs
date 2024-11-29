using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RobbieWagnerGames.StrategyCombat
{
    [Serializable]
    public class ActionEffect
    {
        [Header("General Info")]
        [SerializeField][Range(1,101)] public float accuracy = 101f;
        [SerializeField] private bool failureStopsActionExecution;
        public bool FailureStopsActionExecution 
        { 
            get { return failureStopsActionExecution; }
            private set { failureStopsActionExecution = value; }
        }
    }

    [Serializable]
    public class Attack : ActionEffect
    {
        [Header("Attack")]
        [SerializeField] private int power = 10;

        public int CalculateDamage(Unit user, Unit target)
        {
            int damage = power; // * user.strength - target.defense
            if(damage < 1) damage = 1; 
            return damage;
        }

        public bool AttemptAttack(Unit user, Unit target)
        {
            bool hit = false;
            if (accuracy > 100)
                hit = true;
            else
                hit = UnityEngine.Random.Range(0, 100) < accuracy;

            if (hit)
                target.DealDamage(CalculateDamage(user, target));

            return hit;
        }
    }

    [Serializable]
    public class Heal : ActionEffect
    {
        [Header("Heal")]
        [SerializeField] private int power = 10;
        public bool healsSelf;

        public int CalculateHealing(Unit user, Unit target)
        {
            int heal = power; // * user.care
            if(heal < 1) heal = 1; 
            return heal;
        }

        public bool AttemptHeal(Unit user, Unit target)
        {
            bool success = false;
            if (accuracy > 100)
                success = true;
            else
                success = UnityEngine.Random.Range(0, 100) < accuracy;

            if (success)
                target.Heal(CalculateHealing(user, target));

            return success;
        }
    }

    [Serializable]
    public class StatRaise: ActionEffect
    {
        [Header("Stat Raise")]
        [SerializeField] private int power = 1;
        [SerializeField] public StatType stat;
        public int CalculateStatChange(Unit user, Unit target)
        {
            int statChange = power;
            if(power < 1) power = 1;
            return power;
        }

        public bool AttemptStatRaise(Unit user, Unit target)
        {
            bool success = false;
            if (accuracy > 100)
                success = true;
            else
                success = UnityEngine.Random.Range(0, 100) < accuracy;

            if (success)
                target.ChangeStatValue(stat, CalculateStatChange(user, target));

            return success;
        }
    }

    [Serializable]
    public class StatLower: ActionEffect
    {
        [Header("Stat Lower")]
        [SerializeField] private int power = 1;
        [SerializeField] public StatType stat;

        public int CalculateStatChange(Unit user, Unit target)
        {
            int statChange = power;
            if(power < 1) power = 1;
            return -power;
        }

        public bool AttemptStatLower(Unit user, Unit target)
        {
            bool success = false;
            if (accuracy > 100)
                success = true;
            else
                success = UnityEngine.Random.Range(0, 100) < accuracy;

            if (success)
                target.ChangeStatValue(stat, CalculateStatChange(user, target));

            return success;
        }
    }

    [Serializable]
    public class Pass: ActionEffect
    {

    }
}