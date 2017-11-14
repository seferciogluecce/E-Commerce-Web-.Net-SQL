using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using OnlineTicaret.Models;
using System.Web.WebPages;

namespace OnlineTicaret.Controllers
{
    public class ÜrünController : Controller
    {
        private ShopEntities db = new ShopEntities();

        // GET: Ürün
        public ActionResult Index()
        {
            ViewBag.KategoriId = new SelectList(db.Kategori, "KategoriId", "KategoriAdı");
            var ürün = db.Ürün.Include(ü => ü.Kategori);
            var kats = new SelectList( db.Kategori.Select(a => a.KategoriAdı));


            ViewBag.kategoriler = db.Kategori.Select(a => a.KategoriAdı);

            return View(ürün.OrderBy(s => s.ÜrünAdı).ToList());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index([Bind(Include = "ÜrünId,Ücret,Mevcut,Satıldı,KategoriId,Açıklama,ÜrünAdı")] Ürün ür)
        {
            var ürün = db.Ürün.Include(ü => ü.Kategori).Where(a=>a.KategoriId==ür.KategoriId);
            var kats = new SelectList(db.Kategori.Select(a => a.KategoriAdı));
            ViewBag.KategoriId = new SelectList(db.Kategori, "KategoriId", "KategoriAdı");
            ViewBag.kategoriler = db.Kategori.Select(a => a.KategoriAdı);

            return View(ürün);
        }
        public ActionResult Details(int? id)
        {
            ViewBag.id = id;

            int x = 1;
            ViewBag.adet = x;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ürün ürün = db.Ürün.Find(id);
            if (ürün == null)
            {
                return HttpNotFound();
            }
            return View(ürün);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Details([Bind(Include = "ÜrünId,Ücret,Mevcut,Satıldı,KategoriId,Açıklama,ÜrünAdı,alınacak")] Ürün ür, int? idci)
        {
          

            if (ModelState.IsValid)
            {

                return RedirectToAction("Index", "Sepet", new { id = idci, adet = ür.alınacak });
            }

            return View(ür);
        }


        [HttpGet]
        public ActionResult Sorted(string kategori)
        {
            
            var ürün = db.Ürün.Include(ü => ü.Kategori).Where(a => a.Kategori.KategoriAdı == kategori);
            var kats = new SelectList(db.Kategori.Select(a => a.KategoriAdı));

            

           // ürün = ürün.Where(a => a.Kategori.KategoriAdı == xKategori).Select(a=>a.ÜrünId);
            ViewBag.kategoriler = db.Kategori.Select(a => a.KategoriAdı);

            return View(ürün.OrderBy(s => s.ÜrünAdı).ToList());
        }
        [HttpPost]
        [ActionName("Sorted")]
        public ActionResult SortedPost(IEnumerable<Ürün> Ürünler)
        {

            var ürün = db.Ürün.Include(ü => ü.Kategori).Where(a => a.Kategori.KategoriAdı ==Ürünler.First().kategoriSeç );
            var kats = new SelectList(db.Kategori.Select(a => a.KategoriAdı));



            // ürün = ürün.Where(a => a.Kategori.KategoriAdı == xKategori).Select(a=>a.ÜrünId);
            ViewBag.kategoriler = db.Kategori.Select(a => a.KategoriAdı);

            return View(ürün.OrderBy(s => s.ÜrünAdı).ToList());
        }

        // GET: Ürün/Details/5
       


        public ActionResult redirectToSepet(FormCollection frm)
        {
            int adets = frm["alsan"].AsInt();
            int ids = 6;
            return RedirectToAction("Sepet","Index", new {id=ids, adet=adets });

        }
        
        // GET: Ürün/Create
        public ActionResult Create()
        {
            ViewBag.KategoriId = new SelectList(db.Kategori, "KategoriId", "KategoriAdı");
            return View();
        }

        // POST: Ürün/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ÜrünId,Ücret,Mevcut,Satıldı,KategoriId,Açıklama,ÜrünAdı")] Ürün ürün)
        {
            if (ModelState.IsValid)
            {
               // ürün.ÜrünId = null;
                db.Ürün.Add(ürün);

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.KategoriId = new SelectList(db.Kategori, "KategoriId", "KategoriAdı", ürün.KategoriId);
            return View(ürün);
        }

        // GET: Ürün/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ürün ürün = db.Ürün.Find(id);
            if (ürün == null)
            {
                return HttpNotFound();
            }
            ViewBag.KategoriId = new SelectList(db.Kategori, "KategoriId", "KategoriAdı", ürün.KategoriId);
            return View(ürün);
        }

        // POST: Ürün/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ÜrünId,Ücret,Mevcut,Satıldı,KategoriId,Açıklama,ÜrünAdı")] Ürün ürün)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ürün).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.KategoriId = new SelectList(db.Kategori, "KategoriId", "KategoriAdı", ürün.KategoriId);
            return View(ürün);
        }

        // GET: Ürün/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ürün ürün = db.Ürün.Find(id);
            if (ürün == null)
            {
                return HttpNotFound();
            }
            return View(ürün);
        }

        // POST: Ürün/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ürün ürün = db.Ürün.Find(id);
            db.Ürün.Remove(ürün);
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
