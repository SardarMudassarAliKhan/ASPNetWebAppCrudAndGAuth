<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Products.aspx.cs" Inherits="ASPNetWebAppCrudAndGAuth.Views.Product.Products" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<form id="form1" runat="server" class="container mt-4">

    <h2 class="mb-4 text-center">Product Management (Bootstrap + EF)</h2>

    <!-- Filter Section -->
    <div class="row mb-4">
        <div class="col-md-4">
            <asp:DropDownList ID="ddlFilterCategory" runat="server" CssClass="form-select"
                AutoPostBack="true" OnSelectedIndexChanged="ddlFilterCategory_SelectedIndexChanged">
            </asp:DropDownList>
        </div>
    </div>

    <!-- Add New Button -->
    <div class="mb-3">
        <button type="button" class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#productModal">
            Add New Product
        </button>
    </div>

    <!-- GridView -->
    <asp:GridView ID="gvProducts" runat="server" CssClass="table table-bordered table-striped"
        AutoGenerateColumns="False" DataKeyNames="ProductId"
        OnRowCommand="gvProducts_RowCommand">
        <Columns>

            <asp:BoundField DataField="ProductId" HeaderText="ID" />
            <asp:BoundField DataField="ProductName" HeaderText="Product" />
            <asp:BoundField DataField="Price" HeaderText="Price" />
            <asp:BoundField DataField="CategoryName" HeaderText="Category" />

            <asp:TemplateField HeaderText="Actions">
                <ItemTemplate>
                    <asp:Button CssClass="btn btn-sm btn-warning me-2" Text="Edit" 
                        CommandName="EditRow" CommandArgument="<%# Container.DataItemIndex %>" runat="server" />

                    <asp:Button CssClass="btn btn-sm btn-danger" Text="Delete"
                        CommandName="DeleteRow" CommandArgument="<%# Container.DataItemIndex %>" runat="server" />
                </ItemTemplate>
            </asp:TemplateField>

        </Columns>
    </asp:GridView>

    <!-- Modal Popup -->
    <div class="modal fade" id="productModal" tabindex="-1">
        <div class="modal-dialog">
            <div class="modal-content">

                <div class="modal-header bg-primary text-white">
                    <h5 class="modal-title">Product Form</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                </div>

                <div class="modal-body">

                    <asp:HiddenField ID="hfProductId" runat="server" />

                    <div class="mb-3">
                        <label>Product Name</label>
                        <asp:TextBox ID="txtName" CssClass="form-control" runat="server"></asp:TextBox>
                    </div>

                    <div class="mb-3">
                        <label>Price</label>
                        <asp:TextBox ID="txtPrice" CssClass="form-control" runat="server"></asp:TextBox>
                    </div>

                    <div class="mb-3">
                        <label>Category</label>
                        <asp:DropDownList ID="ddlCategory" CssClass="form-select" runat="server"></asp:DropDownList>
                    </div>

                </div>

                <div class="modal-footer">
                    <asp:Button ID="btnSave" runat="server" CssClass="btn btn-success" Text="Save" OnClick="btnSave_Click" />
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
                </div>

            </div>
        </div>
    </div>

</form>
</asp:Content>
