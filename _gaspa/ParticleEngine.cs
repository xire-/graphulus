using System;
using System.Collections.Generic;
using XnaGeometry;

namespace Application
{
    public interface IParticle
    {
        Vector3 position { get; set; }
        Vector3 velocity { get; set; }
        Vector3 force { get; set; }

        // add this???
        double mass { get; set; }
    }

    public class Spring
    {
        public IParticle end1 { get; set; }
        public IParticle end2 { get; set; }
        public double length { get; set; }
        public double k { get; set; }
    }

    public class ParticleEngine
    {
        private List<IParticle> particles = new List<IParticle>();
        private List<Spring> springs = new List<Spring>();

        public double repulsion { get; set; }
        public double friction { get; set; }
        public bool centerGravity { get; set; }

        public ParticleEngine ()
        {
            repulsion = 1000;
            friction = 0.5;
            centerGravity = true;
        }

        public void update(double delta)
        {
            // TODO implementare update
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Vector3 asd = new Vector3();
            Console.WriteLine("asdasd");
            Console.WriteLine(asd);
        }
    }
}
