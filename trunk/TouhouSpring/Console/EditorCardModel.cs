using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;

namespace TouhouSpring
{
    class EditorCardModel : ICardModel
    {
        public string Id
        {
            get; set;
        }

        public string Name
        {
            get; set;
        }

        public string Description
        {
            get; set;
        }

        public string ArtworkUri
        {
            get; set;
        }

        [ContentSerializer(ElementName = "Behaviors", CollectionItemName = "Behavior")]
        public List<Behaviors.BehaviorModel> Behaviors
        {
            get; set;
        }

        public string Category
        {
            get; set;
        }

        public EditorCardModel()
        {
            Behaviors = new List<Behaviors.BehaviorModel>();
        }

        public BaseCard Instantiate(Player owner)
        {
            throw new InvalidOperationException("Can't instantiate BaseCard from EditorCardModel.");
        }
    }

    class CardModelList : List<EditorCardModel> { }

    [ContentTypeSerializer]
    class CardModelListSerializer : ContentTypeSerializer<CardModelList>
    {
        private ContentSerializerAttribute m_format = new ContentSerializerAttribute()
        {
            ElementName = "Item"
        };

        protected override void Serialize(IntermediateWriter output, CardModelList value, ContentSerializerAttribute format)
        {
            foreach (var cardModel in value)
            {
                output.WriteSharedResource(cardModel, m_format);
            }
        }

        protected override CardModelList Deserialize(IntermediateReader input, ContentSerializerAttribute format, CardModelList existingInstance)
        {
            if (existingInstance == null)
            {
                existingInstance = new CardModelList();
            }

            while (input.MoveToElement(m_format.ElementName))
            {
                input.ReadSharedResource<EditorCardModel>(m_format, existingInstance.Add);
            }

            return existingInstance;
        }
    }

    [ContentTypeSerializer]
    class CardModelReferenceSerializer : ContentTypeSerializer<CardModelReference>
    {
        private ContentSerializerAttribute m_format = new ContentSerializerAttribute()
        {
            FlattenContent = true
        };

        protected override void Serialize(IntermediateWriter output, CardModelReference value, ContentSerializerAttribute format)
        {
            output.WriteSharedResource(value.Target, m_format);
        }

        protected override CardModelReference Deserialize(IntermediateReader input, ContentSerializerAttribute format, CardModelReference existingInstance)
        {
            if (existingInstance == null)
            {
                existingInstance = new CardModelReference();
            }

            input.ReadSharedResource<ICardModel>(m_format, cardModel => existingInstance.Target = cardModel);
            return existingInstance;
        }
    }
}
