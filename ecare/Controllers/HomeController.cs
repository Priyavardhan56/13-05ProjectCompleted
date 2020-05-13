using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ecare.Models;
using System.Data.SqlClient;
using System.Data;
using System.Web.Configuration;

namespace ecare.Controllers
{
    public class HomeController : Controller
    {


        private readonly string cs = WebConfigurationManager.ConnectionStrings["dbECare"].ConnectionString;

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult About()
        {
            return View();
        }
        public ActionResult Contact(Contact objc)
        {
            if (ModelState.IsValid)
            {
                SqlConnection con = new SqlConnection(cs);
                try
                {
                    SqlCommand cmd = new SqlCommand("pInsertContactMaster", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Name", objc.Name);
                    cmd.Parameters.AddWithValue("@Email", objc.Email);
                    cmd.Parameters.AddWithValue("@Phone", objc.Phone);
                    cmd.Parameters.AddWithValue("@Message", objc.Message);
                    cmd.Parameters.AddWithValue("@EntryDateTime", DateTime.Now);

                    con.Open();
                    int i = cmd.ExecuteNonQuery();
                    con.Close();
                    if (i >= 1)
                        ViewBag.SuccessMessage = "success";
                    else
                        ViewBag.ErrorMessage = "Failed";
                         ModelState.Clear();
                    //return RedirectToAction("Contact", "Home");
                }
                catch
                {
                    ViewBag.ErrorMessage = "Failed";
                }

                ModelState.Clear();
            }
            return View();
        }
        public ActionResult Service()
        {
            return View();
        }
        public ActionResult Blog()
        {
            return View();
        }
        public ActionResult Gallary()
        {
            return View();
        }
        public ActionResult Team()
        {
            return View();
        }





        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(Credentials obj)
        {
            if (ModelState.IsValid)
            {

              
                SqlConnection con = new SqlConnection(cs);
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("spValidateHospitalCred", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    SqlCommand cmd1 = new SqlCommand("spAdminValidateCred", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    SqlCommand cmd2 = new SqlCommand("spPatientValidateCred", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    SqlCommand cmd3 = new SqlCommand("spValidateStaffCred", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    SqlCommand cmd4 = new SqlCommand("spValidateDoctorCred", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.AddWithValue("@Email", obj.Email);
                    cmd.Parameters.AddWithValue("@Password", obj.Password);
                    cmd1.Parameters.AddWithValue("@Email", obj.Email);
                    cmd1.Parameters.AddWithValue("@Password", obj.Password);
                    cmd2.Parameters.AddWithValue("@Email", obj.Email);
                    cmd2.Parameters.AddWithValue("@Password", obj.Password);
                    cmd3.Parameters.AddWithValue("@Email", obj.Email);
                    cmd3.Parameters.AddWithValue("@Password", obj.Password);
                    cmd4.Parameters.AddWithValue("@Email", obj.Email);
                    cmd4.Parameters.AddWithValue("@Password", obj.Password);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    da.Fill(ds);
                    SqlDataAdapter da1 = new SqlDataAdapter(cmd1);
                    DataSet ds1 = new DataSet();
                    da1.Fill(ds1);
                    SqlDataAdapter da2 = new SqlDataAdapter(cmd2);
                    DataSet ds2 = new DataSet();
                    da2.Fill(ds2);
                    SqlDataAdapter da3 = new SqlDataAdapter(cmd3);
                    DataSet ds3 = new DataSet();
                    da3.Fill(ds3);
                    SqlDataAdapter da4 = new SqlDataAdapter(cmd4);
                    DataSet ds4 = new DataSet();
                    da4.Fill(ds4);
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        Session["Email"] = ds.Tables[0].Rows[0]["HospitalEmail"].ToString();
                        Session["Name"] = ds.Tables[0].Rows[0]["HospitalName"].ToString();
                        Session["Id"] = Convert.ToInt32(ds.Tables[0].Rows[0]["HospitalId"]);
                        return RedirectToAction("Dashboard", "HospitalMaster");
                    }
                    else if (ds1.Tables[0].Rows.Count > 0)
                    {

                        Session["Email"] = ds1.Tables[0].Rows[0]["Email"].ToString();
                        Session["Name"] = ds1.Tables[0].Rows[0]["Name"].ToString();
                        Session["Id"] = Convert.ToInt32(ds1.Tables[0].Rows[0]["Id"]);
                        return RedirectToAction("Index", "Admin");

                    }
                    else if (ds2.Tables[0].Rows.Count > 0)
                    {

                        Session["Email"] = ds2.Tables[0].Rows[0]["Email"].ToString();
                        Session["Name"] = ds2.Tables[0].Rows[0]["Name"].ToString();
                        Session["Id"] = Convert.ToInt32(ds2.Tables[0].Rows[0]["Id"]);
                        return RedirectToAction("Dashboard", "PatientMaster");

                    }
                    else if (ds3.Tables[0].Rows.Count > 0)
                    {

                        Session["Email"] = ds3.Tables[0].Rows[0]["StaffEmail"].ToString();
                        Session["Name"] = ds3.Tables[0].Rows[0]["StaffName"].ToString();
                        Session["Id"] = Convert.ToInt32(ds3.Tables[0].Rows[0]["StaffId"]);
                        Session["HospitalId"] = ds3.Tables[0].Rows[0]["HospitalId"];
                        return RedirectToAction("ThreeTableList", "ReceptionMaster");

                    }
                    else if (ds4.Tables[0].Rows.Count > 0)
                    {

                        Session["Email"] = ds4.Tables[0].Rows[0]["DoctorEmail"].ToString();
                        Session["Name"] = ds4.Tables[0].Rows[0]["DoctorName"].ToString();
                        Session["Id"] = Convert.ToInt32(ds4.Tables[0].Rows[0]["DoctorId"]);
                        Session["HospitalId"] = ds4.Tables[0].Rows[0]["HospitalId"];
                        return RedirectToAction("Dashboard", "DoctorMaster");

                    }
                    int i = cmd.ExecuteNonQuery();

                    if (i >= 1)
                    {
                        ViewBag.SuccessMessage = "success";
                    }
                    else
                    {
                        TempData["Message"] = "'Account' Doesn't Exist or Your Account Is Not Active, Please Create New Account";
                        ViewBag.ErrorMessage = "'Account' Doesn't Exist or Your Account Is Not Active, Please Create New Account";
                    }

                    ModelState.Clear();
                    return RedirectToAction("Register", "Home");
                }

                catch (SqlException ex)
                {


                }
                finally
                {
                  
                    con.Close();
                }
            }

            return View();

        }





        public ActionResult Register()
        {
            return View();

        }

        [HttpPost]
        public ActionResult Register(User objUser)
        {
            eCareSendEmail objsendEmail = new eCareSendEmail();
            bool isSent = false;
            String fileName = Server.MapPath("~/App_Data/StaffCredEmail.txt");
            if (ModelState.IsValid)
            {
                SqlConnection con = new SqlConnection(cs);
                try
                {

                    con.Open();
                    SqlCommand cmd = new SqlCommand("pInsertPatientMasterCred", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.AddWithValue("@Name", objUser.Name);
                    cmd.Parameters.AddWithValue("@Email", objUser.Email);
                    cmd.Parameters.AddWithValue("@Password", objUser.Password);
                    cmd.Parameters.AddWithValue("@EntryDateTime", DateTime.Now);
                    cmd.Parameters.AddWithValue("@EntryBy", objUser.EntryBy);
                                        int i = cmd.ExecuteNonQuery();
                    if (i >= 1)
                    {
                        TempData["Message"] = "'Account'Created Successfully, Please Login";
                        ViewBag.Message = objUser.Name + " is Registered Successfully";
                        isSent = objsendEmail.SendEmail(objUser.Email, objUser.Name, objUser.Password, fileName);
                    }
                    else
                    {
                        TempData["Message"] = "'Account' Already Exist, Please Login";
                        ViewBag.ErrorMessage = "'Account' Already Exist, Please Login";
                        return RedirectToAction("Login", "Home");
                    }

                    ModelState.Clear();
                    return RedirectToAction("Login", "Home");

                }

                catch (SqlException ex)
                {
                    TempData["Message"] = "'Account' Already Exist, Please Try Different Email";
                    ViewBag.ErrorMessage = "'Account' Already Exist, Please Try Different Email";

                }
                finally
                {
                    con.Close();
                }
            }

            return View();

        }


        public ActionResult ForgottPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgottPassword(ForgotPassword objForgotPassword)
        {
            if (ModelState.IsValid)
            {
                SqlConnection con = new SqlConnection(cs);
                try
                {

                    con.Open();
                    SqlCommand cmd = new SqlCommand("Select Password from AdminCred where Email=@Email", con);
                    cmd.Parameters.AddWithValue("@email", objForgotPassword.Email);
                    DataTable dt = new DataTable();
                    SqlDataAdapter adp = new SqlDataAdapter(cmd);
                    adp.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        String password = dt.Rows[0]["Password"].ToString();
                        ViewBag.SuccessMessage = password;
                        ModelState.Clear();
                          //Code to send the email

                    }
                    else
                    {
                        ViewBag.ErrorMessage = "Null";
                        ModelState.Clear();
                    }


                  
                }
                catch (Exception e) { }
                finally { con.Close(); }
            }
       
            return View();
        }


        public ActionResult Logout()
        {

            Session.RemoveAll();
            Session.Clear();
            return View("~/Views/Home/Index.cshtml");
        }














        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword(ForgotPassword objForgotPassword)
        {

            eCareSendEmail objsendEmail = new eCareSendEmail();
            bool isSent = false;
            String fileName = Server.MapPath("~/App_Data/ForgotPassword.txt");
            if (ModelState.IsValid)
            {

              
                SqlConnection con = new SqlConnection(cs);
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("spForgotPasswordValidateHospitalCred", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    SqlCommand cmd1 = new SqlCommand("spAdminForgotPasswordValidateCred", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    SqlCommand cmd2 = new SqlCommand("spPatientForgotPasswordValidateCred", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    SqlCommand cmd3 = new SqlCommand("spValidateForgotPasswordStaffCred", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    SqlCommand cmd4 = new SqlCommand("spForgotPasswordValidateDoctorCred", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.AddWithValue("@Email", objForgotPassword.Email);
                    cmd1.Parameters.AddWithValue("@Email", objForgotPassword.Email);
                    cmd2.Parameters.AddWithValue("@Email", objForgotPassword.Email);
                    cmd3.Parameters.AddWithValue("@Email", objForgotPassword.Email);
                    cmd4.Parameters.AddWithValue("@Email", objForgotPassword.Email);
                    DataTable dt = new DataTable();
                    SqlDataAdapter adp = new SqlDataAdapter(cmd);
                    adp.Fill(dt);
                    DataTable dt1 = new DataTable();
                    SqlDataAdapter adp1 = new SqlDataAdapter(cmd1);
                    adp1.Fill(dt1);
                    DataTable dt2 = new DataTable();
                    SqlDataAdapter adp2 = new SqlDataAdapter(cmd2);
                    adp2.Fill(dt2);
                    DataTable dt3 = new DataTable();
                    SqlDataAdapter adp3 = new SqlDataAdapter(cmd3);
                    adp3.Fill(dt3);
                    DataTable dt4 = new DataTable();
                    SqlDataAdapter adp4 = new SqlDataAdapter(cmd4);
                    adp4.Fill(dt4);
                    if (dt.Rows.Count > 0)
                    {
                        String password = dt.Rows[0]["HospitalPassword"].ToString();
                        String Name = dt.Rows[0]["HospitalName"].ToString();
                        ViewBag.SuccessMessage = password;
                        ModelState.Clear();                    
                        isSent = objsendEmail.SendEmail(objForgotPassword.Email,Name, password, fileName);


                    }
                    else if (dt.Rows.Count > 0)
                    {

                        String password = dt1.Rows[0]["Password"].ToString();
                        String Name = dt1.Rows[0]["Name"].ToString();
                        ViewBag.SuccessMessage = password;
                        ModelState.Clear();
                        isSent = objsendEmail.SendEmail(objForgotPassword.Email, Name, password, fileName);

                    }
                    else if (dt2.Rows.Count > 0)
                    {

                        String password = dt2.Rows[0]["Password"].ToString();
                        String Name = dt2.Rows[0]["Name"].ToString();
                        ViewBag.SuccessMessage = password;
                        ModelState.Clear();
                        isSent = objsendEmail.SendEmail(objForgotPassword.Email, Name, password, fileName);

                    }
                    else if (dt3.Rows.Count > 0)
                    {

                        String password = dt3.Rows[0]["Password"].ToString();
                        String Name = dt3.Rows[0]["StaffName"].ToString();
                        ViewBag.SuccessMessage = password;
                        ModelState.Clear();
                        isSent = objsendEmail.SendEmail(objForgotPassword.Email, Name, password, fileName);

                    }
                    else if (dt4.Rows.Count > 0)
                    {

                       String password = dt4.Rows[0]["Password"].ToString();
                        String Name = dt4.Rows[0]["DoctorName"].ToString();
                        ViewBag.SuccessMessage = password;
                        ModelState.Clear();
                        isSent = objsendEmail.SendEmail(objForgotPassword.Email, Name, password, fileName);

                    }
               
                    else
                    {
                        TempData["Message"] = "'Account' Doesn't exist plese create new one";
                        ViewBag.ErrorMessage = "Null";
                    ModelState.Clear();
                        return RedirectToAction("Register", "Home");
                    }



            }
                catch (SqlException ex) { }
            finally { con.Close(); }
        }
       
            return View();
    }
}
}
