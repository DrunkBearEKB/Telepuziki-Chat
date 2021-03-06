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
                ForeColor = ClientForm.Style.ForeColorResidual
            };
            this.labelId.MouseEnter += new EventHandler(
                (sender, e) =>
                {
                    this.SuspendLayout();
                    
                    if (this.Selected)
                    {
                        this.labelId.BackColor = ClientForm.Style.BackColorSelected;
                        this.labelLastMessage.BackColor = ClientForm.Style.BackColorSelected;
                        this.BackColor = ClientForm.Style.BackColorSelected;
                    }
                    else
                    {
                        this.labelId.BackColor = ClientForm.Style.BackColorEntered;
                        this.labelLastMessage.BackColor = ClientForm.Style.BackColorEntered;
                        this.BackColor = ClientForm.Style.BackColorEntered;
                    }

                    this.ResumeLayout();
                });
            this.labelId.MouseLeave += new EventHandler(
                (sender, e) =>
                {
                    this.SuspendLayout();
                    
                    if (this.Selected)
                    {
                        this.labelId.BackColor = ClientForm.Style.BackColorSelected;
                        this.labelLastMessage.BackColor = ClientForm.Style.BackColorSelected;
                        this.BackColor = ClientForm.Style.BackColorSelected;
                    }
                    else
                    {
                        this.labelId.BackColor = ClientForm.Style.BackColorSecondary;
                        this.labelLastMessage.BackColor = ClientForm.Style.BackColorSecondary;
                        this.BackColor = ClientForm.Style.BackColorSecondary;
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
                ForeColor = ClientForm.Style.ForeColorResidual
            };
            this.labelLastMessage.MouseEnter += new EventHandler(
                (sender, e) =>
                {
                    this.SuspendLayout();
                    
                    if (this.Selected)
                    {
                        this.labelId.BackColor = ClientForm.Style.BackColorSelected;
                        this.labelLastMessage.BackColor = ClientForm.Style.BackColorSelected;
                        this.BackColor = ClientForm.Style.BackColorSelected;
                    }
                    else
                    {
                        this.labelId.BackColor = ClientForm.Style.BackColorEntered;
                        this.labelLastMessage.BackColor = ClientForm.Style.BackColorEntered;
                        this.BackColor = ClientForm.Style.BackColorEntered;
                    }

                    this.ResumeLayout();
                });
            this.labelLastMessage.MouseLeave += new EventHandler(
                (sender, e) =>
                {
                    this.SuspendLayout();
                    
                    if (this.Selected)
                    {
                        this.labelId.BackColor = ClientForm.Style.BackColorSelected;
                        this.labelLastMessage.BackColor = ClientForm.Style.BackColorSelected;
                        this.BackColor = ClientForm.Style.BackColorSelected;
                    }
                    else
                    {
                        this.labelId.BackColor = ClientForm.Style.BackColorSecondary;
                        this.labelLastMessage.BackColor = ClientForm.Style.BackColorSecondary;
                        this.BackColor = ClientForm.Style.BackColorSecondary;
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
            this.SuspendLayout();
            
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
            
            this.ResumeLayout();
        }

        public void RemoveSelection()
        {
            this.SuspendLayout();

            this.Selected = false;
            this.labelId.BackColor = ClientForm.Style.BackColorSecondary;
            this.labelLastMessage.BackColor = ClientForm.Style.BackColorSecondary;
            this.BackColor = ClientForm.Style.BackColorSecondary;

            this.ResumeLayout();
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);

            if (!this.Selected)
            {
                this.SuspendLayout();

                this.labelId.BackColor = ClientForm.Style.BackColorEntered;
                this.labelLastMessage.BackColor = ClientForm.Style.BackColorEntered;
                this.BackColor = ClientForm.Style.BackColorEntered;

                this.ResumeLayout();
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseEnter(e);

            if (!this.Selected)
            {
                this.SuspendLayout();

                this.labelId.BackColor = ClientForm.Style.BackColorSecondary;
                this.labelLastMessage.BackColor = ClientForm.Style.BackColorSecondary;
                this.BackColor = ClientForm.Style.BackColorSecondary;

                this.ResumeLayout();
            }
        }
    }
}