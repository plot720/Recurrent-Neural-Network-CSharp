using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataPreperation.TextPreperation
{
    /// <summary>
    /// Represents a sentence; a list of words
    /// </summary>
    public class Sentence
    {
        /// <summary>
        /// The list of words that make up the sentence
        /// </summary>
        public List<Word> Words { get; set; }

        /// <summary>
        /// The number of words in the sentence.
        /// </summary>
        public int WordCount
        {
            get
            {
                if (Words != null)
                {
                    return Words.Count;
                }

                return 0;
            }
        }

        public Sentence()
        {
            Words = new List<Word>();
        }
    }
}
