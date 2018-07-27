using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace AbstractGamesCreationKit
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        
        static void Main()
        {
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainWindow());
            
        }

        /// <summary>
        /// The following methods are necessary for validating all the inserted information in the interface in real-time,
        /// notifying the user what's wrong when he does input something invalid
        /// </summary>

        public static bool CheckLegalName(Control textBox, string target)
        {
            return CheckOneWord(textBox, target) &&
                   CheckIllegalOperators(textBox, target) &&
                   CheckStartingNumber(textBox, target) &&
                   CheckValidNames(textBox) &&
                   !string.IsNullOrWhiteSpace(textBox.Text);
        }

        //Return true if the name chosen is valid and false if it isn't (displays a message in this case)
        public static bool CheckValidNames(Control textBox)
        {
            //invalidNames list all the keywords in the ZRF language
            const string invalidNames = @"|absolute-config|add|add-copy|add-copy-partial|add-partial|adjacent-to-enemy?|
                                        |and|any-owner|attacked?|attribute|back|board|board-setup|capture|capture-sound|
                                        |captured|cascade|change-owner|change-sound|change-type|checkmated|click-sound|
                                        |count-condition|create|default|defended?|description|directions|draw-condition|
                                        |draw-sound|dimensions|drop-sound|drops|dummy|else|engine|empty?|enemy?|false|flag?|
                                        |flip|forced|friend?|from|game|go|goal-position?|grid|help|history|if|image|in-zone?|
                                        |kill-positions|last-from|last-from?|last-to|last-to?|links|loss-condition|loss-sound|
                                        |mark|marked?|moves|move-priorities|move-sound|move-type|music|name|neutral?|not|notation|
                                        |not-adjacent-to-enemy?|not-attacked?|not-defended?|not-empty?|not-enemy?|not-flag?|
                                        |not-friend?|not-goal-position?|not-in-zone?|not-last-from?|not-last-to?|not-marked?|
                                        |not-neutral?|not-on-board?|not-piece?|not-position?|not-position-flag?|off|on-board?|
                                        |open|opening-sound|opponent|opposite|option|or|piece|piece?|pieces-remaining|players|
                                        |position?|position-flag?|positions|relative-config|release-sound|repeat|repetition|
                                        |set-attribute|set-flag|set-position-flag|solution|stalemated|start-rectangle|strategy|
                                        |symmetry|title|to|total-piece-count|translate|true|turn-order|unlink|variant|verify|while|
                                        |win-condition|win-sound|zone|define|include|version|\|`|;|n|ne|e|se|s|sw|w|nw";

            if (invalidNames.IndexOf("|" + textBox.Text + "|") < 0) return true; //if IndexOf returns -1 the string is valid
            MessageBox.Show(@"The name """ + textBox.Text + @""" is invalid");
            textBox.Text = "";
            return false;
        }

        public static bool CheckOneWord(Control textBox, string target)
        {
            if (!textBox.Text.Contains(" ")) return true;
            MessageBox.Show(target + @" cannot contain spaces");
            textBox.Text = textBox.Text.Replace(@" ", "");
            return false;
        }

        public static bool CheckStartingNumber(Control textBox, string target)
        {
            //TODO if the number has more than 1 digits
            Regex rgx = new Regex(@"^[0-9]");
            if (!rgx.IsMatch(textBox.Text)) return true;
            MessageBox.Show(target + @" cannot begin with a number");
            textBox.Text = textBox.Text.Remove(0, 1);
            return false;
        }

        public static bool CheckIllegalOperators(Control textBox, string target)
        {
            //TODO - improve illegal operators regular expression
            Regex rgx = new Regex(@"[\,\;\:""\!\#\$\%\&\[\\\^\$\.\|\?\*\+\(\)\{\}]");
            if (!rgx.IsMatch(textBox.Text)) return true;
            target += " cannot contain any of the following characters:\n";
            MessageBox.Show(target + @"! # $ % , ; : "" \ ^ $ . | ? * + ( ) { } [ ]".PadLeft(target.Length + 5));
            textBox.Text = rgx.Replace(textBox.Text, "");
            return false;
        }

        public static bool CheckSomeIllegalOperators(Control textBox, string target)
        {
            Regex rgx = new Regex(@"[""\\\^\$\|]");
            if (!rgx.IsMatch(textBox.Text)) return true;
            target += " cannot contain any of the following characters:\n";
            MessageBox.Show(target + @""" \ ^ $ |".PadLeft(target.Length - 5));
            textBox.Text = rgx.Replace(textBox.Text, "");
            return false;
        }

        public static bool UnorderedEqual<T>(ICollection<T> a, ICollection<T> b)
        {
            if (a.Count != b.Count) return false;

            Dictionary<T, int> d = new Dictionary<T, int>();
            foreach (T item in a)
            {
                int c;
                if (d.TryGetValue(item, out c)) d[item] = c + 1;
                else d.Add(item, 1);
            }
            foreach (T item in b)
            {
                int c;
                if (!d.TryGetValue(item, out c)) return false;
                if (c == 0) return false;
                d[item] = c - 1;
            }
            foreach (int v in d.Values) if (v != 0) return false;
            return true;
        }

    }
}
