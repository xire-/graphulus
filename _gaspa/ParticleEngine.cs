using System;
using System.Collections.Generic;

namespace Application
{
	public interface IParticle
	{
		Vector3 position { get; set; }
		Vector3 velocity { get; set; }
		Vector3 force { get; set; }
	}

	public class ParticleEngine
	{
		private List<IParticle> particles = new List<IParticle>();
		private List<Spring> springs = new List<Spring>();

		public ParticleEngine () { }

		public void update(double delta)
		{
			// TODO implementare update 
		}
	}
}
