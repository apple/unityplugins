using System;
using Apple.CoreHaptics;
using UnityEngine;

public class ShieldBehavior : MonoBehaviour
{
	private GameManager _manager;
	private Rigidbody2D _rigidbody;
	private Collider2D _collider;

	private GameObject _ball;
	private BallManager _ballManager;
	private Animator _animator;
	private const int _maxShieldHealth = 4;

	private const float _maximumReasonableVelocity = 20f;

	private CHHapticPatternPlayer _shieldCollisionHapticPlayer;
	[SerializeField] private TextAsset _shieldCollisionAHAP;

	private const string _shieldHealthTrigger = "ShieldHealth";
	private static readonly int _shieldHealthAnimHash = Animator.StringToHash(_shieldHealthTrigger);

	// Start is called before the first frame update
	void Start()
	{
		_manager = GameObject.Find("GameManager").GetComponent<GameManager>();
		if (_manager is null)
		{
			Debug.LogError("Could not find the GameManager.");
		}
		else
		{
			// Use the shared haptic engine to make a player
			_shieldCollisionHapticPlayer = _manager.HapticEngine.MakePlayer(new CHHapticPattern(_shieldCollisionAHAP));
		}

		_rigidbody = gameObject.GetComponent<Rigidbody2D>() ?? gameObject.AddComponent<Rigidbody2D>();

		_collider = gameObject.GetComponent<CircleCollider2D>() ?? gameObject.AddComponent<CircleCollider2D>();

		_ball = GameObject.FindGameObjectWithTag("Ball");
		if (_ball is null)
		{
			Debug.LogError("Could not find the ball, something is very wrong.");
		}
		else
		{
			_ballManager = _ball.GetComponent<BallManager>();
			_animator = _ball.GetComponent<Animator>();
			_collider.sharedMaterial = _ballManager.BallPhysicsMaterial;
		}

		if (!(_animator is null))
		{
			_animator.SetInteger(_shieldHealthAnimHash, _maxShieldHealth);
		}
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

		var currentShieldHealth = _animator.GetInteger(_shieldHealthAnimHash);
		if (currentShieldHealth == 1)
		{
			// Rely on the hapticPlayer.completionHandler to call for destroying "this"
			Debug.Log("Shield health zero'ed.");

			_rigidbody.gravityScale = 0f;
			_rigidbody.velocity = Vector2.zero;

			_ballManager.ShieldZero();
		}
		else
		{
			Debug.Log("Playing shield collision");
			_shieldCollisionHapticPlayer.Start();
		}

		if (!(_animator is null))
		{
			_animator.SetInteger(_shieldHealthAnimHash, currentShieldHealth - 1);
		}
	}
}
