﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetBible.Books;

namespace NetBible.Books.Old_Testament
{
    public class Hosea : Book
    {
        public override string Name { get; set; } = "Hosea";

        public override int BookNum { get; set; } = 28;

        public override int ChapterCount { get; set; } = 14;
    }
}