using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookTracker
{
    public class Activity
    {
        public int ActivityId { get; set; }
        public int UserId { get; set; }
        public int BookId { get; set; }
        public string BookStatus { get; set; } // "onreadlist", "reading", "completed", "dropped"
        public int ProgressCompleted { get; set; } // As a percentage
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public override string ToString()
        {
            return $"ActivityId: {ActivityId}, UserId: {UserId}, BookId: {BookId}, Status: {BookStatus}, Progress: {ProgressCompleted}%, Start: {StartDate?.ToShortDateString()}, End: {EndDate?.ToShortDateString()}";
        }
    }
}