using UnityEngine;
using UnityEngine.EventSystems;

public class Tab : MonoBehaviour, IPointerClickHandler {
    private int tabIndex;
    private TabManager tabManager;
    private bool isActive = false;

    private void Awake() {
        tabManager = FindObjectOfType<TabManager>();
    }

    public void SetIndex(int index) {
        tabIndex = index;
    }

    public void ToggleActive() => isActive = !isActive;

    public void OnPointerClick(PointerEventData eventData) {
        tabManager.ButtonMouseClick(tabIndex);
    }
}
