import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PayementService } from '../../Services/payement-service';
import { InvoiceService } from '../../Services/invoice.service';

@Component({
  selector: 'app-payment-component',
  imports: [CommonModule, FormsModule],
  templateUrl: './payment-component.html',
  styleUrl: './payment-component.css',
})
export class PaymentComponent implements OnInit {
  payments: any[] = [];
  selectedPaymentMethod: { [key: number]: string } = {};
  invoice: any = null;
  errorMessage = '';
  successMessage = '';
  loading = false;

  constructor(
    private paymentService: PayementService,
    private invoiceService: InvoiceService
  ) {}

  ngOnInit(): void {
    this.loadMyPayments();
  }

  loadMyPayments(): void {
    this.paymentService.getMyPayments().subscribe({
      next: (data) => (this.payments = data),
      error: () => (this.errorMessage = 'Failed to load payments.'),
    });
  }

  makePayment(paymentId: number): void {
    const method = this.selectedPaymentMethod[paymentId];
    if (!method) {
      this.errorMessage = 'Please select a payment method.';
      return;
    }

    this.errorMessage = '';
    this.successMessage = '';
    this.invoice = null;
    this.loading = true;

    this.paymentService.pay(paymentId, method).subscribe({
      next: () => {
        this.successMessage = 'Payment successful! Fetching your invoice...';
        this.loadMyPayments();
        setTimeout(() => this.fetchInvoice(paymentId), 1000);
      },
      error: (err) => {
        this.loading = false;
        this.errorMessage = err?.error?.message || 'Payment failed.';
      },
    });
  }

  fetchInvoice(paymentId: number): void {
    this.invoiceService.getMyInvoices().subscribe({
      next: (invoices) => {
        this.loading = false;
        this.invoice = invoices.find((inv) => inv.paymentId === paymentId) || null;
      },
      error: () => {
        this.loading = false;
        this.errorMessage = 'Payment done but failed to load invoice.';
      },
    });
  }
}
