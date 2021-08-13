using System;
using UnityEngine;

public class Timer {

	public static Timer SetTimer(float time, Action action) {
		GameObject gameObject = new GameObject("Timer", typeof(MonoBehaviourHook));
		Timer timer = new Timer(time, action, gameObject);
		gameObject.GetComponent<MonoBehaviourHook>().onUpdate = timer.Update;

		return timer;
	}

	public class MonoBehaviourHook : MonoBehaviour {
		public Action onUpdate;
		private void Update() {
			if (onUpdate != null) {
				onUpdate();
			}
		}
	}

	private Action action;
	private bool isDestroyed;
	private float time;
	private GameObject gameObject;

	public Timer(float time, Action action, GameObject gameObject) {
		this.time = time;
		this.action = action;
		this.gameObject = gameObject;
		isDestroyed = false;
	}

	public void Update() {
		if (!isDestroyed) {
			time -= Time.deltaTime;
			if (time < 0) {
				action();
				DestroySelf();
			}
		}
	}

	private void DestroySelf() {
		isDestroyed = true;
		UnityEngine.Object.Destroy(gameObject);
	}
}
