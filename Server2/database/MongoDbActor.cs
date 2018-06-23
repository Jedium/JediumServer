using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Akka.Interfaced;
using Common.Logging;
using Domain;
using DomainInternal;
using DomainInternal.Behaviours;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using Server2.Behaviours;

namespace Server2.database
{
    public class MongoDbActor :InterfacedActor, IDatabaseAgent
    {
        private static bool _typesRegistered;
        private readonly ILog _logger;

        private readonly MongoClient client;
        private readonly IMongoDatabase database;

        public MongoDbActor()
        {
            _logger = LogManager.GetLogger("[MongoDB]");
            //  client = new MongoClient("mongodb://localhost:27017");

            client = new MongoClient(MainSettings.DBUrl);
            database = client.GetDatabase(MainSettings.DatabaseName);
            _logger.Info($"DB is created, address {MainSettings.DBUrl}, db {MainSettings.DatabaseName}");

            if (!_typesRegistered)
            {
                var pack = new ConventionPack();
                pack.Add(new IgnoreExtraElementsConvention(true));
                ;
                ConventionRegistry.Register("JediumConventions", pack, t => true);

                BsonClassMap.RegisterClassMap<JediumBehaviourDBSnapshot>(cm =>
                {
                    cm.AutoMap();
                    cm.SetIsRootClass(true);
                    cm.SetIgnoreExtraElements(true);
                });

                foreach (var cls in BehaviourTypeRegistry.DBTypes) BsonClassMap.LookupClassMap(cls);

                // AddSceneObject();
                _typesRegistered = true;
            }
        }

        #region Avatar

        Task<DatabaseAvatar> IDatabaseAgent.GetAvatar(Guid avatarId)
        {
            var collection = database.GetCollection<DatabaseAvatar>("avatars");
            var filter = Builders<DatabaseAvatar>.Filter.Eq("AvatarId", avatarId);

            var document = collection.Find(filter).FirstOrDefault();

            return  Task.FromResult(document);
        }

        Task IDatabaseAgent.CreateAvatar(DatabaseAvatar avatar)
        {
            var collection = database.GetCollection<DatabaseAvatar>("avatars");
            collection.InsertOne(avatar);
            return Task.FromResult(true);
        }

        #endregion


        #region Bundle
        async Task<DatabaseAssetBundle> IDatabaseAgent.GetAssetBundle(Guid bundleId)
        {
            var collection = database.GetCollection<DatabaseAssetBundle>("bundles");

            var filter = Builders<DatabaseAssetBundle>.Filter.Eq("BundleId", bundleId);

            var document = collection.Find(filter).ToList();

            if (document.Count > 0)
                return document[0];

            return null;
        }

         Task<List<DatabaseAssetBundle>> IDatabaseAgent.GetAllBundles()
        {
            var collection = database.GetCollection<DatabaseAssetBundle>("bundles");

            var bundles = collection.Find(_ => true).ToList();
            return Task.FromResult(bundles);
        }

         Task IDatabaseAgent.SaveAssetBundle(DatabaseAssetBundle bundle)
        {
            var collection = database.GetCollection<DatabaseAssetBundle>("bundles");
            collection.InsertOne(bundle);
            return Task.FromResult(true);
        }
        #endregion


        #region Behaviour
         Task IDatabaseAgent.StoreDBBehaviour(JediumBehaviourDBSnapshot snap)
        {
            if (snap.SaveOnShutdown)
            {
            var filter = Builders<JediumBehaviourDBSnapshot>.Filter.And(
                Builders<JediumBehaviourDBSnapshot>.Filter.Eq("LocalId", snap.LocalId),
                Builders<JediumBehaviourDBSnapshot>.Filter.Eq("Type", snap.Type));

               
                var collection = database.GetCollection<JediumBehaviourDBSnapshot>("newbehaviours");

                var document = collection.Find(filter).FirstOrDefault();

                if (document != null)
                    collection.ReplaceOne(filter, snap);
                else
                    collection.InsertOne(snap);
               
            }

            return Task.FromResult(true);
        }

         Task<Dictionary<string, JediumBehaviourDBSnapshot>> IDatabaseAgent.GetObjectBehaviours(Guid localId)
        {
            var filter = Builders<JediumBehaviourDBSnapshot>.Filter.Eq("LocalId", localId);

            var collection = database.GetCollection<JediumBehaviourDBSnapshot>("newbehaviours");

            var documents = collection.Find(filter).ToList();


            Dictionary<string, JediumBehaviourDBSnapshot> ret = new Dictionary<string, JediumBehaviourDBSnapshot>();

            foreach (var d in documents) ret.Add(d.Type, d);

            return Task.FromResult(ret);
        }
        #endregion

        #region ServerObject
         Task<DatabaseObject> IDatabaseAgent.GetObjectServer(Guid objectId)
        {
            var filter = Builders<DatabaseObject>.Filter.Eq("ObjectId", objectId.ToString());
            var collection = database.GetCollection<DatabaseObject>("objects");

            var document = collection.Find(filter).FirstOrDefault();

            return Task.FromResult(document);
        }

       Task IDatabaseAgent.CreateObjectServer(DatabaseObject dobject)
        {
            var collection = database.GetCollection<DatabaseObject>("objects");
            collection.InsertOne(dobject);

            return Task.FromResult(true);
        }

        #endregion


        #region SceneObject
        Task<List<DatabaseSceneObject>> IDatabaseAgent.GetObjectsScene(Guid sceneId)
        {
            var filter = Builders<DatabaseSceneObject>.Filter.Eq("SceneID", sceneId);
            var collection = database.GetCollection<DatabaseSceneObject>("sceneObjects");

            var document = collection.Find(filter).ToList();
            
            return Task.FromResult(document);
        }

        Task IDatabaseAgent.AddSceneObject(DatabaseSceneObject sceneObj)
        {
            var collection = database.GetCollection<DatabaseSceneObject>("sceneObjects");
            collection.InsertOne(sceneObj);

            return Task.FromResult(true);
        }

        #endregion


        #region Scene
         Task<List<DatabaseScene>> IDatabaseAgent.GetScenes()
        {
            var collection = database.GetCollection<DatabaseScene>("scenes");
            var document = collection.Find(new BsonDocument()).ToList();

            return Task.FromResult(document);
        }

         Task IDatabaseAgent.CreateScene(DatabaseScene scene)
        {
            var collection = database.GetCollection<DatabaseScene>("scenes");
            collection.InsertOne(scene);
            return Task.FromResult(true);
        }
        #endregion


        #region User
         Task<DatabaseUser> IDatabaseAgent.GetUserByName(string username)
        {
            var collection = database.GetCollection<DatabaseUser>("users");
            var filter = Builders<DatabaseUser>.Filter.Eq("Username", username);

            var document = collection.Find(filter).FirstOrDefault();

            return Task.FromResult(document);
        }

        Task IDatabaseAgent.CreateUser(DatabaseUser user)
        {
            var collection = database.GetCollection<DatabaseUser>("users");
            collection.InsertOne(user);
            return Task.FromResult(true);
        }

         Task<List<DatabaseUser>> IDatabaseAgent.GetUsers()
        {
            var collection = database.GetCollection<DatabaseUser>("users");

            var document = collection.Find(new BsonDocument()).ToList();

            return Task.FromResult(document);
        }



        Task IDatabaseAgent.UpdateAvatarProps(Guid userId, string props)
        {
            var collection = database.GetCollection<DatabaseUser>("users");
            var filter = Builders<DatabaseUser>.Filter.Eq("UserId", userId);

            var update = Builders<DatabaseUser>.Update.Set("AvatarProps", props);
            collection.UpdateOne(filter, update);

            return Task.FromResult(true);
        }
        #endregion











        

        #region Debug

        void InsertTestBundle()
        {
            var collection = database.GetCollection<DatabaseAssetBundle>("bundles");

            DatabaseAssetBundle test_bundle = new DatabaseAssetBundle
            {
                BundleId = Guid.Parse("bc3b6160-081e-474d-8503-df794046bba7"),
                BundleFile = "testbundle"
            };
            collection.InsertOne(test_bundle);
        }

        void AddSceneObject()
        {
            DatabaseSceneObject tst = new DatabaseSceneObject
            {
                LocalId = Guid.Empty,
                ObjectId = Guid.Empty,
                SceneId = Guid.Empty
            };

            var collection = database.GetCollection<DatabaseSceneObject>("sceneObjects");

            collection.InsertOne(tst);
        }


        Task IDatabaseAgent.SetDummyObjectTest()
        {
            _logger.Warn("_________SETTING DUMMY OBJECT");
            // JediumTransformSnapshot tr = new JediumTransformSnapshot(Guid.Parse("67ed8767-b105-4c82-bb5c-f5cc59664101"),1,-1,1,
            //     0,0,0,1,
            //     1,1,1);
            //
            // tr.X = 1;
            // tr.Y = -1;
            // tr.Z = 1;
            //
            // var trbox = ObjectBox.Serialize(new ObjectBox(tr));
            //
            //
            // var filter = Builders<BsonDocument>.Filter.Eq("localId", "67ed8767-b105-4c82-bb5c-f5cc59664101");
            // var update = Builders<BsonDocument>.Update.Set("transformObj", trbox);
            //
            // var collection = database.GetCollection<BsonDocument>("sceneObjects");
            //
            // await collection.UpdateOneAsync(filter, update);

            return Task.FromResult(true);
        }

        #endregion
    }
}