import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';

@Component({
  selector: 'app-data',
  templateUrl: './data.component.html',
})
export class DataComponent {
  data: any[] = [];

  constructor(private http: HttpClient) {
    this.getData();
  }

  getData() {
    this.http.get<any[]>('http://127.0.0.1:5000/data')
      .subscribe(response => this.data = response);
  }
}
