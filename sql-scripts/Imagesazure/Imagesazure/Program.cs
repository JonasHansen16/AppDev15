using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Data.SqlClient;
using System.IO;

namespace Imagesazure
{
    class Program
    {
        private static string _qImageInsert = "UPDATE question set img = @Img where id = @id;";
        private static string[] imagepaths = { "d155.PNG", "d160.PNG", "d163.PNG" };
        static void Main(string[] args)
        {
            SqlConnection client = getConnection();
            client.Open();
           
            for (int i = 1; i < imagepaths.Length+1; i++)
            {
                SqlCommand insertCommand = new SqlCommand(_qImageInsert, client);
                insertCommand.Parameters.AddWithValue("@Img", GetImage(i));
                insertCommand.Parameters.AddWithValue("@id", i + 3);
                insertCommand.ExecuteNonQuery();
            }
            

            
        }

        static private SqlConnection getConnection()
        {
           return new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["nah_remote"].ConnectionString);
        }

        static private byte[] GetImage(int id)
        {
            id--;
            return File.ReadAllBytes(Path.Combine("../../Image", imagepaths[id]));
            
        }

        
    }
}
