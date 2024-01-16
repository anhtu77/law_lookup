using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Demo.Models;

namespace Demo.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult DanhSachChuong()
        {
            
            using (TestEntities1 db = new TestEntities1())
            {
                List<Chuong> danhSachChuong = db.Chuongs.ToList();
                ViewBag.DanhSachChuong = danhSachChuong;

                var danhSachDieu = db.Dieux.ToList();
                ViewBag.DanhSachDieu = danhSachDieu;

                return View();
            }

        }

     

        public ActionResult ChiTietDieu(int dieuId)
        {
            using (TestEntities1 db = new TestEntities1())
            {
                Dieu dieu = db.Dieux.FirstOrDefault(d => d.id == dieuId);
                if (dieu == null)
                {
                    return HttpNotFound();
                }

                ViewBag.Dieu = dieu;

                List<Khoan> danhSachKhoan = db.Khoans.Where(k => k.idDieu == dieuId).ToList();
                ViewBag.DanhSachKhoan = danhSachKhoan;

                return View();
            }
        }

     
        public ActionResult ChiTietKhoan(int khoanId)
        {
            using (TestEntities1 db = new TestEntities1())
            {
                var khoan = db.Khoans.FirstOrDefault(k => k.id == khoanId);
                if (khoan == null)
                {
                    return HttpNotFound();
                }

                var dieu = db.Dieux.FirstOrDefault(d => d.id == khoan.idDieu);
                if (dieu == null)
                {
                    return HttpNotFound();
                }

                var chuong = db.Chuongs.FirstOrDefault(c => c.id == dieu.idChuong);
                if (chuong == null)
                {
                    return HttpNotFound();
                }

                ViewBag.Khoan = khoan;
                ViewBag.Dieu = dieu;
                ViewBag.Chuong = chuong;

                return View();
            }
        }

        public ActionResult TimKiem(string filter)
        {
            return RedirectToAction("KetQuaTimKiem", new { filter });
        }

        [HttpGet]
        public ActionResult KetQuaTimKiem(string filter)
        {
            using (TestEntities1 db = new TestEntities1())
            {
                var danhSachKhoan = db.Khoans.Where(k => k.ma.ToLower().Contains(filter.ToLower()) ||
                                                        k.noiDung.ToLower().Contains(filter.ToLower()))
                                             .ToList();

                ViewBag.Filter = filter;
                return View(danhSachKhoan);
            }
        }

    }
}