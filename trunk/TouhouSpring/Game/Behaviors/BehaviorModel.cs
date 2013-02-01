using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public class BehaviorModel
    {
        private BehaviorModelAttribute m_bhvModelAttr;
        private string m_name;

        [System.ComponentModel.Category("Basic")]
        public string Name
        {
            get { return m_name ?? m_bhvModelAttr.DefaultName; }
            set { m_name = String.IsNullOrEmpty(value) ? null : value; }
        }

        [System.ComponentModel.Category("Basic")]
        public string ModelTypeName
        {
            get { return GetBehaviorType().FullName; }
        }

        public BehaviorModel()
        {
            m_bhvModelAttr = GetType().GetAttribute<BehaviorModelAttribute>();
            m_name = null;
        }

        public virtual Type GetBehaviorType()
        {
            return m_bhvModelAttr.BehaviorType;
        }

        public IBehavior Instantiate()
        {
            return Instantiate(false);
        }

        internal IBehavior InstantiatePersistent()
        {
            return Instantiate(true);
        }

        private IBehavior Instantiate(bool persistent)
        {
            var bhvType = GetBehaviorType();
            var bhv = bhvType.Assembly.CreateInstance(bhvType.FullName) as IBehavior;
            (bhv as IInternalBehavior).Initialize(this, persistent);
            return bhv;
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
            get { return m_defaultName ?? BehaviorType.Name; }
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

        public Type BehaviorType
        {
            get; private set;
        }

        public BehaviorModelAttribute(Type behaviorType)
        {
            if (behaviorType == null)
            {
                throw new ArgumentNullException("behaviorType");
            }
            else if (!behaviorType.HasInterface<IBehavior>())
            {
                throw new IncompleteTypeDefinitionException(typeof(IBehavior));
            }

            BehaviorType = behaviorType;
        }
    }
}
