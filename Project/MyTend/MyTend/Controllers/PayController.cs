﻿using MyTend.Entites;
using MyTend.Services.Common;
using MyTender.Core;
using MyTender.Security;
using RomanPushkin.BetterRobokassa;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace MyTend.Controllers
{
    public class PayController : BaseController
    {
        // GET: Pay
        public ActionResult Index()
        {
            ViewBag.PayScript = GetPayString();
            ViewBag.PayHistory = new PayService(this.Auth.User).GetHistory();
            ViewBag.PayEnd = new PayService(this.Auth.User).GetDatePayEnd();

            return View();
        }

        private string GetPayString()
        {
            var priceRub = Constants._SUB_SUM;
            var orderId = 0;
            var customerEmail = this.Auth.User.Login;

            string redirectUrl =
                string.Format("<a class=\"btn btn-two\" href=\"{0}\">Оплатить</a>",
                Robokassa.GetRedirectUrl(priceRub, orderId, customerEmail, this.Auth.User.Id.ToString()));
                

            return redirectUrl;

        }

        public ActionResult Success(RobokassaConfirmationRequest confirmationRequest)
        {
            ViewBag.Email = this.Request.Url.ToString();

            try
            {

                if (confirmationRequest.IsQueryValid(RobokassaQueryType.SuccessURL))
                {
                    var userId = int.Parse(confirmationRequest.Shp_item);//GetPrm("Email");

                    var dateBegin = DateTime.Now;
                    var dateEnd = DateTime.Now.AddMonths(1);

                    var user = UserSystem.GetById(userId); //(int.Parse(userId));

                    if (user != null)
                    {
                        var payService = new PayService(user);

                        payService.MakePay();
                    }

                    return View(); // content for user
                }
            }
            catch (Exception) { }

            return RedirectToAction("Fail");
        }

        public ActionResult Fail()
        {
            ViewBag.Email = this.Request.Url.ToString();

            return View();
        }

        public ActionResult Payresult(RobokassaConfirmationRequest confirmationRequest)
        {
            try
            {
                if (confirmationRequest.IsQueryValid(RobokassaQueryType.SuccessURL))
                {
                    var userId = int.Parse(confirmationRequest.Shp_item);//GetPrm("Email");

                    var dateBegin = DateTime.Now;
                    var dateEnd = DateTime.Now.AddMonths(1);

                    var user = UserSystem.GetById(userId); //(int.Parse(userId));

                    if (user != null)
                    {
                        var payService = new PayService(user);

                        payService.MakePay();
                    }

                    return View(); // content for user
                }
            }
            catch (Exception) { }

            return RedirectToAction("Fail");
        }

        private bool IsValid()
        {
            string sMrchPass2 = "my-tend2016";

            // HTTP parameters
            string sOutSum = GetPrm("OutSum");
            string sInvId = GetPrm("InvId");
            string Shp_item = GetPrm("Shp_item");
            string sCrc = GetPrm("SignatureValue");

            string sCrcBase = string.Format("{0}:{1}:{2}:Shp_item={3}",
                                             sOutSum, sInvId, sMrchPass2, Shp_item);

            // build own CRC
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] bSignature = md5.ComputeHash(Encoding.ASCII.GetBytes(sCrcBase));

            StringBuilder sbSignature = new StringBuilder();
            foreach (byte b in bSignature)
                sbSignature.AppendFormat("{0:x2}", b);

            string sMyCrc = sbSignature.ToString();

            if (sMyCrc.ToUpper() != sCrc.ToUpper())
            {
                return false;
            }

            return true;
        }

        private string GetPrm(string sName)
        {
            string sValue = string.Empty;

            if ((this.Request.Params[sName] as string) != null)
                return this.Request.Params[sName] as string;

            if ((this.Request[sName] as string) != null)
                return this.Request[sName] as string;

            if ((this.Request.QueryString[sName] as string) != null)
                return this.Request.QueryString[sName] as string;

            return sValue;
        }
    }
}