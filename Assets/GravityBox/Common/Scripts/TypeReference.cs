using System;

namespace GravityBox
{
    /// <summary>
    /// Class to use in Unity inspector if you need assign class variable.
    /// Works better with custom drawer. Assembly and fullname variables 
    /// are for human readability purposes.
    /// </summary>
    [System.Serializable]
    public class TypeReference
    {
        public string Assembly;
        public string Fullname;
        public string assemblyQualifiedName;

        private Type _type;
        public Type Type
        {
            get
            {
                _type = LoadType();
                return _type;
            }
            set
            {
                Assembly = value.Assembly.FullName;
                Fullname = value.FullName;
                assemblyQualifiedName = value.AssemblyQualifiedName;
                _type = value;
            }
        }

        private Type LoadType()
        {
            return Type.GetType(assemblyQualifiedName);
        }
    }
}