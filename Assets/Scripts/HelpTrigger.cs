using System;
using UnityEngine;
using UnityEngine.Events;

namespace Game {
	public class HelpTrigger: MonoBehaviour {
		[SerializeField] private UnityEvent _event;

		private void OnTriggerEnter(Collider other) {
			if (other.TryGetComponent<Player>(out var player)) {
				_event.Invoke();
			}
		}
	}
}