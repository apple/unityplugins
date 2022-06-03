using Apple.CoreHaptics;
using System.Collections.Generic;
using System;
using UnityEngine;

public class BallManager : MonoBehaviour
{
	private GameManager _manager;

	private GameObject _shield;

	private Rigidbody2D _rigidbody;
	private Collider2D _collider;

	private CHHapticEngine _engine;

	private CHHapticAdvancedPatternPlayer _textureHapticPlayer;
	private CHHapticPatternPlayer _smallCollisionHapticPlayer;
	private CHHapticPatternPlayer _largeCollisionHapticPlayer;
	private CHHapticAdvancedPatternPlayer _implodeHapticPlayer;

	[SerializeField] private TextAsset _spawnAHAP;
	[SerializeField] private TextAsset _smallCollisionAHAP;
	[SerializeField] private TextAsset _growAHAP;
	[SerializeField] private TextAsset _largeCollisionAHAP;
	[SerializeField] private TextAsset _shieldOnAHAP;
	[SerializeField] private TextAsset _implodeAHAP;
	[SerializeField] private TextAsset _rollingTextureAHAP;

	[SerializeField] private GameObject _shieldPrefab;

	[SerializeField] private Material _ballMaterial;

	[NonSerialized] public PhysicsMaterial2D BallPhysicsMaterial;

	private const float _bounciness = 0.8f;
	private const float _friction = 0.6f;

	private BallState _ballSize = BallState.Small;
	private enum BallState
	{
		Small,
		Large,
		Shield
	}

	// Anything >= this value will result in intensity of 1.0
	private const float _maximumReasonableVelocity = 20f;

	// Animations
	private Animator _animator;
	private const string _spawnTrigger = "Spawn";
	private const string _growthTrigger = "Grow";
	private const string _shieldOnTrigger = "ShieldOn";
	private const string _explode = "Explode";
	private static readonly int _spawnAnimHash = Animator.StringToHash(_spawnTrigger);
	private static readonly int _growAnimHash = Animator.StringToHash(_growthTrigger);
	private static readonly int _shieldAnimHash = Animator.StringToHash(_shieldOnTrigger);
	private static readonly int _explodeAnimHash = Animator.StringToHash(_explode);

	private bool _hasTexture = false;
	public bool HasTexture
	{
		get
		{
			return _hasTexture;
		}
		set
		{
			if (value != _hasTexture && !(_textureHapticPlayer is null))
			{
				if (value)
				{
					Debug.Log("Starting texture haptics.");
					_textureHapticPlayer.Start();
				}
				else
				{
					Debug.Log("Stopping texture haptics.");
					_textureHapticPlayer.Stop();
				}
			}
			_hasTexture = value;
		}
	}

	public void Start()
	{
		_manager = GameObject.Find("GameManager").GetComponent<GameManager>();
		if (_manager is null)
		{
			Debug.LogError("Could not find the GameManager.");
		}
		else
		{
			_engine = _manager.HapticEngine;

			SetupHapticPlayers();

			if (HasTexture)
			{
				_textureHapticPlayer.Start();
			}
		}

		_rigidbody = gameObject.GetComponent<Rigidbody2D>() ?? gameObject.AddComponent<Rigidbody2D>();

		_collider = gameObject.GetComponent<CircleCollider2D>() ?? gameObject.AddComponent<CircleCollider2D>();

		SetMaterialProperties();

		_ballSize = BallState.Small;
		_animator = GetComponent<Animator>();
		if (!(_animator is null))
		{
			_animator.ResetTrigger(_spawnAnimHash);
			_animator.ResetTrigger(_growAnimHash);
			_animator.ResetTrigger(_shieldAnimHash);
			_animator.ResetTrigger(_explodeAnimHash);
		}

		// Use the CHHapticEngine OneShot API to play a haptic without a pattern player
		_engine.PlayPatternFromAhap(_spawnAHAP);
		_animator.SetTrigger(_spawnAnimHash);
	}

	public void FixedUpdate()
	{
		if (!(_shield is null))
		{
			// Force the ball to track the shield
			var shieldPos = _shield.transform.position;
			var newPos = new Vector3(shieldPos.x, shieldPos.y, shieldPos.z - 0.1f);
			transform.position = newPos;
			_rigidbody.velocity = _shield.gameObject.GetComponent<Rigidbody2D>().velocity;
		}

		if (HasTexture && !(_textureHapticPlayer is null))
		{
			UpdateTextureIntensity();
		}
	}

	public void OnApplicationPause(bool pause)
	{
		if (pause)
		{
			if (!(_textureHapticPlayer is null))
			{
				_textureHapticPlayer.Stop();
			}
		}
		else
		{
			if (_hasTexture)
			{
				if (!(_engine is null) && !(_textureHapticPlayer is null))
				{
					_engine.Start();
					_textureHapticPlayer.Start();
				}
			}
		}
	}

	private void SetupHapticPlayers()
	{
		_textureHapticPlayer = _engine.MakeAdvancedPlayer(new CHHapticPattern(_rollingTextureAHAP));
		_textureHapticPlayer.LoopEnabled = true;
		_textureHapticPlayer.LoopEnd = 0f;

		_smallCollisionHapticPlayer = _engine.MakePlayer(new CHHapticPattern(_smallCollisionAHAP));
		_largeCollisionHapticPlayer = _engine.MakePlayer(new CHHapticPattern(_largeCollisionAHAP));

		_implodeHapticPlayer = _engine.MakeAdvancedPlayer(new CHHapticPattern(_implodeAHAP));
		_implodeHapticPlayer.CompletionHandler += ImplosionHapticCompletion;
	}

	private void ImplosionHapticCompletion()
	{
		Debug.Log("Destroying ball after implosion haptics complete.");
		_manager.DestroyBall();
	}

	public void OnCollisionEnter2D(Collision2D collision)
	{
		if (Math.Abs(_rigidbody.velocity.y) < (0.1 * _maximumReasonableVelocity)
			&& (collision.gameObject.name.Equals("Top") || collision.gameObject.name.Equals("Bottom")))
		{
			_rigidbody.velocity = new Vector3(_rigidbody.velocity.x, 0f, 0f);
			return;
		}

		if (Math.Abs(_rigidbody.velocity.x) < (0.1 * _maximumReasonableVelocity)
			&& (collision.gameObject.name.Equals("Left") || collision.gameObject.name.Equals("Right")))
		{
			_rigidbody.velocity = new Vector3(0f, _rigidbody.velocity.y, 0f);
			return;
		}

		switch (_ballSize)
		{
			case BallState.Small:
				Debug.Log("Playing small collision");
				_smallCollisionHapticPlayer.Start();
				break;
			case BallState.Large:
				Debug.Log("Playing large collision");
				_largeCollisionHapticPlayer.Start();
				break;
			case BallState.Shield:
				break;
			default:
				Debug.LogWarning("Unknown ball size.");
				break;
		}
	}

	private void SetMaterialProperties()
	{
		BallPhysicsMaterial = new PhysicsMaterial2D("Rubber_physics")
		{
			bounciness = _bounciness,
			friction = _friction
		};

		gameObject.GetComponent<Renderer>().material = _ballMaterial;
		_collider.sharedMaterial = BallPhysicsMaterial;
	}

	public void BallTapped()
	{
		if (!(_animator is null))
		{
			switch (_ballSize)
			{
				case BallState.Small:
					Debug.Log("Growing from small to large");
					_animator.ResetTrigger(_spawnAnimHash);
					_animator.ResetTrigger(_shieldAnimHash);
					_animator.SetTrigger(_growAnimHash);
					if (!(_growAHAP is null))
					{
						_engine.PlayPatternFromAhap(_growAHAP);
					}
					_ballSize = BallState.Large;
					break;

				case BallState.Large:
					Debug.Log("Applying shield");
					_animator.ResetTrigger(_spawnAnimHash);
					_animator.ResetTrigger(_growAnimHash);
					_animator.SetTrigger(_shieldAnimHash);
					if (!(_shieldOnAHAP is null))
					{
						_engine.PlayPatternFromAhap(_shieldOnAHAP);
					}
					_ballSize = BallState.Shield;
					var pos = transform.position;
					var shieldPos = new Vector3(pos.x, pos.y, pos.z + 0.1f);
					_shield = Instantiate(_shieldPrefab, shieldPos, Quaternion.identity);

					// Prevent the ball from bouncing off the shield
					Destroy(_collider);
					break;

				case BallState.Shield:
					break;
				default:
					Debug.LogWarning("Unknown ball size.");
					break;
			}
		}
	}

	public void ShieldZero()
	{
		Debug.Log("Playing implosion haptics.");

		_rigidbody.gravityScale = 0f;
		_rigidbody.velocity = Vector2.zero;

		Destroy(_shield);
		_shield = null;

		_implodeHapticPlayer.Start();
	}

	private void UpdateTextureIntensity()
	{
		var currentSpeed = _rigidbody.velocity.magnitude;
		var intensity = Math.Min(currentSpeed / _maximumReasonableVelocity, 1f);
		var hapticParameters = new List<CHHapticParameter>
			{
				new CHHapticParameter(
					parameterId: CHHapticDynamicParameterID.HapticIntensityControl,
					parameterValue: intensity
				)
			};

		Debug.Log($"Sending intensity {intensity} to texture player.");
		_textureHapticPlayer.SendParameters(hapticParameters);
	}
}
