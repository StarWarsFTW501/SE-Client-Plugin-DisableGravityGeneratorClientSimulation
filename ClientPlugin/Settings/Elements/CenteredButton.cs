using Sandbox.Graphics.GUI;
using System;
using System.Collections.Generic;
using System.Text;
using VRage.Utils;

namespace ClientPlugin.Settings.Elements
{
    internal class CenteredButtonAttribute : Attribute, IElement
    {
        public readonly string Label;
        public readonly string Description;

        public CenteredButtonAttribute(string label = null, string description = null)
        {
            Label = label;
            Description = description;
        }

        public List<Control> GetControls(string name, Func<object> propertyGetter, Action<object> propertySetter)
        {
            var label = Tools.GetLabelOrDefault(name, Label);
            var button = new MyGuiControlButton(text: new StringBuilder(label), toolTip: Description);
            button.ButtonClicked += (_)=>((Action)propertyGetter())();

            return new List<Control>()
            {
                new Control(new MyGuiControlLabel(text: ""), fixedWidth: Control.LabelMinWidth),
                new Control(button, originAlign: MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_TOP),
            };
        }
        public List<Type> SupportedTypes { get; } = new List<Type>()
        {
            typeof(Delegate)
        };
    }
}
