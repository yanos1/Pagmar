using System.Collections;
using DG.Tweening;
using UnityEngine;

public class CameraFollowObject : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _playerTransform;

    [Header("Flip Rotation Stats")]
    [SerializeField] private float _flipYRotationTime = 0.5f;

    private Coroutine _turnCoroutine;
    private PlayerMovement _player;
    private bool _isFacingRight;

    private void Awake()
    {
        _player = _playerTransform.gameObject.GetComponent<PlayerMovement>();
        _isFacingRight = !_player.IsFacingRight;
    }

    private void FixedUpdate()
    {
        transform.position = _playerTransform.position;
    }

    public void CallTurn()
    {
        if (_turnCoroutine != null)
        {
            StopCoroutine(_turnCoroutine);
        }
        LeanTween.rotateY(gameObject, DetermineEndRotation(), _flipYRotationTime).setEaseInOutSine();
              
    }

    private float DetermineEndRotation()
    {
        _isFacingRight = !_isFacingRight;

        return _isFacingRight ? 180f : 0f;
    }
}