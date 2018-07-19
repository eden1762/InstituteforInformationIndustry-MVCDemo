using MVC_HW3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;


namespace MVC_HW3.Areas.Albums.Controllers
{
    public class AlbumsController : Controller
    {
        TravelEntities db = new TravelEntities();
        // GET: Albums/Albums
        public ActionResult Index()
        {
            return View(db.tAlbum.OrderByDescending(a=>a.fAl_ID).ToList());
        }
        //public ActionResult Index()
        //{
        //    return View(db.tPhoto.GroupBy(c => c.fAl_ID, (key, c) => c.FirstOrDefault()).ToList());
        //}
        [ChildActionOnly]
        public ActionResult Home_Photo()
        {
            List<tPhoto> photo = db.tPhoto.ToList();
            Random rand = new Random();
            int count = ((photo.Count>=5? photo.Count:5) /5);
            int[] num = new int[5] {rand.Next(1,count+1),rand.Next(count+1,count*2+1),rand.Next(count*2+1,count*3+1),rand.Next(count*3+1,count*4+1),rand.Next(count*4+1, (photo.Count >= 5 ? photo.Count : 5)) };
            List<tPhoto> photo5=new List<tPhoto>();
            foreach(int a in num)
            {
                if (photo.Count >= a)
                {                   
                    photo5.Add(photo[a-1]);
                }                            
            }
            
            return PartialView(photo5);
        }
        public ActionResult Photo( int ID,int? pID)
        {
            ViewBag.Al =db.tAlbum.Find(ID);
            if (pID != null)
            {              
                ViewBag.pID = pID;             
            }
            return View(db.tPhoto.Where(a=>a.fAl_ID==ID).ToList());
        }
        public ActionResult GetImage(int id)
        {
            tPhoto photo = db.tPhoto.Find(id);
            byte[] file = photo.fPh_PicFile;                 
              
            return File(file, "image/jpeg");
        }
        public ActionResult PhotoMessage(int id,string content="")
        {            
            if(content !="")
            {
                string epCode = Request.Cookies["account"].Value;
                int epID = db.tEmployee.Where(a => a.fEp_Code == epCode).Single().fEp_ID;
                tForumMessage newMess = new tForumMessage();
                newMess.fFM_Date = DateTime.Now;
                newMess.fMC_ID = id;
                newMess.fFM_Content = content;
                newMess.fEp_ID = epID;
                db.tForumMessage.Add(newMess);
                db.SaveChanges();
            }
            return PartialView(db.tForumMessage.Where(a => a.fMC_ID == id).OrderBy(a => a.fFM_Date).ToList());
        }
       
    }
}