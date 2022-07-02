using System;
using System.Collections.Generic;
using System.Diagnostics;
using MySql.Data.MySqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CSharpAuthenticationAndAuthorization.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;

namespace CSharpAuthenticationAndAuthorization.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        public HomeController(IConfiguration config)
        {
            this.config = config;
        }
        private MySqlConnection GetConnection()
        {
            string connectionstr = config.GetConnectionString("DefaultConnectionString");
            return new MySqlConnection(connectionstr);
        }
       
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration config;
        public void GetAllusers()
        {
            MySqlCommand cmd = new MySqlCommand();
            MySqlParameter[] pm = new MySqlParameter[3];
            cmd.CommandText = "sp_getall_leadSource";
            //argStudentName varchar(100),argPhonenumber varchar(15),argCourseId
          

           
            

            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                cmd.Connection = conn;
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                     string name=   reader["SourceName"].ToString();
                    }
                }
            }
           
        }
       

        public IActionResult Index()
        {
            GetAllusers();
            return View();
        }
        [Authorize]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
