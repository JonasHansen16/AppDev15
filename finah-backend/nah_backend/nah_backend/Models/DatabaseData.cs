using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace nah_back.Models
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
                public static int minlen = 255;
            }
        }


    }
}