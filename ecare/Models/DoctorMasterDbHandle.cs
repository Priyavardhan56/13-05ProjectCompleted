using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using ecare.Models;

namespace ecare.Models
{
    public class DoctorMasterDbHandle
    {
 private SqlConnection con;

        private void connection()
        {
            string constring = ConfigurationManager.ConnectionStrings["dbECare"].ToString();
            con = new SqlConnection(constring);
        }

        // **************** ADD NEW Doctor *********************
        public bool AddDoctor(Doctor smodel)
        {
            connection();
            SqlCommand cmd = new SqlCommand("pInsertDoctorMaster", con);
            cmd.CommandType = CommandType.StoredProcedure;

           
            cmd.Parameters.AddWithValue("@EmployeeCode", smodel.EmployeeCode);
            cmd.Parameters.AddWithValue("@DoctorName", smodel.DoctorName);
            cmd.Parameters.AddWithValue("@HospitalId", smodel.HospitalId);
            cmd.Parameters.AddWithValue("@DoctorSpecialization", smodel.DoctorSpecialization);
            cmd.Parameters.AddWithValue("@DoctorDegree", smodel.DoctorDegree);
            cmd.Parameters.AddWithValue("@DoctorPhone", smodel.DoctorPhone);
            cmd.Parameters.AddWithValue("@DoctorEmail", smodel.DoctorEmail);
            cmd.Parameters.AddWithValue("@DoctorCity", smodel.DoctorCity);
            cmd.Parameters.AddWithValue("@DoctorState", smodel.DoctorState);
            cmd.Parameters.AddWithValue("@DoctorCountry", smodel.DoctorCountry);
            cmd.Parameters.AddWithValue("@IsActive", smodel.IsActive);
            cmd.Parameters.AddWithValue("@EntryDate", DateTime.Now);
            cmd.Parameters.AddWithValue("@EntryBy", smodel.EntryBy);
            cmd.Parameters.AddWithValue("@RoleId  ", smodel.RoleId);
            cmd.Parameters.AddWithValue("@DoctorAddress", smodel.DoctorAddress);
            con.Open();
            int i = cmd.ExecuteNonQuery();
            con.Close();

            if (i >= 1)
                return true;
            else
                return false;
        }

        // ********** VIEW Doctor DETAILS ********************
        public List<Doctor> GetDoctor()
        {
            connection();
            List<Doctor> Doctorlist = new List<Doctor>();

            SqlCommand cmd = new SqlCommand("pDoctorMasterDetails", con);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter sd = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();

            con.Open();
            sd.Fill(dt);
            con.Close();

            foreach (DataRow dr in dt.Rows)
            {
                Doctorlist.Add(
                    new Doctor
                    {
                        DoctorId = Convert.ToInt32(dr["DoctorId"]),
                        EmployeeCode = Convert.ToInt32(dr["EmployeeCode"]),
                        DoctorName = Convert.ToString(dr["DoctorName"]),
                        HospitalId = Convert.ToInt32(dr["HospitalId"]),
                        DoctorSpecialization = Convert.ToString(dr["DoctorSpecialization"]),
                        DoctorDegree = Convert.ToString(dr["DoctorDegree"]),
                        DoctorPhone = Convert.ToString(dr["DoctorPhone"]),
                        DoctorEmail = Convert.ToString(dr["DoctorEmail"]),
                        DoctorCity = Convert.ToString(dr["DoctorCity"]),
                        DoctorState = Convert.ToString(dr["DoctorState"]),
                        DoctorCountry = Convert.ToString(dr["DoctorCountry"]),
                        IsActive = Convert.ToInt32(dr["IsActive"]),
                        EntryDate = Convert.ToString(dr["EntryDate"]),
                        EntryBy = Convert.ToString(dr["EntryBy"]),

                        RoleId = Convert.ToInt32(dr["RoleId"]),

                        DoctorAddress =Convert.ToString(dr["DoctorAddress"])



                    });
            }
            return Doctorlist;
        }

        // ***************** UPDATE Doctor DETAILS *********************
        public bool UpdateDetails(Doctor smodel)
        {
            connection();
            SqlCommand cmd = new SqlCommand("pUPdateDoctorMastor", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@DoctorId", smodel.DoctorId);
            cmd.Parameters.AddWithValue("@EmployeeCode", smodel.EmployeeCode);
            cmd.Parameters.AddWithValue("@DoctorName", smodel.DoctorName);
            cmd.Parameters.AddWithValue("@HospitalId", smodel.HospitalId);
            cmd.Parameters.AddWithValue("@DoctorSpecialization", smodel.DoctorSpecialization);
            cmd.Parameters.AddWithValue("@DoctorDegree", smodel.DoctorDegree);
            cmd.Parameters.AddWithValue("@DoctorPhone", smodel.DoctorPhone);
            cmd.Parameters.AddWithValue("@DoctorEmail", smodel.DoctorEmail);
            cmd.Parameters.AddWithValue("@DoctorCity", smodel.DoctorCity);
            cmd.Parameters.AddWithValue("@DoctorState", smodel.DoctorState);
            cmd.Parameters.AddWithValue("@DoctorCountry", smodel.DoctorCountry);
            cmd.Parameters.AddWithValue("@IsActive", smodel.IsActive);
            cmd.Parameters.AddWithValue("@EntryDate", Convert.ToDateTime(smodel.EntryDate));
            cmd.Parameters.AddWithValue("@EntryBy", smodel.EntryBy);
            cmd.Parameters.AddWithValue("@RoleId  ", smodel.RoleId);
            cmd.Parameters.AddWithValue("@DoctorAddress", smodel.DoctorAddress);


            con.Open();
            int i = cmd.ExecuteNonQuery();
            con.Close();

            if (i >= 1)
                return true;
            else
                return false;
        }

        // ********************** DELETE Doctor DETAILS *******************
        public bool DeleteDoctor(int id)
        {
            connection();
            SqlCommand cmd = new SqlCommand("pDeleteDoctorMaster", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@DoctorId", id);

            con.Open();
            int i = cmd.ExecuteNonQuery();
            con.Close();

            if (i >= 1)
                return true;
            else
                return false;
        }
    }
}
