using Game.Locations;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

namespace Game.UI.Tasks {
	public class PumpTask: MonoBehaviour {
		[SerializeField] private Sprite _check;
		[SerializeField] private Image _icon;
		[SerializeField] private LocalizeStringEvent _text;
		[SerializeField] private int _goal = 6000;
		[SerializeField] private Pumps _location;
		private bool _completed;
		private int _current;

		private const string CURRENT_KEY = "current";
		private const string GOAL_KEY = "goal";

		private void RefreshText() {
			_text.StringReference.Set(CURRENT_KEY, $"{_current / 1000:0.#}");
			_text.StringReference.Set(GOAL_KEY, $"{_goal / 1000:0}");
		}
		
		private void OnEnable() {
			_location.OilTaken += OnOilTaken;
			RefreshText();
		}
		private void OnDisable() {
			_location.OilTaken -= OnOilTaken;
		}
		private void OnOilTaken(int value) {
			if (_completed) {
				return;
			}
			
			_current += value;
			RefreshText();

			if (_current >= _goal) {
				_current = _goal;
				_completed = true;
				
				_icon.sprite = _check;
			}
		}
	}
}