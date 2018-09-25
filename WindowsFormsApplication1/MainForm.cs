using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;


namespace WindowsFormsApplication1
{

    public partial class Form1 : Form
    {


        //initialize the space for our dictionary data
        DictionaryData DictData = new DictionaryData();



        //this is what runs at initialization
        public Form1()
        {

            InitializeComponent();

            foreach(var encoding in Encoding.GetEncodings())
            {
                EncodingDropdown.Items.Add(encoding.Name);
            }

            try
            {
                EncodingDropdown.SelectedIndex = EncodingDropdown.FindStringExact("utf-8");
            }
            catch
            {
                EncodingDropdown.SelectedIndex = EncodingDropdown.FindStringExact(Encoding.Default.BodyName);
            }
            


        }







        private void StartButton_Click(object sender, EventArgs e)
        {


                    //make sure that our dictionary is loaded before anything else
                    if (DictData.DictionaryLoaded != true)
                    {
                        MessageBox.Show("You must first load a dictionary file before you can analyze your texts.", "Dictionary not loaded!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
            

                    FolderBrowser.Description = "Please choose the location of your .txt files to analyze";
                    if (FolderBrowser.ShowDialog() != DialogResult.Cancel) {

                        DictData.TextFileFolder = FolderBrowser.SelectedPath.ToString();
                
                        if (DictData.TextFileFolder != "")
                        {

                            saveFileDialog.FileName = "RIOTLite.csv";

                            saveFileDialog.InitialDirectory = DictData.TextFileFolder;
                            if (saveFileDialog.ShowDialog() != DialogResult.Cancel) {


                                DictData.OutputFileLocation = saveFileDialog.FileName;
                                DictData.RawWordCounts = RawWCCheckbox.Checked;

                                if (DictData.OutputFileLocation != "") {


                                    StartButton.Enabled = false;
                                    ScanSubfolderCheckbox.Enabled = false;
                                    PunctuationBox.Enabled = false;
                                    EncodingDropdown.Enabled = false;
                                    LoadDictionaryButton.Enabled = false;
                                    RawWCCheckbox.Enabled = false;
                            
                                    BgWorker.RunWorkerAsync(DictData);
                                }
                            }
                        }

                    }

                

        }

        




        private void BgWorkerClean_DoWork(object sender, DoWorkEventArgs e)
        {


            DictionaryData DictData = (DictionaryData)e.Argument;
            
            //set up our sentence boundary detection
            Regex NewlineClean = new Regex(@"[\r\n]+", RegexOptions.Compiled);

            //selects the text encoding based on user selection
            Encoding SelectedEncoding = null;
            this.Invoke((MethodInvoker)delegate ()
            {
                SelectedEncoding = Encoding.GetEncoding(EncodingDropdown.SelectedItem.ToString());
            });

            

            //get the list of files
            var SearchDepth = SearchOption.TopDirectoryOnly;
            if (ScanSubfolderCheckbox.Checked)
            {
                SearchDepth = SearchOption.AllDirectories;
            }
            var files = Directory.EnumerateFiles(DictData.TextFileFolder, "*.txt", SearchDepth);



            try {

                //open up the output file
                using (StreamWriter outputFile = new StreamWriter(new FileStream(DictData.OutputFileLocation, FileMode.Create), SelectedEncoding))
                {

                    //write the header row to the output file
                    StringBuilder HeaderString = new StringBuilder();
                    HeaderString.Append("\"Filename\",\"WC\",\"DictPercent\"");
                    for (int i = 0; i < DictData.NumCats; i++) HeaderString.Append("," + DictData.CatNames[i].Replace("\"", "\"\""));
                    outputFile.WriteLine(HeaderString.ToString());


                    foreach (string fileName in files)
                    {

                        //set up our variables to report
                        string Filename_Clean = Path.GetFileName(fileName);
                        Dictionary<string, int> DictionaryResults = new Dictionary<string, int>();
                        for (int i = 0; i < DictData.NumCats; i++) DictionaryResults.Add(DictData.CatValues[i], 0);

                        //report what we're working on
                        FilenameLabel.Invoke((MethodInvoker)delegate
                        {
                            FilenameLabel.Text = "Analyzing: " + Filename_Clean;
                        });




                        //read in the text file, convert everything to lowercase
                        string readText = File.ReadAllText(fileName, SelectedEncoding).ToLower();
                        readText = NewlineClean.Replace(readText, " ");

                        //remove all the junk punctuation
                        foreach (char c in PunctuationBox.Text)
                        {
                            readText = readText.Replace(c, ' ');
                        }


                        int NumberOfMatches = 0;


                        //splits everything out into words
                        //we're splitting on spaces here principally because we leave it up to the
                        //user to decide what characters they want to remove. we're assuming that
                        //they have removed tabs already (as is set up by default)
                        string[] Words = readText.Trim().Split(' ');

                        Words = Words.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
                        int TotalStringLength = Words.Length;



                        //     _                _                 _____         _   
                        //    / \   _ __   __ _| |_   _ _______  |_   _|____  _| |_ 
                        //   / _ \ | '_ \ / _` | | | | |_  / _ \   | |/ _ \ \/ / __|
                        //  / ___ \| | | | (_| | | |_| |/ /  __/   | |  __/>  <| |_ 
                        // /_/   \_\_| |_|\__,_|_|\__, /___\___|   |_|\___/_/\_\\__|
                        //                        |___/                             


                        //iterate over all words in the text file
                        for (int i = 0; i < TotalStringLength; i++)
                        {



                            //iterate over n-grams, starting with the largest possible n-gram (derived from the user's dictionary file)
                            for (int NumberOfWords = DictData.MaxWords; NumberOfWords > 0; NumberOfWords--)
                            {



                                //make sure that we don't overextend past the array
                                if (i + NumberOfWords - 1 >= TotalStringLength) continue;

                                //make the target string

                                string TargetString;

                                if (NumberOfWords > 1)
                                {
                                    TargetString = String.Join(" ", Words.Skip(i).Take(NumberOfWords).ToArray());
                                }
                                else
                                {
                                    TargetString = Words[i];
                                }


                                //look for an exact match

                                if (DictData.FullDictionary["Standards"].ContainsKey(NumberOfWords))
                                {
                                    if (DictData.FullDictionary["Standards"][NumberOfWords].ContainsKey(TargetString))
                                    {

                                        NumberOfMatches += NumberOfWords;
                                        //add in the number of words found
                                        for (int j = 0; j < DictData.FullDictionary["Standards"][NumberOfWords][TargetString].Length; j++)
                                        {

                                            if (DictionaryResults.ContainsKey(DictData.FullDictionary["Standards"][NumberOfWords][TargetString][j])) DictionaryResults[DictData.FullDictionary["Standards"][NumberOfWords][TargetString][j]] += NumberOfWords;

                                        }
                                        //manually increment the for loop so that we're not testing on words that have already been picked up
                                        i += NumberOfWords - 1;
                                        //break out of the lower level for loop back to moving on to new words altogether
                                        break;
                                    }
                                }
                                //if there isn't an exact match, we have to go through the wildcards
                                if (DictData.WildCardArrays.ContainsKey(NumberOfWords))
                                {
                                    for (int j = 0; j < DictData.WildCardArrays[NumberOfWords].Length; j++)
                                    {
                                        if (DictData.PrecompiledWildcards[DictData.WildCardArrays[NumberOfWords][j]].Matches(TargetString).Count > 0)
                                        {

                                            NumberOfMatches += NumberOfWords;

                                            for (int k = 0; k < DictData.FullDictionary["Wildcards"][NumberOfWords][DictData.WildCardArrays[NumberOfWords][j]].Length; k++)
                                            {

                                                if (DictionaryResults.ContainsKey(DictData.FullDictionary["Wildcards"][NumberOfWords][DictData.WildCardArrays[NumberOfWords][j]][k])) DictionaryResults[DictData.FullDictionary["Wildcards"][NumberOfWords][DictData.WildCardArrays[NumberOfWords][j]][k]] += NumberOfWords;

                                            }
                                            //manually increment the for loop so that we're not testing on words that have already been picked up
                                            i += NumberOfWords - 1;
                                            //break out of the lower level for loop back to moving on to new words altogether
                                            break;

                                        }
                                    }
                                }


                            }



                        }







                        // __        __    _ _          ___        _               _   
                        // \ \      / / __(_) |_ ___   / _ \ _   _| |_ _ __  _   _| |_ 
                        //  \ \ /\ / / '__| | __/ _ \ | | | | | | | __| '_ \| | | | __|
                        //   \ V  V /| |  | | ||  __/ | |_| | |_| | |_| |_) | |_| | |_ 
                        //    \_/\_/ |_|  |_|\__\___|  \___/ \__,_|\__| .__/ \__,_|\__|
                        //                                            |_|              



                        
                        string[] OutputString = new string[3 + DictData.NumCats];
                        OutputString[0] = "\"" + Filename_Clean + "\"";
                        OutputString[1] = TotalStringLength.ToString();

                        if (TotalStringLength > 0)
                        {

                            OutputString[2] = (((double)NumberOfMatches / TotalStringLength) * 100).ToString();

                            if (DictData.RawWordCounts)
                            {
                                for (int i = 0; i < DictData.NumCats; i++) OutputString[i + 3] = DictionaryResults[DictData.CatValues[i]].ToString();
                            }
                            else
                            {
                                for (int i = 0; i < DictData.NumCats; i++) OutputString[i + 3] = (((double)DictionaryResults[DictData.CatValues[i]] / TotalStringLength) * 100).ToString();
                            }
                                

                        }
                        else
                        {
                            OutputString[2] = "";
                            for (int i = 0; i < DictData.NumCats; i++) OutputString[i + 3] = "";
                        }


                        outputFile.WriteLine(String.Join(",", OutputString));








                    }


                }

            }
            catch
            {
                MessageBox.Show("RIOTLite encountered an issue somewhere while trying to analyze your texts. The most common cause of this is trying to open your output file while RIOTLite is still running. Did any of your input files move, or is your output file being opened/modified by another application?", "Error while analyzing", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            
        }


        //when the bgworker is done running, we want to re-enable user controls and let them know that it's finished
        private void BgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            StartButton.Enabled = true;
            ScanSubfolderCheckbox.Enabled = true;
            PunctuationBox.Enabled = true;
            EncodingDropdown.Enabled = true;
            LoadDictionaryButton.Enabled = true;
            RawWCCheckbox.Enabled = true;
            FilenameLabel.Text = "Finished!";
            MessageBox.Show("RIOTLite has finished analyzing your texts.", "Analysis Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }







        public class DictionaryData
        {

            public string TextFileFolder { get; set; }
            public string OutputFileLocation { get; set; }


            public int NumCats { get; set; }
            public int MaxWords { get; set; }

            public string[] CatNames { get; set; }
            public string[] CatValues { get; set; }

            public bool RawWordCounts { get; set; }

            //yeah, we're going full inception with this variable. dictionary inside of a dictionary inside of a dictionary
            //while it might seem unnecessarily complicated (and it might be), it makes sense.
            //the first level simply differentiates the wildcard entries from the non-wildcard entries                
            //The second level is purely to refer to the word length -- does each sub-entry include 1-word entries, 2-word entries, etc?
            //the third level contains the actual entries from the user's dictionary file
            public Dictionary<string, Dictionary<int, Dictionary<string, string[]>>> FullDictionary { get; set; }

            //this dictionary simply maps the specific wildcard entries to arrays, this way we can iterate across them since we have to do a serial search
            //when we're using wildcards
            public Dictionary<int, string[]> WildCardArrays { get; set; }

            //lastly, this contains the precompiled regexes mapped to their original strings
            public Dictionary<string, Regex> PrecompiledWildcards { get; set; }

            public bool DictionaryLoaded { get; set; } = false;


        }













        private void LoadDictionaryButton_Click(object sender, EventArgs e)
        {


            
            DictData = new DictionaryData();


           //   ____                _   _____         _     _____                        ____ ___ ____   _____ _ _      
           //  |  _ \ ___  __ _  __| | |_   _|____  _| |_  |  ___| __ ___  _ __ ___     |  _ \_ _/ ___| |  ___(_) | ___ 
           //  | |_) / _ \/ _` |/ _` |   | |/ _ \ \/ / __| | |_ | '__/ _ \| '_ ` _ \    | | | | | |     | |_  | | |/ _ \
           //  |  _ <  __/ (_| | (_| |   | |  __/>  <| |_  |  _|| | | (_) | | | | | |  _| |_| | | |___  |  _| | | |  __/
           //  |_| \_\___|\__,_|\__,_|   |_|\___/_/\_\\__| |_|  |_|  \___/|_| |_| |_| (_)____/___\____| |_|   |_|_|\___|
                                                                                                          



            string DictionaryRawText = "";

            openFileDialog.Title = "Please choose your dictionary file";

            if (openFileDialog.ShowDialog() != DialogResult.Cancel)
            {

                FolderBrowser.SelectedPath = System.IO.Path.GetDirectoryName(openFileDialog.FileName);

                //Load dictionary file now
                try
                {


                    Encoding SelectedEncoding = null;
                    SelectedEncoding = Encoding.GetEncoding(EncodingDropdown.SelectedItem.ToString());

                    DictionaryRawText = File.ReadAllText(openFileDialog.FileName, SelectedEncoding);
                }
                catch
                {
                    MessageBox.Show("RIOTLite is having trouble reading data from your dictionary file. Is it open in another application?");
                    return;
                }

            }
            else
            {
                return;
            }



            //  ____                   _       _         ____  _      _   ____        _           ___  _     _           _   
            // |  _ \ ___  _ __  _   _| | __ _| |_ ___  |  _ \(_) ___| |_|  _ \  __ _| |_ __ _   / _ \| |__ (_) ___  ___| |_ 
            // | |_) / _ \| '_ \| | | | |/ _` | __/ _ \ | | | | |/ __| __| | | |/ _` | __/ _` | | | | | '_ \| |/ _ \/ __| __|
            // |  __/ (_) | |_) | |_| | | (_| | ||  __/ | |_| | | (__| |_| |_| | (_| | || (_| | | |_| | |_) | |  __/ (__| |_ 
            // |_|   \___/| .__/ \__,_|_|\__,_|\__\___| |____/|_|\___|\__|____/ \__,_|\__\__,_|  \___/|_.__// |\___|\___|\__|
            //            |_|                                                                             |__/               




            //parse out the the dictionary file
            DictData.MaxWords = 0;

            //yeah, there's levels to this thing
            DictData.FullDictionary = new Dictionary<string, Dictionary<int, Dictionary<string, string[]>>>();

            DictData.FullDictionary.Add("Wildcards", new Dictionary<int, Dictionary<string, string[]>>());
            DictData.FullDictionary.Add("Standards", new Dictionary<int, Dictionary<string, string[]>>());

            DictData.WildCardArrays = new Dictionary<int, string[]>();
            DictData.PrecompiledWildcards = new Dictionary<string, Regex>();


            Dictionary<int, List<string>> WildCardLists = new Dictionary<int, List<string>>();

            try
            {
                string[] DicSplit = DictionaryRawText.Split(new char[] { '%' }, 3, StringSplitOptions.None);

                string[] HeaderLines = DicSplit[1].Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                string[] EntryLines = DicSplit[2].Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

                DictData.NumCats = HeaderLines.Length;

                //now that we know the number of categories, we can fill out the arrays
                DictData.CatNames = new string[DictData.NumCats];
                DictData.CatValues = new string[DictData.NumCats];


                //Map Out the Categories
                for (int i = 0; i < DictData.NumCats; i++)
                {
                    string[] HeaderRow = HeaderLines[i].Trim().Split(new char[] { '\t' }, 2);

                    DictData.CatValues[i] = HeaderRow[0];
                    DictData.CatNames[i] = HeaderRow[1];
                }


                //Map out the dictionary entries
                for (int i = 0; i < EntryLines.Length; i++)
                {

                    string EntryLine = EntryLines[i].Trim();
                    while (EntryLine.Contains("  ")) EntryLine.Replace("  ", " ");

                    string[] EntryRow = EntryLine.Trim().Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);

                    int Words_In_Entry = EntryRow[0].Split(' ').Length;
                    if (Words_In_Entry > DictData.MaxWords) DictData.MaxWords = Words_In_Entry;

                    if (EntryRow[0].Contains("*"))
                    {

                        if (DictData.FullDictionary["Wildcards"].ContainsKey(Words_In_Entry))
                        {
                            DictData.FullDictionary["Wildcards"][Words_In_Entry].Add(EntryRow[0].ToLower(), EntryRow.Skip(1).ToArray());
                            WildCardLists[Words_In_Entry].Add(EntryRow[0].ToLower());
                        }
                        else
                        {
                            DictData.FullDictionary["Wildcards"].Add(Words_In_Entry, new Dictionary<string, string[]> { { EntryRow[0].ToLower(), EntryRow.Skip(1).ToArray() } });
                            WildCardLists.Add(Words_In_Entry, new List<string>(new string[] { EntryRow[0].ToLower() }));
                        }
                        DictData.PrecompiledWildcards.Add(EntryRow[0].ToLower(), new Regex(Regex.Escape(EntryRow[0].ToLower()).Replace("\\*", ".*"), RegexOptions.Compiled));

                    }
                    else
                    {
                        if (DictData.FullDictionary["Standards"].ContainsKey(Words_In_Entry))
                        {
                            DictData.FullDictionary["Standards"][Words_In_Entry].Add(EntryRow[0].ToLower(), EntryRow.Skip(1).ToArray());
                        }
                        else
                        {
                            DictData.FullDictionary["Standards"].Add(Words_In_Entry, new Dictionary<string, string[]> { { EntryRow[0].ToLower(), EntryRow.Skip(1).ToArray() } });
                        }
                    }

                }


                for (int i = DictData.MaxWords; i > 0; i--)
                {
                    if (WildCardLists.ContainsKey(i)) DictData.WildCardArrays.Add(i, WildCardLists[i].ToArray());
                }
                WildCardLists.Clear();
                DictData.DictionaryLoaded = true;

                MessageBox.Show("Your dictionary has been successfully loaded.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            catch
            {
                MessageBox.Show("RIOTLite encountered an issue while parsing your dictionary. Please check to make sure that it is correctly formatted and that it is not currently open in another application. It is also important that your dictionary contains no duplicate entries.", "Error parsing dictionary", MessageBoxButtons.OK, MessageBoxIcon.Error);
                DictData = new DictionaryData();
                return;
            }


    }



    }
    


}
