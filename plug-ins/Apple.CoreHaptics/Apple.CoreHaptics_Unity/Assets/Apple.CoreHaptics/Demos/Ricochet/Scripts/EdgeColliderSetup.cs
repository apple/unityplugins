using UnityEngine;

public class EdgeColliderSetup : MonoBehaviour
{
	private Camera _camera;

	private GameObject _topCollider;
	private GameObject _bottomCollider;
	private GameObject _leftCollider;
	private GameObject _rightCollider;

	public void Start()
	{
		_camera = GameObject.Find("MainCamera").GetComponent<Camera>();
		if (_camera is null)
		{
			Debug.LogError("Could not find main camera.");
		}

		AdjustScreenSize();

		SetupColliders();

		Screen.orientation = ScreenOrientation.Portrait;

		Application.targetFrameRate = Screen.currentResolution.refreshRate;
	}

	public void Update()
	{
		AdjustScreenSize();
		SetColliderPositions();
	}

	private void AdjustScreenSize()
	{
		// Adjust the camera zoom to the screen size so the UI looks consistent across resolutions
		if (Screen.width > 0 && Screen.height > 0)
		{
			const float sceneWidth = 10f;
			var unitsPerPixel = sceneWidth / Screen.width;
			var desiredHalfHeight = 0.5f * unitsPerPixel * Screen.height;
			_camera.orthographicSize = desiredHalfHeight;
		}
	}

	private void SetupColliders()
	{
		// Create box colliders at the edges of the screen for the ball to bounce off
		_topCollider = new GameObject("Top");
		_bottomCollider = new GameObject("Bottom");
		_leftCollider = new GameObject("Left");
		_rightCollider = new GameObject("Right");

		var colliders = new[] { _topCollider, _bottomCollider, _leftCollider, _rightCollider };
		foreach (var c in colliders)
		{
			c.AddComponent<BoxCollider2D>();
			c.transform.parent = transform;
		}

		SetColliderPositions();
	}

	private void SetColliderPositions()
	{
		Vector2 screenSize;
		screenSize.x = Vector2.Distance(_camera.ScreenToWorldPoint(new Vector2(0, 0)), _camera.ScreenToWorldPoint(new Vector2(Screen.width, 0))) * 0.5f;
		screenSize.y = Vector2.Distance(_camera.ScreenToWorldPoint(new Vector2(0, 0)), _camera.ScreenToWorldPoint(new Vector2(0, Screen.height))) * 0.5f;

		var cameraPos = _camera.transform.position;

		const float colDepth = 4f;
		const float zPosition = 0f;

		_rightCollider.transform.localScale = new Vector3(colDepth, screenSize.y * 2, colDepth);
		_rightCollider.transform.position = new Vector3(cameraPos.x + screenSize.x + _rightCollider.transform.localScale.x * 0.5f, cameraPos.y, zPosition);

		_leftCollider.transform.localScale = new Vector3(colDepth, screenSize.y * 2, colDepth);
		_leftCollider.transform.position = new Vector3(cameraPos.x - screenSize.x - _leftCollider.transform.localScale.x * 0.5f, cameraPos.y, zPosition);

		_topCollider.transform.localScale = new Vector3(screenSize.x * 2, colDepth, colDepth);
		_topCollider.transform.position = new Vector3(cameraPos.x, cameraPos.y + screenSize.y + _topCollider.transform.localScale.y * 0.5f, zPosition);

		_bottomCollider.transform.localScale = new Vector3(screenSize.x * 2, colDepth, colDepth);
		_bottomCollider.transform.position = new Vector3(cameraPos.x, cameraPos.y - screenSize.y - _bottomCollider.transform.localScale.y * 0.5f, zPosition);
	}
}
