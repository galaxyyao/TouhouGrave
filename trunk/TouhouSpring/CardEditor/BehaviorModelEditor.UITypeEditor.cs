using System;
using System.Collections.Generic;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;

namespace TouhouSpring
{
    static partial class BehaviorModelEditor
    {
        public class TypeEditor : UITypeEditor
        {
            public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
            {
                return UITypeEditorEditStyle.Modal;
            }

            public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
            {
                var bhvModel = value as Behaviors.IBehaviorModel;
                var dlg = new BehaviorEditor(true, bhvModel != null ? CloneBehaviorModel(bhvModel) : null);
                return dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK
                       ? dlg.EditedBehaviorModel
                       : value;
            }

            private Behaviors.IBehaviorModel CloneBehaviorModel(Behaviors.IBehaviorModel obj)
            {
                using (var stream = new MemoryStream())
                {
                    using (var xw = XmlWriter.Create(stream, new XmlWriterSettings() { Indent = true }))
                    {
                        IntermediateSerializer.Serialize(xw, obj, Environment.CurrentDirectory);
                    }
                    stream.Seek(0, SeekOrigin.Begin);
                    using (var xr = XmlReader.Create(stream))
                    {
                        return IntermediateSerializer.Deserialize<Behaviors.IBehaviorModel>(xr, Environment.CurrentDirectory);
                    }
                }
            }
        }
    }
}
