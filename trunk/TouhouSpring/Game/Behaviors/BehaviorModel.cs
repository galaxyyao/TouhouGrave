using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public class BehaviorModel
    {
        private BehaviorModelAttribute m_bhvModelAttr;

        public BehaviorModel()
        {
            m_bhvModelAttr = GetType().GetAttribute<BehaviorModelAttribute>();
        }

        public string GetName()
        {
            return m_bhvModelAttr.Name;
        }

        public Type GetBehaviorType()
        {
            return m_bhvModelAttr.BehaviorType;
        }

        public string GetDescription()
        {
            return m_bhvModelAttr.Description;
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
            var bhv = m_bhvModelAttr.BehaviorType.Assembly.CreateInstance(m_bhvModelAttr.BehaviorType.FullName) as IBehavior;
            bhv.Initialize(this, persistent);
            return bhv;
        }
    }

    public class BehaviorModelAttribute : Attribute
    {
        private string m_description;

        public string Name
        {
            get; private set;
        }

        public string Description
        {
            get { return m_description ?? String.Empty; }
            set { m_description = value; }
        }

        public Type BehaviorType
        {
            get; private set;
        }

        public BehaviorModelAttribute(string name, Type behaviorType)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            else if (name == String.Empty)
            {
                throw new ArgumentException("Name must not be empty.");
            }
            else if (behaviorType == null)
            {
                throw new ArgumentNullException("behaviorType");
            }
            else if (!behaviorType.HasInterface<IBehavior>())
            {
                throw new IncompleteTypeDefinitionException(typeof(IBehavior));
            }

            Name = name;
            BehaviorType = behaviorType;
        }
    }
}
