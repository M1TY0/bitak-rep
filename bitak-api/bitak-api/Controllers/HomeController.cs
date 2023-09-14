using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Mvc;
using System.Security.Principal;
using System;
using System.Configuration;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using Org.BouncyCastle.Crypto;
namespace bitak_api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class User : ControllerBase
    {
        [HttpPost]

        public string CreateUser(string email, string username, string password, string pictureURL, string location)
        {
            string connection = "SERVER = localhost; database = bitakdb; uid = root; password= 1234;";
            MySqlConnection conn = new MySqlConnection(connection);
            conn.Open();
            string query1 = "SELECT * FROM users WHERE email = '" + email + "';";
            MySqlCommand cmd1 = new MySqlCommand(query1, conn);
            string query2 = "SELECT * FROM users WHERE username = '" + username + "';";
            MySqlCommand cmd2 = new MySqlCommand(query2, conn);
            try
            {
                cmd2.ExecuteScalar().ToString();
            }
            catch
            {
                try
                {
                    cmd1.ExecuteScalar().ToString();
                }
                catch
                {
                    int findid()
                    {
                        string connection = "SERVER = localhost; database = bitakdb; uid = root; password= 1234;";
                        MySqlConnection conn = new MySqlConnection(connection);
                        conn.Open();
                        string query1 = "SELECT COUNT(*) FROM users";
                        MySqlCommand cmd1 = new MySqlCommand(query1, conn);
                        return int.Parse(cmd1.ExecuteScalar().ToString());
                    }
                    string query3 = "INSERT INTO users(id,username,password,email,date,pictureURL,rating,number_of_ratings,location) VALUES(" + findid() + ",'" + username + "','" + password + "','" + email + "',NOW(),'" + pictureURL + "',0,0,'" + location + "');";
                    MySqlCommand cmd3 = new MySqlCommand(query3, conn);
                    cmd3.ExecuteScalar();
                    return "done";
                }
                return "email awready exists";
            }
            return "username awaredy exists";
        }
        [HttpDelete]
        public string DeleteUser(string accaount)
        {
            string connection = "SERVER = localhost; database = bitakdb; uid = root; password= 1234;";
            MySqlConnection conn = new MySqlConnection(connection);
            conn.Open();
            string queryid;
            int id;
            try
            {
                queryid = "SELECT id from users WHERE username ='" + accaount + "';";
                MySqlCommand cmdid = new MySqlCommand(queryid, conn);
                id = int.Parse(cmdid.ExecuteScalar().ToString());
            }
            catch
            {
                return "account doesnt exist";
            }
            string query1 = "DELETE FROM users WHERE username ='" + accaount + "';";
            MySqlCommand cmd1 = new MySqlCommand(query1, conn);
            cmd1.ExecuteScalar();
            string query2 = "SELECT COUNT(*) FROM products";
            MySqlCommand cmd2 = new MySqlCommand(query2, conn);
            int allProducts = int.Parse(cmd2.ExecuteScalar().ToString());
            for (int i = 0; i < allProducts; i++)
            {
                string query = "SELECT idCreater FROM products WHERE id = " + i;
                MySqlCommand cmd = new MySqlCommand(query, conn);
                int idp = int.Parse(cmd.ExecuteScalar().ToString());
                if (idp == id)
                {
                    string query5 = "DELETE FROM products WHERE id = " + i;
                    MySqlCommand cmd5 = new MySqlCommand(query5, conn);
                    cmd5.ExecuteScalar();
                }
            }
            return "done";
        }
        public class UserClass
        {
            public string pictureURL { get; set; }
            public int id { get; set; }
            public string email { get; set; }
            public string location { get; set; }
            public double rating { get; set; }
        }
        [HttpPut]
        public string ratingUpdate(string username, double rate)
        {
            try
            {
                if (rate < 1 || rate > 5)
                {
                    throw new Exception("Custom error message");
                }
                double rating, ratingnumber, newrating;
                string connection = "SERVER = localhost; database = bitakdb; uid = root; password= 1234;";
                MySqlConnection conn = new MySqlConnection(connection);
                conn.Open();
                string query1 = "SELECT rating FROM users WHERE username = '" + username + "';";
                MySqlCommand cmd1 = new MySqlCommand(query1, conn);
                rating = double.Parse(cmd1.ExecuteScalar().ToString());
                string query2 = "SELECT number_of_ratings FROM users WHERE username = '" + username + "';";
                MySqlCommand cmd2 = new MySqlCommand(query2, conn);
                ratingnumber = int.Parse(cmd2.ExecuteScalar().ToString());
                double numRate = rating * ratingnumber + rate;
                ratingnumber++;
                newrating = numRate / ratingnumber;
                string query3 = "UPDATE users SET rating =" + newrating + " WHERE username = '" + username + "';";
                MySqlCommand cmd3 = new MySqlCommand(query3, conn);
                cmd3.ExecuteScalar();
                string query4 = "UPDATE users SET number_of_ratings =" + ratingnumber + " WHERE username = '" + username + "';";
                MySqlCommand cmd4 = new MySqlCommand(query4, conn);
                cmd4.ExecuteScalar();
            }
            catch
            {
                return "wrong username";
            }
            {
                return "done";
            }
        }
        [HttpGet]
        public object UserInfo(string username)
        {
            string connection = "SERVER = localhost; database = bitakdb; uid = root; password= 1234;";
            MySqlConnection conn = new MySqlConnection(connection);
            conn.Open();
            string pictureURL, email;
            int id;
            UserClass user = new UserClass();
            try
            {
                string query1 = "SELECT pictureURL FROM users WHERE username = '" + username + "';";
                MySqlCommand cmd1 = new MySqlCommand(query1, conn);
                user.pictureURL = cmd1.ExecuteScalar().ToString();
                string query2 = "SELECT id FROM users WHERE username = '" + username + "';";
                MySqlCommand cmd2 = new MySqlCommand(query2, conn);
                user.id = int.Parse(cmd2.ExecuteScalar().ToString());
                string query3 = "SELECT email FROM users WHERE username = '" + username + "';";
                MySqlCommand cmd3 = new MySqlCommand(query3, conn);
                user.email = cmd1.ExecuteScalar().ToString();
                string queryrating = "SELECT rating FROM users WHERE username = '" + username + "';";
                MySqlCommand cmdrating = new MySqlCommand(queryrating, conn);
                user.rating = double.Parse(cmdrating.ExecuteScalar().ToString());
                string querylocation = "SELECT location FROM users WHERE username = '" + username + "';";
                MySqlCommand cmdlocation = new MySqlCommand(querylocation, conn);
                user.location = cmdlocation.ExecuteScalar().ToString();
            }
            catch
            {
                return "wrong username";
            }
            return user;
        }
    }
    public class Password : Controller
    {
        [Route("[controller]")]
        [HttpGet]
        public string passwordChecker(string username, string password)
        {
            string connection = "SERVER = localhost; database = bitakdb; uid = root; password= 1234;";
            MySqlConnection conn = new MySqlConnection(connection);
            conn.Open();
            string query1 = "SELECT password FROM users WHERE username = '" + username + "';";
            MySqlCommand cmd1 = new MySqlCommand(query1, conn);
            string password1;
            try
            {
                password1 = cmd1.ExecuteScalar().ToString();
            }
            catch
            {
                return "no such user";
            }
            if (password1 == password)
            {
                return "correct password";
            }
            else
            {
                return "wrong password";
            }
        }
    }
    public class Product : Controller
    {
        [Route("[controller]")]
        [HttpPost]
        public void CreateProduct(string name, int id, string pictureURL, double price, string description)
        {
            string connection = "SERVER = localhost; database = bitakdb; uid = root; password= 1234;";
            MySqlConnection conn = new MySqlConnection(connection);
            conn.Open();
            int findid()
            {
                string connection = "SERVER = localhost; database = bitakdb; uid = root; password= 1234;";
                MySqlConnection conn = new MySqlConnection(connection);
                conn.Open();
                string query1 = "SELECT COUNT(*) FROM products";
                MySqlCommand cmd1 = new MySqlCommand(query1, conn);
                return int.Parse(cmd1.ExecuteScalar().ToString());
            }
            string query1 = "INSERT INTO products(id,nameProduct,idCreater,pictureURL,price,date,description) values(" + findid() + ",'" + name + "'," + id + ",'+" + pictureURL + "'," + price + ",NOW(),'" + description + "');";
            MySqlCommand cmd1 = new MySqlCommand(query1, conn);
            cmd1.ExecuteScalar();
        }
        public class Products
        {
            public string status { get; set; }
            public string pictureURL { get; set; }
            public double price { get; set; }
            public string name { get; set; }
        }
        public class HanddleError
        {
            public string status = "wrong id";
        }

        [Route("[controller]")]
        [HttpGet]
        public List<Products> AllProdutsOfAUser(int id)
        {
            string connection = "SERVER = localhost; database = bitakdb; uid = root; password= 1234;";
            MySqlConnection conn = new MySqlConnection(connection);
            conn.Open();
            List<Products> products = new List<Products>();
            string query1 = "SELECT COUNT(*) FROM products";
            MySqlCommand cmd1 = new MySqlCommand(query1, conn);
            int allProducts = int.Parse(cmd1.ExecuteScalar().ToString());
            for (int i = 0; i < allProducts; i++)
            {
                Products product = new Products();
                int idcreater = 0;
                try
                {
                    string query2 = "SELECT idCreater FROM products WHERE id = " + i;
                    MySqlCommand cmd2 = new MySqlCommand(query2, conn);
                    idcreater = int.Parse(cmd2.ExecuteScalar().ToString());
                }
                catch
                {
                }
                if (id == idcreater)
                {
                    product.status = "done";
                    string query3 = "SELECT nameProduct FROM products WHERE id = " + i;
                    MySqlCommand cmd3 = new MySqlCommand(query3, conn);
                    product.name = cmd3.ExecuteScalar().ToString();
                    string query4 = "SELECT pictureURL FROM products WHERE id = " + i;
                    MySqlCommand cmd4 = new MySqlCommand(query4, conn);
                    product.pictureURL = cmd4.ExecuteScalar().ToString();
                    string query5 = "SELECT price  FROM products WHERE id = " + i;
                    MySqlCommand cmd5 = new MySqlCommand(query5, conn);
                    product.price = double.Parse(cmd5.ExecuteScalar().ToString());
                    products.Add(product);
                }
                else
                {
                    product.status = "wrong id";
                    products.Add(product);
                    return products;
                }
            }
            return products;
        }
    }
    public class rndProduct : Controller
    {
        public class Product
        {
            public string pictureURL { get; set; }
            public string name { get; set; }
            public double price { get; set; }
            public string description { get; set; }
            public string location { get; set; }
        }
        [Route("[controller]")]
        [HttpGet]
        public object GetRandomProduct()
        {
            string connection = "SERVER = localhost; database = bitakdb; uid = root; password= 1234;";
            MySqlConnection conn = new MySqlConnection(connection);
            conn.Open();
            string query2 = "SELECT COUNT(*) FROM products";
            MySqlCommand cmd2 = new MySqlCommand(query2, conn);
            int allProducts = int.Parse(cmd2.ExecuteScalar().ToString());
            Random rnd = new Random();
            int rndid = rnd.Next(0, allProducts);
            Product product = new Product();
            string query3 = "SELECT nameProduct FROM products where id = " + rndid;
            MySqlCommand cmd3 = new MySqlCommand(query3, conn);
            product.name = cmd3.ExecuteScalar().ToString();
            string query4 = "SELECT pictureURL FROM products where id = " + rndid;
            MySqlCommand cmd4 = new MySqlCommand(query4, conn);
            product.pictureURL = cmd4.ExecuteScalar().ToString();
            string query5 = "SELECT price FROM products where id = " + rndid;
            MySqlCommand cmd5 = new MySqlCommand(query5, conn);
            product.price = double.Parse(cmd5.ExecuteScalar().ToString());
            string query6 = "SELECT description FROM products where id = " + rndid;
            MySqlCommand cmd6 = new MySqlCommand(query6, conn);
            product.description = cmd6.ExecuteScalar().ToString();
            string query8 = "SELECT idCreater FROM products where id = " + rndid;
            MySqlCommand cmd8 = new MySqlCommand(query8, conn);
            int id = int.Parse(cmd8.ExecuteScalar().ToString());
            string query7 = "SELECT location FROM users WHERE id = '" + id + "';";
            MySqlCommand cmd7 = new MySqlCommand(query7, conn);
            product.location = cmd7.ExecuteScalar().ToString();
            return product;
        }
    }
    public class Search : Controller
    {
        public class Product
        {
            public string pictureURL { get; set; }
            public string name { get; set; }
            public double price { get; set; }
            public string description { get; set; }
            public string location { get; set; }
        }
        [Route("[controller]")]
        [HttpGet]
        public List<Product> Surch(string productForSurch)
        {
            Product ProductInfo(int idProduct)
            {
                string connection = "SERVER = localhost; database = bitakdb; uid = root; password= 1234;";
                MySqlConnection conn = new MySqlConnection(connection);
                conn.Open();
                Product product = new Product();
                string query3 = "SELECT nameProduct FROM products where id = " + idProduct;
                MySqlCommand cmd3 = new MySqlCommand(query3, conn);
                product.name = cmd3.ExecuteScalar().ToString();
                string query4 = "SELECT pictureURL FROM products where id = " + idProduct;
                MySqlCommand cmd4 = new MySqlCommand(query4, conn);
                product.pictureURL = cmd4.ExecuteScalar().ToString();
                string query5 = "SELECT price FROM products where id = " + idProduct;
                MySqlCommand cmd5 = new MySqlCommand(query5, conn);
                product.price = double.Parse(cmd5.ExecuteScalar().ToString());
                string query6 = "SELECT description FROM products where id = " + idProduct;
                MySqlCommand cmd6 = new MySqlCommand(query6, conn);
                product.description = cmd6.ExecuteScalar().ToString();
                string query8 = "SELECT idCreater FROM products where id = " + idProduct;
                MySqlCommand cmd8 = new MySqlCommand(query8, conn);
                int id = int.Parse(cmd8.ExecuteScalar().ToString());
                string query7 = "SELECT location FROM users WHERE id = '" + idProduct + "';";
                MySqlCommand cmd7 = new MySqlCommand(query7, conn);
                product.location = cmd7.ExecuteScalar().ToString();
                return product;
            }
            string connection = "SERVER = localhost; database = bitakdb; uid = root; password= 1234;";
            MySqlConnection conn = new MySqlConnection(connection);
            conn.Open();
            string query2 = "SELECT id FROM products WHERE nameProduct LIKE '%" + productForSurch + "%';";
            MySqlCommand cmd2 = new MySqlCommand(query2, conn);
            List<int> list = new List<int>();
            List<Product> productList = new List<Product>();
            using (MySqlDataReader reader = cmd2.ExecuteReader())
            {
                while (reader.Read())
                {
                    int id = reader.GetInt32(0); // Assuming the ID is in the first column (index 0)
                    list.Add(id);
                }
            }
            for (int i = 0; i < list.Count(); i++)
            {
                productList.Add(ProductInfo(list[i]));
            }
            return productList;
        }
    }
}