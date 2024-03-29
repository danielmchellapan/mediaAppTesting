﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaPlayer
{
    public class Audio
    {

        /// <summary>
        /// Title property.
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// File location property.
        /// </summary>
        public string fileLocation { get; set; }

        /// <summary>
        /// Album property.
        /// </summary>
        public string album { get; set; }

        /// <summary>
        /// artists array property.
        /// </summary>
        public string[] artists { get; set; }

        /// <summary>
        /// Genres array property
        /// </summary>
        public string[] genres { get; set; }

        /// <summary>
        /// Duration property.
        /// </summary>
        public string duration { get; set; }

        /// <summary>
        /// Cover Art property.
        /// </summary>
        public Image albumArt { get; set; }

        public Audio()
        {
            title = null;
            fileLocation = null;
            album = null;
            artists = null;
            genres = null;
            duration = null;
            albumArt = null;
        }

        /// <summary>
        /// Returns the artists array in a properly formated string.
        /// By Shayan Zahedanaraki
        /// </summary>
        /// <returns>properly formated string of artists</returns>
        public string getArtists()
        {
            StringBuilder sb = new StringBuilder();
            if (artists == null)
            {
                return "Unknown";
            }
            if (artists.Length == 1)
            {
                sb.Append(artists[0]);
            } else if (artists != null || artists.Length != 0)
            {
                for (int i = 0; i < artists.Length-1; i++)
                {
                    if (i == artists.Length - 1)
                    {
                        sb.Append(artists[i]);
                    } else {
                        sb.Append(artists[i] + ", ");
                    }
                }
            }
            return sb.ToString();
        }
    }
}
