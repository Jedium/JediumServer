using System;

namespace Server2.ScriptEngine
{
    //WIP
    public interface IJediumScript
    {
        #region Events

        void Touched(Guid clientId, float u, float v);

        #endregion
    }
}