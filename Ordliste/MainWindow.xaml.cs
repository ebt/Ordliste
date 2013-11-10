using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Ordliste
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SimpleLookup<int, string> wordList;

        public MainWindow()
        {
            InitializeComponent();
        }

        private  void Button_Click(object sender, RoutedEventArgs e)
        {
            var letters = new string(this.lettersTextBox.Text.ToLower().Trim().OrderBy(l=>l).ToArray());
            var dictionary = this.wordList ?? (this.wordList = ReadWordList());

            if (string.IsNullOrWhiteSpace(lengthTextBox.Text))
            {
                //var result = new List<string>();
                foreach (var word in dictionary[letters.Length])
                {
                    var sorted = new string(word.OrderBy(w => w).ToArray());
                    if (sorted == letters)
                    {
                        this.listBox.Items.Add(word);
                        //result.Add(word);
                    }
                }
            }
            else
            {
                this.FindCandidates(null);
            }
        }

        private void FindCandidates(ListBox sourceListBox)
        {
            var letters = new string(this.lettersTextBox.Text.ToLower().Trim().OrderBy(l => l).ToArray());
            var dictionary = this.wordList ?? (this.wordList = ReadWordList());

            if (sourceListBox != null && sourceListBox.SelectedItem != null)
            {
                var selected = (string)sourceListBox.SelectedItem;
                foreach (var ch in selected)
                {
                    var chIx = letters.IndexOf(ch);
                    if (chIx>=0)
                        letters = letters.Remove(chIx,1);
                }
            }

            var lengths = this.lengthTextBox.Text.Split(' ', ',').Select(p => p.Trim()).Select(int.Parse);
            var listBoxs = new[] { this.listBox, this.listBox2, this.listBox3 };
            int ix = 0;
            foreach (var length in lengths)
            {
                if (listBoxs[ix] != sourceListBox)
                {
                    listBoxs[ix].Items.Clear();
                    foreach (var word in dictionary[length].Where(w => WordMatch(w, letters)))
                        listBoxs[ix].Items.Add(word);
                }
                ix++;
            }
        }

        private static bool WordMatch(string w, string letters)
        {
            //return w.OrderBy(_ => _).SequenceEqual(letters);
            if (! w.All(letters.Contains))
                return false;
            var wCount = w.GroupBy(_ => _).ToDictionary(_ => _.Key, _ => _.Count());
            var lCount = letters.GroupBy(_ => _).ToDictionary(_ => _.Key, _ => _.Count());
            foreach (var pair in wCount)
            {
                if (lCount[pair.Key] < pair.Value)
                    return false;
            }
            return true;
        }

        private static SimpleLookup<int, string> ReadWordList()
        {
            var lookup = new SimpleLookup<int, string>();
            using (var file = File.OpenText(@"..\..\NSF-ordlisten.txt"))
                while (!file.EndOfStream)
            {
                var line = file.ReadLine();
                var word = line.Split(' ')[0].ToLower();
                lookup.Add(word.Length, word);
            }
            return lookup;
        }

        private void listBox2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.FindCandidates((ListBox)sender);
        }

        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.FindCandidates((ListBox)sender);

        }

        private void listBox3_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.FindCandidates((ListBox)sender);

        }

    }
}
