using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace winforms_client
{
    public partial class MainForm : Form
    {
        private readonly HttpClient _httpClient;

        public MainForm()
        {
            InitializeComponent();
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("http://127.0.0.1:5000/");  
        }

       
        public class Product
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public double Price { get; set; }
            public string Description { get; set; }
            public string ImageUrl { get; set; }
        }

        
        protected override async void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            await LoadProducts();  
        }

       
        private async Task LoadProducts()
        {
            try
            {
                var products = await _httpClient.GetFromJsonAsync<List<Product>>("products");
                dataGridViewProducts.DataSource = products;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas ładowania produktów: {ex.Message}");
            }
        }

        
        private async void buttonAdd_Click(object sender, EventArgs e)
        {
           
            if (string.IsNullOrWhiteSpace(textBoxName.Text) ||
                string.IsNullOrWhiteSpace(textBoxPrice.Text) ||
                string.IsNullOrWhiteSpace(textBoxDescription.Text)) //||
              //  string.IsNullOrWhiteSpace(textBoxImageUrl.Text))
            {
                
                MessageBox.Show("Wszystkie pola muszą być wypełnione!", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            
            if (!double.TryParse(textBoxPrice.Text, out double price))
            {
                MessageBox.Show("Cena musi być poprawną liczbą!", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            
            if (!IsValidUrl(textBoxImageUrl.Text) && textBoxImageUrl.Text.ToLower() != "brak")
            {
                MessageBox.Show("Pole adresu URL musi zawierać poprawny adres URL lub słowo 'brak'.", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

           
            var newProduct = new Product
            {
                Name = textBoxName.Text,
                Price = price,
                Description = textBoxDescription.Text,
                ImageUrl = textBoxImageUrl.Text
            };

            
            var response = await _httpClient.PostAsJsonAsync("products", newProduct);
            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Produkt dodany!");
               
                ClearTextBoxes();
                await LoadProducts();  
            }
            else
            {
                MessageBox.Show("Błąd dodawania produktu");
            }
        }

       
        private void ClearTextBoxes()
        {
            textBoxName.Text = string.Empty;
            textBoxPrice.Text = string.Empty;
            textBoxDescription.Text = string.Empty;
            textBoxImageUrl.Text = string.Empty;
        }

   
        private bool IsValidUrl(string url)
        {
            Uri uriResult;
            return Uri.TryCreate(url, UriKind.Absolute, out uriResult) &&
                   (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }


     
        private async void buttonUpdate_Click(object sender, EventArgs e)
        {
            if (dataGridViewProducts.SelectedRows.Count > 0)
            {
                var selectedProduct = (Product)dataGridViewProducts.SelectedRows[0].DataBoundItem;
                selectedProduct.Name = textBoxName.Text;
                selectedProduct.Price = double.Parse(textBoxPrice.Text);
                selectedProduct.Description = textBoxDescription.Text;
                selectedProduct.ImageUrl = textBoxImageUrl.Text;

                var response = await _httpClient.PutAsJsonAsync($"products/{selectedProduct.Id}", selectedProduct);
                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Produkt zaktualizowany!");
                    
                    ClearTextBoxes();
                    await LoadProducts();  
                }
                else
                {
                    MessageBox.Show("Błąd aktualizacji produktu");
                }
            }
        }

        
        private async void buttonDelete_Click(object sender, EventArgs e)
        {
            if (dataGridViewProducts.SelectedRows.Count > 0)
            {
                var selectedProduct = (Product)dataGridViewProducts.SelectedRows[0].DataBoundItem;

                var response = await _httpClient.DeleteAsync($"products/{selectedProduct.Id}");
                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Produkt usunięty!");
                    await LoadProducts();  
                }
                else
                {
                    MessageBox.Show("Błąd usuwania produktu");
                }
            }
        }

        
        private void dataGridViewProducts_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewProducts.SelectedRows.Count > 0)
            {
                var selectedProduct = (Product)dataGridViewProducts.SelectedRows[0].DataBoundItem;
                textBoxName.Text = selectedProduct.Name;
                textBoxPrice.Text = selectedProduct.Price.ToString();
                textBoxDescription.Text = selectedProduct.Description;
                textBoxImageUrl.Text = selectedProduct.ImageUrl;
            }
        }

        private async void buttonRefresh_Click(object sender, EventArgs e)
        {
            await LoadProducts();
        }
    }
}
