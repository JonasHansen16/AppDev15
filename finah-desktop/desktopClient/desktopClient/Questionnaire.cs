using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sprint_1_def
{
    public class Questionnaire
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Intro { get; set; }

        public Questionnaire(int id = 0, string title = "", string description = "", string intro = "")
        {
            Id = id;
            Title = title;
            Description = description;
            Intro = intro;
        }
    }
}