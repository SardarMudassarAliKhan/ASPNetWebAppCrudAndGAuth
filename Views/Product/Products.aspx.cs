using ASPNetWebAppCrudAndGAuth.Entity;
using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ASPNetWebAppCrudAndGAuth.Views.Product
{
    public partial class Products : System.Web.UI.Page
    {
        AspNetEntities db = new AspNetEntities();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadCategories();         // for modal dropdown
                LoadCategoryFilter();     // for filter dropdown (includes "All")
                LoadProducts();           // initial grid
            }
        }

        // Modal dropdown
        void LoadCategories()
        {
            var cats = db.Categories.OrderBy(c => c.CategoryName).ToList();
            ddlCategory.DataSource = cats;
            ddlCategory.DataTextField = "CategoryName";
            ddlCategory.DataValueField = "CategoryId";
            ddlCategory.DataBind();
        }

        // Filter dropdown including "All" option (value = 0)
        void LoadCategoryFilter()
        {
            var cats = db.Categories.OrderBy(c => c.CategoryName).ToList();
            ddlFilterCategory.DataSource = cats;
            ddlFilterCategory.DataTextField = "CategoryName";
            ddlFilterCategory.DataValueField = "CategoryId";
            ddlFilterCategory.DataBind();

            // insert default "All" at top. Value "0" = treat as all categories.
            ddlFilterCategory.Items.Insert(0, new ListItem("-- All Categories --", "0"));
        }

        // Load products using optional search + category filter
        void LoadProducts(string search = null, int categoryId = 0)
        {
            var data = from p in db.Products
                       select new
                       {
                           p.ProductId,
                           p.ProductName,
                           p.Price,
                           p.CategoryId,
                           CategoryName = p.Category.CategoryName
                       };

            if (!string.IsNullOrWhiteSpace(search))
            {
                string s = search.Trim();
                data = data.Where(x => x.ProductName.Contains(s));
            }

            if (categoryId > 0)
            {
                data = data.Where(x => x.CategoryId == categoryId);
            }

            gvProducts.DataSource = data.OrderBy(x => x.ProductName).ToList();
            gvProducts.DataBind();
        }

        // Search button
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            int catId = ParseSelectedCategory(ddlFilterCategory.SelectedValue);
            LoadProducts(txtSearch.Text, catId);
        }

        // Filter changed
        protected void ddlFilterCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            int catId = ParseSelectedCategory(ddlFilterCategory.SelectedValue);
            LoadProducts(txtSearch.Text, catId);
        }

        // Helper parses selected value safely
        int ParseSelectedCategory(string selectedValue)
        {
            if (int.TryParse(selectedValue, out int id))
                return id;
            return 0;
        }

        // GridView Edit / Delete
        protected void gvProducts_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int index = Convert.ToInt32(e.CommandArgument);
            int id = Convert.ToInt32(gvProducts.DataKeys[index].Value);

            if (e.CommandName == "EditRow")
            {
                var p = db.Products.Find(id);
                if (p == null) return;

                // populate fields
                hfProductId.Value = p.ProductId.ToString();
                txtName.Text = p.ProductName;
                txtPrice.Text = p.Price.ToString();
                ddlCategory.SelectedValue = p.CategoryId.ToString();

                string showScript = @"
                                    setTimeout(function(){
                                        var modalEl = document.getElementById('productModal');
                                        if(modalEl){
                                            var modal = bootstrap.Modal.getInstance(modalEl) || new bootstrap.Modal(modalEl);
                                            modal.show();
                                        }
                                    }, 100);
                                    ";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "showModal", showScript, true);
            }
            else if (e.CommandName == "DeleteRow")
            {
                var p = db.Products.Find(id);
                if (p != null)
                {
                    db.Products.Remove(p);
                    db.SaveChanges();
                }

                // reload grid keeping current filters
                int catId = ParseSelectedCategory(ddlFilterCategory.SelectedValue);
                LoadProducts(txtSearch.Text, catId);
            }
        }

        // Save (Insert or Update)
        protected void btnSave_Click(object sender, EventArgs e)
        {
            // validate simple
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                // you may show a message; for now just return
                return;
            }

            if (!decimal.TryParse(txtPrice.Text, out decimal price))
            {
                // invalid price - show message or return
                return;
            }

            int selCat;
            if (!int.TryParse(ddlCategory.SelectedValue, out selCat))
            {
                return; // must select category
            }

            int id = 0;
            if (!string.IsNullOrEmpty(hfProductId.Value))
                int.TryParse(hfProductId.Value, out id);

            if (id == 0)
            {
                // Insert
                var p = new Entity.Product
                {
                    ProductName = txtName.Text.Trim(),
                    Price = price,
                    CategoryId = selCat
                };
                db.Products.Add(p);
            }
            else
            {
                // Update
                var p = db.Products.Find(id);
                if (p == null) return;

                p.ProductName = txtName.Text.Trim();
                p.Price = price;
                p.CategoryId = selCat;
            }

            db.SaveChanges();

            // Clear modal fields and hidden id
            hfProductId.Value = "";
            txtName.Text = "";
            txtPrice.Text = "";
            ddlCategory.SelectedIndex = 0;

            string hideScript = @"
                            setTimeout(function(){
                                var modalEl = document.getElementById('productModal');
                                if(modalEl){
                                    var modal = bootstrap.Modal.getInstance(modalEl) || new bootstrap.Modal(modalEl);
                                    modal.hide();
                                }
                            }, 100);
                            ";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "hideModal", hideScript, true);

            // Reload grid with current search + filter
            int catId = ParseSelectedCategory(ddlFilterCategory.SelectedValue);
            LoadProducts(txtSearch.Text, catId);
        }
    }
}