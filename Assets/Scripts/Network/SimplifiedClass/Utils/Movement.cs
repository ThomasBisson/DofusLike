using System;
using UnityEngine;

namespace SerializableClass
{
    [System.Serializable]
    public class MyVector3
    {
        public float x;
        public float y;
        public float z;

        public MyVector3()
        {
            x = 0;
            y = 0;
            z = 0;
        }

        public void Set(Vector3 vec)
        {
            x = vec.x;
            y = vec.y;
            z = vec.z;
        }

        public override string ToString()
        {
            return "[" + x + "," + y + "," + z + "]";
        }

    }

    [System.Serializable]
    public class MyVector2
    {
        public float x;
        public float y;

        public MyVector2()
        {
            x = 0;
            y = 0;
        }

        public void Set(Vector2 vec)
        {
            x = vec.x;
            y = vec.y;
        }

        public override string ToString()
        {
            return "[" + x + "," + y + "]";
        }
    }
}


