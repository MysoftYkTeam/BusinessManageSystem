﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace Config.Controllers
{
    public class FileUploadController : Controller
    {
        // GET: FileUpload
        public ActionResult Index()
        {
            string path = Server.MapPath("~/UploadFiles/demo.txt");
            System.IO.File.Create(path);
            return View();
        }

        // GET: FileUpload/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: FileUpload/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: FileUpload/Create
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

        // GET: FileUpload/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: FileUpload/Edit/5
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

        // GET: FileUpload/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: FileUpload/Delete/5
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
