using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace nah_backend.Models
{
    public class Question
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }

        public Question(int id = 0, string title = "", string text = "")
        {
            Id = id;
            Title = title;
            Text = text;
        }
    }
}