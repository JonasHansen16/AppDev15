﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace sprint_1_def
{
    class ApiConnection
    {
        static public HttpClient getConnection()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            return client;
        }
    }
}
