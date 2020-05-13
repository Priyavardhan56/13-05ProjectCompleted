using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using ecare.Models;

namespace ecare.Controllers
{
    public class AdminController : Controller
    {
        private static readonly string cs = WebConfigurationManager.ConnectionStrings["dbECare"].ConnectionString;
        private static String email = "";
        public ActionResult Index()
        {

             email = Session["Email"].ToString();
            string cs = WebConfigurationManager.ConnectionStrings["dbECare"].ConnectionString;
            SqlConnection con = new SqlConnection(cs);
            con.Open();
            SqlCommand hosptialCountCmd = new SqlCommand("spHospitalsCount", con);
            hosptialCountCmd.CommandType = CommandType.StoredProcedure;
            //hosptialCountCmd.Parameters.AddWithValue("@Email", Session["Email"]);
            int HospitalCount = (int)hosptialCountCmd.ExecuteScalar();



            SqlCommand userCountCmd = new SqlCommand(" spUsersCount", con);
            int userCount = (int)userCountCmd.ExecuteScalar();

            SqlCommand appointmentCountCmd = new SqlCommand("spGetCountAppointments", con);         
            int appointmentCount = (int)appointmentCountCmd.ExecuteScalar();


            ViewBag.hospitalCount = HospitalCount;
            ViewBag.userCount = userCount;
            ViewBag.appointmentCount = appointmentCount;



            return View();
        }

        public ActionResult Settings()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Settings(Credentials obj)

        {
            if (ModelState.IsValid)
            {
                SqlConnection con = new SqlConnection(cs);
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("spUpdateAdminCred", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.AddWithValue("@Id", obj.Id);
                    cmd.Parameters.AddWithValue("@Password", obj.Password);


                    int i = cmd.ExecuteNonQuery();
                    if (i >= 1)
                    {
                        ViewBag.SuccessMessage = "success";
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "Failed";

                    }
                }
                catch (SqlException ex)
                { }
                finally
                {
                    con.Close();
                }
                ModelState.Clear();
            }

            return RedirectToAction("Settings", "Admin");
        }

        public ActionResult Logout() {

            Session.RemoveAll();
            Session.Clear();
            return View("~/Views/Home/Index.cshtml");
        }



[HttpGet]
    public ActionResult Contactinfo()
    {
            SqlConnection con = new SqlConnection(cs);

            List<Contact> ContactList = new List<Contact>();

            SqlCommand cmd = new SqlCommand("pDetailsContactMaster", con);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter sd = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();

            con.Open();
            sd.Fill(dt);
            con.Close();

            foreach (DataRow dr in dt.Rows)
            {
                ContactList.Add(
                    new Contact
                    {
                        Id = Convert.ToInt32(dr["Id"]),
                        Name = Convert.ToString(dr["Name"]),
                        Phone = Convert.ToString(dr["Phone"]),
                        Email = Convert.ToString(dr["Email"]),
                        Message = Convert.ToString(dr["Message"]),
                        EntryDateTime = Convert.ToString(dr["EntryDateTime"])
                    });
            }
            return View(ContactList);
        }



        public ActionResult Delete(int id)
        {
            try
            {

                if (DeleteRole(id))
                {
                    ViewBag.SuccessMessage = "Success";

                }
                else
                {
                    ViewBag.ErrorMessage = "error";

                }
                return RedirectToAction("Contactinfo");
            }
            catch
            {
                return View();
            }
        }
        public bool DeleteRole(int id)
        {
            SqlConnection con = new SqlConnection(cs);
            SqlCommand cmd = new SqlCommand("pDeleteContactMaster", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Id", id);

            con.Open();
            int i = cmd.ExecuteNonQuery();
            con.Close();

            if (i >= 1)
                return true;
            else
                return false;
        }
        public ActionResult Users(string search)
        {
           
            List<Patient> PatientList = new List<Patient>();
            SqlConnection con = new SqlConnection(cs);
            //SqlCommand cmd = new SqlCommand("spDoctorListDetails", con)
            //{
            //    CommandType = CommandType.StoredProcedure
            //};
            if (con.State == System.Data.ConnectionState.Closed)
                con.Open();

            SqlCommand cmd = new SqlCommand("select * from PatientMaster where  Name like '%" + search + "%'", con);
            cmd.Connection = con;
           
            SqlDataAdapter sd = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();

            //con.Open();
            sd.Fill(dt);
            con.Close();

            foreach (DataRow dr in dt.Rows)
            {
                PatientList.Add(
                    new Patient
                    {
                        Id = Convert.ToInt32(dr["Id"]),
                        Name = Convert.ToString(dr["Name"]),
                        Address = Convert.ToString(dr["Address"]),

                        City = Convert.ToString(dr["City"]),
                        State = Convert.ToString(dr["State"]),
                        Country = Convert.ToString(dr["Country"]),
                        Email = Convert.ToString(dr["Email"]),
                        Password = Convert.ToString(dr["Password"]),
                         Phone = Convert.ToString(dr["Phone"]),
                        EntryDateTime = Convert.ToString(dr["EntryDateTime"]),                       
                        EntryBy = Convert.ToString(dr["EntryBy"])
                       
                    });
            }
            return View(PatientList);
        }


        public ActionResult AddUsers()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddUsers(Patient objuser)
        {

            eCareSendEmail objsendEmail = new eCareSendEmail();
            bool isSent = false;
            String fileName = Server.MapPath("~/App_Data/StaffCredEmail.txt");
            //if (ModelState.IsValid)
            //{           
            SqlConnection con = new SqlConnection(cs);
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("pInsertuserMaster", con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@Name", objuser.Name);

                cmd.Parameters.AddWithValue("@Email", objuser.Email);
                string pass = generatePassword.CreateRandomPassword(10);
                objuser.Password = pass;
                cmd.Parameters.AddWithValue("@Password", objuser.Password);
                cmd.Parameters.AddWithValue("@EntryDateTime", DateTime.Now);
                cmd.Parameters.AddWithValue("@EntryBy", objuser.EntryBy);
                int i = cmd.ExecuteNonQuery();
                if (i >= 1)
                {
                    ViewBag.SuccessMessage = "success";
                    isSent = objsendEmail.SendEmail(objuser.Email, objuser.Name, objuser.Password, fileName);
                }
                else
                {
                    ViewBag.ErrorMessage = "Failed";

                    ModelState.Clear();
                    return RedirectToAction("users", "Admin");
                }
            }
            catch (SqlException ex)
            {
                TempData["Message"] = "Username already exists. Enter a different one.";
                ViewBag.ErrorMessage = "Email Already Exist,Please Try Different Email";
            }
            finally
            {
                con.Close();
            }
            ModelState.Clear();
            //}
            return RedirectToAction("users", "Admin");
        }

    }
}