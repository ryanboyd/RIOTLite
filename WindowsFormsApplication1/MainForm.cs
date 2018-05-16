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

            openFileDialog.Title = "Please choose your dictionary file";

            if (openFileDialog.ShowDialog() != DialogResult.Cancel)
            {



                //Load dictionary file now
                try
                {


                    Encoding SelectedEncoding = null;
                    SelectedEncoding = Encoding.GetEncoding(EncodingDropdown.SelectedItem.ToString());

                    string Dictionary = File.ReadAllText(openFileDialog.FileName, SelectedEncoding);

                    FolderBrowser.SelectedPath = System.IO.Path.GetDirectoryName(openFileDialog.FileName);

                    FolderBrowser.Description = "Please choose the location of your .txt files to analyze";
                    if (FolderBrowser.ShowDialog() != DialogResult.Cancel) {

                        string TextFileFolder = FolderBrowser.SelectedPath.ToString();

                        if (TextFileFolder != "")
                        {

                            saveFileDialog.FileName = "RIOTLite.csv";

                            saveFileDialog.InitialDirectory = TextFileFolder;
                            if (saveFileDialog.ShowDialog() != DialogResult.Cancel) {


                                string OutputFileLocation = saveFileDialog.FileName;

                                if (OutputFileLocation != "") {


                                    StartButton.Enabled = false;
                                    ScanSubfolderCheckbox.Enabled = false;
                                    PunctuationBox.Enabled = false;
                                    EncodingDropdown.Enabled = false;
                                    BgWorker.RunWorkerAsync(new string[] { TextFileFolder, OutputFileLocation, Dictionary });
                                }
                            }
                        }

                    }

                }
                catch
                {
                    MessageBox.Show("RIOTLite is having trouble reading your dictionary file. Is it open in another application?");
                }

            }

        }

        




        private void BgWorker_DoWork(object sender, DoWorkEventArgs e)
        {

            string Dictionary = ((string[])e.Argument)[2];

            //parse out the the dictionary file
            int NumCats;
            int MaxWords = 0;

            string[] CatNames;
            string[] CatValues;

            //yeah, there's levels to this thing
            Dictionary<string, Dictionary<int, Dictionary<string, string[]>>> FullDictionary = new Dictionary<string, Dictionary<int, Dictionary<string, string[]>>>();

            FullDictionary.Add("Wildcards", new Dictionary<int, Dictionary<string, string[]>>());
            FullDictionary.Add("Standards", new Dictionary<int, Dictionary<string, string[]>>());

            Dictionary<int, List<string>> WildCardLists = new Dictionary<int, List<string>>();
            Dictionary<int, string[]> WildCardArrays = new Dictionary<int, string[]>();
            Dictionary<string, Regex> PrecompiledWildcards = new Dictionary<string, Regex>();
            
            try
            {
                string[] DicSplit = Dictionary.Split(new char[] { '%'}, 3, StringSplitOptions.None);

                string[] HeaderLines = DicSplit[1].Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                string[] EntryLines = DicSplit[2].Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

                NumCats = HeaderLines.Length;

                //now that we know the number of categories, we can fill out the arrays
                CatNames = new string[NumCats];
                CatValues = new string[NumCats];


                //Map Out the Categories
                for (int i = 0; i < NumCats; i++)
                {
                    string[] HeaderRow = HeaderLines[i].Trim().Split(new char[] { '\t' }, 2);
                    
                    CatValues[i] = HeaderRow[0];
                    CatNames[i] = HeaderRow[1];
                }


                //Map out the dictionary entries
                for (int i = 0; i < EntryLines.Length; i++)
                {

                    string EntryLine = EntryLines[i].Trim();
                    while (EntryLine.Contains("  ")) EntryLine.Replace("  ", " ");

                    string[] EntryRow = EntryLine.Trim().Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);

                    int Words_In_Entry = EntryRow[0].Split(' ').Length;
                    if (Words_In_Entry > MaxWords) MaxWords = Words_In_Entry;

                    if (EntryRow[0].Contains("*"))
                    {

                        if (FullDictionary["Wildcards"].ContainsKey(Words_In_Entry))
                        {
                            FullDictionary["Wildcards"][Words_In_Entry].Add(EntryRow[0].ToLower(), EntryRow.Skip(1).ToArray());
                            WildCardLists[Words_In_Entry].Add(EntryRow[0].ToLower());
                        }
                        else
                        {
                            FullDictionary["Wildcards"].Add(Words_In_Entry, new Dictionary<string, string[]> {{EntryRow[0].ToLower(), EntryRow.Skip(1).ToArray() }});
                            WildCardLists.Add(Words_In_Entry, new List<string>(new string[] { EntryRow[0].ToLower() }));
                        }
                        PrecompiledWildcards.Add(EntryRow[0].ToLower(), new Regex(Regex.Escape(EntryRow[0].ToLower()).Replace("\\*", ".*"), RegexOptions.Compiled));

                    }
                    else
                    {
                        if (FullDictionary["Standards"].ContainsKey(Words_In_Entry))
                        {
                            FullDictionary["Standards"][Words_In_Entry].Add(EntryRow[0].ToLower(), EntryRow.Skip(1).ToArray());
                        }
                        else
                        {
                            FullDictionary["Standards"].Add(Words_In_Entry, new Dictionary<string, string[]> { { EntryRow[0].ToLower(), EntryRow.Skip(1).ToArray() } });
                        }
                    }

                }


                for (int i = MaxWords; i > 0; i--)
                {
                    if (WildCardLists.ContainsKey(i)) WildCardArrays.Add(i, WildCardLists[i].ToArray());
                }
                WildCardLists.Clear();


            }
            catch
            {
                MessageBox.Show("RIOTLite encountered an issue while parsing your dictionary. Please check to make sure that it is correctly formatted (and contains no duplicate entries) and that it is not currently open in another application.");
                return;
            }








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
            var files = Directory.EnumerateFiles( ((string[])e.Argument)[0], "*.txt", SearchDepth);



            try { 
                using (StreamWriter outputFile = new StreamWriter(((string[])e.Argument)[1]))
                {

                    //write the header row to the output file
                    StringBuilder HeaderString = new StringBuilder();
                    HeaderString.Append("\"Filename\",\"WC\",\"DictPercent\"");
                    for (int i = 0; i < NumCats; i++) HeaderString.Append("," + CatNames[i].Replace("\"", "\"\""));
                    outputFile.WriteLine(HeaderString.ToString());


                    foreach (string fileName in files)
                    {

                        //set up our variables to report
                        string Filename_Clean = Path.GetFileName(fileName);
                        Dictionary<string, int> DictionaryResults = new Dictionary<string, int>();
                        for (int i = 0; i < NumCats; i++) DictionaryResults.Add(CatValues[i], 0);

                        //report what we're working on
                        FilenameLabel.Invoke((MethodInvoker)delegate
                        {
                            FilenameLabel.Text = "Analyzing: " + Filename_Clean;
                        });




                        //do stuff here
                        string readText = File.ReadAllText(fileName, SelectedEncoding).ToLower();
                        readText = NewlineClean.Replace(readText, " ");

                        //remove all the junk punctuation
                        foreach (char c in PunctuationBox.Text)
                        {
                            readText = readText.Replace(c, ' ');
                        }


                        int NumberOfMatches = 0;
                        

                        //splits everything out into words
                        string[] Words = readText.Trim().Split(' ');

                        Words = Words.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
                        int TotalStringLength = Words.Length;






                        //now we actually conduct the analysis
                        for (int i = 0; i < TotalStringLength; i++)
                        {

                            //MessageBox.Show(i.ToString());


                            for (int NumberOfWords = MaxWords; NumberOfWords > 0; NumberOfWords--)
                                {

                                

                                //make sure that we don't overextend past the array
                                if (i + NumberOfWords >= TotalStringLength) continue;

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

                            if (FullDictionary["Standards"].ContainsKey(NumberOfWords)) { 
                                if (FullDictionary["Standards"][NumberOfWords].ContainsKey(TargetString))
                                {

                                    NumberOfMatches += NumberOfWords;
                                    //add in the number of words found
                                    for (int j = 0; j < FullDictionary["Standards"][NumberOfWords][TargetString].Length; j++)
                                    {

                                        if (DictionaryResults.ContainsKey(FullDictionary["Standards"][NumberOfWords][TargetString][j])) DictionaryResults[FullDictionary["Standards"][NumberOfWords][TargetString][j]] += NumberOfWords;
                                        
                                    } 
                                    //manually increment the for loop so that we're not testing on words that have already been picked up
                                    i += NumberOfWords - 1;
                                    //break out of the lower level for loop back to moving on to new words altogether
                                    break;
                                }
                             }
                            //if there isn't an exact match, we have to go through the wildcards
                            else
                                {

                                    if (WildCardArrays.ContainsKey(NumberOfWords)) { 
                                        for (int j = 0; j < WildCardArrays[NumberOfWords].Length; j++)
                                        {
                                            if (PrecompiledWildcards[WildCardArrays[NumberOfWords][j]].Matches(TargetString).Count > 0)
                                            {

                                                NumberOfMatches += NumberOfWords;

                                                for (int k = 0; k < FullDictionary["Wildcards"][NumberOfWords][WildCardArrays[NumberOfWords][j]].Length; k++)
                                                {

                                                    if (DictionaryResults.ContainsKey(FullDictionary["Wildcards"][NumberOfWords][WildCardArrays[NumberOfWords][j]][k])) DictionaryResults[FullDictionary["Wildcards"][NumberOfWords][WildCardArrays[NumberOfWords][j]][k]] += NumberOfWords;

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



                        }











                        //pull together the output
                        string[] OutputString = new string[3 + NumCats];
                        OutputString[0] = "\"" + Filename_Clean + "\"";
                        OutputString[1] = TotalStringLength.ToString();

                        if (TotalStringLength > 0) {

                            OutputString[2] = (((double)NumberOfMatches / TotalStringLength) * 100).ToString();
                            for (int i = 0; i < NumCats; i++) OutputString[i + 3] = (((double)DictionaryResults[CatValues[i]] / TotalStringLength)*100).ToString();
                            
                        }
                        else
                        {
                            OutputString[2] = "";
                            for (int i = 0; i < NumCats; i++) OutputString[i + 3] = "";
                        }


                    outputFile.WriteLine(String.Join(",", OutputString));
         
                            

     

                
       

                }


            }

            }
            catch
            {
                MessageBox.Show("RIOTLite encountered an issue somewhere while trying to analyze your texts. The most common cause of this is trying to open your output file while RIOTLite is still running. Did any of your input files move, or is your output file being opened/modified by another application?");
            }



            
        }

        private void BgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            StartButton.Enabled = true;
            ScanSubfolderCheckbox.Enabled = true;
            PunctuationBox.Enabled = true;
            EncodingDropdown.Enabled = true;
            FilenameLabel.Text = "Finished!";
            MessageBox.Show("RIOTLite has finished analyzing your texts.", "Analysis Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void WordWindowSizeTextbox_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }



    }
    


}
