using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using MauiAppMinhasCompras.Models;

namespace MauiAppMinhasCompras.Views
{
    public partial class ListaProduto : ContentPage
    {
        Produto produtoSelecionado;
        ObservableCollection<Produto> produtosLista = new ObservableCollection<Produto>();

        public ListaProduto()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await CarregarProdutos("");
        }

        private async Task CarregarProdutos(string filtro)
        {
            try
            {
                var lista = string.IsNullOrWhiteSpace(filtro)
                            ? await App.Db.GetAll()
                            : await App.Db.Search(filtro);

                produtosLista = new ObservableCollection<Produto>(lista);
                lst_produtos.ItemsSource = produtosLista;

                lblTotalGeral.Text = $"Total Geral: {lista.Sum(p => p.Total):C}";
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ops", $"Erro ao carregar produtos: {ex.Message}", "OK");
            }
        }

        private async void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            await CarregarProdutos(e.NewTextValue);
        }

        private async void lst_produtos_Refreshing(object sender, EventArgs e)
        {
            await CarregarProdutos(searchBar.Text);
            lst_produtos.IsRefreshing = false;
        }

        private async void ToolbarItem_Adicionar_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Views.NovoProduto());
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

            await Navigation.PushAsync(new Views.EditarProduto { BindingContext = produtoSelecionado });
        }

        private async void ToolbarItem_Excluir_Clicked(object sender, EventArgs e)
        {
            if (produtoSelecionado == null)
            {
                await DisplayAlert("Aviso", "Selecione um produto para excluir.", "OK");
                return;
            }

            bool confirmar = await DisplayAlert("Confirmação",
                                               $"Deseja excluir o produto '{produtoSelecionado.Descricao}'?",
                                               "Sim", "Não");
            if (confirmar)
            {
                try
                {
                    await App.Db.Delete(produtoSelecionado.Id);
                    produtoSelecionado = null;
                    await CarregarProdutos(searchBar.Text);
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Erro", $"Erro ao excluir produto: {ex.Message}", "OK");
                }
            }
        }

        private async void MenuItem_Remover_Clicked(object sender, EventArgs e)
        {
            var menuItem = sender as MenuItem;
            var produto = menuItem?.BindingContext as Produto;

            if (produto == null)
                return;

            bool confirmar = await DisplayAlert("Confirmação",
                                               $"Deseja excluir o produto '{produto.Descricao}'?",
                                               "Sim", "Não");
            if (confirmar)
            {
                try
                {
                    await App.Db.Delete(produto.Id);
                    await CarregarProdutos(searchBar.Text);
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Erro", $"Erro ao remover produto: {ex.Message}", "OK");
                }
            }
        }
    }
}

