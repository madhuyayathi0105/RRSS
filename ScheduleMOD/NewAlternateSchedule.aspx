<%@ Page Title="" Language="C#" MasterPageFile="~/ScheduleMOD/ScheduleSubSiteMaster.master"
    AutoEventWireup="true" CodeFile="NewAlternateSchedule.aspx.cs" Inherits="ScheduleMOD_NewAlternateSchedule" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script language="javascript" type="text/javascript" src="../Scripts/jquery-1.4.1.js"></script>
    <style type="text/css">
        .GridDock
        {
            overflow-x: auto;
            overflow-y: auto;
            width: 600px;
            height: 200px;
            padding: 0 0 0 0;
        }
    </style>
    <style type="text/css">
        .style1
        {
            width: 84%;
        }
        .cursorptr
        {
            cursor: pointer;
        }
        .cursordflt
        {
            cursor: default;
        }
        .ModalPopupBG
        {
            background-color: #666699;
            filter: alpha(opacity=50);
            opacity: 0.7;
        }
        
        .HellowWorldPopup
        {
            min-width: 600px;
            min-height: 400px;
            background: white;
        }
        
        #clsbtn
        {
            height: 26px;
            width: 72px;
        }
        
        .txt
        {
        }
        
        .style8
        {
            width: 319px;
        }
        
        
        .style12
        {
            width: 660px;
        }
        
        .style13
        {
            width: 133px;
        }
        .style14
        {
            width: 92px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <center>
        <asp:Label ID="lblhead" runat="server" Text="Master Alternate Schedule Change" class="fontstyleheader"
            Style="color: #008000; font-size: x-large"></asp:Label>
    </center>
    <div>
        <asp:UpdatePanel ID="updSchedule" runat="server">
            <ContentTemplate>
                <asp:UpdateProgress ID="UpdateProgress1" runat="server" DynamicLayout="true" AssociatedUpdatePanelID="updSchedule">
                    <ProgressTemplate>
                        <div class="CenterPB" style="height: 40px; width: 40px;">
                            <img src="../images/progress2.gif" height="180px" width="180px" />
                        </div>
                    </ProgressTemplate>
                </asp:UpdateProgress>
                <asp:ModalPopupExtender ID="modalPopup" runat="server" TargetControlID="UpdateProgress1"
                    PopupControlID="UpdateProgress1">
                </asp:ModalPopupExtender>
                <center>
                    <table cellpadding="0px" cellspacing="0px" style="width: 800px; height: 70px; background-color: #0CA6CA;"
                        class="table">
                        <tr style="height: 47px;">
                            <td style="padding-left: 15px;">
                                <asp:Label runat="server" ID="lblClg" Text="College" Font-Bold="True" Font-Names="Book Antiqua"
                                    Font-Size="Medium"></asp:Label>
                            </td>
                            <td>
                                <asp:DropDownList ID="ddlCollege" runat="server" Font-Bold="True" Font-Names="Book Antiqua"
                                    Font-Size="Medium" Height="25px" Width="320px" AutoPostBack="True">
                                </asp:DropDownList>
                            </td>
                            <td>
                                <asp:Label runat="server" ID="lblbatch" Text="Batch" Font-Bold="True" Font-Names="Book Antiqua"
                                    Font-Size="Medium"></asp:Label>
                            </td>
                            <td>
                                <asp:UpdatePanel ID="UP_batch" runat="server">
                                    <ContentTemplate>
                                        <asp:TextBox ID="txt_batch" runat="server" Style="height: 20px; width: 100px;" Font-Size="Medium"
                                            Font-Bold="True" ReadOnly="true" Font-Names="Book Antiqua">--Select--</asp:TextBox>
                                        <asp:Panel ID="panel_batch" runat="server" CssClass="multxtpanel" Style="width: 121px;
                                            height: auto;">
                                            <asp:CheckBox ID="cb_batch" runat="server" Width="100px" Text="Select All" Font-Size="Medium"
                                                Font-Bold="True" Font-Names="Book Antiqua" AutoPostBack="true" OnCheckedChanged="cb_batch_OnCheckedChanged" />
                                            <asp:CheckBoxList ID="cbl_batch" runat="server" AutoPostBack="true" Font-Size="Medium"
                                                Font-Bold="True" Font-Names="Book Antiqua" OnSelectedIndexChanged="cbl_batch_OnSelectedIndexChanged">
                                            </asp:CheckBoxList>
                                        </asp:Panel>
                                        <asp:PopupControlExtender ID="pce_batch" runat="server" TargetControlID="txt_batch"
                                            PopupControlID="panel_batch" Position="Bottom">
                                        </asp:PopupControlExtender>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </td>
                            <td>
                                <asp:Label runat="server" ID="lblBranch" Text="Branch" Font-Bold="True" Font-Names="Book Antiqua"
                                    Font-Size="Medium"></asp:Label>
                            </td>
                            <td>
                                <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                    <ContentTemplate>
                                        <asp:TextBox ID="txtBranch" runat="server" Style="height: 20px; width: 100px;" Font-Size="Medium"
                                            Font-Bold="True" ReadOnly="true" Font-Names="Book Antiqua">--Select--</asp:TextBox>
                                        <asp:Panel ID="panel3" runat="server" CssClass="multxtpanel" Style="width: 350px;
                                            height: auto;">
                                            <asp:CheckBox ID="chkBranch" runat="server" Width="140px" Font-Size="Medium" Font-Bold="True"
                                                Font-Names="Book Antiqua" Text="Select All" AutoPostBack="true" OnCheckedChanged="chkBranch_OnCheckedChanged" />
                                            <asp:CheckBoxList ID="cblBranch" runat="server" Font-Size="Medium" Font-Bold="True"
                                                Font-Names="Book Antiqua" AutoPostBack="true" OnSelectedIndexChanged="cblBranch_OnSelectedIndexChanged">
                                            </asp:CheckBoxList>
                                        </asp:Panel>
                                        <asp:PopupControlExtender ID="PopupControlExtender1" runat="server" TargetControlID="txtBranch"
                                            PopupControlID="panel3" Position="Bottom">
                                        </asp:PopupControlExtender>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="7">
                                <asp:Label ID="lbldate" runat="server" Text="From Date" Font-Bold="True" Font-Names="Book Antiqua"
                                    Font-Size="Medium"></asp:Label>
                                <asp:TextBox ID="txtDate" CssClass="txt" runat="server" Height="20px" Width="79px"
                                    Font-Bold="True" Font-Names="Book Antiqua" Font-Size="Medium"></asp:TextBox><%--OnTextChanged="txtFromDate_TextChanged"--%>
                                <asp:FilteredTextBoxExtender ID="txtDate_FilteredTextBoxExtender" FilterType="Custom,Numbers"
                                    ValidChars="/" runat="server" TargetControlID="txtDate">
                                </asp:FilteredTextBoxExtender>
                                <asp:CalendarExtender ID="caldate" TargetControlID="txtDate" Format="dd/MM/yyyy"
                                    runat="server">
                                </asp:CalendarExtender>
                                <asp:Label ID="lblTodate" runat="server" Text="To Date" Font-Bold="True" Font-Names="Book Antiqua"
                                    Font-Size="Medium"></asp:Label>
                                <asp:TextBox ID="txttoDate" CssClass="txt" runat="server" Height="20px" Width="79px"
                                    Font-Bold="True" Font-Names="Book Antiqua" Font-Size="Medium"></asp:TextBox><%--OnTextChanged="txtFromDate_TextChanged"--%>
                                <asp:FilteredTextBoxExtender ID="FilteredTextBoxExtender1" FilterType="Custom,Numbers"
                                    ValidChars="/" runat="server" TargetControlID="txttoDate">
                                </asp:FilteredTextBoxExtender>
                                <asp:CalendarExtender ID="CalendarExtender1" TargetControlID="txttoDate" Format="dd/MM/yyyy"
                                    runat="server">
                                </asp:CalendarExtender>
                                <asp:Button ID="btnGo" runat="server" Text="Go" OnClick="btnGo_Click" Font-Bold="True"
                                    Font-Names="Book Antiqua" Font-Size="Medium" Height="28px" Width="48px" />
                            </td>
                        </tr>
                    </table>
                </center>
                <br />
                <table>
                    <tr>
                        <td colspan="7" style="margin-right: 10px;">
                            <div runat="server" id="subDiv">
                                <%--  <asp:Button ID="btnFreeStaffList" runat="server" Text="Free Staff List" Font-Bold="True"
                                    Font-Names="Book Antiqua" Visible="false" Font-Size="Medium" Height="28px" Width="200px" Style="margin: 14px;" />--%>
                                <asp:Button ID="btnBatchAllocation" runat="server" Text=" Batch Allocation" Font-Bold="True"
                                    OnClick="btnBatchAllocation_Click" Font-Names="Book Antiqua" Font-Size="Medium"
                                    Style="margin: 14px;" Height="28px" Width="200px" />
                                <asp:CheckBox ID="chkPerDAySched" runat="server" Width="200px" Text="As per day schedule"
                                    OnCheckedChanged="chkPerDAySched_OnCheckedChanged" AutoPostBack="true" Font-Bold="True"
                                    Font-Names="Book Antiqua" Font-Size="Medium" />
                            </div>
                        </td>
                    </tr>
                </table>
                <br />
                <center>
                    <table>
                        <tr>
                            <td>
                                <asp:Label ID="norecordlbl" runat="server" Style="margin-top: -25px; position: absolute;"
                                    Font-Bold="true" Font-Size="Medium" ForeColor="Red"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </center>
                <br />
                <%-- Main GridView--%>
                <center>
                    <div id="divTimeTable" runat="server">
                        <asp:GridView ID="gridTimeTable" runat="server" AutoGenerateColumns="false" Font-Names="Book Antiqua"
                            HeaderStyle-BackColor="#0CA6CA" BackColor="White" Font-Size="14px" OnDataBound="OnDataBound"
                            OnRowDataBound="OnRowDataBound">
                            <%-- OnDataBound="gridTimeTable_OnDataBound"--%>
                            <Columns>
                                <asp:TemplateField HeaderText="Date">
                                    <ItemTemplate>
                                        <asp:Label ID="lblDateDisp" runat="server" Text='<%#Eval("DateDisp") %>'></asp:Label>
                                        <asp:Label ID="lblDate" runat="server" Text='<%#Eval("DateVal") %>' Visible="false"></asp:Label>
                                        <asp:Label ID="lblDayVal" runat="server" Text='<%#Eval("DayOrder") %>' Visible="false"></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle Width="100px" HorizontalAlign="Center" BackColor="#47a55b" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Degree">
                                    <ItemTemplate>
                                        <asp:Label ID="lblDegree" runat="server" Text='<%#Eval("Degree") %>'></asp:Label>
                                        <asp:Label ID="lblDegreeval" runat="server" Text='<%#Eval("DegreeVal") %>' Visible="false"></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle Width="100px" HorizontalAlign="Center" BackColor="#F8B7B3" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Period 1">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lnkPeriod_1" runat="server" Text='<%#Eval("P1ValDisp") %>' ForeColor="Black"
                                            OnClick="lnkAttMark" Font-Underline="false"></asp:LinkButton>
                                        <asp:Label ID="lblPeriod_1" runat="server" Text='<%#Eval("P1Val") %>' Visible="false"></asp:Label>
                                        <asp:Label ID="lblTT_1" runat="server" Text='<%#Eval("TT_1") %>' Visible="false"></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle Width="200px" HorizontalAlign="Justify" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Period 2">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lnkPeriod_2" runat="server" Text='<%#Eval("P2ValDisp") %>' ForeColor="Black"
                                            OnClick="lnkAttMark" Font-Underline="false"></asp:LinkButton>
                                        <asp:Label ID="lblPeriod_2" runat="server" Text='<%#Eval("P2Val") %>' Visible="false"></asp:Label>
                                        <asp:Label ID="lblTT_2" runat="server" Text='<%#Eval("TT_2") %>' Visible="false"></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle Width="200px" HorizontalAlign="Justify" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Period 3">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lnkPeriod_3" runat="server" Text='<%#Eval("P3ValDisp") %>' ForeColor="Black"
                                            OnClick="lnkAttMark" Font-Underline="false"></asp:LinkButton>
                                        <asp:Label ID="lblPeriod_3" runat="server" Text='<%#Eval("P3Val") %>' Visible="false"></asp:Label>
                                        <asp:Label ID="lblTT_3" runat="server" Text='<%#Eval("TT_3") %>' Visible="false"></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle Width="200px" HorizontalAlign="Justify" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Period 4">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lnkPeriod_4" runat="server" Text='<%#Eval("P4ValDisp") %>' ForeColor="Black"
                                            OnClick="lnkAttMark" Font-Underline="false"></asp:LinkButton>
                                        <asp:Label ID="lblPeriod_4" runat="server" Text='<%#Eval("P4Val") %>' Visible="false"></asp:Label>
                                        <asp:Label ID="lblTT_4" runat="server" Text='<%#Eval("TT_4") %>' Visible="false"></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle Width="200px" HorizontalAlign="Justify" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Period 5">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lnkPeriod_5" runat="server" Text='<%#Eval("P5ValDisp") %>' ForeColor="Black"
                                            OnClick="lnkAttMark" Font-Underline="false"></asp:LinkButton>
                                        <asp:Label ID="lblPeriod_5" runat="server" Text='<%#Eval("P5Val") %>' Visible="false"></asp:Label>
                                        <asp:Label ID="lblTT_5" runat="server" Text='<%#Eval("TT_5") %>' Visible="false"></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle Width="200px" HorizontalAlign="Justify" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Period 6">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lnkPeriod_6" runat="server" Text='<%#Eval("P6ValDisp") %>' ForeColor="Black"
                                            OnClick="lnkAttMark" Font-Underline="false"></asp:LinkButton>
                                        <asp:Label ID="lblPeriod_6" runat="server" Text='<%#Eval("P6Val") %>' Visible="false"></asp:Label>
                                        <asp:Label ID="lblTT_6" runat="server" Text='<%#Eval("TT_6") %>' Visible="false"></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle Width="200px" HorizontalAlign="Justify" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Period 7">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lnkPeriod_7" runat="server" Text='<%#Eval("P7ValDisp") %>' ForeColor="Black"
                                            OnClick="lnkAttMark" Font-Underline="false"></asp:LinkButton>
                                        <asp:Label ID="lblPeriod_7" runat="server" Text='<%#Eval("P7Val") %>' Visible="false"></asp:Label>
                                        <asp:Label ID="lblTT_7" runat="server" Text='<%#Eval("TT_7") %>' Visible="false"></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle Width="200px" HorizontalAlign="Justify" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Period 8">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lnkPeriod_8" runat="server" Text='<%#Eval("P8ValDisp") %>' ForeColor="Black"
                                            OnClick="lnkAttMark" Font-Underline="false"></asp:LinkButton>
                                        <asp:Label ID="lblPeriod_8" runat="server" Text='<%#Eval("P8Val") %>' Visible="false"></asp:Label>
                                        <asp:Label ID="lblTT_8" runat="server" Text='<%#Eval("TT_8") %>' Visible="false"></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle Width="200px" HorizontalAlign="Justify" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Period 9">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lnkPeriod_9" runat="server" Text='<%#Eval("P9ValDisp") %>' ForeColor="Black"
                                            OnClick="lnkAttMark" Font-Underline="false"></asp:LinkButton>
                                        <asp:Label ID="lblPeriod_9" runat="server" Text='<%#Eval("P9Val") %>' Visible="false"></asp:Label>
                                        <asp:Label ID="lblTT_9" runat="server" Text='<%#Eval("TT_9") %>' Visible="false"></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle Width="200px" HorizontalAlign="Justify" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Period 10">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lnkPeriod_10" runat="server" Text='<%#Eval("P10ValDisp") %>'
                                            ForeColor="Blue" OnClick="lnkAttMark" Font-Underline="false"></asp:LinkButton>
                                        <asp:Label ID="lblPeriod_10" runat="server" Text='<%#Eval("P10Val") %>' Visible="false"></asp:Label>
                                        <asp:Label ID="lblTT_10" runat="server" Text='<%#Eval("TT_10") %>' Visible="false"></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle Width="200px" HorizontalAlign="Justify" />
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>
                    <asp:Button ID="btnMasterSave" runat="server" Text="Save" Font-Bold="True" Font-Names="Book Antiqua"
                        Font-Size="Medium" Height="25px" Width="75px" Style="margin-top: 15px;" OnClick="btnMasterSave_Click" />
                </center>
                <center>
                    <div id="spcellClickPopup" runat="server" visible="false" style="height: auto; z-index: 2000;
                        width: 100%; background-color: rgba(54, 25, 25, .2); position: fixed; top: 0;
                        left: 0;">
                        <asp:ImageButton ID="ImageButton2" runat="server" Width="40px" Height="40px" ImageUrl="~/images/close.png"
                            Style="height: 30px; width: 30px; position: absolute; margin-top: 38px; margin-left: 500px;"
                            OnClick="spcellClickPopupclose_Click" />
                        <br />
                        <br />
                        <div class="subdivstyle" style="background-color: White; overflow: auto; width: 1050px;
                            height: auto;" align="center">
                            <center>
                                <br />
                                <asp:Label ID="lblalter" runat="server" class="fontstyleheader" Style="color: Green;"
                                    Text="Alter Schedule"></asp:Label>
                            </center>
                            <br />
                            <table>
                                <tr>
                                    <td>
                                        <asp:Label runat="server" ID="lblSubBatch" Text="Batch" Font-Bold="True" Style="color: Green;"
                                            Font-Names="Book Antiqua" Font-Size="Medium"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:Label runat="server" ID="lbtxtbatch" Font-Bold="True" Font-Names="Book Antiqua"
                                            Font-Size="Medium"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:Label runat="server" ID="lblSubDegree" Text="Degree" Font-Bold="True" Style="color: Green;"
                                            Font-Names="Book Antiqua" Font-Size="Medium"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:Label runat="server" ID="lbltxtDeg" Font-Bold="True" Font-Names="Book Antiqua"
                                            Font-Size="Medium"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:Label runat="server" ID="lblsubSem" Text="Semester" Font-Bold="True" Style="color: Green;"
                                            Font-Names="Book Antiqua" Font-Size="Medium"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:Label runat="server" ID="lbltxtSem" Font-Bold="True" Font-Names="Book Antiqua"
                                            Font-Size="Medium"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:Label runat="server" ID="lbl" Text="Section" Font-Bold="True" Style="color: Green;"
                                            Font-Names="Book Antiqua" Font-Size="Medium"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:Label runat="server" ID="lbltxtSec" Font-Bold="True" Font-Names="Book Antiqua"
                                            Font-Size="Medium"></asp:Label>
                                    </td>
                                </tr>
                            </table>
                            <br />
                            <asp:CheckBox ID="chkForAlternateStaff" runat="server" Font-Bold="True" Font-Names="Book Antiqua"
                                Font-Size="Medium" Text="For Alternate Staff" Checked="false" AutoPostBack="true"
                                OnCheckedChanged="chkForAlternateStaff_CheckedChanged" />
                            <div align="left" style="width: 1000px; height: 400px; border-radius: 10px; border: 1px solid Gray;">
                                <table>
                                    <tr>
                                        <td>
                                            <div align="left" style="overflow: auto; width: 392px; height: 392px; border-radius: 10px;
                                                border: 1px solid Gray;">
                                                <asp:TreeView runat="server" ID="subjtree" BackColor="White" Height="300px" Width="300px"
                                                    SelectedNodeStyle-ForeColor="Red" HoverNodeStyle-BackColor="LightBlue" AutoPostBack="true"
                                                    OnSelectedNodeChanged="subjtree_SelectedNodeChanged" Font-Names="Book Antiqua"
                                                    Font-Size="Small" ForeColor="Black">
                                                </asp:TreeView>
                                            </div>
                                        </td>
                                        <td style="width: 40px;">
                                        </td>
                                        <td runat="server" id="altersp_td" visible="false" class="style12">
                                            <br />
                                            <br />
                                            <table style="margin-top: -57px; float: right;">
                                                <tr runat="server" id="tr_mulstaff" style="visibility: visible;">
                                                    <td colspan="2" style="padding-left: 20px; height: 50px; padding-top: 10px;">
                                                        <asp:Label ID="lblmulstaff" runat="server" Text="For Multiple Staff Selection Only"
                                                            Font-Bold="True" Font-Names="Book Antiqua" Font-Size="Medium" Visible="true">
                                                        </asp:Label>
                                                        <asp:TextBox ID="txtmulstaff" runat="server" CssClass="Dropdown_Txt_Box" ReadOnly="true"
                                                            Width="100px" Font-Bold="True" Font-Names="Book Antiqua" Font-Size="Medium">---Select---</asp:TextBox>
                                                        <asp:Panel ID="pmulstaff" runat="server" CssClass="multxtpanel">
                                                            <asp:CheckBox ID="chkmulstaff" runat="server" Font-Bold="True" Font-Names="Book Antiqua"
                                                                OnCheckedChanged="chkmulstaff_ChekedChange" Font-Size="Medium" Text="Select All"
                                                                AutoPostBack="True" />
                                                            <asp:CheckBoxList ID="chkmullsstaff" runat="server" Font-Size="Medium" AutoPostBack="True"
                                                                OnSelectedIndexChanged="chkmullsstaff_SelectedIndexChanged" Font-Bold="True"
                                                                Font-Names="Book Antiqua">
                                                            </asp:CheckBoxList>
                                                        </asp:Panel>
                                                        <asp:PopupControlExtender ID="PopupControlExtender2" runat="server" TargetControlID="txtmulstaff"
                                                            PopupControlID="pmulstaff" Position="Bottom">
                                                        </asp:PopupControlExtender>
                                                        <asp:Button ID="btnmulstaff" runat="server" Text="Ok" Font-Bold="True" Font-Names="Book Antiqua"
                                                            OnClick="btnmulstaff_Click" Font-Size="Medium" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="2">
                                                        <div class="GridDock" id="dvGridWidth">
                                                            <asp:GridView ID="GridView5" runat="server" Style="width: auto; font-size: 14px"
                                                                Font-Names="Times New Roman" AutoGenerateColumns="false" OnDataBound="GridView5_OnDataBound">
                                                                <HeaderStyle BackColor="#009999" ForeColor="White" />
                                                                <AlternatingRowStyle Height="20px" />
                                                                <Columns>
                                                                    <asp:TemplateField HeaderText="S.No " HeaderStyle-HorizontalAlign="Left">
                                                                        <ItemTemplate>
                                                                            <%#Container.DataItemIndex+1 %>
                                                                        </ItemTemplate>
                                                                        <ItemStyle Width="30px" HorizontalAlign="Center" />
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Subject Name">
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="lblSubName" runat="server" Text='<%# Eval("SubName") %>' />
                                                                            <asp:Label ID="lblSubjectNo" runat="server" Text='<%# Eval("SubNo") %>' Visible="false" />
                                                                        </ItemTemplate>
                                                                        <ItemStyle Width="170px" HorizontalAlign="Left" />
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Staff Name">
                                                                        <ItemTemplate>
                                                                            <asp:DropDownList ID="ddlStaff" runat="server">
                                                                            </asp:DropDownList>
                                                                        </ItemTemplate>
                                                                        <ItemStyle Width="150px" HorizontalAlign="Left" />
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Choose Free staff">
                                                                        <ItemTemplate>
                                                                            <asp:Button ID="btnFreeStaff" runat="server" Text="Select" Font-Bold="True" Font-Names="Book Antiqua"
                                                                                Font-Size="Medium" Height="25px" Width="75px" OnClick="btnFreeStaff_Click" />
                                                                        </ItemTemplate>
                                                                        <ItemStyle Width="80px" HorizontalAlign="Center" />
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Alternate staff Name" Visible="false">
                                                                        <ItemTemplate>
                                                                            <asp:DropDownList ID="ddlAlterStaff" runat="server">
                                                                            </asp:DropDownList>
                                                                        </ItemTemplate>
                                                                        <ItemStyle Width="150px" HorizontalAlign="Left" />
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Choose Busy Staff">
                                                                        <ItemTemplate>
                                                                            <asp:Button ID="btnBussyStaff" runat="server" Text="Select" Font-Bold="True" Font-Names="Book Antiqua"
                                                                                Font-Size="Medium" Height="25px" Width="75px" OnClick="btnBussyStaff_Click" />
                                                                        </ItemTemplate>
                                                                        <ItemStyle Width="80px" HorizontalAlign="Center" />
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Remove">
                                                                        <ItemTemplate>
                                                                            <asp:Button ID="btnRemove" runat="server" Text="Remove" Font-Bold="True" Font-Names="Book Antiqua"
                                                                                Font-Size="Medium" Height="25px" Width="75px" OnClick="btnRemove_Click" />
                                                                        </ItemTemplate>
                                                                        <ItemStyle Width="80px" HorizontalAlign="Center" />
                                                                    </asp:TemplateField>
                                                                </Columns>
                                                            </asp:GridView>
                                                        </div>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style="float: right; padding-top: 15px;">
                                                        <%--   <asp:CheckBox ID="CheckBox1" runat="server" Text="Append to the schedule List" Font-Bold="True"
                                                    Font-Names="Book Antiqua" Font-Size="Medium" ForeColor="blue" OnCheckedChanged="chkSelectAlterStaff_CheckedChanged"
                                                    AutoPostBack="true" />--%>
                                                    </td>
                                                    <td style="padding-left: 40px; padding-top: 15px;">
                                                        <asp:CheckBox ID="chkappend" runat="server" Text="Append to the schedule List" Font-Bold="True"
                                                            Font-Names="Book Antiqua" Font-Size="Medium" ForeColor="blue" OnCheckedChanged="chkSelectAlterStaff_CheckedChanged"
                                                            AutoPostBack="true" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                            <center>
                                <asp:Label ID="errmsg" runat="server" Font-Bold="true" Font-Size="Medium" ForeColor="Red"></asp:Label>
                            </center>
                            <div style="overflow: auto; float: right; margin-right: 25px; margin-top: 25px;">
                                <asp:Button ID="btnOk" runat="server" Text="Ok" Font-Bold="True" Font-Names="Book Antiqua"
                                    Font-Size="Medium" Height="25px" Width="75px" OnClick="btnOk_Click" />
                            </div>
                        </div>
                    </div>
                </center>
                <center>
                    <div id="div1" runat="server" visible="false" style="height: 200%; z-index: 2000;
                        width: 100%; background-color: rgba(54, 25, 25, .2); position: absolute; top: 0;
                        left: 0px;">
                        <center>
                            <div id="div2" runat="server" class="table" style="background-color: White; height: 600px;
                                width: 85%; border: 5px solid #0CA6CA; border-top: 25px solid #0CA6CA; top: 1%;
                                left: 5%; right: 5%; position: fixed; border-radius: 10px;">
                                <center>
                                    <span style="font-family: Book Antiqua; font-size: 20px; font-weight: bold; color: Green;
                                        margin: 0px; margin-bottom: 20px; margin-top: 15px; position: relative;">Semester
                                        Schedule Details</span>
                                </center>
                                <center>
                                    <div style="width: auto; height: auto; overflow: auto; margin: 0px; margin-bottom: auto;
                                        margin-top: auto">
                                        <center>
                                            <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="false" Font-Names="Book Antiqua"
                                                HeaderStyle-BackColor="#0CA6CA" BackColor="White">
                                                <%-- OnDataBound="gridTimeTable_OnDataBound"--%>
                                                <Columns>
                                                    <asp:TemplateField HeaderText="Day">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblDateDisp" runat="server" Text='<%#Eval("DateDisp") %>'></asp:Label>
                                                            <asp:Label ID="lblDayVal" runat="server" Text='<%#Eval("DateVal") %>' Visible="false"></asp:Label>
                                                        </ItemTemplate>
                                                        <ItemStyle Width="100px" HorizontalAlign="Center" BackColor="#F8B7B3" />
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Period 1" Visible="false">
                                                        <ItemTemplate>
                                                            <asp:LinkButton ID="lnkPeriod_1" runat="server" Text='<%#Eval("P1Val") %>' ForeColor="Blue"
                                                                OnClick="lnkAttMark11" Font-Underline="false"></asp:LinkButton>
                                                            <asp:Label ID="lblPeriod_1" runat="server" Text='<%#Eval("PVal1") %>' Visible="false"></asp:Label>
                                                            <asp:Label ID="lblTT_1" runat="server" Text='<%#Eval("TT_1") %>' Visible="false"></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Period 2" Visible="false">
                                                        <ItemTemplate>
                                                            <asp:LinkButton ID="lnkPeriod_2" runat="server" Text='<%#Eval("P2Val") %>' ForeColor="Blue"
                                                                OnClick="lnkAttMark11" Font-Underline="false"></asp:LinkButton>
                                                            <asp:Label ID="lblPeriod_2" runat="server" Text='<%#Eval("PVal2") %>' Visible="false"></asp:Label>
                                                            <asp:Label ID="lblTT_2" runat="server" Text='<%#Eval("TT_2") %>' Visible="false"></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Period 3" Visible="false">
                                                        <ItemTemplate>
                                                            <asp:LinkButton ID="lnkPeriod_3" runat="server" Text='<%#Eval("P3Val") %>' ForeColor="Blue"
                                                                OnClick="lnkAttMark11" Font-Underline="false"></asp:LinkButton>
                                                            <asp:Label ID="lblPeriod_3" runat="server" Text='<%#Eval("PVal3") %>' Visible="false"></asp:Label>
                                                            <asp:Label ID="lblTT_3" runat="server" Text='<%#Eval("TT_3") %>' Visible="false"></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Period 4" Visible="false">
                                                        <ItemTemplate>
                                                            <asp:LinkButton ID="lnkPeriod_4" runat="server" Text='<%#Eval("P4Val") %>' ForeColor="Blue"
                                                                OnClick="lnkAttMark11" Font-Underline="false"></asp:LinkButton>
                                                            <asp:Label ID="lblPeriod_4" runat="server" Text='<%#Eval("PVal4") %>' Visible="false"></asp:Label>
                                                            <asp:Label ID="lblTT_4" runat="server" Text='<%#Eval("TT_4") %>' Visible="false"></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Period 5" Visible="false">
                                                        <ItemTemplate>
                                                            <asp:LinkButton ID="lnkPeriod_5" runat="server" Text='<%#Eval("P5Val") %>' ForeColor="Blue"
                                                                OnClick="lnkAttMark11" Font-Underline="false"></asp:LinkButton>
                                                            <asp:Label ID="lblPeriod_5" runat="server" Text='<%#Eval("PVal5") %>' Visible="false"></asp:Label>
                                                            <asp:Label ID="lblTT_5" runat="server" Text='<%#Eval("TT_5") %>' Visible="false"></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Period 6" Visible="false">
                                                        <ItemTemplate>
                                                            <asp:LinkButton ID="lnkPeriod_6" runat="server" Text='<%#Eval("P6Val") %>' ForeColor="Blue"
                                                                OnClick="lnkAttMark11" Font-Underline="false"></asp:LinkButton>
                                                            <asp:Label ID="lblPeriod_6" runat="server" Text='<%#Eval("PVal6") %>' Visible="false"></asp:Label>
                                                            <asp:Label ID="lblTT_6" runat="server" Text='<%#Eval("TT_6") %>' Visible="false"></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Period 7" Visible="false">
                                                        <ItemTemplate>
                                                            <asp:LinkButton ID="lnkPeriod_7" runat="server" Text='<%#Eval("P7Val") %>' ForeColor="Blue"
                                                                OnClick="lnkAttMark11" Font-Underline="false"></asp:LinkButton>
                                                            <asp:Label ID="lblPeriod_7" runat="server" Text='<%#Eval("PVal7") %>' Visible="false"></asp:Label>
                                                            <asp:Label ID="lblTT_7" runat="server" Text='<%#Eval("TT_7") %>' Visible="false"></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Period 8" Visible="false">
                                                        <ItemTemplate>
                                                            <asp:LinkButton ID="lnkPeriod_8" runat="server" Text='<%#Eval("P8Val") %>' ForeColor="Blue"
                                                                OnClick="lnkAttMark11" Font-Underline="false"></asp:LinkButton>
                                                            <asp:Label ID="lblPeriod_8" runat="server" Text='<%#Eval("PVal8") %>' Visible="false"></asp:Label>
                                                            <asp:Label ID="lblTT_8" runat="server" Text='<%#Eval("TT_8") %>' Visible="false"></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Period 9" Visible="false">
                                                        <ItemTemplate>
                                                            <asp:LinkButton ID="lnkPeriod_9" runat="server" Text='<%#Eval("P9Val") %>' ForeColor="Blue"
                                                                OnClick="lnkAttMark11" Font-Underline="false"></asp:LinkButton>
                                                            <asp:Label ID="lblPeriod_9" runat="server" Text='<%#Eval("PVal9") %>' Visible="false"></asp:Label>
                                                            <asp:Label ID="lblTT_9" runat="server" Text='<%#Eval("TT_9") %>' Visible="false"></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Period 10" Visible="false">
                                                        <ItemTemplate>
                                                            <asp:LinkButton ID="lnkPeriod_10" runat="server" Text='<%#Eval("P10Val") %>' ForeColor="Blue"
                                                                OnClick="lnkAttMark11" Font-Underline="false"></asp:LinkButton>
                                                            <asp:Label ID="lblPeriod_10" runat="server" Text='<%#Eval("PVal10") %>' Visible="false"></asp:Label>
                                                            <asp:Label ID="lblTT_10" runat="server" Text='<%#Eval("TT_10") %>' Visible="false"></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                            </asp:GridView>
                                            <center>
                                                <asp:Label ID="semmsglbl" runat="server" Text="Select any cell" ForeColor="Red" Font-Size="Larger"></asp:Label>
                                            </center>
                                            <br />
                                            <br />
                                            <br />
                                            <asp:Button ID="Button6" runat="server" Text="Exit" Font-Bold="True" Font-Names="Book Antiqua"
                                                Font-Size="Medium" Height="25px" Width="75px" Style="margin-top: 15px;" OnClick="Button6_Clik" />
                                        </center>
                                    </div>
                                </center>
                            </div>
                        </center>
                    </div>
                </center>
                <%--Free Staff list popup--%>
                <center>
                    <div id="divAlterFreeStaffDetails" runat="server" visible="false" style="height: 160em;
                        z-index: 2000; width: 100%; background-color: rgba(54, 25, 25, .2); position: absolute;
                        top: 0; left: 0px;">
                        <center>
                            <div id="divAlterFreeStaff" runat="server" class="table" style="background-color: White;
                                height: 600px; width: 85%; border: 5px solid #0CA6CA; border-top: 25px solid #0CA6CA;
                                top: 1%; left: 5%; right: 5%; position: fixed; border-radius: 10px;">
                                <center>
                                    <span style="font-family: Book Antiqua; font-size: 20px; font-weight: bold; color: Green;
                                        margin: 0px; margin-bottom: 20px; margin-top: 15px; position: relative;">Free Staff
                                        List</span>
                                </center>
                                <div>
                                    <asp:Label ID="lblAlterDate" runat="server" Text="" Visible="false"></asp:Label>
                                    <asp:Label ID="lblAlterHour" runat="server" Text="" Visible="false"></asp:Label>
                                    <table>
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblAlterFreeCollege" Text="College" runat="server"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:DropDownList ID="ddlAlterFreeCollege" runat="server" OnSelectedIndexChanged="ddlAlterFreeCollege_SelectedIndexChanged"
                                                    AutoPostBack="true">
                                                </asp:DropDownList>
                                            </td>
                                            <td>
                                                <asp:Label ID="lblAlterFreeDepartment" Text="Department" runat="server"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:DropDownList ID="ddlAlterFreeDepartment" Width="100px" runat="server" OnSelectedIndexChanged="ddlAlterFreeDepartment_SelectedIndexChanged"
                                                    AutoPostBack="true">
                                                </asp:DropDownList>
                                            </td>
                                            <td>
                                                <asp:Label ID="lblSearchBy" runat="server" Text="Staff By"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:DropDownList ID="ddlAlterFreeStaff" runat="server" Width="150px" OnSelectedIndexChanged="ddlAlterFreeStaff_SelectedIndexChanged"
                                                    AutoPostBack="true">
                                                    <asp:ListItem Value="0">Staff Name</asp:ListItem>
                                                    <asp:ListItem Value="1">Staff Code</asp:ListItem>
                                                </asp:DropDownList>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtAlterFreeStaffSearch" runat="server" OnTextChanged="txtAlterFreeStaffSearch_TextChanged"
                                                    Width="200px" AutoPostBack="True"></asp:TextBox>
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                                <center>
                                    <div style="width: auto; height: auto; overflow: auto; margin: 0px; margin-bottom: auto;
                                        margin-top: auto">
                                        <center>
                                            <asp:GridView ID="GridFreeStaff" runat="server" Style="width: auto; font-size: 14px"
                                                Font-Names="Times New Roman" AutoGenerateColumns="false" AllowPaging="true" PageSize="20"
                                                OnDataBound="GridView5_OnDataBound">
                                                <HeaderStyle BackColor="#009999" ForeColor="White" />
                                                <AlternatingRowStyle Height="20px" />
                                                <Columns>
                                                    <asp:TemplateField HeaderText="S.No " HeaderStyle-HorizontalAlign="Left">
                                                        <ItemTemplate>
                                                            <%#Container.DataItemIndex+1 %>
                                                        </ItemTemplate>
                                                        <ItemStyle Width="30px" HorizontalAlign="Center" />
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Select">
                                                        <ItemTemplate>
                                                            <asp:CheckBox ID="Chk" runat="server" />
                                                        </ItemTemplate>
                                                        <ItemStyle Width="30px" HorizontalAlign="Left" />
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Staff Code">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblstaffCode" runat="server" Text='<%# Eval("staff_code") %>' />
                                                        </ItemTemplate>
                                                        <ItemStyle Width="200px" HorizontalAlign="Left" />
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Staff Name">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblStaffName" runat="server" Text='<%# Eval("staff_name") %>' />
                                                        </ItemTemplate>
                                                        <ItemStyle Width="200px" HorizontalAlign="Left" />
                                                    </asp:TemplateField>
                                                </Columns>
                                            </asp:GridView>
                                        </center>
                                    </div>
                                    <asp:Label ID="lblAlterFreeStaffError" runat="server" Font-Bold="true" Font-Names="Book Antiqua"
                                        Font-Size="Medium" Visible="false" ForeColor="Red" Style="margin: 0px; margin-bottom: 20px;
                                        margin-top: 10px; position: relative;">
                                    </asp:Label>
                                    <asp:Button ID="btnSelectStaff" runat="server" Text="Ok" Font-Bold="True" Font-Names="Book Antiqua"
                                        Font-Size="Medium" Height="25px" Width="75px" Style="margin-top: 15px;" OnClick="btnSelectStaff_Click" /><%----%>
                                    <asp:Button ID="btnFreeStaffExit" runat="server" Text="Exit" Font-Bold="True" Font-Names="Book Antiqua"
                                        Font-Size="Medium" Height="25px" Width="75px" Style="margin-top: 15px;" OnClick="btnFreeStaffExit_Click" />
                                </center>
                            </div>
                        </center>
                    </div>
                </center>
                <%-- Alert Message--%>
                <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                    <ContentTemplate>
                        <asp:Panel>
                            <div id="Div3" runat="server" visible="false" style="height: 200%; z-index: 1000;
                                width: 100%; background-color: rgba(54, 25, 25, .2); position: absolute; top: 0;
                                left: 0px;">
                                <center>
                                    <div id="Div4" runat="server" class="table" style="background-color: White; height: 160px;
                                        width: 530px; border: 5px solid #0CA6CA; border-top: 25px solid #0CA6CA; margin-top: 551px;
                                        border-radius: 10px;">
                                        <center>
                                            <table style="height: 100px; width: 100%">
                                                <tr>
                                                    <td align="center">
                                                        <asp:Label ID="Label12" runat="server" Text="" Style="color: Black;" Font-Bold="true"
                                                            Font-Size="Medium"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <center>
                                                            <asp:Button ID="btnOKsave" runat="server" Text="Ok" CssClass="btn1 textbox1 textbox "
                                                                Width="70px" OnClick="btnOKsave_Clik" />
                                                            <asp:Button ID="bt_closedalter" runat="server" Text="Cancel" CssClass="btn1 textbox1 textbox "
                                                                OnClientClick="return DisplayLoadingDiv();" OnClick="bt_closedalter_Clik" Width="80px" />
                                                        </center>
                                                    </td>
                                                </tr>
                                            </table>
                                        </center>
                                    </div>
                                </center>
                            </div>
                        </asp:Panel>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <center>
                    <div id="Div5" runat="server" visible="false" style="height: 200%; z-index: 1000;
                        width: 100%; background-color: rgba(54, 25, 25, .2); position: absolute; top: 0;
                        left: 0px;">
                        <center>
                            <div id="Div6" runat="server" class="table" style="background-color: White; height: 160px;
                                width: 530px; border: 5px solid #0CA6CA; border-top: 25px solid #0CA6CA; margin-top: 551px;
                                border-radius: 10px;">
                                <center>
                                    <table style="height: 100px; width: 100%">
                                        <tr>
                                            <td align="center">
                                                <asp:Label ID="lblAlrt" runat="server" Text="" Style="color: Red;" Font-Bold="true"
                                                    Font-Size="Medium"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <center>
                                                    <asp:Button ID="Button1" runat="server" Text="Remove" CssClass="btn1 textbox1 textbox "
                                                        Width="70px" OnClick="Button1_Clik" />
                                                    <asp:Button ID="Button2" runat="server" Text="Cancel" CssClass="btn1 textbox1 textbox "
                                                        OnClick="Button2_Clik" Width="80px" />
                                                </center>
                                            </td>
                                        </tr>
                                    </table>
                                </center>
                            </div>
                        </center>
                    </div>
                </center>
                <center>
                    <div id="Div7" runat="server" visible="false" style="height: 200%; z-index: 1000;
                        width: 100%; background-color: rgba(54, 25, 25, .2); position: absolute; top: 0;
                        left: 0px;">
                        <center>
                            <div id="Div8" runat="server" class="table" style="background-color: White; height: 160px;
                                width: 530px; border: 5px solid #0CA6CA; border-top: 25px solid #0CA6CA; margin-top: 551px;
                                border-radius: 10px;">
                                <center>
                                    <table style="height: 100px; width: 100%">
                                        <tr>
                                            <td align="center">
                                                <asp:Label ID="Label1" runat="server" Text="" Style="color: Red;" Font-Bold="true"
                                                    Font-Size="Medium"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <center>
                                                    <asp:Button ID="Button4" runat="server" Text="Close" CssClass="btn1 textbox1 textbox "
                                                        OnClick="Button4_Clik" Width="80px" />
                                                </center>
                                            </td>
                                        </tr>
                                    </table>
                                </center>
                            </div>
                        </center>
                    </div>
                </center>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>
