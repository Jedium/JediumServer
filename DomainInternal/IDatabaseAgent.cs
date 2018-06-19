using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Akka.Interfaced;
using DomainInternal.Behaviours;
using MongoDB.Bson.Serialization.Attributes;

namespace DomainInternal
{
    public interface IDatabaseAgent : IInterfacedActor
    {

        #region Avatar
        Task<DatabaseAvatar> GetAvatar(Guid avatarId);
        Task CreateAvatar(DatabaseAvatar avatar);
        #endregion

        #region Bundle
        Task<DatabaseAssetBundle> GetAssetBundle(Guid bundleId);
        Task<List<DatabaseAssetBundle>> GetAllBundles();
        Task SaveAssetBundle(DatabaseAssetBundle bundle);
        #endregion

        #region Behaviour
        Task StoreDBBehaviour(JediumBehaviourDBSnapshot snap);
        Task<Dictionary<string, JediumBehaviourDBSnapshot>> GetObjectBehaviours(Guid localId);
        #endregion

        #region ServerObject
        Task<DatabaseObject> GetObjectServer(Guid objectId);
        Task CreateObjectServer(DatabaseObject dobject);
        #endregion

        #region SceneObject
        Task<List<DatabaseSceneObject>> GetObjectsScene(Guid sceneId);
        Task AddSceneObject(DatabaseSceneObject sceneObj);
        #endregion

        #region Scene
        Task<List<DatabaseScene>> GetScenes();
        Task CreateScene(DatabaseScene scene);
        #endregion

        #region User
        Task<DatabaseUser> GetUserByName(string username);
        Task CreateUser(DatabaseUser user);
        Task<List<DatabaseUser>> GetUsers();
        Task UpdateAvatarProps(Guid userId, string props);
        #endregion



        #region Debug

        Task SetDummyObjectTest();


        #endregion








    }

    public class DatabaseSceneObject
    {
        public Guid LocalId;
        public Guid ObjectId;

        public Guid SceneId;

        public override string ToString()
        {
            return
                $"\n****************\n" +
                $"LocalID - {LocalId},\n" +
                $"ObjectID - {ObjectId},\n" +
                $"ScenceID - {ObjectId},\n" +
                $"****************\n";
        }
    }

    //TODO - this all should be mapped to Mongo
    public class DatabaseObject
    {
        public Guid BundleId;
        public string Name;
        public Guid ObjectId;
        public string Prefab;

        public override string ToString()
        {
            return
                $"\n****************\n" +
                $"LocalID - {ObjectId},\n" +
                $"ObjectID - {Name},\n" +
                $"ScenceID - {Prefab},\n" +
                $"ScenceID - {BundleId},\n" +
                $"****************\n";
        }
    }

    public class DatabaseScene
    {
        public Guid BundleId;
        public Guid LocalId;
        public string SceneName;
        public string ServerName;
    }


    public class DatabaseAssetBundle
    {
        public Guid BundleId { get; set; }
        public string BundleFile { get; set; }
        public string Hash { get; set; }

        public override string ToString()
        {
            return $"BundleId: {BundleId}, file: {BundleFile}";
        }
    }

    public class DatabaseUser
    {
        public Guid AvatarId;
        public string AvatarProps;
        public string Password;
        public Guid UserId;
        public string Username;
    }

    public class DatabaseAvatar
    {
        public Guid AvatarId;
        public string DefaultProps;
        public string OtherAvatar;
        public string UserAvatar;
    }
}