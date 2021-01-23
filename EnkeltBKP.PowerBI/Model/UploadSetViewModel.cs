using Microsoft.PowerBI.Api.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnkeltBKP.PowerBI.Model
{
    internal class UploadSetViewModel
    {
        public Group Workspace { get; set; }
        public Report Report { get; set; }
    }
}
