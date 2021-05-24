using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using Network.Message;
using Network.Message.ExchangingMessages;

namespace Client.UserInterface.Controls
{
    public class PanelChatBox : Panel
    {
        private List<IMessage> messages;
        private List<PanelMessage> panelsMessages;
        private ScrollBar scroll;

        private static int messageDefaultHeight = 40;
        private static int lineDefaultHeight = 40;
        
        public PanelChatBox()
        {
            this.messages = new List<IMessage>();
            
            this.Initialize();
        }
        public PanelChatBox(List<IMessage> messages)
        {
            this.messages = messages ?? new List<IMessage>();
            
            this.Initialize();
        }

        private void Initialize()
        {
            this.panelsMessages = new List<PanelMessage>();

            this.MouseEnter += this.OnMouseEnter;
                
            foreach (IMessage message in this.messages)
            {
                this.PrintMessage(message);
            }
        }

        public void AddMessage(IMessage message)
        {
            this.messages.Add(message);
            this.PrintMessage(message);
        }

        private void PrintMessage(IMessage message)
        {
            this.SuspendLayout();
            
            int y = this.panelsMessages.Count != 0
                ? this.panelsMessages.Last().Location.Y + this.panelsMessages.Last().Height
                : 0;
            PanelMessage panelMessage = new PanelMessage(message)
            {
                Dock = DockStyle.Bottom,
                BorderStyle = BorderStyle.FixedSingle,
                Visible = false
            };
            this.panelsMessages.Add(panelMessage);
            this.Controls.Add(panelMessage);
            
            this.ResumeLayout();

            panelMessage.Visible = true;
        }

        public void Clear()
        {
            this.Controls.Clear();
            this.panelsMessages.Clear();
        }

        private void OnMouseEnter(object sender, EventArgs  e)
        {
            if (!this.Focused)
            {
                this.Focus();
            }
        }
    }
}