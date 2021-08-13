using UnityEngine;

public class Crosshair : MonoBehaviour {
	private RectTransform reticle;
	private float currentSize;
	[SerializeField] private float restingSize;
	[SerializeField] private float walkingSize;
	[SerializeField] private float speed;

	private void Start() {
		reticle = GetComponent<RectTransform>();
	}

	private void Update() {
		if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0) {
			currentSize = Mathf.Lerp(currentSize, walkingSize, speed * Time.deltaTime);
		} else {
			currentSize = Mathf.Lerp(currentSize, restingSize, speed * Time.deltaTime);
		}
		reticle.sizeDelta = new Vector2(currentSize, currentSize);
	}
}
