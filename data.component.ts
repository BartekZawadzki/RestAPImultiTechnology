import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Product } from '../models/product.model';

@Component({
  selector: 'app-data',
  templateUrl: './data.component.html',
  styleUrls: ['./data.component.css']
})
export class DataComponent implements OnInit {
  products: Product[] = [];
  
  // Zmienne dla nowego produktu
  newName: string = '';
  newPrice: number = 0;
  newDescription: string = '';
  newImageUrl: string = '';

  constructor(private http: HttpClient) { }

  ngOnInit(): void {
    this.getProducts();
  }

  getProducts(): void {
    this.http.get<Product[]>('http://127.0.0.1:5000/products')
      .subscribe(response => {
        this.products = response;
      });
  }

  addProduct(): void {
    const newProduct: Product = {
      id: 0,  // id zostanie nadane przez backend
      name: this.newName,
      price: this.newPrice,
      description: this.newDescription,
      image_url: this.newImageUrl
    };

    this.http.post<Product>('http://127.0.0.1:5000/products', newProduct)
      .subscribe(newProduct => {
        this.products.push(newProduct);
        // Resetowanie wartości po dodaniu produktu
        this.newName = '';
        this.newPrice = 0;
        this.newDescription = '';
        this.newImageUrl = '';
      });
  }

  updateProduct(id: number, updatedProduct: Partial<Product>): void {
    this.http.put<Product>(`http://127.0.0.1:5000/products/${id}`, updatedProduct)
      .subscribe(product => {
        const index = this.products.findIndex(p => p.id === id);
        if (index !== -1) {
          this.products[index] = product;
        }
      });
  }

  deleteProduct(id: number): void {
    this.http.delete(`http://127.0.0.1:5000/products/${id}`)
      .subscribe(() => {
        this.products = this.products.filter(p => p.id !== id);
      });
  }
}
