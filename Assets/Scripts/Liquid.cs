using UnityEngine;

namespace Game {
    [ExecuteInEditMode]
    public class Liquid : MonoBehaviour {
        private enum UpdateMode {
            Normal,
            UnscaledTime
        }

        [SerializeField] private UpdateMode _updateMode;
        [SerializeField] private float _maxWobble = 0.03f;
        [SerializeField] private float _wobbleSpeedMove = 1f;
        [SerializeField] private float _fillAmount = 0.5f;
        [SerializeField] private float _recovery = 1f;
        [SerializeField] private float _thickness = 1f;
        [SerializeField, Range(0, 1)] private float _compensateShapeAmount;
        [SerializeField] private Mesh _mesh;
        [SerializeField] private Renderer _rend;

        private readonly static int WobbleXid = Shader.PropertyToID("_WobbleX");
        private readonly static int WobbleZid = Shader.PropertyToID("_WobbleZ");
        private readonly static int FillAmountID = Shader.PropertyToID("_FillAmount");

        private Vector3 _pos;
        private Vector3 _lastPos;
        private Vector3 _velocity;
        private Quaternion _lastRot;
        private Vector3 _angularVelocity;
        private float _wobbleAmountX;
        private float _wobbleAmountZ;
        private float _wobbleAmountToAddX;
        private float _wobbleAmountToAddZ;
        private float _pulse;
        private float _sinewave;
        private float _time = 0.5f;
        private Vector3 _comp;

        private MaterialPropertyBlock _propertyBlock;

        private void Start() {
            GetMeshAndRenderer();
            InitializePropertyBlock();
        }

        private void OnValidate() {
            GetMeshAndRenderer();
            InitializePropertyBlock();
        }

        private void GetMeshAndRenderer() {
            if (_mesh == null) {
                _mesh = GetComponent<MeshFilter>().sharedMesh;
            }
            if (_rend == null) {
                _rend = GetComponent<Renderer>();
            }
        }

        private void InitializePropertyBlock() {
            if (_propertyBlock == null) {
                _propertyBlock = new MaterialPropertyBlock();
            }
        }

        public void SetFillAmount(float fillAmount) {
            _fillAmount = fillAmount;
        }
        public void SetMaterial(Material material) {
            _rend.material = material;
        }

        private void Update() {
            if (_rend == null || _mesh == null) return;

            float deltaTime = _updateMode switch {
                UpdateMode.Normal => Time.deltaTime,
                UpdateMode.UnscaledTime => Time.unscaledDeltaTime,
                _ => 0f
            };

            _time += deltaTime;

            if (deltaTime != 0f) {
                _wobbleAmountToAddX = Mathf.Lerp(_wobbleAmountToAddX, 0, deltaTime * _recovery);
                _wobbleAmountToAddZ = Mathf.Lerp(_wobbleAmountToAddZ, 0, deltaTime * _recovery);

                _pulse = 2 * Mathf.PI * _wobbleSpeedMove;
                _sinewave = Mathf.Lerp(_sinewave, Mathf.Sin(_pulse * _time), deltaTime * Mathf.Clamp(_velocity.magnitude + _angularVelocity.magnitude, _thickness, 10f));

                _wobbleAmountX = _wobbleAmountToAddX * _sinewave;
                _wobbleAmountZ = _wobbleAmountToAddZ * _sinewave;

                _velocity = (_lastPos - transform.position) / deltaTime;
                _angularVelocity = GetAngularVelocity(_lastRot, transform.rotation);

                _wobbleAmountToAddX += Mathf.Clamp((_velocity.x + (_velocity.y * 0.2f) + _angularVelocity.z + _angularVelocity.y) * _maxWobble, -_maxWobble, _maxWobble);
                _wobbleAmountToAddZ += Mathf.Clamp((_velocity.z + (_velocity.y * 0.2f) + _angularVelocity.x + _angularVelocity.y) * _maxWobble, -_maxWobble, _maxWobble);
            }

            UpdatePos(deltaTime);

            // Обновляем MaterialPropertyBlock
            _rend.GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetFloat(WobbleXid, _wobbleAmountX);
            _propertyBlock.SetFloat(WobbleZid, _wobbleAmountZ);
            _propertyBlock.SetVector(FillAmountID, _pos);
            _rend.SetPropertyBlock(_propertyBlock);

            _lastPos = transform.position;
            _lastRot = transform.rotation;
        }

        private void UpdatePos(float deltaTime) {
            Vector3 worldPos = transform.TransformPoint(_mesh.bounds.center);

            if (_compensateShapeAmount > 0f) {
                if (deltaTime != 0f) {
                    _comp = Vector3.Lerp(_comp, worldPos - new Vector3(0, GetLowestPoint(), 0), deltaTime * 10f);
                } else {
                    _comp = worldPos - new Vector3(0, GetLowestPoint(), 0);
                }

                _pos = worldPos - transform.position - new Vector3(0, _fillAmount - (_comp.y * _compensateShapeAmount), 0);
            } else {
                _pos = worldPos - transform.position - new Vector3(0, _fillAmount, 0);
            }
        }

        private Vector3 GetAngularVelocity(Quaternion foreLastFrameRotation, Quaternion lastFrameRotation) {
            Quaternion q = lastFrameRotation * Quaternion.Inverse(foreLastFrameRotation);

            if (Mathf.Abs(q.w) > 1023.5f / 1024.0f)
                return Vector3.zero;

            float gain;
            if (q.w < 0f) {
                float angle = Mathf.Acos(-q.w);
                gain = -2f * angle / (Mathf.Sin(angle) * Time.deltaTime);
            } else {
                float angle = Mathf.Acos(q.w);
                gain = 2f * angle / (Mathf.Sin(angle) * Time.deltaTime);
            }

            Vector3 angularVelocity = new Vector3(q.x * gain, q.y * gain, q.z * gain);

            if (float.IsNaN(angularVelocity.z))
                angularVelocity = Vector3.zero;

            return angularVelocity;
        }

        private float GetLowestPoint() {
            float lowestY = float.MaxValue;
            Vector3[] vertices = _mesh.vertices;

            foreach (Vector3 vertex in vertices) {
                Vector3 position = transform.TransformPoint(vertex);
                if (position.y < lowestY) {
                    lowestY = position.y;
                }
            }
            return lowestY;
        }
    }
}
