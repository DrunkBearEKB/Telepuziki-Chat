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
                Size = new Size(this.Width - 20, 25),

                Text = this.id,
                Font = new Font(Resources.FontName, 11.8F, FontStyle.Bold, GraphicsUnit.Point, 204),
                ForeColor = Color.White
            };
            this.labelId.MouseEnter += new EventHandler(
                (sender, e) =>
                {
                    this.SuspendLayout();
                    
                    if (this.Selected)
                    {
                        this.labelId.BackColor = ClientForm.BackColorPanelContactSelected;
                        this.labelLastMessage.BackColor = ClientForm.BackColorPanelContactSelected;
                        this.BackColor = ClientForm.BackColorPanelContactSelected;
                    }
                    else
                    {
                        this.labelId.BackColor = ClientForm.BackColorPanelContactEnter;
                        this.labelLastMessage.BackColor = ClientForm.BackColorPanelContactEnter;
                        this.BackColor = ClientForm.BackColorPanelContactEnter;
                    }

                    this.ResumeLayout();
                });
            this.labelId.MouseLeave += new EventHandler(
                (sender, e) =>
                {
                    this.SuspendLayout();
                    
                    if (this.Selected)
                    {
                        this.labelId.BackColor = ClientForm.BackColorPanelContactSelected;
                        this.labelLastMessage.BackColor = ClientForm.BackColorPanelContactSelected;
                        this.BackColor = ClientForm.BackColorPanelContactSelected;
                    }
                    else
                    {
                        this.labelId.BackColor = ClientForm.BackColorPanelCenter;
                        this.labelLastMessage.BackColor = ClientForm.BackColorPanelCenter;
                        this.BackColor = ClientForm.BackColorPanelCenter;
                    }

                    this.ResumeLayout();
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
                Size = new Size(this.Width - 20, 25),

                Text = this.LastMessage,
                Font = new Font(Resources.FontName, 10.8F, FontStyle.Bold, GraphicsUnit.Point, 204),
                ForeColor = Color.White
            };
            this.labelLastMessage.MouseEnter += new EventHandler(
                (sender, e) =>
                {
                    this.SuspendLayout();
                    
                    if (this.Selected)
                    {
                        this.labelId.BackColor = ClientForm.BackColorPanelContactSelected;
                        this.labelLastMessage.BackColor = ClientForm.BackColorPanelContactSelected;
                        this.BackColor = ClientForm.BackColorPanelContactSelected;
                    }
                    else
                    {
                        this.labelId.BackColor = ClientForm.BackColorPanelContactEnter;
                        this.labelLastMessage.BackColor = ClientForm.BackColorPanelContactEnter;
                        this.BackColor = ClientForm.BackColorPanelContactEnter;
                    }

                    this.ResumeLayout();
                });
            this.labelLastMessage.MouseLeave += new EventHandler(
                (sender, e) =>
                {
                    this.SuspendLayout();
                    
                    if (this.Selected)
                    {
                        this.labelId.BackColor = ClientForm.BackColorPanelContactSelected;
                        this.labelLastMessage.BackColor = ClientForm.BackColorPanelContactSelected;
                        this.BackColor = ClientForm.BackColorPanelContactSelected;
                    }
                    else
                    {
                        this.labelId.BackColor = ClientForm.BackColorPanelCenter;
                        this.labelLastMessage.BackColor = ClientForm.BackColorPanelCenter;
                        this.BackColor = ClientForm.BackColorPanelCenter;
                    }

                    this.ResumeLayout();
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
                    $"{message.Substring(0, 14)}...";
            }
        }

        public void RemoveSelection()
        {
            this.SuspendLayout();

            this.Selected = false;
            this.labelId.BackColor = ClientForm.BackColorPanelCenter;
            this.labelLastMessage.BackColor = ClientForm.BackColorPanelCenter;
            this.BackColor = ClientForm.BackColorPanelCenter;

            this.ResumeLayout();
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);

            if (!this.Selected)
            {
                this.SuspendLayout();

                this.labelId.BackColor = ClientForm.BackColorPanelContactEnter;
                this.labelLastMessage.BackColor = ClientForm.BackColorPanelContactEnter;
                this.BackColor = ClientForm.BackColorPanelContactEnter;

                this.ResumeLayout();
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseEnter(e);

            if (!this.Selected)
            {
                this.SuspendLayout();

                this.labelId.BackColor = ClientForm.BackColorPanelCenter;
                this.labelLastMessage.BackColor = ClientForm.BackColorPanelCenter;
                this.BackColor = ClientForm.BackColorPanelCenter;

                this.ResumeLayout();
            }
        }
    }
}