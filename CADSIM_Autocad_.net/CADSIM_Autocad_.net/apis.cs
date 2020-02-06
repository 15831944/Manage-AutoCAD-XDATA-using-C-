using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;


namespace CADSIM_Autocad_.net
{
    static class XData
    {
        public static ObjectId objid;
        //public static string appName;
        //public static string value;


    }
    class apis
    {
        //function to add xdata to selected object
        public static void addXData(ObjectId objectid, string appName, string value)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Transaction tr = doc.TransactionManager.StartTransaction();
            using (tr)
            {
                doc.LockDocument();
                DBObject obj = tr.GetObject(objectid,OpenMode.ForWrite);
                AddRegAppTableRecord(appName.ToString());
                ResultBuffer rb =new ResultBuffer(new TypedValue(1001, appName.ToString()), new TypedValue(1000, value.ToString()));
                obj.XData = rb;
                rb.Dispose();
                tr.Commit();
            }
        }
        static void AddRegAppTableRecord(string regAppName)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;
            Database db = doc.Database;
            Transaction tr = doc.TransactionManager.StartTransaction();
            using (tr)
            {
                RegAppTable rat = (RegAppTable)tr.GetObject(db.RegAppTableId, OpenMode.ForRead, false);
                if (!rat.Has(regAppName))
                {
                    rat.UpgradeOpen();
                    RegAppTableRecord ratr = new RegAppTableRecord();
                    ratr.Name = regAppName;
                    rat.Add(ratr);
                    tr.AddNewlyCreatedDBObject(ratr, true);
                }
                tr.Commit();
            }
        }

        //this function is used to read all xdata of selected object
        // this is not used in dialog
        public static void fnreadXData(ObjectId objectid)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;
            Transaction tr = doc.TransactionManager.StartTransaction();
            using (tr)
            {
                DBObject obj = tr.GetObject(objectid, OpenMode.ForRead);
                ResultBuffer rb = obj.XData;
                if (rb == null)
                {
                    ed.WriteMessage("\nEntity does not have XData attached.");
                }
                else
                {
                    int n = 0;
                    foreach (TypedValue tv in rb)
                    {
                        ed.WriteMessage("\nTypedValue {0} - type: {1}, value: {2}", n++, tv.TypeCode, tv.Value);
                    }
                    rb.Dispose();
                }
            }
        }

        //Function to load xdata of selected object // used in dialog
        public static string loadXData(ObjectId objectid,string appName)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;
            Transaction tr = doc.TransactionManager.StartTransaction();
            using (tr)
            {
                DBObject obj = tr.GetObject(objectid, OpenMode.ForRead);
                Entity ent = (Entity)tr.GetObject(objectid, OpenMode.ForRead);
                ResultBuffer buffer = ent.GetXDataForApplication(appName.ToString());


                if (buffer != null)
                {
                    ResultBuffer rb = ent.XData;
                    foreach (TypedValue tv in buffer)
                    {
                        if (tv.TypeCode == 1000) 
                            return tv.Value.ToString(); //ed.WriteMessage(tv.Value.ToString());//
                    }
                    rb.Dispose();  //should be written before return
                    buffer.Dispose(); //should be written before return
                }
                return "";

            }
        }

        //funtion to update xdata of selected object using appname
        public static void updateXData(ObjectId objectid,string appName, string newVal)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;


            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                doc.LockDocument();
                Entity ent = (Entity)tr.GetObject(objectid, OpenMode.ForWrite);
                    ResultBuffer resbuf = ent.GetXDataForApplication(appName.ToString());
                    TypedValue[] data = resbuf.AsArray();
                    for (int i = 0; i < data.Length; i++)
                    {
                        TypedValue tv = data[i];
                        if (tv.TypeCode == 1000) // && (string)tv.Value == appName)
                        {
                            data[i] = new TypedValue(1000, newVal);
                        }
                    }
                    resbuf = new ResultBuffer(data);
                    ent.XData = resbuf;
                
                tr.Commit();
            }
        }

    }
}
