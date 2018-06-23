using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain
{
    /// <summary>
    /// Internal enum for animator variables
    /// </summary>
    public enum JEDIUM_TYPE_ANIMATOR
    {
        BOOL,
        FLOAT,
        INT,
        TRIGGER
    }

    
    /// <summary>
    /// An extendable class which contains the type map for all object behaviours (both built-in and loaded from plugins)
    /// </summary>
    public static class TYPEBEHAVIOUR
    {
        private static readonly Dictionary<int, string> _registeredTypes = new Dictionary<int, string>
        {
            {0,"Transform" },
            {1,"Touch" },
            {2,"Animation" },
            {3,"CharacterController" },
            {4, "Take" },
            {5,"Sit" }
        };


        private static readonly Dictionary<int, string> _additionalBehaviours = new Dictionary<int, string>();

        public static Dictionary<int, string> AdditionalBehaviours => _additionalBehaviours;

        /// <summary>
        /// Gets behaviour type index by type name
        /// </summary>
        /// <param name="typename">Name of the behaviour type</param>
        /// <returns></returns>
        public static int GetTypeIndex(string typename)
        {
            if (_registeredTypes.ContainsValue(typename))
                return _registeredTypes.FirstOrDefault(x => x.Value == typename).Key;

            return -1;
        }

        /// <summary>
        /// Gets behaviour type by type index
        /// </summary>
        /// <param name="index">Type index</param>
        /// <returns></returns>
        public static string GetTypeByIndex(int index)
        {
            if (_registeredTypes.ContainsKey(index))
                return _registeredTypes[index];

            return string.Empty;
        }

        /// <summary>
        /// Adds a new type to registry
        /// </summary>
        /// <param name="type">Type name</param>
        public static void AddRegisteredType(string type)
        {
            if (!_registeredTypes.ContainsValue(type))
            {
                _registeredTypes.Add(_registeredTypes.Count, type);
                _additionalBehaviours.Add(_registeredTypes.Count, type);
            }
        }

        //for client-side
        /// <summary>
        /// Adds behaviour type with specific index. Client-side only.
        /// </summary>
        /// <param name="index">Type index</param>
        /// <param name="type">Type name</param>
        public static void AddRegisteredTypeAndIndex(int index, string type)
        {
            if (_registeredTypes.ContainsKey(index) && !_registeredTypes.ContainsValue(type))
                _registeredTypes.Add(index, type);
        }
    }

    
}