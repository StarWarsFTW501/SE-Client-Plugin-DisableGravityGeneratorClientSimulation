using Sandbox.Graphics.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using ClientPlugin.Settings.Elements;
using VRageMath;

namespace ClientPlugin.Settings.Layouts
{
    internal class SettingsLayout : Layout
    {
        MyGuiControlList List;

        public override Vector2 SettingsPanelSize => new Vector2(0.5f, 0.5f);

        public SettingsLayout(Func<List<List<Control>>> getControls) : base(getControls) { }

        public override List<MyGuiControlBase> RecreateControls()
        {
            List = new MyGuiControlList();

            foreach (var row in GetControls())
            {
                foreach (var control in row)
                {
                    List.Controls.Add(control.GuiControl);
                }
            }

            return new List<MyGuiControlBase> { List };
        }

        public override void LayoutControls()
        {
            var controls = GetControls();
            var totalHeight = .01f + controls.Select(row => row.Max(c => c.GuiControl.Size.Y) + .01f).Sum();

            var rowY = -0.5f * totalHeight + .01f;
            foreach (var row in controls)
            {
                // Vertical

                var rowHeight = row.Max(c => c.GuiControl.Size.Y);
                var controlY = rowY + 0.5f * rowHeight;

                rowY += rowHeight + .01f;

                // Horizontal

                var totalMinWidth = row.Select(c => (c.FixedWidth ?? c.MinWidth) + c.RightMargin).Sum();
                var remainingWidth = Math.Max(0f, SettingsPanelSize.X - .02f - totalMinWidth);
                var sumFillFactors = row.Select(c => c.FixedWidth.HasValue ? 0f : c.FillFactor ?? 0f).Sum();
                var unitWidth = sumFillFactors > 0f ? remainingWidth / sumFillFactors : 0f;

                var totalInherentWidth = row.Select(c => (c.FixedWidth ?? Math.Max(c.MinWidth, c.FillFactor.HasValue ? unitWidth * c.FillFactor.Value : c.GuiControl.Size.X)) + c.RightMargin).Sum();

                var controlX = -0.5f * SettingsPanelSize.X + .01f + (unitWidth > 0 ? 0 : (remainingWidth - (totalInherentWidth - totalMinWidth)) * .5f);
                foreach (var control in row)
                {
                    var guiControl = control.GuiControl;
                    guiControl.Position = new Vector2(controlX, controlY) + control.Offset;
                    guiControl.OriginAlign = control.OriginAlign;

                    var sizeY = guiControl.Size.Y;
                    if (control.FixedWidth.HasValue)
                    {
                        guiControl.Size = new Vector2(control.FixedWidth.Value, sizeY);
                        guiControl.SetMaxWidth(control.FixedWidth.Value);
                    }
                    else if (control.FillFactor.HasValue)
                    {
                        guiControl.Size = new Vector2(Math.Max(control.MinWidth, unitWidth * control.FillFactor.Value), sizeY);
                    }
                    else
                    {
                        guiControl.Size = new Vector2(Math.Max(guiControl.Size.X, control.MinWidth), sizeY);
                    }

                    controlX += guiControl.Size.X + control.RightMargin;
                }
            }
        }
    }
}
