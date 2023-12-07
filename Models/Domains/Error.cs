﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrorsReporting.Models.Domains
{
    public class Error
    {   
        public int Id { get; set; }
        public string Message { get; set; }
        public DateTime Date { get; set; }
        public bool WasSend { get; set; }
    }
}
