using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Network.Message;
using Network.Message.ExchangingMessages;

namespace Client.UserInterface.Controls
{
    public class PanelChatBox : Panel
    {
        private List<IMessage> messages;
        private List<Label> labels;

        private static int messageDefaultHeight = 40;
        private static int lineDefaultHeight = 40;
        public PanelChatBox(List<IMessage> messages)
        {
            this.messages = messages ?? new List<IMessage>();

            this.Initialize();
        }

        public void AddMessage(IMessage message)
        {
            this.messages.Add(message);
        }

        private void Initialize()
        {
            int previousHeight = 0;
            int sep = 5;
            for (int i = 0; i < this.messages.Count; i++)
            {
                var size = GetMessagePanelSize(this.messages[i]);
                var label = new Label
                {
                    Location = new Point(5, previousHeight + sep),
                    Size = size,
                    Text = (this.messages[i] as TextMessage)?.Content
                };
                previousHeight += size.Height + sep;
            }
        }

        private static Size GetMessagePanelSize(IMessage message)
        {
            int width;
            int height;
            
            if (message.Type == MessageType.File || message.Type == MessageType.Voice)
            {
                width = 100;
                height = messageDefaultHeight;
            }
            else
            {
                var textMessage = message as TextMessage;
                var lines = textMessage.Content.Split('\n');
                var amount = 0;
                foreach (var line in lines)
                {
                    amount += line.Length / 30 + 1;
                }

                width = 100;
                height = 20 + amount * lineDefaultHeight;
            }

            return new Size(width, height);
        }
    }
}