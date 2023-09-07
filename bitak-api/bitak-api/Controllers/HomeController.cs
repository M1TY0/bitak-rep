using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Mvc;
using System.Security.Principal;

namespace bitak_api.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class User : ControllerBase
    {
        [HttpGet]
        public void createUser(int id, string username, string password, string email)
        {
            string connection = "SERVER = localhost; database = bitakdb; uid = root; password= 1234;";
            MySqlConnection conn = new MySqlConnection(connection);
            conn.Open();
            string query1 = "INSERT INTO users(id,username,password,email,date) VALUES(" + id + ",'" + username + "','" + password + "','" + email + "',NOW())";
            MySqlCommand cmd1 = new MySqlCommand(query1, conn);
            cmd1.ExecuteScalar();
        }
    }
    public class Password : Controller
    {
        
        [Route("[controller]")]
        [HttpGet]
        public bool checkPassword(string email)
        {
            string connection = "SERVER = localhost; database = bitakdb; uid = root; password= 1234;";
            MySqlConnection conn = new MySqlConnection(connection);
            conn.Open();
            string query1 = "SELECT * FROM users WHERE email = '" + email + "';";
            MySqlCommand cmd1 = new MySqlCommand(query1, conn);
            try
            {
                cmd1.ExecuteScalar().ToString();
            }
            catch
            {
                return false;
            }
            return true;
        }
    
}
       
}
