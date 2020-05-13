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
    public class StaffMasterController : Controller
    {
        // GET: StaffMaster
        private static readonly string cs = WebConfigurationManager.ConnectionStrings["dbECare"].ConnectionString;
        private static String Id = "";
        public ActionResult Index()
        {
            return View(GetStaffs());
        }

        public ActionResult Create()
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
        public ActionResult Create(Staff objStaff)
        {
            
            //eCareSendEmail objsendEmail = new eCareSendEmail();
            bool isSent = false;
            String fileName = Server.MapPath("~/App_Data/hospitalCredentialEmail.txt");
            if (ModelState.IsValid)
            {
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
                        ViewBag.SuccessMessage = "success";
                    else
                        ViewBag.ErrorMessage = "Failed";

                    ModelState.Clear();
                    return RedirectToAction("Index", "StaffMaster");

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
            }
            return RedirectToAction("Create", "StaffMaster");
        }




        public ActionResult Delete(int id)
        {

            try
            {

                if (DeleteStaff(id))
                {
                    ViewBag.SuccessMessage = "Success";

                }
                else
                {
                    ViewBag.ErrorMessage = "error";

                }
                return RedirectToAction("StaffList","HospitalMaster");
            }
            catch
            {
                return View();
            }

        }

        public ActionResult Edit(int id)
        {
            Id = Session["Id"].ToString();
            return View(GetStaffs().Find(model => model.StaffId == id));
        }
        [HttpPost]
        public ActionResult Edit(int id, Staff objStaff)

        {
            SqlConnection con = new SqlConnection(cs);
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("pUPdateStaffMaster", con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@StaffId", objStaff.StaffId);
                cmd.Parameters.AddWithValue("@StaffName", objStaff.StaffName);
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
                cmd.Parameters.AddWithValue("@StaffAddress", objStaff.StaffAddress);


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


            return RedirectToAction("StaffList", "HospitalMaster");
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
        public static bool DeleteStaff(int id)
        {
            SqlConnection con = new SqlConnection(cs);

            SqlCommand cmd = new SqlCommand("pDeleteStaffMaster", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@StaffId", id);
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

    }
}