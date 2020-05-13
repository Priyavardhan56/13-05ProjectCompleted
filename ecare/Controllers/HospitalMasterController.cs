using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using ecare.Models;

namespace ecare.Controllers
{
    public class HospitalMasterController : Controller
    {

        // GET: HospitalMaster
        private static readonly string cs = WebConfigurationManager.ConnectionStrings["dbECare"].ConnectionString;
        private static String email = "";
        private static String Id = "";
        private static String HospitalSearch = "";
      
        public ActionResult Create()
        {
            return View();
        }
        public ActionResult HospitalProfile()
        {

            return View(GetHospitals().Find(model => model.HospitalId == (int)Session["Id"]));
        }
        [HttpPost]
        public ActionResult HospitalProfile( Hospital objHospital)

        {
            SqlConnection con = new SqlConnection(cs);
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("spUpdateHospitalProfile", con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@HospitalId", (int)Session["Id"]);
                cmd.Parameters.AddWithValue("@HospitalName", objHospital.HospitalName);
                cmd.Parameters.AddWithValue("@HospitalAddress", objHospital.HospitalAddress);
                cmd.Parameters.AddWithValue("@HospitalCity", objHospital.HospitalCity);
                cmd.Parameters.AddWithValue("@HospitalState", objHospital.HospitalState);
                cmd.Parameters.AddWithValue("@HospitalCountry", objHospital.HospitalCountry);
                cmd.Parameters.AddWithValue("@HospitalPhone", objHospital.HospitalPhone);
                cmd.Parameters.AddWithValue("@HospitalEmail", objHospital.HospitalEmail);
                
                int i = cmd.ExecuteNonQuery();
                if (i >= 1)
                    ViewBag.SuccessMessage = "success";
                else
                    ViewBag.ErrorMessage = "Failed";
            }
            catch (SqlException ex)
            { }
            finally
            {
                con.Close();
            }
            ModelState.Clear();


            return RedirectToAction("Dashboard","HospitalMaster");
        }


        [HttpPost]
        public ActionResult Create(Hospital objHospital,FormCollection f)
        {

            eCareSendEmail objsendEmail = new eCareSendEmail();
            bool isSent = false;
            String fileName = Server.MapPath("~/App_Data/hospitalCredentialEmail.txt");
            if (ModelState.IsValid)
            {
                HttpFileCollectionBase file = Request.Files;
                string HospitalLogoo = Path.GetFileName(file[0].FileName);
                if (Request.Files["Filebutton"] != null && Request.Files["Filebutton"].ContentLength > 0)

                {
                    string path = Path.Combine(Server.MapPath(ConfigurationManager.AppSettings["HospitalLogoPath"].ToString()),HospitalLogoo);
                  HospitalLogoo = Path.GetFileName(Request.Files["Filebutton"].FileName) ;
                    Request.Files["Filebutton"].SaveAs(path);
                                   }
                SqlConnection con = new SqlConnection(cs);
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("pInsertHospitalMaster", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.AddWithValue("@HospitalName", objHospital.HospitalName);
                    cmd.Parameters.AddWithValue("@HospitalAddress", objHospital.HospitalAddress);
                    cmd.Parameters.AddWithValue("@HospitalCity", objHospital.HospitalCity);
                    cmd.Parameters.AddWithValue("@HospitalState", objHospital.HospitalState);
                    cmd.Parameters.AddWithValue("@HospitalCountry", objHospital.HospitalCountry);
                    cmd.Parameters.AddWithValue("@HospitalPhone", objHospital.HospitalPhone);
                    cmd.Parameters.AddWithValue("@HospitalEmail", objHospital.HospitalEmail);
                    cmd.Parameters.AddWithValue("@HospitalLogo", HospitalLogoo);

                    //if (objHospital.HospitalLogo != null && objHospital.HospitalLogo.ContentLength > 0)
                    //{

                    //    string FileName = Path.GetFileName(objHospital.HospitalLogo.FileName);
                    //    FileName = DateTime.Now.ToString("yyyyMMdd") + "-" + FileName;
                    //    string UploadPath = Path.Combine(Server.MapPath(ConfigurationManager.AppSettings["HospitalLogoPath"].ToString()), FileName);
                    //    objHospital.HospitalLogo.SaveAs(UploadPath);
                    //    cmd.Parameters.AddWithValue("@HospitalLogo", (DateTime.Now.ToString("yyyyMMdd") + "-" + objHospital.HospitalLogo.FileName));
                    //}
                    cmd.Parameters.AddWithValue("@IsActive", objHospital.IsActive);
                    cmd.Parameters.AddWithValue("@EntryDateTime", DateTime.Now);
                    cmd.Parameters.AddWithValue("@EntryBy", objHospital.EntryBy);
                    //password generate
                    string pass = generatePassword.CreateRandomPassword(10);
                
                    objHospital.Password = pass;
                    cmd.Parameters.AddWithValue("@HospitalPassword", objHospital.Password);
                    int i = cmd.ExecuteNonQuery();

                    if (i >= 1)
                    {
                        ViewBag.SuccessMessage = "success";
                      isSent=  objsendEmail.SendEmail(objHospital.HospitalEmail, objHospital.HospitalName, objHospital.Password, fileName);
                    }
                    else
                    {
                       
                        ViewBag.ErrorMessage = "Email Already Exist,Please Try Different Email";
                    }

                    ModelState.Clear();
                   return RedirectToAction("Index", "HospitalMaster");
                }
          
                catch (SqlException ex)
                {
                    TempData["Message"] = "'EMAIL' already exists. Enter a different one.";
                    ViewBag.ErrorMessage = "Email Already Exist,Please Try Different Email";
                }
                finally
                {
                    con.Close();
                }
                ModelState.Clear();
            }
            return View();
        }


    
     
        public ActionResult Delete(int id)
        {

            try
            {

                if (DeleteHospital(id))
                {
                    ViewBag.SuccessMessage = "Success";
                 
                }
                else
                {
                    ViewBag.ErrorMessage = "error";
                   
                }
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
           
        }

        public ActionResult Edit(int id)
        {
            return View(GetHospitals().Find(model=>model.HospitalId==id));
        }
        [HttpPost]
        public ActionResult Edit(int id,Hospital objHospital)

        {
           
            SqlConnection con = new SqlConnection(cs);
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("pUPdateHospitalMaster", con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@HospitalId", objHospital.HospitalId);
                cmd.Parameters.AddWithValue("@HospitalName", objHospital.HospitalName);
                cmd.Parameters.AddWithValue("@HospitalAddress", objHospital.HospitalAddress);
                cmd.Parameters.AddWithValue("@HospitalCity", objHospital.HospitalCity);
                cmd.Parameters.AddWithValue("@HospitalState", objHospital.HospitalState);
                cmd.Parameters.AddWithValue("@HospitalCountry", objHospital.HospitalCountry);
                cmd.Parameters.AddWithValue("@HospitalPhone", objHospital.HospitalPhone);
                cmd.Parameters.AddWithValue("@HospitalEmail", objHospital.HospitalEmail);
                //if (objHospital.HospitalLogo != null && objHospital.HospitalLogo.ContentLength > 0)
                //{

                //    string FileName = Path.GetFileName(objHospital.HospitalLogo.FileName);
                //    FileName = DateTime.Now.ToString("yyyyMMdd") + "-" + FileName;
                //    string UploadPath = Path.Combine(Server.MapPath(ConfigurationManager.AppSettings["HospitalLogoPath"].ToString()), FileName);
                //    objHospital.HospitalLogo.SaveAs(UploadPath);
                //    cmd.Parameters.AddWithValue("@HospitalLogo", (DateTime.Now.ToString("yyyyMMdd") + "-" + objHospital.HospitalLogo.FileName));
                //}

                cmd.Parameters.AddWithValue("@HospitalLogo", objHospital.HospitalLogo);

                cmd.Parameters.AddWithValue("@IsActive", objHospital.IsActive);
                cmd.Parameters.AddWithValue("@EntryDateTime", DateTime.Now);
              

                int i = cmd.ExecuteNonQuery();
                if (i >= 1)
                    ViewBag.SuccessMessage = "success";
                else
                    ViewBag.ErrorMessage = "Failed";
            }
            catch (SqlException ex)
            { }
            finally
            {
                con.Close();
            }
            ModelState.Clear();


            return RedirectToAction("Index");
        }



        public ActionResult Index(string search)
        {
            List<Hospital> HospitalList = new List<Hospital>();
            SqlConnection con = new SqlConnection(cs);
            //SqlCommand cmd = new SqlCommand("spGetDetailsHospitalsMaster", con)
            //{
            //    CommandType = CommandType.StoredProcedure
            //};
            //SqlCommand cmd1 = new SqlCommand("Select * from HospitalMaster where HospitalName like '%" + HospitalSearch + "%'", con);
            if (con.State == System.Data.ConnectionState.Closed)
                con.Open();

            SqlCommand cmd = new SqlCommand("select * from HospitalMaster where HospitalName like '%" + search + "%'", con);
            cmd.Connection = con;
            SqlDataAdapter sd = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();

            //con.Open();
            sd.Fill(dt);
            con.Close();

            foreach (DataRow dr in dt.Rows)
            {
                HospitalList.Add(
                    new Hospital
                    {
                        HospitalId = Convert.ToInt32(dr["HospitalId"]),
                        HospitalName = Convert.ToString(dr["HospitalName"]),
                        HospitalAddress = Convert.ToString(dr["HospitalAddress"]),
                        HospitalCity = Convert.ToString(dr["HospitalCity"]),
                        HospitalLogo = Convert.ToString(dr["HospitalLogo"]),
                        HospitalState = Convert.ToString(dr["HospitalState"]),
                        HospitalCountry = Convert.ToString(dr["HospitalCountry"]),
                        HospitalPhone = Convert.ToString(dr["HospitalPhone"]),
                        HospitalEmail = Convert.ToString(dr["HospitalEmail"]),
                        IsActive = Convert.ToBoolean(dr["IsActive"]),
                        EntryDateTime = Convert.ToString(dr["EntryDateTime"]),
                        EntryBy = Convert.ToString(dr["EntryBy"])
                    });
            }
            return View(HospitalList);
        }


        [HttpGet]
        public static List<Hospital> GetHospitals()
        {
            List<Hospital> HospitalList = new List<Hospital>();
            SqlConnection con = new SqlConnection(cs);
            SqlCommand cmd = new SqlCommand("spGetDetailsHospitalsMaster", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            //SqlCommand cmd1 = new SqlCommand("Select * from HospitalMaster where HospitalName like '%" + HospitalSearch + "%'", con);

            SqlDataAdapter sd = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();

            con.Open();
            sd.Fill(dt);
            con.Close();

            foreach (DataRow dr in dt.Rows)
            {
                HospitalList.Add(
                    new Hospital
                    {
                        HospitalId = Convert.ToInt32(dr["HospitalId"]),
                        HospitalName = Convert.ToString(dr["HospitalName"]),
                        HospitalAddress = Convert.ToString(dr["HospitalAddress"]),
                        HospitalCity = Convert.ToString(dr["HospitalCity"]),
                        HospitalLogo = Convert.ToString(dr["HospitalLogo"]),
                        HospitalState = Convert.ToString(dr["HospitalState"]),
                        HospitalCountry = Convert.ToString(dr["HospitalCountry"]),
                        HospitalPhone = Convert.ToString(dr["HospitalPhone"]),
                        HospitalEmail = Convert.ToString(dr["HospitalEmail"]),
                        IsActive = Convert.ToBoolean(dr["IsActive"]),
                        EntryDateTime = Convert.ToString(dr["EntryDateTime"]),
                        EntryBy = Convert.ToString(dr["EntryBy"])
                    });
            }
            return HospitalList;
        }
        public static bool DeleteHospital(int id)
        {
            SqlConnection con = new SqlConnection(cs);

            SqlCommand cmd = new SqlCommand("pDeleteHospitalMaster", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@HospitalId", id);
            try
            {
                con.Open();
                int i = cmd.ExecuteNonQuery();
                con.Close();

                if (i >= 1)
                    return true;
                else
                    return false;
            }
            catch (Exception e)
            {
            }
            finally
            {
                con.Close();
            }
            return false;
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
                    SqlCommand cmd = new SqlCommand("spUpdateHospitalCred", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.AddWithValue("@HospitalId", obj.Id);
                    cmd.Parameters.AddWithValue("@HospitalPassword", obj.Password);


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
                { ViewBag.ErrorMessage = "Failed"; }
                finally
                {
                    con.Close();
                }
                ModelState.Clear();

            }
            return View();
        }




        public ActionResult Dashboard()
        {
            email = Session["Id"].ToString();
            string cs = WebConfigurationManager.ConnectionStrings["dbECare"].ConnectionString;
            SqlConnection con = new SqlConnection(cs);
            con.Open();
            SqlCommand DoctorCountCmd = new SqlCommand("spDoctorsCount", con);
            DoctorCountCmd.CommandType = CommandType.StoredProcedure;
            DoctorCountCmd.Parameters.AddWithValue("@HospitalId", Session["Id"]);
             int DoctorCount = (int)DoctorCountCmd.ExecuteScalar();

            SqlCommand StaffCountCmd = new SqlCommand("spStaffCount", con);
            StaffCountCmd.CommandType = CommandType.StoredProcedure;
            StaffCountCmd.Parameters.AddWithValue("@HospitalId", Session["Id"]);
            int StaffCount = (int)StaffCountCmd.ExecuteScalar();


            SqlCommand appointmentCountCmd = new SqlCommand("spGetCountAppointmentForHospital", con);
            appointmentCountCmd.CommandType = CommandType.StoredProcedure;
            appointmentCountCmd.Parameters.AddWithValue("@Email", Session["Email"]);        
           
            int appointmentCount = (int)appointmentCountCmd.ExecuteScalar();


            ViewBag.DoctorCount = DoctorCount;
            ViewBag.StaffCount = StaffCount;
            ViewBag.appointmentCount = appointmentCount;



            return View();
        }
























        public ActionResult DoctorsList(string search)
        {
            Id = Session["Id"].ToString();
            List<Doctor> DoctorList = new List<Doctor>();
            SqlConnection con = new SqlConnection(cs);
            //SqlCommand cmd = new SqlCommand("spDoctorListDetails", con)
            //{
            //    CommandType = CommandType.StoredProcedure
            //};
            if (con.State == System.Data.ConnectionState.Closed)
                con.Open();

            SqlCommand cmd = new SqlCommand("select * from DoctorMaster where HospitalId=@HospitalId  and DoctorName like '%" + search + "%'", con);
            cmd.Connection = con;
            cmd.Parameters.AddWithValue("@HospitalId", Id);
            SqlDataAdapter sd = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();

            //con.Open();
            sd.Fill(dt);
            con.Close();

            foreach (DataRow dr in dt.Rows)
            {
                DoctorList.Add(
                    new Doctor
                    {
                        DoctorId = Convert.ToInt32(dr["DoctorId"]),
                        EmployeeCode = Convert.ToString(dr["EmployeeCode"]),
                        DoctorName = Convert.ToString(dr["DoctorName"]),

                        HospitalId = Convert.ToInt32(dr["HospitalId"]),
                        DoctorSpecialization = Convert.ToString(dr["DoctorSpecialization"]),
                        DoctorDegree = Convert.ToString(dr["DoctorDegree"]),
                        DoctorPhone = Convert.ToString(dr["DoctorPhone"]),
                        DoctorEmail = Convert.ToString(dr["DoctorEmail"]),

                        DoctorCity = Convert.ToString(dr["DoctorCity"]),
                        DoctorState = Convert.ToString(dr["DoctorState"]),
                        DoctorCountry = Convert.ToString(dr["DoctorCountry"]),
                        IsActive = Convert.ToBoolean(dr["IsActive"]),
                        EntryBy = Convert.ToString(dr["EntryBy"]),
                        DoctorAddress = Convert.ToString(dr["DoctorAddress"]),

                    });
            }
            return View(DoctorList);
        }
        [HttpGet]
        public static List<Doctor> GetDoctors()
        {

            List<Doctor> DoctorList = new List<Doctor>();
            SqlConnection con = new SqlConnection(cs);
            SqlCommand cmd = new SqlCommand("spDoctorListDetails", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@HospitalId",Id);
            SqlDataAdapter sd = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();

            con.Open();
            sd.Fill(dt);
            con.Close();

            foreach (DataRow dr in dt.Rows)
            {
                DoctorList.Add(
                    new Doctor
                    {
                        DoctorId = Convert.ToInt32(dr["DoctorId"]),
                        EmployeeCode = Convert.ToString(dr["EmployeeCode"]),
                        DoctorName = Convert.ToString(dr["DoctorName"]),

                        HospitalId = Convert.ToInt32(dr["HospitalId"]),
                        DoctorSpecialization = Convert.ToString(dr["DoctorSpecialization"]),
                        DoctorDegree = Convert.ToString(dr["DoctorDegree"]),
                        DoctorPhone = Convert.ToString(dr["DoctorPhone"]),
                        DoctorEmail = Convert.ToString(dr["DoctorEmail"]),

                        DoctorCity = Convert.ToString(dr["DoctorCity"]),
                        DoctorState = Convert.ToString(dr["DoctorState"]),
                        DoctorCountry = Convert.ToString(dr["DoctorCountry"]),
                        IsActive = Convert.ToBoolean(dr["IsActive"]),
                        EntryBy = Convert.ToString(dr["EntryBy"]),
                        DoctorAddress = Convert.ToString(dr["DoctorAddress"]),

                    });
            }
            return DoctorList;
        }





        public ActionResult AddDoctor()
        {
            return View();
        }



        [HttpPost]
        public ActionResult AddDoctor(Doctor objDoctor)
        {
            eCareSendEmail objsendEmail = new eCareSendEmail();
            bool isSent = false;
            String fileName = Server.MapPath("~/App_Data/DoctorCredEmail.txt");

            //if (ModelState.IsValid)
            //{
            SqlConnection con = new SqlConnection(cs);
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("pInsertDoctorMaster", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };


                    cmd.Parameters.AddWithValue("@EmployeeCode", objDoctor.EmployeeCode);

                    if (objDoctor.DoctorPhoto != null && objDoctor.DoctorPhoto.ContentLength > 0)
                    {

                        string FileName = Path.GetFileName(objDoctor.DoctorPhoto.FileName);
                        FileName = DateTime.Now.ToString("yyyyMMdd") + "-" + FileName;
                        string UploadPath = Path.Combine(Server.MapPath(ConfigurationManager.AppSettings["HospitalLogoPath"].ToString()), FileName);
                        objDoctor.DoctorPhoto.SaveAs(UploadPath);
                        cmd.Parameters.AddWithValue("@DoctorPhoto", (DateTime.Now.ToString("yyyyMMdd") + "-" + objDoctor.DoctorPhoto.FileName));
                    }
                    cmd.Parameters.AddWithValue("@DoctorName", objDoctor.DoctorName);
                    cmd.Parameters.AddWithValue("@HospitalId", objDoctor.HospitalId);
                    cmd.Parameters.AddWithValue("@DoctorSpecialization", objDoctor.DoctorSpecialization);
                    cmd.Parameters.AddWithValue("@DoctorDegree", objDoctor.DoctorDegree);
                    cmd.Parameters.AddWithValue("@DoctorPhone", objDoctor.DoctorPhone);
                    cmd.Parameters.AddWithValue("@DoctorEmail", objDoctor.DoctorEmail);
                    cmd.Parameters.AddWithValue("@DoctorCity", objDoctor.DoctorCity);
                    cmd.Parameters.AddWithValue("@DoctorState", objDoctor.DoctorState);
                    cmd.Parameters.AddWithValue("@DoctorCountry", objDoctor.DoctorCountry);
                    cmd.Parameters.AddWithValue("@IsActive", objDoctor.IsActive);
                    cmd.Parameters.AddWithValue("@EntryDate", DateTime.Now);
                    cmd.Parameters.AddWithValue("@EntryBy", objDoctor.EntryBy);
                    cmd.Parameters.AddWithValue("@DoctorAddress", objDoctor.DoctorAddress);
                    //password generate


                    string pass = generatePassword.CreateRandomPassword(10);
                    objDoctor.Password = pass;

                    cmd.Parameters.AddWithValue("@Password", objDoctor.Password);
                    int i = cmd.ExecuteNonQuery();
                    if (i >= 1)
                    {
                        ViewBag.SuccessMessage = "success";
                    isSent = objsendEmail.SendEmail(objDoctor.DoctorEmail, objDoctor.DoctorName, objDoctor.Password, fileName);

                    return RedirectToAction("DoctorsList", "Hospitalmaster");
                    }
                    else
                        ViewBag.ErrorMessage = "Failed";
                    ModelState.Clear();


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
            return View();
        }

        public ActionResult StaffList(string search)
        {
            List<Staff> StaffList = new List<Staff>();
            SqlConnection con = new SqlConnection(cs);
            //SqlCommand cmd = new SqlCommand("spStaffListDetails", con)
            //{
            //    CommandType = CommandType.StoredProcedure
            //};

            if (con.State == System.Data.ConnectionState.Closed)
                con.Open();

            SqlCommand cmd = new SqlCommand("select * from StaffMaster where HospitalId=@HospitalId  and StaffName like '%" + search + "%'", con);
            cmd.Connection = con;
            cmd.Parameters.AddWithValue("@HospitalId", Id);
            SqlDataAdapter sd = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();

            //con.Open();
            sd.Fill(dt);
            con.Close();

            foreach (DataRow dr in dt.Rows)
            {
                StaffList.Add(
                    new Staff
                    {
                        StaffId = Convert.ToInt32(dr["StaffId"]),
                        StaffName = Convert.ToString(dr["StaffName"]),
                        HospitalId = Convert.ToInt32(dr["HospitalId"]),
                        EmployeeCode = Convert.ToInt32(dr["EmployeeCode"]),
                        StaffSpecialization = Convert.ToString(dr["StaffSpecialization"]),
                        StaffDegree = Convert.ToString(dr["StaffDegree"]),
                        Designation = Convert.ToString(dr["Designation"]),

                        StaffPhone = Convert.ToString(dr["StaffPhone"]),
                        StaffEmail = Convert.ToString(dr["StaffEmail"]),

                        StaffCity = Convert.ToString(dr["StaffCity"]),
                        StaffState = Convert.ToString(dr["StaffState"]),
                        StaffCountry = Convert.ToString(dr["StaffCountry"]),
                        IsActive = Convert.ToBoolean(dr["IsActive"]),
                        EntryDate = Convert.ToString(dr["EntryDate"]),
                        EntryBy = Convert.ToString(dr["EntryBy"]),
                        StaffAddress = Convert.ToString(dr["StaffAddress"]),

                    });
            }
            return View(StaffList);
        }
        [HttpGet]
        public static List<Staff> GetStaffs()
        {
            List<Staff> StaffList = new List<Staff>();
            SqlConnection con = new SqlConnection(cs);
            SqlCommand cmd = new SqlCommand("spStaffListDetails", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@HospitalId", Id);
            SqlDataAdapter sd = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();

            con.Open();
            sd.Fill(dt);
            con.Close();

            foreach (DataRow dr in dt.Rows)
            {
                StaffList.Add(
                    new Staff
                    {
                        StaffId = Convert.ToInt32(dr["StaffId"]),
                        StaffName = Convert.ToString(dr["StaffName"]),
                        HospitalId = Convert.ToInt32(dr["HospitalId"]),
                        EmployeeCode = Convert.ToInt32(dr["EmployeeCode"]),
                        StaffSpecialization = Convert.ToString(dr["StaffSpecialization"]),
                        StaffDegree = Convert.ToString(dr["StaffDegree"]),
                        Designation = Convert.ToString(dr["Designation"]),

                        StaffPhone = Convert.ToString(dr["StaffPhone"]),
                        StaffEmail = Convert.ToString(dr["StaffEmail"]),

                        StaffCity = Convert.ToString(dr["StaffCity"]),
                        StaffState = Convert.ToString(dr["StaffState"]),
                        StaffCountry = Convert.ToString(dr["StaffCountry"]),
                        IsActive = Convert.ToBoolean(dr["IsActive"]),
                        EntryDate = Convert.ToString(dr["EntryDate"]),
                        EntryBy = Convert.ToString(dr["EntryBy"]),
                        StaffAddress = Convert.ToString(dr["StaffAddress"]),

                    });
            }
            return StaffList;
        }



    

        public ActionResult AddStaff()
        {
            SqlConnection con = new SqlConnection(cs);
            con.Open(); if (con.State == System.Data.ConnectionState.Closed)
                con.Open();


            SqlCommand cmd = new SqlCommand("Select * From RoleMaster", con);
            cmd.Connection = con;
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            sda.Fill(ds);
            ViewBag.state = ds.Tables[0];
            List<SelectListItem> getstate = new List<SelectListItem>();
            foreach (System.Data.DataRow dr in ViewBag.state.Rows)
            {
                getstate.Add(new SelectListItem { Text = dr["RoleName"].ToString(), Value = dr["RoleId"].ToString() });
            }
            ViewBag.RoleId = getstate;

            return View();

        }



        [HttpPost]
        public ActionResult AddStaff(Staff objStaff)
        {

            eCareSendEmail objsendEmail = new eCareSendEmail();
            bool isSent = false;
            String fileName = Server.MapPath("~/App_Data/StaffCredEmail.txt");
            //if (ModelState.IsValid)
            //{
            string RoleId = Request.Form["RoleId"].ToString();
                SqlConnection con = new SqlConnection(cs);
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("pInsertStaffMaster", con)
                {
                    CommandType = CommandType.StoredProcedure
                };


                cmd.Parameters.AddWithValue("@StaffName", objStaff.StaffName);
                if (objStaff.StaffPhoto != null && objStaff.StaffPhoto.ContentLength > 0)
                {

                    string FileName = Path.GetFileName(objStaff.StaffPhoto.FileName);
                    FileName = DateTime.Now.ToString("yyyyMMdd") + "-" + FileName;
                    string UploadPath = Path.Combine(Server.MapPath(ConfigurationManager.AppSettings["HospitalLogoPath"].ToString()), FileName);
                    objStaff.StaffPhoto.SaveAs(UploadPath);
                    cmd.Parameters.AddWithValue("@StaffPhoto", (DateTime.Now.ToString("yyyyMMdd") + "-" + objStaff.StaffPhoto.FileName));
                }
                cmd.Parameters.AddWithValue("@HospitalId", objStaff.HospitalId);
                cmd.Parameters.AddWithValue("@EmployeeCode", objStaff.EmployeeCode);
                cmd.Parameters.AddWithValue("@StaffSpecialization", objStaff.StaffSpecialization);
                cmd.Parameters.AddWithValue("@StaffDegree", objStaff.StaffDegree);
                cmd.Parameters.AddWithValue("@Designation", objStaff.Designation);

                cmd.Parameters.AddWithValue("@StaffPhone", objStaff.StaffPhone);
                cmd.Parameters.AddWithValue("@StaffEmail", objStaff.StaffEmail);

                cmd.Parameters.AddWithValue("@StaffCity", objStaff.StaffCity);
                cmd.Parameters.AddWithValue("@StaffState", objStaff.StaffState);
                cmd.Parameters.AddWithValue("@StaffCountry", objStaff.StaffCountry);

                cmd.Parameters.AddWithValue("@IsActive", objStaff.IsActive);
                cmd.Parameters.AddWithValue("@EntryDate", DateTime.Now);
                cmd.Parameters.AddWithValue("@EntryBy", objStaff.EntryBy);
                cmd.Parameters.AddWithValue("@RoleId", RoleId);

                cmd.Parameters.AddWithValue("@StaffAddress", objStaff.StaffAddress);


                //password generate


                string pass = generatePassword.CreateRandomPassword(10);
                objStaff.Password = pass;

                cmd.Parameters.AddWithValue("@Password", objStaff.Password);
                int i = cmd.ExecuteNonQuery();
                if (i >= 1)
                {
                    ViewBag.SuccessMessage = "success";
                    isSent = objsendEmail.SendEmail(objStaff.StaffEmail, objStaff.StaffName, objStaff.Password, fileName);
                }
                else
                {
                    ViewBag.ErrorMessage = "Failed";

                    ModelState.Clear();
                    return RedirectToAction("StaffList", "HospitalMaster");
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
            return RedirectToAction("AddStaff", "HospitalMaster");
        }




        public ActionResult HospitalHistory(string search)
        {
            List<Hospital> HospitalList = new List<Hospital>();
            SqlConnection con = new SqlConnection(cs);
            //SqlCommand cmd = new SqlCommand("spGetDetailsHospitalsMaster", con)
            //{
            //    CommandType = CommandType.StoredProcedure
            //};
            //SqlCommand cmd1 = new SqlCommand("Select * from HospitalMaster where HospitalName like '%" + HospitalSearch + "%'", con);
            if (con.State == System.Data.ConnectionState.Closed)
                con.Open();

            SqlCommand cmd = new SqlCommand("select * from HospitalMasterTriggerTable where HospitalName like '%" + search + "%'", con);
            cmd.Connection = con;
            SqlDataAdapter sd = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();

            //con.Open();
            sd.Fill(dt);
            con.Close();

            foreach (DataRow dr in dt.Rows)
            {
                HospitalList.Add(
                    new Hospital
                    {
                     
                        HospitalId = Convert.ToInt32(dr["HospitalId"]),
                        HospitalName = Convert.ToString(dr["HospitalName"]),
                        HospitalAddress = Convert.ToString(dr["HospitalAddress"]),
                        HospitalCity = Convert.ToString(dr["HospitalCity"]),
                        HospitalLogo = Convert.ToString(dr["HospitalLogo"]),
                        HospitalState = Convert.ToString(dr["HospitalState"]),
                        HospitalCountry = Convert.ToString(dr["HospitalCountry"]),
                        HospitalPhone = Convert.ToString(dr["HospitalPhone"]),
                        HospitalEmail = Convert.ToString(dr["HospitalEmail"]),
                        IsActive = Convert.ToBoolean(dr["IsActive"]),
                        EntryDateTime = Convert.ToString(dr["EntryDateTime"]),
                        EntryBy = Convert.ToString(dr["EntryBy"]),
                        ActionType = Convert.ToString(dr["ActionType"])
                    });
            }
            return View(HospitalList);
        }




        public ActionResult StaffHistory(string search)
        {
            List<Staff> StaffList = new List<Staff>();
            SqlConnection con = new SqlConnection(cs);
            //SqlCommand cmd = new SqlCommand("spStaffListDetails", con)
            //{
            //    CommandType = CommandType.StoredProcedure
            //};

            if (con.State == System.Data.ConnectionState.Closed)
                con.Open();

            SqlCommand cmd = new SqlCommand("select * from StaffMasterTriggerTable where HospitalId=@HospitalId  and StaffName like '%" + search + "%'", con);
            cmd.Connection = con;
            cmd.Parameters.AddWithValue("@HospitalId", Id);
            SqlDataAdapter sd = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();

            //con.Open();
            sd.Fill(dt);
            con.Close();

            foreach (DataRow dr in dt.Rows)
            {
                StaffList.Add(
                    new Staff
                    {
                        StaffId = Convert.ToInt32(dr["StaffId"]),
                        StaffName = Convert.ToString(dr["StaffName"]),
                        HospitalId = Convert.ToInt32(dr["HospitalId"]),
                        EmployeeCode = Convert.ToInt32(dr["EmployeeCode"]),
                        StaffSpecialization = Convert.ToString(dr["StaffSpecialization"]),
                        StaffDegree = Convert.ToString(dr["StaffDegree"]),
                        Designation = Convert.ToString(dr["Designation"]),

                        StaffPhone = Convert.ToString(dr["StaffPhone"]),
                        StaffEmail = Convert.ToString(dr["StaffEmail"]),

                        StaffCity = Convert.ToString(dr["StaffCity"]),
                        StaffState = Convert.ToString(dr["StaffState"]),
                        StaffCountry = Convert.ToString(dr["StaffCountry"]),
                        IsActive = Convert.ToBoolean(dr["IsActive"]),
                        EntryDate = Convert.ToString(dr["EntryDate"]),
                        EntryBy = Convert.ToString(dr["EntryBy"]),
                        StaffAddress = Convert.ToString(dr["StaffAddress"]),
                        ActionType = Convert.ToString(dr["ActionType"]),
                    });
            }
            return View(StaffList);
        }


        public ActionResult DoctorsHistory(string search)
        {
            Id = Session["Id"].ToString();
            List<Doctor> DoctorList = new List<Doctor>();
            SqlConnection con = new SqlConnection(cs);
            //SqlCommand cmd = new SqlCommand("spDoctorListDetails", con)
            //{
            //    CommandType = CommandType.StoredProcedure
            //};
            if (con.State == System.Data.ConnectionState.Closed)
                con.Open();

            SqlCommand cmd = new SqlCommand("select * from DoctorMasterTriggerTable where HospitalId=@HospitalId  and DoctorName like '%" + search + "%'", con);
            cmd.Connection = con;
            cmd.Parameters.AddWithValue("@HospitalId", Id);
            SqlDataAdapter sd = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();

            //con.Open();
            sd.Fill(dt);
            con.Close();

            foreach (DataRow dr in dt.Rows)
            {
                DoctorList.Add(
                    new Doctor
                    {
                        DoctorId = Convert.ToInt32(dr["DoctorId"]),
                        EmployeeCode = Convert.ToString(dr["EmployeeCode"]),
                        DoctorName = Convert.ToString(dr["DoctorName"]),

                        HospitalId = Convert.ToInt32(dr["HospitalId"]),
                        DoctorSpecialization = Convert.ToString(dr["DoctorSpecialization"]),
                        DoctorDegree = Convert.ToString(dr["DoctorDegree"]),
                        DoctorPhone = Convert.ToString(dr["DoctorPhone"]),
                        DoctorEmail = Convert.ToString(dr["DoctorEmail"]),

                        DoctorCity = Convert.ToString(dr["DoctorCity"]),
                        DoctorState = Convert.ToString(dr["DoctorState"]),
                        DoctorCountry = Convert.ToString(dr["DoctorCountry"]),
                        IsActive = Convert.ToBoolean(dr["IsActive"]),
                        EntryBy = Convert.ToString(dr["EntryBy"]),
                        DoctorAddress = Convert.ToString(dr["DoctorAddress"]),
                        ActionType = Convert.ToString(dr["ActionType"])
                    });
            }
            return View(DoctorList);
        }
    }
}