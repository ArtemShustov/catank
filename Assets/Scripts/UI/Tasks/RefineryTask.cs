using System;
using Game.Locations;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Tasks {
	public class RefineryTask: MonoBehaviour {
		[SerializeField] private Sprite _check;
		[SerializeField] private Image _icon;
		[SerializeField] private Refinery _location;

		private void OnEnable() {
			_location.UsedFirstTime += OnUsedFirstTime;
		}
		private void OnDisable() {
			_location.UsedFirstTime -= OnUsedFirstTime;
		}
		private void OnUsedFirstTime() {
			_icon.sprite = _check;
		}
	}
}