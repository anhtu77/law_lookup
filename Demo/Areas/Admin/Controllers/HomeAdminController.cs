using Demo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Demo.Areas.Admin.Controllers
{
    public class HomeAdminController : Controller
    {
        
      public ActionResult DanhSachChuongDieuAdmin()
        {
            using (TestEntities1 db = new TestEntities1())
            {
                List<Chuong> danhSachChuong = db.Chuongs.ToList();
                ViewBag.DanhSachChuong = danhSachChuong;

                var danhSachDieu = db.Dieux.ToList();
                ViewBag.DanhSachDieu = danhSachDieu;

                if (Session["user"] == null)
                {
                    return RedirectToAction("DangNhap");
                }
                else
                {
                    return View();

                }
            }
        }
        public ActionResult ChiTietKhoanAdmin(int khoanId)
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

                if (Session["user"] == null)
                {
                    return RedirectToAction("DangNhap");
                }
                else
                {
                    return View();

                }
            }
        }
        public ActionResult ChiTietDieuAdmin(int dieuId)
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

                if (Session["user"] == null)
                {
                    return RedirectToAction("DangNhap");
                }
                else
                {
                    return View();

                }
            }
        }

        public ActionResult TimKiemAdmin(string filter)
        {
                return RedirectToAction("KetQuaTimKiemAdmin", new { filter });        
        }

        [HttpGet]
        public ActionResult KetQuaTimKiemAdmin(string filter)
        {
            using (TestEntities1 db = new TestEntities1())
            {
                var danhSachKhoan = db.Khoans.Where(k => k.ma.ToLower().Contains(filter.ToLower()) ||
                                                        k.noiDung.ToLower().Contains(filter.ToLower()))
                                             .ToList();

                ViewBag.Filter = filter;
                if (Session["user"] == null)
                {
                    return RedirectToAction("DangNhap");
                }
                else
                {
                    return View(danhSachKhoan);

                }
               
            }
        }
        public ActionResult DangNhap()
      {
            return View();
      }
        
      [HttpPost]
        public ActionResult DangNhap(string user, string password)
        {
            // check db
            TestEntities1 db = new TestEntities1();
            var admin = db.accounts.SingleOrDefault(m => m.username.ToLower() == user.ToLower() && m.password == password);

            if (admin != null)
            {
                Session["user"] = admin;
                return RedirectToAction("DanhSachChuongDieuAdmin");
            }
            else
            {
                TempData["error"] = "Tài khoản đăng nhập không đúng";
                return View();
            }


        }
        public ActionResult DangXuat()
        {
            // xóa sesstion
            Session.Remove("user");
            // xóa session form authent
            FormsAuthentication.SignOut();
            return RedirectToAction("DanhSachChuong","Home", new {area = "",controllerNamespace = "Demo.Controllers" });
        }

        public ActionResult ThemMoiChuong()
        {

            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap");

            }
            else
            {
                return View();
            }

           
        }
        [HttpPost]
        public ActionResult ThemMoiChuong(Chuong model)
        {
            TestEntities1 db = new TestEntities1();

            // Kiểm tra xem chương đã tồn tại hay chưa
            bool check = db.Chuongs.Any(c => c.ma == model.ma);

            if (check)
            {
                // Hiển thị thông báo lỗi cho người dùng
                ModelState.AddModelError("", "Chương đã tồn tại.");

                // Trả về view với model để hiển thị thông báo lỗi
                return View(model);
            }
            if (string.IsNullOrEmpty(model.ma) || string.IsNullOrEmpty(model.noiDung))
            {
                ModelState.AddModelError("", "Bạn phải điền đẩy đủ thông tin ");
                return View(model);
            }
            // Chương chưa tồn tại, thực hiện thêm mới
            db.Chuongs.Add(model);
            db.SaveChanges();

            return RedirectToAction("DanhSachChuongDieuAdmin");
        }

        public ActionResult ThemMoiDieu()
        {

            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap");

            }
            else
            {
                return View();
            }


        }
        [HttpPost]
        public ActionResult ThemMoiDieu(Dieu model)
        {
            using (TestEntities1 db = new TestEntities1())
            {

                bool check = db.Dieux.Any(d => d.ma == model.ma);
                if (check)
                {
                    // Điều đã tồn tại, hiển thị thông báo lỗi
                    ModelState.AddModelError("", "Điều này đã tồn tại.");
                    return View(model);
                }
                if (string.IsNullOrEmpty(model.ma) || string.IsNullOrEmpty(model.noiDung))
                {
                    ModelState.AddModelError("", "Bạn phải điền đẩy đủ thông tin ");
                    return View(model);
                }
                // Thêm mới điều vào chương
                db.Dieux.Add(model);
                db.SaveChanges();

                return RedirectToAction("DanhSachChuongDieuAdmin");
            }
        }

        public ActionResult ThemMoiKhoan()
        {

            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap");

            }
            else
            {
                return View();
            }


        }
        [HttpPost]
        public ActionResult ThemMoiKhoan(Khoan model)
        {
            using (TestEntities1 db = new TestEntities1())
            {

                // Kiểm tra xem khoản đã tồn tại trong điều hay chưa
                bool check = db.Khoans.Any(d => d.idDieu == model.idDieu && d.ma == model.ma);
                if (check)
                {
                    
                    ModelState.AddModelError("", "Khoản này đã tồn tại trong điều.");
                    return View(model);
                }
                if (string.IsNullOrEmpty(model.ma) || string.IsNullOrEmpty(model.noiDung))
                {
                    ModelState.AddModelError("", "Bạn phải điền đầy đủ tên khoản và nội dung ");
                    return View(model);
                }
                // Kiểm tra mức xử phạt tối thiểu và tối đa
                if (model.MXPTT >= model.MXPTD)
                {
                    ModelState.AddModelError("", "Mức xử phạt tối thiểu phải nhỏ hơn mức xử phạt tối đa.");
                    return View(model);
                }
                if (model.MXPTT.HasValue && (model.MXPTT <= 0))
                {
                    ModelState.AddModelError("", "Mức xử phạt tối thiểu phải là số dương hoặc null.");
                    return View(model);
                }

                if (model.MXPTD.HasValue && (model.MXPTD <= 0))
                {
                    ModelState.AddModelError("", "Mức xử phạt tối đa phải là số dương hoặc null.");
                    return View(model);
                }
                // Thêm mới khoản mới vào điều
                db.Khoans.Add(model);

                db.SaveChanges();

                return RedirectToAction("DanhSachChuongDieuAdmin");
            }
        }

        public ActionResult CapNhatChuong(int ID)
        {
            TestEntities1 db = new TestEntities1();
            Chuong model2 = db.Chuongs.Find(ID);

            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap");

            }
            else
            {
                return View(model2);
            }
        }
        [HttpPost]
        public ActionResult CapNhatChuong(Chuong model)
        {
            TestEntities1 db = new TestEntities1();
            if (string.IsNullOrEmpty(model.ma) || string.IsNullOrEmpty(model.noiDung))
            {
                ModelState.AddModelError("", "Bạn phải điền đẩy đủ thông tin ");
                return View(model);
            }
            bool check = db.Chuongs.Any(c => c.id != model.id && c.ma == model.ma);
            if (check)
            {
                // Chương đã tồn tại, hiển thị thông báo lỗi
                ModelState.AddModelError("", "Chương này đã tồn tại.");
                return View(model);
            }
            // tìm đối tượng
            var updateModel = db.Chuongs.Find(model.id);
            // gán giá trị
            updateModel.ma = model.ma;
            updateModel.noiDung = model.noiDung;
       
            // lưu thay đổi
            db.SaveChanges();
            return RedirectToAction("DanhSachChuongDieuAdmin");
        }
        public ActionResult CapNhatDieu(int ID)
        {
            TestEntities1 db = new TestEntities1();
            Dieu model2 = db.Dieux.Find(ID);

            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap");

            }
            else
            {
                return View(model2);
            }
        }
        [HttpPost]
        public ActionResult CapNhatDieu(Dieu model)
        {
            TestEntities1 db = new TestEntities1();
            bool check = db.Dieux.Any(d => d.id != model.id && d.ma == model.ma);
            if (check)
            {
                // Điều đã tồn tại, hiển thị thông báo lỗi
                ModelState.AddModelError("", "Điều này đã tồn tại.");
                return View(model);
            }
            if (string.IsNullOrEmpty(model.ma) || string.IsNullOrEmpty(model.noiDung))
            {
                ModelState.AddModelError("", "Bạn phải điền đẩy đủ thông tin ");
                return View(model);
            }
            // tìm đối tượng
            var updateModel = db.Dieux.Find(model.id);
            // gán giá trị
            updateModel.idChuong = model.idChuong;
            updateModel.ma = model.ma;
            updateModel.noiDung = model.noiDung;
            // lưu thay đổi
            db.SaveChanges();
            return RedirectToAction("DanhSachChuongDieuAdmin");
        }
        public ActionResult CapNhatKhoan(int ID)
        {
            TestEntities1 db = new TestEntities1();
            Khoan model2 = db.Khoans.Find(ID);

            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap");

            }
            else
            {
                return View(model2);
            }
        }
        [HttpPost]
        public ActionResult CapNhatKhoan(Khoan model)
        {
            TestEntities1 db = new TestEntities1();
            if (string.IsNullOrEmpty(model.ma) || string.IsNullOrEmpty(model.noiDung))
            {
                ModelState.AddModelError("", "Bạn phải điền đẩy đủ thông tin ");
                return View(model);
            }

            // Kiểm tra mức xử phạt tối thiểu và tối đa
            if (model.MXPTT >= model.MXPTD)
            {
                ModelState.AddModelError("", "Mức xử phạt tối thiểu phải nhỏ hơn mức xử phạt tối đa.");
                return View(model);
            }
            bool check = db.Khoans.Any(k => k.idDieu == model.idDieu && k.ma == model.ma && k.id != model.id);
            if (check)
            {
                // Khoản đã tồn tại trong điều, hiển thị thông báo lỗi
                ModelState.AddModelError("", "Khoản này đã tồn tại trong điều.");
                return View(model);
            }
            if (model.MXPTT.HasValue && (model.MXPTT <= 0))
            {
                ModelState.AddModelError("", "Mức xử phạt tối thiểu phải là số dương hoặc null.");
                return View(model);
            }

            if (model.MXPTD.HasValue && (model.MXPTD <= 0))
            {
                ModelState.AddModelError("", "Mức xử phạt tối đa phải là số dương hoặc null.");
                return View(model);
            }

            // tìm đối tượng
            var updateModel = db.Khoans.Find(model.id);
            // gán giá trị
            updateModel.idDieu = model.idDieu;
            updateModel.ma = model.ma;
            updateModel.noiDung = model.noiDung;
            updateModel.MXPTT = model.MXPTT;
            updateModel.MXPTD = model.MXPTD;
            // lưu thay đổi
            db.SaveChanges();
            return RedirectToAction("ChiTietKhoanAdmin", new { khoanId = updateModel.id });
        }

        public ActionResult XoaChuong(int id)
        {
            using (TestEntities1 db = new TestEntities1())
            {
                // Tìm chương cần xóa trong cơ sở dữ liệu
                var chuong = db.Chuongs.Include("Dieux.Khoans").SingleOrDefault(c => c.id == id);

                if (chuong == null)
                {
                    // Chương không tồn tại, hiển thị thông báo lỗi hoặc xử lý theo ý muốn của bạn
                    return HttpNotFound();
                }

                // Xóa tất cả các khoản thuộc các điều trong chương
                foreach (var dieu in chuong.Dieux)
                {
                    db.Khoans.RemoveRange(dieu.Khoans);
                }

                // Xóa tất cả các điều thuộc chương
                db.Dieux.RemoveRange(chuong.Dieux);

                // Xóa chương
                db.Chuongs.Remove(chuong);
                db.SaveChanges();

                // Chuyển hướng về trang danh sách chương sau khi xóa thành công
                return RedirectToAction("DanhSachChuongDieuAdmin");
            }
        }
        public ActionResult XoaDieu(int id)
        {
            using (TestEntities1 db = new TestEntities1())
            {
                // Tìm điều cần xóa trong cơ sở dữ liệu
                var dieu = db.Dieux.Include("Khoans").SingleOrDefault(d => d.id == id);

                if (dieu == null)
                {
                    // Điều không tồn tại, hiển thị thông báo lỗi hoặc xử lý theo ý muốn của bạn
                    return HttpNotFound();
                }

                // Xóa tất cả các khoản thuộc điều
                db.Khoans.RemoveRange(dieu.Khoans);

                // Xóa điều
                db.Dieux.Remove(dieu);
                db.SaveChanges();

                // Chuyển hướng về trang danh sách điều sau khi xóa thành công
                return RedirectToAction("DanhSachChuongDieuAdmin");
            }
        }
        public ActionResult XoaKhoan(int id)
        {
            using (TestEntities1 db = new TestEntities1())
            {
                // Tìm khoản cần xóa trong cơ sở dữ liệu
                var khoan = db.Khoans.Find(id);

                if (khoan == null)
                {
                    // Khoản không tồn tại, hiển thị thông báo lỗi hoặc xử lý theo ý muốn của bạn
                    return HttpNotFound();
                }

                // Xóa khoản
                db.Khoans.Remove(khoan);
                db.SaveChanges();

                // Chuyển hướng về trang danh sách chương hoặc điều sau khi xóa thành công
                return RedirectToAction("DanhSachChuongDieuAdmin");
            }
        }


    }
}