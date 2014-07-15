using System;
using System.Collections.Generic;
using System.IO;
using VVVV.Utils.VColor;
using VVVV.Utils.VMath;
using VVVV.Packs.Time;


namespace VVVV.Packs.Message.Core
{
    public class TypeIdentity : Dictionary<Type, string>
    {
        private static TypeIdentity _instance;
        public static TypeIdentity Instance
        {
            get { 
                if (_instance == null) _instance = new TypeIdentity();
                return _instance;
            }
            private set { throw new NotImplementedException(); }
        }

        public TypeIdentity()
	    {
            // This is the only place where you need to add new datatypes.

            Add(typeof(bool), "bool".ToLower());
            Add(typeof(int), "int".ToLower());
            Add(typeof(double), "double".ToLower());
            Add(typeof(float), "float".ToLower());
            Add(typeof(string), "string".ToLower());
//            Add(typeof(char), "char".ToLower());

            Add(typeof(RGBAColor), "Color".ToLower());
            Add(typeof(Matrix4x4), "Transform".ToLower());
            Add(typeof(Vector2D), "Vector2D".ToLower());
            Add(typeof(Vector3D), "Vector3D".ToLower());
            Add(typeof(Vector4D), "Vector4D".ToLower());

            Add(typeof(Stream), "Raw".ToLower());
            Add(typeof(Time.Time), "Time".ToLower());
            
            Add(typeof(Message), "Message".ToLower());	        
	    }

        public static string FindAlias(Type t)
        {
            foreach (Type key in Instance.Keys)
            {
                if (key == t) return Instance[key];
            }
            return null;
        }

        public static string FindBaseAlias(Type t)
        {
            foreach (Type key in Instance.Keys)
            {
                if (key == t) return Instance[key];
                if (t.IsSubclassOf(key)) return Instance[key];
            }
            return null;
        }

        public static Type FindType(string alias)
        {
            Type type = typeof(string);
            foreach (Type key in Instance.Keys)
            {
                if (Instance[key] == alias)
                {
                    type = key;
                }
            }
            return type;
        }
    }
}
