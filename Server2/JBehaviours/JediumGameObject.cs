using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Event;


namespace JBehaviours
{

    

   public class JediumGameObject
   {
       //private JediumAnimator _animator;
       //private JediumTransform _transform;
       //private JediumTouchable _touchable;
       private Dictionary<TYPEBEHAVIOUR,JediumBehaviour> _behaviours;
       public IGameObject Actor;

       //Мы предполагаем, что компонент Transform есть всегда
        public JediumGameObject(IGameObject actor,JediumTransformSnapshot transform,List<JediumBehaviourSnapshot> behaviours)
        {
            Actor = actor;

            _behaviours=new Dictionary<TYPEBEHAVIOUR, JediumBehaviour>();
          //  this._transform = new JediumTransform(this,transform);



            _behaviours.Add(TYPEBEHAVIOUR.TRANSFORM,new JediumTransform(this,transform));

           // _animator = null;
           // _touchable = null;

            foreach (var beh in behaviours)
            {
                if (beh is JediumToucheableSnapshot)
                {
                   JediumToucheableSnapshot s_touchable = beh as JediumToucheableSnapshot;
                   JediumTouchable _touchable=new JediumTouchable(this);
                    _touchable.FromSnapshot(s_touchable);
                    _behaviours.Add(TYPEBEHAVIOUR.TOUCH,_touchable);
                }

                if (beh is JediumAnimatorSnapshot)
                {
                    JediumAnimatorSnapshot s_animator = beh as JediumAnimatorSnapshot;
                    JediumAnimator _animator=new JediumAnimator(this);
                    _animator.FromSnapshot(s_animator);

                    _behaviours.Add(TYPEBEHAVIOUR.ANIMATION,_animator);
                }
            }
        }

       public void ProcessComponentMessage(Guid clientId, JediumBehaviourMessage message)
       {
           foreach (var comp in _behaviours)
           {
               comp.Value.ProcessMessage(clientId,message);
           }
       }

       public ObjectSnapshot GetSnapshot()
       {
           ObjectSnapshot ret=new ObjectSnapshot();

           if (_behaviours.ContainsKey(TYPEBEHAVIOUR.ANIMATION))
           {
               ret.HasAnimator = true;
             
           }
           else
           {
               ret.HasAnimator = false;
           }

           if (_behaviours.ContainsKey(TYPEBEHAVIOUR.TOUCH))
           {
               ret.HasTouchable = true;
           }
           else
           {
               ret.HasTouchable = false;
           }

           JediumTransform trans = (JediumTransform) _behaviours[TYPEBEHAVIOUR.TRANSFORM];
           //transform
           ret.X = trans._posX;
           ret.Y = trans._posY;
           ret.Z = trans._posZ;
           ret.RotX = trans._quatX;
           ret.RotY = trans._quatY;
           ret.RotZ = trans._quatZ;
           ret.RotW = trans._quatW;
           ret.ScaleX = trans._scaleX;
           ret.ScaleY = trans._scaleY;
           ret.ScaleZ = trans._scaleZ;
            //

            return ret;
       }
    }

   
}
