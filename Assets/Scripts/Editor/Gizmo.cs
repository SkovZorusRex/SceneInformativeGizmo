using System;
using UnityEngine;

namespace technical.test.editor
{
    [Serializable]
    public struct Gizmo
    {
        public string Name;   
        public Vector3 Position;

        public Gizmo(string name, Vector3 position)
        {
            Name = name;
            Position = position;
        }
    }

}