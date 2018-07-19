using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataPreperation.TextPreperation.WordMapping
{
    /// <summary>
    /// Contains a map of words to integer values.
    /// </summary>
    public class WordMap
    {
        /// <summary>
        /// The list of word map instances
        /// </summary>
        public List<WordMapInstance> MapInstances { get; set; }

        public WordMap()
        {
            MapInstances = new List<WordMapInstance>();
        }

        /// <summary>
        ///  Adds a map instance
        /// </summary>
        /// <param name="instance"></param>
        public void Add(WordMapInstance instance)
        {
            MapInstances.Add(instance);
        }
        
        /// <summary>
        /// Gets the id of the passed in word or returns -1 if the word hasn't been added.
        /// </summary>
        /// <param name="Word"></param>
        /// <returns></returns>
        public int GetID(string Word)
        {
            if(MapInstances == null || !MapInstances.Any())
            {
                return -1;
            }

            try
            {
                return MapInstances.Where(x => x.Word == Word).SingleOrDefault().ID;
            }
            catch
            {
                return -1;
            }
        }

    }
}
