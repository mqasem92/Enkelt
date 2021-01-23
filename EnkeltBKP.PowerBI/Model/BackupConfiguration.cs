using System;
using System.Collections.Generic;
using System.Text;

namespace EnkeltBKP.PowerBI.Model
{
    public class BackupConfiguration
    {
        /// <summary>
        /// Folder name for the backup files
        /// </summary>
        public string Folder { get; set; } = "Backup";

        /// <summary>
        /// File extension for the backup file
        /// </summary>
        public string FileExtension { get; set; } = "pbix";

        /// <summary>
        /// Add date in backup file name
        /// </summary>
        public bool AddDatePostfix { get; set; } = true;

        /// <summary>
        /// Add time in backup file name
        /// </summary>
        public bool AddTimePostfix { get; set; } = true;

        public ScheduleConfiguration ScheduleConfiguration { get; set; }

        public BackupConfiguration()
        {
            ScheduleConfiguration = new ScheduleConfiguration();
        }

        public BackupConfiguration(ScheduleConfiguration scheduleConfiguration)
        {
            ScheduleConfiguration = scheduleConfiguration;
        }
    }
}
