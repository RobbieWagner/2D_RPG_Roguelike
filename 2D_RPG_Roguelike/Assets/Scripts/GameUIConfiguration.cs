using RobbieWagnerGames.StrategyCombat;
using RobbieWagnerGames.TurnBasedCombat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUIConfiguration : MonoBehaviour
{
    public static GameUIConfiguration Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    public Color GetStatColor(StatType stat)
    {
        switch(stat)
        {
            case StatType.BRAWN:
                return new Color(.65f,.16f,.16f, 1f); // BROWN
            case StatType.AGILITY:
                return Color.green;
            case StatType.FOCUS:
                return Color.yellow;
            case StatType.HEART:
                return new Color(1,.71f,.75f,1f);
            case StatType.PSYCH:
                return Color.cyan;
            case StatType.NONE:
                return Color.red;
        }
        return Color.white;
    }

    public Color GetHPChangeColor(int delta)
    {
        return delta == 0 ? Color.gray : delta > 0 ? Color.green : Color.red;
    }
}
