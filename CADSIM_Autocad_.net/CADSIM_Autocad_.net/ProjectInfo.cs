using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.AutoCAD.Runtime;

using Autodesk.AutoCAD.ApplicationServices;

using Autodesk.AutoCAD.DatabaseServices;

using Autodesk.AutoCAD.EditorInput;

using Autodesk.AutoCAD.Geometry;

namespace CADSIM_Autocad_.net
{
    public partial class ProjectInfo : Form
    {
        public ProjectInfo()
        {
            InitializeComponent();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string pname = txtPName.Text;
            string location = txtLocation.Text;
            string ptype = txtPType.Text;
            apis.addXData(XData.objid, "PName", pname);
            apis.addXData(XData.objid, "location", location);
            apis.addXData(XData.objid, "type", ptype);
            MessageBox.Show("XDATA added");
            this.Hide();
            
        }

        private void btnSelectObject_Click(object sender, EventArgs e)
        {
            this.Hide();
            Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;
            //method1
            //PromptEntityOptions opt = new PromptEntityOptions("\nSelect entity: ");
            //PromptEntityResult res = ed.GetEntity(opt);

            //if (res.Status == PromptStatus.OK)
            //{
            //    this.Show();
            //    //SelectedObject res = resp.
            //    XData.objid = res.ObjectId;
            //    textBox1.Text = res.ObjectId.ToString();
            //    string pname = apis.loadXData(XData.objid, "pname");
            //    txtPName.Text = pname;
            //    string location = apis.loadXData(XData.objid, "location");
            //    txtLocation.Text = location;
            //    string type = apis.loadXData(XData.objid, "type");
            //    txtPType.Text = type;

            //}

            //method2
            PromptSelectionOptions selectionOpts = new PromptSelectionOptions();
            selectionOpts.SingleOnly = true;
            PromptSelectionResult psr =  ed.GetSelection(selectionOpts);

            if (psr.Status == PromptStatus.OK)
            {
                this.Show();
                ObjectId selId = psr.Value[0].ObjectId;
                XData.objid = selId;
                textBox1.Text = selId.ToString();
                string pname = apis.loadXData(XData.objid, "PName");
                txtPName.Text = pname;
                string location = apis.loadXData(XData.objid, "location");
                txtLocation.Text = location;
                string type = apis.loadXData(XData.objid, "type");
                txtPType.Text = type;
            }

        }

        private void ProjectInfo_Load(object sender, EventArgs e)
        {
            if(XData.objid != null)
            {
                textBox1.Text = XData.objid.ToString();
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            apis.updateXData(XData.objid, "pname", txtPName.Text);
            apis.updateXData(XData.objid, "location", txtLocation.Text);
            apis.updateXData(XData.objid, "type", txtPType.Text);
            MessageBox.Show("XData updated");
        }
    }
}
