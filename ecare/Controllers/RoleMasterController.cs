using ecare.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace ecare.Controllers
{
    public class RoleMasterController : Controller
    {


        private static readonly string cs = WebConfigurationManager.ConnectionStrings["dbECare"].ConnectionString;

        // GET: RoleMaster
        public ActionResult Index()
        {
            return View(GetRole());
        }


        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Role objRole)
        {
            if (ModelState.IsValid)
            {
                SqlConnection con = new SqlConnection(cs);
                try
                {
                    SqlCommand cmd = new SqlCommand("pInsertRoleMaster", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@RoleName", objRole.RoleName);
                    cmd.Parameters.AddWithValue("@RoleDescription", objRole.RoleDescription);
                    cmd.Parameters.AddWithValue("@EntryBy", objRole.EntryBy);
                    cmd.Parameters.AddWithValue("@EntryDateTime", DateTime.Now);

                    con.Open();
                    int i = cmd.ExecuteNonQuery();
                    con.Close();
                    if (i >= 1)
                        ViewBag.SuccessMessage = "success";
                    else
                        ViewBag.ErrorMessage = "Failed";
                    ModelState.Clear();
                    return RedirectToAction("Index", "RoleMaster");
                }
                catch
                {
                    ViewBag.ErrorMessage = "Failed";
                }

                ModelState.Clear();
            }
                return View();
            }


        public ActionResult Edit(int id)
        {
            return View(GetRole().Find(model=>model.RoleId==id));
        }

        // POST: RoleMaster/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, Role objRole)
        {
            SqlConnection con = new SqlConnection(cs);
            try
            {
                SqlCommand cmd = new SqlCommand("pUpdateRoleMaster", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@RoleId", objRole.RoleId);
                cmd.Parameters.AddWithValue("@RoleName", objRole.RoleName);
                cmd.Parameters.AddWithValue("@RoleDescription", objRole.RoleDescription);
                cmd.Parameters.AddWithValue("@EntryBy", objRole.EntryBy);
                cmd.Parameters.AddWithValue("@EntryDateTime", Convert.ToDateTime(objRole.EntryDateTime));

                con.Open();
                int i = cmd.ExecuteNonQuery();
                con.Close();

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
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
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        public List<Role> GetRole()
        {
            SqlConnection con = new SqlConnection(cs);

            List<Role> Rolelist = new List<Role>();

            SqlCommand cmd = new SqlCommand("pDetailsRoleMaster", con);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter sd = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();

            con.Open();
            sd.Fill(dt);
            con.Close();

            foreach (DataRow dr in dt.Rows)
            {
                Rolelist.Add(
                    new Role
                    {
                        RoleId = Convert.ToInt32(dr["RoleId"]),
                        RoleName = Convert.ToString(dr["RoleName"]),
                        RoleDescription = Convert.ToString(dr["RoleDescription"]),
                        EntryBy = Convert.ToString(dr["EntryBy"]),
                        EntryDateTime = Convert.ToString(dr["EntryDateTime"])
                    });
            }
            return Rolelist;
        }
        public bool DeleteRole(int id)
        {
            SqlConnection con = new SqlConnection(cs);
            SqlCommand cmd = new SqlCommand("pDeleteRoleMaster", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@RoleId", id);

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
