using System.Drawing;

namespace Client.UserInterface.Styles
{
    public class BlackPinkStyle : Style
    {
        public override Color BackColorMain => Color.Black;
        public override Color BackColorSecondary => Color.FromArgb(255, 128, 192);
        public override Color BackColorEntered => Color.FromArgb(255, 86, 171);
        public override Color BackColorSelected => Color.FromArgb(255, 43, 150);
        public override Color ForeColorMain => Color.FromArgb(255, 0, 128);
        public override Color ForeColorSecondary => Color.FromArgb(255, 128, 192);
        public override Color ForeColorResidual => Color.Black;
        public override string pathImages => "themeBlackPink";

        public BlackPinkStyle(string pathResources) : base(pathResources)
        {
            
        }
    }
}