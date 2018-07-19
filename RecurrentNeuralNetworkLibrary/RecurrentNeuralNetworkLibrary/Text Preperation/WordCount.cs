using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataPreperation.TextPreperation
{
    /// <summary>
    /// Represents the count of a unique word
    /// </summary>
    public class WordCount
    {
        /// <summary>
        /// The number of occurences of the word
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// The word being counted
        /// </summary>
        public string Word { get; set; }

        public WordCount(int count, string word)
        {
            Count = count;
            Word = word;
        }

        public WordCount()
        {

        }
        
    }
}
