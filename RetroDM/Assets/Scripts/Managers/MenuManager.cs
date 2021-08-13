using System;
using UnityEngine;

public class MenuManager : MonoBehaviour {
    public static MenuManager Instance;
    public event Action<string> OnMenuOpened;
    public event Action<string> OnMenuClosed;
    [SerializeField] private Menu[] menus;

    private void Awake() => Instance = this;

    public void OpenMenu(string menuName) {
        for (int i = 0; i < menus.Length; i++) {
            if (menus[i].menuName == menuName) {
                OnMenuOpened?.Invoke(menuName);
                menus[i].Open();
            } else if (menus[i].isOpen) {
                CloseMenu(menus[i]);
            }
        }
    }

    public void OpenMenu(Menu menu) {
        for (int i = 0; i < menus.Length; i++) {
            if (menus[i].isOpen) {
                CloseMenu(menus[i]);
            }
        }
        menu.Open();        
    }

    public void CloseMenu(Menu menu) {
        OnMenuClosed?.Invoke(menu.menuName);
        menu.Close();
    }
}
