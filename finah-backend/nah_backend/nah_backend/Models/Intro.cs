using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace nah_backend.Models
{
    public class Intro
    {
        public string Title { get; set; }
        public string Text { get; set; }

        public Intro(string title = "", string text = "")
        {
            Title = title;
            Text = text;
        }
    }
}