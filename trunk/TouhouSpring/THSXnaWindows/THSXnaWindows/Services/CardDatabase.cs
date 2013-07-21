using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace TouhouSpring.Services
{
    [LifetimeDependency(typeof(ResourceManager))]
    class CardDatabase : GameService
    {
        public class CardModelDatabaseReader : ContentTypeReader<CardModel.Database>
        {
            protected override CardModel.Database Read(ContentReader input, CardModel.Database existingInstance)
            {
                if (existingInstance == null)
                {
                    existingInstance = new CardModel.Database();
                }

                var count = input.ReadInt32();
                for (int i = 0; i < count; ++i)
                {
                    input.ReadSharedResource<CardModel>(cm => existingInstance.Add(cm.Id, cm));
                }
                return existingInstance;
            }
        }

        public class CardModelReferenceReader : ContentTypeReader<CardModelReference>
        {
            protected override CardModelReference Read(ContentReader input, CardModelReference existingInstance)
            {
                if (existingInstance == null)
                {
                    existingInstance = new CardModelReference();
                }

                input.ReadSharedResource<CardModel>(cm =>
                {
                    existingInstance.Value = cm;
                });
                return existingInstance;
            }
        }

        private CardModel.Database m_cardModels;

        public CardModel GetModel(string id)
        {
            if (id == null)
            {
                throw new ArgumentNullException("id");
            }

            return m_cardModels[id];
        }

        public override void Startup()
        {
            m_cardModels = GameApp.Service<ResourceManager>().Acquire<CardModel.Database>("TouhouSpring");

            int bhvModels = 0;
            int staticBhvModel = 0;
            foreach (var bhvModelType in AssemblyReflection.GetTypesImplements<Behaviors.IBehaviorModel>())
            {
                var attr = bhvModelType.GetAttribute<Behaviors.BehaviorModelAttribute>();
                if (attr == null)
                {
                    continue;
                }

                ++bhvModels;
                var bhvType = attr.BehaviorType;
                if (Behaviors.BehaviorModel.GetIsBehaviorStatic(bhvType))
                {
                    ++staticBhvModel;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine(String.Format("NonStatic behavior : {0}", bhvType.FullName));
                }
            }

            System.Diagnostics.Debug.WriteLine("Static/Behaviors {0}/{1}", staticBhvModel, bhvModels);
        }
    }
}
