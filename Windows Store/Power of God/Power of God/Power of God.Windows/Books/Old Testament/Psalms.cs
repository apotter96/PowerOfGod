﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Power_of_God.Books.Old_Testament
{
    public class Psalms : Book
    {
        public override string Name { get; set; } = "Psalms";

        public override int BookNum { get; set; } = 19;

        public override int ChapterCount { get; set; } = 150;
    }
}
