/// <summary>
/// パーティクル回転軸制御
/// 
/// 2013/08/28
/// </summary>
using UnityEngine;
using System.Collections;

public class AxisParticle : MonoBehaviour
{
	public Vector3 axis = Vector3.up;
	private ParticleSystem _particleSystem;
	private ParticleSystem ParticleSystem
	{
		get
		{
			if(_particleSystem == null)
			{
				_particleSystem = this.GetComponent<ParticleSystem>();
			}
			return _particleSystem;
		}
	}

	void Start()
	{
		this.SetAxis();
	}
	void LateUpdate()
	{
		this.SetAxis();
	}
	void Update()
	{
		this.SetAxis();
	}

	void SetAxis()
	{
#if !UNITY_3
		if (this.ParticleSystem == null)
			{ return; }
		int particleCount = this.ParticleSystem.particleCount;
		if (particleCount <= 0)
			{ return; }

		ParticleSystem.Particle[] particles = new ParticleSystem.Particle[particleCount];
		int count = this.ParticleSystem.GetParticles(particles);
		for (int i=0; i<count; i++)
		{
			particles[i].axisOfRotation = this.axis;
		}
		this.ParticleSystem.SetParticles(particles, count);
#endif
	}
}

