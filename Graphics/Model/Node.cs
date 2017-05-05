using System.Collections.Generic;

namespace engenious.Graphics
{
    public class Node
    {
        public Node()
        {
            GlobalTransform = AnimationTransform.Identity;
            LocalTransform = Matrix.Identity;
        }

        public string Name{ get; set; }

        public List<Mesh> Meshes{ get; set; }

        public Matrix Transformation{ get; set; }

        public Matrix LocalTransform{ get; set; }

        public AnimationTransform GlobalTransform{ get; set; }

        public Node Parent{get;set;}
        public List<Node> Children{ get; set; }

        public bool IsTransformed{get;set;}
    }
}

