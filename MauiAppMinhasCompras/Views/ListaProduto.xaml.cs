using MauiAppMinhasCompras.Models;

namespace MauiAppMinhasCompras.Views;

public partial class ListaProduto : ContentPage
{
    Produto produtoSelecionado;

    public ListaProduto()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CarregarProdutos();
    }

    private async Task CarregarProdutos()
    {
        var produtos = await App.Db.GetAll();
        lst_produtos.ItemsSource = produtos;
        lblTotalGeral.Text = $"Total Geral: {produtos.Sum(p => p.Total):C}";
    }

    private async void ToolbarItem_Adicionar_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new NovoProduto());
    }

    private void lst_produtos_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        produtoSelecionado = e.SelectedItem as Produto;
    }

    private async void ToolbarItem_Editar_Clicked(object sender, EventArgs e)
    {
        if (produtoSelecionado == null)
        {
            await DisplayAlert("Aviso", "Selecione um produto para editar.", "OK");
            return;
        }

        await Navigation.PushAsync(new EditarProduto { BindingContext = produtoSelecionado });
    }

    private async void ToolbarItem_Excluir_Clicked(object sender, EventArgs e)
    {
        if (produtoSelecionado == null)
        {
            await DisplayAlert("Aviso", "Selecione um produto para excluir.", "OK");
            return;
        }

        bool confirmar = await DisplayAlert("Confirmação", $"Deseja excluir o produto '{produtoSelecionado.Descricao}'?", "Sim", "Não");
        if (confirmar)
        {
            await App.Db.Delete(produtoSelecionado.Id);
            produtoSelecionado = null;
            await CarregarProdutos();
        }
    }

    private async void ToolbarItem_Somar_Clicked(object sender, EventArgs e)
    {
        var produtos = await App.Db.GetAll();
        double somaTotal = produtos.Sum(p => p.Total);

        await DisplayAlert("Total da Compra", $"O valor total é {somaTotal:C}", "OK");
    }

    private async void MenuItem_Remover_Clicked(object sender, EventArgs e)
    {
        var menuItem = sender as MenuItem;
        var produto = menuItem?.BindingContext as Produto;

        bool confirmar = await DisplayAlert("Confirmação", $"Deseja excluir o produto '{produto.Descricao}'?", "Sim", "Não");
        if (confirmar)
        {
            await App.Db.Delete(produto.Id);
            await CarregarProdutos();
        }
    }

    private async void lst_produtos_Refreshing(object sender, EventArgs e)
    {
        await CarregarProdutos();
        lst_produtos.IsRefreshing = false;
    }
}
