using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Management;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows.Forms;

using Client.Network;
using Client.UserInterface.Controls;
using Network.Message;
using Network.Message.ExchangingMessages;

namespace Client.UserInterface
{
    public class ClientForm : Form, IClientForm
    {
        private TableLayoutPanel tableLayoutPanel;

        private Panel panelLeft;
        private readonly PerformanceCounter cpuCounter;
        private Label labelCpu;
        private readonly PerformanceCounter ramCounter;
        private readonly ulong ramTotalAmount;
        private Label labelRam;
        private Ping ping;
        private Label labelPing;
        private readonly Color colorPanelLeft = Color.FromArgb(14, 22, 33);

        private Panel panelCenter;
        private TextBox textBoxSearch;
        private Label labelSearchQuestion;
        private PanelContactsBox panelContactsBox;
        private readonly Color colorPanelCenter = Color.FromArgb(23, 33, 43);
        private readonly Color foreColorTextBoxSearch = Color.DarkGray;
        private readonly Color foreColorTextBoxSearchActive = Color.White;

        private TableLayoutPanel tableLayoutPanelRight;
        private PanelContactInfo panelContactInfo;
        private TextBox textBoxChat;
        private TableLayoutPanel tableLayoutPanelEnter;
        private TextBox textBoxEnter;
        private Button buttonSend;
        private readonly Color colorPanelRight = Color.FromArgb(14, 22, 33);

        private readonly System.Timers.Timer timer;
        private System.Timers.Timer timerTemp;
        private int historyLoaded = 0;

        private readonly ClientObject client;
        public Dictionary<string, List<IMessage>> dictMessageHistory;
        private readonly Dictionary<string, List<IMessage>> dictMessagesNotSended;
        private readonly Dictionary<string, string> dictMessageNotFinished;

        public ClientForm()
        {
            this.dictMessageHistory = new Dictionary<string, List<IMessage>>();
            this.dictMessagesNotSended = new Dictionary<string, List<IMessage>>();
            this.dictMessageNotFinished = new Dictionary<string, string>();

            this.cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            this.ramCounter = new PerformanceCounter("Memory", "Available MBytes");
            this.ping = new Ping();

            ManagementObjectSearcher ramMonitor =    //запрос к WMI для получения памяти ПК
            new ManagementObjectSearcher("SELECT TotalVisibleMemorySize,FreePhysicalMemory FROM Win32_OperatingSystem");
            foreach (ManagementObject objram in ramMonitor.Get())
            {
                this.ramTotalAmount = Convert.ToUInt64(objram["TotalVisibleMemorySize"]);    //общая память ОЗУ
            }

            this.InitializeComponent();
            this.PerformLayout();

            try
            {
                this.client = new ClientObject(Resources.Id);
                this.client.Start();
            }
            catch
            {

            }

            this.timer = new System.Timers.Timer(300);
            this.timer.Elapsed += new ElapsedEventHandler(this.OnTimerTick);
            this.timer.AutoReset = true;
            this.timer.Enabled = true;

            if (this.client.IsConnected)
            {
                this.timerTemp = new System.Timers.Timer(100);
                this.timerTemp.Elapsed += new ElapsedEventHandler(this.OnTimerTempTick);
                this.timerTemp.AutoReset = true;
                this.timerTemp.Enabled = true;
            }
        }

        public void InitializeComponent()
        {
            this.SuspendLayout();

            this.tableLayoutPanel = new TableLayoutPanel()
            {
                Location = new Point(0, 0),
                Dock = DockStyle.Fill
            };
            this.Controls.Add(this.tableLayoutPanel);

            this.InitializePanelLeft();
            this.InitializePanelCenter();
            this.InitializePanelRight();
            this.InitializeForm();

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        public void InitializePanelLeft()
        {
            this.tableLayoutPanel.ColumnStyles.Add(
                new ColumnStyle()
                {
                    Width = 80,
                    SizeType = SizeType.Absolute
                });

            // panelLeft
            this.panelLeft = new Panel()
            {
                Location = new Point(0, 0),
                Dock = DockStyle.Fill,
                BackColor = this.colorPanelLeft,
                Margin = new Padding(0)
            };
            this.tableLayoutPanel.Controls.Add(this.panelLeft, 0, 0);

            this.labelCpu = new Label
            {
                Dock = DockStyle.Bottom,

                BackColor = this.colorPanelLeft,
                ForeColor = Color.White,
                Font = new Font(Resources.FontName, 9F, FontStyle.Regular, GraphicsUnit.Point, 204),
                TextAlign = ContentAlignment.MiddleLeft
            };
            this.panelLeft.Controls.Add(this.labelCpu);

            this.labelRam = new Label
            {
                Dock = DockStyle.Bottom,

                BackColor = this.colorPanelLeft,
                ForeColor = Color.White,
                Font = new Font(Resources.FontName, 9F, FontStyle.Regular, GraphicsUnit.Point, 204),
                TextAlign = ContentAlignment.MiddleLeft
            };
            this.panelLeft.Controls.Add(this.labelRam);

            this.labelPing = new Label
            {
                Dock = DockStyle.Bottom,

                BackColor = this.colorPanelLeft,
                ForeColor = Color.White,
                Font = new Font(Resources.FontName, 9F, FontStyle.Regular, GraphicsUnit.Point, 204),
                TextAlign = ContentAlignment.MiddleLeft
            };
            this.panelLeft.Controls.Add(this.labelPing);
        }

        public void InitializePanelCenter()
        {
            this.tableLayoutPanel.ColumnStyles.Add(
                new ColumnStyle()
                {
                    Width = 240,
                    SizeType = SizeType.Absolute
                });

            // panelCenter
            this.panelCenter = new Panel()
            {
                Location = new Point(0, 0),
                Dock = DockStyle.Fill,
                BackColor = this.colorPanelCenter,
                Margin = new Padding(1)
            };
            this.tableLayoutPanel.Controls.Add(this.panelCenter, 1, 0);

            // textBoxSearch
            this.textBoxSearch = new TextBox()
            {
                Location = new Point(15, 15),
                Anchor = AnchorStyles.Right | AnchorStyles.Top,
                Size = new Size((int)this.tableLayoutPanel.ColumnStyles[1].Width - 30, 76 - 30),

                BackColor = Color.FromArgb(36, 47, 61),
                Text = "Search...",
                Font = new Font(Resources.FontName, 13.8F, FontStyle.Regular, GraphicsUnit.Point, 204),
                ForeColor = this.foreColorTextBoxSearch,
                BorderStyle = BorderStyle.FixedSingle,
                //Multiline = true,
                //AcceptsReturn = true,
                TabStop = false
            };
            //this.textBoxSearch.TextChanged += new EventHandler(this.TextBoxSearchTextChanged);
            this.textBoxSearch.Enter += new EventHandler(this.TextBoxSearchEnterEvent);
            this.textBoxSearch.GotFocus += new EventHandler(this.TextBoxSearchGotFocus);
            this.textBoxSearch.LostFocus += new EventHandler(this.TextBoxSearchLostFocus);
            this.textBoxSearch.KeyDown += new KeyEventHandler(this.TextBoxSearchKeyDownEvent);
            this.panelCenter.Controls.Add(this.textBoxSearch);

            this.labelSearchQuestion = new Label
            {
                Location = new Point(this.textBoxSearch.Location.X, this.textBoxSearch.Location.Y + this.textBoxSearch.Height),
                Size = this.textBoxSearch.Size,
                MinimumSize = this.textBoxSearch.Size,
                MaximumSize = this.textBoxSearch.Size,

                BackColor = Color.FromArgb(36, 47, 61),
                Font = new Font(Resources.FontName, 13.8F, FontStyle.Regular, GraphicsUnit.Point, 204),
                ForeColor = this.foreColorTextBoxSearch,
                BorderStyle = BorderStyle.FixedSingle,
                TabStop = false
            };
            this.labelSearchQuestion.Visible = false;
            this.labelSearchQuestion.Click += new EventHandler(this.LabelSearchQuestionClickEvent);
            this.panelCenter.Controls.Add(this.labelSearchQuestion);
            

            //PanelContactsBox
            List<Contact> contacts = new List<Contact>
            {
                new Contact("alex"),
                new Contact("grisha"),
                new Contact("server")
            };

            foreach (Contact c in contacts)
            {
                this.dictMessageHistory.Add(c.Id, new List<IMessage>());
            }

            this.panelContactsBox = new PanelContactsBox(contacts, this)
            {
                Location = new Point(0, this.textBoxSearch.Height + 30),
                Width = this.panelCenter.Width,
                Height = 2000,

                BackColor = this.colorPanelCenter
            };
            this.panelCenter.Controls.Add(this.panelContactsBox);
        }

        public void InitializePanelRight()
        {
            this.tableLayoutPanel.ColumnStyles.Add(
                new ColumnStyle()
                {
                    SizeType = SizeType.AutoSize
                });

            // tableLayoutPanelRight
            this.tableLayoutPanelRight = new TableLayoutPanel()
            {
                Location = new Point(0, 0),
                Dock = DockStyle.Fill,
                BackColor = this.colorPanelRight,

                Margin = new Padding(0)
            };
            this.tableLayoutPanel.Controls.Add(this.tableLayoutPanelRight, 2, 0);

            this.tableLayoutPanelRight.RowStyles.Add(
                new RowStyle
                {
                    Height = this.textBoxSearch.Height + 30,
                    SizeType = SizeType.Absolute
                });

            // panelContactInfo
            this.panelContactInfo = new PanelContactInfo("")
            {
                Location = new Point(0, 0),
                Dock = DockStyle.Fill,

                BackColor = this.colorPanelCenter,
                Margin = new Padding(0),
                Visible = false
            };
            this.tableLayoutPanelRight.Controls.Add(this.panelContactInfo, 0, 0);

            this.tableLayoutPanelRight.RowStyles.Add(
                new RowStyle
                {
                    SizeType = SizeType.Percent,
                    Height = 50
                });

            Panel panelTextBoxChat = new Panel
            {
                Dock = DockStyle.Fill,
                //BackgroundImage = Properties.Resources.ImageChatBox_1,
                //BackgroundImageLayout = ImageLayout.Stretch
            };
            this.tableLayoutPanelRight.Controls.Add(panelTextBoxChat, 0, 1);

            // textBoxChat 
            this.textBoxChat = new TextBox
            {
                Dock = DockStyle.Fill,

                BackColor = this.colorPanelLeft,
                Font = new Font(Resources.FontName, 12.8F, FontStyle.Regular, GraphicsUnit.Point, 204),
                ForeColor = Color.White,
                Multiline = true,
                BorderStyle = BorderStyle.None,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                AcceptsReturn = true,
                Margin = new Padding(0),
                TabStop = false
            };
            panelTextBoxChat.Controls.Add(this.textBoxChat);

            this.tableLayoutPanelRight.RowStyles.Add(
                new RowStyle
                {
                    Height = 60,
                    SizeType = SizeType.Absolute
                });

            this.tableLayoutPanelEnter = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = this.colorPanelCenter,
                Margin = new Padding(0),
                Visible = false
            };
            this.tableLayoutPanelRight.Controls.Add(this.tableLayoutPanelEnter, 0, 2);


            this.tableLayoutPanelEnter.ColumnStyles.Add(
                new ColumnStyle
                {
                    SizeType = SizeType.Percent,
                    Width = 100
                });
            this.tableLayoutPanelEnter.ColumnStyles.Add(
               new ColumnStyle
               {
                   Width = 60,
                   SizeType = SizeType.Absolute
               });

            // textBoxEnter
            this.textBoxEnter = new TextBox
            {
                Dock = DockStyle.Fill,

                BackColor = this.colorPanelCenter,
                Font = new Font(Resources.FontName, 13.8F, FontStyle.Regular, GraphicsUnit.Point, 204),
                ForeColor = Color.White,
                Multiline = true,
                BorderStyle = BorderStyle.None,
                Margin = new Padding(0),
                TabStop = false
            };
            this.textBoxEnter.KeyDown += new KeyEventHandler(this.TextBoxEnterKeyDownEvent);
            this.tableLayoutPanelEnter.Controls.Add(this.textBoxEnter, 0, 0);

            // buttonSend
            this.buttonSend = new Button
            {
                Location = new Point(930, 420),
                Size = new Size(55, 55),
                Anchor = AnchorStyles.Right,

                BackColor = this.colorPanelCenter,
                Font = new Font(Resources.FontName, 22.2F, FontStyle.Regular, GraphicsUnit.Point, 204),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                TabStop = false,
                Text = ">",
                Margin = new Padding(0),
                UseVisualStyleBackColor = false
            };
            this.buttonSend.FlatAppearance.BorderSize = 0;
            this.buttonSend.Click += new EventHandler(this.ButtonSendClickEvent);
            this.tableLayoutPanelEnter.Controls.Add(this.buttonSend, 1, 0);
        }

        public void InitializeForm()
        {
            this.MinimumSize = new Size(650, 350);

            this.BackColor = Color.Black;
            this.ClientSize = new Size(997, 487);

            this.Text = "Чатик плачущего котика";
            this.StartPosition = FormStartPosition.CenterScreen;

            this.KeyPreview = true;
            this.FormClosed += new FormClosedEventHandler(this.FormCloseEvent);
        }

        public void AddMessage(string sender, IMessage message)
        {
            this.SuspendLayout();

            if (!this.dictMessageHistory.ContainsKey(sender))
            {
                if (sender == "")
                {
                    sender = "server";
                }
                else
                {
                    this.dictMessageHistory[sender] = new List<IMessage>();
                    this.panelContactsBox.AddContact(new Contact(sender));
                }
            }
            this.dictMessageHistory[sender].Add(message);
            switch (message)
            {
                case TextMessage textMessage:
                    this.panelContactsBox.SetLastMessage(sender, textMessage.IdAuthor, textMessage.Content);

                    if (sender == this.panelContactsBox.CurrentContact.Id)
                    {
                        this.textBoxChat.AppendText($"{textMessage.IdAuthor}: {textMessage.Content}     " +
                                                    $"[{textMessage.Time}]" + Environment.NewLine);
                    }
                    break;
            }

            this.ResumeLayout();
        }

        public void ChangeChat(string idPrevious)
        {
            this.SuspendLayout();

            if (!this.panelContactInfo.Visible)
            {
                this.panelContactInfo.Visible = true;
                this.tableLayoutPanelEnter.Visible = true;
            }

            this.textBoxChat.Clear();
            this.textBoxEnter.Clear();

            if (!this.panelContactsBox.CurrentContact.HistoryReceived)
            {
                try
                {
                    this.client.SendText("", $"history {this.panelContactsBox.CurrentContact.Id}");
                    Thread.Sleep(100);
                    this.panelContactsBox.CurrentContact.HistoryReceived = true;
                }
                catch
                {

                }
            }

            if (this.textBoxEnter.Text.Length != 0 && idPrevious != null)
            {
                this.dictMessageNotFinished.Add(idPrevious, this.textBoxEnter.Text);
            }

            this.textBoxChat.SuspendLayout();
            while (true)
            {
                try
                {
                    StringBuilder builder = new StringBuilder();
                    foreach (IMessage m in this.dictMessageHistory[this.panelContactsBox.CurrentContact.Id])
                    {
                        switch (m)
                        {
                            case TextMessage textMessage:
                                builder.Append($"{textMessage.IdAuthor}: {textMessage.Content}     " +
                                               $"[{m.Time}]{Environment.NewLine}");
                                break;
                            default:
                                break;
                        }
                    }
                    this.textBoxChat.AppendText(builder.ToString());
                    break;
                }
                catch
                {
                    this.textBoxChat.Clear();
                }
            }
            this.textBoxChat.ResumeLayout();

            
            if (this.dictMessageNotFinished.ContainsKey(this.panelContactsBox.CurrentContact.Id))
            {
                this.textBoxEnter.AppendText(this.dictMessageNotFinished[this.panelContactsBox.CurrentContact.Id]);
                this.dictMessageNotFinished.Remove(this.panelContactsBox.CurrentContact.Id);
            }


            this.panelContactInfo.ChangeContact(this.panelContactsBox.CurrentContact.Id);

            this.ResumeLayout();
        }

        public void AddContactQuestion(string id)
        {
            this.labelSearchQuestion.Visible = true;
            this.labelSearchQuestion.Text = id;
        }

        public Dictionary<string, List<IMessage>> GetHistory()
        {
            return this.dictMessageHistory;
        }

        public void SetHistory(string id, List<IMessage> messages)
        {
            this.dictMessageHistory[id] = messages;

            switch (messages[^1])
            {
                case TextMessage textMessage:
                    this.panelContactsBox.SetLastMessage(
                        id, textMessage.IdAuthor, textMessage.Content);
                    break;
                case FileMessage fileMessage:
                    this.panelContactsBox.SetLastMessage(
                        id, fileMessage.IdAuthor, fileMessage.File.Name);
                    break;
                case VoiceMessage voiceMessage:
                    this.panelContactsBox.SetLastMessage(
                        id, voiceMessage.IdAuthor, "Audio message");
                    break;
            }
        }

        private void SendNotSendedMessages()
        {
            foreach (string id in this.dictMessagesNotSended.Keys)
            {
                if (this.dictMessagesNotSended[id].Count != 0)
                {
                    foreach (IMessage message in this.dictMessagesNotSended[id])
                    {
                        try
                        {
                            switch (message)
                            {
                                case TextMessage textMessage:
                                    this.client.SendText(textMessage.IdReceiver, textMessage.Content);
                                    break;
                                case FileMessage fileMessage:
                                    byte[] buffer = new byte[1024];
                                    fileMessage.File.Read(buffer);
                                    this.client.SendFile(fileMessage.IdReceiver, buffer);
                                    break;
                                case VoiceMessage voiceMessage:
                                    this.client.SendVoice(voiceMessage.IdReceiver, voiceMessage.Content);
                                    break;
                            }
                            
                            this.dictMessagesNotSended[id].Remove(message);
                        }
                        catch
                        {

                        }
                    }
                }
            }
        }

        private void OnTimerTick(object source, ElapsedEventArgs e)
        {
            //this.SendNotSendedMessages();

            if (!this.client.IsConnected)
            {
                try
                {
                    this.client.Start();

                    this.timerTemp = new System.Timers.Timer(100);
                    this.timerTemp.Elapsed += new ElapsedEventHandler(this.OnTimerTempTick);
                    this.timerTemp.AutoReset = true;
                    this.timerTemp.Enabled = true;
                }
                catch
                {

                }
            }

            if (this.labelCpu.Visible)
            {
                int valueCpu = (int)this.cpuCounter.NextValue();
                this.labelCpu.Text = $"CPU  {valueCpu}%";
                if (valueCpu < 40)
                {
                    this.labelCpu.ForeColor = Color.Lime;
                }
                else if (valueCpu < 60)
                {
                    this.labelCpu.ForeColor = Color.Yellow;
                }
                else if (valueCpu < 80)
                {
                    this.labelCpu.ForeColor = Color.Orange;
                }
                else
                {
                    this.labelCpu.ForeColor = Color.Red;
                }

                int valueRam = (int)(100 - (double)ramCounter.NextValue() / (this.ramTotalAmount / 1024) * 100);
                this.labelRam.Text = $"RAM  {valueRam}%";
                if (valueRam < 40)
                {
                    this.labelRam.ForeColor = Color.Lime;
                }
                else if (valueRam < 60)
                {
                    this.labelRam.ForeColor = Color.Yellow;
                }
                else if (valueRam < 80)
                {
                    this.labelRam.ForeColor = Color.Orange;
                }
                else
                {
                    this.labelRam.ForeColor = Color.Red;
                }

                long time = this.ping.Send("192.168.88.123").RoundtripTime; // 192.168.88.123 // www.google.com
                this.labelPing.Text = $"PING {time}ms";
                if (time < 10)
                {
                    this.labelPing.ForeColor = Color.Lime;
                }
                else if (time < 20)
                {
                    this.labelPing.ForeColor = Color.Yellow;
                }
                else if (time < 50)
                {
                    this.labelPing.ForeColor = Color.Orange;
                }
                else
                {
                    this.labelPing.ForeColor = Color.Red;
                }
            }
        }

        private void OnTimerTempTick(object source, ElapsedEventArgs e)
        {
            foreach (Contact contact in this.panelContactsBox.Contacts)
            {
                if (!contact.HistoryReceived)
                {
                    //this.client.SendMessage("", $"history {contact.Id}");
                    contact.HistoryReceived = true;
                    return;
                }
            }

            this.timerTemp.Stop();
            this.timerTemp.Dispose();
        }

        private void FormCloseEvent(object sender, EventArgs e)
        {
            this.Hide();

            try
            {
                this.client.Disconnect();
            }
            catch
            {

            }
        }

        private void ButtonSendClickEvent(object sender, EventArgs e)
        {
            this.SuspendLayout();

            string content = this.textBoxEnter.Text;
            if (content.Length != 0)
            {
                IMessage message = new TextMessage(this.panelContactsBox.CurrentContact.Id, this.client.Id, DateTime.Now, content);

                try
                {
                    this.client.SendText(message.IdReceiver, content);
                }
                catch
                {
                    if (!this.dictMessagesNotSended.ContainsKey(this.panelContactsBox.CurrentContact.Id))
                    {
                        this.dictMessagesNotSended.Add(
                            this.panelContactsBox.CurrentContact.Id, new List<IMessage>());
                    }
                    this.dictMessagesNotSended[this.panelContactsBox.CurrentContact.Id].Add(message);
                }

                this.textBoxChat.AppendText($"{this.client.Id}: {content} [{DateTime.Now}]{Environment.NewLine}");
                this.dictMessageHistory[this.panelContactsBox.CurrentContact.Id].Add(message);
                this.textBoxEnter.Clear();
                this.panelContactsBox.SetLastMessage(this.panelContactsBox.CurrentContact.Id, this.client.Id, content);

                this.panelContactsBox.SetFirst(this.panelContactsBox.CurrentContact);
            }
        }

        private void TextBoxSearchTextChanged(object sender, EventArgs e)
        {
            //this.textBoxSearch.Text = $"*{e.KeyCode.ToString()}*";
            //if (this.textBoxSearch.Text.Contains(Environment.NewLine))
            //{
            //    this.textBoxSearch.Text = this.textBoxSearch.Text.Substring(0, this.textBoxSearch.Text.Length - 1);
            //    this.client.SendMessage("", $"exist {this.textBoxSearch.Text}");
            //}
        }

        private void TextBoxSearchEnterEvent(object sender, EventArgs e)
        {
            //this.textBoxSearch.Text = this.textBoxSearch.Text.Substring(0, this.textBoxSearch.Text.Length - 1);
            //this.client.SendMessage("", $"exist {this.textBoxSearch.Text}");
        }

        private void TextBoxSearchGotFocus(object sender, EventArgs e)
        {
            /*if (this.textBoxSearch.Text == "Search...")
            {
                this.textBoxSearch.Text = "";
            }
            this.textBoxSearch.ForeColor = this.foreColorTextBoxSearchActive;*/
        }

        private void TextBoxSearchLostFocus(object sender, EventArgs e)
        {
            /*if (this.textBoxSearch.Text == "")
            {
                this.textBoxSearch.Text = "Search...";
                this.textBoxSearch.ForeColor = this.foreColorTextBoxSearch;
            }
            else if (this.textBoxSearch.Text == "Search...")
            {
                this.textBoxSearch.ForeColor = this.foreColorTextBoxSearch;
            }*/
        }

        private void LabelSearchQuestionClickEvent(object sender, EventArgs e)
        {
            //this.labelSearchQuestion.Visible = false;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.Control && e.KeyCode == Keys.I)
            {
                if (this.labelCpu.Visible)
                {
                    this.labelCpu.Visible = false;
                    this.labelRam.Visible = false;
                    this.labelPing.Visible = false;
                }
                else
                {
                    this.labelCpu.Visible = true;
                    this.labelRam.Visible = true;
                    this.labelPing.Visible = true;
                }
            }
            else if (e.KeyCode == Keys.F5)
            {
                this.PerformLayout();
            }
        }

        private void TextBoxEnterKeyDownEvent(object sender, KeyEventArgs e)
        {
            if (e.Shift && e.KeyCode == Keys.Return)
            {
                this.textBoxEnter.AppendText(Environment.NewLine);
            }
            else if (e.KeyCode == Keys.Return)
            {
                if (this.textBoxEnter.Text.Length != 0)
                {
                    this.textBoxEnter.Text = this.textBoxEnter.Text.Substring(0, this.textBoxEnter.Text.Length - 1);
                }
                this.ButtonSendClickEvent(this.buttonSend, null);
            }
        }

        private void TextBoxSearchKeyDownEvent(object sender, KeyEventArgs e)
        {
            /*this.textBoxSearch.SuspendLayout();

            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                this.client.SendMessage("", $"exist {this.textBoxSearch.Text}");
            }

            this.textBoxSearch.ResumeLayout();*/
        }
    }
}