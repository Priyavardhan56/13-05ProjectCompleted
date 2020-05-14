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
    public class DoctorMasterController : Controller
    {
        // GET: DoctorMaster
        private static readonly string cs = WebConfigurationManager.ConnectionStrings["dbECare"].ConnectionString;
        private static String email = "";
        private static String DoctorId = "";
        private static String Id = "";
        private static String PatientId = "";
        private static String HospitalId = "";
        private static String Name = "";

        public ActionResult Index()
        {
            return View(GetDoctors());
        }

        public ActionResult Create()
        {
            return View();
        }



        [HttpPost]
        public ActionResult Create(Doctor objDoctor)
        {
            //eCareSendEmail objsendEmail = new eCareSendEmail();
            bool isSent = false;
            String fileName = Server.MapPath("~/App_Data/hospitalCredentialEmail.txt");

            if (ModelState.IsValid)
            {
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
            }
            return View();
        }




        public ActionResult Delete(int id)
        {

            try
            {

                if (DeleteDoctor(id))
                {
                    ViewBag.SuccessMessage = "Success";

                }
                else
                {
                    ViewBag.ErrorMessage = "error";

                }
                return RedirectToAction("DoctorsList","HospitalMaster");
            }
            catch
            {
                return View();
            }

        }

        public ActionResult Edit(int id)
        {
            return View(GetDoctors().Find(model => model.DoctorId == id));
        }
        [HttpPost]
        public ActionResult Edit(int id, Doctor objDoctor)

        {
            SqlConnection con = new SqlConnection(cs);
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("pUpdateDoctorMaster", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@DoctorId", objDoctor.DoctorId);
                cmd.Parameters.AddWithValue("@EmployeeCode", objDoctor.EmployeeCode);
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

                cmd.Parameters.AddWithValue("@EntryBy", objDoctor.EntryBy);
                cmd.Parameters.AddWithValue("@DoctorAddress", objDoctor.DoctorAddress);


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


            return RedirectToAction("DoctorsList","HospitalMaster");
        }


        [HttpGet]
        public static List<Doctor> GetDoctors()
        {
           
            List<Doctor> DoctorList = new List<Doctor>();
            SqlConnection con = new SqlConnection(cs);
            SqlCommand cmd = new SqlCommand("pDoctorMasterDetails", con)
            {
                CommandType = CommandType.StoredProcedure
            };
          
            //cmd.Parameters.AddWithValue("@Email",email);
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
        public static bool DeleteDoctor(int id)
        {
            SqlConnection con = new SqlConnection(cs);

            SqlCommand cmd = new SqlCommand("pDeleteDoctorMaster", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@DoctorId", id);
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
                    SqlCommand cmd = new SqlCommand("spUpdateDoctorCred", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.AddWithValue("@DoctorId", obj.Id);
                    cmd.Parameters.AddWithValue("@Password", obj.Password);


                    int i = cmd.ExecuteNonQuery();
                    if (i >= 1)
                        ViewBag.SuccessMessage = "success";
                    else
                        ViewBag.ErrorMessage = "Failed";

                    return RedirectToAction("Index", "Home");
                }
                catch (SqlException ex)
                { }
                finally
                {
                    con.Close();
                }
                ModelState.Clear();

            }
            return View();
        }

        public ActionResult DoctorProfile()
        {

            return View(GetDoctors().Find(model => model.DoctorId == (int)Session["Id"]));
        }
        [HttpPost]
        public ActionResult DoctorProfile(Doctor objDoctor)

        {
            SqlConnection con = new SqlConnection(cs);
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("spUpdateDoctorProfile", con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@DoctorId", (int)Session["Id"]);
                cmd.Parameters.AddWithValue("@DoctorName", objDoctor.DoctorName);
                cmd.Parameters.AddWithValue("@DoctorAddress", objDoctor.DoctorAddress);
                cmd.Parameters.AddWithValue("@DoctorCity", objDoctor.DoctorCity);
                cmd.Parameters.AddWithValue("@DoctorState", objDoctor.DoctorState);
                cmd.Parameters.AddWithValue("@DoctorCountry", objDoctor.DoctorCountry);
                cmd.Parameters.AddWithValue("@DoctorPhone", objDoctor.DoctorPhone);
                cmd.Parameters.AddWithValue("@DoctorEmail", objDoctor.DoctorEmail);

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


            return RedirectToAction("Dashboard", "DoctorMaster");
        }

        public ActionResult Dashboard()
        {
            email = Session["Email"].ToString();
            string cs = WebConfigurationManager.ConnectionStrings["dbECare"].ConnectionString;
            SqlConnection con = new SqlConnection(cs);
            con.Open();

            SqlCommand appointmentCountCmd = new SqlCommand("spDoctorAppointmentsCount", con);
            appointmentCountCmd.CommandType = CommandType.StoredProcedure;
            appointmentCountCmd.Parameters.AddWithValue("@DoctorId", Session["Id"]);
            int appointmentCount = (int)appointmentCountCmd.ExecuteScalar();
            ViewBag.appointmentCount = appointmentCount;  

            return View();
        }

        //public ActionResult Vitals()
        //{
        //    return View();
        //}



        //[HttpPost]
        //public ActionResult Vitals(Vitals objDoctor)
        //{
        //    SqlConnection con = new SqlConnection(cs);
        //    try
        //    {
        //        con.Open();
        //        SqlCommand cmd = new SqlCommand("spInsertVitalsMaster", con)
        //        {
        //            CommandType = CommandType.StoredProcedure
        //        };

        //        cmd.Parameters.AddWithValue("@BloodPressure", objDoctor.BloodPressure);
        //        cmd.Parameters.AddWithValue("@Temperature", objDoctor.Temperature);
        //        cmd.Parameters.AddWithValue("@Pulse", objDoctor.Pulse);
        //        cmd.Parameters.AddWithValue("@DoctorName", objDoctor.DoctorName);
        //        cmd.Parameters.AddWithValue("@PatientName", objDoctor.PatientName);
        //        cmd.Parameters.AddWithValue("@HospitalId", objDoctor.HospitalId);
        //        cmd.Parameters.AddWithValue("@EntryDate", DateTime.Now);
        //        cmd.Parameters.AddWithValue("@EntryBy", objDoctor.EntryBy);
        //        int i = cmd.ExecuteNonQuery();
        //        if (i >= 1)
        //            ViewBag.SuccessMessage = "success";
        //        else
        //            ViewBag.ErrorMessage = "Failed";


        //    }
        //    catch (SqlException ex)
        //    { }
        //    finally
        //    {
        //        con.Close();
        //    }
        //    ModelState.Clear();
        //    return View();
        //}



        //public ActionResult Medicine()
        //{
        //    return View();
        //}



        //[HttpPost]
        //public ActionResult Medicine(Medicine objDoctor)
        //{
        //    SqlConnection con = new SqlConnection(cs);
        //    try
        //    {
        //        con.Open();
        //        SqlCommand cmd = new SqlCommand("pInsertMedicineMaster", con)
        //        {
        //            CommandType = CommandType.StoredProcedure
        //        };

        //        cmd.Parameters.AddWithValue("@MedicineName", objDoctor.MedicineName);
        //        cmd.Parameters.AddWithValue("@Dosage", objDoctor.Dosage);
        //        cmd.Parameters.AddWithValue("@Timings", objDoctor.Timings);
        //        cmd.Parameters.AddWithValue("@DoctorName", objDoctor.DoctorName);
        //        cmd.Parameters.AddWithValue("@PatientName", objDoctor.PatientName);
        //        cmd.Parameters.AddWithValue("@HospitalId", objDoctor.HospitalId);
        //        cmd.Parameters.AddWithValue("@EntryDate", objDoctor.EntryDate);
        //        cmd.Parameters.AddWithValue("@EntryBy", objDoctor.EntryBy);
        //        int i = cmd.ExecuteNonQuery();
        //        if (i >= 1)
        //            ViewBag.SuccessMessage = "success";
        //        else
        //            ViewBag.ErrorMessage = "Failed";


        //    }
        //    catch (SqlException ex)
        //    { }
        //    finally
        //    {
        //        con.Close();
        //    }
        //    ModelState.Clear();
        //    return View();
        //}


        //public ActionResult Opd()
        //{
        //    return View();
        //}



        //[HttpPost]
        //public ActionResult Opd(Opd objDoctor)
        //{
        //    SqlConnection con = new SqlConnection(cs);
        //    try
        //    {
        //        con.Open();
        //        SqlCommand cmd = new SqlCommand("pInsertOpdMaster", con)
        //        {
        //            CommandType = CommandType.StoredProcedure
        //        };

        //        cmd.Parameters.AddWithValue("@Symptoms", objDoctor.Symptoms);
        //        cmd.Parameters.AddWithValue("@Diagnosis", objDoctor.Diagnosis);
        //        cmd.Parameters.AddWithValue("@Remarks", objDoctor.Remarks);
        //        cmd.Parameters.AddWithValue("@ReferedTo", objDoctor.RefferedTo);
        //        cmd.Parameters.AddWithValue("@DoctorName", objDoctor.DoctorName);
        //        cmd.Parameters.AddWithValue("@PatientName", objDoctor.PatientName);
        //        cmd.Parameters.AddWithValue("@HospitalId", objDoctor.HospitalId);
        //        cmd.Parameters.AddWithValue("@EntryDate", objDoctor.EntryDate);
        //        cmd.Parameters.AddWithValue("@EntryBy", objDoctor.EntryBy);
        //        int i = cmd.ExecuteNonQuery();
        //        if (i >= 1)
        //            ViewBag.SuccessMessage = "success";
        //        else
        //            ViewBag.ErrorMessage = "Failed";


        //    }
        //    catch (SqlException ex)
        //    { }
        //    finally
        //    {
        //        con.Close();
        //    }
        //    ModelState.Clear();
        //    return View();
        //}






























        //public ActionResult DoctorCheckUp()
        //{
        //    return View();
        //}

        //[HttpPost]
        //public ActionResult DoctorCheckUp(DoctorCheckUp obj)
        //{

        //    SqlConnection con = new SqlConnection(cs);
        //    try
        //    {
        //        con.Open();
        //        SqlCommand cmd = new SqlCommand("spInsertVitalsSymptomsMedicineMaster", con)
        //        {
        //            CommandType = CommandType.StoredProcedure
        //        };
                
          
        //    cmd.CommandType = System.Data.CommandType.StoredProcedure;
           
        //    cmd.Parameters.AddWithValue("@BloodPressure", obj.BloodPressure);
        //    cmd.Parameters.AddWithValue("@Temperature", obj.Temperature);
        //    cmd.Parameters.AddWithValue("@Pulse", obj.Pulse);
        //    cmd.Parameters.AddWithValue("@Symptoms", obj.Symptoms);
        //    cmd.Parameters.AddWithValue("@Diagnosis", obj.Diagnosis);
        //    cmd.Parameters.AddWithValue("@Remarks", obj.Remarks);
        //    cmd.Parameters.AddWithValue("@RefferedTo", obj.RefferedTo);
        //    cmd.Parameters.AddWithValue("@RefferedRemarks", obj.RefferedRemarks);
        //    cmd.Parameters.AddWithValue("@MedicineName", obj.MedicineName);
        //    cmd.Parameters.AddWithValue("@Dosage", obj.Dosage);
        //    cmd.Parameters.AddWithValue("@Timings", obj.Timings);
        //        int i = cmd.ExecuteNonQuery();
        //        if (i >= 1)
        //            ViewBag.SuccessMessage = "success";
        //        else
        //            ViewBag.ErrorMessage = "Failed";


        //    }
        //    catch (SqlException ex)
        //    { }
        //    finally
        //    {
        //        con.Close();
        //    }
        //    ModelState.Clear();
        //    return View();
        //}














        public ActionResult Appointments(string search)
        {
            Id = Session["Id"].ToString();
            List<Appointment> AppointmentList = new List<Appointment>();
            SqlConnection con = new SqlConnection(cs);
            //SqlCommand cmd = new SqlCommand("pDoctorAppointmentDetails", con)
            //{
            //    CommandType = CommandType.StoredProcedure
            //};
            if (con.State == System.Data.ConnectionState.Closed)
                con.Open();

            SqlCommand cmd = new SqlCommand("select * from AppointmentMaster where Doctor=@DoctorId  and Name like '%" + search + "%'", con);
            cmd.Connection = con;
            cmd.Parameters.AddWithValue("@DoctorId", Id);

            SqlDataAdapter sd = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            //con.Open();
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
                        HospitalId = Convert.ToInt32(dr["HospitalId"]),
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
            return View(AppointmentList);
        }
        [HttpGet]
        public static List<Appointment> GetAppointment()
        {
            List<Appointment> AppointmentList = new List<Appointment>();
            SqlConnection con = new SqlConnection(cs);
            SqlCommand cmd = new SqlCommand("pDoctorAppointmentDetails", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@DoctorId", Id);

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
                        HospitalId = Convert.ToInt32(dr["HospitalId"]),
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







        public ActionResult EditAppointments(int id)
        {
          
            return View(GetAppointment().Find(model => model.AppointmentId == id));
        }
        [HttpPost]
        public ActionResult EditAppointments(int id, Appointment objAppointment)

        {
      
            SqlConnection con = new SqlConnection(cs);
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("spDoctorUpdatingAppointmentPatientMaster", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@AppointmentId", objAppointment.AppointmentId);
                cmd.Parameters.AddWithValue("@Name", objAppointment.Name);
                cmd.Parameters.AddWithValue("@Phone", objAppointment.Phone);
            
                cmd.Parameters.AddWithValue("@Gender", objAppointment.Gender);
                cmd.Parameters.AddWithValue("@Age", objAppointment.Age);
                cmd.Parameters.AddWithValue("@Problem", objAppointment.Problem);
                cmd.Parameters.AddWithValue("@Address", objAppointment.Address);
                cmd.Parameters.AddWithValue("@Date", objAppointment.Date);
                cmd.Parameters.AddWithValue("@Slot", objAppointment.Slot);
                cmd.Parameters.AddWithValue("@EntryDateTime", DateTime.Now);

                int i = cmd.ExecuteNonQuery();
                if (i >= 1)
                {
                    ViewBag.SuccessMessage = "success";
                    return RedirectToAction("Appointments","DoctorMaster");
                }
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


            return RedirectToAction("Appointments");
        }





        public ActionResult DeletePatientAppointment(int id)
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
                return RedirectToAction("Appointments","DoctorMaster");
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


        public ActionResult Chat()
        {
            return View();
        }




        public ActionResult DoctorCheckup()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DoctorCheckup(Vitals obj)
        {
            SqlConnection con = new SqlConnection(cs);
            try
            {
                if (con.State == System.Data.ConnectionState.Closed)
                    con.Open();
                SqlCommand cmd = new SqlCommand("SpDoctorCheckup", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;            
     
                cmd.Parameters.AddWithValue("@PatientEmail ", obj.PatientEmail);              
                cmd.Parameters.AddWithValue("@BloodPressure", obj.BloodPressure);
                cmd.Parameters.AddWithValue("@Temperature", obj.Temperature);
                cmd.Parameters.AddWithValue("@Pulse", obj.Pulse);
                cmd.Parameters.AddWithValue("@Symptoms", obj.Symptoms);
                cmd.Parameters.AddWithValue("@Diagnosis", obj.Diagnosis);
                cmd.Parameters.AddWithValue("@Remarks", obj.Remarks);
                cmd.Parameters.AddWithValue("@MedicineName", obj.MedicineName);
                cmd.Parameters.AddWithValue("@Dosage", obj.Dosage);
                cmd.Parameters.AddWithValue("@Timings", obj.Timings);
                cmd.Parameters.AddWithValue("@EntryBy", obj.EntryBy);
                cmd.Parameters.AddWithValue("@EntryDateTime",DateTime.Now);
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
            return View();

        }



        public ActionResult Prescription()
        {
            Name = Session["Name"].ToString();
            List<Vitals> AppointmentList = new List<Vitals>();
            SqlConnection con = new SqlConnection(cs);
            //SqlCommand cmd = new SqlCommand("spGetDetailsHospitalsMaster", con)
            //{
            //    CommandType = CommandType.StoredProcedure
            //};
            //SqlCommand cmd1 = new SqlCommand("Select * from HospitalMaster where HospitalName like '%" + HospitalSearch + "%'", con);

            SqlCommand cmd = new SqlCommand("SpDetailsDoctorCheckupPrescriptionPatientMaster", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@Name", Name);
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







        
                