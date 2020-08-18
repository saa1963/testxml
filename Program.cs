using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Data.SqlClient;
using System.Security;

namespace testxml
{
    class Program
    {
        enum Fields1
        {
            v_num, v_num1, v_data, v_isx_num, v_isx_data, v_otkogo, 
            v_info, r_name, v_ruk_data, v_srok, o_name, d_name, v_regnum, v_id
        }
        static void Main(string[] args)
        {
            SecureString secureUserPassword = new SecureString();
            string userId = "sa";
            string userPassword = "1";
            foreach (var ch in userPassword)
            {
                secureUserPassword.AppendChar(ch);
            }
            secureUserPassword.MakeReadOnly();
            try
            {
                XmlDocument root = new XmlDocument();
                root.AppendChild(root.CreateXmlDeclaration("1.0", "UTF-8", null));
                XmlNode docroot = root.AppendChild(root.CreateElement("root"));
                using (var cn = new SqlConnection("Data Source=oragala;Initial Catalog=galadelo;MultipleActiveResultSets=True",
                    new SqlCredential(userId, secureUserPassword)))
                {
                    cn.Open();
                    SqlCommand cmd =
                        new SqlCommand(
                            @"
                            SELECT 
	                            v.num v_num,            --0
	                            v.num1 v_num1,          --1
	                            v.data v_data,          --2
	                            v.isx_num v_isx_num,    --3
	                            v.isx_data v_isx_data,  --4
	                            v.otkogo v_otkogo,      --5
	                            v.info v_info,          --6
	                            r.name r_name,          --7
	                            v.ruk_data v_ruk_data,  --8
	                            v.srok v_srok,          --9
	                            o.name o_name,          --10
                                d.name d_name,          --11
                                v.regnum v_regnum,      --12
                                v.id v_id               --13
                            FROM dbo.vxod v LEFT JOIN dbo.otdel o ON v.otdel = o.id
	                            LEFT JOIN sotr r ON v.ruk = r.id
                                LEFT JOIN dostavka d ON v.dostavka = d.id
                            order by data", 
                            cn);
                    SqlCommand cmd1 = new SqlCommand(
                        @"
                        SELECT
                            s.name s_name,
                            vs.sign vs_sign
                        FROM dbo.vxod_sotr vs LEFT JOIN dbo.sotr s ON vs.sotr = s.id
                        WHERE vs.vxod = @vxod
                        ", cn);
                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            var vxod = root.CreateElement("vxod");

                            var el = root.CreateElement("num");
                            el.InnerText = dr["v_num"].ToString();
                            vxod.AppendChild(el);

                            el = root.CreateElement("num1");
                            el.InnerText = dr["v_num1"].ToString();
                            vxod.AppendChild(el);

                            el = root.CreateElement("data");
                            el.InnerText = ((DateTime)dr["v_data"]).ToString("dd.MM.yyyy");
                            vxod.AppendChild(el);

                            el = root.CreateElement("isx_num");
                            el.InnerText = dr["v_isx_num"] is DBNull ? "" : dr["v_isx_num"].ToString();
                            vxod.AppendChild(el);

                            el = root.CreateElement("isx_data");
                            ind = dr.GetOrdinal("v_isx_data");
                            el.InnerText = dr.IsDBNull(ind) 
                                ? "" : dr.GetDateTime(ind).ToString("dd.MM.yyyy");
                            vxod.AppendChild(el);

                            el = root.CreateElement("otkogo");
                            ind = dr.GetOrdinal("v_otkogo");
                            el.InnerText = dr.IsDBNull(ind) ? "" : dr.GetString(ind);
                            vxod.AppendChild(el);

                            el = root.CreateElement("info");
                            ind = dr.GetOrdinal("v_info");
                            el.InnerText = dr.IsDBNull(ind) ? "" : dr.GetString(ind);
                            vxod.AppendChild(el);

                            el = root.CreateElement("ruk");
                            ind = dr.GetOrdinal("r_name");
                            el.InnerText = dr.GetString(ind);
                            vxod.AppendChild(el);

                            el = root.CreateElement("ruk_data");
                            ind = dr.GetOrdinal("v_ruk_data");
                            el.InnerText = dr.GetDateTime(ind).ToString("dd.MM.yyyy");
                            vxod.AppendChild(el);

                            el = root.CreateElement("srok");
                            ind = dr.GetOrdinal("v_srok");
                            el.InnerText = dr.IsDBNull(ind) ? "" : dr.GetInt32(ind).ToString();
                            vxod.AppendChild(el);

                            el = root.CreateElement("otdel");
                            ind = dr.GetOrdinal("o_name");
                            el.InnerText = dr.IsDBNull(ind) ? "" : dr.GetString(ind);
                            vxod.AppendChild(el);

                            el = root.CreateElement("dostavka");
                            ind = dr.GetOrdinal("d_name");
                            el.InnerText = dr.IsDBNull(ind) ? "" : dr.GetString(ind);
                            vxod.AppendChild(el);

                            el = root.CreateElement("regnum");
                            ind = dr.GetOrdinal("v_regnum");
                            el.InnerText = dr.IsDBNull(ind) ? "" : dr.GetString(ind);
                            vxod.AppendChild(el);

                            cmd1.Parameters.Clear();
                            ind = dr.GetOrdinal("v_id");
                            cmd1.Parameters.AddWithValue("@vxod", dr.GetInt32(ind));

                            el = root.CreateElement("vxod_sotr");
                            vxod.AppendChild(el);
                            using (var dr1 = cmd1.ExecuteReader())
                            {
                                while (dr1.Read())
                                {
                                    var elem = root.CreateElement("sotr");
                                    var ind1 = dr1.GetOrdinal("s_name");
                                    elem.InnerText = dr1.GetString(ind1);
                                    el.AppendChild(elem);

                                    elem = root.CreateElement("sign");
                                    ind1 = dr1.GetOrdinal("vs_sign");
                                    elem.InnerText = dr1.IsDBNull(ind1) ? "Нет данных" : dr1.GetBoolean(ind1).ToString();
                                    el.AppendChild(elem);
                                }
                            }
                            docroot.AppendChild(vxod);
                        }
                    }
                    root.Save(@"c:\1\qq.xml");
                }
            }
            finally
            {
                secureUserPassword.Dispose();
            }
        }
    }
}
