using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Xml.Linq;
using TouhouSpring.ExtensionMethods;
namespace TouhouSpring
{
    class ConversationManager
    {
        XDocument _doc;
        XElement _sceneNode;
        XElement _sentenceNode;

        public ConversationManager()
        {
            Load();
        }

        public void Load(){
            _doc = XDocument.Load(Assembly.GetExecutingAssembly().GetManifestResourceStream("TouhouSpring.Resources.Conversations.xml"));
        }

        public string Scene
        {
            get
            {
                if (_sceneNode == null) return String.Empty;
                return _sceneNode.GetAttribute("name");
            }
            set
            {
                _sceneNode = _doc.Descendants("scene").First(el => el.GetAttribute("name") == value);
                _sentenceNode = _sceneNode.Descendants("sentence").First();
            }
        }

        public bool Next()
        {
            if (_sentenceNode == null) return false;
            if (_sentenceNode.NextNode == null) return false;
            _sentenceNode = _sentenceNode.NextNode as XElement;
            return true;
        }

        public string GetCurrentText()
        {
            return (_sentenceNode == null) ? String.Empty : _sentenceNode.Value;
        }
    }
}
