using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class Collinfo : System.Web.UI.Page
{
    DataSet ds = new DataSet();
    protected void Page_Load(object sender, EventArgs e)
    {
        DataTable dtCollInfi = new DataTable();
        dtCollInfi.Columns.Add("Affiliatedby");
        dtCollInfi.Columns.Add("AffiliatedYR");
        DataRow drNEw = null;

        gridAffiliation.Visible = true;
        for (int i = 1; i <= 3; i++)
        {
            drNEw = dtCollInfi.NewRow();
            drNEw["Affiliatedby"] = "";
            drNEw["AffiliatedYR"] = "";
            dtCollInfi.Rows.Add(drNEw);
        }
        if (dtCollInfi.Rows.Count > 0)
        {
            gridAffiliation.DataSource = dtCollInfi;
            gridAffiliation.DataBind();
        }
        
    }

    protected void ddlcollege1_SelectedIndexChanged(object sender, EventArgs e)
    {

    }

}