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
    public class PatientMasterController : Controller
    {
        private static readonly string cs = WebConfigurationManager.ConnectionStrings["dbECare"].ConnectionString;
        private static String email = "";
        // GET: Patient
   
        public ActionResult Dashboard()
        {
            email = Session["Email"].ToString();
            string cs = WebConfigurationManager.ConnectionStrings["dbECare"].ConnectionString;
            SqlConnection con = new SqlConnection(cs);
            con.Open();

            SqlCommand appointmentCountCmd = new SqlCommand("spGetCountAppointmentForUser", con);

            appointmentCountCmd.CommandType = CommandType.StoredProcedure;
            appointmentCountCmd.Parameters.AddWithValue("@Email", Session["Email"]);
            int appointmentCount = (int)appointmentCountCmd.ExecuteScalar();
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
                    SqlCommand cmd = new SqlCommand("spUpdatePatientCred", con)
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
            return RedirectToAction("Settings", "PatientMaster");
        }




    



        [HttpGet]
        public static List<Patient> GetPatient()
        {
            List<Patient> PatientList = new List<Patient>();
            SqlConnection con = new SqlConnection(cs);
            SqlCommand cmd = new SqlCommand("pPatientMasterDetails", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            SqlDataAdapter sd = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();

            con.Open();
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
                        Phone = Convert.ToString(dr["Phone"]),
                        Email = Convert.ToString(dr["Email"]),
                        Date = Convert.ToString(dr["Date"]),
                       EntryBy = Convert.ToString(dr["EntryBy"]),
                        EntryDateTime = Convert.ToString(dr["EntryDateTime"])
                   
                    });
            }
            return PatientList;
        }
          
        public ActionResult PatientProfile()
        {

            return View(GetPatient().Find(model => model.Id == (int)Session["Id"]));
        }
        [HttpPost]
        public ActionResult PatientProfile(Patient obj)

        {
            SqlConnection con = new SqlConnection(cs);
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("pUpdatePatientMasterProfile", con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@Id", (int)Session["Id"]);
                cmd.Parameters.AddWithValue("@Name", obj.Name);
                cmd.Parameters.AddWithValue("@Address", obj.Address);
                cmd.Parameters.AddWithValue("@City", obj.City);
                cmd.Parameters.AddWithValue("@State", obj.State);
                cmd.Parameters.AddWithValue("@Country", obj.Country);
                cmd.Parameters.AddWithValue("@Phone", obj.Phone);
                cmd.Parameters.AddWithValue("@Email", obj.Email);

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


            return RedirectToAction("Dashboard", "PatientMaster");
        }
               
           public ActionResult Delete(int id)
        {

            try
            {

                if (DeleteAppointment(id))
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

        public static bool DeleteAppointment(int id)
        {
            SqlConnection con = new SqlConnection(cs);

            SqlCommand cmd = new SqlCommand("pDeleteAppointmentMaster", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@AppointmentId", id);
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


    














   

        public ActionResult Edit(int id)
        {
            SqlConnection con = new SqlConnection(cs);
            con.Open(); if (con.State == System.Data.ConnectionState.Closed)
                con.Open();


            SqlCommand cmd = new SqlCommand("Select * From HospitalMaster", con);
            cmd.Connection = con;
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            sda.Fill(ds);
            ViewBag.state = ds.Tables[0];
            List<SelectListItem> getstate = new List<SelectListItem>();
            foreach (System.Data.DataRow dr in ViewBag.state.Rows)
            {
                getstate.Add(new SelectListItem { Text = dr["HospitalName"].ToString(), Value = dr["HospitalId"].ToString() });
            }
            ViewBag.HospitalId = getstate;
            return View(GetAppointment().Find(model => model.AppointmentId == id));
        }
        [HttpPost]
        public ActionResult Edit(int id, Appointment objAppointment)

        {
           // string HospitalId = Request.Form["HospitalId"].ToString();
            SqlConnection con = new SqlConnection(cs);
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("pUpdateAppointmentPatientMaster", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@AppointmentId", objAppointment.AppointmentId);
                cmd.Parameters.AddWithValue("@Name", objAppointment.Name);
                cmd.Parameters.AddWithValue("@Phone", objAppointment.Phone);
                //cmd.Parameters.AddWithValue("@HospitalId", HospitalId);
                cmd.Parameters.AddWithValue("@Gender", objAppointment.Gender);
                cmd.Parameters.AddWithValue("@Age", objAppointment.Age);
                cmd.Parameters.AddWithValue("@Problem", objAppointment.Problem);
                cmd.Parameters.AddWithValue("@Address", objAppointment.Address);
                cmd.Parameters.AddWithValue("@Date", objAppointment.Date);
                cmd.Parameters.AddWithValue("@Slot", objAppointment.Slot);
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




        public ActionResult Index()
        {
            return View(GetAppointment());
        }
        [HttpGet]
        public static List<Appointment> GetAppointment()
        {
            List<Appointment> AppointmentList = new List<Appointment>();
            SqlConnection con = new SqlConnection(cs);
            SqlCommand cmd = new SqlCommand("spGetDetailsAppointmentPatientMaster", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@Email", email);

            SqlDataAdapter sd = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            con.Open();
            sd.Fill(dt);
            con.Close();
            foreach (DataRow dr in dt.Rows)
            {
                AppointmentList.Add(
                    new Appointment
                    {
                        AppointmentId = Convert.ToInt32(dr["AppointmentId"]),
                        Name = Convert.ToString(dr["Name"]),
                        Phone = Convert.ToString(dr["Phone"]),
                       // HospitalId = Convert.ToInt32(dr["HospitalId"]),
                        AppointmentType = Convert.ToString(dr["AppointmentType"]),
                        Gender = Convert.ToString(dr["Gender"]),
                        Age = Convert.ToInt32(dr["Age"]),
                        Problem = Convert.ToString(dr["Problem"]),
                        Address = Convert.ToString(dr["Address"]),
                        Date = Convert.ToString(dr["Date"]),
                        Slot = Convert.ToString(dr["Slot"]),
                        EntryDateTime = Convert.ToString(dr["EntryDateTime"]),
                       
                        EntryBy = Convert.ToString(dr["EntryBy"])
                    });
            }
            return AppointmentList;
        }
                                         

        [HttpGet]
        public ActionResult Create()
        {

            SqlConnection con = new SqlConnection(cs);
            con.Open(); if (con.State == System.Data.ConnectionState.Closed)
                con.Open();


            SqlCommand cmd = new SqlCommand("Select * From HospitalMaster", con);
            cmd.Connection = con;
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            sda.Fill(ds);
            ViewBag.state = ds.Tables[0];
            List<SelectListItem> getstate = new List<SelectListItem>();
            foreach (System.Data.DataRow dr in ViewBag.state.Rows)
            {
                getstate.Add(new SelectListItem { Text = dr["HospitalName"].ToString(), Value = dr["HospitalId"].ToString() });
            }
            ViewBag.HospitalId = getstate;

            return View();
        }

        [HttpPost]

        public ActionResult Create(Appointment f)
        {
            //if (ModelState.IsValid)
            //{
                string HospitalId = Request.Form["HospitalId"].ToString();

                SqlConnection con = new SqlConnection(cs);
                try
                {
                    con.Open();
                    SqlCommand cmd1 = new SqlCommand("pInsertAppointmentPatientMaster", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd1.Parameters.AddWithValue("@Name", f.Name);
                    cmd1.Parameters.AddWithValue("@Phone", f.Phone);
                    cmd1.Parameters.AddWithValue("@AppointmentType", f.AppointmentType);
                    cmd1.Parameters.AddWithValue("@Gender", f.Gender);
                    cmd1.Parameters.AddWithValue("@HospitalId", HospitalId);
                    cmd1.Parameters.AddWithValue("@Age", f.Age);
                    cmd1.Parameters.AddWithValue("@Problem", f.Problem);
                    cmd1.Parameters.AddWithValue("@Address", f.Address);
                    cmd1.Parameters.AddWithValue("@Payment", f.Payment);
                    cmd1.Parameters.AddWithValue("@Date", f.Date);
                    cmd1.Parameters.AddWithValue("@Slot", f.Slot);
                    cmd1.Parameters.AddWithValue("@EntryBy", f.EntryBy);
                    cmd1.Parameters.AddWithValue("@EntryDateTime", DateTime.Now);
                    cmd1.Parameters.AddWithValue("@Doctor", f.Doctor);



                    int i = cmd1.ExecuteNonQuery();
                    if (i >= 1)
                        ViewBag.SuccessMessage = "success";
                    else
                        ViewBag.ErrorMessage = "Failed";


                    ModelState.Clear();
                    return RedirectToAction("Index", "PatientMaster");
                }

                catch (SqlException ex)
                { }
                finally
                {
                    con.Close();
                }
                ModelState.Clear();
            //}
            return RedirectToAction("Dashboard", "PatientMaster");
        }



        public ActionResult Chat()
        {
            return View();
        }




        public ActionResult Prescription()
        {
            email = Session["Email"].ToString();
            List<Vitals> AppointmentList = new List<Vitals>();
            SqlConnection con = new SqlConnection(cs);
            //SqlCommand cmd = new SqlCommand("spGetDetailsHospitalsMaster", con)
            //{
            //    CommandType = CommandType.StoredProcedure
            //};
            //SqlCommand cmd1 = new SqlCommand("Select * from HospitalMaster where HospitalName like '%" + HospitalSearch + "%'", con);
         
            SqlCommand cmd = new SqlCommand("SpDetailsDoctorCheckupPatientMaster", con)
            {
                CommandType = CommandType.StoredProcedure
            };  
                  cmd.Parameters.AddWithValue("@Email", email);
            cmd.Connection = con;
            SqlDataAdapter sd = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();

            //con.Open();
            sd.Fill(dt);
            con.Close();

            foreach (DataRow dr in dt.Rows)
            {
                AppointmentList.Add(
                    new Vitals
                    {
                        Id = Convert.ToInt32(dr["Id"]),
                        PatientEmail = Convert.ToString(dr["PatientEmail"]),
                        BloodPressure = Convert.ToString(dr["BloodPressure"]),
                        Temperature = Convert.ToString(dr["Temperature"]),
                        Pulse = Convert.ToString(dr["Pulse"]),
                        Symptoms = Convert.ToString(dr["Symptoms"]),
                        Diagnosis = Convert.ToString(dr["Diagnosis"]),
                        Remarks = Convert.ToString(dr["Remarks"]),
                        MedicineName = Convert.ToString(dr["MedicineName"]),
                        Dosage = Convert.ToString(dr["Dosage"]),
                        Timings = Convert.ToString(dr["Timings"]),
                        EntryDateTime = Convert.ToString(dr["EntryDateTime"]),
                        EntryBy = Convert.ToString(dr["EntryBy"])
                    });
            }
            return View(AppointmentList);
        }
    }
}