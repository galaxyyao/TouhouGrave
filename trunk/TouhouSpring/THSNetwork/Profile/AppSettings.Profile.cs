using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace TouhouSpring
{
    public partial class AppSettings
    {
        public bool IsLoggedIn
        {
            get;
            set;
        }

        public string Email
        {
            get;
            set;
        }

        private Profile CurrentProfile
        {
            get;
            set;
        }

        public Profile ReadProfile()
        {
            XElement currentProfileElement = null;
            if (CurrentProfile != null)
                return CurrentProfile;
            CurrentProfile = new Profile();
            if (!IsLoggedIn)
            {
                #region Data Validation
                IEnumerable<XElement> profiles = (from el in ProfileDocument.Root.Elements("Profile")
                                                  where (string)el.Element("Uid") == "default"
                                                  select el);
                if (profiles.Count() != 1)
                {
                    throw new InvalidDataException("failed to read default profile");
                }
                #endregion
                currentProfileElement = profiles.First();

                //If no guid, give a new guid as initial value
                XElement guidElement = (from el in currentProfileElement.Elements("GUID")
                                        select el).FirstOrDefault();
                if (string.IsNullOrEmpty(guidElement.Value))
                {
                    guidElement.Value = Guid.NewGuid().ToString();
                    ProfileDocument.Save(m_profileFilePath, SaveOptions.None);
                }
                CurrentProfile.GUID = guidElement.Value;
            }
            else
            {
                #region Data Validation
                if (Email == null)
                {
                    throw new InvalidDataException("Failed to get login information");
                }
                IEnumerable<XElement> profiles = (from el in ProfileDocument.Root.Elements("Profile")
                                                  where (string)el.Element("Email") == Email
                                                  select el);
                if (profiles.Count() != 1)
                {
                    throw new InvalidDataException("Failed to read login user's profile");
                }
                currentProfileElement = profiles.First();

                XElement guidElement = (from el in currentProfileElement.Elements("GUID")
                                        select el).FirstOrDefault();
                if (string.IsNullOrEmpty(guidElement.Value))
                {
                    throw new InvalidDataException("login user's GUID should not be none");
                }
                CurrentProfile.GUID = guidElement.Value;
                #endregion
            }

            CurrentProfile.Uid = (from el in currentProfileElement.Elements("Uid")
                                  select (string)el).FirstOrDefault();
            CurrentProfile.Email = (from el in currentProfileElement.Elements("Email")
                                    select (string)el).FirstOrDefault();
            int deck1Id;
            int deck2Id;
            if (!Int32.TryParse((from el in currentProfileElement.Elements("Deck1Id")
                                 select (string)el).FirstOrDefault(), out deck1Id))
                throw new InvalidCastException("Failed to read Deck1Id");
            if (!Int32.TryParse((from el in currentProfileElement.Elements("Deck2Id")
                                 select (string)el).FirstOrDefault(), out deck2Id))
                throw new InvalidCastException("Failed to read Deck1Id");
            CurrentProfile.Deck1Id = deck1Id;
            CurrentProfile.Deck2Id = deck2Id;

            IEnumerable<XElement> deckElements = from deck in 
                                                     (from el in currentProfileElement.Elements("Decks")
                                                  select el).FirstOrDefault().Elements("Deck")
                                                     select deck;
            foreach (var deckElement in deckElements)
            {
                string strDeckId = (from el in deckElement.Elements("Id")
                                 select (string)el).FirstOrDefault();
                int deckId;
                if(!Int32.TryParse(strDeckId, out deckId))
                    throw new InvalidCastException("Failed to cast strDeckId to int");
                Profile.CardBuild cardBuild = new Profile.CardBuild(deckId);
                cardBuild.CardModelIds = (from card in
                                              (from el in deckElements.Elements("Card")
                                               select el).FirstOrDefault().Elements("Model")
                                          select (string)card).ToList();
                cardBuild.AssistModelIds = (from card in
                                              (from el in deckElements.Elements("Assist")
                                               select el).FirstOrDefault().Elements("Model")
                                          select (string)card).ToList();
                CurrentProfile.CardBuilds.Add(cardBuild);
            }

            return CurrentProfile;
        }
    }
}
