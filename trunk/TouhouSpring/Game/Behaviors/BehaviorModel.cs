using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public class BehaviorModel : IInternalBehaviorModel
    {
        private class BehaviorModelData
        {
            public Type BehaviorType;
            public Func<IBehavior> BhvFactory;
            public BehaviorModelAttribute BhvModelAttr;
        }

        private static Dictionary<Type, BehaviorModelData> s_data = new Dictionary<Type, BehaviorModelData>();

        private string m_name;
        private BehaviorModelData m_data;

        [System.ComponentModel.Category("Basic")]
        public string Name
        {
            get { return m_name ?? (m_data.BhvModelAttr != null ? m_data.BhvModelAttr.DefaultName : SupplementBehaviorType.Name); }
            set { m_name = String.IsNullOrEmpty(value) ? null : value; }
        }

        [System.ComponentModel.Category("Basic")]
        public string BehaviorTypeName
        {
            get { return (m_data.BhvModelAttr != null ? m_data.BhvModelAttr.BehaviorType : SupplementBehaviorType).FullName; }
        }

        // This type only affects if BehaviorModel is not provided
        [System.ComponentModel.Browsable(false)]
        protected virtual Type SupplementBehaviorType
        {
            get
            {
                throw new InvalidOperationException("Shall not be evaluated if BehaviorModelAttribute is provided.");
            }
        }

        public BehaviorModel()
        {
            var modelType = GetType();
            if (!s_data.TryGetValue(modelType, out m_data))
            {
                lock (s_data)
                {
                    if (!s_data.TryGetValue(modelType, out m_data))
                    {
                        m_data = new BehaviorModelData
                        {
                            BhvModelAttr = modelType.GetAttribute<BehaviorModelAttribute>()
                        };
                        var bhvType = m_data.BhvModelAttr != null
                                      ? m_data.BhvModelAttr.BehaviorType
                                      : SupplementBehaviorType; // virtual function call to get the most derived value

                        var dynMethod = new DynamicMethod("DM$OBJ_FACTORY_" + bhvType.FullName, bhvType, null, bhvType);
                        var ilGen = dynMethod.GetILGenerator();
                        ilGen.Emit(OpCodes.Newobj, bhvType.GetConstructor(Type.EmptyTypes));
                        ilGen.Emit(OpCodes.Ret);

                        m_data.BehaviorType = bhvType;
                        m_data.BhvFactory = (Func<IBehavior>)dynMethod.CreateDelegate(typeof(Func<IBehavior>));

                        s_data.Add(modelType, m_data);
                    }
                }
            }

            m_name = null;
        }

        public IBehavior CreateInitialized()
        {
            return CreateInitialized(false);
        }

        IBehavior IInternalBehaviorModel.Instantiate()
        {
            return m_data.BhvFactory();
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
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class BehaviorModelAttribute : Attribute
    {
        private string m_defaultName;
        private string m_description;
        private string m_category;

        public string DefaultName
        {
            get { return m_defaultName ?? BehaviorType.Name; }
            set { m_defaultName = String.IsNullOrEmpty(value) ? null : value; }
        }

        public string Description
        {
            get { return m_description ?? String.Empty; }
            set { m_description = String.IsNullOrEmpty(value) ? null : value; }
        }

        public string Category
        {
            get { return m_category ?? String.Empty; }
            set { m_category = String.IsNullOrEmpty(value) ? null : value; }
        }

        public bool HideFromEditor
        {
            get; set;
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
                throw new ArgumentException("Not a valid behavior type.");
            }

            BehaviorType = behaviorType;
            HideFromEditor = false;
        }
    }
}
