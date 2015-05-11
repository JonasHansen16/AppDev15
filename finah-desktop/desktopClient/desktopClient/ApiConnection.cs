using System;
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

        static public HttpResponseMessage genericRequest(string connection, Object parameter = null)
        {
             using (var client = ApiConnection.getConnection())
             {
             // Make our request and request the results
                 HttpResponseMessage response = client.PostAsJsonAsync(connection, parameter).Result;
                 // Throw an exception if an error occurs
                 response.EnsureSuccessStatusCode();
                 return response;
             }
        }

        static public HttpResponseMessage genericRequest(string connection, Object parameter = null, int parameter2 = 0)
        {
            using (var client = ApiConnection.getConnection())
            {
                // Make our request and request the results
                HttpResponseMessage response = client.PostAsJsonAsync(connection,parameter).Result;
                // Throw an exception if an error occurs
                response.EnsureSuccessStatusCode();
                return response;
            }
        }


    }
}
