using UnityEngine;
using System.Collections;

public class ExplodeOnClick : MonoBehaviour {

	private Explodable _explodable;
	public ExplosionForce f;

	void Start()
	{
		_explodable = GetComponent<Explodable>();
	}
	void OnMouseDown()
	{
		_explodable.explode();
		f.doExplosion(f.transform.position);
	}
}
