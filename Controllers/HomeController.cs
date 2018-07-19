using MVC_HW3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;
using System.Data.Entity;
using System.Net;

namespace MVC_HW3.Controllers
{
    public class HomeController : Controller
    {
        TravelEntities db = new TravelEntities();
        // GET: Home
        public ActionResult Index()
        {

            return View();
        }
        [ChildActionOnly]
        public ActionResult Home_Bulletin()
        {
            return PartialView(db.tBulletin.OrderByDescending(a => a.fBu_Date).Take(5).ToList());
        }

        [HttpPost]
        public ActionResult trylog(string EmployeeID, string password, string remember)
        {
            var ep = db.tEmployee.Where(a => a.fEp_Code == EmployeeID && a.fEp_Password == password);
            if (ep.Any())
            {
                Response.Cookies["account"].Value = EmployeeID;
                if (ep.Single().fId_ID == 3)
                {
                    Response.Cookies["identity"].Value = "Manager";
                }
                if (remember == "yes")
                {
                    Response.Cookies["account"].Expires = DateTime.MaxValue;
                }
            }

            return new EmptyResult();
        }

        public ActionResult logout()
        {
            Response.Cookies["account"].Expires = DateTime.Now.AddDays(-1);
            Response.Cookies["Manergers"].Expires = DateTime.Now.AddDays(-1);
            return RedirectToAction("Index");
        }


        [HttpGet]
        public ActionResult Connection()
        {
            if (Request.Cookies["account"] == null)
            {
                return RedirectToAction("Index");
            }
            ViewBag.datas = db.tConnectionClass.Where(a => a.fCC_Class != "訊息").ToList();

            return View();
        }

        [HttpPost]
        public ActionResult Connection(tConnection Connect, HttpPostedFileBase Image)
        {
            if (Request.Cookies["account"] == null)
            {
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                if (Image != null && Image.ContentLength > 0)
                {
                    //將上傳的圖轉成二進位
                    var imgSize = Image.ContentLength;
                    byte[] imgByte = new byte[imgSize];
                    Image.InputStream.Read(imgByte, 0, imgSize);
                    Connect.fCo_Image = imgByte;
                }
                string epID = Request.Cookies["account"].Value;
                Connect.fEp_ID = db.tEmployee.Where(a => a.fEp_Code == epID).Single().fEp_ID;
                Connect.fCo_Date = DateTime.Now;
                db.tConnection.Add(Connect);
                db.SaveChanges();
                ViewBag.message = "訊息已送出,感謝您的聯絡!!";

            }

            ViewBag.datas = db.tConnectionClass.Where(a => a.fCC_Class != "訊息").ToList();
            return View();
        }
     

        [HttpGet]
        public ActionResult Bulletin(int? page, string searchString, int ID = 0)
        {

            if (ID == 0 && string.IsNullOrEmpty(searchString))
            {
                ViewBag.Class = "最新消息";
                return View(db.tBulletin.AsQueryable().OrderByDescending(a => a.fBu_Date).ToPagedList(page ?? 1, 10));
            }
            else if (!string.IsNullOrEmpty(searchString))
            {

                var BulletinVar = from b in db.tBulletin
                                  select b;
                ViewBag.Class = "搜尋包含「" + searchString + "」之字串";
                BulletinVar = db.tBulletin.Where(a => (a.fBu_Title.Contains(searchString)) || (a.fBu_Content.Contains(searchString))).AsQueryable().OrderByDescending(a => a.fBu_Date);
                //BulletinVar = db.tBulletin.Where(a => (a.fBu_Title.Contains(searchString))).AsQueryable().OrderByDescending(a => a.fBu_Date);
                ID = 0;
                return View(BulletinVar.ToPagedList(page ?? 1, 10));
            }
            else
            {
                ViewBag.Class = db.tBulletinClass.Where(a => a.fBC_ID == ID).Single().fBC_Class;
                return View(db.tBulletin.Where(a => a.fBC_ID == ID).AsQueryable().OrderByDescending(a => a.fBu_Date).ToPagedList(page ?? 1, 10));

            }

        }
        public ActionResult BulletinDetail(int ID)
        {
            return View(db.tBulletin.Find(ID));
        }

        public ActionResult GetImageFile(string fileName)
        {
            string Furl = Url.Content("~/Images/" + fileName + ".png");
            return File(Furl, "image/png");
        }

        //[ChildActionOnly]
        public ActionResult Marquee(int No)
        {
            if (No == 5)
            {
                No = 0;
            }

            return PartialView(db.tBulletin.OrderByDescending(a => a.fBu_Date).Skip(No).First());
        }
        public ActionResult ImagePreview(int? id)
        {
            tEmployee tEmployee = db.tEmployee.Find(id);
            byte[] img = tEmployee.fEp_Picture;
            return File(img, "image/jpeg");
        }
        public ActionResult tDependentEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tDependents tDependents = db.tDependents.Find(id);
            if (tDependents == null)
            {
                return HttpNotFound();
            }
            return View(tDependents);
        }

        // POST: tDependents/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult tDependentEdit([Bind(Include = "fDe_ID,fEp_ID,fDe_Name,fDe_Tel,fDe_SSNumber,fRe_ID,fDe_Birth")] tDependents tDependents)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tDependents).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tDependents);
        }
        [HttpGet]
        public ActionResult AccountEdit(string idstring)
        {
            if (Request.Cookies["account"] == null)
            {
                //id = 0;
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            idstring = Request.Cookies["account"].Value;
            int id = db.tEmployee.Where(p => p.fEp_Code == idstring).Single().fEp_ID;
            tEmployee tEmployee = db.tEmployee.Find(id);
            if (tEmployee == null)
            {
                return HttpNotFound();
            }
            ViewBag.fId_ID = new SelectList(db.tIdentity, "fId_ID", "fId_Identity", tEmployee.fId_ID);
            ViewBag.fSe_ID = new SelectList(db.tSector, "fSe_ID", "fSe_Sector", tEmployee.fSe_ID);
            ViewBag.Class = tEmployee.fEp_Name;
            //return View(tEmployee);
            return PartialView(tEmployee);
        }

        // POST: tEmployees/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AccountEdit([Bind(Include = "fEp_ID,fEp_Code,fEp_Name,fEp_SSNumber,fEp_Addr,fEp_Tel,fEp_Phone,fId_ID,fEp_Seniority,fSe_ID,fEp_Nickname,fEp_Password,fEp_Email,fEp_Picture,fEp_Birth")] tEmployee tEmployee,  HttpPostedFileBase EmployeeImage, string employeepassword)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tEmployee).State = EntityState.Modified;
                if (EmployeeImage != null && EmployeeImage.ContentLength > 0 && employeepassword != null)
                {
                    //將上傳的圖轉成二進位
                    var imgSize = EmployeeImage.ContentLength;
                    byte[] imgByte = new byte[imgSize];
                    EmployeeImage.InputStream.Read(imgByte, 0, imgSize);
                    tEmployee.fEp_Password = employeepassword;
                    tEmployee.fEp_Picture = imgByte;

                    db.Entry(tEmployee).State = EntityState.Modified;

                    db.SaveChanges();

                    return RedirectToAction("Bulletin", "Home");

                }
            }
            ViewBag.fId_ID = new SelectList(db.tIdentity, "fId_ID", "fId_Identity", tEmployee.fId_ID);
            ViewBag.fSe_ID = new SelectList(db.tSector, "fSe_ID", "fSe_Sector", tEmployee.fSe_ID);
            return PartialView(tEmployee);
            //return View(tEmployee);
        }
        [HttpGet]
        public ActionResult AccountManager(string idstring)
        {
            if (Request.Cookies["account"] == null)
            {
                //id = 0;
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            idstring = Request.Cookies["account"].Value;
            int id = db.tEmployee.Where(p => p.fEp_Code == idstring).Single().fEp_ID;
            tEmployee tEmployee = db.tEmployee.Find(id);
            if (tEmployee == null)
            {
                return HttpNotFound();
            }
            ViewBag.fId_ID = new SelectList(db.tIdentity, "fId_ID", "fId_Identity", tEmployee.fId_ID);
            ViewBag.fSe_ID = new SelectList(db.tSector, "fSe_ID", "fSe_Sector", tEmployee.fSe_ID);
            ViewBag.Class = tEmployee.fEp_Name;
            //return View(tEmployee);
            return PartialView(tEmployee);
        }
        [HttpPost]
        public ActionResult AccountManager([Bind(Include = "fEp_ID,fEp_Code,fEp_Name,fEp_SSNumber,fEp_Addr,fEp_Tel,fEp_Phone,fId_ID,fEp_Seniority,fSe_ID,fEp_Nickname,fEp_Password,fEp_Email,fEp_Picture,fEp_Birth")] tEmployee tEmployee, HttpPostedFileBase EmployeeImage, string employeepassword)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tEmployee).State = EntityState.Modified;
                if (EmployeeImage != null && EmployeeImage.ContentLength > 0 && employeepassword != null)
                {
                    //將上傳的圖轉成二進位
                    var imgSize = EmployeeImage.ContentLength;
                    byte[] imgByte = new byte[imgSize];
                    EmployeeImage.InputStream.Read(imgByte, 0, imgSize);
                    tEmployee.fEp_Password = employeepassword;
                    tEmployee.fEp_Picture = imgByte;

                    db.Entry(tEmployee).State = EntityState.Modified;

                    db.SaveChanges();

                    return RedirectToAction("Bulletin", "Home");

                }
            }
            ViewBag.fId_ID = new SelectList(db.tIdentity, "fId_ID", "fId_Identity", tEmployee.fId_ID);
        ViewBag.fSe_ID = new SelectList(db.tSector, "fSe_ID", "fSe_Sector", tEmployee.fSe_ID);
            return PartialView(tEmployee);

    }

    }

    }