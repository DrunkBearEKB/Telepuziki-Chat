using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Network.Message;
using Network.Message.ExchangingMessages;

namespace Client.UserInterface.Controls
{
    public class PanelMessage : Panel
    {
        private readonly IMessage message;
        private PictureBox pictureBoxAuthor;
        private Label labelAuthor;
        private Label labelContent;
        private Label labelTime;

        public PanelMessage(IMessage message)
        {
            this.message = message;

            this.Initialize();
        }

        private void Initialize()
        {
            this.Resize += this.OnResize;
            
            this.pictureBoxAuthor = new PictureBox
            {
                Location = new Point(10, 10),
                Size = new Size(40, 40),

                TabStop = false,
                Margin = new Padding(0),
                Image = ClientForm.Style.GetImage("profile", true),
                SizeMode = PictureBoxSizeMode.Zoom
            };
            this.Controls.Add(this.pictureBoxAuthor);
            
            this.labelAuthor = new Label
            {
                Location = new Point(55, 5),
                Height = 25,
                ForeColor = ClientForm.Style.ForeColorMain,
                Font = new Font(Resources.FontName, 12F, FontStyle.Bold, GraphicsUnit.Point, 204),
                Text = this.message.IdAuthor,
                AutoSize = true
            };
            this.Controls.Add(this.labelAuthor);
            
            this.labelTime = new Label
            {
                Location = new Point( this.labelAuthor.Location.X + 10 + this.labelAuthor.Width, 10),
                Height = 25,
                ForeColor = ClientForm.Style.ForeColorSecondary,
                Font = new Font(Resources.FontName, 8F, FontStyle.Regular, GraphicsUnit.Point, 204),
                Text = $"{message.Time.ToShortTimeString()}  {message.Time.ToLongDateString()}",
                AutoSize = true
            };
            this.Controls.Add(this.labelTime);
            
            this.labelContent = new Label
            {
                Location = new Point(this.labelAuthor.Location.X + 10, 
                    this.labelAuthor.Location.Y + this.labelAuthor.Height),
                ForeColor = ClientForm.Style.ForeColorMain,
                Font = new Font(Resources.FontName, 12F, FontStyle.Regular, GraphicsUnit.Point, 204),
                Text = ((TextMessage)message).Content,
                AutoSize = true,
                MaximumSize = new Size(this.Width - (this.labelAuthor.Location.X + 10), this.Height)
            };
            this.Controls.Add(this.labelContent);

            this.Height = this.labelContent.Location.Y + this.labelContent.Height + 10;
        }

        private void OnResize(object sender, EventArgs e)
        {
            this.labelContent.MaximumSize = new Size(this.Width - (this.labelAuthor.Location.X + 20), this.Height);
            this.Height = this.labelContent.Location.Y + this.labelContent.Height + 10;
        }
    }
}