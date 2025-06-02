using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class CameraFollowObject : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private Transform _bigTransform;
    [SerializeField] private Transform _big1Transform;

    [FormerlySerializedAs("_followOffset")]
    [Header("Follow Settings")]
    [SerializeField] private Vector3 _bigFollowOffset = new Vector3(8f, 0, 0);
    [SerializeField] private float _transitionDuration = 0.5f;

    [Header("Flip Rotation")]
    [SerializeField] private float _flipYRotationTime = 0.5f;

    private Transform _currentTarget;
    private bool _isFacingRight = true;
    private PlayerMovement _player;

    private bool _isTransitioning = false;
    private Coroutine _transitionCoroutine;

    private void Awake()
    {
        _player = _playerTransform.GetComponent<PlayerMovement>();
        _isFacingRight = _player.IsFacingRight;
        _currentTarget = _playerTransform;

        transform.rotation = Quaternion.Euler(0f, _isFacingRight ? 180f : 0f, 0f);
        transform.position = _currentTarget.position;
    }

    private void LateUpdate()
    {
        if (!_isTransitioning && _currentTarget != null)
        {
            transform.position = _currentTarget.position;
            if (_currentTarget == _bigTransform)
            {
                transform.position += _bigFollowOffset;
            }
        }

        if (_bigTransform is not null && _currentTarget == _bigTransform)
        {
            if (Vector3.Distance(_bigTransform.position, _playerTransform.position) < 4)
            {
                SetFollower();
            }
        }
    }

    // one time use in cut scene
    public void SetFollower()
    {
        print("set follower is called");
        Transform newTarget = _currentTarget == _playerTransform ? _bigTransform : _playerTransform;
        _currentTarget = newTarget;
        if (_transitionCoroutine != null)
            StopCoroutine(_transitionCoroutine);

        _transitionCoroutine = StartCoroutine(SmoothFollowTransition(newTarget));
    }

    // one time use in cut scene
    public void EnterNightCutScene()
    {
        _currentTarget = _big1Transform;
    }

    private IEnumerator SmoothFollowTransition(Transform newTarget)
    {
        _isTransitioning = true;
        float elapsed = 0f;
        Vector3 start = transform.position;

        while (elapsed < _transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / _transitionDuration);

            Vector3 dynamicTargetPos = newTarget.position + _bigFollowOffset;
            transform.position = Vector3.Lerp(start, dynamicTargetPos, t);

            yield return null;
        }

        _isTransitioning = false;
    }

    public void CallTurn()
    {
        float targetY = _isFacingRight ? 0f : 180f;
        _isFacingRight = !_isFacingRight;

        LeanTween.cancel(gameObject);
        LeanTween.rotateY(gameObject, targetY, _flipYRotationTime).setEaseInOutSine();
    }
}
