﻿<%@ Page Title="Individual Student's Chapter And Question Wise DMG Analysis Report"
    Language="C#" MasterPageFile="~/QuestionMOD/QuestionBankSubSiteMaster.master" AutoEventWireup="true"
    CodeFile="Individual_Students_Chapter_Question_Wise_DMG_Analysis.aspx.cs" Inherits="Individual_Students_Chapter_Question_Wise_DMG_Analysis" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    '
    <script src="//ajax.googleapis.com/ajax/libs/jquery/1.11.1/jquery.min.js"></script>
    <script type="text/javascript">
        function display1() {
            document.getElementById('<%#lbl_norec1.ClientID %>').innerHTML = "";
        }
    </script>
    <script type="text/javascript">
        function PrintPanel() {
            var panel = document.getElementById("<%#divMainContent.ClientID %>");
            var printWindow = window.open('', '', 'height=842,width=1100');
            printWindow.document.write('<html');
            printWindow.document.write('<head>');
            printWindow.document.write('</head><body>');
            printWindow.document.write('<form>');
            printWindow.document.write(panel.innerHTML);
            printWindow.document.write(' </form>');
            printWindow.document.write('</body></html>');
            printWindow.document.close();
            setTimeout(function () {
                printWindow.print();
            }, 500);
            return false;
        }
    </script>
    <script type="text/javascript">
        function SetContextKey() {
            var count = $get("<%#ddlSec.ClientID %>").Count;

            var qry = "";

            var coll_code = $get("<%#ddlCollege.ClientID %>").value;
            var batch = $get("<%#ddlBatch.ClientID %>").value;
            var degreeCode = $get("<%#ddlBranch.ClientID %>").value;
            var sem = $get("<%#ddlSem.ClientID %>").value;
            var sec = $get("<%#ddlSec.ClientID %>").value;

            if (coll_code != "") {
                qry = " and college_code='" + coll_code + "'";
            }
            if (batch != "") {
                qry += " and Batch_Year='" + batch + "'";
            }
            if (degreeCode != "") {
                qry += " and degree_code='" + degreeCode + "'";
            }
            if (sem != "") {
                qry += " and Current_Semester='" + sem + "'";
            }
            if (sec != "") {
                qry += " and sections='" + sec + "'";
            }

            $find('<%#autoCmpExtRollNo.ClientID%>').set_contextKey(qry);
            ////            var sec = "";
            //            $find('<%#autoCmpExtRollNo.ClientID%>').set_contextKey(" and college_code='" + $get("<%#ddlCollege.ClientID %>").value + "' and Batch_Year='" + $get("<%#ddlBatch.ClientID %>").value + "' and degree_code='" + $get("<%#ddlBranch.ClientID %>").value + "' " + sec);
            //            $find('<%#autoCmpExtRollNo.ClientID%>').set_contextKey($get("<%#ddlBatch.ClientID %>").value);
            //            $find('<%#autoCmpExtRollNo.ClientID%>').set_contextKey($get("<%#ddlBranch.ClientID %>").value);
            //            $find('<%#autoCmpExtRollNo.ClientID%>').set_contextKey($get("<%#ddlSem.ClientID %>").value);
            //            $find('<%#autoCmpExtRollNo.ClientID%>').set_contextKey($get("<%#ddlSec.ClientID %>").value);

        }
    </script>
    <style type="text/css">
        body
        {
            font-family: Book Antiqua;
            height: auto;
            background-color: #ffffff;
            color: Black;
        }
        .Chartdiv
        {
            background-color: #ffffff;
            margin: 0px;
            color: #000000;
            position: relative;
            font-family: Book Antiqua;
            height: auto;
            width: 100%;
        }
        .txtSearchAuto
        {
            width: auto;
            height: auto;
        }
        .notshow
        {
            display: none;
            margin: 0px;
        }
        .Header
        {
            font-weight: bold;
            text-align: center;
            font-size: 22px;
            color: Green;
            margin-top: 20px;
            margin-bottom: 20px;
            line-height: 3em;
        }
        .fontCommon
        {
            font-family: Book Antiqua;
            font-size: medium;
            font-weight: bold;
            color: #000000;
        }
        .defaultHeight
        {
            width: auto;
            height: auto;
        }
    </style>
    <style tyle="text/css">
        .printclass
        {
            display: none;
        }
        @media print
        {
            #divMainContent
            {
                display: block;
            }
            .printclass
            {
                display: block;
                font-family: Book Antiqua;
            }
            .noprint
            {
                display: none;
            }
            #divChapterWiseDMG
            {
                display: block;
            }
            #FpSpreadChapterWiseDMG, FpSpreadChapterWiseDMG_viewport
            {
                display: block;
            }
        
        }
        @media screen,print
        {
        
        }
        @page
        {
            size: A4;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <center>
        <div style="width: 100%; height: auto;">
            <table>
                <thead>
                    <tr>
                        <td colspan="3">
                            <center>
                                <span class="Header">Individual Student's Chapter And Question Wise DMG Analysis Report</span>
                            </center>
                        </td>
                    </tr>
                </thead>
            </table>
            <center>
                <div id="divSearch" runat="server" visible="true" style="width: 100%; height: auto;">
                    <table id="tblSearch" style="background-color: #0ca6ca; border: 1px solid #ccc; border-radius: 10px;
                        box-shadow: 0 0 8px #999999; height: auto; margin-left: 0px; margin-top: 8px;
                        padding: 1em; margin-left: 0px; width: 930px;">
                        <tr>
                            <td colspan="14" align="center">
                                <%-- <fieldset style="width: 200px; height: 20px; margin: 0px; position: relative; background-color: #ffccff;
                            border-radius: 10px; border-color: #6699ee; overflow: auto;">--%>
                                <table style="width: auto; margin: 0px; position: relative; background-color: #ffccff;
                                    border-radius: 10px; border-color: #6699ee;">
                                    <tr>
                                        <td>
                                            <asp:RadioButtonList ID="rblMultiSingleSelective" runat="server" Font-Bold="true"
                                                Font-Names="Book Antiqua" AutoPostBack="true" RepeatDirection="Horizontal" RepeatLayout="Table"
                                                OnSelectedIndexChanged="rblMultiSingleSelective_SelectedIndexChanged" Style="width: auto;
                                                height: 20px; margin: 0px; background-color: #ffccff; border-radius: 10px; border-color: #6699ee;">
                                                <asp:ListItem Selected="True" Text="All Students" Value="0"></asp:ListItem>
                                                <asp:ListItem Text="Individual Students" Value="1"></asp:ListItem>
                                                <asp:ListItem Text="Selective Students" Value="2"></asp:ListItem>
                                            </asp:RadioButtonList>
                                        </td>
                                    </tr>
                                </table>
                                </<%--fieldset--%>>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="lblCollege" runat="server" Text="College" Font-Bold="True" Style="font-family: 'Book Antiqua';"
                                    ForeColor="Black" Font-Size="Medium" Font-Names="Book Antiqua"></asp:Label>
                            </td>
                            <td>
                                <asp:DropDownList ID="ddlCollege" runat="server" CssClass="dropdown" Style="font-family: 'Book Antiqua';"
                                    Font-Bold="True" Font-Names="Book Antiqua" Font-Size="Medium" Width="150px" AutoPostBack="True"
                                    OnSelectedIndexChanged="ddlCollege_SelectedIndexChanged">
                                </asp:DropDownList>
                            </td>
                            <td>
                                <asp:Label ID="lblBatch" Font-Bold="True" Font-Names="Book Antiqua" Font-Size="Medium"
                                    runat="server" Text="Batch"></asp:Label>
                            </td>
                            <td>
                                <asp:DropDownList ID="ddlBatch" runat="server" Font-Bold="True" Font-Size="Medium"
                                    Font-Names="Book Antiqua" OnSelectedIndexChanged="ddlBatch_SelectedIndexChanged"
                                    AutoPostBack="true" Width="100px">
                                </asp:DropDownList>
                            </td>
                            <td>
                                <asp:Label ID="lblDegree" runat="server" Text="Degree" Font-Bold="True" ForeColor="Black"
                                    Font-Size="Medium" Font-Names="Book Antiqua"></asp:Label>
                            </td>
                            <td>
                                <asp:DropDownList ID="ddlDegree" runat="server" Font-Bold="True" Font-Size="Medium"
                                    Font-Names="Book Antiqua" OnSelectedIndexChanged="ddlDegree_SelectedIndexChanged"
                                    AutoPostBack="true" Width="100px">
                                </asp:DropDownList>
                            </td>
                            <td>
                                <asp:Label ID="lblBranch" runat="server" Text="Branch" Font-Bold="True" ForeColor="Black"
                                    Font-Size="Medium" Font-Names="Book Antiqua" AutoPostBack="true"></asp:Label>
                            </td>
                            <td>
                                <asp:DropDownList ID="ddlBranch" runat="server" Font-Bold="True" Font-Size="Medium"
                                    Font-Names="Book Antiqua" OnSelectedIndexChanged="ddlBranch_SelectedIndexChanged"
                                    AutoPostBack="true" Width="150px">
                                </asp:DropDownList>
                            </td>
                            <td>
                                <asp:Label ID="lblSem" runat="server" Text="Sem" Font-Bold="True" ForeColor="Black"
                                    Font-Size="Medium" Font-Names="Book Antiqua"></asp:Label>
                            </td>
                            <td>
                                <asp:DropDownList ID="ddlSem" runat="server" Font-Bold="True" Font-Size="Medium"
                                    Font-Names="Book Antiqua" OnSelectedIndexChanged="ddlSem_SelectedIndexChanged"
                                    AutoPostBack="true" Width="50px">
                                </asp:DropDownList>
                            </td>
                            <td>
                                <asp:Label ID="lblSec" runat="server" Text="Sec" Font-Bold="True" ForeColor="Black"
                                    Font-Size="Medium" Font-Names="Book Antiqua"></asp:Label>
                            </td>
                            <td>
                                <asp:DropDownList ID="ddlSec" runat="server" Font-Bold="True" Font-Size="Medium"
                                    Font-Names="Book Antiqua" OnSelectedIndexChanged="ddlSec_SelectedIndexChanged"
                                    AutoPostBack="true" Width="50px">
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="14">
                                <table id="tblRow2" runat="server">
                                    <tr>
                                        <td>
                                            <asp:Label ID="lblSubject" runat="server" Text="Subject" Font-Bold="True" ForeColor="Black"
                                                Font-Size="Medium" Font-Names="Book Antiqua"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="ddlSubject" runat="server" Font-Bold="True" Font-Size="Medium"
                                                Font-Names="Book Antiqua" AutoPostBack="true" Width="150px" OnSelectedIndexChanged="ddlSubject_Selectchanged">
                                            </asp:DropDownList>
                                        </td>
                                        <td>
                                            <asp:Label ID="lblTest" runat="server" Text="Test" Font-Bold="True" ForeColor="Black"
                                                Font-Size="Medium" Font-Names="Book Antiqua"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="ddlTest" runat="server" Font-Bold="True" Font-Size="Medium"
                                                Font-Names="Book Antiqua" AutoPostBack="true" Width="150px" OnSelectedIndexChanged="ddlTest_Selectchanged">
                                            </asp:DropDownList>
                                        </td>
                                        <td style="display: none;">
                                            <asp:Label ID="lblQuestions" runat="server" Text="Questions" Font-Bold="True" ForeColor="Black"
                                                Font-Size="Medium" Font-Names="Book Antiqua"></asp:Label>
                                        </td>
                                        <td style="display: none;">
                                            <asp:UpdatePanel ID="UpnlQuestions" runat="server">
                                                <ContentTemplate>
                                                    <asp:TextBox ID="txtQuestions" Width=" 139px" runat="server" Font-Bold="True" Font-Names="Book Antiqua"
                                                        Font-Size="Medium" CssClass="textbox  txtheight2" ReadOnly="true">-- Select --</asp:TextBox>
                                                    <asp:Panel ID="pnlQuestions" runat="server" CssClass="multxtpanel" Height="200px"
                                                        Width="250px">
                                                        <asp:CheckBox ID="chkQuestions" Font-Bold="True" Font-Names="Book Antiqua" Font-Size="Medium"
                                                            runat="server" Text="Select All" AutoPostBack="True" OnCheckedChanged="chkQuestions_CheckedChanged" />
                                                        <asp:CheckBoxList ID="cblQuestions" Font-Bold="True" Font-Size="Medium" Font-Names="Book Antiqua"
                                                            runat="server" AutoPostBack="True" OnSelectedIndexChanged="cblQuestions_SelectedIndexChanged">
                                                        </asp:CheckBoxList>
                                                    </asp:Panel>
                                                    <asp:PopupControlExtender ID="pupExtQues" runat="server" TargetControlID="txtQuestions"
                                                        PopupControlID="pnlQuestions" Position="Bottom">
                                                    </asp:PopupControlExtender>
                                                </ContentTemplate>
                                            </asp:UpdatePanel>
                                        </td>
                                        <%--<div id="divRollNo" runat="server" >--%>
                                        <td id="RollNo" runat="server" class="notshow">
                                            <asp:Label ID="lblRollNo" runat="server" Text="Roll No" Font-Bold="true" Font-Names="Book Antiqua"
                                                Font-Size="Medium"></asp:Label>
                                        </td>
                                        <td id="typeRollNo" runat="server" class="notshow">
                                            <asp:TextBox ID="txtRollNo" runat="server" AutoPostBack="true" OnTextChanged="txtRollNo_OnTextChanged"
                                                CssClass="textbox txtheight2" onkeyup="SetContextKey()" Style="font-weight: bold;
                                                width: 100px; font-family: book antiqua; font-size: medium; margin-left: 0px;"></asp:TextBox>
                                            <asp:AutoCompleteExtender ID="autoCmpExtRollNo" runat="server" DelimiterCharacters=","
                                                Enabled="True" ServiceMethod="NewGetRollNo" MinimumPrefixLength="0" CompletionInterval="100"
                                                EnableCaching="false" CompletionSetCount="10" ServicePath="" TargetControlID="txtRollNo"
                                                CompletionListCssClass="autocomplete_completionListElement" CompletionListHighlightedItemCssClass="autocomplete_highlightedListItem"
                                                UseContextKey="true" CompletionListItemCssClass="txtSearchAuto">
                                            </asp:AutoCompleteExtender>
                                        </td>
                                        <%--</div>--%>
                                        <td id="tdSelRollNo" runat="server" class="notshow">
                                            <asp:Label ID="lblSelRollNo" runat="server" Text="Roll No" Font-Bold="True" ForeColor="Black"
                                                Font-Size="Medium" Font-Names="Book Antiqua"></asp:Label>
                                        </td>
                                        <td id="tdSelRollNo1" runat="server" class="notshow">
                                            <asp:UpdatePanel ID="upnlSelRollNo" runat="server">
                                                <ContentTemplate>
                                                    <asp:TextBox ID="txtSelRollNo" Width=" 100px" ReadOnly="true" Font-Bold="True" ForeColor="Black"
                                                        runat="server" CssClass="textbox  txtheight2">--Select--</asp:TextBox>
                                                    <asp:Panel ID="pnlSelRollNo" runat="server" CssClass="multxtpanel" Height="200px"
                                                        Width="250px">
                                                        <asp:CheckBox ID="chkSelRollNo" runat="server" Text="Select All" AutoPostBack="True"
                                                            OnCheckedChanged="chkSelRollNo_CheckedChanged" />
                                                        <asp:CheckBoxList ID="cblSelRollNo" runat="server" AutoPostBack="True" OnSelectedIndexChanged="cblSelRollNo_SelectedIndexChanged">
                                                        </asp:CheckBoxList>
                                                    </asp:Panel>
                                                    <asp:PopupControlExtender ID="popExtSelRollNo" runat="server" TargetControlID="txtSelRollNo"
                                                        PopupControlID="pnlSelRollNo" Position="Bottom">
                                                    </asp:PopupControlExtender>
                                                </ContentTemplate>
                                            </asp:UpdatePanel>
                                        </td>
                                        <td>
                                            <asp:Button ID="btnGo" runat="server" Font-Bold="True" Font-Size="Medium" Font-Names="Book Antiqua"
                                                Width="59px" CssClass="textbox defaultHeight" Text="Go" OnClick="btnGo_Click" />
                                        </td>
                                        <td colspan="3">
                                            <asp:CheckBox ID="chkShowSelQuestions" runat="server" Font-Bold="True" Font-Size="Medium"
                                                Font-Names="Book Antiqua" Text="Show Questions" Checked="false" AutoPostBack="true"
                                                OnCheckedChanged="chkShowSelQuestions_CheckedChanged" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </div>
            </center>
            <br />
            <asp:Label ID="lblErrSearch" runat="server" Text="" ForeColor="Red" Visible="False"
                Font-Bold="True" Font-Names="Book Antiqua" Font-Size="Medium"></asp:Label>
            <center>
                <div id="divMainContent" runat="server" class="Chartdiv" visible="false">
                    <center>
                        <asp:Label ID="spancollname" runat="server" class="printclass" Style="text-align: center;
                            font-weight: bolder;">
                        </asp:Label>
                        <br />
                        <asp:Label ID="spanaddr" runat="server" class="printclass" Style="text-align: center;
                            font-weight: bolder;">
                        </asp:Label>
                        <br />
                        <asp:Label ID="spandegdetails" runat="server" class="printclass" Style="text-align: center;
                            font-weight: bolder;">
                        </asp:Label>
                        <br />
                        <asp:Label ID="spanTitle" runat="server" class="printclass" Style="text-align: center;
                            font-weight: bolder;">
        Individual Student's Chapter And Question Wise DMG Analysis Report
                        </asp:Label>
                    </center>
                    <br />
                    <asp:Label ID="spanSub" runat="server" class="printclass" Style="text-align: left;
                        font-weight: bolder;">
                    </asp:Label>
                    <br />
                    <div id="divChapterWiseDMG" runat="server" class="Chartdiv" visible="false">
                        <FarPoint:FpSpread ID="FpSpreadChapterWiseDMG" AutoPostBack="false" runat="server"
                            Visible="true" BorderStyle="Solid" BorderWidth="0px" ShowHeaderSelection="false"
                            Style="width: 100%; height: auto; display: table; border: 0px solid #000000;"
                            CssClass="Chartdiv">
                            <Sheets>
                                <FarPoint:SheetView SheetName="Sheet1">
                                </FarPoint:SheetView>
                            </Sheets>
                        </FarPoint:FpSpread>
                    </div>
                    <br />
                    <div id="divQuestionWiseDMG" runat="server" visible="false">
                        <FarPoint:FpSpread ID="FpSpreadQuestionWiseDMG" AutoPostBack="false" runat="server"
                            Visible="false" BorderStyle="Solid" BorderWidth="1px" ShowHeaderSelection="false"
                            Style="width: 100%; height: auto;">
                            <Sheets>
                                <FarPoint:SheetView SheetName="Sheet1">
                                </FarPoint:SheetView>
                            </Sheets>
                        </FarPoint:FpSpread>
                    </div>
                    <br />
                    <div id="divMainChart" runat="server" visible="false" class="Chartdiv" style="display: table-row;
                        border: 1px solid #000000; width: 100%; height: auto">
                        <%-- width:1000px; height:auto;"--%>
                        <div id="divChartChapterWiseDMG" runat="server" style="display: table-cell; width: 50%;
                            height: auto">
                            <asp:Panel ID="plhChapterWise" runat="server" Style="display: table-cell; width: 80%;
                                height: auto">
                            </asp:Panel>
                        </div>
                        <div id="divChartQuestionWiseDMG" runat="server" style="display: table-cell; width: 50%;
                            height: auto">
                            <asp:Panel ID="plhQuestionWise" runat="server" Style="display: table-cell; width: 80%;
                                height: auto">
                            </asp:Panel>
                        </div>
                    </div>
                    <asp:Panel ID="pnlQuestions_DMG" runat="server" Visible="false" Style="display: table;
                        width: 80%; height: auto">
                    </asp:Panel>
                </div>
                <center>
                    <div id="rptprint1" class="noprint" runat="server" visible="false">
                        <br />
                        <asp:Label ID="lbl_norec1" runat="server" Font-Bold="True" Font-Names="Book Antiqua"
                            Font-Size="Medium" ForeColor="Red" Text="Please Enter Your Report Name" Visible="false"></asp:Label>
                        <asp:Label ID="lblrptname1" runat="server" Font-Bold="True" Font-Names="Book Antiqua"
                            Font-Size="Medium" Text="Report Name"></asp:Label>
                        <asp:TextBox ID="txtexcelname1" runat="server" CssClass="textbox textbox1" Height="20px"
                            Width="180px" Style="font-family: 'Book Antiqua'" Font-Bold="True" Font-Names="Book Antiqua"
                            onkeypress="display1()" Font-Size="Medium"></asp:TextBox>
                        <asp:FilteredTextBoxExtender ID="FilteredTextBoxExtender9" runat="server" TargetControlID="txtexcelname1"
                            FilterType="LowercaseLetters,UppercaseLetters,Numbers,Custom" ValidChars="(),.[]_"
                            InvalidChars="/\">
                        </asp:FilteredTextBoxExtender>
                        <asp:Button ID="btnExcel1" runat="server" Font-Bold="true" Font-Names="Book Antiqua"
                            OnClick="btnExcel1_Click" Font-Size="Medium" Text="Export To Excel" Width="127px"
                            Height="35px" CssClass="textbox textbox1" />
                        <asp:Button ID="btnprintmaster1" runat="server" Text="Print" OnClick="btnprintmaster1_Click"
                            Font-Names="Book Antiqua" Font-Size="Medium" Font-Bold="true" Width="60px" Height="35px"
                            CssClass="textbox textbox1" />
                        <Insproplus:printmaster runat="server" ID="Printcontrol1" Visible="false" />
                        <asp:Button ID="btnPrint" runat="server" Text="Direct Print" OnClientClick="return PrintPanel();"
                            Font-Names="Book Antiqua" Font-Size="Medium" Font-Bold="true" Height="35px" CssClass="textbox textbox1" />
                    </div>
                </center>
            </center>
            <%-- </fieldset>
        </center>--%>
        </div>
        <div id="popupdiv" runat="server" visible="false" style="height: 100%; z-index: 1000;
            width: 100%; background-color: rgba(54, 25, 25, .2); position: absolute; top: 0;
            left: 0px;">
            <center>
                <div id="pnl2" runat="server" class="table" style="background-color: White; height: 120px;
                    width: 238px; border: 5px solid #0CA6CA; border-top: 25px solid #0CA6CA; margin-top: 200px;
                    border-radius: 10px;">
                    <center>
                        <table style="height: 100px; width: 100%">
                            <tr>
                                <td align="center">
                                    <asp:Label ID="lblpopuperr" runat="server" Text="" Style="color: Red;" Font-Bold="true"
                                        Font-Size="Medium" Font-Names="Book Antiqua"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <center>
                                        <asp:Button ID="btn_errorclose" runat="server" CssClass=" textbox btn1 comm" Font-Size="Medium"
                                            Font-Bold="True" Font-Names="Book Antiqua" Style="height: 28px; width: 65px;"
                                            OnClick="btn_errorclose_Click" Text="Ok" />
                                    </center>
                                </td>
                            </tr>
                        </table>
                    </center>
                </div>
            </center>
        </div>
        <div id="divShowQuestions" runat="server" visible="false" style="height: 100%; z-index: 1000;
            width: auto; background-color: rgba(54, 25, 25, .2); position: absolute; top: 0;
            left: 0px;">
            <center>
                <asp:ImageButton ID="imgbtnClose" runat="server" Width="40px" Height="40px" ImageUrl="~/images/close.png"
                    Style="height: 30px; width: 30px; position: relative; top: 0%; left: 0%" OnClick="imgbtnClose_OnClick" />
                <div id="divGridQuestions" runat="server" class="table" style="background-color: White;
                    height: auto; width: auto; border: 5px solid #0CA6CA; border-top: 25px solid #0CA6CA;
                    border-radius: 10px; position: relative;">
                    <center>
                        <table style="margin: 0px;">
                            <tr>
                                <td colspan="2">
                                    <FarPoint:FpSpread ID="FpShowQuestions" AutoPostBack="false" runat="server" Visible="true"
                                        BorderStyle="Solid" BorderWidth="0px" ShowHeaderSelection="false" OnUpdateCommand="FpShowQuestions_UpdateCommand"
                                        Style="width: 100%; height: auto; display: table; border: 0px solid #000000;"
                                        CssClass="Chartdiv">
                                        <Sheets>
                                            <FarPoint:SheetView SheetName="Sheet1">
                                            </FarPoint:SheetView>
                                        </Sheets>
                                    </FarPoint:FpSpread>
                                </td>
                            </tr>
                            <tr>
                                <td align="center" colspan="2">
                                    <asp:Button ID="btnShowQuesGo" runat="server" Font-Bold="True" Font-Size="Medium"
                                        Font-Names="Book Antiqua" Width="59px" CssClass="textbox defaultHeight" Text="Go"
                                        OnClick="btnGo_Click" />
                                    <asp:Button ID="btnExit" runat="server" Font-Bold="True" Font-Size="Medium" Font-Names="Book Antiqua"
                                        CssClass="textbox defaultHeight" Text="Exit" OnClick="btnExit_Click" />
                                </td>
                            </tr>
                        </table>
                    </center>
                </div>
            </center>
        </div>
        </div>
    </center>
</asp:Content>
