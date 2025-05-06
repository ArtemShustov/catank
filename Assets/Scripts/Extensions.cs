using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

namespace Game {
	public static class Extensions {
		public static void Set(this LocalizedString localizedString, string key, string value) {
			if (localizedString == null) {
				return;
			}
			
			if (localizedString.TryGetValue(key, out var variable) && variable is StringVariable strVar) {
				strVar.Value = value;
			} else {
				var stringVariable = new StringVariable {
					Value = value
				};
				localizedString[key] = stringVariable;
			}
		}
		public static void Set(this LocalizedString localizedString, string key, LocalizedString value) {
			if (localizedString == null) {
				return;
			}
			localizedString[key] = value;
		}
		public static T Random<T>(this ICollection<T> collection) {
			if (collection == null || collection.Count == 0) {
				return default;
			}
			return collection.ToArray()[UnityEngine.Random.Range(0, collection.Count)];
		}
		public static void Forget(this Task task) {
			if (task == null)
				return;

			task.ContinueWith(t => {
				if (t.Exception != null) {
					Debug.LogError($"Error in Task: {t.Exception.InnerException}");
				}
			}, TaskScheduler.FromCurrentSynchronizationContext());
		}
		public static async Task ToggleCameraAsync(this CinemachineCamera cam, float duration, CancellationToken cancellationToken = default) {
			cam.gameObject.SetActive(true);
			cam.Priority = 1000;
			await Awaitable.WaitForSecondsAsync(duration, cancellationToken);
			cam.gameObject.SetActive(false);
			cam.Priority = 0;
		}
	}
}