using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetflixHistory
{
    public class ViewedItem
    {
        public string Title { get; set; }
        public int MovieId { get; set; }
        public int Bookmark { get; set; }
        public int Duration { get; set; }
        /// <summary>
        /// The raw integer value of the date and time watched, as returned by Netflix.
        /// </summary>
        public Int64 Date { get; set; }
        public string DateStr { get; set; }
        public int Index { get; set; }
        public string TopNodeId { get; set; }
        public string Rating { get; set; }

        public DateTime ViewedOn
        {
            get
            {
                var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

                var seconds = Convert.ToDouble(Date) / 1000;

                return epoch.AddSeconds(seconds);
            }
        }
    }
}
