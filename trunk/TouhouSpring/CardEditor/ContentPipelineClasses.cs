using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace TouhouSpring
{
    [ContentTypeWriter]
    class CardModelDatabaseWriter : ContentTypeWriter<CardModel.Database>
    {
        public override bool CanDeserializeIntoExistingObject
        {
            get { return true; }
        }

        public override string GetRuntimeReader(Microsoft.Xna.Framework.Content.Pipeline.TargetPlatform targetPlatform)
        {
            return "TouhouSpring.Services.CardDatabase+CardModelDatabaseReader, TouhouSpring.XnaWindows";
        }

        protected override void Write(ContentWriter output, CardModel.Database value)
        {
            output.Write(value.Count);
            foreach (var cm in value.Values)
            {
                output.WriteSharedResource(cm);
            }
        }
    }

    [ContentProcessor(DisplayName = "Card Database - TouhouSpring")]
    class CardModelProcessor : ContentProcessor<List<EditorCardModel>, CardModel.Database>
    {
        public static Dictionary<EditorCardModel, CardModel> CardModelMapping
        {
            get; private set;
        }

        static CardModelProcessor()
        {
            CardModelMapping = new Dictionary<EditorCardModel, CardModel>();
        }

        public override CardModel.Database Process(List<EditorCardModel> input, ContentProcessorContext context)
        {
            CardModel.Database output = new CardModel.Database();
            for (int i = 0; i < input.Count; ++i)
            {
                var ecm = input[i];
                var cm = new CardModel
                {
                    Id = ecm.Id,
                    Name = ecm.Name,
                    Description = ecm.Description,
                    ArtworkUri = ecm.ArtworkUri,
                    Behaviors = ecm.Behaviors
                };
                output.Add(ecm.Id, cm);
                CardModelMapping.Add(ecm, cm);
            }
            return output;
        }
    }

    [ContentTypeWriter]
    class CardModelReferenceWriter : ContentTypeWriter<CardModelReference>
    {
        public override bool CanDeserializeIntoExistingObject
        {
            get { return true; }
        }

        public override string GetRuntimeReader(Microsoft.Xna.Framework.Content.Pipeline.TargetPlatform targetPlatform)
        {
            return "TouhouSpring.Services.CardDatabase+CardModelReferenceReader, TouhouSpring.XnaWindows";
        }

        protected override void Write(ContentWriter output, CardModelReference value)
        {
            output.WriteSharedResource(CardModelProcessor.CardModelMapping[(value.Value as EditorCardModel)]);
        }
    }
}
