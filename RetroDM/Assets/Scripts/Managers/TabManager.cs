using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TabManager : MonoBehaviour {
    [Header("Panels")]
    public Transform buttonsPanel;
    public Transform panelsPanel;

    [Header("Events")]
    public UnityEvent TabSelectionChangedEvent;

    private int selectedIndex;
    private Tab selectedTab;

    private List<Tab> tabs = new List<Tab>();
    private List<Transform> panels = new List<Transform>();

    private void OnEnable() {
        for(int i = 0; i < buttonsPanel.transform.childCount; i++) {
            GameObject tabGameObject = buttonsPanel.transform.GetChild(i).gameObject;
            Tab tab = tabGameObject.GetComponent<Tab>();
            tab.SetIndex(i);
            if (!tabs.Contains(tab)) {
                tabs.Add(tab);
            }
        }

        foreach(Transform panel in panelsPanel.transform) {
            if (!panels.Contains(panel)) {
                panels.Add(panel);
            }
        }

        ButtonMouseClick(0);
    }

    public void ButtonMouseClick(int id) {
        if(selectedTab != null) {
            selectedTab.ToggleActive();
        }

        selectedIndex = id;
        selectedTab = tabs[selectedIndex];
        selectedTab.ToggleActive();
        HideAllPanels();
    }

    private void HideAllPanels() {
        for(int i = 0; i < panels.Count; i++) {
            if(i == selectedIndex) {
                panels[i].gameObject.SetActive(true);
            } else {
                panels[i].gameObject.SetActive(false);
            }
        }

        TabSelectionChangedEvent?.Invoke();
    }
}
