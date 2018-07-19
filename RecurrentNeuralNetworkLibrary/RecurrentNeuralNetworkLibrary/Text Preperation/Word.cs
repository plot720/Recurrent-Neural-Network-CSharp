using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataPreperation.TextPreperation
{
    /// <summary>
    /// Represents a singular word.
    /// </summary>
    public class Word
    {
        /// <summary>
        /// Contains the word itself
        /// </summary>
        public string Text { get; set; }

        public Word()
        {
            Text = "";
        }

        public Word(string text)
        {
            Text = text;
        }
    }
}
