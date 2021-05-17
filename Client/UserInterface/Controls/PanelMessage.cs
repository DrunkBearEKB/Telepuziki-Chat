using System.Windows.Forms;

namespace Client.UserInterface.Controls
{
    public class PanelMessage : Panel
    {
        private string message;
        private string time;

        public PanelMessage(string message, string time)
        {
            this.message = message;
            this.time = time;

            this.Initialze();
        }

        private void Initialze()
        {
            Label labelMessage = new Label
            {

            };
            this.Controls.Add(labelMessage);

            Label labelTime = new Label
            {
                
            };
            this.Controls.Add(labelTime);
        }
    }
}