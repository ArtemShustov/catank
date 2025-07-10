using System.Threading;
using System.Threading.Tasks;
using EasyButtons;
using Game.Fluids;
using UnityEngine;

namespace Game {
	public class BubbleEffect: MonoBehaviour {
		[SerializeField] private float _duration = 1;
		[SerializeField] private ParticleSystem _particles;
		[SerializeField] private AudioSource _source;
		private MaterialPropertyBlock _block;
		private CancellationTokenSource _animation;
		
		private readonly static int LitColor = Shader.PropertyToID("_LitColor");
		private readonly static int DarkColor = Shader.PropertyToID("_DarkColor");
		private readonly static int TopColor = Shader.PropertyToID("_TopColor");
		private readonly static int BottomColor = Shader.PropertyToID("_BottomColor");

		private void Awake() {
			_block = new MaterialPropertyBlock();
		}

		public void SetFluid(Fluid fluid) {
			var renderer = _particles.GetComponent<ParticleSystemRenderer>();
			renderer.GetPropertyBlock(_block);
			_block.SetColor(LitColor, fluid.Material.GetColor(TopColor));
			_block.SetColor(DarkColor, fluid.Material.GetColor(BottomColor));
			renderer.SetPropertyBlock(_block);
		}
		
		[Button(Mode = ButtonMode.EnabledInPlayMode)]
		public void Play(Vector3 start, Vector3 end) {
			_animation?.Cancel();
			
			transform.position = start;
			transform.LookAt(end);
			var dist = Vector3.Distance(transform.position, end);
			var module = _particles.main;
			module.startLifetime = dist / module.startSpeed.Evaluate(0);
			
			_source.volume = 1;
			_source.Play();
			_particles.Play();
		}
		[Button(Mode = ButtonMode.EnabledInPlayMode)]
		public void Stop() {
			_particles.Stop();
			_animation = new CancellationTokenSource();
			StopAsync(_animation.Token).Forget();
		}

		private async Task StopAsync(CancellationToken cancellationToken = default) {
			var duration = 0.25f;
			
			var t = 0f;
			while (t < duration) {
				if (cancellationToken.IsCancellationRequested) {
					break;
				}
				
				t += Time.deltaTime;
				_source.volume = 1 - t / duration;
				await Awaitable.NextFrameAsync(cancellationToken);
			}
			
			_source.Stop();
			_source.volume = 1;
		}
		public async Task TransferAsync(FluidContainer from, FluidContainer to, bool clearFluids = true) {
			var player = GameRoot.Instance.Player;
			var fluid = from.Fluid;

			SetFluid(fluid);
			Play(from.transform.position + Vector3.up, to.transform.position + Vector3.up);
			player.DisableInput();
			
			to.SetFluid(fluid);
			var startFrom = from.Stored;
			var startTo = to.Stored;
			var count = GetTransferCount(from, to);
			
			var t = 0f;
			while (t < _duration) {
				t += Time.deltaTime;
				var c = Mathf.RoundToInt(Mathf.Lerp(0, count, t / _duration));
				from.SetFill((startFrom - c) / (float)from.Capacity);
				to.SetFill((startTo + c) / (float)to.Capacity);
				await Awaitable.NextFrameAsync();
			}

			count = GetTransferCount(from, to);
			from.SetStored(from.Stored - count);
			if (clearFluids && from.Fill == 0) {
				from.SetFluid(null);
			}
			to.SetStored(to.Stored + count);
			
			player.EnableInput();
			Stop();
		}
		private int GetTransferCount(FluidContainer from, FluidContainer to) => Mathf.Min(from.Stored, to.Capacity - to.Stored);
	}
}