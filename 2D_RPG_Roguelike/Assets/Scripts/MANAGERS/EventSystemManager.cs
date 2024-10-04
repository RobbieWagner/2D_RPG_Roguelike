using UnityEngine;
using UnityEngine.EventSystems;

namespace RobbieWagnerGames
{
    public class EventSystemManager : MonoBehaviour
    {
        public EventSystem eventSystem;

        public static EventSystemManager Instance { get; private set; }
        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
                Instance = this;
        }

        public void SetSelectedGameObject(GameObject go)
        {
            if(go != null)
                Debug.Log($"Set event system selected game object to {go.name}");
            eventSystem.SetSelectedGameObject(go);
        }
    }
}