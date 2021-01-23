using System;
using System.Collections.Generic;
using System.Text;

namespace EnkeltBKP.PowerBI.Model
{
    public class ScheduleConfiguration
    {
        public bool Activated { get; set; }
        public DateTime StartingDate { get; set; }
        public DateTime? EndDate { get; set; }
        public RecurrenceType RecurrenceType { get; set; }
        public int RecurrenceEvery { get; set; }

        public ScheduleConfiguration()
        {
            Activated = false;
            RecurrenceEvery = 1;
            RecurrenceType = RecurrenceType.Day;
            StartingDate = DateTime.Now;
            EndDate = null;
        }
    }

    public enum RecurrenceType
    {
        Hour,
        Day,
        Week,
        Month
    }
}
