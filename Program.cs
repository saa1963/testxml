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
        static void Main(string[] args)
        {
            using (XmlWriter xr = XmlWriter.Create(@"c:\1\qq.xml", new XmlWriterSettings 
            { 
                 Indent = true
            }))
            {
                xr.WriteStartDocument();
                xr.WriteStartElement("root");
                foreach (var o in getRecord())
                {
                    xr.WriteStartElement("item");
                    xr.WriteAttributeString("product", o.Item1);
                    xr.WriteAttributeString("price", o.Item2.ToString());
                    xr.WriteEndElement();
                }
                xr.WriteEndElement();
                xr.WriteEndDocument();
            }
        }
        private static IEnumerable<Tuple<string, int>> getRecord()
        {
            List<Tuple<string, int>> lst = new List<Tuple<string, int>>();
            using (var cn = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;
                                                Initial Catalog=Northwind;
                                                Integrated Security=SSPI;
                                                MultipleActiveResultSets=True"))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand(@"SELECT 
                                                    ProductName,    -- 0
                                                    UnitPrice       -- 1
                                                    FROM Products
                                                    WHERE(((UnitPrice) >= 15 And(UnitPrice) <= 25)
                                                    AND((Products.Discontinued) = 0))
                                                    ORDER BY Products.UnitPrice DESC; ", cn);

                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lst.Add(Tuple.Create(dr.GetString(0), Convert.ToInt32(dr.GetValue(1))));
                    }
                }
            }
            foreach (var t in lst)
            {
                yield return t;
            }
        }
    }
}
