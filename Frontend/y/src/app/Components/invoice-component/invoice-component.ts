import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { InvoiceService } from '../../Services/invoice.service';
import { AuthService } from '../../Services/auth.service';

@Component({
  selector: 'app-invoice-component',
  imports: [CommonModule, FormsModule],
  templateUrl: './invoice-component.html',
  styleUrl: './invoice-component.css',
})
export class InvoiceComponent implements OnInit {
  invoice: any = null;
  myInvoices: any[] = [];
  allInvoices: any[] = [];
  paymentId: number | null = null;
  errorMessage = '';
  successMessage = '';
  role: string | null = null;

  constructor(private invoiceService: InvoiceService, private authService: AuthService) {}

  ngOnInit(): void {
    this.role = this.authService.getRole();
    if (this.role === 'Customer') {
      this.loadMyInvoices();
    } else if (this.role === 'Admin') {
      this.loadAllInvoices();
    }
  }

  loadMyInvoices(): void {
    this.invoiceService.getMyInvoices().subscribe({
      next: (data) => (this.myInvoices = data),
      error: () => (this.errorMessage = 'Failed to load invoices.'),
    });
  }

  loadAllInvoices(): void {
    this.invoiceService.getAll().subscribe({
      next: (data) => (this.allInvoices = data),
      error: () => (this.errorMessage = 'Failed to load invoices.'),
    });
  }

  generateInvoice(): void {
    if (!this.paymentId) return;
    this.errorMessage = '';
    this.successMessage = '';
    this.invoice = null;

    this.invoiceService.generate(this.paymentId).subscribe({
      next: (data) => {
        this.invoice = data;
        this.successMessage = 'Invoice generated successfully!';
      },
      error: (err) => {
        this.errorMessage = err?.error?.message || 'Failed to generate invoice.';
      },
    });
  }
}
