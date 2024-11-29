using RobbieWagnerGames.TurnBasedCombat;
using UnityEngine;

public class CombatBattlefield : MonoBehaviour
{
    public Vector3 scenePosition;

    public static CombatBattlefield Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;

        transform.position = scenePosition;
    }
}
