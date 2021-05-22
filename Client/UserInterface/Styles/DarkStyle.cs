using System.Drawing;

namespace Client.UserInterface.Styles
{
    public class DarkStyle : Style
    {
        public override Color BackColorMain => Color.FromArgb(14, 22, 33);
        public override Color BackColorSecondary => Color.FromArgb(23, 33, 43);
        public override Color BackColorEntered => Color.FromArgb(36, 47, 61);
        public override Color BackColorSelected => Color.FromArgb(43, 82, 120);
        public override Color ForeColorMain => Color.DarkGray;
        public override Color ForeColorSecondary => Color.White;
        public override Color ForeColorResidual => Color.White;
        public override string pathImages => "themeDark";

        public DarkStyle(string pathResources) : base(pathResources)
        {
            
        }
    }
}