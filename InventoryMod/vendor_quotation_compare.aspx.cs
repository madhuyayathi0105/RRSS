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
using System.Text;


public partial class vendor_quotation_compare : System.Web.UI.Page
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
    static ArrayList ItemList = new ArrayList();
    static ArrayList Itemindex = new ArrayList();
    Hashtable ht = new Hashtable();
    static bool rowCheck = false;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["collegecode"] == null)
        {
            Response.Redirect("~/Default.aspx");
        }
        usercode = Session["usercode"].ToString();
        collegecode1 = Session["collegecode"].ToString();
        singleuser = Session["single_user"].ToString();
        group_user = Session["group_code"].ToString();
        lblvalidation1.Text = "";
        if (!IsPostBack)
        {
            ddl_reqcompcode_bind();
            ItemList.Clear();
            rowCheck = false;
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
    protected void cb_venname_CheckedChanged(object sender, EventArgs e)
    {
        int cout = 0;
        txt_venname.Text = "--Select--";

        if (cb_venname.Checked == true)
        {
            cout++;
            for (int i = 0; i < cbl_venname.Items.Count; i++)
            {
                cbl_venname.Items[i].Selected = true;
            }
            txt_venname.Text = "Supplier Name(" + (cbl_venname.Items.Count) + ")";
        }
        else
        {
            for (int i = 0; i < cbl_venname.Items.Count; i++)
            {
                cbl_venname.Items[i].Selected = false;
            }
        }
    }
    protected void cbl_venname_SelectedIndexChange(object sender, EventArgs e)
    {
        int i = 0;
        cb_venname.Checked = false;
        int commcount = 0;
        txt_venname.Text = "--Select--";
        for (i = 0; i < cbl_venname.Items.Count; i++)
        {
            if (cbl_venname.Items[i].Selected == true)
            {
                commcount = commcount + 1;
                cb_venname.Checked = false;
            }
        }
        if (commcount > 0)
        {
            if (commcount == cbl_venname.Items.Count)
            {
                cb_venname.Checked = true;
            }
            txt_venname.Text = "Supplier Name(" + commcount.ToString() + ")";
        }
    }
    protected void btn_go_Click(object sender, EventArgs e)
    {
        try
        {
            int c = FpSpread1.Sheets[0].RowCount;
            Printcontrol.Visible = false;
            int j = 0;
            string venquocode = "";
            for (j = 0; j < cbl_venname.Items.Count; j++)
            {
                if (cbl_venname.Items[j].Selected == true)
                {
                    string build = cbl_venname.Items[j].Value.ToString();
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

            if (txt_venname.Text.Trim() != "--Select--" && ddl_reqcompcode.SelectedItem.Text.Trim() != "Select")
            {
                string q2 = "select distinct rq.reqcompcode,vm.VendorPK,vm.VendorCompName,vm.VendorCode, vm.VendorMobileNo,rq.VenReqCode,vq.VenQuotNo,vq.VenQuotCode from IT_VendorReq rq,IT_VendorQuot vq,IT_VednorQuotDet vd,CO_VendorMaster vm where vq.VendorQuotPK=vd.VendorQuotFK and rq.VendorFK=vq.VendorFK and vm.VendorPK=vq.VendorFK and vq.VendorFK in('" + venquocode + "') and rq.ReqCompCode in ('" + ddl_reqcompcode.SelectedItem.Text + "')";
                ds.Clear();
                ds = d2.select_method_wo_parameter(q2, "Text");

                if (ds.Tables[0].Rows.Count > 0)
                {
                    int startspanpoint = 0;
                    int rowcount1 = 1;
                    FpSpread1.Sheets[0].RowCount = 0;
                    FpSpread1.Sheets[0].ColumnCount = 0;
                    FpSpread1.CommandBar.Visible = false;
                    FpSpread1.Sheets[0].AutoPostBack = false;
                    FpSpread1.Sheets[0].ColumnHeader.RowCount = 1;
                    FpSpread1.Sheets[0].RowHeader.Visible = false;
                    FpSpread1.Sheets[0].ColumnCount = 8;

                    FarPoint.Web.Spread.StyleInfo darkstyle = new FarPoint.Web.Spread.StyleInfo();
                    darkstyle.BackColor = ColorTranslator.FromHtml("#0CA6CA");
                    darkstyle.ForeColor = Color.White;
                    FpSpread1.ActiveSheetView.ColumnHeader.DefaultStyle = darkstyle;

                    FpSpread1.Sheets[0].ColumnHeader.Cells[0, 0].Text = "S.No";
                    FpSpread1.Sheets[0].ColumnHeader.Cells[0, 0].Font.Bold = true;
                    FpSpread1.Sheets[0].ColumnHeader.Cells[0, 0].HorizontalAlign = HorizontalAlign.Center;
                    FpSpread1.Sheets[0].ColumnHeader.Cells[0, 0].Font.Name = "Book Antiqua";
                    FpSpread1.Sheets[0].ColumnHeader.Cells[0, 0].Font.Size = FontUnit.Medium;
                    FpSpread1.Columns[0].Width = 50;
                    FpSpread1.Columns[0].Locked = true;

                    FpSpread1.Sheets[0].ColumnHeader.Cells[0, 1].Text = "Select";
                    FpSpread1.Sheets[0].ColumnHeader.Cells[0, 1].Font.Bold = true;
                    FpSpread1.Sheets[0].ColumnHeader.Cells[0, 1].HorizontalAlign = HorizontalAlign.Center;
                    FpSpread1.Sheets[0].ColumnHeader.Cells[0, 1].Font.Name = "Book Antiqua";
                    FpSpread1.Sheets[0].ColumnHeader.Cells[0, 1].Font.Size = FontUnit.Medium;
                    FpSpread1.Columns[1].Width = 50;

                    FpSpread1.Sheets[0].ColumnHeader.Cells[0, 2].Text = "Supplier Name";
                    FpSpread1.Sheets[0].ColumnHeader.Cells[0, 2].Font.Bold = true;
                    FpSpread1.Sheets[0].ColumnHeader.Cells[0, 2].HorizontalAlign = HorizontalAlign.Center;
                    FpSpread1.Sheets[0].ColumnHeader.Cells[0, 2].Font.Name = "Book Antiqua";
                    FpSpread1.Sheets[0].ColumnHeader.Cells[0, 2].Font.Size = FontUnit.Medium;
                    FpSpread1.Columns[2].Width = 200;

                    FpSpread1.Sheets[0].ColumnHeader.Cells[0, 3].Text = "Supplier Code";
                    FpSpread1.Sheets[0].ColumnHeader.Cells[0, 3].Font.Bold = true;
                    FpSpread1.Sheets[0].ColumnHeader.Cells[0, 3].HorizontalAlign = HorizontalAlign.Center;
                    FpSpread1.Sheets[0].ColumnHeader.Cells[0, 3].Font.Name = "Book Antiqua";
                    FpSpread1.Sheets[0].ColumnHeader.Cells[0, 3].Font.Size = FontUnit.Medium;
                    FpSpread1.Columns[3].Width = 100;
                    FpSpread1.Columns[3].Locked = true;
                    FpSpread1.Sheets[0].ColumnHeader.Cells[0, 4].Text = "Supplier Mobile No";
                    FpSpread1.Sheets[0].ColumnHeader.Cells[0, 4].Font.Bold = true;
                    FpSpread1.Sheets[0].ColumnHeader.Cells[0, 4].HorizontalAlign = HorizontalAlign.Center;
                    FpSpread1.Sheets[0].ColumnHeader.Cells[0, 4].Font.Name = "Book Antiqua";
                    FpSpread1.Sheets[0].ColumnHeader.Cells[0, 4].Font.Size = FontUnit.Medium;
                    FpSpread1.Columns[4].Width = 150;
                    FpSpread1.Columns[4].Locked = true;
                    FpSpread1.Sheets[0].ColumnHeader.Cells[0, 5].Text = "Supplier Request Code";
                    FpSpread1.Sheets[0].ColumnHeader.Cells[0, 5].Font.Bold = true;
                    FpSpread1.Sheets[0].ColumnHeader.Cells[0, 5].HorizontalAlign = HorizontalAlign.Center;
                    FpSpread1.Sheets[0].ColumnHeader.Cells[0, 5].Font.Name = "Book Antiqua";
                    FpSpread1.Sheets[0].ColumnHeader.Cells[0, 5].Font.Size = FontUnit.Medium;
                    FpSpread1.Columns[5].Width = 100;
                    FpSpread1.Columns[5].Locked = true;
                    FpSpread1.Sheets[0].ColumnHeader.Cells[0, 6].Text = "Supplier Quotation Code";
                    FpSpread1.Sheets[0].ColumnHeader.Cells[0, 6].Font.Bold = true;
                    FpSpread1.Sheets[0].ColumnHeader.Cells[0, 6].HorizontalAlign = HorizontalAlign.Center;
                    FpSpread1.Sheets[0].ColumnHeader.Cells[0, 6].Font.Name = "Book Antiqua";
                    FpSpread1.Sheets[0].ColumnHeader.Cells[0, 6].Font.Size = FontUnit.Medium;
                    FpSpread1.Columns[6].Width = 100;
                    FpSpread1.Columns[6].Locked = true;
                    FpSpread1.Sheets[0].ColumnHeader.Cells[0, 7].Text = "Supplier Quotation No";
                    FpSpread1.Sheets[0].ColumnHeader.Cells[0, 7].Font.Bold = true;
                    FpSpread1.Sheets[0].ColumnHeader.Cells[0, 7].HorizontalAlign = HorizontalAlign.Center;
                    FpSpread1.Sheets[0].ColumnHeader.Cells[0, 7].Font.Name = "Book Antiqua";
                    FpSpread1.Sheets[0].ColumnHeader.Cells[0, 7].Font.Size = FontUnit.Medium;
                    FpSpread1.Columns[7].Width = 100;
                    FpSpread1.Columns[7].Locked = true;
                    FarPoint.Web.Spread.CheckBoxCellType chkall = new FarPoint.Web.Spread.CheckBoxCellType();
                    chkall.AutoPostBack = false;
                    string vennam = "";
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        FpSpread1.Sheets[0].RowCount++;
                        FpSpread1.Sheets[0].Cells[FpSpread1.Sheets[0].RowCount - 1, 0].Text = Convert.ToString(i + 1);
                        FpSpread1.Sheets[0].Cells[FpSpread1.Sheets[0].RowCount - 1, 0].HorizontalAlign = HorizontalAlign.Center;
                        FpSpread1.Sheets[0].Cells[FpSpread1.Sheets[0].RowCount - 1, 0].Font.Size = FontUnit.Medium;
                        FpSpread1.Sheets[0].Cells[FpSpread1.Sheets[0].RowCount - 1, 0].Font.Name = "Book Antiqua";

                        if (vennam != Convert.ToString(ds.Tables[0].Rows[i]["VendorCompName"]))
                        {
                            FpSpread1.Sheets[0].Cells[FpSpread1.Sheets[0].RowCount - 1, 1].CellType = chkall;
                            FpSpread1.Sheets[0].Cells[FpSpread1.Sheets[0].RowCount - 1, 1].HorizontalAlign = HorizontalAlign.Center;

                            FpSpread1.Sheets[0].SpanModel.Add(startspanpoint, 1, rowcount1, 1);
                            startspanpoint = FpSpread1.Sheets[0].RowCount - 1;
                        }
                        else
                        {
                            rowcount1++;
                        }
                        vennam = Convert.ToString(ds.Tables[0].Rows[i]["VendorCompName"]);

                        FpSpread1.Sheets[0].Cells[FpSpread1.Sheets[0].RowCount - 1, 2].Text = Convert.ToString(ds.Tables[0].Rows[i]["VendorCompName"]);
                        FpSpread1.Sheets[0].Cells[FpSpread1.Sheets[0].RowCount - 1, 2].Tag = Convert.ToString(ds.Tables[0].Rows[i]["VendorPK"]);
                        FpSpread1.Sheets[0].Cells[FpSpread1.Sheets[0].RowCount - 1, 2].HorizontalAlign = HorizontalAlign.Left;
                        FpSpread1.Sheets[0].Cells[FpSpread1.Sheets[0].RowCount - 1, 2].Font.Size = FontUnit.Medium;
                        FpSpread1.Sheets[0].Cells[FpSpread1.Sheets[0].RowCount - 1, 2].Font.Name = "Book Antiqua";


                        FpSpread1.Sheets[0].Cells[FpSpread1.Sheets[0].RowCount - 1, 3].Text = Convert.ToString(ds.Tables[0].Rows[i]["VendorCode"]);
                        FpSpread1.Sheets[0].Cells[FpSpread1.Sheets[0].RowCount - 1, 3].Tag = Convert.ToString(ds.Tables[0].Rows[i]["reqcompcode"]);

                        FpSpread1.Sheets[0].Cells[FpSpread1.Sheets[0].RowCount - 1, 3].HorizontalAlign = HorizontalAlign.Left;
                        FpSpread1.Sheets[0].Cells[FpSpread1.Sheets[0].RowCount - 1, 3].Font.Size = FontUnit.Medium;
                        FpSpread1.Sheets[0].Cells[FpSpread1.Sheets[0].RowCount - 1, 3].Font.Name = "Book Antiqua";

                        FpSpread1.Sheets[0].Cells[FpSpread1.Sheets[0].RowCount - 1, 4].Text = Convert.ToString(ds.Tables[0].Rows[i]["VendorMobileNo"]);
                        FpSpread1.Sheets[0].Cells[FpSpread1.Sheets[0].RowCount - 1, 4].HorizontalAlign = HorizontalAlign.Left;
                        FpSpread1.Sheets[0].Cells[FpSpread1.Sheets[0].RowCount - 1, 4].Font.Size = FontUnit.Medium;
                        FpSpread1.Sheets[0].Cells[FpSpread1.Sheets[0].RowCount - 1, 4].Font.Name = "Book Antiqua";

                        FpSpread1.Sheets[0].Cells[FpSpread1.Sheets[0].RowCount - 1, 5].Text = Convert.ToString(ds.Tables[0].Rows[i]["VenReqCode"]);
                        FpSpread1.Sheets[0].Cells[FpSpread1.Sheets[0].RowCount - 1, 5].HorizontalAlign = HorizontalAlign.Left;
                        FpSpread1.Sheets[0].Cells[FpSpread1.Sheets[0].RowCount - 1, 5].Font.Size = FontUnit.Medium;
                        FpSpread1.Sheets[0].Cells[FpSpread1.Sheets[0].RowCount - 1, 5].Font.Name = "Book Antiqua";

                        FpSpread1.Sheets[0].Cells[FpSpread1.Sheets[0].RowCount - 1, 6].Text = Convert.ToString(ds.Tables[0].Rows[i]["VenQuotCode"]);
                        FpSpread1.Sheets[0].Cells[FpSpread1.Sheets[0].RowCount - 1, 6].HorizontalAlign = HorizontalAlign.Left;
                        FpSpread1.Sheets[0].Cells[FpSpread1.Sheets[0].RowCount - 1, 6].Font.Size = FontUnit.Medium;
                        FpSpread1.Sheets[0].Cells[FpSpread1.Sheets[0].RowCount - 1, 6].Font.Name = "Book Antiqua";

                        FpSpread1.Sheets[0].Cells[FpSpread1.Sheets[0].RowCount - 1, 7].Text = Convert.ToString(ds.Tables[0].Rows[i]["VenQuotNo"]);
                        FpSpread1.Sheets[0].Cells[FpSpread1.Sheets[0].RowCount - 1, 7].HorizontalAlign = HorizontalAlign.Left;
                        FpSpread1.Sheets[0].Cells[FpSpread1.Sheets[0].RowCount - 1, 7].Font.Size = FontUnit.Medium;
                        FpSpread1.Sheets[0].Cells[FpSpread1.Sheets[0].RowCount - 1, 7].Font.Name = "Book Antiqua";

                    }

                    FpSpread1.Sheets[0].PageSize = FpSpread1.Sheets[0].RowCount;
                    FpSpread1.Sheets[0].SetColumnMerge(2, FarPoint.Web.Spread.Model.MergePolicy.Always);
                    // FpSpread1.Sheets[0].SetColumnMerge(1, FarPoint.Web.Spread.Model.MergePolicy.Always);
                    FpSpread1.Sheets[0].FrozenRowCount = 0;
                    FpSpread1.Visible = true;
                    spreaddiv.Visible = true;
                    btn_selectitem.Visible = true;
                    btn_purchasereq.Visible = true;

                    pheaderfilter.Visible = true;
                    pcolumnorder.Visible = true;
                }
                else
                {
                    FpSpread1.Visible = false;
                    spreaddiv.Visible = false;
                    lbl_baseerror.Visible = true;
                    btn_purchasereq.Visible = false;
                    btn_selectitem.Visible = false;
                    btn_purchasereq.Visible = false;
                    rptprint.Visible = false;
                    pheaderfilter.Visible = false;
                    pcolumnorder.Visible = false;
                    //tborder.Visible = false;
                    lbl_baseerror.Text = "No Record Founds";
                }
            }
            else
            {
                FpSpread1.Visible = false;
                spreaddiv.Visible = false;
                rptprint.Visible = false;
                btn_selectitem.Visible = false;
                lbl_baseerror.Visible = true;
                pheaderfilter.Visible = false;
                pcolumnorder.Visible = false;
                spreaddiv1.Visible = false;
                lbl_baseerror.Text = "Please Select All fields";
            }
        }
        catch
        {

        }

    }
    protected void btn_go1_Click(object sender, EventArgs e)
    {
        try
        {
            bool chk = false;
            bool check = false;
            string uncheck = "";
            string vendorpk = "";
            string reqcomparecode="";
            StringBuilder sbvendorpk = new StringBuilder();
            StringBuilder sbreqcomparecode = new StringBuilder();
        
            string venpk = "";
            string comcode = "";
            string vendorpk1 = string.Empty;
            string reqcomparecode1 = string.Empty;
            sbvendorpk.Append(venpk).Append("','");
            FpSpread5.SaveChanges();
            int rowcount = 0;
            FpSpread5.Visible = true;
            int c = FpSpread1.Sheets[0].RowCount;

            if (rowCheck)
            {
                rowcount = FpSpread5.Sheets[0].RowCount;
                for (int i = 0; i < rowcount; i++)
                {
                    vendorpk = Convert.ToString(FpSpread5.Sheets[0].Cells[i, 2].Tag);
                    reqcomparecode = Convert.ToString(FpSpread5.Sheets[0].Cells[i, 17].Tag);


                    if (vendorpk != "" && reqcomparecode != "")
                    {
                        sbvendorpk.Append(vendorpk).Append("','");
                        sbreqcomparecode.Append(reqcomparecode).Append("','");
                    }
                }
            }
            vendorpk1 = Convert.ToString(sbvendorpk);
            vendorpk1 = vendorpk1.TrimEnd(',');
            reqcomparecode1 = Convert.ToString(sbreqcomparecode);
            reqcomparecode1 = reqcomparecode1.TrimEnd(',');
            if (FpSpread1.Sheets[0].RowCount > 0)
            {
                FpSpread1.SaveChanges();
                for (int row = 0; row < FpSpread1.Sheets[0].RowCount; row++)
                {
                    int checkval = Convert.ToInt32(FpSpread1.Sheets[0].Cells[row, 1].Value);
                    if (checkval == 1)
                    {
                         vendorpk = Convert.ToString(FpSpread1.Sheets[0].Cells[row, 2].Tag);
                         reqcomparecode = Convert.ToString(FpSpread1.Sheets[0].Cells[row, 3].Tag);
                         vendorpk = vendorpk +"','"+ vendorpk1;
                         reqcomparecode = reqcomparecode + "','" + reqcomparecode1;
                        string q1 = "select distinct vq.VendorfK,i.itemname,i.ItemCode,vd.Qty,vd.Qty,vd.RPU, vd.DiscountAmt,vd.TaxPercent,vd.ItemFK, vd.EduCessPer,HigherEduCessPer,vd.ExeciseTaxPer, vd.OtherChargeAmt,vm.VendorCompName,vm.VendorCode,rq.ReqCompCode from IT_VendorReq rq,IT_VendorQuot vq,IT_VednorQuotDet vd,IM_ItemMaster i,CO_VendorMaster vm where vq.VendorQuotPK=vd.VendorQuotFK and rq.VendorFK=vq.VendorFK and i.ItemPK=vd.ItemFK and vm.VendorPK=vq.VendorFK and vq.VendorFK in('" + vendorpk + "') and rq.ReqCompCode in ('" + reqcomparecode + "')";
                        ds.Clear();
                        ds = d2.select_method_wo_parameter(q1, "Text");
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            rowCheck = true;
                            int startspanpoint = 0;
                            int rowcount1 = 1;
                            FpSpread5.Sheets[0].RowCount = 0;
                            FpSpread5.Sheets[0].ColumnCount = 0;
                            FpSpread5.CommandBar.Visible = false;
                            FpSpread5.Sheets[0].AutoPostBack = false;
                            FpSpread5.Sheets[0].ColumnHeader.RowCount = 1;
                            FpSpread5.Sheets[0].RowHeader.Visible = false;
                            FpSpread5.Sheets[0].ColumnCount = 18;

                            FarPoint.Web.Spread.StyleInfo darkstyle = new FarPoint.Web.Spread.StyleInfo();
                            darkstyle.BackColor = ColorTranslator.FromHtml("#0CA6CA");
                            darkstyle.ForeColor = Color.White;
                            FpSpread5.ActiveSheetView.ColumnHeader.DefaultStyle = darkstyle;

                            FpSpread5.Sheets[0].ColumnHeader.Cells[0, 0].Text = "S.No";
                            FpSpread5.Sheets[0].ColumnHeader.Cells[0, 0].Font.Bold = true;
                            FpSpread5.Sheets[0].ColumnHeader.Cells[0, 0].HorizontalAlign = HorizontalAlign.Center;
                            FpSpread5.Sheets[0].ColumnHeader.Cells[0, 0].Font.Name = "Book Antiqua";
                            FpSpread5.Sheets[0].ColumnHeader.Cells[0, 0].Font.Size = FontUnit.Medium;
                            FpSpread5.Columns[0].Width = 50;
                            FpSpread5.Columns[0].Locked = true;

                            FpSpread5.Sheets[0].ColumnHeader.Cells[0, 1].Text = "Select";
                            FpSpread5.Sheets[0].ColumnHeader.Cells[0, 1].Font.Bold = true;
                            FpSpread5.Sheets[0].ColumnHeader.Cells[0, 1].HorizontalAlign = HorizontalAlign.Center;
                            FpSpread5.Sheets[0].ColumnHeader.Cells[0, 1].Font.Name = "Book Antiqua";
                            FpSpread5.Sheets[0].ColumnHeader.Cells[0, 1].Font.Size = FontUnit.Medium;
                            FpSpread5.Columns[1].Width = 50;

                            FpSpread5.Sheets[0].ColumnHeader.Cells[0, 2].Text = "Supplier Name";
                            FpSpread5.Sheets[0].ColumnHeader.Cells[0, 2].Font.Bold = true;
                            FpSpread5.Sheets[0].ColumnHeader.Cells[0, 2].HorizontalAlign = HorizontalAlign.Center;
                            FpSpread5.Sheets[0].ColumnHeader.Cells[0, 2].Font.Name = "Book Antiqua";
                            FpSpread5.Sheets[0].ColumnHeader.Cells[0, 2].Font.Size = FontUnit.Medium;
                            FpSpread5.Columns[2].Width = 200;
                            FpSpread5.Columns[2].Locked = true;
                            FpSpread5.Columns[2].Visible = false;
                            FpSpread5.Sheets[0].ColumnHeader.Cells[0, 3].Text = "Item Name";
                            FpSpread5.Sheets[0].ColumnHeader.Cells[0, 3].Font.Bold = true;
                            FpSpread5.Sheets[0].ColumnHeader.Cells[0, 3].HorizontalAlign = HorizontalAlign.Center;
                            FpSpread5.Sheets[0].ColumnHeader.Cells[0, 3].Font.Name = "Book Antiqua";
                            FpSpread5.Sheets[0].ColumnHeader.Cells[0, 3].Font.Size = FontUnit.Medium;
                            FpSpread5.Columns[3].Width = 200;
                            FpSpread5.Columns[3].Locked = true;
                            FpSpread5.Columns[3].Visible = false;


                            FpSpread5.Sheets[0].ColumnHeader.Cells[0, 4].Text = "Item Code";
                            FpSpread5.Sheets[0].ColumnHeader.Cells[0, 4].Font.Bold = true;
                            FpSpread5.Sheets[0].ColumnHeader.Cells[0, 4].HorizontalAlign = HorizontalAlign.Center;
                            FpSpread5.Sheets[0].ColumnHeader.Cells[0, 4].Font.Name = "Book Antiqua";
                            FpSpread5.Sheets[0].ColumnHeader.Cells[0, 4].Font.Size = FontUnit.Medium;
                            FpSpread5.Columns[4].Width = 100;
                            FpSpread5.Columns[4].Locked = true;
                            FpSpread5.Columns[4].Visible = false;

                            FpSpread5.Sheets[0].ColumnHeader.Cells[0, 5].Text = "Quantity";
                            FpSpread5.Sheets[0].ColumnHeader.Cells[0, 5].Font.Bold = true;
                            FpSpread5.Sheets[0].ColumnHeader.Cells[0, 5].HorizontalAlign = HorizontalAlign.Center;
                            FpSpread5.Sheets[0].ColumnHeader.Cells[0, 5].Font.Name = "Book Antiqua";
                            FpSpread5.Sheets[0].ColumnHeader.Cells[0, 5].Font.Size = FontUnit.Medium;
                            FpSpread5.Columns[5].Width = 100;
                            FpSpread5.Columns[5].Locked = true;
                            FpSpread5.Columns[5].Visible = false;

                            FpSpread5.Sheets[0].ColumnHeader.Cells[0, 6].Text = "Rpu";
                            FpSpread5.Sheets[0].ColumnHeader.Cells[0, 6].Font.Bold = true;
                            FpSpread5.Sheets[0].ColumnHeader.Cells[0, 6].HorizontalAlign = HorizontalAlign.Center;
                            FpSpread5.Sheets[0].ColumnHeader.Cells[0, 6].Font.Name = "Book Antiqua";
                            FpSpread5.Sheets[0].ColumnHeader.Cells[0, 6].Font.Size = FontUnit.Medium;
                            FpSpread5.Columns[6].Width = 100;
                            FpSpread5.Columns[6].Locked = true;
                            FpSpread5.Columns[6].Visible = false;

                            FpSpread5.Sheets[0].ColumnHeader.Cells[0, 7].Text = "Discount";
                            FpSpread5.Sheets[0].ColumnHeader.Cells[0, 7].Font.Bold = true;
                            FpSpread5.Sheets[0].ColumnHeader.Cells[0, 7].HorizontalAlign = HorizontalAlign.Center;
                            FpSpread5.Sheets[0].ColumnHeader.Cells[0, 7].Font.Name = "Book Antiqua";
                            FpSpread5.Sheets[0].ColumnHeader.Cells[0, 7].Font.Size = FontUnit.Medium;
                            FpSpread5.Columns[7].Width = 100;
                            FpSpread5.Columns[7].Locked = true;
                            FpSpread5.Columns[7].Visible = false;

                            //FpSpread5.Sheets[0].ColumnHeader.Cells[0, 8].Text = "Discount";
                            //FpSpread5.Sheets[0].ColumnHeader.Cells[0, 8].Font.Bold = true;
                            //FpSpread5.Sheets[0].ColumnHeader.Cells[0, 8].HorizontalAlign = HorizontalAlign.Center;
                            //FpSpread5.Sheets[0].ColumnHeader.Cells[0, 8].Font.Name = "Book Antiqua";
                            //FpSpread5.Sheets[0].ColumnHeader.Cells[0, 8].Font.Size = FontUnit.Medium;
                            //FpSpread5.Columns[8].Width = 100;
                            //FpSpread5.Columns[8].Locked = true;
                            FpSpread5.Columns[8].Visible = false;
                            FpSpread5.Sheets[0].ColumnHeader.Cells[0, 9].Text = "Tax";
                            FpSpread5.Sheets[0].ColumnHeader.Cells[0, 9].Font.Bold = true;
                            FpSpread5.Sheets[0].ColumnHeader.Cells[0, 9].Font.Name = "Book Antiqua";
                            FpSpread5.Sheets[0].ColumnHeader.Cells[0, 9].Font.Size = FontUnit.Medium;
                            FpSpread5.Sheets[0].ColumnHeader.Cells[0, 9].HorizontalAlign = HorizontalAlign.Center;
                            FpSpread5.Columns[9].Width = 100;
                            FpSpread5.Columns[9].Locked = true;
                            FpSpread5.Columns[9].Visible = false;

                            FpSpread5.Sheets[0].ColumnHeader.Cells[0, 10].Text = "Exercies tax";
                            FpSpread5.Sheets[0].ColumnHeader.Cells[0, 10].Font.Bold = true;
                            FpSpread5.Sheets[0].ColumnHeader.Cells[0, 10].Font.Name = "Book Antiqua";
                            FpSpread5.Sheets[0].ColumnHeader.Cells[0, 10].Font.Size = FontUnit.Medium;
                            FpSpread5.Sheets[0].ColumnHeader.Cells[0, 10].HorizontalAlign = HorizontalAlign.Center;
                            FpSpread5.Columns[10].Width = 100;
                            FpSpread5.Columns[10].Locked = true;
                            FpSpread5.Columns[10].Visible = false;


                            FpSpread5.Sheets[0].ColumnHeader.Cells[0, 11].Text = "Education Cess";
                            FpSpread5.Sheets[0].ColumnHeader.Cells[0, 11].Font.Bold = true;
                            FpSpread5.Sheets[0].ColumnHeader.Cells[0, 11].Font.Name = "Book Antiqua";
                            FpSpread5.Sheets[0].ColumnHeader.Cells[0, 11].Font.Size = FontUnit.Medium;
                            FpSpread5.Sheets[0].ColumnHeader.Cells[0, 11].HorizontalAlign = HorizontalAlign.Center;
                            FpSpread5.Columns[11].Width = 100;
                            FpSpread5.Columns[11].Locked = true;
                            FpSpread5.Columns[11].Visible = false;

                            FpSpread5.Sheets[0].ColumnHeader.Cells[0, 12].Text = "Higher Edu.Cess";
                            FpSpread5.Sheets[0].ColumnHeader.Cells[0, 12].Font.Bold = true;
                            FpSpread5.Sheets[0].ColumnHeader.Cells[0, 12].Font.Name = "Book Antiqua";
                            FpSpread5.Sheets[0].ColumnHeader.Cells[0, 12].Font.Size = FontUnit.Medium;
                            FpSpread5.Sheets[0].ColumnHeader.Cells[0, 12].HorizontalAlign = HorizontalAlign.Center;
                            FpSpread5.Columns[12].Width = 150;
                            FpSpread5.Columns[12].Locked = true;
                            FpSpread5.Columns[12].Visible = false;

                            FpSpread5.Sheets[0].ColumnHeader.Cells[0, 13].Text = "Other Charges";
                            FpSpread5.Sheets[0].ColumnHeader.Cells[0, 13].Font.Bold = true;
                            FpSpread5.Sheets[0].ColumnHeader.Cells[0, 13].Font.Name = "Book Antiqua";
                            FpSpread5.Sheets[0].ColumnHeader.Cells[0, 13].Font.Size = FontUnit.Medium;
                            FpSpread5.Sheets[0].ColumnHeader.Cells[0, 13].HorizontalAlign = HorizontalAlign.Center;
                            FpSpread5.Columns[13].Width = 150;
                            FpSpread5.Columns[13].Locked = true;
                            FpSpread5.Columns[13].Visible = false;

                            FpSpread5.Sheets[0].ColumnHeader.Cells[0, 14].Text = "CallEduCess";
                            FpSpread5.Sheets[0].ColumnHeader.Cells[0, 14].Font.Bold = true;
                            FpSpread5.Sheets[0].ColumnHeader.Cells[0, 14].Font.Name = "Book Antiqua";
                            FpSpread5.Sheets[0].ColumnHeader.Cells[0, 14].Font.Size = FontUnit.Medium;
                            FpSpread5.Sheets[0].ColumnHeader.Cells[0, 14].HorizontalAlign = HorizontalAlign.Center;
                            FpSpread5.Columns[14].Width = 150;
                            FpSpread5.Columns[14].Locked = true;
                            FpSpread5.Columns[14].Visible = false;

                            FpSpread5.Sheets[0].ColumnHeader.Cells[0, 15].Text = "CallExTax";
                            FpSpread5.Sheets[0].ColumnHeader.Cells[0, 15].Font.Bold = true;
                            FpSpread5.Sheets[0].ColumnHeader.Cells[0, 15].Font.Name = "Book Antiqua";
                            FpSpread5.Sheets[0].ColumnHeader.Cells[0, 15].Font.Size = FontUnit.Medium;
                            FpSpread5.Sheets[0].ColumnHeader.Cells[0, 15].HorizontalAlign = HorizontalAlign.Left;
                            FpSpread5.Columns[15].Width = 150;
                            FpSpread5.Columns[15].Locked = true;
                            FpSpread5.Columns[15].Visible = false;

                            FpSpread5.Sheets[0].ColumnHeader.Cells[0, 16].Text = "Cost";
                            FpSpread5.Sheets[0].ColumnHeader.Cells[0, 16].Font.Bold = true;
                            FpSpread5.Sheets[0].ColumnHeader.Cells[0, 16].HorizontalAlign = HorizontalAlign.Center;
                            FpSpread5.Sheets[0].ColumnHeader.Cells[0, 16].Font.Name = "Book Antiqua";
                            FpSpread5.Sheets[0].ColumnHeader.Cells[0, 16].Font.Size = FontUnit.Medium;
                            FpSpread5.Columns[16].Width = 100;
                            FpSpread5.Columns[16].Locked = true;
                            FpSpread5.Columns[16].Visible = false;

                            FpSpread5.Sheets[0].ColumnHeader.Cells[0, 17].Text = "Compare Code";
                            FpSpread5.Sheets[0].Columns[17].Visible = false;
                            FpSpread5.Sheets[0].ColumnHeader.Cells[0, 17].Font.Bold = true;
                            FpSpread5.Sheets[0].ColumnHeader.Cells[0, 17].HorizontalAlign = HorizontalAlign.Center;
                            FpSpread5.Sheets[0].ColumnHeader.Cells[0, 17].Font.Name = "Book Antiqua";
                            FpSpread5.Sheets[0].ColumnHeader.Cells[0, 17].Font.Size = FontUnit.Medium;
                        
                          
                          
                            FarPoint.Web.Spread.CheckBoxCellType chkall = new FarPoint.Web.Spread.CheckBoxCellType();
                            chkall.AutoPostBack = false;
                            string vennam = "";
                            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                            {
                                FpSpread5.Sheets[0].RowCount++;
                                FpSpread5.Sheets[0].Cells[FpSpread5.Sheets[0].RowCount - 1, 0].Text = Convert.ToString(i + 1);
                                FpSpread5.Sheets[0].Cells[FpSpread5.Sheets[0].RowCount - 1, 0].HorizontalAlign = HorizontalAlign.Center;
                                FpSpread5.Sheets[0].Cells[FpSpread5.Sheets[0].RowCount - 1, 0].Font.Size = FontUnit.Medium;
                                FpSpread5.Sheets[0].Cells[FpSpread5.Sheets[0].RowCount - 1, 0].Font.Name = "Book Antiqua";

                                if (vennam != Convert.ToString(ds.Tables[0].Rows[i]["VendorCompName"]))
                                {
                                    FpSpread5.Sheets[0].Cells[FpSpread5.Sheets[0].RowCount - 1, 1].CellType = chkall;
                                    FpSpread5.Sheets[0].Cells[FpSpread5.Sheets[0].RowCount - 1, 1].HorizontalAlign = HorizontalAlign.Center;
                                    FpSpread5.Sheets[0].SpanModel.Add(startspanpoint, 1, rowcount1, 1);
                                    startspanpoint = FpSpread5.Sheets[0].RowCount - 1;
                                }
                                else
                                {
                                    rowcount1++;
                                }
                                vennam = Convert.ToString(ds.Tables[0].Rows[i]["VendorCompName"]);

                                FpSpread5.Sheets[0].Cells[FpSpread5.Sheets[0].RowCount - 1, 2].Text = Convert.ToString(ds.Tables[0].Rows[i]["VendorCompName"]);
                                FpSpread5.Sheets[0].Cells[FpSpread5.Sheets[0].RowCount - 1, 2].Tag = Convert.ToString(ds.Tables[0].Rows[i]["VendorfK"]);
                                FpSpread5.Sheets[0].Cells[FpSpread5.Sheets[0].RowCount - 1, 2].HorizontalAlign = HorizontalAlign.Left;
                                FpSpread5.Sheets[0].Cells[FpSpread5.Sheets[0].RowCount - 1, 2].Font.Size = FontUnit.Medium;
                                FpSpread5.Sheets[0].Cells[FpSpread5.Sheets[0].RowCount - 1, 2].Font.Name = "Book Antiqua";

                                FpSpread5.Sheets[0].Cells[FpSpread5.Sheets[0].RowCount - 1, 3].Text = Convert.ToString(ds.Tables[0].Rows[i]["itemname"]);
                                FpSpread5.Sheets[0].Cells[FpSpread5.Sheets[0].RowCount - 1, 3].HorizontalAlign = HorizontalAlign.Left;
                                FpSpread5.Sheets[0].Cells[FpSpread5.Sheets[0].RowCount - 1, 3].Font.Size = FontUnit.Medium;
                                FpSpread5.Sheets[0].Cells[FpSpread5.Sheets[0].RowCount - 1, 3].Font.Name = "Book Antiqua";

                                double qty = Convert.ToDouble(ds.Tables[0].Rows[i]["Qty"]);
                                double rpu = Convert.ToDouble(ds.Tables[0].Rows[i]["RPU"]);
                                double discount = Convert.ToDouble(ds.Tables[0].Rows[i]["DiscountAmt"]);
                                double tax = Convert.ToDouble(ds.Tables[0].Rows[i]["TaxPercent"]);
                                double extax = Convert.ToDouble(ds.Tables[0].Rows[i]["ExeciseTaxPer"]);
                                double otherharge = Convert.ToDouble(ds.Tables[0].Rows[i]["OtherChargeAmt"]);
                                double cost = 0;
                                cost = qty * rpu;
                                if (discount != 0)
                                {
                                    cost = cost - discount;
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

                                //FpSpread5.Sheets[0].Cells[FpSpread5.Sheets[0].RowCount - 1, 4].Text = Convert.ToString(Math.Round(cost, 2));
                                //FpSpread5.Sheets[0].Cells[FpSpread5.Sheets[0].RowCount - 1, 4].HorizontalAlign = HorizontalAlign.Right;
                                //FpSpread5.Sheets[0].Cells[FpSpread5.Sheets[0].RowCount - 1, 4].Font.Size = FontUnit.Medium;
                                //FpSpread5.Sheets[0].Cells[FpSpread5.Sheets[0].RowCount - 1, 4].Font.Name = "Book Antiqua";

                                FpSpread5.Sheets[0].Cells[FpSpread5.Sheets[0].RowCount - 1, 4].Text = Convert.ToString(ds.Tables[0].Rows[i]["ItemCode"]);
                                FpSpread5.Sheets[0].Cells[FpSpread5.Sheets[0].RowCount - 1, 4].HorizontalAlign = HorizontalAlign.Left;
                                FpSpread5.Sheets[0].Cells[FpSpread5.Sheets[0].RowCount - 1, 4].Font.Size = FontUnit.Medium;
                                FpSpread5.Sheets[0].Cells[FpSpread5.Sheets[0].RowCount - 1, 4].Font.Name = "Book Antiqua";

                                FpSpread5.Sheets[0].Cells[FpSpread5.Sheets[0].RowCount - 1, 5].Text = Convert.ToString(ds.Tables[0].Rows[i]["qty"]);
                                FpSpread5.Sheets[0].Cells[FpSpread5.Sheets[0].RowCount - 1, 5].HorizontalAlign = HorizontalAlign.Right;
                                FpSpread5.Sheets[0].Cells[FpSpread5.Sheets[0].RowCount - 1, 5].Font.Size = FontUnit.Medium;
                                FpSpread5.Sheets[0].Cells[FpSpread5.Sheets[0].RowCount - 1, 5].Font.Name = "Book Antiqua";


                                FpSpread5.Sheets[0].Cells[FpSpread5.Sheets[0].RowCount - 1, 6].Text = Convert.ToString(ds.Tables[0].Rows[i]["RPU"]);
                                FpSpread5.Sheets[0].Cells[FpSpread5.Sheets[0].RowCount - 1, 6].HorizontalAlign = HorizontalAlign.Right;
                                FpSpread5.Sheets[0].Cells[FpSpread5.Sheets[0].RowCount - 1, 6].Font.Size = FontUnit.Medium;
                                FpSpread5.Sheets[0].Cells[FpSpread5.Sheets[0].RowCount - 1, 6].Font.Name = "Book Antiqua";


                                FpSpread5.Sheets[0].Cells[FpSpread5.Sheets[0].RowCount - 1, 7].Text = Convert.ToString(ds.Tables[0].Rows[i]["DiscountAmt"]);
                                FpSpread5.Sheets[0].Cells[FpSpread5.Sheets[0].RowCount - 1, 7].HorizontalAlign = HorizontalAlign.Right;
                                FpSpread5.Sheets[0].Cells[FpSpread5.Sheets[0].RowCount - 1, 7].Font.Size = FontUnit.Medium;
                                FpSpread5.Sheets[0].Cells[FpSpread5.Sheets[0].RowCount - 1, 7].Font.Name = "Book Antiqua";

                                FpSpread5.Sheets[0].Cells[FpSpread5.Sheets[0].RowCount - 1, 9].Text = Convert.ToString(ds.Tables[0].Rows[i]["TaxPercent"]);
                                FpSpread5.Sheets[0].Cells[FpSpread5.Sheets[0].RowCount - 1, 9].HorizontalAlign = HorizontalAlign.Right;
                                FpSpread5.Sheets[0].Cells[FpSpread5.Sheets[0].RowCount - 1, 9].Font.Size = FontUnit.Medium;
                                FpSpread5.Sheets[0].Cells[FpSpread5.Sheets[0].RowCount - 1, 9].Font.Name = "Book Antiqua";

                                FpSpread5.Sheets[0].Cells[FpSpread5.Sheets[0].RowCount - 1, 10].Text = Convert.ToString(ds.Tables[0].Rows[i]["ExeciseTaxPer"]);
                                FpSpread5.Sheets[0].Cells[FpSpread5.Sheets[0].RowCount - 1, 10].HorizontalAlign = HorizontalAlign.Right;
                                FpSpread5.Sheets[0].Cells[FpSpread5.Sheets[0].RowCount - 1, 10].Font.Size = FontUnit.Medium;
                                FpSpread5.Sheets[0].Cells[FpSpread5.Sheets[0].RowCount - 1, 10].Font.Name = "Book Antiqua";
                                FpSpread5.Sheets[0].Cells[FpSpread5.Sheets[0].RowCount - 1, 11].Text = Convert.ToString(ds.Tables[0].Rows[i]["EduCessPer"]);
                                FpSpread5.Sheets[0].Cells[FpSpread5.Sheets[0].RowCount - 1, 11].HorizontalAlign = HorizontalAlign.Right;
                                FpSpread5.Sheets[0].Cells[FpSpread5.Sheets[0].RowCount - 1, 11].Font.Size = FontUnit.Medium;
                                FpSpread5.Sheets[0].Cells[FpSpread5.Sheets[0].RowCount - 1, 11].Font.Name = "Book Antiqua";

                                FpSpread5.Sheets[0].Cells[FpSpread5.Sheets[0].RowCount - 1, 12].Text = Convert.ToString(ds.Tables[0].Rows[i]["HigherEduCessPer"]);
                                FpSpread5.Sheets[0].Cells[FpSpread5.Sheets[0].RowCount - 1, 12].HorizontalAlign = HorizontalAlign.Right;
                                FpSpread5.Sheets[0].Cells[FpSpread5.Sheets[0].RowCount - 1, 12].Font.Size = FontUnit.Medium;
                                FpSpread5.Sheets[0].Cells[FpSpread5.Sheets[0].RowCount - 1, 12].Font.Name = "Book Antiqua";

                                FpSpread5.Sheets[0].Cells[FpSpread5.Sheets[0].RowCount - 1, 13].Text = Convert.ToString(ds.Tables[0].Rows[i]["OtherChargeAmt"]);
                                FpSpread5.Sheets[0].Cells[FpSpread5.Sheets[0].RowCount - 1, 13].HorizontalAlign = HorizontalAlign.Right;
                                FpSpread5.Sheets[0].Cells[FpSpread5.Sheets[0].RowCount - 1, 13].Font.Size = FontUnit.Medium;
                                FpSpread5.Sheets[0].Cells[FpSpread5.Sheets[0].RowCount - 1, 13].Font.Name = "Book Antiqua";

                                //FpSpread5.Sheets[0].Cells[FpSpread5.Sheets[0].RowCount - 1, 14].Text = Convert.ToString(ds.Tables[0].Rows[i]["CallEduCess"]);
                                //FpSpread5.Sheets[0].Cells[FpSpread5.Sheets[0].RowCount - 1, 14].HorizontalAlign = HorizontalAlign.Right;
                                //FpSpread5.Sheets[0].Cells[FpSpread5.Sheets[0].RowCount - 1, 14].Font.Size = FontUnit.Medium;
                                //FpSpread5.Sheets[0].Cells[FpSpread5.Sheets[0].RowCount - 1, 14].Font.Name = "Book Antiqua";

                                //FpSpread5.Sheets[0].Cells[FpSpread5.Sheets[0].RowCount - 1, 15].Text = Convert.ToString(ds.Tables[0].Rows[i]["CallExTax"]);
                                //FpSpread5.Sheets[0].Cells[FpSpread5.Sheets[0].RowCount - 1, 15].HorizontalAlign = HorizontalAlign.Right;
                                //FpSpread5.Sheets[0].Cells[FpSpread5.Sheets[0].RowCount - 1, 15].Font.Size = FontUnit.Medium;
                                //FpSpread5.Sheets[0].Cells[FpSpread5.Sheets[0].RowCount - 1, 15].Font.Name = "Book Antiqua";


                                FpSpread5.Sheets[0].Cells[FpSpread5.Sheets[0].RowCount - 1, 16].Text = Convert.ToString(Math.Round(cost, 2));
                                FpSpread5.Sheets[0].Cells[FpSpread5.Sheets[0].RowCount - 1, 16].HorizontalAlign = HorizontalAlign.Right;
                                FpSpread5.Sheets[0].Cells[FpSpread5.Sheets[0].RowCount - 1, 16].Font.Size = FontUnit.Medium;
                                FpSpread5.Sheets[0].Cells[FpSpread5.Sheets[0].RowCount - 1, 16].Font.Name = "Book Antiqua";
                                FpSpread5.Sheets[0].Cells[FpSpread5.Sheets[0].RowCount - 1, 17].Tag = Convert.ToString(ds.Tables[0].Rows[i]["ReqCompCode"]);

                                chk = true;
                                if (chk == true)
                                {
                                    uncheck = "1";
                                }
                            }
                            if (cblcolumnorder.Items.Count > 0)
                            {
                                for (int i = 0; i < cblcolumnorder.Items.Count; i++)
                                {
                                    if (cblcolumnorder.Items[i].Selected == true)
                                    {
                                        string headername = Convert.ToString(cblcolumnorder.Items[i].ToString());

                                        if (headername == "Supplier Name")
                                        {
                                            FpSpread5.Columns[2].Visible = true;
                                        }
                                        else if (headername == "Item Name")
                                        {
                                            FpSpread5.Columns[3].Visible = true;
                                        }
                                        else if (headername == "Item Code")
                                        {
                                            FpSpread5.Columns[4].Visible = true;
                                        }
                                        else if (headername == "Quantity")
                                        {
                                            FpSpread5.Columns[5].Visible = true;
                                        }
                                        else if (headername == "Rpu")
                                        {
                                            FpSpread5.Columns[6].Visible = true;
                                        }
                                        else if (headername == "Discount")
                                        {
                                            FpSpread5.Columns[7].Visible = true;
                                        }
                                        else if (headername == "Tax")
                                        {
                                            FpSpread5.Columns[9].Visible = true;
                                        }
                                        else if (headername == "Exercies Tax")
                                        {
                                            FpSpread5.Columns[10].Visible = true;
                                        }
                                        else if (headername == "Education Cess")
                                        {
                                            FpSpread5.Columns[11].Visible = true;
                                        }
                                        else if (headername == " Higher Education Cess")
                                        {
                                            FpSpread5.Columns[12].Visible = true;
                                        }
                                        else if (headername == "Other Charges")
                                        {
                                            FpSpread5.Columns[13].Visible = true;
                                        }
                                        else if (headername == "Cost")
                                        {
                                            FpSpread5.Columns[16].Visible = true;
                                        }
                                        check = true;
                                    }
                                }
                            }
                            if (check == false)
                            {
                                CheckBox_column.Checked = true;
                                LinkButtonsremove_Click(sender, e);
                                for (int i = 0; i < cblcolumnorder.Items.Count; i++)
                                {
                                    if (cblcolumnorder.Items[i].Selected == true)
                                    {
                                        string headername = Convert.ToString(cblcolumnorder.Items[i].ToString());

                                        if (headername == "Supplier Name")
                                        {
                                            FpSpread5.Columns[2].Visible = true;
                                        }
                                        else if (headername == "Item Name")
                                        {
                                            FpSpread5.Columns[3].Visible = true;
                                        }
                                        else if (headername == "Item Code")
                                        {
                                            FpSpread5.Columns[4].Visible = true;
                                        }
                                        else if (headername == "Quantity")
                                        {
                                            FpSpread5.Columns[5].Visible = true;
                                        }
                                        else if (headername == "Rpu")
                                        {
                                            FpSpread5.Columns[6].Visible = true;
                                        }
                                        else if (headername == "Discount")
                                        {
                                            FpSpread5.Columns[7].Visible = true;
                                        }
                                        else if (headername == "Tax")
                                        {
                                            FpSpread5.Columns[9].Visible = true;
                                        }
                                        else if (headername == "Exercies Tax")
                                        {
                                            FpSpread5.Columns[10].Visible = true;
                                        }
                                        else if (headername == "Education Cess")
                                        {
                                            FpSpread5.Columns[11].Visible = true;
                                        }
                                        else if (headername == " Higher Education Cess")
                                        {
                                            FpSpread5.Columns[12].Visible = true;
                                        }
                                        else if (headername == "Other Charges")
                                        {
                                            FpSpread5.Columns[13].Visible = true;
                                        }
                                        else if (headername == "Cost")
                                        {
                                            FpSpread5.Columns[16].Visible = true;
                                        }
                                    }
                                }
                            }
                            FpSpread5.Sheets[0].PageSize = FpSpread5.Sheets[0].RowCount;
                            FpSpread5.Sheets[0].SetColumnMerge(2, FarPoint.Web.Spread.Model.MergePolicy.Always);
                            //FpSpread5.Sheets[0].SetColumnMerge(1, FarPoint.Web.Spread.Model.MergePolicy.Always);
                            FpSpread5.Sheets[0].FrozenRowCount = 0;
                            FpSpread5.Visible = true;
                            spreaddiv1.Visible = true;
                            btn_purchasereq.Visible = true;
                            rptprint.Visible = true;
                            lbl_baseerror.Visible = false;
                            pheaderfilter.Visible = true;
                            //tborder.Visible = true;
                            rowcount = FpSpread5.Rows.Count;
                           
                        }
                        else
                        {
                            FpSpread5.Visible = false;
                            spreaddiv1.Visible = false;
                            lbl_baseerror.Visible = true;
                            btn_purchasereq.Visible = false;
                            rptprint.Visible = false;

                            lbl_baseerror.Text = "No Record Founds";
                        }
                    }

                }
                if (uncheck.Trim() != "1")
                {
                    FpSpread5.Visible = false;
                    spreaddiv1.Visible = false;
                    lblalerterr.Visible = true;
                    alertpopwindow.Visible = true;

                    lblalerterr.Text = "Please Select Any One Item";
                }
            }
           
        }
        catch { }

    }
    protected void btnerrclose_Click(object sender, EventArgs e)
    {
        alertpopwindow.Visible = false;
    }
    protected void ddl_reqcompcode_bind()
    {
        try
        {
            string q1 = "select distinct reqcompcode from it_vendorreq where PurchaseStatus<>'1' order by ReqCompCode Asc";
            
            ds.Clear();
            ds = d2.select_method_wo_parameter(q1, "Text");
            if (ds.Tables[0].Rows.Count > 0)
            {
                ddl_reqcompcode.DataSource = ds;
                ddl_reqcompcode.DataTextField = "ReqCompCode";
                ddl_reqcompcode.DataBind();
                ddl_reqcompcode.Items.Insert(0, "Select");
            }
        }
        catch { }
    }
    protected void ddl_reqcompcode_selectedIndexchange(object sender, EventArgs e)
    {
        try
        {
            string venquery = "select distinct vq.Vendorfk,vm.VendorCompName from CO_VendorMaster vm,IT_VendorReq vq where vm.VendorPK=vq.VendorFK and vq.ReqCompCode in('" + ddl_reqcompcode.SelectedItem.Text + "') ";
            ds = d2.select_method_wo_parameter(venquery, "Text");
            if (ds.Tables[0].Rows.Count > 0)
            {
                cbl_venname.DataSource = ds;
                cbl_venname.DataTextField = "VendorCompName";
                cbl_venname.DataValueField = "Vendorfk";
                cbl_venname.DataBind();
                if (cbl_venname.Items.Count > 0)
                {
                    for (int i = 0; i < cbl_venname.Items.Count; i++)
                    {
                        cbl_venname.Items[i].Selected = true;
                    }
                    txt_venname.Text = "Supplier Name(" + cbl_venname.Items.Count + ")";
                }
            }
        }
        catch
        { }
    }

    protected void btnExcel_Click(object sender, EventArgs e)
    {
        try
        {
            string reportname = txtexcelname.Text;
            if (reportname.ToString().Trim() != "")
            {
                d2.printexcelreport(FpSpread5, reportname);
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
            string degreedetails = "Supplier Quotation Compare";
            string pagename = "vendor_quotation_compare.aspx";
            Printcontrol.loadspreaddetails(FpSpread5, pagename, degreedetails);
            Printcontrol.Visible = true;
        }
        catch
        {

        }

    }
    protected void btn_purchase_request_Click(object sender, EventArgs e)
    {
        try
        {
            bool insert = false;
            int spreaditemchk = 0;
            if (FpSpread5.Sheets[0].RowCount > 0)
            {
                FpSpread5.SaveChanges();
                for (int row = 0; row < FpSpread5.Sheets[0].RowCount; row++)
                {
                    int checkval = Convert.ToInt32(FpSpread5.Sheets[0].Cells[row, 1].Value);
                    if (checkval == 1)
                    {
                        spreaditemchk = spreaditemchk + 1;
                    }
                }

                if (spreaditemchk == 1)
                {
                    if (FpSpread5.Sheets[0].RowCount > 0)
                    {
                        FpSpread5.SaveChanges();
                        string purchasevendorfk = "";
                        string recomcode = "";
                        for (int row = 0; row < FpSpread5.Sheets[0].RowCount; row++)
                        {
                            int checkval = Convert.ToInt32(FpSpread5.Sheets[0].Cells[row, 1].Value);
                            if (checkval == 1)
                            {
                                purchasevendorfk = Convert.ToString(FpSpread5.Sheets[0].Cells[row, 2].Tag);
                                recomcode= Convert.ToString(FpSpread5.Sheets[0].Cells[row, 17].Tag );
                                Response.Redirect("inv_purchase.aspx?@@barath$$=" + (purchasevendorfk)+","+(recomcode));
                            }
                        }
                    }
                    //string get_value = (Request.QueryString["app"].ToString());
                    //Response.Redirect("IndReport.aspx?app=" + Encrypt(app_no_stud));
                }
                else if (spreaditemchk > 1)
                {
                    lblalerterr.Visible = true;
                    alertpopwindow.Visible = true;
                    lblalerterr.Text = "You can't select more then one vendor";
                }
                else if (spreaditemchk < 1)
                {
                    lblalerterr.Visible = true;
                    alertpopwindow.Visible = true;
                    lblalerterr.Text = "Please select any one vendor";
                }

            }
            if (insert == true)
            {
                lblalerterr.Visible = true;
                alertpopwindow.Visible = true;
                lblalerterr.Text = "Saved Successfully";
            }
        }
        catch
        { }
    }

    protected void LinkButtonsremove_Click(object sender, EventArgs e)
    {
        try
        {
            if (CheckBox_column.Checked == true)
            {
                for (int i = 0; i < cblcolumnorder.Items.Count; i++)
                {
                    cblcolumnorder.Items[i].Selected = true;
                }
            }
            else
            {
                for (int i = 0; i < cblcolumnorder.Items.Count; i++)
                {
                    cblcolumnorder.Items[i].Selected = false;
                }
            }
        }
        catch { }
    }
    protected void CheckBox_column_CheckedChanged(object sender, EventArgs e)
    { }
    protected void cblcolumnorder_SelectedIndexChanged(object sender, EventArgs e)
    {
    }
    //column order old method
    //protected void cblcolumnorder_SelectedIndexChanged(object sender, EventArgs e)
    //{

    //    try
    //    {
    //        CheckBox_column.Checked = false;
    //        string value = "";
    //        int index;
    //        cblcolumnorder.Items[0].Selected = true;
    //        // cblcolumnorder.Items[0].Enabled = false;
    //        value = string.Empty;
    //        string result = Request.Form["__EVENTTARGET"];
    //        string[] checkedBox = result.Split('$');
    //        index = int.Parse(checkedBox[checkedBox.Length - 1]);
    //        string sindex = Convert.ToString(index);
    //        if (cblcolumnorder.Items[index].Selected)
    //        {
    //            if (!Itemindex.Contains(sindex))
    //            {
    //                //if (tborder.Text == "")
    //                //{
    //                //    ItemList.Add("Company Code");
    //                //}
    //                //ItemList.Add("Admission No");
    //                //ItemList.Add("Name");
    //                ItemList.Add(cblcolumnorder.Items[index].Text.ToString());
    //                Itemindex.Add(sindex);
    //            }
    //        }
    //        else
    //        {
    //            ItemList.Remove(cblcolumnorder.Items[index].Text.ToString());
    //            Itemindex.Remove(sindex);
    //        }
    //        for (int i = 0; i < cblcolumnorder.Items.Count; i++)
    //        {
    //            if (cblcolumnorder.Items[i].Selected == false)
    //            {
    //                sindex = Convert.ToString(i);
    //                ItemList.Remove(cblcolumnorder.Items[i].Text.ToString());
    //                Itemindex.Remove(sindex);
    //            }
    //        }

    //        lnk_columnorder.Visible = true;
    //        tborder.Visible = false;
    //        tborder.Text = "";
    //        string colname12 = "";
    //        for (int i = 0; i < ItemList.Count; i++)
    //        {
    //            if (colname12 == "")
    //            {
    //                colname12 = ItemList[i].ToString() + "(" + (i + 1).ToString() + ")";
    //            }
    //            else
    //            {
    //                colname12 = colname12 + "," + ItemList[i].ToString() + "(" + (i + 1).ToString() + ")";
    //            }
    //            tborder.Text = tborder.Text + ItemList[i].ToString();

    //            tborder.Text = tborder.Text + "(" + (i + 1).ToString() + ")";

    //        }
    //        tborder.Text = colname12;
    //        if (ItemList.Count == 14)
    //        {
    //            CheckBox_column.Checked = true;
    //        }
    //        if (ItemList.Count == 0)
    //        {
    //            tborder.Visible = false;
    //            lnk_columnorder.Visible = false;
    //        }
    //    }
    //    catch (Exception ex)
    //    {

    //    }
    //}
    //protected void LinkButtonsremove_Click(object sender, EventArgs e)
    //{
    //    cblcolumnorder.ClearSelection();
    //    CheckBox_column.Checked = false;
    //    lnk_columnorder.Visible = false;
    //    //cblcolumnorder.Items[0].Selected = true;
    //    ItemList.Clear();
    //    Itemindex.Clear();
    //    tborder.Text = "";
    //    tborder.Visible = false;
    //}
    //protected void CheckBox_column_CheckedChanged(object sender, EventArgs e)
    //{
    //    try
    //    {
    //        if (CheckBox_column.Checked == true)
    //        {
    //            tborder.Text = "";
    //            ItemList.Clear();
    //            for (int i = 0; i < cblcolumnorder.Items.Count; i++)
    //            {
    //                string si = Convert.ToString(i);
    //                cblcolumnorder.Items[i].Selected = true;
    //                lnk_columnorder.Visible = true;
    //                ItemList.Add(cblcolumnorder.Items[i].Text.ToString());
    //                Itemindex.Add(si);
    //            }
    //            lnk_columnorder.Visible = true;
    //            tborder.Visible = true;
    //            tborder.Text = "";
    //            int j = 0;
    //            string colname12 = "";
    //            for (int i = 0; i < ItemList.Count; i++)
    //            {
    //                j = j + 1;
    //                if (colname12 == "")
    //                {
    //                    colname12 = ItemList[i].ToString() + "(" + (j).ToString() + ")";

    //                }
    //                else
    //                {
    //                    colname12 = colname12 + "," + ItemList[i].ToString() + "(" + (j).ToString() + ")";
    //                }
    //            }
    //            tborder.Text = colname12;
    //        }
    //        else
    //        {
    //            for (int i = 0; i < cblcolumnorder.Items.Count; i++)
    //            {
    //                cblcolumnorder.Items[i].Selected = false;
    //                lnk_columnorder.Visible = false;
    //                ItemList.Clear();
    //                Itemindex.Clear();
    //                //cblcolumnorder.Items[0].Selected = true;
    //            }

    //            tborder.Text = "";
    //            tborder.Visible = false;

    //        }
    //    }
    //    catch (Exception ex)
    //    {

    //    }
    //}

}