using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Ordliste
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SimpleLookup<int, string> wordList;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
        if (string.IsNullOrWhiteSpace(lengthTextBox.Text))
                lengthTextBox.Text = lettersTextBox.Text.Length.ToString();
            
            FindCandidates(null);
        }

        private void FindCandidates(ListBox sourceListBox)
        {
            var letters = new string(lettersTextBox.Text.ToLower().Trim().OrderBy(l => l).ToArray());
            SimpleLookup<int, string> dictionary = wordList ?? (wordList = ReadWordList());

            if (sourceListBox != null && sourceListBox.SelectedItem != null)
            {
                var selected = (string) sourceListBox.SelectedItem;
                foreach (char ch in selected)
                {
                    int chIx = letters.IndexOf(ch);
                    if (chIx >= 0)
                        letters = letters.Remove(chIx, 1);
                }
            }

            IEnumerable<int> lengths = lengthTextBox.Text.Split(' ', ',').Select(p => p.Trim()).Select(int.Parse);
            var listBoxs = new[] {listBox, listBox2, listBox3};
            int ix = 0;
            foreach (int length in lengths)
            {
                if (listBoxs[ix] != sourceListBox)
                {
                    listBoxs[ix].Items.Clear();
                    foreach (string word in dictionary[length].Where(w => WordMatch(w, letters)))
                        listBoxs[ix].Items.Add(word);
                }
                ix++;
            }
        }

        private static bool WordMatch(string w, string letters)
        {
            //return w.OrderBy(_ => _).SequenceEqual(letters);
            if (!w.All(letters.Contains))
                return false;
            Dictionary<char, int> wCount = w.GroupBy(_ => _).ToDictionary(_ => _.Key, _ => _.Count());
            Dictionary<char, int> lCount = letters.GroupBy(_ => _).ToDictionary(_ => _.Key, _ => _.Count());
            return wCount.All(pair => lCount[pair.Key] >= pair.Value);
        }

        private static SimpleLookup<int, string> ReadWordList()
        {
            var lookup = new SimpleLookup<int, string>();
            using (StreamReader file = File.OpenText(@"..\..\NSF-ordlisten.txt"))
                while (!file.EndOfStream)
                {
                    string line = file.ReadLine();
                    string word = line.Split(' ')[0].ToLower();
                    lookup.Add(word.Length, word);
                }
            return lookup;
        }

        private void listBox2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FindCandidates((ListBox) sender);
        }

        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FindCandidates((ListBox) sender);
        }

        private void listBox3_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FindCandidates((ListBox) sender);
        }
    }
}