using QueueConnect.Config;
using QueueConnect.GameSystem;
using UnityEngine;

namespace QueueConnect.Robot
{
    [RequireComponent(typeof(SpriteRenderer), typeof(Rigidbody2D))]
    public class RobotExplosion : MonoBehaviour
    {
        #region Inspector Fields
            [Tooltip("RigidBody2D Component")]
            [SerializeField] private Rigidbody2D rigidBody2D;
            [Tooltip("Robot GameObject")]
            [SerializeField] private GameObject robot;
        #endregion

        #region Privates
            private Vector3 originPosition;
            private Vector3 originRotation;
            private bool shake;
            private float duration;
        #endregion

        private void Awake()
        {
            if (rigidBody2D == null)
            {
                rigidBody2D = GetComponent<Rigidbody2D>();
            }
            if (robot == null)
            {
                robot = GetComponentInParent<RobotBehaviour>().gameObject;
            }
        }

        private void Update()
        {
            Shake();
        }

        /// <summary>
        /// Initializes values for this Object
        /// </summary>
        public void Initialize()
        {
            rigidBody2D.simulated = false;
            rigidBody2D.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;

            var _transform = transform;
            var _localPosition = _transform.localPosition;
            var _localEulerAngles = _transform.localEulerAngles;
            originPosition = new Vector3(_localPosition.x, _localPosition.y, _localPosition.z);
            originRotation = new Vector3(_localEulerAngles.x, _localEulerAngles.y, _localEulerAngles.z);
        }

        /// <summary>
        /// Starts the explosion on this Robot Part
        /// </summary>
        /// <param name="_SkipShake">Skips the shake if set to "true"</param>
        public void StartExplosion(bool _SkipShake)
        {
            shake = true;
            if (_SkipShake)
            {
                duration = GameConfig.ShakeDuration;
            }
        }

        /// <summary>
        /// Shakes this Part before it explodes
        /// </summary>
        private void Shake()
        {
            if (shake && duration <= GameConfig.ShakeDuration)
            {
                duration += Time.deltaTime * GameController.CustomTimeScale;

                var _random = Random.Range(GameConfig.ShakeAmount.x, GameConfig.ShakeAmount.y);

                transform.localPosition = new Vector2(originPosition.x + _random, originPosition.y + _random);
            }

            if (!(duration >= GameConfig.ShakeDuration)) return;
                shake = false;
                duration = 0f;

                Explode();
        }

        /// <summary>
        /// Adds a Force to the Robot Parts and makes them fly away
        /// </summary>
        private void Explode()
        {        
            var _offsetX = 0f;
            var _offsetY = 0f;

            var _position = transform.position;
            var _partPosition = _position;
            var _robotPosition = robot.transform.position;
            var _direction = new Vector2(_partPosition.x - _robotPosition.x, _partPosition.y - _robotPosition.y);

            // Only moves the Parts in a direction they already have from the center of the Robot (Left arm can only move to the left, right arm can only move to the right, etc.)
            _offsetX = _direction.x < 0 ? Random.Range(-.5f, 0f) : Random.Range(0f, .5f);
            _offsetY = _direction.y < 0 ? Random.Range(-.5f, 0f) : Random.Range(0f, .5f);

            _direction = new Vector2(_direction.x + _offsetX, _direction.y + _offsetY).normalized;

            var _rotationForceX = Random.Range(GameConfig.RotationSpeed.x, GameConfig.RotationSpeed.y);
            var _rotationForceY = Random.Range(GameConfig.RotationSpeed.x, GameConfig.RotationSpeed.y);
            
            rigidBody2D.simulated = true;
            rigidBody2D.constraints = RigidbodyConstraints2D.None;
            rigidBody2D.mass = GameConfig.PartMass;
            rigidBody2D.gravityScale = GameConfig.PartGravity;
            rigidBody2D.AddForceAtPosition(_direction * GameConfig.ExplosionForce, new Vector2(_position.x + _rotationForceX, _position.y + _rotationForceY), GameConfig.ForceMode);
        }

        /// <summary>
        /// Resets this Part to its original values
        /// </summary>
        public void ResetPosition()
        {
            rigidBody2D.velocity = Vector2.zero;
            rigidBody2D.angularVelocity = 0f;
            rigidBody2D.simulated = false;
            rigidBody2D.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;

            var _transform = transform;
            _transform.localPosition = originPosition;
            _transform.localEulerAngles = originRotation;
        }
    }
}