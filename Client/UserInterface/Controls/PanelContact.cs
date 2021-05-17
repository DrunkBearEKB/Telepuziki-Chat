using System;
using System.Drawing;
using System.Windows.Forms;

namespace Client.UserInterface.Controls
{
    public class PanelContact : Panel
    {
        public bool Selected { get; set; }
        private readonly string id;
        public string LastMessage;

        public Label labelId;
        public Label labelLastMessage;

        private readonly Color backColor = Color.FromArgb(23, 33, 43);
        private readonly Color backColorEnter = Color.FromArgb(36, 47, 61);
        private readonly Color backColorSelected = Color.FromArgb(43, 82, 120);

        public PanelContact(string id)
        {
            this.id = id;
            this.LastMessage = "";

            this.Selected = false;

            this.Initialize();
        }

        public PanelContact(string id, string message)
        {
            this.id = id;
            this.LastMessage = message;

            this.Selected = false;

            this.Initialize();
        }

        private void Initialize()
        {
            this.labelId = new Label
            {
                Location = new Point(5, 5),
                Size = new Size(this.Width - 20, 20),

                Text = this.id,
                Font = new Font(Resources.FontName, 13.8F, FontStyle.Bold, GraphicsUnit.Point, 204),
                ForeColor = Color.White
            };
            this.labelId.MouseEnter += new EventHandler(
                (sender, e) =>
                {
                    if (this.Selected)
                    {
                        this.labelId.BackColor = this.backColorSelected;
                        this.labelLastMessage.BackColor = this.backColorSelected;
                        this.BackColor = this.backColorSelected;
                    }
                    else
                    {
                        this.labelId.BackColor = this.backColorEnter;
                        this.labelLastMessage.BackColor = this.backColorEnter;
                        this.BackColor = this.backColorEnter;
                    }
                });
            this.labelId.MouseLeave += new EventHandler(
                (sender, e) =>
                {

                });
            this.labelId.Click += new EventHandler(
                (sender, e) =>
                {
                    this.OnClick(e);
                });
            this.Controls.Add(this.labelId);

            this.labelLastMessage = new Label
            {
                Location = new Point(25, 30),
                Size = new Size(this.Width - 20, 20),

                Text = this.LastMessage,
                Font = new Font(Resources.FontName, 10.8F, FontStyle.Bold, GraphicsUnit.Point, 204),
                ForeColor = Color.White
            };
            this.labelLastMessage.MouseEnter += new EventHandler(
                (sender, e) =>
                {
                    if (this.Selected)
                    {
                        this.labelId.BackColor = this.backColorSelected;
                        this.labelLastMessage.BackColor = this.backColorSelected;
                        this.BackColor = this.backColorSelected;
                    }
                    else
                    {
                        this.labelId.BackColor = this.backColorEnter;
                        this.labelLastMessage.BackColor = this.backColorEnter;
                        this.BackColor = this.backColorEnter;
                    }
                });
            this.labelLastMessage.MouseLeave += new EventHandler(
                (sender, e) =>
                {
                    
                });
            this.labelLastMessage.Click += new EventHandler(
                (sender, e) =>
                {
                    this.OnClick(e);
                });
            this.Controls.Add(this.labelLastMessage);
        }

        public void SetLastMessage(string author, string message)
        {
            if (message.Length < 22)
            {
                this.labelLastMessage.Text =
                    $"{(author == this.id ? author : "you")}: " +
                    $"{message}";
            }
            else
            {
                this.labelLastMessage.Text =
                    $"{(author == this.id ? author : "you")}: " +
                    $"{message.Substring(0, 18)}...";
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);

            if (!this.Selected)
            {
                this.SuspendLayout();

                this.labelId.BackColor = this.backColorEnter;
                this.labelLastMessage.BackColor = this.backColorEnter;
                this.BackColor = this.backColorEnter;

                this.ResumeLayout();
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseEnter(e);

            if (!this.Selected)
            {
                this.SuspendLayout();

                this.labelId.BackColor = this.backColor;
                this.labelLastMessage.BackColor = this.backColor;
                this.BackColor = this.backColor;

                this.ResumeLayout();
            }
        }
    }
}