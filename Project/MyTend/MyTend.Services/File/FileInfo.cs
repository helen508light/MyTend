﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTend.Services.File
{
    public class FileInfo
    {
        public string MimeType { get; set; }

        public Stream Data { get; set; }

        public string Name { get; set; }

        public bool IsAvatar { get; set; }
    }
}
