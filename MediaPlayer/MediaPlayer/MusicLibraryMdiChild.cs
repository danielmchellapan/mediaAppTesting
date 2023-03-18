﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace MediaPlayer
{
    public partial class MusicLibraryForm : Form
    {

        // Declare variables and collections
        private Dictionary<string, Audio> titleToAudioLookup;
        Audio selectedAudio;
        List<Audio> songs = new List<Audio>();
        List<Audio> Queue = new List<Audio>();

        /// <summary>
        /// Constructor for the MusicLibraryForm class
        /// </summary>
        public MusicLibraryForm()
        {
            InitializeComponent();
            getSongs();
            titleToAudioLookup = songs.ToDictionary(audio => audio.title);
            SongList.View = View.Details;
            SongList.Columns.Add("Songs: ");
            SongList.Columns[0].Width = 600;
            SongList.ItemSelectionChanged += myListView_ItemSelectionChanged;
            
            fillList();
            SongList.Width = Width;
            fillPictures();
            
        }

        /// <summary>
        /// Fills the form with pictures
        /// </summary>
        public void fillPictures()
        {
            int numOfCompatible = 0;
            List<PictureBox> pictureBoxes = new List<PictureBox>
            {
                pictureBox1,
                pictureBox2,
                pictureBox3,
                pictureBox4
            };
            for (int i = 0; i < pictureBoxes.Count; i++)
            {
                pictureBoxes[i].SizeMode = PictureBoxSizeMode.StretchImage;
            }
            for (int i = 0; i < songs.Count && numOfCompatible < 4; i++)
            {
                if (songs[i].albumArt != null)
                {
                    pictureBoxes[numOfCompatible].Image = songs[i].albumArt;
                    numOfCompatible++;
                }
            }
        }

        /// <summary>
        /// Fills the queue with the songs
        /// </summary>
        void fillQueue()
        {
            Queue.Clear();
            bool add = false;
            for (int i = 0; i < songs.Count; i++)
            {
                if (songs[i] == selectedAudio)
                {
                    add = true;
                }
                if (add)
                {
                    Queue.Add(songs[i]);
                }
            }
        }

        /// <summary>
        /// Fills the ListView with the songs
        /// </summary>
        void fillList()
        {
            for (int i = 0; i < songs.Count; i++)
            {
                StringBuilder titleBuilder = new StringBuilder();
                StringBuilder artistBuilder = new StringBuilder();
                StringBuilder durationBuilder = new StringBuilder();
                if (songs[i] != null)
                {
                    titleBuilder.Append(songs[i].title);
                    artistBuilder.Append(songs[i].getArtists());
                    if (songs[i].duration == null)
                    {
                        durationBuilder.Append("Unknown");
                    }
                    else
                    {
                        durationBuilder.Append(songs[i].duration.ToString());
                    }
                    ListViewItem songItem = new ListViewItem();
                    songItem.Name = titleBuilder.ToString();
                    format(ref titleBuilder);
                    format(ref artistBuilder);
                    songItem.Text = titleBuilder.ToString()
                        + artistBuilder.ToString() + durationBuilder.ToString();

                    SongList.Items.Add(songItem);
                }
            }
        }

        /// <summary>
        /// Formats the StringBuilder instance to a specific length
        /// </summary>
        void format(ref StringBuilder stringBuilder)
        {
            if (stringBuilder.Length >= 50)
            {
                int lengthTaken = stringBuilder.Length - 50;
                for (int j = lengthTaken; j > 0; j--)
                {
                    stringBuilder.Remove(stringBuilder.Length - 1, 1);
                }
            }
            if (stringBuilder.Length < 50)
            {
                int lengthAdded = 50 - stringBuilder.Length;
                for (int j = 0; j < lengthAdded; j++)
                {
                    stringBuilder.Append(' ');
                }
            }
        }

        /// <summary>
        /// Retrieves the songs from MediaScanner
        /// </summary>
        private void getSongs()
        {
            for (int i = 0; i < MediaScanner.Audios.Count; i++)
            {
                songs.Add(MediaScanner.Audios[i]);
            }
        }

        /// <summary>
        /// Performs a weighted shuffle on the list
        /// </summary>
        public static void WeightedShuffle<T>(IList<T> list, Func<T, T, double> weightFunc)
        {
            Random random = new Random();

            for (int i = list.Count - 1; i > 0; i--)
            {
                List<double> cumulativeWeights = new List<double>();
                double totalWeight = 0;

                for (int j = 0; j <= i; j++)
                {
                    double weight = weightFunc(list[i], list[j]);
                    totalWeight += weight;
                    cumulativeWeights.Add(totalWeight);
                }

                double randomWeight = random.NextDouble() * totalWeight;

                int selectedIndex = cumulativeWeights.FindIndex(w => w > randomWeight);

                if (selectedIndex < 0)
                {
                    selectedIndex = i;
                }

                T temp = list[i];
                list[i] = list[selectedIndex];
                list[selectedIndex] = temp;
            }
        }

        /// <summary>
        /// Calculates the Jaro-Winkler similarity between two strings
        /// </summary>
        public static double JaroWinklerSimilarity(string s1, string s2)
        {
            int m = 0;
            int n = s1.Length;
            int p = s2.Length;

            if (n == 0 || p == 0)
            {
                return 0;
            }

            int range = Math.Max(0, Math.Max(n, p) / 2 - 1);

            bool[] s1Matches = new bool[n];
            bool[] s2Matches = new bool[p];

            for (int i = 0; i < n; i++)
            {
                int start = Math.Max(0, i - range);
                int end = Math.Min(i + range + 1, p);

                for (int j = start; j < end; j++)
                {
                    if (s2Matches[j]) continue;
                    if (s1[i] != s2[j]) continue;

                    s1Matches[i] = true;
                    s2Matches[j] = true;
                    m++;
                    break;
                }
            }

            if (m == 0)
            {
                return 0;
            }

            int k = 0;
            int numTranspositions = 0;
            for (int i = 0; i < n; i++)
            {
                if (!s1Matches[i]) continue;

                while (!s2Matches[k]) k++;

                if (s1[i] != s2[k]) numTranspositions++;

                k++;
            }

            double jaro = ((double)m / n + (double)m / p + (double)(m - numTranspositions / 2) / m) / 3;

            int prefixLength = 0;

            for (int i = 0; i < Math.Min(4, Math.Min(n, p)); i++)
            {
                if (s1[i] == s2[i]) prefixLength++;
                else break;
            }

            return jaro + prefixLength * 0.1 * (1 - jaro);
        }

        /// <summary>
        /// Handles the event when the selected item in the ListView changes
        /// </summary>
        private void myListView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (e.IsSelected)
            {
                ListViewItem selectedItem = e.Item;
                selectedAudio = null;
                titleToAudioLookup.TryGetValue(selectedItem.Name, out selectedAudio);

                if (selectedAudio != null)
                {
                    MainForm parent = (MainForm)this.MdiParent;
                    fillQueue();
                    parent.FillQueue(Queue);
                }
            }
        }

        /// <summary>
        /// Handles the click event for the shuffle button
        /// </summary>
        private void shuffleButton_Click(object sender, EventArgs e)
        {
            List<Audio> songCandidates = new List<Audio>(songs);
            WeightedShuffle(songCandidates, (a, b)
                => JaroWinklerSimilarity(a.getArtists(), b.getArtists()));
            Queue = new List<Audio>(songCandidates);
            MainForm parent = (MainForm)MdiParent;
            parent.FillQueue(Queue);

        }

        /// <summary>
        /// Handles the click event for the play button
        /// </summary>
        private void playButton_Click(object sender, EventArgs e)
        {
            Queue = new List<Audio>(songs);
            MainForm parent = (MainForm)this.MdiParent;
            parent.FillQueue(Queue);
        }
    }
}