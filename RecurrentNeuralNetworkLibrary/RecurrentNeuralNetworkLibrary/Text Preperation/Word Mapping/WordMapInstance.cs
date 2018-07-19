using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataPreperation.TextPreperation.WordMapping
{
    /// <summary>
    /// The map used to map a word to a certain integer value. Used by the Word Map class.
    /// </summary>
    public class WordMapInstance
    {
        /// <summary>
        /// The word
        /// </summary>
        public string Word { get; set; }

        /// <summary>
        /// The mapped integer
        /// </summary>
        public int ID { get; set; }

        public WordMapInstance()
        {

        }

        public WordMapInstance(string word, int id)
        {
            Word = word;
            ID = id;
        }
    }
}
