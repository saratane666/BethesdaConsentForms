﻿using System;
using System.Data;
using System.Globalization;
using WindowsCEConsentForms.ConsentFormsService;

namespace WindowsCEConsentForms
{
    public partial class ConsentPrint : System.Web.UI.UserControl
    {
        public ConsentType ConsentType;

        protected void Page_Load(object sender, EventArgs e)
        {
            string patientId;
            try
            {
                patientId = Request.QueryString["PatientID"];
            }
            catch (Exception)
            {
                patientId = string.Empty;
            }
            if (!string.IsNullOrEmpty(patientId))
            {
                var formHandlerServiceClient = new FormHandlerServiceClient();
                var patientDetails = formHandlerServiceClient.GetPatientDetail(patientId);
                if (patientDetails != null)
                {
                    var primaryDoctor = formHandlerServiceClient.GetPrimaryDoctorDetail(patientDetails.PrimaryDoctorId);
                    if (primaryDoctor != null)
                    {
                        //LblAssociatedDoctor.Text = primaryDoctor.Fname + " " + primaryDoctor.Lname;
                        LblAuthoriseDoctors.Text = primaryDoctor.Fname + " " + primaryDoctor.Lname;
                    }
                    foreach (DataRow row in formHandlerServiceClient.GetAssociatedPhysiciansList(patientDetails.PrimaryDoctorId).Rows)
                    {
                        LblAuthoriseDoctors.Text += " , " + row["Lname"].ToString().Trim() + " " + row["Fname"].ToString().Trim();
                    }

                    LblDOB.Text = DateTime.Now.ToString("MMM dd yyyy");
                    LblPatientAdminDate.Text = patientDetails.AdmDate.ToString("MMM dd yyyy");
                    LblPatientAdminTime.Text = patientDetails.AdmDate.ToLongTimeString();
                    LblPatientId.Text = patientId;
                    LblPatientMrHash.Text = patientDetails.MRHash;
                    LblPatientName.Text = patientDetails.name;
                    LblPatientName2.Text = patientDetails.name;
                    LblPatientName3.Text = patientDetails.name;
                    LblPatientUnableToSignBecause.Text = patientDetails.UnableToSignReason;
                    LblProcedureName.Text = patientDetails.ProcedureName;

                    LblPatientSignatureDateTime.Text = DateTime.Now.ToString("MMM dd yyyy") + " <br /> " + DateTime.Now.ToLongTimeString();
                    LblAuthorizedSignDateTime.Text = DateTime.Now.ToString("MMM dd yyyy") + " <br /> " + DateTime.Now.ToLongTimeString();
                    LblWitnessSignature1DateTime.Text = DateTime.Now.ToString("MMM dd yyyy") + " <br /> " + DateTime.Now.ToLongTimeString();
                    LblWitnessSignature2DateTime.Text = DateTime.Now.ToString("MMM dd yyyy") + " <br /> " + DateTime.Now.ToLongTimeString();
                    LblTranslatedDateTime.Text = DateTime.Now.ToString("MMM dd yyyy") + " <br /> " + DateTime.Now.ToLongTimeString();

                    LblDate.Text = DateTime.Now.ToString("MMM dd yyyy");
                    LblAge.Text = patientDetails.age.ToString(CultureInfo.InvariantCulture);
                    LblGender.Text = patientDetails.gender;

                    ImgSignature1.ImageUrl = "/GetImage.ashx?PatientId=" + patientId + "&Signature=" + SignatureType.DoctorSign1.ToString() + @"&ConsentType=" + ConsentType.ToString();
                    ImgSignature2.ImageUrl = "/GetImage.ashx?PatientId=" + patientId + "&Signature=" + SignatureType.DoctorSign2.ToString() + @"&ConsentType=" + ConsentType.ToString();
                    ImgSignature3.ImageUrl = "/GetImage.ashx?PatientId=" + patientId + "&Signature=" + SignatureType.DoctorSign3.ToString() + "&ConsentType=" + ConsentType.ToString();
                    ImgSignature4.ImageUrl = "/GetImage.ashx?PatientId=" + patientId + "&Signature=" + SignatureType.DoctorSign4.ToString() + "&ConsentType=" + ConsentType.ToString();
                    ImgSignature5.ImageUrl = "/GetImage.ashx?PatientId=" + patientId + "&Signature=" + SignatureType.DoctorSign5.ToString() + "&ConsentType=" + ConsentType.ToString();

                    if (!string.IsNullOrEmpty(LblPatientUnableToSignBecause.Text.Trim()))
                    {
                        PnlPatientSignature.Visible = false;
                        PnlPatientUnableToSignBecause.Visible = true;
                        PnlAuthorizedSignature.Visible = true;
                    }
                    else
                    {
                        PnlPatientSignature.Visible = true;

                        PnlPatientUnableToSignBecause.Visible = false;
                        PnlAuthorizedSignature.Visible = false;
                    }

                    ImgPatientSignature.ImageUrl = "/GetImage.ashx?PatientId=" + patientId + "&Signature=" + SignatureType.PatientSign + "&ConsentType=" + ConsentType.ToString();
                    ImgAuthorizedSignature.ImageUrl = "/GetImage.ashx?PatientId=" + patientId + "&Signature=" + SignatureType.PatientAuthorizeSign + "&ConsentType=" + ConsentType.ToString();
                    ImgWitnessSignature1.ImageUrl = "/GetImage.ashx?PatientId=" + patientId + "&Signature=" + SignatureType.WitnessSignature1 + "&ConsentType=" + ConsentType.ToString();
                    ImgWitnessSignature2.ImageUrl = "/GetImage.ashx?PatientId=" + patientId + "&Signature=" + SignatureType.WitnessSignature2 + "&ConsentType=" + ConsentType.ToString();
                    ImgTranslatedBy.ImageUrl = "/GetImage.ashx?PatientId=" + patientId + "&Signature=" + SignatureType.TranslatedBySign + "&ConsentType=" + ConsentType.ToString();
                }
            }
        }
    }
}