﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using WindowsCEConsentForms.ConsentFormSvc;

namespace WindowsCEConsentForms.PICC
{
    public partial class PICCConsentDeclaration : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            DeclarationSignatures.BtnCompleted.Click += BtnCompleted_Click;
            DeclarationSignatures.BtnReset.Click += BtnReset_Click;

            if (IsPostBack) return;
            try
            {
                HdnPatientId.Value = Session["PatientID"].ToString();
            }
            catch (Exception)
            {
                Response.Redirect("/PatientConsent.aspx");
            }
            try
            {
                HdnLocation.Value = Session["Location"].ToString();
            }
            catch (Exception)
            {
                Response.Redirect("/PatientConsent.aspx");
            }
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            DeclarationSignatures.ResetSignatures();
        }

        protected void BtnCompleted_Click(object sender, EventArgs e)
        {
            try
            {
                //validation

                const ConsentType consentType = ConsentType.PICC;

                DeclarationSignatures.LblError.Text = string.Empty;

                DeclarationSignatures.ValidateForm();

                if (!string.IsNullOrEmpty(DeclarationSignatures.LblError.Text))
                    return;

                string patientId;
                try
                {
                    patientId = Session["PatientID"].ToString();
                }
                catch (Exception)
                {
                    if (string.IsNullOrEmpty(HdnPatientId.Value))
                    {
                        Response.Redirect("/PatientConsent.aspx");
                        return;
                    }
                    patientId = HdnPatientId.Value;
                    Session["PatientID"] = patientId;
                }

                string location;
                try
                {
                    location = Session["Location"].ToString();
                }
                catch (Exception)
                {
                    if (string.IsNullOrEmpty(HdnPatientId.Value))
                    {
                        Response.Redirect("/PatientConsent.aspx");
                        return;
                    }
                    location = HdnLocation.Value;
                    Session["Location"] = location;
                }

                string ip = Request.ServerVariables["REMOTE_ADDR"];
                string device;
                if (Request.Browser.IsMobileDevice)
                    device = Request.Browser.Browser + " " + Request.Browser.Version;
                else
                    device = Request.Browser.Browser + " " + Request.Browser.Version;

                var signatureses = new List<Signatures>();

                signatureses.AddRange(DeclarationSignatures.GetSignatures());

                string empID = string.Empty;
                if (Session["EmpID"] != null)
                    empID = Session["EmpID"].ToString();

                var treatment = new Treatment
                {
                    _patientId = patientId,
                    _consentType = consentType,
                    _signatureses = signatureses.ToArray(),
                    _isPatientUnableSign = DeclarationSignatures.ChkPatientisUnableToSign.Checked,
                    _unableToSignReason = DeclarationSignatures.TxtPatientNotSignedBecause.Text,
                    _translatedBy = DeclarationSignatures.TxtTranslatedBy.Text,
                    _trackingInformation = new TrackingInformation
                    {
                        _device = device,
                        _iP = ip
                    },
                    _empID = empID
                };

                var formHandlerServiceClient = Utilities.GetConsentFormSvcClient();
                formHandlerServiceClient.AddTreatment(treatment);
                Utilities.GeneratePdfAndUploadToSharePointSite(formHandlerServiceClient, consentType, patientId, Request, location);
                try
                {
                    Response.Redirect("/PatientConsent.aspx");
                }
                catch (Exception)
                {
                }
            }
            catch (Exception ex)
            {
                var client = Utilities.GetConsentFormSvcClient();
                client.CreateLog(Utilities.GetUsername(Session), LogType.E, GetType().Name + "-" + new StackTrace().GetFrame(0).GetMethod().ToString(),
                                 ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }
    }
}