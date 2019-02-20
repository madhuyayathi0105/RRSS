using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.Configuration;
using System.Web.Services;
using System.Drawing;

public partial class vendor_quatation_request : System.Web.UI.Page
{
    DAccess2 d2 = new DAccess2();
    string usercode = string.Empty;
    string collegecode1 = string.Empty;
    string collegecode = string.Empty;
    string singleuser = string.Empty;
    string group_user = string.Empty;
    DataSet ds = new DataSet();
    DataSet ds1 = new DataSet();
    bool check = false;
    Boolean Cellclick = false;
    string dtaccessdate = DateTime.Now.ToString();
    string dtaccesstime = DateTime.Now.ToLongTimeString();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["collegecode"] == null)
        {
            Response.Redirect("~/Default.aspx");
        }
        lblvalidation1.Text = "";
        usercode = Session["usercode"].ToString();
        collegecode1 = Session["collegecode"].ToString();
        singleuser = Session["single_user"].ToString();
        group_user = Session["group_code"].ToString();
        CalendarExtender1.EndDate = DateTime.Now;
        caltodate.EndDate = DateTime.Now;
        if (!IsPostBack)
        {
            txt_fromdate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            txt_todate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            txt_venduedate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            txt_fromdate.Attributes.Add("readonly", "readonly");
            txt_todate.Attributes.Add("readonly", "readonly");
            txt_date.Text = DateTime.Now.ToString("dd/MM/yyyy");
            txt_date.Attributes.Add("readonly", "readonly");
            txt_reqcode.Attributes.Add("readonly", "readonly");
            txt_venduedate.Attributes.Add("readonly", "readonly");
            txt_venname.Attributes.Add("readonly", "readonly");
            txt_reqcode.Enabled = false;
            Session["vendorcode"] = null;
            Session["actrow"] = null;
            Session["actcol"] = null;
            Session["vendorpk"] = null;
            // bindrequestcode();
            vendor();
            bindquocode();
            FpSpread1.Sheets[0].RowCount = 0;
            FpSpread1.Sheets[0].ColumnCount = 0;
            FpSpread1.Visible = false;
            FpSpread2.Sheets[0].RowCount = 0;
            FpSpread2.Sheets[0].ColumnCount = 0;
            FpSpread2.Visible = false;
            FpSpread3.Sheets[0].RowCount = 0;
            FpSpread3.Sheets[0].ColumnCount = 0;
            FpSpread3.Visible = false;
            Bindrequestcode();
            bindvendorrequestcode();
            btn_basego_Click(sender, e);
            btn_go_Click(sender, e);
        }
    }
    protected void lnk_btnlogout_Click(object sender, EventArgs e)
    {
        Session.Abandon();
        Session.Clear();
        Session.RemoveAll();
        System.Web.Security.FormsAuthentication.SignOut();
        Response.Redirect("~/Default.aspx", false);

    }
    public void bindrequestcode()
    {
        try
        {
            string newitemcode = "";
            string selectquery = "select QuoAcr,QuoStNo,QuoSize from IM_CodeSettings  order by startdate desc";
            ds = d2.select_method_wo_parameter(selectquery, "Text");
            if (ds.Tables[0].Rows.Count > 0)
            {
                string ordcode = Convert.ToString(ds.Tables[0].Rows[0]["QuoAcr"]);
                string itemacronym = Convert.ToString(ds.Tables[0].Rows[0]["QuoAcr"]);
                string itemstarno = Convert.ToString(ds.Tables[0].Rows[0]["QuoStNo"]);
                string itemsize = Convert.ToString(ds.Tables[0].Rows[0]["QuoSize"]);
                selectquery = "select distinct top (1) VenQuotCode from IT_VendorQuot where VenQuotCode like '" + Convert.ToString(ordcode) + "%' order by VenQuotCode desc";
                ds.Clear();
                ds = d2.select_method_wo_parameter(selectquery, "Text");
                if (ds.Tables[0].Rows.Count > 0)
                {
                    string itemcode = Convert.ToString(ds.Tables[0].Rows[0]["VenQuotCode"]);
                    string itemacr = Convert.ToString(itemacronym);
                    int len = itemacr.Length;
                    itemcode = itemcode.Remove(0, len);
                    int len1 = Convert.ToString(itemcode).Length;
                    string newnumber = Convert.ToString((Convert.ToInt32(itemcode) + 1));
                    len = Convert.ToString(newnumber).Length;
                    len1 = Convert.ToInt32(itemsize) - len;
                    if (len1 == 2)
                    {
                        newitemcode = "00" + newnumber;
                    }
                    else if (len1 == 1)
                    {
                        newitemcode = "0" + newnumber;
                    }
                    else if (len1 == 4)
                    {
                        newitemcode = "0000" + newnumber;
                    }
                    else if (len1 == 3)
                    {
                        newitemcode = "000" + newnumber;
                    }
                    else if (len1 == 5)
                    {
                        newitemcode = "00000" + newnumber;
                    }
                    else if (len1 == 6)
                    {
                        newitemcode = "000000" + newnumber;
                    }
                    else
                    {
                        newitemcode = Convert.ToString(newnumber);
                    }
                    if (newitemcode.Trim() != "")
                    {
                        newitemcode = itemacr + "" + newitemcode;
                    }
                }
                else
                {
                    string itemacr = Convert.ToString(itemstarno);
                    int len = itemacr.Length;
                    string items = Convert.ToString(itemsize);
                    int len1 = Convert.ToInt32(items);
                    int size = len1 - len;
                    if (size == 2)
                    {
                        newitemcode = "00" + itemstarno;
                    }
                    else if (size == 1)
                    {
                        newitemcode = "0" + itemstarno;
                    }
                    else if (size == 4)
                    {
                        newitemcode = "0000" + itemstarno;
                    }
                    else if (size == 3)
                    {
                        newitemcode = "000" + itemstarno;
                    }
                    else if (size == 5)
                    {
                        newitemcode = "00000" + itemstarno;
                    }
                    else if (size == 6)
                    {
                        newitemcode = "000000" + itemstarno;
                    }
                    else
                    {
                        newitemcode = Convert.ToString(itemstarno);
                    }
                    newitemcode = Convert.ToString(itemacronym) + "" + Convert.ToString(newitemcode);
                }
                txt_reqcode.Text = Convert.ToString(newitemcode);
            }
        }
        catch
        {

        }
    }
    protected void Bindrequestcode()
    {
        try
        {
            string q1 = "select distinct RequestCode,RequisitionPK from RQ_Requisition rq,RQ_RequisitionDet rd where rq.RequisitionPK=rd.RequisitionFK and rq.ReqAppStatus='1'  and RequestType='1' and rq.VendorReq_Type='1' order by RequestCode desc";
            ds.Clear();
            ds = d2.select_method_wo_parameter(q1, "Text");
            if (ds.Tables[0].Rows.Count > 0)
            {
                ddl_requestcode.DataSource = ds;
                ddl_requestcode.DataTextField = "RequestCode";
                ddl_requestcode.DataValueField = "RequisitionPK";
                ddl_requestcode.DataBind();
            }
            bindvendorrequestcode();
        }
        catch { }
    }
    protected void bindvendorrequestcode()
    {
        try
        {
            ddl_vendorreqcode.Items.Clear();
            string q2 = "select distinct vq.VenReqCode,vq.VenReqPK from IT_VendorReq vq,IT_VendorReqDet vrq,RQ_Requisition rq where vq.VenReqPK =vrq.VenReqPK and vq.ReqFK=vrq.ReqFK and rq.RequestType='1' and VendorReq_Type='1' and rq.RequisitionPK=vq.ReqFK and vq.ReqFK in('" + Convert.ToString(ddl_requestcode.SelectedItem.Value) + "') order by VenReqCode ";
            ds.Clear();
            ds = d2.select_method_wo_parameter(q2, "Text");
            if (ds.Tables[0].Rows.Count > 0)
            {
                ddl_vendorreqcode.DataSource = ds;
                ddl_vendorreqcode.DataTextField = "VenReqCode";
                ddl_vendorreqcode.DataValueField = "VenReqPK";
                ddl_vendorreqcode.DataBind();
            }
            txt_venname.Text = bindvendorname(Convert.ToString(ddl_vendorreqcode.SelectedItem.Value));
        }
        catch
        {
            txt_venname.Text = "";
        }
    }
    protected void ddl_requestcode_SelectedIndexChanged(object sender, EventArgs e)
    {
        bindvendorrequestcode();
        FpSpread1.Visible = false;
        btnmain_save.Visible = false;
        btn_exit.Visible = false;

    }
    protected void ddl_vendorreqcode_SelectedIndexChanged(object sender, EventArgs e)
    {
        txt_venname.Text = bindvendorname(Convert.ToString(ddl_vendorreqcode.SelectedItem.Value));
    }
    public string bindvendorname(string itemcode)
    {
        string venname = "";
        try
        {
            txt_venname.Text = "";
            venname = d2.GetFunction("select VendorCompName from IT_VendorReq vq,CO_VendorMaster vm where vm.VendorPK=vq.VendorFK and  VenReqPK='" + itemcode + "'");
            string q2 = d2.GetFunction("select distinct Vendorcode from CO_VendorMaster WHERE VendorCompName='" + venname + "'");
            string q3 = d2.GetFunction("select distinct VendorPK from CO_VendorMaster WHERE Vendorcode='" + q2 + "'");
            Session["vendorcode"] = q2;
            Session["vendorpk"] = q3;
        }
        catch { }
        return venname;
    }

    protected void btn_go_Click(object sender, EventArgs e)
    {
        try
        {
            string q = "";
            if (txt_search.Text.Trim() != "")
            {
                q = "select distinct vm.VendorCompName,im.ItemName,im.ItemCode,im.itempk ,vr.ReqFK from CO_VendorMaster vm, IT_VendorReq vr,IM_ItemMaster im,IT_VendorReqDet vd,RQ_Requisition rq where vm.VendorPK=vr.VendorFK and vr.ReqFK=vd.ReqFK and im.ItemPK=vd.ItemFK  and rq.RequisitionPK=vr.ReqFK and rq.VendorReq_Type='1' and vr.VenReqCode in ('" + Convert.ToString(txt_search.Text) + "') and vr.VenReqPK not in(select VenReqFK from IT_VendorQuot)";
            }
            else if (txt_searchvendor.Text.Trim() != "")
            {
                q = "select distinct vm.VendorCompName,im.ItemName,im.ItemCode,im.itempk ,vr.ReqFK from CO_VendorMaster vm, IT_VendorReq vr,IM_ItemMaster im,IT_VendorReqDet vd,RQ_Requisition rq where vm.VendorPK=vr.VendorFK and vr.ReqFK=vd.ReqFK and im.ItemPK=vd.ItemFK  and rq.RequisitionPK=vr.ReqFK and rq.VendorReq_Type='1' and vm.VendorCode in ('" + Convert.ToString(Session["vendorcode"]) + "') and vr.VenReqPK not in(select VenReqFK from IT_VendorQuot)";
            }
            else
            {
                q = "select distinct vm.VendorCompName,im.ItemName,im.ItemCode,im.itempk ,vr.ReqFK from CO_VendorMaster vm, IT_VendorReq vr,IM_ItemMaster im,IT_VendorReqDet vd,RQ_Requisition rq where vm.VendorPK=vr.VendorFK and vr.ReqFK=vd.ReqFK and im.ItemPK=vd.ItemFK  and rq.RequisitionPK=vr.ReqFK and rq.VendorReq_Type='1' and rq.RequisitionPK ='" + ddl_requestcode.SelectedItem.Value + "' and vr.VenReqPK='" + ddl_vendorreqcode.SelectedItem.Value + "' and vr.VenReqPK not in(select VenReqFK from IT_VendorQuot)";
                //and vm.VendorPK='" + Convert.ToString(Session["vendorpk"]) + "'
              
            }
            ds1.Clear();
            ds1 = d2.select_method_wo_parameter(q, "Text");
            if (ds1.Tables[0].Rows.Count > 0)
            {
                FpSpread1.Sheets[0].RowCount = 0;
                FpSpread1.Sheets[0].ColumnCount = 0;
                FpSpread1.CommandBar.Visible = false;
                FpSpread1.Sheets[0].AutoPostBack = false;
                FpSpread1.Sheets[0].ColumnHeader.RowCount = 1;
                FpSpread1.Sheets[0].RowHeader.Visible = false;
                FpSpread1.Sheets[0].ColumnCount = 17;
                FarPoint.Web.Spread.StyleInfo darkstyle = new FarPoint.Web.Spread.StyleInfo();
                darkstyle.BackColor = ColorTranslator.FromHtml("#0CA6CA");
                darkstyle.ForeColor = Color.White;
                FpSpread1.ActiveSheetView.ColumnHeader.DefaultStyle = darkstyle;
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 0].Text = "S.No";
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 0].Font.Bold = true;
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 0].Font.Name = "Book Antiqua";
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 0].Font.Size = FontUnit.Medium;
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 0].HorizontalAlign = HorizontalAlign.Center;
                FpSpread1.Columns[0].Locked = true;
                FpSpread1.Columns[0].Width = 37;
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 1].Text = "Select";
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 1].Font.Bold = true;
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 1].Font.Name = "Book Antiqua";
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 1].Font.Size = FontUnit.Medium;
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 1].HorizontalAlign = HorizontalAlign.Center;
                //FpSpread1.Columns[1].Locked = true;
                FpSpread1.Columns[1].Width = 40;

                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 2].Text = "Edit";
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 2].Font.Bold = true;
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 2].Font.Name = "Book Antiqua";
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 2].Font.Size = FontUnit.Medium;
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 2].HorizontalAlign = HorizontalAlign.Center;
                FpSpread1.Columns[2].Width = 45;
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 3].Text = "Item Name";
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 3].Font.Bold = true;
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 3].Font.Name = "Book Antiqua";
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 3].Font.Size = FontUnit.Medium;
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 3].HorizontalAlign = HorizontalAlign.Center;
                FpSpread1.Columns[3].Locked = true;
                FpSpread1.Columns[3].Width = 100;
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 4].Text = "Qty";
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 4].Font.Bold = true;
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 4].Font.Name = "Book Antiqua";
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 4].Font.Size = FontUnit.Medium;
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 4].HorizontalAlign = HorizontalAlign.Center;
                FpSpread1.Columns[4].Width = 50;
                FpSpread1.Columns[4].Locked = true;
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 5].Text = "RPU";
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 5].Font.Bold = true;
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 5].Font.Name = "Book Antiqua";
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 5].Font.Size = FontUnit.Medium;
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 5].HorizontalAlign = HorizontalAlign.Center;
                FpSpread1.Columns[5].Width = 50;
                FpSpread1.Columns[5].Locked = true;
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 6].Text = "Discount";
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 6].Font.Bold = true;
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 6].Font.Name = "Book Antiqua";
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 6].Font.Size = FontUnit.Medium;
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 6].HorizontalAlign = HorizontalAlign.Center;
                FpSpread1.Columns[6].Width = 100;
                FpSpread1.Columns[6].Locked = true;
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 7].Text = "Tax(%)";
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 7].Font.Bold = true;
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 7].Font.Name = "Book Antiqua";
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 7].Font.Size = FontUnit.Medium;
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 7].HorizontalAlign = HorizontalAlign.Center;
                FpSpread1.Columns[7].Width = 100;
                FpSpread1.Columns[7].Locked = true;
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 8].Text = "Cost";
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 8].Font.Bold = true;
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 8].Font.Name = "Book Antiqua";
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 8].Font.Size = FontUnit.Medium;
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 8].HorizontalAlign = HorizontalAlign.Center;
                FpSpread1.Columns[8].Width = 150;
                FpSpread1.Columns[8].Locked = true;
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 9].Text = "Exercies tax";
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 9].Font.Bold = true;
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 9].Font.Name = "Book Antiqua";
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 9].Font.Size = FontUnit.Medium;
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 9].HorizontalAlign = HorizontalAlign.Center;
                FpSpread1.Columns[9].Width = 100;
                FpSpread1.Columns[9].Locked = true;
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 10].Text = "Education Cess";
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 10].Font.Bold = true;
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 10].Font.Name = "Book Antiqua";
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 10].Font.Size = FontUnit.Medium;
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 10].HorizontalAlign = HorizontalAlign.Center;
                FpSpread1.Columns[10].Width = 100;
                FpSpread1.Columns[10].Locked = true;
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 11].Text = "Higher Edu.Cess";
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 11].Font.Bold = true;
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 11].Font.Name = "Book Antiqua";
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 11].Font.Size = FontUnit.Medium;
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 11].HorizontalAlign = HorizontalAlign.Center;
                FpSpread1.Columns[11].Width = 150;
                FpSpread1.Columns[11].Locked = true;
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 12].Text = "Other Charges";
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 12].Font.Bold = true;
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 12].Font.Name = "Book Antiqua";
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 12].Font.Size = FontUnit.Medium;
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 12].HorizontalAlign = HorizontalAlign.Center;
                FpSpread1.Columns[12].Width = 150;
                FpSpread1.Columns[12].Locked = true;

                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 13].Text = "CallEduCess";
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 13].Font.Bold = true;
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 13].Font.Name = "Book Antiqua";
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 13].Font.Size = FontUnit.Medium;
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 13].HorizontalAlign = HorizontalAlign.Center;
                FpSpread1.Columns[13].Width = 150;
                FpSpread1.Columns[13].Locked = true;
                FpSpread1.Columns[13].Visible = false;

                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 14].Text = "CallExTax";
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 14].Font.Bold = true;
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 14].Font.Name = "Book Antiqua";
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 14].Font.Size = FontUnit.Medium;
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 14].HorizontalAlign = HorizontalAlign.Left;
                FpSpread1.Columns[14].Width = 150;
                FpSpread1.Columns[14].Locked = true;
                FpSpread1.Columns[14].Visible = false;

                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 15].Text = "Description";
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 15].Font.Bold = true;
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 15].Font.Name = "Book Antiqua";
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 15].Font.Size = FontUnit.Medium;
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 15].HorizontalAlign = HorizontalAlign.Center;
                FpSpread1.Columns[15].Locked = true;
                //FpSpread1.Columns[13].Visible = false;

                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 16].Text = "Delivery Date";
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 16].Font.Bold = true;
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 16].Font.Name = "Book Antiqua";
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 16].Font.Size = FontUnit.Medium;
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 16].HorizontalAlign = HorizontalAlign.Left;
                FpSpread1.Columns[16].Locked = true;
                FarPoint.Web.Spread.ButtonCellType btnType = new FarPoint.Web.Spread.ButtonCellType();
                FarPoint.Web.Spread.CheckBoxCellType chkall = new FarPoint.Web.Spread.CheckBoxCellType();
                chkall.AutoPostBack = true;
                FpSpread1.Sheets[0].RowCount++;

                FpSpread1.Sheets[0].Cells[FpSpread1.Sheets[0].RowCount - 1, 1].CellType = chkall;
                FpSpread1.Sheets[0].Cells[FpSpread1.Sheets[0].RowCount - 1, 1].HorizontalAlign = HorizontalAlign.Center;
                FarPoint.Web.Spread.CheckBoxCellType chkbox = new FarPoint.Web.Spread.CheckBoxCellType();
                chkbox.AutoPostBack = false;

                for (int i = 0; i < ds1.Tables[0].Rows.Count; i++)
                {
                    FpSpread1.Sheets[0].RowCount++;
                    FpSpread1.Sheets[0].Cells[FpSpread1.Sheets[0].RowCount - 1, 0].Text = Convert.ToString(i + 1);
                    FpSpread1.Sheets[0].Cells[FpSpread1.Sheets[0].RowCount - 1, 0].HorizontalAlign = HorizontalAlign.Center;
                    FpSpread1.Sheets[0].Cells[FpSpread1.Sheets[0].RowCount - 1, 0].Font.Size = FontUnit.Medium;
                    FpSpread1.Sheets[0].Cells[FpSpread1.Sheets[0].RowCount - 1, 0].Font.Name = "Book Antiqua";

                    FpSpread1.Sheets[0].Cells[FpSpread1.Sheets[0].RowCount - 1, 1].CellType = chkbox;
                    FpSpread1.Sheets[0].Cells[FpSpread1.Sheets[0].RowCount - 1, 1].HorizontalAlign = HorizontalAlign.Center;
                    FpSpread1.Sheets[0].Cells[FpSpread1.Sheets[0].RowCount - 1, 1].Font.Size = FontUnit.Medium;
                    FpSpread1.Sheets[0].Cells[FpSpread1.Sheets[0].RowCount - 1, 1].Font.Name = "Book Antiqua";


                    FpSpread1.Sheets[0].Cells[FpSpread1.Sheets[0].RowCount - 1, 2].CellType = btnType;
                    btnType.Text = "Edit";
                    btnType.CssClass = "textbox btn";
                    btnType.ForeColor = Color.Blue;
                    FpSpread1.Sheets[0].Cells[FpSpread1.Sheets[0].RowCount - 1, 2].HorizontalAlign = HorizontalAlign.Center;
                    FpSpread1.Sheets[0].Cells[FpSpread1.Sheets[0].RowCount - 1, 2].Font.Size = FontUnit.Medium;
                    FpSpread1.Sheets[0].Cells[FpSpread1.Sheets[0].RowCount - 1, 2].Font.Name = "Book Antiqua";

                    FpSpread1.Sheets[0].Cells[FpSpread1.Sheets[0].RowCount - 1, 3].Text = Convert.ToString(ds1.Tables[0].Rows[i]["ItemName"]);
                    FpSpread1.Sheets[0].Cells[FpSpread1.Sheets[0].RowCount - 1, 3].Tag = Convert.ToString(ds1.Tables[0].Rows[i]["ItemCode"]);
                    FpSpread1.Sheets[0].Cells[FpSpread1.Sheets[0].RowCount - 1, 3].Note = Convert.ToString(ds1.Tables[0].Rows[i]["itempk"]);
                    FpSpread1.Sheets[0].Cells[FpSpread1.Sheets[0].RowCount - 1, 3].HorizontalAlign = HorizontalAlign.Left;
                    FpSpread1.Sheets[0].Cells[FpSpread1.Sheets[0].RowCount - 1, 3].Font.Size = FontUnit.Medium;
                    FpSpread1.Sheets[0].Cells[FpSpread1.Sheets[0].RowCount - 1, 3].Font.Name = "Book Antiqua";

                    //FpSpread1.Sheets[0].Cells[FpSpread1.Sheets[0].RowCount - 1, 21].HorizontalAlign = HorizontalAlign.Center;
                    //FpSpread1.Sheets[0].Cells[FpSpread1.Sheets[0].RowCount - 1, 21].Font.Size = FontUnit.Medium;
                    //FpSpread1.Sheets[0].Cells[FpSpread1.Sheets[0].RowCount - 1, 21].Font.Name = "Book Antiqua";
                }
                FpSpread1.Sheets[0].PageSize = FpSpread1.Sheets[0].RowCount;
                FpSpread1.Sheets[0].SetColumnMerge(2, FarPoint.Web.Spread.Model.MergePolicy.Always);
                FpSpread1.Sheets[0].FrozenRowCount = 0;
                FpSpread1.Visible = true;
                //spreaddiv.Visible = true;
                btnmain_save.Visible = true;
                btn_exit.Visible = true;
                btnmain_save.Text = "Save";
                btn_delete.Visible = false;
                lbl_error.Visible = false;
                FpSpread1.SaveChanges();
                // rptprint.Visible = true;

            }
            else
            {
                lbl_error.Visible = true;
                FpSpread1.Visible = false;
                btnmain_save.Visible = false;
                btn_exit.Visible = false;
                // spreaddiv.Visible = false;
                lbl_error.Text = "No Record Found";
                //  rptprint.Visible = false;
            }

        }
        catch { }
    }
    protected void imagebtnpopclose_Click(object sender, EventArgs e)
    {
        popwindow2.Visible = false;
    }
    protected void btnType_Click(object sender, FarPoint.Web.Spread.SpreadCommandEventArgs e)
    {
        try
        {
            FpSpread1.SaveChanges();
            string actrow = e.SheetView.ActiveRow.ToString();
            string actcol = e.SheetView.ActiveColumn.ToString();
            string itemcode = "";
            string qty = "";
            //Added By Saranyadevi 4.4.2018
            string rpu = string.Empty;
            string discount = string.Empty;
            string tax = string.Empty;
            string extax = string.Empty;
            string edcess = string.Empty;
            string hiedcess = string.Empty;
            string otchar = string.Empty;
            string Dis = string.Empty;
            string totcost = string.Empty;
            if (actrow.Trim() != "" && actcol.Trim() != "" && actrow.Trim() != "0")
            {
                itemcode = Convert.ToString(FpSpread1.Sheets[0].Cells[Convert.ToInt32(actrow), 3].Tag);
                qty = Convert.ToString(FpSpread1.Sheets[0].Cells[Convert.ToInt32(actrow), 4].Text);
                rpu = Convert.ToString(FpSpread1.Sheets[0].Cells[Convert.ToInt32(actrow), 5].Text);
                discount = Convert.ToString(FpSpread1.Sheets[0].Cells[Convert.ToInt32(actrow), 6].Text);
                tax = Convert.ToString(FpSpread1.Sheets[0].Cells[Convert.ToInt32(actrow), 7].Text);
                totcost = Convert.ToString(FpSpread1.Sheets[0].Cells[Convert.ToInt32(actrow), 8].Text);
                extax = Convert.ToString(FpSpread1.Sheets[0].Cells[Convert.ToInt32(actrow), 9].Text);
                edcess = Convert.ToString(FpSpread1.Sheets[0].Cells[Convert.ToInt32(actrow), 10].Text);
                hiedcess = Convert.ToString(FpSpread1.Sheets[0].Cells[Convert.ToInt32(actrow), 11].Text);
                otchar = Convert.ToString(FpSpread1.Sheets[0].Cells[Convert.ToInt32(actrow), 12].Text);
                Dis = Convert.ToString(FpSpread1.Sheets[0].Cells[Convert.ToInt32(actrow), 13].Text);
                Session["actrow"] = actrow;
                Session["actcol"] = actcol;
                popbtntypediv.Visible = true;
                txtpop1qnty.Text = qty;
                txtpop1rateunit.Text = rpu;
                txtpop1dia.Text = discount;
                txtpop1tax.Text = tax;
                txtpop1exetax.Text = extax;
                txtpop1educess.Text = edcess;
                txtpop1otherchar.Text = otchar;
                txtpop1des.Text = Dis;
                txtpop1totalcost.Text = totcost;
            }
        }
        catch
        {


        }
    }
    protected void btnmainsave_Click(object sender, EventArgs e)
    {
        try
        {
            bool chk = false;
            if (FpSpread1.Sheets[0].RowCount > 0)
            {
                int VenQuotqueryins = 0;
                int VednorQuotDetins = 0;
                if (txt_venquano.Text.Trim() != "")
                {
                    FpSpread1.SaveChanges();
                    for (int row = 1; row < FpSpread1.Sheets[0].RowCount; row++)
                    {
                        int checkval = Convert.ToInt32(FpSpread1.Sheets[0].Cells[row, 1].Value);
                        if (checkval == 1)
                        {
                            string itemfk = Convert.ToString(FpSpread1.Sheets[0].Cells[row, 3].Note);
                            string qty = FpSpread1.Sheets[0].Cells[row, 4].Text;
                            if (qty.Trim() == "")
                            {
                                qty = "0";
                            }
                            string rpu = FpSpread1.Sheets[0].Cells[row, 5].Text;
                            if (rpu.Trim() == "")
                            {
                                rpu = "0";
                            }
                            string discount = FpSpread1.Sheets[0].Cells[row, 6].Text;
                            if (discount.Trim() == "")
                            {
                                discount = "0";
                            }
                            string tax = FpSpread1.Sheets[0].Cells[row, 7].Text;
                            if (tax.Trim() == "")
                            {
                                tax = "0";
                            }
                            string cost = FpSpread1.Sheets[0].Cells[row, 8].Text;
                            if (cost.Trim() == "")
                            {
                                cost = "0";
                            }
                            string extax = FpSpread1.Sheets[0].Cells[row, 9].Text;
                            if (extax.Trim() == "")
                            {
                                extax = "0";
                            }
                            string educess = FpSpread1.Sheets[0].Cells[row, 10].Text;
                            if (educess.Trim() == "")
                            {
                                educess = "0";
                            }
                            string higheducss = FpSpread1.Sheets[0].Cells[row, 11].Text;
                            if (higheducss.Trim() == "")
                            {
                                higheducss = "0";
                            }
                            string othercharge = FpSpread1.Sheets[0].Cells[row, 12].Text;
                            if (othercharge.Trim() == "")
                            {
                                othercharge = "0";
                            }
                            string calledu = FpSpread1.Sheets[0].Cells[row, 13].Text;
                            if (calledu.Trim() == "")
                            {
                                calledu = "0";
                            }
                            string callexe = FpSpread1.Sheets[0].Cells[row, 14].Text;
                            if (callexe.Trim() == "")
                            {
                                callexe = "0";
                            }
                            string decription = FpSpread1.Sheets[0].Cells[row, 15].Text;
                            if (decription.Trim() == "")
                            {
                                decription = "";
                            }
                            string deliverydate = FpSpread1.Sheets[0].Cells[row, 16].Text;

                            string VenQuotquery = " if exists(select*from IT_VendorQuot where VendorFK='" + Convert.ToString(Session["vendorpk"]) + "' and VenQuotDate='" + Convert.ToString(DateTime.Now.ToString("MM/dd/yyyy")) + "' and ReqFK='" + Convert.ToString(ddl_requestcode.SelectedItem.Value) + "' and VenReqFK='" + Convert.ToString(ddl_vendorreqcode.SelectedItem.Value) + "')update IT_VendorQuot set VenQuotDate='" + Convert.ToString(DateTime.Now.ToString("MM/dd/yyyy")) + "',VenQuotType='2',VendorFK='" + Convert.ToString(Session["vendorpk"]) + "', VenQuotNo='" + txt_venquano.Text + "',InwardStatus='0',ReqFK='" + Convert.ToString(ddl_requestcode.SelectedItem.Value) + "' ,VenReqFK='" + Convert.ToString(ddl_vendorreqcode.SelectedItem.Value) + "' where VendorFK='" + Convert.ToString(Session["vendorpk"]) + "' and VenQuotDate='" + Convert.ToString(DateTime.Now.ToString("MM/dd/yyyy")) + "' and ReqFK='" + Convert.ToString(ddl_requestcode.SelectedItem.Value) + "' and VenReqFK='" + Convert.ToString(ddl_vendorreqcode.SelectedItem.Value) + "' else insert into IT_VendorQuot (VenQuotCode,VenQuotDate,VenQuotType,VendorFK, VenQuotNo,InwardStatus,ReqFK,VenReqFK)values('" + txt_reqcode.Text + "','" + Convert.ToString(DateTime.Now.ToString("MM/dd/yyyy")) + "','2','" + Convert.ToString(Session["vendorpk"]) + "','" + txt_venquano.Text + "','0','" + Convert.ToString(ddl_requestcode.SelectedItem.Value) + "','" + Convert.ToString(ddl_vendorreqcode.SelectedItem.Value) + "')";
                            VenQuotqueryins = d2.update_method_wo_parameter(VenQuotquery, "Text");

                            string getvenquapk = d2.GetFunction("select VendorQuotPK from IT_VendorQuot where vendorfk='" + Convert.ToString(Session["vendorpk"]) + "'");
                            string VednorQuotDetquery = "if exists(select*from IT_VednorQuotDet where itemfk='" + itemfk + "' and VendorQuotFK='" + getvenquapk + "')update IT_VednorQuotDet set ItemFK='" + itemfk + "',Qty='" + qty + "',RPU='" + rpu + "',DiscountAmt='" + discount + "', TaxPercent='" + tax + "',EduCessPer='" + educess + "',HigherEduCessPer='" + higheducss + "', ExeciseTaxPer='" + extax + "', OtherChargeAmt='" + othercharge + "',OtherChargeDesc='" + decription + "' where ItemFK='" + itemfk + "' and VendorQuotFK='" + getvenquapk + "' else insert into IT_VednorQuotDet (ItemFK,Qty ,RPU,DiscountAmt, TaxPercent,EduCessPer,HigherEduCessPer, ExeciseTaxPer, OtherChargeAmt,VendorQuotFK,OtherChargeDesc) values ('" + itemfk + "', '" + qty + "','" + rpu + "','" + discount + "','" + tax + "','" + educess + "','" + higheducss + "','" + extax + "','" + othercharge + "','" + getvenquapk + "','" + decription + "')";
                            VednorQuotDetins = d2.update_method_wo_parameter(VednorQuotDetquery, "text");
                            chk = true;
                        }
                        if (chk == false)
                        {
                            lbl_alerterror.Visible = true;
                            lbl_alerterror.Text = "Please Select Item";
                            alertmessage.Visible = true;
                        }
                    }
                }
                else
                {
                    lbl_alerterror.Visible = true;
                    lbl_alerterror.Text = "Please Enter Quotation No";
                    alertmessage.Visible = true;
                }

                if (VenQuotqueryins != 0 && VednorQuotDetins != 0)
                {
                    if (btnmain_save.Text == "Update")
                    {
                        lbl_alerterror.Visible = true;
                        lbl_alerterror.Text = "Update Successfully";
                        alertmessage.Visible = true;
                        popwindow2.Visible = false;
                        txt_searchvendor.Text = "";
                        txt_venquano.Text = "";
                    }
                    else
                    {
                        lbl_alerterror.Visible = true;
                        lbl_alerterror.Text = "Saved Successfully";
                        alertmessage.Visible = true;
                        popwindow2.Visible = false;
                        txt_searchvendor.Text = "";
                        txt_venquano.Text = "";
                        btn_go_Click(sender, e);
                    }
                    Clear();
                }
                else
                {
                }
            }
            vendor();
            bindquocode();
            btn_basego_Click(sender, e);
        }
        catch
        { }
    }
    protected void btn_errorclose_Click(object sender, EventArgs e)
    {
        alertmessage.Visible = false;
    }
    protected void btnpopbtntypeok_Click(object sender, EventArgs e)
    {
        try
        {
            string actrow = Convert.ToString(Session["actrow"]);
            string actcol = Convert.ToString(Session["actcol"]);
            if (actcol.Trim() != "" && actrow.Trim() != "")
            {
                if (FpSpread1.Sheets[0].RowCount > 0)
                {
                    FpSpread1.Sheets[0].Cells[Convert.ToInt32(actrow), 4].Text = Convert.ToString(txtpop1qnty.Text);
                    FpSpread1.Sheets[0].Cells[Convert.ToInt32(actrow), 4].HorizontalAlign = HorizontalAlign.Right;
                    FpSpread1.Sheets[0].Cells[Convert.ToInt32(actrow), 5].Text = Convert.ToString(txtpop1rateunit.Text);
                    FpSpread1.Sheets[0].Cells[Convert.ToInt32(actrow), 5].HorizontalAlign = HorizontalAlign.Right;
                    FpSpread1.Sheets[0].Cells[Convert.ToInt32(actrow), 6].Text = Convert.ToString(txtpop1dia.Text);
                    FpSpread1.Sheets[0].Cells[Convert.ToInt32(actrow), 6].HorizontalAlign = HorizontalAlign.Right;
                    FpSpread1.Sheets[0].Cells[Convert.ToInt32(actrow), 7].Text = Convert.ToString(txtpop1tax.Text);
                    FpSpread1.Sheets[0].Cells[Convert.ToInt32(actrow), 7].HorizontalAlign = HorizontalAlign.Right;
                    FpSpread1.Sheets[0].Cells[Convert.ToInt32(actrow), 8].Text = Convert.ToString(txtpop1totalcost.Text);
                    FpSpread1.Sheets[0].Cells[Convert.ToInt32(actrow), 8].HorizontalAlign = HorizontalAlign.Right;
                    FpSpread1.Sheets[0].Cells[Convert.ToInt32(actrow), 9].Text = Convert.ToString(txtpop1exetax.Text);
                    FpSpread1.Sheets[0].Cells[Convert.ToInt32(actrow), 9].HorizontalAlign = HorizontalAlign.Right;
                    FpSpread1.Sheets[0].Cells[Convert.ToInt32(actrow), 10].Text = Convert.ToString(txtpop1educess.Text);
                    FpSpread1.Sheets[0].Cells[Convert.ToInt32(actrow), 10].HorizontalAlign = HorizontalAlign.Right;
                    FpSpread1.Sheets[0].Cells[Convert.ToInt32(actrow), 11].Text = Convert.ToString(txtpop1higher.Text);
                    FpSpread1.Sheets[0].Cells[Convert.ToInt32(actrow), 11].HorizontalAlign = HorizontalAlign.Right;
                    FpSpread1.Sheets[0].Cells[Convert.ToInt32(actrow), 12].Text = Convert.ToString(txtpop1otherchar.Text);
                    FpSpread1.Sheets[0].Cells[Convert.ToInt32(actrow), 12].HorizontalAlign = HorizontalAlign.Right;
                    FpSpread1.Sheets[0].Cells[Convert.ToInt32(actrow), 15].Text = Convert.ToString(txtpop1des.Text);
                    FpSpread1.Sheets[0].Cells[Convert.ToInt32(actrow), 15].HorizontalAlign = HorizontalAlign.Right;
                    FpSpread1.Sheets[0].Cells[Convert.ToInt32(actrow), 16].Text = Convert.ToString(txt_date.Text);
                    FpSpread1.Sheets[0].Cells[Convert.ToInt32(actrow), 16].HorizontalAlign = HorizontalAlign.Center;

                    FpSpread1.Sheets[0].Cells[Convert.ToInt32(actrow), 1].Value = 1;
                    FpSpread1.SaveChanges();
                    popbtntypediv.Visible = false;
                    Clear();
                }
            }
        }
        catch { }

    }
    protected void Clear()
    {
        txtpop1des.Text = "";
        txtpop1dia.Text = "";
        txtpop1educess.Text = "";
        txtpop1exetax.Text = "";
        txtpop1higher.Text = "";
        txtpop1otherchar.Text = "";
        txtpop1qnty.Text = "";
        txtpop1rateunit.Text = "";
        txtpop1tax.Text = "";
        txtpop1totalcost.Text = "";

        //if (FpSpread1.Sheets[0].RowCount > 0)
        //{
        //    for (int i = 0; i < FpSpread1.Sheets[0].RowCount; i++)
        //    {
        //        for (int j = 4; j < FpSpread1.Sheets[0].ColumnCount; j++)
        //        {
        //            FpSpread1.Sheets[0].Cells[i, j].Text = "";
        //        }
        //    }
        //}
    }
    protected void btnpop1Exit_Click(object sender, EventArgs e)
    {
        popbtntypediv.Visible = false;
    }
    protected void imagebtnpopclose1_Click(object sender, EventArgs e)
    {
        popbtntypediv.Visible = false;
    }
    protected void txt_searchvendor_txt_change(object sender, EventArgs e)
    {
        try
        {
            string q1 = d2.GetFunction("select distinct VendorCompName from CO_VendorMaster WHERE VendorCompName='" + txt_searchvendor.Text + "' order by VendorCompName");
            if (q1.Trim() != "" && q1.Trim() != "0")
            {
                lbl_error.Visible = false;
                string q2 = d2.GetFunction("select distinct Vendorcode from CO_VendorMaster WHERE VendorCompName='" + q1 + "'");
                string q3 = d2.GetFunction("select distinct VendorPK from CO_VendorMaster WHERE Vendorcode='" + q2 + "'");
                Session["vendorcode"] = q2;
                Session["vendorpk"] = q3;
            }
            else
            {
                txt_searchvendor.Text = "";
                lbl_error.Visible = true;
                //vendorsearch_div.Visible = false;
                lbl_error.Text = "Please enter the correct Supplier name";
            }
        }
        catch
        { }
    }
    protected void txt_popsearchvendor_txt_change(object sender, EventArgs e)
    {
        try
        {

            string q1 = d2.GetFunction("select distinct vendorcode,VendorCompName from CO_VendorMaster where VendorCompName='" + txt_popsearchvendor.Text + "' order by VendorCompName");
            if (q1.Trim() != "" && q1.Trim() != "0")
            {
                lbl_error2.Visible = false;
                Session["vendorpk"] = d2.GetFunction("select distinct VendorPK from CO_VendorMaster WHERE Vendorcode='" + txt_popsearchvendor.Text + "'");
            }
            else
            {
                txt_popsearchvendor.Text = "";
                lbl_error2.Visible = true;
                vendorsearch_div.Visible = false;
                lbl_error2.Text = "Please enter the correct Supplier name";

            }
        }
        catch
        { }
    }

    protected void btn_popgo_Click(object sender, EventArgs e)
    {
        string vendorname = "";
        if (txt_popsearchvendor.Text.Trim() != "")
        {
            vendorname = " select distinct vendorcode,VendorCompName from CO_VendorMaster where VendorCompName='" + txt_popsearchvendor.Text + "' order by vendorcode";
        }
        else
        {
            vendorname = "select distinct vendorcode,VendorCompName from CO_VendorMaster order by vendorcode";
        }
        ds.Clear();
        ds = d2.select_method_wo_parameter(vendorname, "Text");
        if (ds.Tables[0].Rows.Count > 0)
        {
            FpSpread3.Sheets[0].RowCount = 0;
            FpSpread3.Sheets[0].ColumnCount = 0;
            FpSpread3.CommandBar.Visible = false;
            FpSpread3.Sheets[0].AutoPostBack = true;
            FpSpread3.Sheets[0].ColumnHeader.RowCount = 1;
            FpSpread3.Sheets[0].RowHeader.Visible = false;
            FpSpread3.Sheets[0].ColumnCount = 3;

            FarPoint.Web.Spread.StyleInfo darkstyle = new FarPoint.Web.Spread.StyleInfo();
            darkstyle.BackColor = ColorTranslator.FromHtml("#0CA6CA");
            darkstyle.ForeColor = Color.White;
            FpSpread3.ActiveSheetView.ColumnHeader.DefaultStyle = darkstyle;

            FpSpread3.Sheets[0].ColumnHeader.Cells[0, 0].Text = "S.No";
            FpSpread3.Sheets[0].ColumnHeader.Cells[0, 0].Font.Bold = true;
            FpSpread3.Sheets[0].ColumnHeader.Cells[0, 0].HorizontalAlign = HorizontalAlign.Center;
            FpSpread3.Sheets[0].ColumnHeader.Cells[0, 0].Font.Name = "Book Antiqua";
            FpSpread3.Sheets[0].ColumnHeader.Cells[0, 0].Font.Size = FontUnit.Medium;
            FpSpread3.Columns[0].Width = 50;
            FpSpread3.Columns[0].Locked = true;

            FpSpread3.Sheets[0].ColumnHeader.Cells[0, 1].Text = "Supplier Code";
            FpSpread3.Sheets[0].ColumnHeader.Cells[0, 1].Font.Bold = true;
            FpSpread3.Sheets[0].ColumnHeader.Cells[0, 1].HorizontalAlign = HorizontalAlign.Center;
            FpSpread3.Sheets[0].ColumnHeader.Cells[0, 1].Font.Name = "Book Antiqua";
            FpSpread3.Sheets[0].ColumnHeader.Cells[0, 1].Font.Size = FontUnit.Medium;
            FpSpread3.Columns[1].Width = 150;
            FpSpread3.Columns[1].Locked = true;

            FpSpread3.Sheets[0].ColumnHeader.Cells[0, 2].Text = "Supplier Name";
            FpSpread3.Sheets[0].ColumnHeader.Cells[0, 2].Font.Bold = true;
            FpSpread3.Sheets[0].ColumnHeader.Cells[0, 2].HorizontalAlign = HorizontalAlign.Center;
            FpSpread3.Sheets[0].ColumnHeader.Cells[0, 2].Font.Name = "Book Antiqua";
            FpSpread3.Sheets[0].ColumnHeader.Cells[0, 2].Font.Size = FontUnit.Medium;
            FpSpread3.Columns[2].Width = 350;
            FpSpread3.Columns[2].Locked = true;

            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                FpSpread3.Sheets[0].RowCount++;
                FpSpread3.Sheets[0].Cells[FpSpread3.Sheets[0].RowCount - 1, 0].Text = Convert.ToString(i + 1);
                FpSpread3.Sheets[0].Cells[FpSpread3.Sheets[0].RowCount - 1, 0].HorizontalAlign = HorizontalAlign.Center;
                FpSpread3.Sheets[0].Cells[FpSpread3.Sheets[0].RowCount - 1, 0].Font.Size = FontUnit.Medium;
                FpSpread3.Sheets[0].Cells[FpSpread3.Sheets[0].RowCount - 1, 0].Font.Name = "Book Antiqua";

                FpSpread3.Sheets[0].Cells[FpSpread3.Sheets[0].RowCount - 1, 1].Text = Convert.ToString(ds.Tables[0].Rows[i]["vendorcode"]);
                FpSpread3.Sheets[0].Cells[FpSpread3.Sheets[0].RowCount - 1, 1].HorizontalAlign = HorizontalAlign.Left;
                FpSpread3.Sheets[0].Cells[FpSpread3.Sheets[0].RowCount - 1, 1].Font.Size = FontUnit.Medium;
                FpSpread3.Sheets[0].Cells[FpSpread3.Sheets[0].RowCount - 1, 1].Font.Name = "Book Antiqua";

                FpSpread3.Sheets[0].Cells[FpSpread3.Sheets[0].RowCount - 1, 2].Text = Convert.ToString(ds.Tables[0].Rows[i]["VendorCompName"]);
                FpSpread3.Sheets[0].Cells[FpSpread3.Sheets[0].RowCount - 1, 2].HorizontalAlign = HorizontalAlign.Left;
                FpSpread3.Sheets[0].Cells[FpSpread3.Sheets[0].RowCount - 1, 2].Font.Size = FontUnit.Medium;
                FpSpread3.Sheets[0].Cells[FpSpread3.Sheets[0].RowCount - 1, 2].Font.Name = "Book Antiqua";
            }

            FpSpread3.Sheets[0].PageSize = FpSpread3.Sheets[0].RowCount;
            FpSpread3.Sheets[0].SetColumnMerge(1, FarPoint.Web.Spread.Model.MergePolicy.Always);

            FpSpread3.Visible = true;
            vendorsearch_div.Visible = true;
            lbl_error2.Visible = false;

        }
        else
        {
            FpSpread3.Visible = false;
            vendorsearch_div.Visible = false;
            lbl_error2.Visible = true;
            lbl_error2.Text = "No Record Found";
        }
    }
    protected void FpSpread3_CellClick(object sender, EventArgs e)
    {
        try
        {
            Cellclick = true;
        }
        catch (Exception ex)
        {
        }
    }
    protected void FpSpread3_render(object sender, EventArgs e)
    {
        if (Cellclick == true)
        {
            try
            {
                string activerow = "";
                string activecol = "";
                activerow = FpSpread3.ActiveSheetView.ActiveRow.ToString();
                activecol = FpSpread3.ActiveSheetView.ActiveColumn.ToString();
                if (activerow.Trim() != "" && activecol != "0")
                {
                    string vendorcode = Convert.ToString(FpSpread3.Sheets[0].Cells[Convert.ToInt32(activerow), 1].Text);
                    string vendorname = Convert.ToString(FpSpread3.Sheets[0].Cells[Convert.ToInt32(activerow), 2].Text);

                    txt_searchvendor.Text = vendorname;
                    Session["vendorcode"] = vendorcode;
                    lbl_error.Text = "";
                    pop_vendor.Visible = false;
                }
            }
            catch { }
        }
    }

    [System.Web.Services.WebMethod]
    [System.Web.Script.Services.ScriptMethod()]
    public static List<string> Getname1(string prefixText)
    {
        DAccess2 dn = new DAccess2();
        DataSet dw = new DataSet();
        List<string> name = new List<string>();
        string query = "select distinct VendorCompName from CO_VendorMaster WHERE VendorCompName like '" + prefixText + "%' order by VendorCompName ";
        dw = dn.select_method_wo_parameter(query, "Text");
        if (dw.Tables[0].Rows.Count > 0)
        {
            for (int i = 0; i < dw.Tables[0].Rows.Count; i++)
            {
                name.Add(dw.Tables[0].Rows[i]["VendorCompName"].ToString());
            }
        }
        return name;
    }
    protected void btn_vendorqmark_Click(object sender, EventArgs e)
    {
        pop_vendor.Visible = true;
    }
    protected void imagebtnpopclose3_Click(object sender, EventArgs e)
    {
        pop_vendor.Visible = false;
    }

    protected void cb_vendor_CheckedChanged(object sender, EventArgs e)
    {
        int i = 0;
        int cout = 0;
        txt_vendorname.Text = "--Select--";

        if (cb_vendor.Checked == true)
        {
            cout++;
            for (i = 0; i < cbl_vendor.Items.Count; i++)
            {
                cbl_vendor.Items[i].Selected = true;
            }
            txt_vendorname.Text = "Supplier Name(" + (cbl_vendor.Items.Count) + ")";
        }
        else
        {
            for (i = 0; i < cbl_vendor.Items.Count; i++)
            {
                cbl_vendor.Items[i].Selected = false;
            }
        }
        bindquocode();
    }
    protected void cbl_vendor_SelectedIndexChanged(object sender, EventArgs e)
    {
        int i = 0;
        cb_vendor.Checked = false;

        int commcount = 0;
        txt_vendorname.Text = "--Select--";
        for (i = 0; i < cbl_vendor.Items.Count; i++)
        {
            if (cbl_vendor.Items[i].Selected == true)
            {
                commcount = commcount + 1;
                cb_vendor.Checked = false;
            }
        }
        if (commcount > 0)
        {
            if (commcount == cbl_vendor.Items.Count)
            {
                cb_vendor.Checked = true;
            }
            txt_vendorname.Text = "Supplier Name(" + commcount.ToString() + ")";
        }
        bindquocode();
    }

    protected void cb_quocode_CheckedChanged(object sender, EventArgs e)
    {
        int i = 0;
        int cout = 0;
        txt_basereqcode.Text = "--Select--";
        if (cb_quocode.Checked == true)
        {
            for (i = 0; i < cbl_quocode.Items.Count; i++)
            {
                cbl_quocode.Items[i].Selected = true;
            }
            txt_basereqcode.Text = "Quotation Code(" + (cbl_quocode.Items.Count) + ")";
        }
        else
        {
            for (i = 0; i < cbl_quocode.Items.Count; i++)
            {
                cbl_quocode.Items[i].Selected = false;
            }
        }
        //bindquocode();
    }
    protected void cbl_quocode_SelectedIndexChanged(object sender, EventArgs e)
    {
        int i = 0;
        cb_quocode.Checked = false;
        int commcount = 0;
        txt_basereqcode.Text = "--Select--";
        for (i = 0; i < cbl_quocode.Items.Count; i++)
        {
            if (cbl_quocode.Items[i].Selected == true)
            {
                commcount = commcount + 1;
                cb_quocode.Checked = false;
            }
        }
        if (commcount > 0)
        {
            if (commcount == cbl_quocode.Items.Count)
            {
                cb_quocode.Checked = true;
            }
            txt_basereqcode.Text = "Quotation Code(" + commcount.ToString() + ")";
        }
        // bindquocode();
    }

    protected void btn_basego_Click(object sender, EventArgs e)
    {
        int i = 0;
        string quacode = "";
        Printcontrol.Visible = false;
        for (i = 0; i < cbl_quocode.Items.Count; i++)
        {
            if (cbl_quocode.Items[i].Selected == true)
            {
                string build = cbl_quocode.Items[i].Text.ToString();
                if (quacode == "")
                {
                    quacode = build;
                }
                else
                {
                    quacode = quacode + "'" + "," + "'" + build;
                }
            }
        }
        string venquocode = "";
        for (i = 0; i < cbl_vendor.Items.Count; i++)
        {
            if (cbl_vendor.Items[i].Selected == true)
            {
                string build = cbl_vendor.Items[i].Value.ToString();
                if (venquocode == "")
                {
                    venquocode = build;
                }
                else
                {
                    venquocode = venquocode + "'" + "," + "'" + build;
                }
            }
        }
        string venquoqury = "select vd.VednorQuotDetPK,vd.VendorQuotFK,vm.VendorPK,vm.VendorCompName,vq.VenQuotCode,im.ItemName,vd.Qty,vd.RPU, vd.DiscountAmt,vd.IsDiscountPercent,vd.TaxPercent,vd.ItemFK,vd.ExeciseTaxPer,vd.OtherChargeAmt from IT_VendorQuot vq,IT_VednorQuotDet vd,IM_ItemMaster im, CO_VendorMaster vm where vq.VendorQuotPK=vd.VendorQuotFK and im.ItemPK=vd.ItemFK and vm.VendorPK=vq.vendorfk and VendorFK in('" + venquocode + "') and vq.VenQuotCode in('" + quacode + "')";
        ds = d2.select_method_wo_parameter(venquoqury, "Text");
        if (ds.Tables[0].Rows.Count > 0)
        {
            FpSpread2.Sheets[0].RowCount = 0;
            FpSpread2.Sheets[0].ColumnCount = 0;
            FpSpread2.CommandBar.Visible = false;
            FpSpread2.Sheets[0].AutoPostBack = true;
            FpSpread2.Sheets[0].ColumnHeader.RowCount = 1;
            FpSpread2.Sheets[0].RowHeader.Visible = false;
            FpSpread2.Sheets[0].ColumnCount = 9;

            FarPoint.Web.Spread.StyleInfo darkstyle = new FarPoint.Web.Spread.StyleInfo();
            darkstyle.BackColor = ColorTranslator.FromHtml("#0CA6CA");
            darkstyle.ForeColor = Color.White;
            FpSpread2.ActiveSheetView.ColumnHeader.DefaultStyle = darkstyle;

            FpSpread2.Sheets[0].ColumnHeader.Cells[0, 0].Text = "S.No";
            FpSpread2.Sheets[0].ColumnHeader.Cells[0, 0].Font.Bold = true;
            FpSpread2.Sheets[0].ColumnHeader.Cells[0, 0].HorizontalAlign = HorizontalAlign.Center;
            FpSpread2.Sheets[0].ColumnHeader.Cells[0, 0].Font.Name = "Book Antiqua";
            FpSpread2.Sheets[0].ColumnHeader.Cells[0, 0].Font.Size = FontUnit.Medium;
            FpSpread2.Columns[0].Width = 50;
            FpSpread2.Columns[0].Locked = true;

            FpSpread2.Sheets[0].ColumnHeader.Cells[0, 1].Text = "Supplier Name";
            FpSpread2.Sheets[0].ColumnHeader.Cells[0, 1].Font.Bold = true;
            FpSpread2.Sheets[0].ColumnHeader.Cells[0, 1].HorizontalAlign = HorizontalAlign.Center;
            FpSpread2.Sheets[0].ColumnHeader.Cells[0, 1].Font.Name = "Book Antiqua";
            FpSpread2.Sheets[0].ColumnHeader.Cells[0, 1].Font.Size = FontUnit.Medium;
            FpSpread2.Columns[1].Width = 200;
            FpSpread2.Columns[1].Locked = true;
            FpSpread2.Sheets[0].ColumnHeader.Cells[0, 2].Text = "Quotation Code";
            FpSpread2.Sheets[0].ColumnHeader.Cells[0, 2].Font.Bold = true;
            FpSpread2.Sheets[0].ColumnHeader.Cells[0, 2].HorizontalAlign = HorizontalAlign.Center;
            FpSpread2.Sheets[0].ColumnHeader.Cells[0, 2].Font.Name = "Book Antiqua";
            FpSpread2.Sheets[0].ColumnHeader.Cells[0, 2].Font.Size = FontUnit.Medium;
            FpSpread2.Columns[2].Width = 100;
            FpSpread2.Columns[2].Locked = true;

            FpSpread2.Sheets[0].ColumnHeader.Cells[0, 3].Text = "Item Name";
            FpSpread2.Sheets[0].ColumnHeader.Cells[0, 3].Font.Bold = true;
            FpSpread2.Sheets[0].ColumnHeader.Cells[0, 3].HorizontalAlign = HorizontalAlign.Center;
            FpSpread2.Sheets[0].ColumnHeader.Cells[0, 3].Font.Name = "Book Antiqua";
            FpSpread2.Sheets[0].ColumnHeader.Cells[0, 3].Font.Size = FontUnit.Medium;
            FpSpread2.Columns[3].Width = 200;
            FpSpread2.Columns[3].Locked = true;
            FpSpread2.Sheets[0].ColumnHeader.Cells[0, 4].Text = "Quantity";
            FpSpread2.Sheets[0].ColumnHeader.Cells[0, 4].Font.Bold = true;
            FpSpread2.Sheets[0].ColumnHeader.Cells[0, 4].HorizontalAlign = HorizontalAlign.Center;
            FpSpread2.Sheets[0].ColumnHeader.Cells[0, 4].Font.Name = "Book Antiqua";
            FpSpread2.Sheets[0].ColumnHeader.Cells[0, 4].Font.Size = FontUnit.Medium;
            FpSpread2.Columns[4].Width = 100;
            FpSpread2.Columns[4].Locked = true;
            FpSpread2.Sheets[0].ColumnHeader.Cells[0, 5].Text = "Rate Per Unit";
            FpSpread2.Sheets[0].ColumnHeader.Cells[0, 5].Font.Bold = true;
            FpSpread2.Sheets[0].ColumnHeader.Cells[0, 5].HorizontalAlign = HorizontalAlign.Center;
            FpSpread2.Sheets[0].ColumnHeader.Cells[0, 5].Font.Name = "Book Antiqua";
            FpSpread2.Sheets[0].ColumnHeader.Cells[0, 5].Font.Size = FontUnit.Medium;
            FpSpread2.Columns[5].Width = 100;
            FpSpread2.Columns[5].Locked = true;

            FpSpread2.Sheets[0].ColumnHeader.Cells[0, 6].Text = "Discount Amount";
            FpSpread2.Sheets[0].ColumnHeader.Cells[0, 6].Font.Bold = true;
            FpSpread2.Sheets[0].ColumnHeader.Cells[0, 6].HorizontalAlign = HorizontalAlign.Center;
            FpSpread2.Sheets[0].ColumnHeader.Cells[0, 6].Font.Name = "Book Antiqua";
            FpSpread2.Sheets[0].ColumnHeader.Cells[0, 6].Font.Size = FontUnit.Medium;
            FpSpread2.Columns[6].Width = 100;
            FpSpread2.Columns[6].Locked = true;
            FpSpread2.Sheets[0].ColumnHeader.Cells[0, 7].Text = "Tax";
            FpSpread2.Sheets[0].ColumnHeader.Cells[0, 7].Font.Bold = true;
            FpSpread2.Sheets[0].ColumnHeader.Cells[0, 7].HorizontalAlign = HorizontalAlign.Center;
            FpSpread2.Sheets[0].ColumnHeader.Cells[0, 7].Font.Name = "Book Antiqua";
            FpSpread2.Sheets[0].ColumnHeader.Cells[0, 7].Font.Size = FontUnit.Medium;
            FpSpread2.Columns[7].Width = 100;
            FpSpread2.Columns[7].Locked = true;
            FpSpread2.Sheets[0].ColumnHeader.Cells[0, 8].Text = "Cost";
            FpSpread2.Sheets[0].ColumnHeader.Cells[0, 8].Font.Bold = true;
            FpSpread2.Sheets[0].ColumnHeader.Cells[0, 8].HorizontalAlign = HorizontalAlign.Center;
            FpSpread2.Sheets[0].ColumnHeader.Cells[0, 8].Font.Name = "Book Antiqua";
            FpSpread2.Sheets[0].ColumnHeader.Cells[0, 8].Font.Size = FontUnit.Medium;
            FpSpread2.Columns[8].Width = 100;
            FpSpread2.Columns[8].Locked = true;

            for (i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                FpSpread2.Sheets[0].RowCount++;
                FpSpread2.Sheets[0].Cells[FpSpread2.Sheets[0].RowCount - 1, 0].Text = Convert.ToString(i + 1);
                FpSpread2.Sheets[0].Cells[FpSpread2.Sheets[0].RowCount - 1, 0].HorizontalAlign = HorizontalAlign.Center;
                FpSpread2.Sheets[0].Cells[FpSpread2.Sheets[0].RowCount - 1, 0].Font.Size = FontUnit.Medium;
                FpSpread2.Sheets[0].Cells[FpSpread2.Sheets[0].RowCount - 1, 0].Font.Name = "Book Antiqua";

                FpSpread2.Sheets[0].Cells[FpSpread2.Sheets[0].RowCount - 1, 1].Text = Convert.ToString(ds.Tables[0].Rows[i]["VendorCompName"]);
                FpSpread2.Sheets[0].Cells[FpSpread2.Sheets[0].RowCount - 1, 1].Tag = Convert.ToString(ds.Tables[0].Rows[i]["VendorQuotFK"]);
                FpSpread2.Sheets[0].Cells[FpSpread2.Sheets[0].RowCount - 1, 1].Note = Convert.ToString(ds.Tables[0].Rows[i]["VendorPK"]);
                FpSpread2.Sheets[0].Cells[FpSpread2.Sheets[0].RowCount - 1, 1].HorizontalAlign = HorizontalAlign.Left;
                FpSpread2.Sheets[0].Cells[FpSpread2.Sheets[0].RowCount - 1, 1].Font.Size = FontUnit.Medium;
                FpSpread2.Sheets[0].Cells[FpSpread2.Sheets[0].RowCount - 1, 1].Font.Name = "Book Antiqua";

                FpSpread2.Sheets[0].Cells[FpSpread2.Sheets[0].RowCount - 1, 2].Text = Convert.ToString(ds.Tables[0].Rows[i]["VenQuotCode"]);
                FpSpread2.Sheets[0].Cells[FpSpread2.Sheets[0].RowCount - 1, 2].Tag = Convert.ToString(ds.Tables[0].Rows[i]["VednorQuotDetPK"]);
                FpSpread2.Sheets[0].Cells[FpSpread2.Sheets[0].RowCount - 1, 2].HorizontalAlign = HorizontalAlign.Left;
                FpSpread2.Sheets[0].Cells[FpSpread2.Sheets[0].RowCount - 1, 2].Font.Size = FontUnit.Medium;
                FpSpread2.Sheets[0].Cells[FpSpread2.Sheets[0].RowCount - 1, 2].Font.Name = "Book Antiqua";


                FpSpread2.Sheets[0].Cells[FpSpread2.Sheets[0].RowCount - 1, 3].Text = Convert.ToString(ds.Tables[0].Rows[i]["itemname"]);
                FpSpread2.Sheets[0].Cells[FpSpread2.Sheets[0].RowCount - 1, 3].HorizontalAlign = HorizontalAlign.Left;
                FpSpread2.Sheets[0].Cells[FpSpread2.Sheets[0].RowCount - 1, 3].Font.Size = FontUnit.Medium;
                FpSpread2.Sheets[0].Cells[FpSpread2.Sheets[0].RowCount - 1, 3].Font.Name = "Book Antiqua";

                FpSpread2.Sheets[0].Cells[FpSpread2.Sheets[0].RowCount - 1, 4].Text = Convert.ToString(ds.Tables[0].Rows[i]["Qty"]);
                FpSpread2.Sheets[0].Cells[FpSpread2.Sheets[0].RowCount - 1, 4].HorizontalAlign = HorizontalAlign.Right;
                FpSpread2.Sheets[0].Cells[FpSpread2.Sheets[0].RowCount - 1, 4].Font.Size = FontUnit.Medium;
                FpSpread2.Sheets[0].Cells[FpSpread2.Sheets[0].RowCount - 1, 4].Font.Name = "Book Antiqua";

                FpSpread2.Sheets[0].Cells[FpSpread2.Sheets[0].RowCount - 1, 5].Text = Convert.ToString(ds.Tables[0].Rows[i]["RPU"]);
                FpSpread2.Sheets[0].Cells[FpSpread2.Sheets[0].RowCount - 1, 5].HorizontalAlign = HorizontalAlign.Right;
                FpSpread2.Sheets[0].Cells[FpSpread2.Sheets[0].RowCount - 1, 5].Font.Size = FontUnit.Medium;
                FpSpread2.Sheets[0].Cells[FpSpread2.Sheets[0].RowCount - 1, 5].Font.Name = "Book Antiqua";

                FpSpread2.Sheets[0].Cells[FpSpread2.Sheets[0].RowCount - 1, 6].Text = Convert.ToString(ds.Tables[0].Rows[i]["DiscountAmt"]);
                FpSpread2.Sheets[0].Cells[FpSpread2.Sheets[0].RowCount - 1, 6].HorizontalAlign = HorizontalAlign.Right;
                FpSpread2.Sheets[0].Cells[FpSpread2.Sheets[0].RowCount - 1, 6].Font.Size = FontUnit.Medium;
                FpSpread2.Sheets[0].Cells[FpSpread2.Sheets[0].RowCount - 1, 6].Font.Name = "Book Antiqua";

                FpSpread2.Sheets[0].Cells[FpSpread2.Sheets[0].RowCount - 1, 7].Text = Convert.ToString(ds.Tables[0].Rows[i]["TaxPercent"]);
                FpSpread2.Sheets[0].Cells[FpSpread2.Sheets[0].RowCount - 1, 7].Tag = Convert.ToString(ds.Tables[0].Rows[i]["ItemFK"]);

                FpSpread2.Sheets[0].Cells[FpSpread2.Sheets[0].RowCount - 1, 7].HorizontalAlign = HorizontalAlign.Right;
                FpSpread2.Sheets[0].Cells[FpSpread2.Sheets[0].RowCount - 1, 7].Font.Size = FontUnit.Medium;
                FpSpread2.Sheets[0].Cells[FpSpread2.Sheets[0].RowCount - 1, 7].Font.Name = "Book Antiqua";

                double qty = Convert.ToDouble(ds.Tables[0].Rows[i]["Qty"]);
                double rpu = Convert.ToDouble(ds.Tables[0].Rows[i]["RPU"]);
                string discountamt = Convert.ToString(ds.Tables[0].Rows[i]["DiscountAmt"]);
                string discountper = Convert.ToString(ds.Tables[0].Rows[i]["IsDiscountPercent"]);
                double discount = 0;
                if (discountamt.Trim() == "0" || discountamt.Trim() == null || discountamt.Trim() == "")
                {
                    discount = 0;
                }

                if (discountper.Trim() == "0" || discountper.Trim() == null || discountper.Trim() == "")
                {
                    discount = 0;
                }
                double tax = Convert.ToDouble(ds.Tables[0].Rows[i]["TaxPercent"]);
                double extax = Convert.ToDouble(ds.Tables[0].Rows[i]["ExeciseTaxPer"]);
                double otherharge = Convert.ToDouble(ds.Tables[0].Rows[i]["OtherChargeAmt"]);
                double cost = 0;
                double dis = 0;
                cost = qty * rpu;
                if (discountamt.Trim() != "")
                {
                    cost = cost - Convert.ToDouble(discountamt);
                }
                if (discountper.Trim() != "")
                {
                    dis = (cost / 100) * Convert.ToDouble(discountper);
                    cost = cost - dis;
                }
                if (tax != 0)
                {
                    double t = (cost / 100) * tax;
                    cost = cost + t;
                }
                if (extax != 0)
                {
                    double ex = cost / 100 * extax;
                    cost = cost + ex;
                }

                cost = cost + otherharge;

                FpSpread2.Sheets[0].Cells[FpSpread2.Sheets[0].RowCount - 1, 8].Text = Convert.ToString(Math.Round(cost, 2));
                FpSpread2.Sheets[0].Cells[FpSpread2.Sheets[0].RowCount - 1, 8].HorizontalAlign = HorizontalAlign.Right;
                FpSpread2.Sheets[0].Cells[FpSpread2.Sheets[0].RowCount - 1, 8].Font.Size = FontUnit.Medium;
                FpSpread2.Sheets[0].Cells[FpSpread2.Sheets[0].RowCount - 1, 8].Font.Name = "Book Antiqua";
            }
            FpSpread2.Sheets[0].PageSize = FpSpread2.Sheets[0].RowCount;
            FpSpread2.Sheets[0].SetColumnMerge(1, FarPoint.Web.Spread.Model.MergePolicy.Always);
            FpSpread2.Sheets[0].SetColumnMerge(2, FarPoint.Web.Spread.Model.MergePolicy.Always);
            FpSpread2.Columns[1].VerticalAlign = VerticalAlign.Middle;
            FpSpread2.Columns[2].VerticalAlign = VerticalAlign.Middle;
            FpSpread2.Sheets[0].FrozenRowCount = 0;
            lbl_base_error.Visible = false;
            FpSpread2.Visible = true;
            rptprint.Visible = true;
            spreaddiv1.Visible = true;
            FpSpread2.SaveChanges();
        }
        else
        {
            lbl_base_error.Visible = true;
            FpSpread2.Visible = false;
            rptprint.Visible = false;
            spreaddiv1.Visible = false;
            lbl_base_error.Text = "No Record Founds";

        }
    }

    protected void btn_exit_Click(object sender, EventArgs e)
    {
        popwindow2.Visible = false;
    }
    protected void Cell_Click(object sender, EventArgs e)
    {
        try
        {
            check = true;
        }
        catch
        {

        }
    }
    protected void updatespread()
    {
        FpSpread1.Sheets[0].RowCount = 0;
        FpSpread1.Sheets[0].ColumnCount = 0;
        FpSpread1.CommandBar.Visible = false;
        FpSpread1.Sheets[0].AutoPostBack = false;
        FpSpread1.Sheets[0].ColumnHeader.RowCount = 1;
        FpSpread1.Sheets[0].RowHeader.Visible = false;
        FpSpread1.Sheets[0].ColumnCount = 17;
        FarPoint.Web.Spread.StyleInfo darkstyle = new FarPoint.Web.Spread.StyleInfo();
        darkstyle.BackColor = ColorTranslator.FromHtml("#0CA6CA");
        darkstyle.ForeColor = Color.White;
        FpSpread1.ActiveSheetView.ColumnHeader.DefaultStyle = darkstyle;
        FpSpread1.Sheets[0].ColumnHeader.Cells[0, 0].Text = "S.No";
        FpSpread1.Sheets[0].ColumnHeader.Cells[0, 0].Font.Bold = true;
        FpSpread1.Sheets[0].ColumnHeader.Cells[0, 0].Font.Name = "Book Antiqua";
        FpSpread1.Sheets[0].ColumnHeader.Cells[0, 0].Font.Size = FontUnit.Medium;
        FpSpread1.Sheets[0].ColumnHeader.Cells[0, 0].HorizontalAlign = HorizontalAlign.Center;
        FpSpread1.Columns[0].Locked = true;
        FpSpread1.Columns[0].Width = 37;
        FpSpread1.Sheets[0].ColumnHeader.Cells[0, 1].Text = "Select";
        FpSpread1.Sheets[0].ColumnHeader.Cells[0, 1].Font.Bold = true;
        FpSpread1.Sheets[0].ColumnHeader.Cells[0, 1].Font.Name = "Book Antiqua";
        FpSpread1.Sheets[0].ColumnHeader.Cells[0, 1].Font.Size = FontUnit.Medium;
        FpSpread1.Sheets[0].ColumnHeader.Cells[0, 1].HorizontalAlign = HorizontalAlign.Center;
        //FpSpread1.Columns[1].Locked = true;
        FpSpread1.Columns[1].Width = 40;

        FpSpread1.Sheets[0].ColumnHeader.Cells[0, 2].Text = "Edit";
        FpSpread1.Sheets[0].ColumnHeader.Cells[0, 2].Font.Bold = true;
        FpSpread1.Sheets[0].ColumnHeader.Cells[0, 2].Font.Name = "Book Antiqua";
        FpSpread1.Sheets[0].ColumnHeader.Cells[0, 2].Font.Size = FontUnit.Medium;
        FpSpread1.Sheets[0].ColumnHeader.Cells[0, 2].HorizontalAlign = HorizontalAlign.Center;
        FpSpread1.Columns[2].Width = 45;
        FpSpread1.Sheets[0].ColumnHeader.Cells[0, 3].Text = "Item Name";
        FpSpread1.Sheets[0].ColumnHeader.Cells[0, 3].Font.Bold = true;
        FpSpread1.Sheets[0].ColumnHeader.Cells[0, 3].Font.Name = "Book Antiqua";
        FpSpread1.Sheets[0].ColumnHeader.Cells[0, 3].Font.Size = FontUnit.Medium;
        FpSpread1.Sheets[0].ColumnHeader.Cells[0, 3].HorizontalAlign = HorizontalAlign.Center;
        FpSpread1.Columns[3].Locked = true;
        FpSpread1.Columns[3].Width = 100;
        FpSpread1.Sheets[0].ColumnHeader.Cells[0, 4].Text = "Qty";
        FpSpread1.Sheets[0].ColumnHeader.Cells[0, 4].Font.Bold = true;
        FpSpread1.Sheets[0].ColumnHeader.Cells[0, 4].Font.Name = "Book Antiqua";
        FpSpread1.Sheets[0].ColumnHeader.Cells[0, 4].Font.Size = FontUnit.Medium;
        FpSpread1.Sheets[0].ColumnHeader.Cells[0, 4].HorizontalAlign = HorizontalAlign.Center;
        FpSpread1.Columns[4].Width = 50;
        FpSpread1.Columns[4].Locked = true;
        FpSpread1.Sheets[0].ColumnHeader.Cells[0, 5].Text = "RPU";
        FpSpread1.Sheets[0].ColumnHeader.Cells[0, 5].Font.Bold = true;
        FpSpread1.Sheets[0].ColumnHeader.Cells[0, 5].Font.Name = "Book Antiqua";
        FpSpread1.Sheets[0].ColumnHeader.Cells[0, 5].Font.Size = FontUnit.Medium;
        FpSpread1.Sheets[0].ColumnHeader.Cells[0, 5].HorizontalAlign = HorizontalAlign.Center;
        FpSpread1.Columns[5].Width = 50;
        FpSpread1.Columns[5].Locked = true;
        FpSpread1.Sheets[0].ColumnHeader.Cells[0, 6].Text = "Discount";
        FpSpread1.Sheets[0].ColumnHeader.Cells[0, 6].Font.Bold = true;
        FpSpread1.Sheets[0].ColumnHeader.Cells[0, 6].Font.Name = "Book Antiqua";
        FpSpread1.Sheets[0].ColumnHeader.Cells[0, 6].Font.Size = FontUnit.Medium;
        FpSpread1.Sheets[0].ColumnHeader.Cells[0, 6].HorizontalAlign = HorizontalAlign.Center;
        FpSpread1.Columns[6].Width = 100;
        FpSpread1.Columns[6].Locked = true;
        FpSpread1.Sheets[0].ColumnHeader.Cells[0, 7].Text = "Tax(%)";
        FpSpread1.Sheets[0].ColumnHeader.Cells[0, 7].Font.Bold = true;
        FpSpread1.Sheets[0].ColumnHeader.Cells[0, 7].Font.Name = "Book Antiqua";
        FpSpread1.Sheets[0].ColumnHeader.Cells[0, 7].Font.Size = FontUnit.Medium;
        FpSpread1.Sheets[0].ColumnHeader.Cells[0, 7].HorizontalAlign = HorizontalAlign.Center;
        FpSpread1.Columns[7].Width = 100;
        FpSpread1.Columns[7].Locked = true;
        FpSpread1.Sheets[0].ColumnHeader.Cells[0, 8].Text = "Cost";
        FpSpread1.Sheets[0].ColumnHeader.Cells[0, 8].Font.Bold = true;
        FpSpread1.Sheets[0].ColumnHeader.Cells[0, 8].Font.Name = "Book Antiqua";
        FpSpread1.Sheets[0].ColumnHeader.Cells[0, 8].Font.Size = FontUnit.Medium;
        FpSpread1.Sheets[0].ColumnHeader.Cells[0, 8].HorizontalAlign = HorizontalAlign.Center;
        FpSpread1.Columns[8].Width = 150;
        FpSpread1.Columns[8].Locked = true;

        FpSpread1.Sheets[0].ColumnHeader.Cells[0, 9].Text = "Exercies tax";
        FpSpread1.Sheets[0].ColumnHeader.Cells[0, 9].Font.Bold = true;
        FpSpread1.Sheets[0].ColumnHeader.Cells[0, 9].Font.Name = "Book Antiqua";
        FpSpread1.Sheets[0].ColumnHeader.Cells[0, 9].Font.Size = FontUnit.Medium;
        FpSpread1.Sheets[0].ColumnHeader.Cells[0, 9].HorizontalAlign = HorizontalAlign.Center;
        FpSpread1.Columns[9].Width = 100;
        FpSpread1.Columns[9].Locked = true;
        FpSpread1.Sheets[0].ColumnHeader.Cells[0, 10].Text = "Education Cess";
        FpSpread1.Sheets[0].ColumnHeader.Cells[0, 10].Font.Bold = true;
        FpSpread1.Sheets[0].ColumnHeader.Cells[0, 10].Font.Name = "Book Antiqua";
        FpSpread1.Sheets[0].ColumnHeader.Cells[0, 10].Font.Size = FontUnit.Medium;
        FpSpread1.Sheets[0].ColumnHeader.Cells[0, 10].HorizontalAlign = HorizontalAlign.Center;
        FpSpread1.Columns[10].Width = 100;
        FpSpread1.Columns[10].Locked = true;
        FpSpread1.Sheets[0].ColumnHeader.Cells[0, 11].Text = "Higher Edu.Cess";
        FpSpread1.Sheets[0].ColumnHeader.Cells[0, 11].Font.Bold = true;
        FpSpread1.Sheets[0].ColumnHeader.Cells[0, 11].Font.Name = "Book Antiqua";
        FpSpread1.Sheets[0].ColumnHeader.Cells[0, 11].Font.Size = FontUnit.Medium;
        FpSpread1.Sheets[0].ColumnHeader.Cells[0, 11].HorizontalAlign = HorizontalAlign.Center;
        FpSpread1.Columns[11].Width = 150;
        FpSpread1.Columns[11].Locked = true;
        FpSpread1.Sheets[0].ColumnHeader.Cells[0, 12].Text = "Other Charges";
        FpSpread1.Sheets[0].ColumnHeader.Cells[0, 12].Font.Bold = true;
        FpSpread1.Sheets[0].ColumnHeader.Cells[0, 12].Font.Name = "Book Antiqua";
        FpSpread1.Sheets[0].ColumnHeader.Cells[0, 12].Font.Size = FontUnit.Medium;
        FpSpread1.Sheets[0].ColumnHeader.Cells[0, 12].HorizontalAlign = HorizontalAlign.Center;
        FpSpread1.Columns[12].Width = 150;
        FpSpread1.Columns[12].Locked = true;

        FpSpread1.Sheets[0].ColumnHeader.Cells[0, 13].Text = "CallEduCess";
        FpSpread1.Sheets[0].ColumnHeader.Cells[0, 13].Font.Bold = true;
        FpSpread1.Sheets[0].ColumnHeader.Cells[0, 13].Font.Name = "Book Antiqua";
        FpSpread1.Sheets[0].ColumnHeader.Cells[0, 13].Font.Size = FontUnit.Medium;
        FpSpread1.Sheets[0].ColumnHeader.Cells[0, 13].HorizontalAlign = HorizontalAlign.Center;
        FpSpread1.Columns[13].Width = 150;
        FpSpread1.Columns[13].Locked = true;
        FpSpread1.Columns[13].Visible = false;

        FpSpread1.Sheets[0].ColumnHeader.Cells[0, 14].Text = "CallExTax";
        FpSpread1.Sheets[0].ColumnHeader.Cells[0, 14].Font.Bold = true;
        FpSpread1.Sheets[0].ColumnHeader.Cells[0, 14].Font.Name = "Book Antiqua";
        FpSpread1.Sheets[0].ColumnHeader.Cells[0, 14].Font.Size = FontUnit.Medium;
        FpSpread1.Sheets[0].ColumnHeader.Cells[0, 14].HorizontalAlign = HorizontalAlign.Left;
        FpSpread1.Columns[14].Width = 150;
        FpSpread1.Columns[14].Locked = true;
        FpSpread1.Columns[14].Visible = false;

        FpSpread1.Sheets[0].ColumnHeader.Cells[0, 15].Text = "Description";
        FpSpread1.Sheets[0].ColumnHeader.Cells[0, 15].Font.Bold = true;
        FpSpread1.Sheets[0].ColumnHeader.Cells[0, 15].Font.Name = "Book Antiqua";
        FpSpread1.Sheets[0].ColumnHeader.Cells[0, 15].Font.Size = FontUnit.Medium;
        FpSpread1.Sheets[0].ColumnHeader.Cells[0, 15].HorizontalAlign = HorizontalAlign.Center;
        FpSpread1.Columns[15].Locked = true;

        FpSpread1.Sheets[0].ColumnHeader.Cells[0, 16].Text = "Delivery Date";
        FpSpread1.Sheets[0].ColumnHeader.Cells[0, 16].Font.Bold = true;
        FpSpread1.Sheets[0].ColumnHeader.Cells[0, 16].Font.Name = "Book Antiqua";
        FpSpread1.Sheets[0].ColumnHeader.Cells[0, 16].Font.Size = FontUnit.Medium;
        FpSpread1.Sheets[0].ColumnHeader.Cells[0, 16].HorizontalAlign = HorizontalAlign.Left;
        FpSpread1.Columns[16].Locked = true;
        FarPoint.Web.Spread.ButtonCellType btnType = new FarPoint.Web.Spread.ButtonCellType();
        FarPoint.Web.Spread.CheckBoxCellType chkall = new FarPoint.Web.Spread.CheckBoxCellType();
        chkall.AutoPostBack = true;
        FpSpread1.Sheets[0].RowCount++;

        FpSpread1.Sheets[0].Cells[FpSpread1.Sheets[0].RowCount - 1, 1].CellType = chkall;
        FpSpread1.Sheets[0].Cells[FpSpread1.Sheets[0].RowCount - 1, 1].HorizontalAlign = HorizontalAlign.Center;
        FarPoint.Web.Spread.CheckBoxCellType chkbox = new FarPoint.Web.Spread.CheckBoxCellType();
        chkbox.AutoPostBack = false;

        FpSpread1.Sheets[0].RowCount++;
        FpSpread1.Sheets[0].Cells[1, 2].CellType = btnType;
        FpSpread1.Sheets[0].Cells[1, 2].HorizontalAlign = HorizontalAlign.Center;
        FpSpread1.Sheets[0].Cells[1, 1].CellType = chkbox;
        FpSpread1.Sheets[0].Cells[1, 1].HorizontalAlign = HorizontalAlign.Center;
        FpSpread1.Sheets[0].Cells[1, 0].Text = "1";
        btnType.Text = "Edit";
        btnType.CssClass = "textbox btn";
        btnType.ForeColor = Color.Blue;
        lbl_error.Text = "";
        FpSpread1.Visible = true;

    }
    protected void FpSpread2_render(object sender, EventArgs e)
    {
        try
        {
            if (check == true)
            {
                FpSpread2.SaveChanges();
                btn_vendorqmark.Enabled = false;
                btn_go.Enabled = false;
                txt_venquano.Enabled = false;
                txt_searchvendor.Enabled = false;
                btnmain_save.Text = "Update";
                btn_delete.Visible = true;
                btnmain_save.Visible = true;
                btn_exit.Visible = true;

                string activerow = FpSpread2.ActiveSheetView.ActiveRow.ToString();
                string activecol = FpSpread2.ActiveSheetView.ActiveColumn.ToString();
                string VendorQuotFK = Convert.ToString(FpSpread2.Sheets[0].Cells[Convert.ToInt32(activerow), 1].Tag);
                string VendorPK = Convert.ToString(FpSpread2.Sheets[0].Cells[Convert.ToInt32(activerow), 1].Note);
                string VednorQuotDetPK = Convert.ToString(FpSpread2.Sheets[0].Cells[Convert.ToInt32(activerow), 2].Tag);
                string ItemFK = Convert.ToString(FpSpread2.Sheets[0].Cells[Convert.ToInt32(activerow), 7].Tag);

                string updateselectquery = "select vq.VenQuotCode, im.ItemName,vq.VenQuotNo, vd.VednorQuotDetPK,vd.VendorQuotFK,vm.VendorPK, vm.VendorCompName, vq.VenQuotCode,im.ItemName,vd.Qty,vd.RPU,vd.DiscountAmt,vd.IsDiscountPercent,vd.TaxPercent,vd.ItemFK,vd.EduCessPer,HigherEduCessPer,vd.ExeciseTaxPer,vd.OtherChargeAmt,vd.OtherChargeDesc from IT_VendorQuot vq,IT_VednorQuotDet vd,IM_ItemMaster im, CO_VendorMaster vm where vq.VendorQuotPK=vd.VendorQuotFK and im.ItemPK=vd.ItemFK and vm.VendorPK=vq.vendorfk and ItemFK in('" + ItemFK + "') and VendorQuotFK='" + VendorQuotFK + "' and VendorPK='" + VendorPK + "' and VednorQuotDetPK ='" + VednorQuotDetPK + "' and vq.InwardStatus=0";
                ds.Clear();
                ds = d2.select_method_wo_parameter(updateselectquery, "Text");
                if (ds.Tables[0].Rows.Count > 0)
                {
                    updatespread();
                    //int row = Convert.ToInt32(activerow);
                    int row = 0;
                    txt_venquano.Text = Convert.ToString(ds.Tables[0].Rows[row]["VenQuotNo"]);
                    txt_searchvendor.Text = Convert.ToString(ds.Tables[0].Rows[row]["VendorCompName"]);
                    txt_reqcode.Text = Convert.ToString(ds.Tables[0].Rows[row]["VenQuotCode"]);

                    FpSpread1.Sheets[0].Cells[1, 3].Text = Convert.ToString(ds.Tables[0].Rows[row]["ItemName"]);
                    FpSpread1.Sheets[0].Cells[1, 3].Note = ItemFK;
                    Session["vendorpk"] = Convert.ToString(ds.Tables[0].Rows[row]["VendorPK"]);
                    FpSpread1.Sheets[0].Cells[1, 3].HorizontalAlign = HorizontalAlign.Left;
                    FpSpread1.Sheets[0].Cells[1, 4].Text = Convert.ToString(ds.Tables[0].Rows[row]["Qty"]);
                    FpSpread1.Sheets[0].Cells[1, 4].Tag = Convert.ToString(ds.Tables[0].Rows[row]["VendorQuotFK"]);
                    FpSpread1.Sheets[0].Cells[1, 4].HorizontalAlign = HorizontalAlign.Right;
                    FpSpread1.Sheets[0].Cells[1, 5].Text = Convert.ToString(ds.Tables[0].Rows[row]["RPU"]);
                    FpSpread1.Sheets[0].Cells[1, 5].HorizontalAlign = HorizontalAlign.Right;

                    double dispercal = 0;
                    double disamtcal = 0;
                    string disper = Convert.ToString(ds.Tables[0].Rows[row]["IsDiscountPercent"]);
                    string disamt = Convert.ToString(ds.Tables[0].Rows[row]["DiscountAmt"]);
                    string discount = "";
                    if (disper.Trim() != "" && disper.Trim() != null)
                    {
                        discount = disper;
                        dispercal = Convert.ToDouble(disper);
                    }
                    else if (disamt.Trim() != "" && disamt.Trim() != null)
                    {
                        discount = disamt;
                        disamtcal = Convert.ToDouble(disamt);
                    }
                    else
                    {
                        discount = "0";
                    }
                    FpSpread1.Sheets[0].Cells[1, 6].Text = Convert.ToString(discount);
                    FpSpread1.Sheets[0].Cells[1, 6].HorizontalAlign = HorizontalAlign.Right;
                    FpSpread1.Sheets[0].Cells[1, 7].Text = Convert.ToString(ds.Tables[0].Rows[row]["TaxPercent"]);
                    FpSpread1.Sheets[0].Cells[1, 7].HorizontalAlign = HorizontalAlign.Right;
                    //FpSpread1.Sheets[0].Cells[0, 8].Text = Convert.ToString(ds.Tables[0].Rows[row]["RPU"]);
                    //FpSpread1.Sheets[0].Cells[0, 8].HorizontalAlign = HorizontalAlign.Right;
                    FpSpread1.Sheets[0].Cells[1, 9].Text = Convert.ToString(ds.Tables[0].Rows[row]["ExeciseTaxPer"]);
                    FpSpread1.Sheets[0].Cells[1, 9].HorizontalAlign = HorizontalAlign.Right;
                    FpSpread1.Sheets[0].Cells[1, 10].Text = Convert.ToString(ds.Tables[0].Rows[row]["EduCessPer"]);
                    FpSpread1.Sheets[0].Cells[1, 10].HorizontalAlign = HorizontalAlign.Right;
                    FpSpread1.Sheets[0].Cells[1, 11].Text = Convert.ToString(ds.Tables[0].Rows[row]["HigherEduCessPer"]);
                    FpSpread1.Sheets[0].Cells[1, 11].HorizontalAlign = HorizontalAlign.Right;
                    FpSpread1.Sheets[0].Cells[1, 12].Text = Convert.ToString(ds.Tables[0].Rows[row]["OtherChargeAmt"]);
                    FpSpread1.Sheets[0].Cells[1, 12].HorizontalAlign = HorizontalAlign.Right;

                    double qty = Convert.ToDouble(ds.Tables[0].Rows[row]["Qty"]);
                    double rpu = Convert.ToDouble(ds.Tables[0].Rows[row]["RPU"]);

                    double tax = Convert.ToDouble(ds.Tables[0].Rows[row]["TaxPercent"]);
                    double extax = Convert.ToDouble(ds.Tables[0].Rows[row]["ExeciseTaxPer"]);
                    double otherharge = Convert.ToDouble(ds.Tables[0].Rows[row]["OtherChargeAmt"]);
                    double cost = 0;
                    double dis = 0;
                    cost = qty * rpu;
                    if (disamtcal != 0)
                    {
                        cost = cost - disamtcal;
                    }
                    if (dispercal != 0)
                    {
                        dis = (cost / 100) * dispercal;
                        cost = cost - dis;
                    }
                    if (tax != 0)
                    {
                        double t = (cost / 100) * tax;
                        cost = cost + t;
                    }
                    if (extax != 0)
                    {
                        double ex = cost / 100 * extax;
                        cost = cost + ex;
                    }
                    cost = cost + otherharge;
                    FpSpread1.Sheets[0].Cells[1, 8].Text = Convert.ToString(cost);
                    FpSpread1.Sheets[0].Cells[1, 8].HorizontalAlign = HorizontalAlign.Right;

                    FpSpread1.Sheets[0].Cells[1, 15].Text = Convert.ToString(ds.Tables[0].Rows[row]["OtherChargeDesc"]);
                    FpSpread1.Sheets[0].Cells[1, 15].HorizontalAlign = HorizontalAlign.Right;
                    FpSpread1.Sheets[0].Cells[1, 16].Text = Convert.ToString(System.DateTime.Now.ToString("MM/dd/yyyy"));
                    FpSpread1.Sheets[0].Cells[1, 16].HorizontalAlign = HorizontalAlign.Center;
                    popwindow2.Visible = true;
                }
            }
        }
        catch
        {
        }
    }

    protected void btn_deletevendorequest(object sender, EventArgs e)
    {
        try
        {
            if (btn_delete.Text == "Delete")
            {
                surediv.Visible = true;
                lbl_sure.Text = "Do you want to Delete this Record?";
            }
        }
        catch
        {

        }
    }
    protected void btn_sureyes_Click(object sender, EventArgs e)
    {
        delete();
        // btn_go_Click(sender, e);
    }
    protected void btn_sureno_Click(object sender, EventArgs e)
    {
        surediv.Visible = false;
        alertmessage.Visible = false;
        //popwindow2.Visible = true;
    }

    protected void delete()
    {
        try
        {
            if (FpSpread1.Sheets[0].RowCount > 0)
            {
                FpSpread1.SaveChanges();
                for (int row = 1; row < FpSpread1.Sheets[0].RowCount; row++)
                {
                    int checkval = Convert.ToInt32(FpSpread1.Sheets[0].Cells[row, 1].Value);
                    if (checkval == 1)
                    {
                        surediv.Visible = false;
                        string VendorQuotfk = Convert.ToString(FpSpread1.Sheets[0].Cells[1, 4].Tag);
                        string delquery = "delete IT_VendorQuot where VendorQuotPK ='" + VendorQuotfk + "'";
                        delquery = delquery + " delete IT_VednorQuotDet where VendorQuotFK ='" + VendorQuotfk + "'";
                        int ins = d2.update_method_wo_parameter(delquery, "Text");
                        if (ins != 0)
                        {
                            //popwindow2.Visible = false;
                            lbl_alerterror.Visible = true;
                            lbl_alerterror.Text = "Deleted Successfully";
                            alertmessage.Visible = true;
                        }
                    }
                    else
                    {
                        lbl_alerterror.Visible = true;
                        lbl_alerterror.Text = "Please Select Any One Item";
                        alertmessage.Visible = true;
                        surediv.Visible = false;
                    }
                }
            }
        }
        catch
        {
        }
    }


    protected void btnExcel_Click(object sender, EventArgs e)
    {
        try
        {
            string reportname = txtexcelname.Text;
            if (reportname.ToString().Trim() != "")
            {
                d2.printexcelreport(FpSpread2, reportname);
                lblvalidation1.Visible = false;
            }
            else
            {
                lblvalidation1.Text = "Please Enter Your Report Name";
                lblvalidation1.Visible = true;
                txtexcelname.Focus();
            }
        }
        catch
        {

        }

    }
    protected void btnprintmaster_Click(object sender, EventArgs e)
    {
        try
        {
            string degreedetails = "Supplier Quotation Request";
            string pagename = "vendor_quatation_request.aspx";
            Printcontrol.loadspreaddetails(FpSpread2, pagename, degreedetails);
            Printcontrol.Visible = true;
        }
        catch
        {

        }

    }
    protected void btn_baseaddnew_Click(object sender, EventArgs e)
    {
        bindrequestcode();
        btnmain_save.Visible = false;
        btnmain_save.Text = "Save";
        btn_delete.Visible = false;
        txt_venquano.Enabled = true;
        txt_searchvendor.Enabled = true;
        popwindow2.Visible = true;
        txt_venquano.Text = "";
        txt_searchvendor.Text = "";
        btn_vendorqmark.Enabled = true;
        btn_go.Enabled = true;
        btn_go_Click(sender, e);
    }
    public void vendor()
    {
        int i = 0;
        cbl_vendor.Items.Clear();
        string vendor = "select distinct vm.VendorCompName,vq.vendorfk from IT_VendorQuot vq,CO_VendorMaster vm where vm.VendorPK=vq.vendorfk and vm.vendortype='1'";
        ds.Clear();
        ds = d2.select_method_wo_parameter(vendor, "Text");
        if (ds.Tables[0].Rows.Count > 0)
        {
            cbl_vendor.DataSource = ds;
            cbl_vendor.DataTextField = "VendorCompName";
            cbl_vendor.DataValueField = "vendorfk";
            cbl_vendor.DataBind();

            if (cbl_vendor.Items.Count > 0)
            {
                for (i = 0; i < cbl_vendor.Items.Count; i++)
                {

                    cbl_vendor.Items[i].Selected = true;
                }

                txt_vendorname.Text = "Supplier Name(" + cbl_vendor.Items.Count + ")";
            }
        }
        else
        {
            txt_vendorname.Text = "--Select--";
        }
        bindquocode();
    }
    public void bindquocode()
    {
        int i = 0;
        string quacode = "";
        for (i = 0; i < cbl_vendor.Items.Count; i++)
        {
            if (cbl_vendor.Items[i].Selected == true)
            {
                string build = cbl_vendor.Items[i].Value.ToString();
                if (quacode == "")
                {
                    quacode = build;
                }
                else
                {
                    quacode = quacode + "'" + "," + "'" + build;
                }
            }
        }
        cbl_quocode.Items.Clear();
        string vendor = "select vq.VenQuotCode,vq.VendorQuotPK from IT_VendorQuot vq,CO_VendorMaster vm where vm.VendorPK=vq.vendorfk and vm.VendorType='1' and VenQuotType='2' and vq.vendorfk in('" + quacode + "')";
        ds.Clear();
        ds = d2.select_method_wo_parameter(vendor, "Text");
        if (ds.Tables[0].Rows.Count > 0)
        {
            cbl_quocode.DataSource = ds;
            cbl_quocode.DataTextField = "VenQuotCode";
            cbl_quocode.DataValueField = "VendorQuotPK";
            cbl_quocode.DataBind();
            if (cbl_quocode.Items.Count > 0)
            {
                for (i = 0; i < cbl_quocode.Items.Count; i++)
                {
                    cbl_quocode.Items[i].Selected = true;
                }
                txt_basereqcode.Text = "Quotation Code(" + cbl_quocode.Items.Count + ")";
            }
        }
        else
        {
            txt_basereqcode.Text = "--Select--";
        }
    }
    [System.Web.Services.WebMethod]
    [System.Web.Script.Services.ScriptMethod()]
    public static List<string> venreqcode(string prefixText)
    {
        DAccess2 dn = new DAccess2();
        DataSet dw = new DataSet();
        List<string> name = new List<string>();
        string query = " select distinct vq.VenReqCode from IT_VendorReq vq,IT_VendorReqDet vrq,RQ_Requisition rq where vq.VenReqPK =vrq.VenReqPK and vq.ReqFK=vrq.ReqFK and rq.RequestType='1' and isnull(VendorReq_Type,'0')=0 and rq.RequisitionPK=vq.ReqFK and vq.VenReqPK not in(select VenReqFK from IT_VendorQuot) and vq.VenReqCode like '" + prefixText + "%' order by VenReqCode ";
        dw = dn.select_method_wo_parameter(query, "Text");
        if (dw.Tables[0].Rows.Count > 0)
        {
            for (int i = 0; i < dw.Tables[0].Rows.Count; i++)
            {
                name.Add(dw.Tables[0].Rows[i]["VenReqCode"].ToString());
            }
        }
        return name;
    }
}