using Interfaces;
using Player;
using UnityEngine;

public class BreakableGuilutineBlade : MonoBehaviour,IBreakable,IResettable
{
    [SerializeField] private Explodable e;
    [SerializeField] private ExplosionForce f;

    private Vector3 startingPosition;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startingPosition = transform.position;
    }

    public void OnBreak()
    {
        e.explode();
        f.doExplosion(f.transform.position);
    }

    public void OnHit(Vector2 hitDir, PlayerManager.PlayerStage stage)
    {
        OnBreak();
    }

    public void ResetToInitialState()
    {
        transform.position = startingPosition;
    }
}
