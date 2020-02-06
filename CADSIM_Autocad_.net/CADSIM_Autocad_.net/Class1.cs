using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.Runtime;

using Autodesk.AutoCAD.ApplicationServices;

using Autodesk.AutoCAD.DatabaseServices;

using Autodesk.AutoCAD.EditorInput;

using Autodesk.AutoCAD.Geometry;

namespace CADSIM_Autocad_.net
{
    public class Commands

  {
        public ObjectId objid;
    [CommandMethod("CB")]

    public void CreateBlock()

    {

        Document doc = Application.DocumentManager.MdiActiveDocument;
        Database db = doc.Database;
        Editor ed = doc.Editor;
        Transaction tr = db.TransactionManager.StartTransaction();
        using (tr)
        {
                // Get the block table from the drawing
            BlockTable bt =(BlockTable)tr.GetObject( db.BlockTableId, OpenMode.ForRead);
            // Check the block name, to see whether it's
            // already in use
            PromptStringOptions pso = new PromptStringOptions("\nEnter new block name: ");
            pso.AllowSpaces = true;
            // A variable for the block's name
            string blkName = "";
            do
            {
                PromptResult pr = ed.GetString(pso);
                // Just return if the user cancelled
                // (will abort the transaction as we drop out of the using
                // statement's scope)
                if (pr.Status != PromptStatus.OK)
                    return;
                try
                {
                    // Validate the provided symbol table name
                    SymbolUtilityServices.ValidateSymbolName(pr.StringResult,false);
                    // Only set the block name if it isn't in use
                    if (bt.Has(pr.StringResult))
                        ed.WriteMessage("\nA block with this name already exists.");
                    else
                        blkName = pr.StringResult;
                }
                catch
                {
                    // An exception has been thrown, indicating the
                    // name is invalid
                    ed.WriteMessage("\nInvalid block name.");
                }
            } while (blkName == "");
            // Create our new block table record...
            BlockTableRecord btr = new BlockTableRecord();
            // ... and set its properties
            btr.Name = blkName;
            //input insert point
            PromptPointOptions ppo = new PromptPointOptions("\nSelect point to insert block");
            PromptPointResult ppr = ed.GetPoint(ppo);
            Point3d pt = ppr.Value;

                


            // Add the new block to the block table
            bt.UpgradeOpen();
            ObjectId btrId = bt.Add(btr);
            tr.AddNewlyCreatedDBObject(btr, true);
            // Add some lines to the block to form a square
            // (the entities belong directly to the block)
            Circle circle = new Circle();
            circle.Center = pt;
            circle.Radius = 20;
            btr.AppendEntity(circle);
            tr.AddNewlyCreatedDBObject(circle, true);
            // Add a block reference to the model space
            BlockTableRecord ms =(BlockTableRecord)tr.GetObject(bt[BlockTableRecord.ModelSpace],OpenMode.ForWrite);
            BlockReference br = new BlockReference(Point3d.Origin, btrId);
            ms.AppendEntity(br);
            tr.AddNewlyCreatedDBObject(br, true);
            // Commit the transaction
            tr.Commit();
            // Report what we've done
            ed.WriteMessage( "\nCreated block named \"{0}\" .", blkName);
        }
    }



        [CommandMethod("GXD")]
        static public void GetXData()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;
            // Ask the user to select an entity
            // for which to retrieve XData
            PromptEntityOptions opt = new PromptEntityOptions( "\nSelect entity: " );
            PromptEntityResult res =ed.GetEntity(opt);
            if (res.Status == PromptStatus.OK)
            {
                apis.fnreadXData(res.ObjectId);
                
            }
        }
        [CommandMethod("SXD")]
        static public void SetXData()
        {

            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;
            // Ask the user to select an entity
            // for which to set XData
            PromptEntityOptions opt = new PromptEntityOptions("\nSelect entity: ");
            PromptEntityResult res = ed.GetEntity(opt);
            if (res.Status == PromptStatus.OK)
            {
                XData.objid = res.ObjectId;

                ProjectInfo frmObj = new ProjectInfo();
                frmObj.Show();

                //PromptStringOptions pso = new PromptStringOptions("\nEnter xdata key: ");
                //PromptResult pr = ed.GetString(pso);
                //string key = pr.StringResult;

                //PromptStringOptions pso_val = new PromptStringOptions("\nEnter xdata value: ");
                //pso_val.AllowSpaces = true;
                //PromptResult pr_val = ed.GetString(pso_val);
                //string value = pr_val.StringResult;
                //apis.addXData(res.ObjectId, key, value);

            }
        }

       
        [CommandMethod("SXD2")]
        static public void SetXData2()
        {
            ProjectInfo frmObj = new ProjectInfo();
            frmObj.Show();
            
        }
        
        

    }

}
