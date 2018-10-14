﻿// Licensed to the Swastika I/O Foundation under one or more agreements.
// The Swastika I/O Foundation licenses this file to you under the GNU General Public License v3.0.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swastika.Cms.Lib;
using Swastika.Cms.Lib.Repositories;
using Swastika.Cms.Lib.ViewModels;
using Swastika.Cms.Mvc.Controllers;
using Swastika.Common.Helper;
using System.Collections.Generic;

namespace Swastika.Cms.Mvc.Areas.Portal.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize]
    [Area("Portal")]
    [Route("{culture}/Portal/File")]
    public class FileController : BaseController
    {
        public FileController(IHostingEnvironment env
            )
            : base(env)
        {
        }

        #region Theme Files

        [HttpGet]
        [Route("Theme/{themeName}")]
        [Route("Theme/{themeName}/{folder}")]
        public IActionResult Theme(string themeName, string folder)
        {
            string fullPath = CommonHelper.GetFullPath(new string[]
            {
                SWCmsConstants.Parameters.TemplatesAssetFolder,
                themeName,
                folder
            });
            List<string> directories = FileRepository.Instance.GetTopDirectories(fullPath);
            List<FileViewModel> files = !string.IsNullOrEmpty(folder) ? FileRepository.Instance.GetTopFiles(fullPath)
                : new List<FileViewModel>();
            ViewData["name"] = themeName;
            ViewData["directories"] = directories;
            return View(files);
        }

        [HttpGet]
        [Route("EditTheme/{themeName}/{folder}/{name}/{ext}")]
        public IActionResult EditTheme(string themeName, string folder, string name, string ext)
        {
            string fullPath = CommonHelper.GetFullPath(new string[]
          {
                SWCmsConstants.Parameters.TemplatesAssetFolder,
                themeName,
                folder,
                name
          });
            string filePath = string.Format("{0}{1}", fullPath, ext);
            return View(FileRepository.Instance.GetWebFile(filePath, folder));
        }

        [HttpPost]
        [Route("EditTheme/{themeName}/{folder}/{name}/{ext}")]
        public IActionResult Edit(string themeName, string folder, string name, string ext,
            FileViewModel template)
        {
            if (ModelState.IsValid)
            {
                template.FileFolder = CommonHelper.GetFullPath(new string[]
         {
                SWCmsConstants.Parameters.TemplatesAssetFolder,
                themeName,
                folder
         });
                var result = FileRepository.Instance.SaveWebFile(template);
                if (result)
                {
                    return RedirectToAction("Theme", new { themeName, folder });
                }
            }
            ModelState.AddModelError(string.Empty, "Invalid Model");
            return View(template);
        }

        [HttpGet]
        [Route("DeleteTheme/{themeName}/{folder}/{name}/{ext}")]
        [Route("DeleteTheme/{themeName}/{name}/{ext}")]
        public IActionResult DeleteTheme(string themeName, string folder, string name, string ext)
        {
            string fullPath = CommonHelper.GetFullPath(new string[]
          {
                SWCmsConstants.Parameters.WebRootPath,
                SWCmsConstants.Parameters.TemplatesAssetFolder,
                themeName,
                folder,
                name
          });
            string filePath = string.Format("{0}{1}", fullPath, ext);
            FileRepository.Instance.DeleteFile(filePath);
            return RedirectToAction("Theme", routeValues: new { themeName, folder });
        }

        #endregion Theme Files

        [HttpGet]
        [Route("")]
        public IActionResult Index(string folder)
        {
            var templates = FileRepository.Instance.GetTopFiles(folder);
            var directories = FileRepository.Instance.GetTopDirectories(folder);
            ViewData["directories"] = directories;
            ViewBag.folder = folder;
            return View(templates);
        }

        [HttpPost]
        [Route("")]
        public IActionResult Index(string folder, IFormFile file)
        {
            if (file?.Length > 0)
            {
                folder = SwCmsHelper.GetFullPath(new string[]
                {
                    SWCmsConstants.Parameters.FileFolder,
                    folder
                });
                //string filename = 
                FileRepository.Instance.SaveWebFile(file, folder);
            }
            var files = FileRepository.Instance.GetTopFiles(folder);
            var directories = FileRepository.Instance.GetTopDirectories(folder);
            ViewData["directories"] = directories;
            ViewBag.folder = folder;
            return View(files);
        }

        // GET: article/Edit/5
        [HttpGet]
        [Route("Edit/{name}/{ext}")]
        public IActionResult Edit(string name, string ext, string folder)
        {
            string filename = string.Format("{0}{1}", name, ext);
            var template = FileRepository.Instance.GetWebFile(filename, folder);
            if (template == null)
            {
                return RedirectToAction("", new { folder });
            }
            return View(template);
        }

        // GET: article/Edit/5
        [HttpPost]
        [Route("Edit/{name}/{ext}")]
        public IActionResult Edit(FileViewModel template)
        {
            if (ModelState.IsValid)
            {
                var result = FileRepository.Instance.SaveWebFile(template);
                if (result)
                {
                    return RedirectToAction("", new { folder = template.FileFolder });
                }
            }
            ModelState.AddModelError(string.Empty, "Invalid Model");
            return View(template);
        }

        // GET: article/Edit/5
        [HttpGet]
        [Route("Delete/{name}/{ext}")]
        public IActionResult Delete(string name, string ext, string folder)
        {
            return RedirectToAction("", routeValues: new { folder });
        }
    }
}
