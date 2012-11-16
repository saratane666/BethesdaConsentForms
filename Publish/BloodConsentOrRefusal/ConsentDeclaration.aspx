﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true"
    CodeBehind="ConsentDeclaration.aspx.cs" Inherits="WindowsCEConsentForms.BloodConsentOrRefusal.Consent" %>

<%@ Register TagPrefix="uc1" TagName="PatientDetails" Src="~/PatientDetails.ascx" %>
<%@ Register TagPrefix="uc1" TagName="DoctorsAndProcedures" Src="~/DoctorsAndProcedures.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <ul class="content">
        <li class="center">
            <h3>
                Blood Transfusion Consent</h3>
        </li>
        <li class="center">
            <h3>
                CONSENT FOR BLOOD TRANSFUSION</h3>
        </li>
    </ul>
    <uc1:PatientDetails ID="PatientDetails1" runat="server" />
    <uc1:DoctorsAndProcedures ID="DoctorsAndProcedures1" runat="server" />
    <ul class="content noBorder">
        <li>
            <div class="small-content">
                In the course of your treatment, your physician has determined that it is necessary
                to administer a transfusion of blood or blood products. This form provides basic
                information concerning this procedure, and if signed by you, authorizes its performance
                by qualified medical personnel.
            </div>
            <div class="small-content">
                <br />
                <span class="small-header">DESCRIPTION OF PROCEDURE</span><br />
                Blood is introduced into one of your veins, commonly in the arm, using a sterilized
                disposable needle. The amount of blood transfused and whether the transfusion will
                be of blood, blood components or blood products, such as plasma, is a judgment the
                physician will make based on your particular needs.
            </div>
        </li>
        <li>
            <div class="small-content">
                <span class="small-header">RISKS:</span>
                <br />
                • A transfusion is a common procedure of low risk.<br />
                • Minor and temporary reactions are not uncommon, including bruising, or local reaction
                in the area where the needle pierces your skin or a non serious reaction to the
                transfused material itself, including headache, fever or mild reaction such as skin
                rash.<br />
                • A serious reaction is possible, but unlikely since all blood is carefully matched
                prior to transfusion, except in life-threatening emergencies.<br />
                • Infectious diseases, which are known to be transmittable by blood, include Transfusion
                Associated Viral Hepatitis (TAVH), a viral infection of the liver and Acquired Immunodeficiency
                Syndrome (AIDS). The risk of acquiring an Infectious Disease from transfused blood
                is relatively low and blood units are tested to avoid TAVH and HIV as required by
                state and feral standards. However, these laboratory tests are not foolproof.<br />
            </div>
        </li>
        <li>
            <div class="small-content">
                <span class="small-header">BENEFITS/ALTERNATIVES</span>
                <div class="small-content">
                    • The loss of blood can pose serious threats during the course of treatment for
                    which there is no effective alternative to blood transfusion. If you have any further
                    questions on this matter, your physician or his/her colleagues will explain the
                    alternatives to you if this has not already been done.
                </div>
            </div>
        </li>
        <li>
            <div class="small-content">
                <span class="small-header">BENEFITS/ALTERNATIVES</span>
                <div class="small-content">
                    • The loss of blood can pose serious threats during the course of treatment for
                    which there is no effective alternative to blood transfusion. If you have any further
                    questions on this matter, your physician or his/her colleagues will explain the
                    alternatives to you if this has not already been done.</div>
            </div>
        </li>
        <li>
            <script language="javascript" type="text/javascript">
                $(function () {
                    $("input[id$='RdoStatementOfConsentAccepted']").click(function () {
                        $('#ListElementRefusal').hide();
                        $('#ListElementAccepted').show();
                        $("input[id$='RdoStatementOfConsentRefusal']").attr('checked', false);
                    });
                    $("input[id$='RdoStatementOfConsentRefusal']").click(function () {
                        $('#ListElementAccepted').hide();
                        $('#ListElementRefusal').show();
                        $("input[id$='RdoStatementOfConsentAccepted']").attr('checked', false);
                    });
                })
            </script>
            <table class="noBorder">
                <tr>
                    <td>
                        <asp:RadioButton runat="server" ID="RdoStatementOfConsentAccepted" Text="Statement of Consent"
                            CssClass="leftPush" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:RadioButton runat="server" ID="RdoStatementOfConsentRefusal" Text="Statement of Refused"
                            CssClass="leftPush" />
                    </td>
                </tr>
            </table>
        </li>
        <li id="ListElementAccepted" class="Hide">
            <ul class="noBorder">
                <li class="center">
                    <h3>
                        STATEMENT OF CONSENT</h3>
                </li>
                <li>
                    <div class="small-content">
                        I have read or had read to me, the above. I do not have any questions, which have
                        not been answered to my full satisfaction. I hereby consent to such transfusion,
                        as my physician may deem necessary or advisable in the course of my treatment.<br />
                        <table class="noBorder">
                            <tr>
                                <td>
                                    <asp:CheckBox runat="server" ID="Chk1" Text="I have Directed Units" CssClass="leftPush" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:CheckBox runat="server" ID="CheckBox1" Text="I have Autologous Units" CssClass="leftPush" />
                                </td>
                            </tr>
                        </table>
                    </div>
                </li>
            </ul>
        </li>
        <li id="ListElementRefusal" class="Hide">
            <ul>
                <li class="center">
                    <h3>
                        STATEMENT OF REFUSAL</h3>
                </li>
                <li>
                    <div class="small-content">
                        I request that no blood or blood component be administered to me during the course
                        of this hospitalization. I hereby release my physician(s), the hospital and its
                        personnel from any responsibility whatsoever for unfavorable reactions, untoward
                        results or death due to my refusal to permit the use of blood or blood components.
                        The possible consequences of such refusal on my part have been fully explained to
                        me by a physician and I fully understand that such consequences may occur as a result
                        of my refusal.
                    </div>
                </li>
            </ul>
        </li>
        <li>
            <p>
                I Understand and that no guarantee have been made to me that this operation will
                improve my condition.
            </p>
        </li>
        <li>
            <div>
                <asp:CheckBox runat="server" ID="ChkPatientisUnableToSign" Text="Patient is unable to sign?"
                    AutoPostBack="True" OnCheckedChanged="ChkPatientisUnableToSign_CheckedChanged" />
            </div>
            <!--
            <div id="TxtSignature1" class="signature" hdfld="HdnImage1">
            </div>
            <div class="clear">
            </div>
            <asp:HiddenField runat="server" ID="HdnImage1" /> -->
        </li>
        <li class="PatientReason">
            <asp:Panel runat="server" ID="PnlPatientReason1">
                Please specify reason
                <br />
                <asp:TextBox runat="server" ID="TxtPatientNotSignedBecause"></asp:TextBox>
            </asp:Panel>
        </li>
        <li class="PatientReason">
            <asp:Panel runat="server" ID="PnlPatientReason2">
                <div>
                    If patient is unable to sign/person authorized to sign consent / relationship to
                    patient.</div>
                <div class="sig1 sigWrapper">
                    <canvas class="pad" width="198" height="55"></canvas>
                    <input type="hidden" name="HdnImage1" class="HdnImage1" value='<%= ViewState["Signature1"].ToString() %>' />
                </div>
                <div class="clear">
                </div>
            </asp:Panel>
        </li>
        <li class="PatientSign">
            <asp:Panel runat="server" ID="PnlPatientSign">
                <div>
                    Patient Signature</div>
                <div class="sig2 sigWrapper">
                    <canvas class="pad" width="198" height="55"></canvas>
                    <input type="hidden" name="HdnImage2" class="HdnImage2" value='<%= ViewState["Signature2"].ToString() %>' />
                </div>
                <div class="clear">
                </div>
            </asp:Panel>
        </li>
        <li>
            <div>
                Witness To Signature Only</div>
            <div class="sig4 sigWrapper">
                <canvas class="pad" width="198" height="55"></canvas>
                <input type="hidden" name="HdnImage4" class="HdnImage4" value='<%= ViewState["Signature4"].ToString() %>' />
            </div>
            <div class="clear">
            </div>
        </li>
        <li>
            <asp:Panel runat="server" ID="PnlAdditionalwitness">
                <div>
                    Witness To signature
                </div>
                <div class="sig5 sigWrapper">
                    <canvas class="pad" width="198" height="55"></canvas>
                    <input type="hidden" name="HdnImage5" class="HdnImage5" value='<%= ViewState["Signature5"].ToString() %>' />
                </div>
                <div class="clear">
                </div>
            </asp:Panel>
        </li>
        <li>
            <asp:Label runat="server" ID="LblError" CssClass="errorInfo"></asp:Label>
        </li>
        <li class="center">
            <asp:Button runat="server" ID="BtnCompleted" Text="Complete" OnClick="BtnCompleted_Click"
                OnClientClick="javascript: return confirm('Are you sure that do you want to complete the form?');" />
        </li>
    </ul>
</asp:Content>