using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using Client.Common;
using Client.Network;
using Client.UserInterface.Controls;
using Client.UserInterface.Styles;
using Network.Message;
using Network.Message.ExchangingMessages;

namespace Client.UserInterface
{
    public class ClientForm : Form, IClientForm
    {
        private TableLayoutPanel tableLayoutPanel;

        private Panel panelLeft;
        private PictureBox pictureBoxProfile;
        private PictureBox pictureBoxChats;
        private PictureBox pictureBoxSettings;
        private readonly PerformanceCounter cpuCounter;
        private Label labelCpu;
        private readonly PerformanceCounter ramCounter;
        private readonly ulong ramTotalAmount;
        private Label labelRam;
        
        private Panel panelCenter;
        private PictureBox pictureBoxSearch;
        private TextBox textBoxSearch;
        private Label labelSearchQuestion;
        private PanelContactsBox panelContactsBox;
        private static readonly Color ForeColorTextBoxSearch = Color.DarkGray;
        private static readonly Color ForeColorTextBoxSearchActive = Color.White;

        private TableLayoutPanel tableLayoutPanelRight;
        private PanelContactInfo panelContactInfo;
        private PanelChatBox panelChatBox;
        private TableLayoutPanel tableLayoutPanelEnter;
        private TextBox textBoxEnter;
        private PictureBox pictureBoxSend;

        public static Style Style;

        private TextBox textBoxTemp;

        private readonly System.Timers.Timer timer;
        private System.Timers.Timer timerTemp;

        private static string pathResources = "..\\..\\..\\Resources\\";

        private readonly ClientObject client;
        private readonly Dictionary<string, List<IMessage>> dictMessageHistory;
        private readonly Dictionary<string, List<IMessage>> dictMessagesNotSent;
        private readonly Dictionary<string, string> dictMessageNotFinished;

        public ClientForm()
        {
            this.dictMessageHistory = new Dictionary<string, List<IMessage>>();
            this.dictMessagesNotSent = new Dictionary<string, List<IMessage>>();
            this.dictMessageNotFinished = new Dictionary<string, string>();

            Style = new DarkStyle(pathResources);

            this.cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            this.ramCounter = new PerformanceCounter("Memory", "Available MBytes");

            ManagementObjectSearcher ramMonitor = new ManagementObjectSearcher(
                "SELECT TotalVisibleMemorySize,FreePhysicalMemory FROM Win32_OperatingSystem");
            foreach (var objram in ramMonitor.Get())
            {
                this.ramTotalAmount = Convert.ToUInt64(objram["TotalVisibleMemorySize"]);    //общая память ОЗУ
            }

            this.InitializeComponent();
            this.PerformLayout();

            try
            {
                this.client = new ClientObject(Resources.Id);
                this.client.Start();

                this.client.OnTextMessageReceive += this.OnTextMessageReceive;
                this.client.OnHistoryReceive += this.OnHistoryReceive;
                this.client.OnUsersListAnswerReceive += this.OnUsersListAnswerReceive;
            }
            catch
            {
                // ignored
            }

            this.timer = new System.Timers.Timer(300);
            this.timer.Elapsed += this.OnTimerTick;
            this.timer.AutoReset = true;
            this.timer.Enabled = true;

            if (this.client.IsConnected)
            {
                this.timerTemp = new System.Timers.Timer(100);
                this.timerTemp.Elapsed += this.OnTimerTempTick;
                this.timerTemp.AutoReset = true;
                this.timerTemp.Enabled = true;
            }
        }

        private void InitializeComponent()
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

        private void InitializePanelLeft()
        {
            var width = 75;
            this.tableLayoutPanel.ColumnStyles.Add(
                new ColumnStyle()
                {
                    Width = width,
                    SizeType = SizeType.Absolute
                });

            // panelLeft
            this.panelLeft = new Panel
            {
                Location = new Point(0, 0),
                Dock = DockStyle.Fill,
                BackColor = Style.BackColorMain,
                Margin = new Padding(1),
                //BorderStyle = BorderStyle.FixedSingle
            };
            this.tableLayoutPanel.Controls.Add(this.panelLeft, 0, 0);

            var pictureBoxSize = 40;
            var padding = 20;
            this.pictureBoxProfile = new PictureBox
            {
                Location = new Point((width - pictureBoxSize) / 2, (width - pictureBoxSize) / 2),
                Size = new Size(pictureBoxSize, pictureBoxSize),

                BackColor = Style.BackColorMain,
                TabStop = false,
                Margin = new Padding(0),
                Image = Style.GetImage("profile", false),
                SizeMode = PictureBoxSizeMode.Zoom
            };
            this.pictureBoxProfile.MouseEnter += PictureBoxesEnterEvent;
            this.pictureBoxProfile.MouseLeave += PictureBoxesLeaveEvent;
            this.panelLeft.Controls.Add(this.pictureBoxProfile);
            
            this.pictureBoxChats = new PictureBox
            {
                Location = new Point((width - pictureBoxSize) / 2, 
                    (width - pictureBoxSize) / 2 + pictureBoxSize + padding),
                Size = new Size(pictureBoxSize, pictureBoxSize),

                BackColor = Style.BackColorMain,
                TabStop = false,
                Margin = new Padding(0),
                Image = Style.GetImage("chats", false),
                SizeMode = PictureBoxSizeMode.Zoom
            };
            this.pictureBoxChats.MouseEnter += PictureBoxesEnterEvent;
            this.pictureBoxChats.MouseLeave += PictureBoxesLeaveEvent;
            this.panelLeft.Controls.Add(this.pictureBoxChats);

            this.pictureBoxSettings = new PictureBox
            {
                Location = new Point((width - pictureBoxSize) / 2, 
                    (width - pictureBoxSize) / 2 + 2 * (pictureBoxSize + padding)),
                Size = new Size(pictureBoxSize, pictureBoxSize),

                BackColor = Style.BackColorMain,
                TabStop = false,
                Margin = new Padding(0),
                Image = Style.GetImage("settings", false),
                SizeMode = PictureBoxSizeMode.Zoom
            };
            this.pictureBoxSettings.MouseEnter += PictureBoxesEnterEvent;
            this.pictureBoxSettings.MouseLeave += PictureBoxesLeaveEvent;
            this.panelLeft.Controls.Add(this.pictureBoxSettings);

            this.labelCpu = new Label
            {
                Dock = DockStyle.Bottom,

                BackColor = Style.BackColorMain,
                ForeColor = Color.White,
                Font = new Font(Resources.FontName, 8F, FontStyle.Regular, GraphicsUnit.Point, 204),
                TextAlign = ContentAlignment.MiddleLeft
            };
            this.panelLeft.Controls.Add(this.labelCpu);

            this.labelRam = new Label
            {
                Dock = DockStyle.Bottom,

                BackColor = Style.BackColorMain,
                ForeColor = Style.ForeColorMain,
                Font = new Font(Resources.FontName, 8F, FontStyle.Regular, GraphicsUnit.Point, 204),
                TextAlign = ContentAlignment.MiddleLeft
            };
            this.panelLeft.Controls.Add(this.labelRam);
        }

        private void InitializePanelCenter()
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
                BackColor = Style.BackColorSecondary,
                Margin = new Padding(1),
                //BorderStyle = BorderStyle.FixedSingle
            };
            this.tableLayoutPanel.Controls.Add(this.panelCenter, 1, 0);

            this.pictureBoxSearch = new PictureBox
            {
                Location = new Point(10, 18),
                Size = new Size(25, 25),

                BackColor = Style.BackColorSecondary,
                TabStop = false,
                Image = Style.GetImage("search", false),
                SizeMode = PictureBoxSizeMode.Zoom
            };
            this.pictureBoxSearch.MouseEnter += PictureBoxesEnterEvent;
            this.pictureBoxSearch.MouseLeave += PictureBoxesLeaveEvent;
            //this.pictureBoxSearch.Click += this.PictureBoxSearchClickEvent;
            this.panelCenter.Controls.Add(pictureBoxSearch);

            // textBoxSearch
            this.textBoxSearch = new TextBox()
            {
                Location = new Point(45, 15),
                Anchor = AnchorStyles.Right | AnchorStyles.Top,
                Size = new Size((int)this.tableLayoutPanel.ColumnStyles[1].Width - 30 - 25, 76 - 30),

                BackColor = Color.FromArgb(36, 47, 61),
                Text = "Search...",
                Font = new Font(Resources.FontName, 11.8F, FontStyle.Regular, GraphicsUnit.Point, 204),
                ForeColor = Style.ForeColorSecondary,
                BorderStyle = BorderStyle.FixedSingle,
                //Multiline = true,
                //AcceptsReturn = true,
                TabStop = false
            };
            //this.textBoxSearch.TextChanged += new EventHandler(this.TextBoxSearchTextChanged);
            this.textBoxSearch.Enter += this.TextBoxSearchEnterEvent;
            this.textBoxSearch.GotFocus += this.TextBoxSearchGotFocus;
            this.textBoxSearch.LostFocus += this.TextBoxSearchLostFocus;
            this.textBoxSearch.KeyDown += this.TextBoxSearchKeyDownEvent;
            this.panelCenter.Controls.Add(this.textBoxSearch);

            this.labelSearchQuestion = new Label
            {
                Location = new Point(this.textBoxSearch.Location.X,
                    this.textBoxSearch.Location.Y + this.textBoxSearch.Height),
                Size = this.textBoxSearch.Size,
                MinimumSize = this.textBoxSearch.Size,
                MaximumSize = this.textBoxSearch.Size,
                BackColor = Color.FromArgb(36, 47, 61),
                Font = new Font(Resources.FontName, 13.8F, FontStyle.Regular, GraphicsUnit.Point, 204),
                ForeColor = Style.ForeColorSecondary,
                BorderStyle = BorderStyle.FixedSingle,
                TabStop = false,
                Visible = false
            };
            this.labelSearchQuestion.Click += this.LabelSearchQuestionClickEvent;
            this.panelCenter.Controls.Add(this.labelSearchQuestion);
            

            //PanelContactsBox
            var contacts = new List<Contact>
            {
                new Contact("artem"),
                new Contact("grisha"),
                new Contact("julia"),
                new Contact("vova"),
                new Contact("test5"),
                new Contact("test6"),
                new Contact("test7"), 
                new Contact("test8")
            };

            foreach (var c in contacts)
            {
                this.dictMessageHistory.Add(c.Id, new List<IMessage>());
            }

            this.panelContactsBox = new PanelContactsBox(contacts, this)
            {
                Location = new Point(0, this.textBoxSearch.Height + 30),
                Width = this.panelCenter.Width,
                Height = 2000,
                Margin = new Padding(0),
                BackColor = Style.BackColorSecondary,
                AutoScroll = true
            };
            this.panelCenter.Controls.Add(this.panelContactsBox);
        }

        private void InitializePanelRight()
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
                BackColor = Style.BackColorMain,
                //BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(1)
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

                BackColor = Style.BackColorSecondary,
                Margin = new Padding(0),
                //BorderStyle = BorderStyle.FixedSingle,
                Visible = false
            };
            this.tableLayoutPanelRight.Controls.Add(this.panelContactInfo, 0, 0);

            this.tableLayoutPanelRight.RowStyles.Add(
                new RowStyle
                {
                    SizeType = SizeType.Percent,
                    Height = 50
                });

            this.panelChatBox = new PanelChatBox
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.None,
                Margin = new Padding(0),
                TabStop = false,
                Visible = false,
                AutoScroll = true
            };
            this.tableLayoutPanelRight.Controls.Add(this.panelChatBox, 0, 1);

            this.tableLayoutPanelRight.RowStyles.Add(
                new RowStyle
                {
                    Height = 55,
                    SizeType = SizeType.Absolute
                });

            this.tableLayoutPanelEnter = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Style.BackColorSecondary,
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
                   Width = 55,
                   SizeType = SizeType.Absolute
               });

            // textBoxEnter
            this.textBoxEnter = new TextBox
            {
                Dock = DockStyle.Fill,

                BackColor = Style.BackColorSecondary,
                Font = new Font(Resources.FontName, 13.8F, FontStyle.Regular, GraphicsUnit.Point, 204),
                ForeColor = Style.ForeColorResidual,
                Multiline = true,
                BorderStyle = BorderStyle.None,
                Margin = new Padding(0),
                TabStop = false,
                ScrollBars = ScrollBars.Vertical
            };
            this.textBoxEnter.KeyDown += this.TextBoxEnterKeyDownEvent;
            this.tableLayoutPanelEnter.Controls.Add(this.textBoxEnter, 0, 0);

            var panelTemp = new Panel
            {
                Location = new Point(940, 430),
                Size = new Size(55, 55),
                BackColor = Style.BackColorSecondary,
                TabStop = false,
                Margin = new Padding(0),
                //BorderStyle = BorderStyle.FixedSingle
            };
            this.tableLayoutPanelEnter.Controls.Add(panelTemp, 1, 0);
            
            this.pictureBoxSend = new PictureBox
            {
                Location = new Point(15, 15),
                Size = new Size(23, 23),

                BackColor = Style.BackColorSecondary,
                TabStop = false,
                Image = Style.GetImage("send", false),
                SizeMode = PictureBoxSizeMode.Zoom
            };
            this.pictureBoxSend.MouseEnter += PictureBoxesEnterEvent;
            this.pictureBoxSend.MouseLeave += PictureBoxesLeaveEvent;
            this.pictureBoxSend.Click += this.PictureBoxSendClickEvent;
            panelTemp.Controls.Add(this.pictureBoxSend);
        }

        private void InitializeForm()
        {
            this.textBoxTemp = new TextBox
            {
                Location = new Point(0, 0),
                Size = new Size(25, 25)
            };
            this.Controls.Add(this.textBoxTemp);
            
            this.MinimumSize = new Size(650, 350);

            this.BackColor = Color.Black;
            this.ClientSize = new Size(997, 487);

            this.Text = "Чатик плачущего котика";
            this.StartPosition = FormStartPosition.CenterScreen;

            this.KeyPreview = true;
            this.FormClosed += this.FormCloseEvent;
        }

        public void AddMessage(string idSender, IMessage message)
        {
            this.SuspendLayout();

            if (!this.dictMessageHistory.ContainsKey(idSender))
            {
                if (idSender == "")
                {
                    idSender = "server";
                }
                else
                {
                    this.dictMessageHistory[idSender] = new List<IMessage>();
                    this.panelContactsBox.AddContact(new Contact(idSender));
                }
            }
            this.dictMessageHistory[idSender].Add(message);
            switch (message)
            {
                case TextMessage textMessage:
                    this.panelContactsBox.SetLastMessage(idSender, textMessage.IdAuthor, textMessage.Content);

                    if (idSender == this.panelContactsBox.CurrentContact.Id)
                    {
                        this.panelChatBox.AddMessage(textMessage);
                    }
                    break;
                case FileMessage:
                    break;
                case VoiceMessage:
                    break;
            }

            this.ResumeLayout();
        }

        public async void ChangeChat(string idPrevious)
        {
            this.textBoxTemp.Focus();

            if (!this.panelContactInfo.Visible)
            {
                this.panelChatBox.Visible = true;
                this.panelContactInfo.Visible = true;
                this.tableLayoutPanelEnter.Visible = true;
            }

            this.panelChatBox.Clear();
            this.textBoxEnter.Clear();

            Console.WriteLine(this.panelContactsBox.CurrentContact.HistoryReceived);
            if (!this.panelContactsBox.CurrentContact.HistoryReceived)
            {
                try
                {
                    Console.WriteLine("Request");
                    this.client.RequestHistory(this.panelContactsBox.CurrentContact.Id);
                }
                catch
                {
                    // ignored
                }
            }

            if (this.textBoxEnter.Text.Length != 0 && idPrevious != null)
            {
                this.dictMessageNotFinished.Add(idPrevious, this.textBoxEnter.Text);
            }

            Thread.Sleep(100);
            this.panelContactsBox.CurrentContact.HistoryReceived = true;
            //this.panelChatBox.Visible = false;
            while (true)
            {
                try
                {
                    foreach (var m in this.dictMessageHistory[this.panelContactsBox.CurrentContact.Id])
                    {
                        this.panelChatBox.SuspendLayout();
                        switch (m)
                        {
                            case TextMessage textMessage:
                                this.panelChatBox.AddMessage(textMessage);
                                break;
                            
                        }
                        this.panelChatBox.ResumeLayout();
                    }
                    
                    break;
                }
                catch
                {
                    this.panelChatBox.Clear();
                }
            }
            //this.panelChatBox.Visible = true;
            
            if (this.dictMessageNotFinished.ContainsKey(this.panelContactsBox.CurrentContact.Id))
            {
                this.textBoxEnter.AppendText(this.dictMessageNotFinished[this.panelContactsBox.CurrentContact.Id]);
                this.dictMessageNotFinished.Remove(this.panelContactsBox.CurrentContact.Id);
            }
            
            this.panelContactInfo.ChangeContact(this.panelContactsBox.CurrentContact.Id);
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

        private async void SendNotSentMessages()
        {
            foreach (var id in this.dictMessagesNotSent.Keys)
            {
                if (this.dictMessagesNotSent[id].Count != 0)
                {
                    foreach (var message in this.dictMessagesNotSent[id])
                    {
                        try
                        {
                            switch (message)
                            {
                                case TextMessage textMessage:
                                    await this.client.SendText(textMessage.IdReceiver, textMessage.Content);
                                    break;
                                case FileMessage fileMessage:
                                    byte[] buffer = new byte[1024];
                                    fileMessage.File.Read(buffer);
                                    await this.client.SendFile(fileMessage.IdReceiver, buffer);
                                    break;
                                case VoiceMessage voiceMessage:
                                    await this.client.SendVoice(voiceMessage.IdReceiver, voiceMessage.Content);
                                    break;
                            }
                            
                            this.dictMessagesNotSent[id].Remove(message);
                        }
                        catch
                        {
                            // ignored
                        }
                    }
                }
            }
        }
        
        [DllImport("user32.dll")]
        public static extern int FlashWindow(IntPtr Hwnd, bool Revert);

        private void OnTimerTick(object source, ElapsedEventArgs e)
        {
            //this.SendNotSentMessages();

            if (!this.client.IsConnected)
            {
                try
                {
                    this.client.Start();

                    this.timerTemp = new System.Timers.Timer(100);
                    this.timerTemp.Elapsed += this.OnTimerTempTick;
                    this.timerTemp.AutoReset = true;
                    this.timerTemp.Enabled = true;
                }
                catch
                {
                    // ignored
                }
            }

            if (this.labelCpu.Visible)
            {
                var valueCpu = (int)this.cpuCounter.NextValue();
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

                var valueRam = (int)(100 - (double)ramCounter.NextValue() / ((double)this.ramTotalAmount / 1024) * 100);
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
            }
        }

        private void OnTimerTempTick(object source, ElapsedEventArgs e)
        {
            foreach (var contact in this.panelContactsBox.Contacts)
            {
                if (!contact.HistoryReceived)
                {
                    //this.client.SendMessage("", $"history {contact.Id}");
                    //contact.HistoryReceived = true;
                    //return;
                }
            }

            this.timerTemp.Stop();
            this.timerTemp.Dispose();
        }

        private async void FormCloseEvent(object sender, EventArgs e)
        {
            this.Hide();

            try
            {
                await this.client.Disconnect();
            }
            catch
            {
                // ignored
            }
        }
        
        private void PictureBoxesEnterEvent(object sender, EventArgs e)
        {
            this.SuspendLayout();

            PictureBox pictureBox = sender as PictureBox;
            if (pictureBox == this.pictureBoxProfile)
            {
                pictureBox.Image = Style.GetImage("profile", true);
            }
            else if (pictureBox == this.pictureBoxChats)
            {
                pictureBox.Image = Style.GetImage("chats", true);
            }
            else if (pictureBox == this.pictureBoxSettings)
            {
                pictureBox.Image = Style.GetImage("settings", true);
            }
            else if (pictureBox == this.pictureBoxSearch)
            {
                pictureBox.Image = Style.GetImage("search", true);
            }
            else if (pictureBox == this.pictureBoxSend)
            {
                pictureBox.Image = Style.GetImage("send", true);
            }
            
            this.ResumeLayout();
        }
        
        private void PictureBoxesLeaveEvent(object sender, EventArgs e)
        {
            this.SuspendLayout();

            PictureBox pictureBox = sender as PictureBox;
            if (pictureBox == this.pictureBoxProfile)
            {
                pictureBox.Image = Style.GetImage("profile", false);
            }
            else if (pictureBox == this.pictureBoxChats)
            {
                pictureBox.Image = Style.GetImage("chats", false);
            }
            else if (pictureBox == this.pictureBoxSettings)
            {
                pictureBox.Image = Style.GetImage("settings", false);
            }
            else if (pictureBox == this.pictureBoxSearch)
            {
                pictureBox.Image = Style.GetImage("search", false);
            }
            else if (pictureBox == this.pictureBoxSend)
            {
                pictureBox.Image = Style.GetImage("send", false);
            }

            this.ResumeLayout();
        }

        private async void PictureBoxSendClickEvent(object sender, EventArgs e)
        {
            this.SuspendLayout();

            var content = this.textBoxEnter.Text;
            this.textBoxEnter.Clear();
            
            while (content.StartsWith(Environment.NewLine))
            {
                content = content.Substring(Environment.NewLine.Length, content.Length - Environment.NewLine.Length);
            }
            while (content.EndsWith(Environment.NewLine))
            {
                content = content.Substring(0, content.Length - Environment.NewLine.Length);
            }
            
            if (content.Length != 0)
            {
                int maxSize = 1024;
                for (int i = 0; i < content.Length / maxSize + 1; i++)
                {
                    int indexEnd = i != content.Length / maxSize ? (i + 1) * maxSize : content.Length;
                    string cont = content.Substring(i * maxSize, indexEnd - i * maxSize);

                    if (cont.Length == 0)
                    {
                        break;
                    }
                    
                    IMessage message = new TextMessage(this.panelContactsBox.CurrentContact.Id, this.client.Id, DateTime.Now, cont);

                    try
                    {
                        await this.client.SendText(message.IdReceiver, cont);
                    }
                    catch
                    {
                        if (!this.dictMessagesNotSent.ContainsKey(this.panelContactsBox.CurrentContact.Id))
                        {
                            this.dictMessagesNotSent.Add(
                                this.panelContactsBox.CurrentContact.Id, new List<IMessage>());
                        }
                        this.dictMessagesNotSent[this.panelContactsBox.CurrentContact.Id].Add(message);
                    }

                    this.SuspendLayout();
                    this.panelChatBox.AddMessage(message);
                    this.ResumeLayout();
                    
                    this.dictMessageHistory[this.panelContactsBox.CurrentContact.Id].Add(message);
                    this.panelContactsBox.SetLastMessage(this.panelContactsBox.CurrentContact.Id, this.client.Id, content);
                }
                
                this.panelContactsBox.SetFirst(this.panelContactsBox.CurrentContact);
            }
            
            this.ResumeLayout();
        }

        private async void TextBoxSearchTextChanged(object sender, EventArgs e)
        {
            if (this.textBoxSearch.Text.Contains(Environment.NewLine))
            {
                this.textBoxSearch.Text = this.textBoxSearch.Text.Substring(0, this.textBoxSearch.Text.Length - 1);
                await this.client.SendSearchRequest(this.textBoxSearch.Text);
            }
        }

        private async void TextBoxSearchEnterEvent(object sender, EventArgs e)
        {
            /*this.textBoxSearch.Text = this.textBoxSearch.Text.Substring(0, this.textBoxSearch.Text.Length - 1);
            await this.client.SendSearchRequest(this.textBoxSearch.Text);*/
        }

        private void TextBoxSearchGotFocus(object sender, EventArgs e)
        {
            if (this.textBoxSearch.Text == "Search...")
            {
                this.textBoxSearch.Text = "";
            }
            this.textBoxSearch.ForeColor = Style.ForeColorMain;
        }

        private void TextBoxSearchLostFocus(object sender, EventArgs e)
        {
            if (this.textBoxSearch.Text == "")
            {
                this.textBoxSearch.Text = "Search...";
                this.textBoxSearch.ForeColor = Style.ForeColorSecondary;
            }
            else if (this.textBoxSearch.Text == "Search...")
            {
                this.textBoxSearch.ForeColor = Style.ForeColorSecondary;
            }
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
                }
                else
                {
                    this.labelCpu.Visible = true;
                    this.labelRam.Visible = true;
                }
            }
            else if (e.KeyCode == Keys.F5)
            {
                this.PerformLayout();
            }
            else if (e.KeyCode == Keys.Escape)
            {
                this.panelChatBox.Visible = false;
                this.panelContactInfo.Visible = false;
                this.tableLayoutPanelEnter.Visible = false;
                this.panelContactsBox.RemoveSelection();
            }
        }

        private void TextBoxEnterKeyDownEvent(object sender, KeyEventArgs e)
        {
            if (e.Shift && e.KeyCode == Keys.Return)
            {

            }
            else if (e.KeyCode == Keys.Return)
            {
                this.PictureBoxSendClickEvent(this.pictureBoxSend, null);
                this.textBoxEnter.Clear();
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

        private void OnTextMessageReceive(string idSender, TextMessage message)
        {
            this.AddMessage(idSender, message);
        }

        private void OnHistoryReceive(string id, IEnumerable<IMessage> messages)
        {
            this.SetHistory(id, messages.ToList());
        }
        
        private void OnUsersListAnswerReceive(List<string> users)
        {
            
        }
    }
}