using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public class BehaviorModel<T> : IInternalBehaviorModel
        where T : class, IBehavior, new()
    {
        private static Func<IBehavior> s_factory;

        private BehaviorModelAttribute m_bhvModelAttr;
        private string m_name;

        [System.ComponentModel.Category("Basic")]
        public string Name
        {
            get { return m_name ?? m_bhvModelAttr.DefaultName ?? ModelTypeName; }
            set { m_name = String.IsNullOrEmpty(value) ? null : value; }
        }

        [System.ComponentModel.Category("Basic")]
        public string ModelTypeName
        {
            get { return typeof(T).FullName; }
        }

        public BehaviorModel()
        {
            m_bhvModelAttr = GetType().GetAttribute<BehaviorModelAttribute>();
            m_name = null;
        }

        public IBehavior CreateInitialized()
        {
            return CreateInitialized(false);
        }

        IBehavior IInternalBehaviorModel.Instantiate()
        {
            return s_factory();
        }

        IBehavior IInternalBehaviorModel.CreateInitializedPersistent()
        {
            return CreateInitialized(true);
        }

        private IBehavior CreateInitialized(bool persistent)
        {
            var bhv = (this as IInternalBehaviorModel).Instantiate();
            (bhv as IInternalBehavior).Initialize(this, persistent);
            return bhv;
        }

        static BehaviorModel()
        {
            var bhvType = typeof(T);
            var dynMethod = new DynamicMethod("DM$OBJ_FACTORY_" + bhvType.FullName, bhvType, null, bhvType);
            var ilGen = dynMethod.GetILGenerator();
            ilGen.Emit(OpCodes.Newobj, bhvType.GetConstructor(Type.EmptyTypes));
            ilGen.Emit(OpCodes.Ret);
            s_factory = (Func<T>)dynMethod.CreateDelegate(typeof(Func<T>));
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class BehaviorModelAttribute : Attribute
    {
        private string m_defaultName;
        private string m_description;
        private string m_category;

        public string DefaultName
        {
            get { return m_defaultName; }
            set { m_defaultName = value; }
        }

        public string Description
        {
            get { return m_description ?? String.Empty; }
            set { m_description = value; }
        }

        public string Category
        {
            get { return m_category ?? String.Empty; }
            set { m_category = value; }
        }
    }
}
