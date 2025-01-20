using RobbieWagnerGames.Utilities;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RobbieWagnerGames
{
    public class EventSystemManager : MonoBehaviourSingleton<EventSystemManager>
    {
        public EventSystem eventSystem;

        public void SetSelectedGameObject(GameObject go)
        {
            eventSystem.SetSelectedGameObject(go);
        }
    }
}