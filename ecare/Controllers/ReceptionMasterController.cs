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
    public class ReceptionMasterController : Controller
    {
        // GET: ReceptionMaster
        private static readonly string cs = WebConfigurationManager.ConnectionStrings["dbECare"].ConnectionString;
        private static String HospitalId = "";
        private static String psearch = "";
        private static String Id = "";
        public ActionResult Index()
        {
            return View(GetReceptions());
        }

        public ActionResult Create()
        {
            return View();
        }



        [HttpPost]
        public ActionResult Create(Appointment objReception)
        {
            //if (ModelState.IsValid)
            //{
                SqlConnection con = new SqlConnection(cs);
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("pInsertReceptionMaster", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };


                    cmd.Parameters.AddWithValue("@Name", objReception.Name);
                    cmd.Parameters.AddWithValue("@Phone", objReception.Phone);
                    cmd.Parameters.AddWithValue("@AppointmentType", objReception.AppointmentType);
                    cmd.Parameters.AddWithValue("@Gender", objReception.Gender);
                    cmd.Parameters.AddWithValue("@Age", objReception.Age);
                    cmd.Parameters.AddWithValue("@Problem", objReception.Problem);
                    cmd.Parameters.AddWithValue("@Address", objReception.Address);
                    cmd.Parameters.AddWithValue("@Payment", objReception.Payment);
                    cmd.Parameters.AddWithValue("@HospitalId", objReception.HospitalId);
                    cmd.Parameters.AddWithValue("@Date", objReception.Date);
                    if (objReception.Slot == null)
                    {
                        cmd.Parameters.AddWithValue("@Slot", DateTime.Parse("00:00:00"));
                    }
                    else
                        cmd.Parameters.AddWithValue("@Slot", objReception.Slot);
                    cmd.Parameters.AddWithValue("@EntryBy", objReception.EntryBy);
                    cmd.Parameters.AddWithValue("@EntryDateTime", DateTime.Now);
                cmd.Parameters.AddWithValue("@Doctor", objReception.Doctor);

                    int i = cmd.ExecuteNonQuery();
                    if (i >= 1)
                        ViewBag.SuccessMessage = "success";
                    else
                        ViewBag.ErrorMessage = "Failed";

                    ModelState.Clear();
                    return RedirectToAction("Index", "ReceptionMaster");
                }
                catch (SqlException ex)
                {
                    ViewBag.ErrorMessage = "Failed";
                }
                finally
                {
                    con.Close();
                }
                ModelState.Clear();
            //}
            return View();
        }




        public ActionResult Delete(int id)
        {

            try
            {

                if (DeleteReception(id))
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
            return View(GetReceptions().Find(model => model.AppointmentId == id));
        }
        [HttpPost]
        public ActionResult Edit(int id, Appointment objReception)

        {

            SqlConnection con = new SqlConnection(cs);
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("pUpdateReceptionMaster", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@AppointmentId", objReception.AppointmentId);
                cmd.Parameters.AddWithValue("@Name", objReception.Name);
                cmd.Parameters.AddWithValue("@Phone", objReception.Phone);
                cmd.Parameters.AddWithValue("@AppointmentType", objReception.AppointmentType);
                cmd.Parameters.AddWithValue("@Gender", objReception.Gender);
                cmd.Parameters.AddWithValue("@Age", objReception.Age);
                cmd.Parameters.AddWithValue("@Problem", objReception.Problem);
                cmd.Parameters.AddWithValue("@Address", objReception.Address);
                cmd.Parameters.AddWithValue("@Payment", objReception.Payment);
                cmd.Parameters.AddWithValue("@Slot", objReception.Slot);
                cmd.Parameters.AddWithValue("@Date", objReception.Date);
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


        [HttpGet]
        public static List<Appointment> GetReceptions()
        {
            List<Appointment> ReceptionList = new List<Appointment>();
            SqlConnection con = new SqlConnection(cs);
            SqlCommand cmd = new SqlCommand("pDetailsCountAppointmentReceptionMaster", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@HospitalId", HospitalId);
            SqlDataAdapter sd = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();

            con.Open();
            sd.Fill(dt);
            con.Close();

            foreach (DataRow dr in dt.Rows)
            {
                ReceptionList.Add(
                    new Appointment
                    {
                        AppointmentId = Convert.ToInt32(dr["AppointmentId"]),
                        Name = Convert.ToString(dr["Name"]),
                        Phone = Convert.ToString(dr["Phone"]),
                        AppointmentType = Convert.ToString(dr["AppointmentType"]),
                        Gender = Convert.ToString(dr["Gender"]),
                        Age = Convert.ToInt32(dr["Age"]),

                        Problem = Convert.ToString(dr["Problem"]),
                        Address = Convert.ToString(dr["Address"]),
                        Payment = Convert.ToString(dr["Payment"]),
                        Slot = Convert.ToString(dr["Slot"]),
                        Date = Convert.ToString(dr["Date"]),
                        EntryDateTime = Convert.ToString(dr["EntryDateTime"]),
                        EntryBy = Convert.ToString(dr["EntryBy"])
                    });
            }
            return ReceptionList;
        }
        public static bool DeleteReception(int id)
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



        public ActionResult ReceptionProfile()
        {
            return View();
        }


        //public ActionResult Settings()
        //{
        //    return View();
        //}
        //[HttpPost]
        //public ActionResult Settings(Credentials obj)

        //{
        //    SqlConnection con = new SqlConnection(cs);
        //    try
        //    {
        //        con.Open();
        //        SqlCommand cmd = new SqlCommand("spUpdateAdminCred", con)
        //        {
        //            CommandType = CommandType.StoredProcedure
        //        };
        //        cmd.Parameters.AddWithValue("@Id", obj.Id);
        //        cmd.Parameters.AddWithValue("@Password", obj.Password);


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


        //    return RedirectToAction("Index");
        //}

        public ActionResult Dashboard()
        {
            HospitalId = Session["HospitalId"].ToString();
            string cs = WebConfigurationManager.ConnectionStrings["dbECare"].ConnectionString;
            SqlConnection con = new SqlConnection(cs);
            con.Open();

            SqlCommand appointmentCountCmd = new SqlCommand("spDetailsReceptionAppointmentsCount", con);
            appointmentCountCmd.CommandType = CommandType.StoredProcedure;
            appointmentCountCmd.Parameters.AddWithValue("@HospitalId", Session["HospitalId"]);
            int appointmentCount = (int)appointmentCountCmd.ExecuteScalar();
            ViewBag.appointmentCount = appointmentCount;



            //int appointmentCount = (int)appointmentCountCmd.ExecuteScalar();



            //ViewBag.appointmentCount = appointmentCount;



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
                    SqlCommand cmd = new SqlCommand("spChangePasswordStaffCred", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.AddWithValue("@StaffId", obj.Id);
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
            return RedirectToAction("Settings", "HospitalMaster");
        }


        [HttpGet]
        public ActionResult AssignDoctor(int id)
        {
            SqlConnection con = new SqlConnection(cs);
            if (con.State == System.Data.ConnectionState.Closed)
                con.Open();


            SqlCommand cmd = new SqlCommand("Select * From DoctorMaster where HospitalId=@HospitalId", con);
            cmd.Connection = con;
            cmd.Parameters.AddWithValue("@HospitalId", (int)Session["HospitalId"]);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            sda.Fill(ds);
            ViewBag.state = ds.Tables[0];
            List<SelectListItem> getstate = new List<SelectListItem>();
            foreach (System.Data.DataRow dr in ViewBag.state.Rows)
            {
                getstate.Add(new SelectListItem { Text = dr["DoctorName"].ToString(), Value = dr["DoctorId"].ToString() });
            }
            ViewBag.DoctorId = getstate;

            return View(GetAppointment().Find(model => model.AppointmentId == id));
        }
        [HttpPost]
        public ActionResult AssignDoctor( Appointment objAppointment)

        { string DoctorId = Request.Form["DoctorId"].ToString();
           
            SqlConnection con = new SqlConnection(cs);
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("pUpdateReceptionPatientMaster", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@AppointmentId", objAppointment.AppointmentId);
                cmd.Parameters.AddWithValue("@Name", objAppointment.Name);
                cmd.Parameters.AddWithValue("@Phone", objAppointment.Phone);
                
                cmd.Parameters.AddWithValue("@Gender", objAppointment.Gender);
                cmd.Parameters.AddWithValue("@Age", objAppointment.Age);
                cmd.Parameters.AddWithValue("@Payment", objAppointment.Payment);
                cmd.Parameters.AddWithValue("@Problem", objAppointment.Problem);
                cmd.Parameters.AddWithValue("@Address", objAppointment.Address);
                cmd.Parameters.AddWithValue("@Date", objAppointment.Date);
                cmd.Parameters.AddWithValue("@Slot", objAppointment.Slot);
                cmd.Parameters.AddWithValue("@EntryDateTime", DateTime.Now);
                cmd.Parameters.AddWithValue("@Doctor", DoctorId);

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


            return RedirectToAction("AppointmentList");
        }

       
        //public ActionResult AppointmentList()
        //{
        //             return View(GetAppointment());
        //}
        [HttpGet]
        public static List<Appointment> GetAppointment()
        {


            List<Appointment> AppointmentList = new List<Appointment>();
            SqlConnection con = new SqlConnection(cs);
            SqlCommand cmd = new SqlCommand("pDetailsCountAppointmentReceptionMaster", con)

            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@HospitalId", HospitalId);
            //if (con.State == System.Data.ConnectionState.Closed)
            //    con.Open();

            //SqlCommand cmd = new SqlCommand("select * from AppointmentMaster where HospitalId=@HospitalId  and Name like '%" + psearch + "%'", con);
            //cmd.Connection = con;


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
                        Payment = Convert.ToString(dr["Payment"]),
                        Problem = Convert.ToString(dr["Problem"]),
                        Address = Convert.ToString(dr["Address"]),
                        Date = Convert.ToString(dr["Date"]),
                        Slot = Convert.ToString(dr["Slot"]),
                        EntryDateTime = Convert.ToString(dr["EntryDateTime"]),
                        Doctor= Convert.ToString(dr["Doctor"]),
                        EntryBy = Convert.ToString(dr["EntryBy"])
                    });
            }
            return AppointmentList;
        }


        public ActionResult AppointmentList(string psearch)
        {


            List<Appointment> AppointmentList = new List<Appointment>();
            SqlConnection con = new SqlConnection(cs);
            //SqlCommand cmd = new SqlCommand("pDetailsCountAppointmentReceptionMaster", con)

            //{
            //    CommandType = CommandType.StoredProcedure
            //};

            if (con.State == System.Data.ConnectionState.Closed)
                con.Open();

            SqlCommand cmd = new SqlCommand("select * from AppointmentMaster where HospitalId=@HospitalId  and Phone like '%" + psearch + "%'", con);
            cmd.Connection = con;
            cmd.Parameters.AddWithValue("@HospitalId", HospitalId);
            //cmd.Parameters.AddWithValue("@psearch", psearch);

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
                        Payment = Convert.ToString(dr["Payment"]),
                        Problem = Convert.ToString(dr["Problem"]),
                        Address = Convert.ToString(dr["Address"]),
                        Date = Convert.ToString(dr["Date"]),
                        Slot = Convert.ToString(dr["Slot"]),
                        EntryDateTime = Convert.ToString(dr["EntryDateTime"]),
                        Doctor = Convert.ToString(dr["Doctor"]),
                        EntryBy = Convert.ToString(dr["EntryBy"])
                    });
            }
            return View(AppointmentList);
        }















        //public ActionResult ThreeTableList()
        //{
        //    return View(GetAppointmentForThreeTables());

        //}
        //[HttpGet]
        //public static List<Appointment> GetAppointmentForThreeTables()
        //{
        //    List<Appointment> AppointmentList = new List<Appointment>();
        //    SqlConnection con = new SqlConnection(cs);
        //    SqlCommand cmd = new SqlCommand("spDetailsThreeTables", con)
        //    {
        //        CommandType = CommandType.StoredProcedure
        //    };
        //    SqlCommand cmd1 = new SqlCommand("spDetailsSecondTables", con)
        //    {
        //        CommandType = CommandType.StoredProcedure
        //    };
        //    SqlCommand cmd2 = new SqlCommand("spDetailsFirstTables", con)
        //    {
        //        CommandType = CommandType.StoredProcedure
        //    };
        //    //cmd.Parameters.AddWithValue("@Email", email);

        //    SqlDataAdapter sd = new SqlDataAdapter(cmd);
        //    DataTable dt = new DataTable();
        //    con.Open();
        //    sd.Fill(dt);
        //    SqlDataAdapter sd1 = new SqlDataAdapter(cmd1);
        //    DataTable dt1 = new DataTable();

        //    sd1.Fill(dt1);
        //    SqlDataAdapter sd2 = new SqlDataAdapter(cmd2);
        //    DataTable dt2 = new DataTable();

        //    sd2.Fill(dt2);
        //    con.Close();
        //    foreach (DataRow dr in dt.Rows)
        //    {
        //        AppointmentList.Add(
        //            new Appointment
        //            {
        //                AppointmentId = Convert.ToInt32(dr["AppointmentId"]),
        //                Name = Convert.ToString(dr["Name"]),
        //                Phone = Convert.ToString(dr["Phone"]),
        //                HospitalId = Convert.ToInt32(dr["HospitalId"]),
        //                AppointmentType = Convert.ToString(dr["AppointmentType"]),
        //                Gender = Convert.ToString(dr["Gender"]),
        //                Age = Convert.ToInt32(dr["Age"]),
        //                Problem = Convert.ToString(dr["Problem"]),
        //                Address = Convert.ToString(dr["Address"]),
        //                Date = Convert.ToString(dr["Date"]),
        //                Slot = Convert.ToString(dr["Slot"]),
        //                EntryDateTime = Convert.ToString(dr["EntryDateTime"]),

        //                EntryBy = Convert.ToString(dr["EntryBy"])
        //            });
        //    }

        //    foreach (DataRow dr1 in dt1.Rows)
        //    {
        //        AppointmentList.Add(
        //            new Appointment
        //            {
        //                AppointmentId = Convert.ToInt32(dr1["AppointmentId"]),
        //                Name = Convert.ToString(dr1["Name"]),
        //                Phone = Convert.ToString(dr1["Phone"]),
        //                HospitalId = Convert.ToInt32(dr1["HospitalId"]),
        //                AppointmentType = Convert.ToString(dr1["AppointmentType"]),
        //                Gender = Convert.ToString(dr1["Gender"]),
        //                Age = Convert.ToInt32(dr1["Age"]),
        //                Problem = Convert.ToString(dr1["Problem"]),
        //                Address = Convert.ToString(dr1["Address"]),
        //                Date = Convert.ToString(dr1["Date"]),
        //                Slot = Convert.ToString(dr1["Slot"]),
        //                EntryDateTime = Convert.ToString(dr1["EntryDateTime"]),

        //                EntryBy = Convert.ToString(dr1["EntryBy"])
        //            });
        //    }

        //    foreach (DataRow dr2 in dt2.Rows)
        //    {
        //        AppointmentList.Add(
        //            new Appointment
        //            {
        //                AppointmentId = Convert.ToInt32(dr2["AppointmentId"]),
        //                Name = Convert.ToString(dr2["Name"]),
        //                Phone = Convert.ToString(dr2["Phone"]),
        //                HospitalId = Convert.ToInt32(dr2["HospitalId"]),
        //                AppointmentType = Convert.ToString(dr2["AppointmentType"]),
        //                Gender = Convert.ToString(dr2["Gender"]),
        //                Age = Convert.ToInt32(dr2["Age"]),
        //                Problem = Convert.ToString(dr2["Problem"]),
        //                Address = Convert.ToString(dr2["Address"]),
        //                Date = Convert.ToString(dr2["Date"]),
        //                Slot = Convert.ToString(dr2["Slot"]),
        //                EntryDateTime = Convert.ToString(dr2["EntryDateTime"]),

        //                EntryBy = Convert.ToString(dr2["EntryBy"])
        //            });
        //    }
        //    return AppointmentList;
        //}





        public ActionResult ThreeTableListt(string psearch)
        {

            //SqlConnection con = new SqlConnection("server=SAITEJA\\SQLEXPRESS01; Integrated Security=True; database=paidi");
            SqlConnection con = new SqlConnection(cs);
            if (con.State == System.Data.ConnectionState.Closed)                
            con.Open();
            List<Appointment> AppointmentList = new List<Appointment>();
            //SqlConnection con = new SqlConnection(cs);
            SqlCommand cmd = new SqlCommand("select * from AppointmentMaster where AppointmentType='Online' and  HospitalId = @HospitalId and  Name like '%" + psearch + "%'", con);
         
          

            cmd.Parameters.AddWithValue("@HospitalId", (int)Session["HospitalId"]);
            SqlCommand cmd1 = new SqlCommand("select * from AppointmentMaster where AppointmentType='Walkin' and HospitalId = @HospitalId and Name like '%" + psearch + "%'", con);
       
            cmd1.Parameters.AddWithValue("@HospitalId", (int)Session["HospitalId"]);
            SqlCommand cmd2 = new SqlCommand("select * from AppointmentMaster where AppointmentType='Telephonic'and HospitalId = @HospitalId and Name like '%" + psearch + "%'", con);
        
            cmd2.Parameters.AddWithValue("@HospitalId", (int)Session["HospitalId"]);
            //cmd.Parameters.AddWithValue("@Email", email);

            SqlDataAdapter sd = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();

            sd.Fill(dt);
            SqlDataAdapter sd1 = new SqlDataAdapter(cmd1);
            DataTable dt1 = new DataTable();

            sd1.Fill(dt1);
            SqlDataAdapter sd2 = new SqlDataAdapter(cmd2);
            DataTable dt2 = new DataTable();

            sd2.Fill(dt2);
            con.Close();
            foreach (DataRow dr in dt.Rows)
            {
                AppointmentList.Add(
                    new Appointment
                    {
                        AppointmentId = Convert.ToInt32(dr["AppointmentId"]),
                        Name = Convert.ToString(dr["Name"]),
                        Age = Convert.ToInt32(dr["Age"]),
                        Gender = Convert.ToString(dr["Gender"]),
                        AppointmentType = Convert.ToString(dr["AppointmentType"]),
                        Doctor = Convert.ToString(dr["Doctor"]),
                        Phone = Convert.ToString(dr["Phone"]),
                        Payment = Convert.ToString(dr["Payment"]),
                        Problem = Convert.ToString(dr["Problem"]),
                        Slot = Convert.ToString(dr["Slot"]),
                        HospitalId = Convert.ToInt32(dr["HospitalId"])

                    });
            }

            foreach (DataRow dr1 in dt1.Rows)
            {
                AppointmentList.Add(
                    new Appointment
                    {
                        AppointmentId = Convert.ToInt32(dr1["AppointmentId"]),
                        Name = Convert.ToString(dr1["Name"]),
                        Age = Convert.ToInt32(dr1["Age"]),
                        Gender = Convert.ToString(dr1["Gender"]),
                        AppointmentType = Convert.ToString(dr1["AppointmentType"]),
                        Doctor = Convert.ToString(dr1["Doctor"]),
                        Phone = Convert.ToString(dr1["Phone"]),
                        Payment = Convert.ToString(dr1["Payment"]),
                        Problem = Convert.ToString(dr1["Problem"]),
                        Slot = Convert.ToString(dr1["Slot"]),
                        HospitalId = Convert.ToInt32(dr1["HospitalId"])

                    });
            }

            foreach (DataRow dr2 in dt2.Rows)
            {
                AppointmentList.Add(
                    new Appointment
                    {
                        AppointmentId = Convert.ToInt32(dr2["AppointmentId"]),
                        Name = Convert.ToString(dr2["Name"]),
                        Age = Convert.ToInt32(dr2["Age"]),
                        Gender = Convert.ToString(dr2["Gender"]),
                        AppointmentType = Convert.ToString(dr2["AppointmentType"]),
                        Doctor = Convert.ToString(dr2["Doctor"]),
                        Phone = Convert.ToString(dr2["Phone"]),
                        Payment = Convert.ToString(dr2["Payment"]),
                        Problem = Convert.ToString(dr2["Problem"]),
                        Slot = Convert.ToString(dr2["Slot"]),
                        HospitalId = Convert.ToInt32(dr2["HospitalId"])
                    });
            }
            return View(AppointmentList);
        }



        public ActionResult ThreeTableList(string psearch)
        {
          Id = Session["HospitalId"].ToString();



            SqlConnection con = new SqlConnection(cs);
            if (con.State == System.Data.ConnectionState.Closed)
                con.Open();
            List<Appointment> AppointmentList = new List<Appointment>();
            //SqlConnection con = new SqlConnection(cs);
            SqlCommand cmd = new SqlCommand("select * from AppointmentMaster where AppointmentType='Online' and HospitalId=@HospitalId  and Name like '%" + psearch + "%'", con);
            cmd.Parameters.AddWithValue("@HospitalId", Id);
            SqlCommand cmd1 = new SqlCommand("select * from AppointmentMaster where AppointmentType='Walkin' and HospitalId=@HospitalId and Name like '%" + psearch + "%'", con);
            cmd1.Parameters.AddWithValue("@HospitalId",Id);

            SqlCommand cmd2 = new SqlCommand("select * from AppointmentMaster where AppointmentType='Telephonic'and HospitalId=@HospitalId and Name like '%" + psearch + "%'", con);
            cmd2.Parameters.AddWithValue("@HospitalId", Id);

            //cmd.Parameters.AddWithValue("@Email", email);

            SqlDataAdapter sd = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();

            sd.Fill(dt);
            SqlDataAdapter sd1 = new SqlDataAdapter(cmd1);
            DataTable dt1 = new DataTable();

            sd1.Fill(dt1);
            SqlDataAdapter sd2 = new SqlDataAdapter(cmd2);
            DataTable dt2 = new DataTable();

            sd2.Fill(dt2);
            con.Close();
            foreach (DataRow dr in dt.Rows)
            {
                AppointmentList.Add(
                    new Appointment
                    {
                        AppointmentId = Convert.ToInt32(dr["AppointmentId"]),
                        Name = Convert.ToString(dr["Name"]),
                        Age = Convert.ToInt32(dr["Age"]),
                        Gender = Convert.ToString(dr["Gender"]),
                        AppointmentType = Convert.ToString(dr["AppointmentType"]),
                        Doctor = Convert.ToString(dr["Doctor"]),
                        Phone = Convert.ToString(dr["Phone"]),
                        Payment = Convert.ToString(dr["Payment"]),
                        Problem = Convert.ToString(dr["Problem"]),
                        Slot = Convert.ToString(dr["Slot"]),
                        HospitalId = Convert.ToInt32(dr["HospitalId"])

                    });
            }

            foreach (DataRow dr1 in dt1.Rows)
            {
                AppointmentList.Add(
                    new Appointment
                    {
                        AppointmentId = Convert.ToInt32(dr1["AppointmentId"]),
                        Name = Convert.ToString(dr1["Name"]),
                        Age = Convert.ToInt32(dr1["Age"]),
                        Gender = Convert.ToString(dr1["Gender"]),
                        AppointmentType = Convert.ToString(dr1["AppointmentType"]),
                        Doctor = Convert.ToString(dr1["Doctor"]),
                        Phone = Convert.ToString(dr1["Phone"]),
                        Payment = Convert.ToString(dr1["Payment"]),
                        Problem = Convert.ToString(dr1["Problem"]),
                        Slot = Convert.ToString(dr1["Slot"]),
                        HospitalId = Convert.ToInt32(dr1["HospitalId"])

                    });
            }

            foreach (DataRow dr2 in dt2.Rows)
            {
                AppointmentList.Add(
                    new Appointment
                    {
                        AppointmentId = Convert.ToInt32(dr2["AppointmentId"]),
                        Name = Convert.ToString(dr2["Name"]),
                        Age = Convert.ToInt32(dr2["Age"]),
                        Gender = Convert.ToString(dr2["Gender"]),
                        AppointmentType = Convert.ToString(dr2["AppointmentType"]),
                        Doctor = Convert.ToString(dr2["Doctor"]),
                        Phone = Convert.ToString(dr2["Phone"]),
                        Payment = Convert.ToString(dr2["Payment"]),
                        Problem = Convert.ToString(dr2["Problem"]),
                        Slot = Convert.ToString(dr2["Slot"]),
                        HospitalId = Convert.ToInt32(dr2["HospitalId"])
                    });
            }
            return View(AppointmentList);
        }
    }
}