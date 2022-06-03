using Apple.CoreHaptics;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.UI;

[SuppressMessage("ReSharper", "Unity.PerformanceCriticalCodeCameraMain")]
public class GameManager : MonoBehaviour
{
	private GameObject _ball;
	private bool _hasSpawnedFirstBall = false;

	private Camera _mainCam;

	private const float _gravity = 9.8f;

	// Make the ball more responsive to changes in gravity
	private const float _scalingFactor = 5.0f;

	public CHHapticEngine HapticEngine;

	private bool _hasTexture = false;
	private bool HasTexture
	{
		get
		{
			return _hasTexture;
		}
		set
		{
			if (!(_ball is null))
			{
				_ball.GetComponent<BallManager>().HasTexture = value;
			}

			_dotsTexture.SetActive(value);

			_hasTexture = value;
			_skipBackgroundText = true;
		}
	}

	[SerializeField] private GameObject _ballPrefab;
	[SerializeField] private GameObject _dotsTexture;

	[SerializeField] private GameObject _startupTextObject;
	private Text _startupText;

	[SerializeField] private GameObject _ballTextObject;
	private Text _ballText;
	private bool _skipBallText = false;

	[SerializeField] private GameObject _backgroundTextObject;
	private Text _backgroundText;
	private bool _skipBackgroundText = false;

	public void Start()
	{
		Application.targetFrameRate = Screen.currentResolution.refreshRate;

		_mainCam = Camera.main;

		SetupHapticEngine();

		_startupText = _startupTextObject.GetComponent<Text>();
		_ballText = _ballTextObject.GetComponent<Text>();
		_backgroundText = _backgroundTextObject.GetComponent<Text>();
	}

	/*
	 * Create a single haptic engine to be shared throughout the app
	 */
	private void SetupHapticEngine()
	{
		HapticEngine = new CHHapticEngine
		{
			IsAutoShutdownEnabled = false
		};

		HapticEngine.Start();
	}

	public void Update()
	{
		CheckTouches();
	}

	public void FixedUpdate()
	{
		UpdateGravity();
	}

	public void OnApplicationPause(bool pause)
	{
		if (pause)
		{
			HapticEngine.Stop();
		}
		else
		{
			HapticEngine.Start();
		}
	}

	private static void UpdateGravity()
	{
		var x = Vector3.Dot(Input.gyro.gravity, Vector3.right);
		var y = Vector3.Dot(Input.gyro.gravity, Vector3.up);
		var gravityVector = _scalingFactor * _gravity * new Vector2(x, y);
		Physics2D.gravity = gravityVector;
	}

	private void CheckTouches()
	{
		if (Input.touchCount != 1) return;

		var t = Input.touches[0];
		if (t.phase == TouchPhase.Began && !(_mainCam is null))
		{
			if (_ball is null)
			{
				Debug.Log("Instantiating new ball.");
				_ball = Instantiate(_ballPrefab, new Vector3(0f, 0.18f, -9.5f), Quaternion.identity);
				_ball.GetComponent<BallManager>().HasTexture = HasTexture;

				if (!_hasSpawnedFirstBall)
				{
					StartCoroutine(InstructionsRoutine(3f));
					_hasSpawnedFirstBall = true;
				}
			}
			else
			{
				var ray = _mainCam.ScreenToWorldPoint(new Vector3(t.position.x, t.position.y, 0f));
				var hitInfo = Physics2D.Raycast(ray, Vector2.zero);
				if (hitInfo)
				{
					if (hitInfo.transform.gameObject == _ball)
					{
						_ball.GetComponent<BallManager>().BallTapped();
						_skipBallText = true;
					}
				}
				else
				{
					HasTexture = !HasTexture;
				}
			}
		}
	}

	public void DestroyBall()
	{
		Debug.Log("GameManager destroying ball");
		Destroy(_ball);
		_ball = null;
	}

	private IEnumerator InstructionsRoutine(float instructionDelayTime)
	{
		StartCoroutine(FadeOutText(1f, _startupText));

		yield return new WaitForSecondsRealtime(instructionDelayTime);

		// Only show this text if the user doesn't figure it out on their own.
		if (!_skipBallText)
		{
			StartCoroutine(FadeInText(1f, _ballText));

			yield return new WaitForSecondsRealtime(instructionDelayTime);
			StartCoroutine(FadeOutText(1f, _ballText));
		}

		yield return new WaitForSecondsRealtime(instructionDelayTime);

		// Only show this text if the user doesn't figure it out on their own.
		if (!_skipBackgroundText)
		{
			StartCoroutine(FadeInText(1f, _backgroundText));

			yield return new WaitForSecondsRealtime(instructionDelayTime);
			StartCoroutine(FadeOutText(1f, _backgroundText));
		}
	}

	private IEnumerator FadeInText(float t, Text i)
	{
		i.color = new Color(i.color.r, i.color.g, i.color.b, 0);
		while (i.color.a < 1.0f)
		{
			i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a + (Time.deltaTime / t));
			yield return null;
		}
	}

	private IEnumerator FadeOutText(float t, Text i)
	{
		i.color = new Color(i.color.r, i.color.g, i.color.b, 1);
		while (i.color.a > 0.0f)
		{
			i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a - (Time.deltaTime / t));
			yield return null;
		}
	}
}
