using UnityEngine;

namespace Springy {

    public class Node : IMassPoint {

        public Node(int id, float mass, float randRange = 100f) {
            this.Id = id;

            this.Position = new Vector3(
                Random.Range(-randRange, randRange),
                Random.Range(-randRange, randRange),
                Random.Range(-randRange, randRange)
            );
            this.Velocity = new Vector3();
            this.Acceleration = new Vector3();

            this.Mass = mass;
            this.forcesAccumulator = new Vector3();
        }

        public Vector3 Acceleration { get; set; }
        public int Id { get; private set; }

        float IMassPoint.Mass {
            get { return Mass; }
            set { Mass = value; }
        }

        Vector3 IMassPoint.Position {
            get { return Position; }
            set { Position = value; }
        }

        public float Mass { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Velocity { get; set; }
        private Vector3 forcesAccumulator { get; set; }

        public void AddForce(Vector3 f) {
            forcesAccumulator += f;
        }

        public void ComputeAcceleration() {
            Acceleration = forcesAccumulator / Mass;
            forcesAccumulator = Vector3.zero;
        }
    }
}
