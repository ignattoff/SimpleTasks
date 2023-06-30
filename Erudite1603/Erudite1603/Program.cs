using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using NUnit.Framework;


namespace Erudite1603
{
    public class Program
    {
        public class QuatroItem
        {
            public char value;
            public int index;
            public QuatroItem? Up;
            public QuatroItem? Down;
            public QuatroItem? Right;
            public QuatroItem? Left;

            private QuatroItem() : this(value: new char(), 0)
            {
            }
            public QuatroItem(char value, int index)
            {
                this.value = value;
                this.index = index;
            }
            public QuatroItem Clone()
            {
                return new QuatroItem()
                {
                    value = value,
                    index = index,
                    Up = Up,
                    Down = Down,
                    Right = Right,
                    Left = Left,
                };
            }
        }
        public class QuatroItemLinkedList
        {
            public QuatroItem?[]? list { get; set; }
            public Dictionary<char, List<int>>? keyLetters { get; set; }

            private QuatroItemLinkedList()
            {
            }
            public QuatroItemLinkedList(Dictionary<int, char> monoArray)
            {
                var len = Convert.ToInt32(Math.Sqrt(monoArray.Count));
                list = new QuatroItem[monoArray.Count];
                keyLetters = new Dictionary<char, List<int>>();
                foreach (var e in monoArray)
                {
                    CheckAndCreateElement(e.Value, e.Key);
                    if ((e.Key % len) > 0)
                    {
                        CheckAndCreateElement(monoArray[e.Key - 1], e.Key - 1);
                        AddLink(e.Key, e.Key - 1);
                    }

                    if (e.Key % len < len - 1)
                    {
                        CheckAndCreateElement(monoArray[e.Key + 1], e.Key + 1);
                        AddLink(e.Key, e.Key + 1);
                    }

                    if (e.Key - len >= 0)
                    {
                        CheckAndCreateElement(monoArray[e.Key - len], e.Key - len);
                        AddLink(e.Key, e.Key - len);
                    }

                    if (monoArray.Count - len - e.Key > 0)
                    {
                        CheckAndCreateElement(monoArray[e.Key + len], e.Key + len);
                        AddLink(e.Key, e.Key + len);
                    }
                }
            } //инициализируем начальный связный список
            public QuatroItemLinkedList Clone() //создаём копию экземпляра
            {
                return new QuatroItemLinkedList
                {
                    list = list.Select(x => x?.Clone()).ToArray(),
                    keyLetters = keyLetters.ToDictionary(note => note.Key, note => note.Value.ToList())
                };
            }
            private void CheckAndCreateElement(char element, int index) //проверяем, есть ли элемент и создаём
            {
                list[index] ??= new QuatroItem(element, index);
                if (keyLetters.TryGetValue(element, out List<int> value))
                {
                    if (!value.Contains(index))
                        value.Add(index);
                }
                else
                    keyLetters.Add(element, new List<int> { index });
            }
            private void AddLink(int mainIndex, int linkIndex) //добавляем указатель на связанный элемент
            {
                switch (mainIndex - linkIndex)
                {
                    case -1:
                        list[mainIndex].Right = list[linkIndex];
                        break;
                    case 1:
                        list[mainIndex].Left = list[linkIndex];
                        break;
                    case 4:
                        list[mainIndex].Up = list[linkIndex];
                        break;
                    case -4:
                        list[mainIndex].Down = list[linkIndex];
                        break;
                }
            }
            public void RemoveElementById(int id) //удаляем элемент и ссылки на него
            {
                if (keyLetters.TryGetValue(list[id].value, out List<int> value))
                    value.Remove(id);
                else
                    keyLetters.Remove(list[id].value);
                if (list[id].Right != null)
                    list[list[id].Right.index].Left = null;
                if (list[id].Left != null)
                    list[list[id].Left.index].Right = null;
                if (list[id].Up != null)
                    list[list[id].Up.index].Down = null;
                if (list[id].Down != null)
                    list[list[id].Down.index].Up = null;
                list[id] = null;
            }
            //рекурсивно ищем по буквам слово
            public int TryFindByWord(char[] word, int letterIndex, char nextLetter, int localindex)
            {
                if (list[letterIndex]?.Left?.value == nextLetter)
                {
                    if (localindex + 1 == word.Length) return localindex + 1;
                    var linkedListLocal = Clone();
                    linkedListLocal.RemoveElementById(letterIndex);
                    var tempResult = linkedListLocal.TryFindByWord(word, list[letterIndex].Left.index,
                        word[localindex + 1], localindex + 1);
                    if (tempResult == word.Length)
                        return tempResult;
                }

                if (list[letterIndex]?.Right?.value == nextLetter)
                {
                    if (localindex + 1 == word.Length) return localindex + 1;
                    var linkedListLocal = Clone();
                    linkedListLocal.RemoveElementById(letterIndex);
                    var tempResult = linkedListLocal.TryFindByWord(word, list[letterIndex].Right.index,
                        word[localindex + 1], localindex + 1);
                    if (tempResult == word.Length)
                        return tempResult;
                }

                if (list[letterIndex]?.Up?.value == nextLetter)
                {
                    if (localindex + 1 == word.Length) return localindex + 1;
                    var linkedListLocal = Clone();
                    linkedListLocal.RemoveElementById(letterIndex);
                    var tempResult = linkedListLocal.TryFindByWord(word, list[letterIndex].Up.index,
                        word[localindex + 1], localindex + 1);
                    if (tempResult == word.Length)
                        return tempResult;
                }

                if (list[letterIndex]?.Down?.value == nextLetter)
                {
                    if (localindex + 1 == word.Length) return localindex + 1;
                    var linkedListLocal = Clone();
                    linkedListLocal.RemoveElementById(letterIndex);
                    var tempResult = linkedListLocal.TryFindByWord(word, list[letterIndex].Down.index,
                        word[localindex + 1], localindex + 1);
                    if (tempResult == word.Length)
                        return tempResult;
                }

                return localindex;
            }
        }
        public static string[] MainProgram(string[] words, string[,] lettersArray)
        {
            var monoArray = new Dictionary<int, char>();
            for (var i = 0; i < lettersArray.GetLength(0); i++)
            {
                for (var j = 0; j < lettersArray.GetLength(0); j++)
                {
                    monoArray.Add(lettersArray.GetLength(0) * i + j, Convert.ToChar(lettersArray[i, j]));
                }
            }

            var resultWords = new List<string>();
            var localQuatroList = new QuatroItemLinkedList(monoArray);
            foreach (var word in words)
            {
                var wordInChar = word.ToCharArray();
                var controlWordLength = 0;
                if (localQuatroList.keyLetters.TryGetValue(wordInChar[0], out List<int> value))
                {
                    foreach (var firstLetter in value)
                    {
                        controlWordLength = localQuatroList.Clone()
                            .TryFindByWord(wordInChar, firstLetter, wordInChar[1], 1);
                        if (controlWordLength == wordInChar.Length)
                            break;
                    }
                }
                resultWords.Add($"{new string(wordInChar)}: {(controlWordLength == wordInChar.Length ? "YES" : "NO")}");
            }
            return resultWords.ToArray();
        }
    }
}