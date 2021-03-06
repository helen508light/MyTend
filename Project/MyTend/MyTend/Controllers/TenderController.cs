﻿namespace MyTend.Controllers
{
    using MyTend.Attributes;
    using MyTend.Entites;
    using MyTend.Models;
    using MyTend.Services.EmailService;
    using MyTender.Core;
    using MyTender.Security;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    public class TenderController : BaseController
    {
        private string tempString = string.Empty;

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {

            base.OnActionExecuted(filterContext);
        }

        /// <summary>
        /// Выбор создания тендера
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var model = new ListThemeTenderModel();

            return View(model);
        }

        /// <summary>
        /// Список тендеров
        /// </summary>
        /// <returns></returns>
        [BanResource]
        public ActionResult List()
        {
            var model = new ActiveTenderListModel();

            return View(model);
        }

        public ActionResult Hide(int id)
        {
            var exist = TenderHide
                .FindAll()
                .Where(x => x.User.Id == this.Auth.User.Id)
                .FirstOrDefault(x => x.Tender.Id == id);

            if (exist == null)
            {
                var hiden = new TenderHide()
                {
                    User = this.Auth.User,
                    Tender = Tender.GetById(id)
                };

                hiden.Save();
            }

            return JsonSuccess();
        }

        /// <summary>
        /// Создать тенедер
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [BanResource]
        public ActionResult Create(int id)
        {
            var model = new CreateTenderModel(id);

            return View(model);
        }

        /// <summary>
        /// Создать тенедер
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        [BanResource]
        public ActionResult Create(CreateTenderModel model)
        {
            if (this.Auth.User == null)
            {
                return RedirectToAction("Registration", "Account");
            }

            if (model.IsValid())
            {
                var isSub = this.ViewBag.HasPay;

                model.Save(isSub);

                this.tempString = model.Title;

                try
                {
                    var service = new EmailService(this.Auth.User.Email);
                    service.CreateTender(model.Id.ToString(), model.Title);
                }
                catch
                { }

                return RedirectToAction("Created", new { id = model.Id });
            }

            return View(model);
        }

        public ActionResult Created(int id)
        {
            var tender = new TenderService().GetTender(id);

            return View(tender);
        }

        /// <summary>
        /// Активные тендеры
        /// </summary>
        /// <returns></returns>
        [BanResource]
        public ActionResult Active() 
        {
            var model = new ListTendersModel();

            return View(model);
        }

        /// <summary>
        /// Мои тендеры
        /// </summary>
        /// <returns></returns>
        public ActionResult My()
        {
            var model = new MyTendersModel();

            return View(model);
        }
        
        [BanResource]
        public ActionResult Winner()
        {
            var model = new WinnerTenderModel();

            model.Load();

            return View(model);
        }

        /// <summary>
        /// Подробно
        /// </summary>
        /// <param name="id">Id тендера</param>
        /// <returns></returns>
        public ActionResult Details(int id)
        {
            var model = new TenderDetailsModel(this.Auth, id);

            return View(model);
        }

        public ActionResult SelectedWinner(int id)
        {
            var tender = new TenderService().GetTender(id);

            return View(tender);
        }

        public ActionResult SelectWinner(int userId, int tenderId)
        {
            try
            {
                var model = new CloseTenderModel(userId, tenderId);
                model.Close();

                try
                {
                    var service = new EmailService(model.Winner.Email);
                    service.Winner(tenderId.ToString(), model.Winner.FullName, model.Tender.Title);
                }
                catch
                { }
            }
            catch(Exception ex)
            {
                return JsonFailur(ex.Message);
            }

            return RedirectToAction("SelectedWinner", new { @id = tenderId });
        }

        public ActionResult CancelTender(int id)
        {
            var model = new CloseTenderModel(this.Auth.User.Id, id);
            model.Cancel();

            return RedirectToAction("My");
        }

        [HttpPost]
        [BanResource]
        public ActionResult AddComment(TenderMessageModel model)
        {
            try
            {
                if (model.IsValid())
                {
                    model.Save();

                    try
                    {
                        if (model.Tender.User.Id != this.Auth.User.Id)
                        {
                            var service = new EmailService(this.Auth.User.Email);
                            service.AddComment(this.Auth.User.FullName, model.TenderId.ToString(), model.Tender.User.FullName, model.Tender.Title);
                        }
                    }
                    catch
                    { }

                    return JsonSuccess();
                }
            }
            catch
            { }

            return JsonFailur(model);
        }

        public ActionResult Map()
        {
            var themes = new Dictionary<string, List<TenderTheme>>();

            var all = TenderTheme.FindAll()
                .OrderBy(x => x.Theme)
                .ToList();

            all.ForEach(x => {
                if (!themes.Keys.Contains(x.Theme))
                {
                    themes.Add(x.Theme, new List<TenderTheme>());
                }

                themes[x.Theme].Add(x);
            });

            return View(themes);
        }
    }
}