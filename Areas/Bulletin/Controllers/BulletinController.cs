using MVC_HW3.Models;
using MVC_HW3.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace MVC_HW3.Areas.Bulletin.Controllers
{
    public class BulletinController : Controller
    {
        TravelEntities db = new TravelEntities();
        // GET: Bulletin/Bulletin
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult tBulletinCreate()
        {
            TempData["Date"] = DateTime.Now;
            ViewBag.fBC_ID = new SelectList(db.tBulletinClass, "fBC_ID", "fBC_Class");
            return View();
        }
        [HttpPost]
        public ActionResult tBulletinCreate([Bind(Include = "fBu_ID,fBu_Date,fBC_ID,fBu_Title,fBu_Content")] tBulletin tBulletin)
        {
            if (ModelState.IsValid)
            {

                tBulletin.fBu_Date = DateTime.Now;
                db.tBulletin.Add(tBulletin);
                db.SaveChanges();
                return RedirectToAction("Bulletin", "Home", new { area = "" });
            }

            ViewBag.fBC_ID = new SelectList(db.tBulletinClass, "fBC_ID", "fBC_Class", tBulletin.fBC_ID);
            return View(tBulletin);

        }
        public ActionResult tBulletinEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tBulletin tBulletin = db.tBulletin.Find(id);
            if (tBulletin == null)
            {
                return HttpNotFound();
            }
            ViewBag.fBC_ID = new SelectList(db.tBulletinClass, "fBC_ID", "fBC_Class", tBulletin.fBC_ID);
            return View(tBulletin);
        }

        // POST: tBulletins/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult tBulletinEdit([Bind(Include = "fBu_ID,fBu_Date,fBC_ID,fBu_Title,fBu_Content")] tBulletin tBulletin)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tBulletin).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Bulletin", "Home", new { area = "" });
            }
            ViewBag.fBC_ID = new SelectList(db.tBulletinClass, "fBC_ID", "fBC_Class", tBulletin.fBC_ID);
            return View(tBulletin);
        }
        public ActionResult tBulletinDelete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tBulletin tBulletin = db.tBulletin.Find(id);
            if (tBulletin == null)
            {
                return HttpNotFound();
            }
            return View(tBulletin);
        }

        // POST: tBulletins/Delete/5
        [HttpPost, ActionName("tBulletinDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult tBulletinDeleteConfirmed(int id)
        {
            tBulletin tBulletin = db.tBulletin.Find(id);
            db.tBulletin.Remove(tBulletin);
            db.SaveChanges();
            return RedirectToAction("Bulletin", "Home", new { area = "" });
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public ActionResult tAlbumDelete(int? id)
        {
            
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tAlbum tAlbum = db.tAlbum.Find(id);
            if (tAlbum == null)
            {
                return HttpNotFound();
            }
            ViewBag.Class = tAlbum.tTravelCase.fTC_Title;
            return View(tAlbum);
        }

        // POST: tAlbums/Delete/5
        [HttpPost, ActionName("tAlbumDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult tAlbumDeleteConfirmed(int id)
        {
            tAlbum tAlbum = db.tAlbum.Find(id);
            var list = db.tPhoto.Where(p=>p.fAl_ID==id).ToList();
            db.tAlbum.Remove(tAlbum);            
            db.tPhoto.RemoveRange(list);
            db.SaveChanges();
            return RedirectToAction("Index", "Albums", new { area = "Albums" });
        }
        public ActionResult tPhotoDelete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tPhoto tPhoto = db.tPhoto.Find(id);
            if (tPhoto == null)
            {
                return HttpNotFound();
            }            
            return View(tPhoto);
        }

        // POST: tPhotoes/Delete/5
        [HttpPost, ActionName("tPhotoDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult tPhotoDeleteConfirmed(int id)
        {
            tPhoto tPhoto = db.tPhoto.Find(id);
            db.tPhoto.Remove(tPhoto);
            db.SaveChanges();
            return RedirectToAction("Index", "Albums", new { area = "Albums" });
        }
        public ActionResult tAlbumCreate()
        {
            ViewBag.fTC_ID = new SelectList(db.tTravelCase.Where(c => c.tAlbum.Where(a => a.fTC_ID == c.fTC_ID).Count() == 0).Select(c => c).Where(p=>p.fTC_LorD>0), "fTC_ID", "fTC_Title");
            return View();
        }

        // POST: tAlbums/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult tAlbumCreate([Bind(Include = "fAl_ID,fTC_ID,fAl_Date")] tAlbum tAlbum)
        {
            try
            {
                if (db.tAlbum.Where(p => p.fTC_ID == tAlbum.fTC_ID).Count() < 1)
                {
                    if (ModelState.IsValid)
                    {
                        db.tAlbum.Add(tAlbum);
                        tAlbum.fAl_Date = DateTime.Now;
                        db.SaveChanges();
                        return RedirectToAction("Index", "Albums", new { area = "Albums" });
                    }

                    ViewBag.fTC_ID = new SelectList(db.tTravelCase, "fTC_ID", "fTC_Title", tAlbum.fTC_ID);
                    return View(tAlbum);
                }
                else
                {

                    return Content("<script language='javascript' type='text/javascript'>alert('該旅遊提案相簿已建立!');</script>");

                }
            }
            catch(Exception ex)
            {
                return RedirectToAction("Index", "Albums", new { area = "Albums"});
            }
        }
            public ActionResult tPhotoCreate(tPhoto tPhoto,int id)
        {

                tPhoto.fAl_ID = id;
                ViewBag.fAl_ID = new SelectList(db.tAlbum, "fAl_ID", "fAl_ID");
                ViewBag.fMC_ID = new SelectList(db.tMessageCode, "fMC_ID", "fMC_ID");
                return View(tPhoto);                

        }

        // POST: tPhotoes/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult tPhotoCreate([Bind(Include = "fPh_ID,fPh_PicFile,fAl_ID,fPh_Date,fPh_Notes,fMC_ID")] tPhoto tPhoto, HttpPostedFileBase[] photoImage)
        {
            try {
            
                if (ModelState.IsValid)
            {
                if (photoImage != null && photoImage.Count() > 0)
                {

                        foreach (var uploadFile in photoImage)
                        {
                            if (uploadFile.ContentLength > 0)
                            {
                                var imgSize = uploadFile.ContentLength;
                                byte[] imgByte = new byte[imgSize];
                                uploadFile.InputStream.Read(imgByte, 0, imgSize);
                                tMessageCode tMessageCode = new tMessageCode();
                                db.tMessageCode.Add(tMessageCode);
                                db.SaveChanges();
                                tPhoto.fPh_Notes = "";
                                tPhoto.fPh_PicFile = imgByte;
                                tPhoto.fPh_Date = DateTime.Now;                                
                                tPhoto.fMC_ID = db.tMessageCode.AsEnumerable().Last().fMC_ID;
                                db.tPhoto.Add(tPhoto);
                                db.SaveChanges();
                                
                            }
                        }
                        return RedirectToAction("Photo", "Albums", new { area = "Albums" ,id=tPhoto.fAl_ID});
                    }
            }
            ViewBag.fAl_ID = new SelectList(db.tAlbum, "fAl_ID", "fAl_ID", tPhoto.fAl_ID);
            ViewBag.fMC_ID = new SelectList(db.tMessageCode, "fMC_ID", "fMC_ID", tPhoto.fMC_ID);
            return View(tPhoto);
            }
            catch(Exception ex)
            {
                return RedirectToAction("Index", "Albums", new { area = "Albums"});
            }
        }
        //public ActionResult tPhotoAdd(tPhoto tPhoto)
        //{

        //    ViewBag.fAl_ID = new SelectList(db.tAlbum, "fAl_ID", "fAl_ID");
        //    ViewBag.fMC_ID = new SelectList(db.tMessageCode, "fMC_ID", "fMC_ID");
        //    return View(tPhoto);

        //}
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult tPhotoAdd([Bind(Include = "fPh_ID,fPh_PicFile,fAl_ID,fPh_Date,fPh_Notes,fMC_ID")] tPhoto tPhoto, HttpPostedFileBase[] photoImage)
        //{
        //    try
        //    {
        //        //if (photoImage.Count() > 0)
        //        //{
        //        //    foreach (var uploadFile in photoImage)
        //        //    {
        //        //        if (uploadFile.ContentLength > 0)
        //        //        {
        //        //            string savePath = Server.MapPath("~/Files/");
        //        //            uploadFile.SaveAs(savePath + uploadFile.FileName);
        //        //        }
        //        //    }
        //        //}
        //        //return RedirectToAction("Index", "Albums", new { area = "Albums" });
        //        if (ModelState.IsValid)
        //        {
        //            if (photoImage != null && photoImage.Count() > 0)
        //            {

        //                foreach (var uploadFile in photoImage)
        //                {
        //                    if (uploadFile.ContentLength > 0)
        //                    {
        //                        var imgSize = uploadFile.ContentLength;
        //                        byte[] imgByte = new byte[imgSize];
        //                        uploadFile.InputStream.Read(imgByte, 0, imgSize);

        //                        tPhoto.fPh_PicFile = imgByte;
        //                        tPhoto.fPh_Date = DateTime.Now;
        //                        db.tPhoto.Add(tPhoto);
        //                        db.SaveChanges();
        //                        return RedirectToAction("Index", "Albums", new { area = "Albums" });
        //                    }
        //                }
        //            }
        //        }
        //        ViewBag.fAl_ID = new SelectList(db.tAlbum, "fAl_ID", "fAl_ID", tPhoto.fAl_ID);
        //        ViewBag.fMC_ID = new SelectList(db.tMessageCode, "fMC_ID", "fMC_ID", tPhoto.fMC_ID);
        //        return View(tPhoto);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        public ActionResult ImageView(int? id)
        {
            tPhoto tPhoto = db.tPhoto.Find(id);
            byte[] img = tPhoto.fPh_PicFile;
            return File(img, "image/jpeg");
        }
        //public ActionResult MultiFileUploadDB()
        //{
        //    return View();
        //}
        //[HttpPost]
        //public ActionResult MultiFileUploadDB(IEnumerable<HttpPostedFileBase> files, string text)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        string message = null;
        //        foreach (var file in files)
        //        {
        //            if (file != null && file.ContentLength > 0)
        //            {
        //                string fileName = Path.GetFileName(file.FileName);
        //                int length = file.ContentLength;
        //                byte[] buffer = new byte[length];
        //                file.InputStream.Read(buffer, 0, length);
        //                var imgSize = file.ContentLength;
        //                byte[] imgByte = new byte[imgSize];
        //                file.InputStream.Read(imgByte, 0, imgSize);

        //                tPhoto tPhoto = new tPhoto()
        //                {
        //                    fPh_Notes = text,
        //                    fPh_PicFile = imgByte,
        //                    fPh_Date = DateTime.Now,
        //                };
        //                try
        //                {
        //                    db.tPhoto.Add(tPhoto);
        //                    db.SaveChanges();
        //                    message = "Name:" + fileName + ",<br>" + "Content Type:" + file.ContentType + ",<br>" + "Size:" + file.ContentLength + ",<br>" + "上傳成功。";
        //                    TempData["Message"] = message;
        //                }
        //                catch (Exception ex)
        //                {
        //                    TempData["Message"] = "儲存錯誤：" + ex.Message;
        //                }
        //            }
        //            else
        //            {
        //               TempData["Message"] = "未選擇或空白檔案。";
        //            }
        //        }
        //    }
        //    return View();
                
            
        //}
        public ActionResult tPhotoEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tPhoto tPhoto = db.tPhoto.Find(id);            
            if (tPhoto == null)
            {
                return HttpNotFound();
            }
            ViewBag.fAl_ID = new SelectList(db.tAlbum, "fAl_ID", "fAl_ID", tPhoto.fAl_ID);
            ViewBag.fMC_ID = new SelectList(db.tMessageCode, "fMC_ID", "fMC_ID", tPhoto.fMC_ID);
            return View(tPhoto);
        }

        // POST: tPhotoes/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult tPhotoEdit([Bind(Include = "fPh_ID,fPh_PicFile,fAl_ID,fPh_Date,fPh_Notes,fMC_ID")] tPhoto tPhoto)
        {
            try
            {

                if (ModelState.IsValid)
                {
  
   
                                db.Entry(tPhoto).State = EntityState.Modified;

                                db.SaveChanges();

                         

                       
                        return RedirectToAction("Photo", "Albums", new { area = "Albums" ,id=tPhoto.fAl_ID});
                    }
             
                ViewBag.fAl_ID = new SelectList(db.tAlbum, "fAl_ID", "fAl_ID", tPhoto.fAl_ID);
                ViewBag.fMC_ID = new SelectList(db.tMessageCode, "fMC_ID", "fMC_ID", tPhoto.fMC_ID);
                return View(tPhoto);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
       

        public ActionResult ImagePreview(int? id)
        {
            tPhoto tPhoto = db.tPhoto.Find(id);
            byte[] img = tPhoto.fPh_PicFile;
            return File(img, "image/jpeg");
        }
    }   
}