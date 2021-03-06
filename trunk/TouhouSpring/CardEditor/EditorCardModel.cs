﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace TouhouSpring
{
    class EditorCardModel : ICardModel
    {
        [ReadOnly(true)]
        public string Id
        {
            get; set;
        }

        public string Name
        {
            get; set;
        }

        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        public string Description
        {
            get; set;
        }

        public string ArtworkUri
        {
            get; set;
        }

        [Browsable(false)]
        [ContentSerializer(ElementName = "Behaviors", CollectionItemName = "Behavior")]
        public List<Behaviors.IBehaviorModel> Behaviors
        {
            get; set;
        }

        IList<Behaviors.IBehaviorModel> ICardModel.Behaviors { get { return Behaviors; } }

        private string m_category;

        public string Category
        {
            get { return m_category ?? String.Empty; }
            set
            {
                m_category = String.Join("/", value.Split(new char[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries));
            }
        }

        public EditorCardModel()
        {
            Behaviors = new List<Behaviors.IBehaviorModel>();
        }

        public CardInstance Instantiate(Player owner)
        {
            throw new InvalidOperationException("Can't instantiate CardInstance from EditorCardModel.");
        }
    }

    class CardModelList : List<EditorCardModel> { }
}
