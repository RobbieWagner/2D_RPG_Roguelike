using RobbieWagnerGames.TurnBasedCombat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RobbieWagnerGames
{
    public class Interactable : MonoBehaviour
    {

        [HideInInspector] public bool canInteract;

        [SerializeField] protected SpriteRenderer visualCue;

        protected virtual void Awake()
        {
            canInteract = false;
            if (visualCue != null) visualCue.enabled = false;
            InputManager.Instance.gameControls.EXPLORATION.Interact.performed += OnInteract;
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.CompareTag("Player") && ExplorationManager.Instance.IsObjectInteractionEnabled) 
            {
                canInteract = true;
                if (visualCue != null) visualCue.enabled = true;
            }
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            if(other.gameObject.CompareTag("Player")) 
            {
                canInteract = false;
                if (visualCue != null) visualCue.enabled = false;
            }
        }

        protected virtual void OnInteract(InputAction.CallbackContext context)
        {
            if(canInteract && ExplorationManager.Instance.CurrentInteractable == null && ExplorationManager.Instance.IsObjectInteractionEnabled)
            {
                ExplorationManager.Instance.CurrentInteractable = this;
                if(PlayerMovement.Instance != null) PlayerMovement.Instance.CeasePlayerMovement();
                StartCoroutine(Interact());
            }
        }

        protected virtual void OnUninteract()
        {
            ExplorationManager.Instance.CurrentInteractable = null;
            canInteract = true;
            if(PlayerMovement.Instance != null) 
                PlayerMovement.Instance.CanMove = true;
        }

        protected virtual IEnumerator Interact()
        {
            yield return null;
            //Debug.Log("Interact");

            OnUninteract();
            StopCoroutine(Interact());
        }
    }
}