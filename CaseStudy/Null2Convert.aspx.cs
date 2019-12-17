using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CaseStudy
{
    public partial class Null2Convert : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string a;
            a = Convert.ToString(DBNull.Value);
            Response.Write("value>>" + a);
        }
    }
}