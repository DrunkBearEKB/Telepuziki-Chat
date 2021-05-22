using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Client.UserInterface.Styles
{
    public abstract class Style
    {
        public abstract Color BackColorMain { get; }
        public abstract Color BackColorSecondary { get; }
        public abstract Color BackColorEntered { get; }
        public abstract Color BackColorSelected { get; }
        
        public abstract Color ForeColorMain { get; }
        public abstract Color ForeColorSecondary { get; }
        public abstract Color ForeColorResidual { get; }
        public abstract string pathImages { get; }

        private string pathResources;
        private Dictionary<string, Dictionary<bool, Image>> dictionaryImages;

        protected Style(string pathResources)
        {
            this.pathResources = pathResources;
            
            this.dictionaryImages = new Dictionary<string, Dictionary<bool, Image>>();
            foreach (string fileName in Directory.GetFiles($"{Directory.GetCurrentDirectory()}\\{this.pathResources}\\{pathImages}"))
            {
                int index = fileName.LastIndexOf("\\");
                string name = fileName.Substring(index + 1, fileName.Length - index - 1).Replace(".png", "");
                bool isEntered = name.Contains('_');
                name = isEntered ? name.Substring(0, name.IndexOf('_')) : name;

                if (!this.dictionaryImages.ContainsKey(name))
                {
                    this.dictionaryImages.Add(name, new Dictionary<bool, Image>());   
                }
                
                this.dictionaryImages[name].Add(isEntered, Image.FromFile(fileName));
            }
        }

        public Image GetImage(string name, bool isEntered)
        {
            return this.dictionaryImages[name][isEntered];
        }
    }
}