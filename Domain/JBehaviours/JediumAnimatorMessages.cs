using System;

namespace Domain.BehaviourMessages
{
    public struct JediumAnimatorMessage : JediumBehaviourMessage
    {
        public readonly JEDIUM_TYPE_ANIMATOR Type;
        public readonly string NameParameter;

        public readonly object Value;
        public readonly bool IsDirectUpdate;
        public readonly float DampTime;
        public readonly float DeltaTime;

        private readonly int _behType;

        /// <summary>
        /// </summary>
        /// <param name="type"></param>
        /// <param name="nameParametr"></param>
        /// <param name="value"></param>
        /// <param name="isDirect">Setting the value directly, not tracking animator</param>
        /// <param name="dampTime"></param>
        /// <param name="deltaTime"></param>
        public JediumAnimatorMessage(JEDIUM_TYPE_ANIMATOR type, string nameParametr, object value, bool isDirect,
            float dampTime, float deltaTime)
        {
            Type = type;
            NameParameter = nameParametr;
            Value = value;
            IsDirectUpdate = isDirect;
            DampTime = dampTime;
            DeltaTime = deltaTime;

            _behType = TYPEBEHAVIOUR.GetTypeIndex("Animation");
        }

        public int GetBehaviourType()
        {
            return _behType;
        }
    }

    public class JediumAnimatorSnapshot : JediumBehaviourSnapshot
    {
        public JediumAnimatorSnapshot(Guid localId) : base("Animation", localId)
        {
        }
    }
}