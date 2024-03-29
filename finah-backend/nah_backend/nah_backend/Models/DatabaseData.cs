﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace nah_backend.Models
{
    public static class DatabaseData
    {
        public static int default_maxlen = 255;

        public static class User
        {
            public static class Name
            {
                public static int maxlen = 50;
                public static int minlen = 1;
            }
            public static class LastName
            {
                public static int maxlen = 50;
                public static int minlen = 1;
            }
            public static class Email
            {
                public static int minlen = 3;
            }
            public static class UserName
            {
                public static int maxlen = 20;
                public static int minlen = 4;
            }
            public static class Password
            {
                public static int minlen = 20;
            }
        }
        public static class Answer
        {
            public static class Score
            {
                public static int minval = 1;
                public static int maxval = 5;
            }
            public static class Help
            {
                public static int minscore = 3;
            }
        }
        public static class Form
        {
            public static class Memo
            {
                public static int maxlen = 100;
            }
            public static class Category
            {
                public static int maxlen = 100;
            }
            public static class Relation
            {
                public static int maxlen = 100;
            }
            public static class ClientList
            {
                public static int minlen = 1;
            }
        }
        public static class Client
        {
            public static class Hash
            {
                public static int maxlen = 20;
            }
            public static class Function
            {
                public static int maxlen = 255;
            }
        }
        public static class UserList
        {
            public static int maxlen = 10;
        }
        public static class FormList
        {
            public static int maxlen = 10;
        }
    }
}