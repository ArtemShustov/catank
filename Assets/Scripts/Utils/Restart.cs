using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game.Utils {
	public class Restart: MonoBehaviour {
		[SerializeField] private Button _button;
		
		public void RestartLevel() {
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}

		private void OnEnable() {
			_button.onClick.AddListener(RestartLevel);
		}
		private void OnDisable() {
			_button.onClick.RemoveListener(RestartLevel);
		}
	}
}