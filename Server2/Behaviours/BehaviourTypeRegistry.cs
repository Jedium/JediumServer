using System;
using System.Collections.Generic;

namespace Server2.Behaviours
{

   public static  class BehaviourTypeRegistry
   {

       public static Dictionary<string, Type> BehaviourTypes = new Dictionary<string, Type>()
       {
           {"Transform", typeof(JediumTransform)},
           {"Touch", typeof(JediumTouchable)},
           {"Animation", typeof(JediumAnimator)},
           {"CharacterController", typeof(JediumCharacterController)},
           {"Take", typeof(JediumTakeable) },
           {"Sit", typeof(JediumSiteable) }
       };

       public static List<Type> DBTypes = new List<Type>()
       {
           typeof(JediumTransformDBSnapshot),
           typeof(JediumAnimatorDBSnapshot),
           typeof(JediumTouchableDBSnapshot),
           typeof(JediumCharacterControllerDBSnapshot),
           typeof(JediumTakeableDBSnapshot),
           typeof(JediumSiteableDBSnapshot)
       };
   }
}









