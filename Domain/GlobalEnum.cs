using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain
{
    public enum JEDIUM_TYPE_ANIMATOR
    {
        BOOL,
        FLOAT,
        INT,
        TRIGGER
    }

    public enum TYPEACTOR
    {
        DATABASE,
        CONNECTION,
        OBJECTMANAGER,
        RANDOM,
        EMPTY
    }
    
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

        public static int GetTypeIndex(string typename)
        {
            if (_registeredTypes.ContainsValue(typename))
                return _registeredTypes.FirstOrDefault(x => x.Value == typename).Key;

            return -1;
        }

        public static string GetTypeByIndex(int index)
        {
            if (_registeredTypes.ContainsKey(index))
                return _registeredTypes[index];

            return string.Empty;
        }

        public static void AddRegisteredType(string type)
        {
            if (!_registeredTypes.ContainsValue(type))
            {
                _registeredTypes.Add(_registeredTypes.Count, type);
                _additionalBehaviours.Add(_registeredTypes.Count, type);
            }
        }

        //for client-side
        public static void AddRegisteredTypeAndIndex(int index, string type)
        {
            if (_registeredTypes.ContainsKey(index) && !_registeredTypes.ContainsValue(type))
                _registeredTypes.Add(index, type);
        }
    }


    public static class GenerateGuids
    {
        public static Guid GetActorGuid(TYPEACTOR typeactor)
        {
            switch (typeactor)
            {
                case TYPEACTOR.EMPTY:
                    return new Guid("00000000-0000-0000-0000-000000000000");
                    break;
                case TYPEACTOR.CONNECTION:
                    return new Guid("11111111-1111-1111-1111-111111111111");
                    break;
                case TYPEACTOR.OBJECTMANAGER:
                    return new Guid("22222222-2222-2222-2222-222222222222");
                    break;
                case TYPEACTOR.RANDOM:
                    return Guid.NewGuid();
                    break;
                case TYPEACTOR.DATABASE:
                    return new Guid("33333333-3333-3333-3333-333333333333");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(typeactor), typeactor, null);
            }
        }
    }
}