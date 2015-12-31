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
            this.lettersTextBox.Focus();
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
            SimpleLookup<int, string> lookup = wordList ?? (wordList = ReadWordList());

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
            var textBoxs = new[] {textBox0, textBox1, textBox2};
            int ix = 0;
            foreach (int length in lengths)
            {
                if (listBoxs[ix] != sourceListBox)
                {
                    listBoxs[ix].Items.Clear();
                    foreach (string word in lookup[length].Where(w => WordMatch(w, letters)))
                    {
                        var exclude = textBoxs[ix].Text.ToLower();
                        bool include = true;
                        if (exclude.Length == length)
                        {
                            for (int j=0; j<length; j++)
                            {
                                if (exclude[j] != '_' && exclude[j]!=word[j])
                                {
                                    include = false;
                                    break;
                                }
                            }
                        }
                        if (include)
                            listBoxs[ix].Items.Add(word);
                    }
                }
                ix++;
            }

            if (sourceListBox == null && lengths.Count()>1)
            {                
                //todo fjern ord som ikke gir muligherer for andre ord
            }
            
        }

        private static bool WordMatch(string word, string letters)
        {
            if (!word.All(letters.Contains))  return false;
            Dictionary<char, int> wCount = word.GroupBy(_ => _).ToDictionary(_ => _.Key, _ => _.Count());
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
                    var split = line.Split(' ');
                    string word = split[0].ToLower();
                   // if (split[1] == "subst")  //bare substantiv
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