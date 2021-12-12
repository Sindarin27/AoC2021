using System;
using System.Collections.Generic;

namespace Day12
{
    public class Node
    {
        public string name;
        public List<Node> connections;
        public bool isBig;
        
        public Node(string name)
        {
            this.name = name;
            connections = new List<Node>();
            isBig = char.IsUpper(name[0]);
        }
        public void AddConnection(Node other)
        {
            this.connections.Add(other);
            other.connections.Add(this);
        }
    }
}