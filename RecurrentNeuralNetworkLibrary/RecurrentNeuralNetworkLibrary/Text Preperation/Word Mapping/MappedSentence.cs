using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataPreperation.TextPreperation.WordMapping
{
    /// <summary>
    /// Represents a sentence who's words were converted to integers using a Word map.
    /// </summary>
    public class MappedSentence
    {
        /// <summary>
        /// The list of IDs of the words
        /// </summary>
        public List<int> IDs { get; set; }

        /// <summary>
        /// The number of words.
        /// </summary>
        public int Count
        {
            get
            {
                if(IDs != null)
                {
                    return IDs.Count;
                }

                return 0;
            }
        }

        public MappedSentence()
        {
            IDs = new List<int>();
        }

        public MappedSentence(List<int> ids)
        {
            IDs = ids;
        }
    }
}
