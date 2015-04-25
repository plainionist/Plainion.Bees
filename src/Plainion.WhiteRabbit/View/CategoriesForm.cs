using System;
using System.Data;
using System.Windows.Forms;
using Plainion.WhiteRabbit.Presentation;

namespace Plainion.WhiteRabbit
{
    public partial class CategoriesForm : Form
    {
        private Controller myController = null;
        private DataTable myTable = null;

        public CategoriesForm( Controller ctrl )
        {
            myController = ctrl;

            InitializeComponent();

            CancelButton = myCancelBtn;

            myTable = myController.Database.LoadCategories();
            foreach ( DataRow row in myTable.Rows )
            {
                myCategoryList.Items.Add( row[ 0 ] as string );
            }
        }

        private void myCancelBtn_Click( object sender, EventArgs e )
        {
            Close();
        }

        private void myOkBtn_Click( object sender, EventArgs e )
        {
            myTable.Rows.Clear();
            foreach ( string name in myCategoryList.Items )
            {
                DataRow row = myTable.NewRow();
                row[ 0 ] = name;
                myTable.Rows.Add( row );
            }
            myTable.AcceptChanges();
            myController.Database.StoreCategories( myTable );

            Close();
        }

        private void myAddBtn_Click( object sender, EventArgs e )
        {
            if ( !myCategoryTxt.Text.IsNullOrTrimmedEmpty() )
            {
                myCategoryList.Items.Add( myCategoryTxt.Text );
            }
        }

        private void myDelBtn_Click( object sender, EventArgs e )
        {
            if ( myCategoryList.SelectedItem != null )
            {
                myCategoryList.Items.Remove( myCategoryList.SelectedItem );
            }
        }

        private void myCategoryList_SelectedIndexChanged( object sender, EventArgs e )
        {
            myCategoryTxt.Text = myCategoryList.SelectedItem as string;
        }

        private void myRenameBtn_Click( object sender, EventArgs e )
        {
            myController.RenameCategory( (string)myCategoryList.SelectedItem, myCategoryTxt.Text );

            var idx = myCategoryList.SelectedIndex;
            myCategoryList.Items[ idx ] = myCategoryTxt.Text;
        }
    }
}