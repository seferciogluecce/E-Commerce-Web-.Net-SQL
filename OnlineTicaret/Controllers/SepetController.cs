using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using OnlineTicaret.Models;

namespace OnlineTicaret.Controllers
{
    public class SepetController : Controller
    {
        private ShopEntities db = new ShopEntities();

        // GET: Sepets
        public ActionResult Index(int? id,int? adet)
        {
            if (id == null)
            {
                var sepet = db.Sepet.Include(s => s.Ürün).Where(s => s.SepetDurumu == 1);
                return View(sepet.ToList());
            }
           else if (id == -1)
            {
                foreach(var item in db.Sepet.Where(m => m.SepetDurumu == 1))
                {
                    item.SepetDurumu = 0;
                    // db.Ürün.Where(m => m.ÜrünId == item.ÜrünId).Select(a => a.Mevcut) -= 1;
                    var sepetElemanları = db.Ürün.Where(m => m.ÜrünId == item.Ürün.ÜrünId);
                    foreach (var eleman in sepetElemanları)
                    {
                        eleman.Mevcut += item.ÜrünAdedi;
                    }
                   
                    db.Sepet.Remove(item);
                }
                db.SaveChanges();
                var sepet = db.Sepet.Include(s => s.Ürün).Where(s=>s.SepetDurumu==1);
                return View(sepet.ToList());

            }
            else if (id == -2)
            {
                foreach (var item in db.Sepet.Where(m => m.SepetDurumu == 1))
                {
                    item.SepetDurumu = 0;
                    var sepetElemanları = db.Ürün.Where(m => m.ÜrünId == item.ÜrünId);
                    foreach (var eleman in sepetElemanları)
                    {
                        eleman.Mevcut -= item.ÜrünAdedi;
                        eleman.Satıldı += item.ÜrünAdedi;
                    }
                }
                db.SaveChanges();
                var sepet = db.Sepet.Include(s => s.Ürün).Where(s => s.SepetDurumu == 1);
                return View(sepet.ToList());

            }
            else
            {
                //SepetId = db.Sepet.Where(m => m.SepetDurumu == 1).Select(t => t.SepetId).First()
                int ekleAdet;
                if (adet == null)
                    ekleAdet = 1;
                else
                    ekleAdet = adet.Value;
                if (db.Sepet.Where(m => m.SepetDurumu == 0).Count() == db.Sepet.Count() && db.Sepet.Count() == 0)
                { //ilk baş           
                    db.Sepet.Add(new Sepet { SepetNo = 1, ÜrünId = id, SepetDurumu = 1, ÜrünAdedi = ekleAdet });
                    db.Ürün.Where(s => s.ÜrünId == id).First().Mevcut -= ekleAdet;
                }
                else if (db.Sepet.Where(m => m.SepetDurumu == 0).Count() == db.Sepet.Count())//boşken
                {
                    db.Sepet.Add(new Sepet { SepetNo = db.Sepet.OrderByDescending(m => m.SepetNo).Select(t => t.SepetNo).First() + 1, ÜrünId = id, SepetDurumu = 1, ÜrünAdedi = ekleAdet });
                    db.Ürün.Where(s => s.ÜrünId == id).First().Mevcut -= ekleAdet;

                }
                else if (db.Sepet.Where(m => m.SepetDurumu == 1).Where(m => m.Ürün.ÜrünId == id).Count() != 0)
                {
                    db.Sepet.Where(m => m.SepetDurumu == 1).Where(m => m.Ürün.ÜrünId == id).First().ÜrünAdedi += ekleAdet;

                    db.Ürün.Where(s => s.ÜrünId == id).First().Mevcut -= ekleAdet;

                }
                else
                {
                    db.Sepet.Add(new Sepet { SepetNo = db.Sepet.Where(m => m.SepetDurumu == 1).Select(t => t.SepetNo).First(), ÜrünId = id, SepetDurumu = 1, ÜrünAdedi = ekleAdet });
                    db.Ürün.Where(s => s.ÜrünId == id).First().Mevcut -= ekleAdet;

                }
                db.SaveChanges();
                var sepet = db.Sepet.Where(s => s.SepetDurumu == 1).Include(s =>s.Ürün);
                return View(sepet.ToList());
            }
        }

        // GET: Sepets/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sepet sepet = db.Sepet.Find(id);
            if (sepet == null)
            {
                return HttpNotFound();
            }
            return View(sepet);
        }

        // GET: Sepets/Create
        public ActionResult Create()
        {
            ViewBag.ÜrünId = new SelectList(db.Ürün, "ÜrünId", "Açıklama");
            return View();
        }

        // POST: Sepets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SepetId,ÜrünId,SepetDurumu")] Sepet sepet)
        {
            if (ModelState.IsValid)
            {
                db.Sepet.Add(sepet);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ÜrünId = new SelectList(db.Ürün, "ÜrünId", "Açıklama", sepet.ÜrünId);
            return View(sepet);
        }

        // GET: Sepets/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sepet sepet = db.Sepet.Find(id);
            if (sepet == null)
            {
                return HttpNotFound();
            }
            ViewBag.ÜrünId = new SelectList(db.Ürün, "ÜrünId", "Açıklama", sepet.ÜrünId);
            return View(sepet);
        }

        // POST: Sepets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SepetId,ÜrünId,SepetDurumu")] Sepet sepet)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sepet).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ÜrünId = new SelectList(db.Ürün, "ÜrünId", "Açıklama", sepet.ÜrünId);
            return View(sepet);
        }

        // GET: Sepets/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sepet sepet = db.Sepet.Find(id);
            if (sepet == null)
            {
                return HttpNotFound();
            }
            return View(sepet);
        }

        // POST: Sepets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Sepet sepet = db.Sepet.Find(id);
            db.Sepet.Where(a => a.SepetId == id).First().Ürün.Mevcut += db.Sepet.Where(a => a.SepetId == id).First().ÜrünAdedi;
            db.Sepet.Remove(sepet);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
