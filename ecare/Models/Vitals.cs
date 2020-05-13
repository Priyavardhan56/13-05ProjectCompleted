using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ecare.Models
{
    public class Vitals
    {
        public string PatientEmail { get; set; }
        public string DoctorID { get; set; }
        public string BloodPressure { get; set; }
        public string Temperature { get; set; }
        public string Pulse { get; set; }
        public string Symptoms { get; set; }
        public string Diagnosis { get; set; }
        public string Remarks { get; set; }
        public string MedicineName { get; set; }
        public string Dosage { get; set; }
        public string Timings { get; set; }
        public string EntryDateTime { get; set; }
        public string EntryBy { get; set; }
        public int Id { get; set; }
    }

    //public class Medicine
    //{
    //    public int Id { get; set; }
    //    public string PatientName { get; set; }
    //    public int HospitalId { get; set; }
    //    public string DoctorName { get; set; }
    //    public string MedicineName { get; set; }
    //    public string Dosage { get; set; }
    //    public string Timings { get; set; }
    //    public string EntryDate { get; set; }
    //    public string EntryBy { get; set; }
    //}

    //public class Opd
    //{
    //    public int Id { get; set; }
    //    public string PatientName { get; set; }
    //    public int HospitalId { get; set; }
    //    public string DoctorName { get; set; }
    //    public string Symptoms { get; set; }
    //    public string Diagnosis { get; set; }
    //    public string Remarks { get; set; }
    //    public string RefferedTo { get; set; }
    //    public string EntryDate { get; set; }
    //    public string EntryBy { get; set; }
    //}
}