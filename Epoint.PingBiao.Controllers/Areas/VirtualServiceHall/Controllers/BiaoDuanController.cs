using Epoint.PingBiao.Controllers.Expand;
using Epoint.PingBiao.Controllers.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Epoint.PingBiao.Controllers.Areas.VirtualServiceHall.Controllers
{
    public class BiaoDuanController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /VirtualServiceHall/BiaoDuan/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /VirtualServiceHall/BiaoDuan/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /VirtualServiceHall/BiaoDuan/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /VirtualServiceHall/BiaoDuan/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /VirtualServiceHall/BiaoDuan/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /VirtualServiceHall/BiaoDuan/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /VirtualServiceHall/BiaoDuan/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
