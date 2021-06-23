using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Client.Common;
using Network.Message;
using Network.Message.ExchangingMessages;

namespace Client.UserInterface.Controls
{
    public class PanelContactsBox : Panel
    {
        public List<Contact> Contacts { get; }
        public readonly Dictionary<string, PanelContact> dictionaryPanelContacts;
        private readonly List<PanelContact> listPanelContacts;
        private readonly IClientForm form;

        private readonly int heightLabel = 70;

        public Contact CurrentContact = null;

        public PanelContactsBox(List<Contact> listContacts, IClientForm form)
        {
            this.Contacts = listContacts;
            this.dictionaryPanelContacts = new Dictionary<string, PanelContact>();
            this.listPanelContacts = new List<PanelContact>();

            this.form = form;

            this.PaintContacts();
        }

        public void AddContact(Contact contact)
        {
            this.Contacts.Add(contact);

            PanelContact panelContact = new PanelContact(this.Contacts[^1].Id)
            {
                Location = new Point(0, this.heightLabel * (this.Contacts.Count - 1)),
                Size = new Size(240, this.heightLabel),

                BackColor = ClientForm.Style.BackColorSecondary
            };
            panelContact.Click += new EventHandler(
                (sender, e) => 
                {
                    this.SuspendLayout();

                    string idPrevious = null;
                    if (this.CurrentContact != null)
                    {
                        idPrevious = this.CurrentContact.Id;

                        this.listPanelContacts[this.Contacts.IndexOf(this.CurrentContact)].labelId.BackColor = ClientForm.Style.BackColorSecondary;
                        this.listPanelContacts[this.Contacts.IndexOf(this.CurrentContact)].labelLastMessage.BackColor = ClientForm.Style.BackColorSecondary;
                        this.listPanelContacts[this.Contacts.IndexOf(this.CurrentContact)].BackColor = ClientForm.Style.BackColorSecondary;

                        this.listPanelContacts[this.Contacts.IndexOf(this.CurrentContact)].Selected = false;
                    }

                    this.CurrentContact = this.Contacts[this.listPanelContacts.IndexOf((PanelContact)sender)];

                    ((PanelContact)sender).labelId.BackColor = ClientForm.Style.BackColorSelected;
                    ((PanelContact)sender).labelLastMessage.BackColor = ClientForm.Style.BackColorSelected;
                    ((PanelContact)sender).BackColor = ClientForm.Style.BackColorSelected;
                    ((PanelContact)sender).Selected = true;

                    this.form.ChangeChat(idPrevious);

                    this.ResumeLayout();
                });

            this.form.dictMessageHistory.Add(contact.Id, new List<TextMessage>());
            this.Controls.Add(panelContact);
            this.listPanelContacts.Add(panelContact);
            this.dictionaryPanelContacts.Add(contact.Id, panelContact);
        }

        public void RemoveContact(Contact contact)
        {
            this.listPanelContacts.RemoveAt(this.Contacts.IndexOf(contact));
            this.Contacts.Remove(contact);
        }

        public void RemoveSelection()
        {
            this.dictionaryPanelContacts[this.CurrentContact.Id].RemoveSelection();
            this.CurrentContact = null;
        }

        public void SetFirst(Contact contact)
        {
            string idPrevious = this.CurrentContact.Id;

            int index = this.Contacts.IndexOf(contact);
            this.Contacts.Remove(contact);

            PanelContact temp = this.dictionaryPanelContacts[contact.Id];
            this.listPanelContacts.Remove(temp);

            this.Contacts.Insert(0, contact);
            this.listPanelContacts.Insert(0, temp);

            for (int i = 1; i < index + 1; i++)
            {
                this.listPanelContacts[i].Location = new Point(0, this.heightLabel * i);
            }
            this.listPanelContacts[0].Location = new Point(0, 0);

            try
            {
                this.form.ChangeChat(idPrevious);
            }
            catch
            {

            }
        }

        public void SetLastMessage(string id, string authorId, string message)
        {
            this.dictionaryPanelContacts[id].SetLastMessage(authorId, message);
        }

        private void PaintContacts()
        {
            this.SuspendLayout();

            foreach (PanelContact b in this.listPanelContacts)
            {
                b.Dispose();
            }
            this.listPanelContacts.Clear();
            this.dictionaryPanelContacts.Clear();

            PanelContact panelContact;

            for (int i = 0; i < this.Contacts.Count; i++)
            {
                panelContact = new PanelContact(this.Contacts[i].Id)
                {
                    Location = new Point(0, this.heightLabel * i),
                    Size = new Size(240, this.heightLabel),

                    BackColor = this.Contacts.IndexOf(this.CurrentContact) != i ? ClientForm.Style.BackColorSecondary : ClientForm.Style.BackColorSelected
                };
                panelContact.Click += new EventHandler(
                    (sender, e) => 
                    {
                        this.SuspendLayout();

                        string idPrevious = null;
                        if (this.CurrentContact != null)
                        {
                            idPrevious = this.CurrentContact.Id;

                            this.listPanelContacts[this.Contacts.IndexOf(this.CurrentContact)].labelId.BackColor = ClientForm.Style.BackColorSecondary;
                            this.listPanelContacts[this.Contacts.IndexOf(this.CurrentContact)].labelLastMessage.BackColor = ClientForm.Style.BackColorSecondary;
                            this.listPanelContacts[this.Contacts.IndexOf(this.CurrentContact)].BackColor = ClientForm.Style.BackColorSecondary;

                            this.listPanelContacts[this.Contacts.IndexOf(this.CurrentContact)].Selected = false;
                        }

                        this.CurrentContact = this.Contacts[this.listPanelContacts.IndexOf((PanelContact)sender)];

                        ((PanelContact)sender).labelId.BackColor = ClientForm.Style.BackColorSelected;
                        ((PanelContact)sender).labelLastMessage.BackColor = ClientForm.Style.BackColorSelected;
                        ((PanelContact)sender).BackColor = ClientForm.Style.BackColorSelected;
                        ((PanelContact)sender).Selected = true;

                        this.form.ChangeChat(idPrevious);

                        this.ResumeLayout();
                    });

                this.Controls.Add(panelContact);
                this.listPanelContacts.Add(panelContact);
                this.dictionaryPanelContacts.Add(this.Contacts[i].Id, panelContact);
            }

            this.ResumeLayout();
        }
    }
}