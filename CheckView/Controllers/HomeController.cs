using CheckView.Models;
using Newtonsoft.Json.Linq;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace CheckView.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            EPORTAL_UNISEntities dbEP = new EPORTAL_UNISEntities();
            List<ID_Terminal> dt = dbEP.ID_Terminal.ToList();
            ViewBag.IDTerminal = new SelectList(dt, "IDTerminal", "TenCong");

            return View();
        }

        public JsonResult GetNV(int? IDTerminal)
        {
            string connStr = ConfigurationManager.ConnectionStrings["OracleDbContext"].ToString();

            string begindcover = "";
            string enddcover = "";
            string begindcoverTime = "";
            string enddcoverTime = "";
            if (IDTerminal == null) IDTerminal = 0;

            begindcover = DateTime.Now.Date.ToString("yyyyMMdd");
            enddcover = DateTime.Now.Date.ToString("yyyyMMdd");

            DateTime dt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.AddMinutes(-1).Minute, 0);
            enddcoverTime = DateTime.Now.ToString("HHmmss");
            begindcoverTime = dt.ToString("HHmmss");
            //enddcoverTime = "235959";
            //begindcoverTime = "230000";
            string sql = "SELECT * FROM  (SELECT te.C_Date,te.C_Time,te.L_TID,te.L_UID,te.C_Name,te.C_Unique,te.C_Post,te.C_Card, tt.C_Name as C_Name1,tt.L_ID as T_Terminal, tu.C_REGDATE , tu.L_OPTDATELIMIT, tu.C_DATELIMIT" +
                       " FROM tEnter te, TTERMINAL tt, TUSER tu " +
                       " where te.L_TID=tt.L_ID AND tt.L_ID = "+ IDTerminal+" AND tu.l_ID=te.L_UID AND te.C_Date >= '" + begindcover + "' AND te.C_Date <= '" + begindcover + "' AND te.C_Time >='" + begindcoverTime + "' AND te.C_Time <='" + enddcoverTime + "' ORDER BY te.C_Time DESC ) WHERE ROWNUM < 2";
            OracleConnection con = new OracleConnection(connStr);
            OracleCommand cmd = new OracleCommand();
            cmd.CommandText = sql;
            cmd.Connection = con;
            con.Open();
            OracleDataReader dr = cmd.ExecuteReader();
            List<NhanVienView> listEntry = new List<NhanVienView>();
            if (dr.HasRows)
            {
                NhanVienView NhanvienNT = null;
                while (dr.Read())
                {
                    NhanvienNT = new NhanVienView();
                    NhanvienNT.Mathe = dr["C_Card"].ToString();
                    NhanvienNT.Hoten = dr["C_Name"].ToString();
                    NhanvienNT.MaNT = dr["C_Post"].ToString();
                    NhanvienNT.NgayQuet = dr["C_Date"].ToString();
                    NhanvienNT.GioQuet = dr["C_Time"].ToString();
                    NhanvienNT.Thoigianquyet = dr["C_Date"].ToString() + dr["C_Time"].ToString();
                    NhanvienNT.Tenmayquyet = dr["C_Name1"].ToString();
                    NhanvienNT.IDTerminal = dr["T_Terminal"].ToString();
                    NhanvienNT.NgayGio = DateTime.ParseExact(NhanvienNT.Thoigianquyet, "yyyyMMddHHmmss", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy HH:mm:ss");
                    listEntry.Add(NhanvienNT);
                }
            }
            else
            {
                Console.WriteLine("No Data In DataBase");
            }
            con.Close();

            return Json(listEntry.FirstOrDefault(), JsonRequestBehavior.AllowGet);
        }


        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}