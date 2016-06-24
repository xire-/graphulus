namespace Springy {

    public class Edge {

        public Edge(int id, Node source, Node target, float length) {
            this.Id = id;
            this.Source = source;
            this.Target = target;
            this.Length = length;
        }

        public int Id { get; private set; }
        public float Length { get; set; }
        public Node Source { get; private set; }
        public Node Target { get; private set; }
    }
}
