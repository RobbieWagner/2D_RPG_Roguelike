using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

namespace RobbieWagnerGames.UI
{
    public class MenuWithTabs : Menu
    {
        [SerializeField] public bool navigateMenuHorizontally = true;

        [SerializeField] protected LayoutGroup tabBar;
        [SerializeField] protected List<TextMeshProUGUI> tabBarTextObjects;
        [SerializeField] protected List<MenuTab> menus;
        [SerializeField] protected Color inactiveColor;
        [SerializeField] protected Color activeColor;

        private UIControls uiControls;

        private int activeTab = -1;
        public int ActiveTab
        {
            get { return activeTab; }
            set 
            {
                if(value == activeTab || menus.Count == 0) return;
                DisableActiveTab();

                activeTab = value;
                if(activeTab < 0)
                {
                    activeTab = menus.Count - 1;
                }
                else if(activeTab >= menus.Count) 
                {
                    activeTab = 0;
                }

                EnableTab(activeTab);
                OnActiveTabChanged?.Invoke(activeTab);
            }
        }
        public delegate void OnActiveTabChangedDelegate(int tab);
        public event OnActiveTabChangedDelegate OnActiveTabChanged;

        protected override void Awake()
        {
            base.Awake();
            uiControls = new UIControls();
            uiControls.UI.NavigateTab.performed += NavigateTab;
        }

        protected override void OnEnable()
        {
            BuildMenu();
            base.OnEnable();
            ActiveTab = 0;
            EnableTab(ActiveTab);
            uiControls.Enable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            uiControls.Disable();
        }

        protected virtual void BuildMenu()
        {
            foreach(MenuTab tab in menus)
                tab.BuildTab();
        }

        private void NavigateTab(InputAction.CallbackContext context)
        {
            float value = context.ReadValue<float>();

            if (value > 0)
            {
                ActiveTab++;
            }
            else if (value < 0)
            {
                ActiveTab--;
            }
        }

        public virtual void EnableTab(int tab)
        {
            foreach(MenuTab menuTab in menus)
                menuTab.gameObject.SetActive(false);
            foreach(TextMeshProUGUI text in tabBarTextObjects)
                text.color = inactiveColor;

            menus[tab].gameObject.SetActive(true);
            tabBarTextObjects[tab].color = activeColor;
        }

        public virtual void DisableActiveTab()
        {
            if(ActiveTab > -1 && ActiveTab < menus.Count)
            {
                menus[ActiveTab].gameObject.SetActive(false);
                tabBarTextObjects[ActiveTab].color = inactiveColor;
            }
        }
    }
}