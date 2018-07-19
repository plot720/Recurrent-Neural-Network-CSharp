using DataPreperation.TextPreperation.WordMapping;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataPreperation.TextPreperation
{
    /// <summary>
    /// The different modes that can be used to search content to tokenize sentences.
    /// Random represents a random search through the content. This can return sentence duplicates.
    /// In Order represents a search that goes sequentially sentence by sentence through the content.
    /// Equally split jumps ahead after grabbing a sentence a value determined by the contents length and the maximum word count.
    /// </summary>
    public enum TextSplitterModes { Random, InOrder, EquallySplit }
    
    /// <summary>
    /// Contains various methods involved in preparing text for data processing.
    /// </summary>
    public static class TextPreperation
    {
        /// <summary>
        /// Adds token to the beggining and end of each sentence
        /// </summary>
        /// <param name="Sentences"></param>
        /// <param name="BegginingToken">The token that should be placed at the beggining of each sentence</param>
        /// <param name="EndToken">The token that should be placed at the end of each sentence</param>
        /// <returns></returns>
        public static List<Sentence> AddBegginingAndEndTokens(List<Sentence> Sentences, string BegginingToken, string EndToken)
        {
            List<Sentence> NewSentences = new List<Sentence>();
            if(Sentences == null || !Sentences.Any())
            {
                return null;
            }

            foreach(Sentence s in Sentences)
            {
                Sentence Temp = new Sentence();
                Temp.Words.Add(new Word(BegginingToken));
                foreach(Word w in s.Words)
                {
                    Temp.Words.Add(w);
                }
                Temp.Words.Add(new Word(EndToken));
                NewSentences.Add(Temp);
            }

            return NewSentences;
        }

        /// <summary>
        /// Returns true if the current character is a ., !, or ?
        /// </summary>
        /// <param name="CurrentCharacter"></param>
        /// <returns></returns>
        private static bool AtEndOfSentence(char CurrentCharacter)
        {
            if(CurrentCharacter == '.' || CurrentCharacter == '!' || CurrentCharacter == '?')
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns a list of all of the words in the passed in sentences. Returns each instance of a word.
        /// </summary>
        /// <param name="Sentences"></param>
        /// <returns></returns>
        public static List<Word> GetAllWords(List<Sentence> Sentences)
        {

            List<Word> Words = new List<Word>();
            foreach (Sentence s in Sentences)
            {
                foreach (Word w in s.Words)
                {
                    Words.Add(w);
                }
            }

            return Words;
        }

        /// <summary>
        /// Returns the next spot that should be searched for a sentence. The returned spot depends on the current index and the chosen mode.
        /// This method is used solely by the Tokenize method.
        /// </summary>
        /// <param name="CurrentPosition">Current index that was being searched</param>
        /// <param name="ContentLength">How long the content is</param>
        /// <param name="Mode">How the content should be searched</param>
        /// <param name="MaximumNumberOfSentences">The maximum amount of sentences</param>
        /// <returns></returns>
        private static int GetNextTextSplitterIndex(int CurrentPosition, int ContentLength, TextSplitterModes Mode, int MaximumNumberOfSentences)
        {
            switch (Mode)
            {
                case TextSplitterModes.EquallySplit:

                    int Split = ContentLength / MaximumNumberOfSentences;
                    int CurrentJump;
                    if (Split != 0)
                    {
                        CurrentJump = CurrentPosition / Split;
                        return (CurrentJump + 1) * Split;
                    }
                    else
                    {
                        return CurrentPosition;
                    }

                case TextSplitterModes.InOrder:

                    return CurrentPosition;
                case TextSplitterModes.Random:

                    Random r = new Random(Guid.NewGuid().GetHashCode());
                    return r.Next(0, ContentLength - 10);
            }

            return 0;
        }

        /// <summary>
        /// Returns each distinct word and the number of times it occurs.
        /// </summary>
        /// <param name="Content"></param>
        /// <returns></returns>
        public static List<WordCount> GetWordCounts(string Content, bool CaseSensitive)
        {
            return GetWordCounts(Tokenize(Content, 100000, Mode: TextSplitterModes.InOrder, IsCaseSensitive:CaseSensitive));
        }

        /// <summary>
        /// Returns the count of each word in the passed in sentences.
        /// </summary>
        /// <param name="Sentences"></param>
        /// <returns></returns>
        public static List<WordCount> GetWordCounts(List<Sentence> Sentences)
        {

            return GetWordCounts(GetAllWords(Sentences));
        }

        /// <summary>
        /// Returns the count of each word in the list of words.
        /// </summary>
        /// <param name="Words"></param>
        /// <returns></returns>
        public static List<WordCount> GetWordCounts(List<Word> Words)
        {
            List<WordCount> WordCounter = new List<WordCount>();

            WordCount temp;
            foreach (Word w in Words)
            {
                temp = WordCounter.Where(x => x.Word == w.Text).SingleOrDefault();
                if (temp != null)
                {
                    temp.Count++;
                }
                else
                {
                    WordCounter.Add(new WordCount(1, w.Text));
                }
            }

            return WordCounter;
        }

        /// <summary>
        /// Returns a word map based on the words in the passed in sentences. More frequent words are placed lower in the map.
        /// </summary>
        /// <param name="Sentences"></param>
        /// <returns></returns>
        public static WordMap GetWordMap(List<Sentence> Sentences)
        {
            WordMap map = new WordMap();
            List<WordCount> Counter = GetWordCounts(Sentences);

            Counter = Counter.OrderByDescending(x => x.Count).ToList();

            int counter = 0;
            foreach(WordCount count in Counter)
            {
                counter++;
                map.Add(new WordMapInstance(count.Word, counter));
            }

            return map;
        }

        /// <summary>
        /// Maps the set of words in the passed in sentences based on the passed in word map. Returns a list of the newly mapped sentences.
        /// </summary>
        /// <param name="Map"></param>
        /// <param name="Sentences"></param>
        /// <returns></returns>
        public static List<MappedSentence> MapSentences(WordMap Map, List<Sentence> Sentences)
        {
            List<MappedSentence> MappedSentences = new List<MappedSentence>();

            foreach(Sentence s in Sentences)
            {
                MappedSentence temp = new MappedSentence();
                foreach(Word w in s.Words)
                {
                    int ID = Map.GetID(w.Text);
                    if(ID == -1)
                    {
                        throw new Exception("Cannot have a negative ID!");
                    }

                    temp.IDs.Add(ID);
                }
                MappedSentences.Add(temp);
            }

            return MappedSentences;
        }

        /// <summary>
        /// Maps the words in the passed in sentences to integers. Creates a new word map based on the passed in sentences.
        /// </summary>
        /// <param name="Sentences"></param>
        /// <returns></returns>
        public static List<MappedSentence> MapSentences(List<Sentence> Sentences)
        {
            return MapSentences(GetWordMap(Sentences), Sentences);
        }

        /// <summary>
        /// Removes all but the top X most frequent words in the passed in sentences. Removed words are replaced with the blank word token.
        /// </summary>
        /// <param name="Sentences"></param>
        /// <param name="NumberOfWordsToKeep">The number of most frequent words that should be kept</param>
        /// <param name="BlankWordToken">What removed words should be replaced with</param>
        /// <returns></returns>
        public static List<Sentence> RemoveInfrequentWords(List<Sentence> Sentences, int NumberOfWordsToKeep, string BlankWordToken)
        {
            List<WordCount> WordCounter = GetWordCounts(Sentences);

            if (WordCounter.Count > NumberOfWordsToKeep)
            {
                WordCounter = WordCounter.OrderByDescending(x => x.Count).ToList();
                WordCounter = WordCounter.GetRange(0, NumberOfWordsToKeep);
            }

            foreach (Sentence s in Sentences)
            {
                foreach (Word w in s.Words)
                {
                    if (!WordCounter.Where(x => x.Word == w.Text).Any())
                    {
                        w.Text = BlankWordToken;
                    }
                }
            }

            return Sentences;
        }

        /// <summary>
        /// Tokenizes the content into words and sentences.
        /// </summary>
        /// <param name="Content"></param>
        /// <param name="MaximumNumberOfSentences">The maximum number of sentences that should be returned</param>
        /// <param name="MaximumWordCount">The maximum number of words per sentence. If set to null there is no maximum.</param>
        /// <param name="MinimumWordCount">The minimum number of words per sentence. If set to null there is no minimum</param>
        /// <param name="Mode">The mode of traversing the content that should be used. By default the content is searched in sequential order.</param>
        /// <param name="IsCaseSensitive">If set to false all words will be converted to lower case.</param>
        /// <returns></returns>
        public static List<Sentence> Tokenize(string Content, int MaximumNumberOfSentences,
        int? MaximumWordCount = 9, int? MinimumWordCount = 4, TextSplitterModes Mode = TextSplitterModes.InOrder,
        bool IsCaseSensitive = false)
        {
            //Initialize Variables
            List<Sentence> Sentences = new List<Sentence>();

            if (Content == null || Content.Length == 0)
            {
                return new List<Sentence>();
            }
            int ContentLength = Content.Length;

            int CurrentPosition = 0;
            Sentence CurrentSentence = new Sentence();
            Word CurrentWord = new Word();

            char CurrentCharacter;

            // While the number of sentence is less than the maximum and the mode is random or a different mode and not at teh end of the content.
            while (
                ((Mode == TextSplitterModes.Random) || ((Mode == TextSplitterModes.EquallySplit || Mode == TextSplitterModes.InOrder) && CurrentPosition < ContentLength))
                && Sentences.Count < MaximumNumberOfSentences
                )
            {
                // Grab the character and conver to lowercase if needed.
                CurrentCharacter = Content[CurrentPosition];
                if (!IsCaseSensitive)
                {
                    CurrentCharacter = char.ToLower(CurrentCharacter);
                }

                // If the current position denotes the end of the sentence
                #region End Of Sentence
                if (AtEndOfSentence(CurrentCharacter))
                {
                    CurrentSentence.Words.Add(CurrentWord);
                    CurrentSentence.Words.Add(new Word(CurrentCharacter.ToString()));

                    // If the current sentence fits the criteria for sentences add it to the list of sentences.
                    if (((MinimumWordCount == null && CurrentSentence.WordCount > 0) || CurrentSentence.WordCount > MinimumWordCount)
                        && (MaximumWordCount == null || CurrentSentence.WordCount < MaximumWordCount))
                    {
                        Sentences.Add(CurrentSentence);
                        if (Sentences.Count > MaximumNumberOfSentences)
                        {
                            break;
                        }
                        CurrentPosition = GetNextTextSplitterIndex(CurrentPosition, ContentLength, Mode, MaximumNumberOfSentences);
                    }

                    // If not ignore it and move onto the next sentence
                    CurrentSentence = new Sentence();
                    CurrentWord = new Word();
                }
                #endregion End Of Sentence

                //If the current position is a space
                #region Space
                // If the current position is a tab, space, or newline go to the next word.
                else if (CurrentCharacter == ' ' || CurrentCharacter == '\n' || CurrentCharacter == '\r' || CurrentCharacter == '\t')
                {
                    if (CurrentWord != null && CurrentWord.Text.Length > 0)
                    {
                        CurrentSentence.Words.Add(CurrentWord);
                        CurrentWord = new Word();
                    }
                }
                #endregion Space

                // If the current position is a special character, add it as a seperate word.
                #region Special Characters
                else if (CurrentCharacter == ';' ||
                    CurrentCharacter == '(' ||
                    CurrentCharacter == ')' ||
                    CurrentCharacter == '$' ||
                    CurrentCharacter == '@' ||
                    CurrentCharacter == '#' ||
                    CurrentCharacter == '%' ||
                    CurrentCharacter == '+' ||
                    CurrentCharacter == '-' ||
                    CurrentCharacter == '/' ||
                    CurrentCharacter == '"' ||
                    CurrentCharacter == ',' ||
                    CurrentCharacter == '=' ||
                    CurrentCharacter == '|')
                {
                    if (CurrentWord != null && CurrentWord.Text.Length > 0)
                    {
                        CurrentSentence.Words.Add(CurrentWord);
                        CurrentSentence.Words.Add(new Word(CurrentCharacter.ToString()));
                        CurrentWord = new Word();
                    }
                }
                #endregion Special Characters

                // If the current character is a single quote, treat it as the start of a new word.
                #region Single Quote
                else if (CurrentCharacter == '\'')
                {
                    if (CurrentWord != null && CurrentWord.Text.Length > 0)
                    {
                        CurrentSentence.Words.Add(CurrentWord);
                    }
                    CurrentWord = new Word(CurrentCharacter.ToString());
                }
                #endregion Single Quote

                // If it is any other character, add it to the current word.
                #region All Other Characters
                else
                {
                    CurrentWord.Text += CurrentCharacter;
                }
                #endregion All Other Characters

                CurrentPosition++;

                // If the end of the content has been reached, progress based on the mode.
                #region If At End Of Content
                if (CurrentPosition >= ContentLength)
                {
                    // If the current sentence has the mimum amount of words, add it as a new sentence
                    if ((MinimumWordCount == null && CurrentSentence.WordCount > 0) || CurrentSentence.WordCount > MinimumWordCount)
                    {
                        Sentences.Add(CurrentSentence);
                        if (Sentences.Count > MaximumNumberOfSentences)
                        {
                            break;
                        }
                    }

                    // If the mode is Equally Split or In Order quit
                    if (Mode == TextSplitterModes.EquallySplit || Mode == TextSplitterModes.InOrder)
                    {
                        break;
                    }
                    // If its random keep going
                    else if (Mode == TextSplitterModes.Random)
                    {
                        CurrentSentence = new Sentence();
                        CurrentPosition = GetNextTextSplitterIndex(CurrentPosition, ContentLength, Mode, MaximumNumberOfSentences);
                    }
                }
                #endregion If At End Of Content
            }


            return Sentences;
        }
    }
}
