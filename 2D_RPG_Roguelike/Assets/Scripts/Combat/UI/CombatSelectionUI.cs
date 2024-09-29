using RobbieWagnerGames.StrategyCombat;
using RobbieWagnerGames.StrategyCombat.Units;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CombatSelectionUI : MonoBehaviour
{
    [SerializeField] protected Canvas canvas;

    protected Dictionary<Ally, int> savedMainSelectionIndices = new Dictionary<Ally, int>();
    protected Dictionary<Ally, int> savedActionSelectionIndices = new Dictionary<Ally, int>();
    protected Dictionary<Ally, int> savedTargetSelectionIndices = new Dictionary<Ally, int>();

    protected CombatControls controls;

    protected Ally selectingAlly;

    protected virtual void Awake()
    {
        controls = new CombatControls();

    }

    public virtual void SetupUI(Ally selectingUnit)
    {
        selectingAlly = selectingUnit;
        DisplayMainSelectionUI();
    }

    public virtual void DisplayMainSelectionUI()
    {
        if (selectingAlly != null)
        {

        }
        else
            Debug.LogWarning("Could not display combat selection UI: unit found null!!");
    }

    public virtual void DisplayActionSelectionUI() 
    {

    }

    #region target selection
    public virtual void DisplayTargetSelectionUI()
    {

    }

    public void NavigateTargets(InputAction.CallbackContext context)
    {
        float inputValue = context.ReadValue<float>();

        if(inputValue < 0)
        {

        }
    }
    #endregion
}
