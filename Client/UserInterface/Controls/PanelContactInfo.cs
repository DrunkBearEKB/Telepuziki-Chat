using System.Drawing;
using System.Windows.Forms;

namespace Client.UserInterface.Controls
{
    public class PanelContactInfo : Panel
    {
        private string id;

        private Label labelId;

        public PanelContactInfo(string id)
        {
            this.id = id;

            this.Initialize();
        }

        private void Initialize()
        {
            this.labelId = new Label
            {
                Location = new Point(0, 0),
                Size = new Size(this.Width, 30),
                Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right,

                Text = this.id,
                Font = new Font(Resources.FontName, 12.8F, FontStyle.Bold, GraphicsUnit.Point, 204),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.White
            };
            this.Controls.Add(this.labelId);
        }

        public void ChangeContact(string id)
        {
            this.id = id;
            this.labelId.Text = id;
        }
    }
}