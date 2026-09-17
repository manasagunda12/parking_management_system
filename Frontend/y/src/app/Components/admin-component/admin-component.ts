import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../Services/auth.service';
import { UserService } from '../../Services/user.service';
import { ParkingLotService } from '../../Services/parking-lot.service';
import { ParkingSpaceService } from '../../Services/parking-space.service';
import { ParkingSessionService } from '../../Services/parking-session.service';
import { ReservationService } from '../../Services/reservation.service';
import { PayementService } from '../../Services/payement-service';
import { InvoiceService } from '../../Services/invoice.service';
import { VehicleService } from '../../Services/vehicle.service';

@Component({
  selector: 'app-admin-component',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './admin-component.html',
  styleUrl: './admin-component.css'
})
export class AdminComponent implements OnInit {

  activeSection = 'dashboard';

  // Data lists
  users: any[] = [];
  parkingLots: any[] = [];
  parkingSpaces: any[] = [];
  sessions: any[] = [];
  reservations: any[] = [];
  payments: any[] = [];
  invoices: any[] = [];
  vehicles: any[] = [];

  // Summary counts
  totalUsers = 0;
  totalLots = 0;
  totalSessions = 0;
  totalPayments = 0;
  totalReservations = 0;
  totalVehicles = 0;

  // UI state
  showOperatorForm = false;
  showLotForm = false;
  showSpaceForm = false;
  editingLot: any = null;
  selectedLotId: number | null = null;
  isLoading = false;
  errorMessage = '';
  successMessage = '';

  // Forms
  operatorForm!: FormGroup;
  lotForm!: FormGroup;
  spaceForm!: FormGroup;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private userService: UserService,
    private parkingLotService: ParkingLotService,
    private parkingSpaceService: ParkingSpaceService,
    private parkingSessionService: ParkingSessionService,
    private reservationService: ReservationService,
    private paymentService: PayementService,
    private invoiceService: InvoiceService,
    private vehicleService: VehicleService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.buildForms();
    this.loadDashboard();
  }

  buildForms(): void {
    this.operatorForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(2)]],
      email: ['', [Validators.required, Validators.email]],
      phoneNumber: ['', [Validators.required, Validators.pattern(/^[0-9]{10,15}$/)]],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });

    this.lotForm = this.fb.group({
      name: ['', Validators.required],
      location: ['', Validators.required],
      totalSlots: ['', [Validators.required, Validators.min(1)]],
      numberOfFloors: ['', [Validators.required, Validators.min(1)]]
    });

    this.spaceForm = this.fb.group({
      lotId: ['', Validators.required],
      totalSlots: ['', [Validators.required, Validators.min(1)]],
      numberOfFloors: ['', [Validators.required, Validators.min(1)]]
    });
  }

  loadDashboard(): void {
    this.userService.getAllUsers().subscribe({ next: (d) => { this.users = d; this.totalUsers = d.length; } });
    this.parkingLotService.getAll().subscribe({ next: (d) => { this.parkingLots = d; this.totalLots = d.length; } });
    this.parkingSessionService.getAll().subscribe({ next: (d) => { this.sessions = d; this.totalSessions = d.length; } });
    this.paymentService.getAll().subscribe({ next: (d) => { this.payments = d; this.totalPayments = d.length; } });
    this.reservationService.getAll().subscribe({ next: (d) => { this.reservations = d; this.totalReservations = d.length; } });
    this.vehicleService.getAll().subscribe({ next: (d) => { this.vehicles = d; this.totalVehicles = d.length; } });
  }

  setSection(section: string): void {
    this.activeSection = section;
    this.clearMessages();
    this.showOperatorForm = false;
    this.showLotForm = false;
    this.showSpaceForm = false;
    this.editingLot = null;

    if (section === 'users') this.loadUsers();
    if (section === 'lots') this.loadLots();
    if (section === 'spaces') this.loadLots();
    if (section === 'sessions') this.loadSessions();
    if (section === 'reservations') this.loadReservations();
    if (section === 'payments') this.loadPayments();
    if (section === 'invoices') this.loadInvoices();
    if (section === 'vehicles') this.loadVehicles();
  }

  // ─── Users ───────────────────────────────────────────

  loadUsers(): void {
    this.userService.getAllUsers().subscribe({
      next: (d) => this.users = d,
      error: () => this.errorMessage = 'Failed to load users'
    });
  }

  submitOperator(): void {
    if (this.operatorForm.invalid) { this.operatorForm.markAllAsTouched(); return; }
    this.isLoading = true;
    this.userService.createOperator(this.operatorForm.value).subscribe({
      next: () => {
        this.isLoading = false;
        this.successMessage = 'Operator created successfully';
        this.showOperatorForm = false;
        this.operatorForm.reset();
        this.loadUsers();
      },
      error: (err) => {
        this.isLoading = false;
        this.errorMessage = err?.error?.message || 'Failed to create operator';
      }
    });
  }

  // ─── Parking Lots ─────────────────────────────────────

  loadLots(): void {
    this.parkingLotService.getAll().subscribe({
      next: (d) => this.parkingLots = d,
      error: () => this.errorMessage = 'Failed to load parking lots'
    });
  }

  openCreateLot(): void {
    this.editingLot = null;
    this.lotForm.reset();
    this.showLotForm = true;
  }

  openEditLot(lot: any): void {
    this.editingLot = lot;
    this.lotForm.patchValue({ name: lot.name, location: lot.location, totalSlots: lot.totalSlots, numberOfFloors: lot.numberOfFloors });
    this.showLotForm = true;
  }

  submitLot(): void {
    if (this.lotForm.invalid) { this.lotForm.markAllAsTouched(); return; }
    this.isLoading = true;
    const request = this.editingLot
      ? this.parkingLotService.update(this.editingLot.lotId, this.lotForm.value)
      : this.parkingLotService.create(this.lotForm.value);

    request.subscribe({
      next: () => {
        this.isLoading = false;
        this.successMessage = this.editingLot ? 'Parking lot updated' : 'Parking lot created';
        this.showLotForm = false;
        this.editingLot = null;
        this.lotForm.reset();
        this.loadLots();
      },
      error: (err) => {
        this.isLoading = false;
        this.errorMessage = err?.error?.message || 'Failed to save parking lot';
      }
    });
  }

  deleteLot(id: number): void {
    if (!confirm('Delete this parking lot?')) return;
    this.parkingLotService.delete(id).subscribe({
      next: () => { this.successMessage = 'Parking lot deleted'; this.loadLots(); },
      error: () => this.errorMessage = 'Failed to delete parking lot'
    });
  }

  // ─── Parking Spaces ───────────────────────────────────

  loadSpacesByLot(lotId: number): void {
    this.selectedLotId = lotId;
    this.parkingSpaceService.getByLotId(lotId).subscribe({
      next: (d) => this.parkingSpaces = d,
      error: () => this.errorMessage = 'Failed to load spaces'
    });
  }

  submitSpaces(): void {
    if (this.spaceForm.invalid) { this.spaceForm.markAllAsTouched(); return; }
    const { lotId, totalSlots, numberOfFloors } = this.spaceForm.value;
    this.isLoading = true;
    this.parkingSpaceService.createSpaces(lotId, totalSlots, numberOfFloors).subscribe({
      next: () => {
        this.isLoading = false;
        this.successMessage = 'Parking spaces created successfully';
        this.showSpaceForm = false;
        this.spaceForm.reset();
        this.loadSpacesByLot(lotId);
      },
      error: (err) => {
        this.isLoading = false;
        this.errorMessage = err?.error?.message || 'Failed to create spaces';
      }
    });
  }

  updateSpaceStatus(spaceId: number, status: string): void {
    if (!status) return;
    this.parkingSpaceService.updateStatus(spaceId, status).subscribe({
      next: () => {
        this.successMessage = 'Space status updated';
        if (this.selectedLotId) this.loadSpacesByLot(this.selectedLotId);
      },
      error: () => this.errorMessage = 'Failed to update status'
    });
  }

  // ─── Sessions ─────────────────────────────────────────

  loadSessions(): void {
    this.parkingSessionService.getAll().subscribe({
      next: (d) => this.sessions = d,
      error: () => this.errorMessage = 'Failed to load sessions'
    });
  }

  // ─── Reservations ─────────────────────────────────────

  loadReservations(): void {
    this.reservationService.getAll().subscribe({
      next: (d) => this.reservations = d,
      error: () => this.errorMessage = 'Failed to load reservations'
    });
  }

  cancelReservation(id: number): void {
    if (!confirm('Cancel this reservation?')) return;
    this.reservationService.cancel(id).subscribe({
      next: () => { this.successMessage = 'Reservation cancelled'; this.loadReservations(); },
      error: (err) => this.errorMessage = err?.error?.message || 'Failed to cancel reservation'
    });
  }

  // ─── Payments ─────────────────────────────────────────

  loadPayments(): void {
    this.paymentService.getAll().subscribe({
      next: (d) => this.payments = d,
      error: () => this.errorMessage = 'Failed to load payments'
    });
  }

  // ─── Invoices ─────────────────────────────────────────

  selectedInvoice: any = null;

  loadInvoices(): void {
    this.invoiceService.getAll().subscribe({
      next: (d) => this.invoices = d,
      error: () => this.errorMessage = 'Failed to load invoices'
    });
  }

  viewInvoice(id: number): void {
    this.invoiceService.getById(id).subscribe({
      next: (data) => this.selectedInvoice = data,
      error: () => this.errorMessage = 'Failed to load invoice details'
    });
  }

  closeInvoice(): void {
    this.selectedInvoice = null;
  }

  generateInvoice(paymentId: number): void {
    this.invoiceService.generate(paymentId).subscribe({
      next: () => { this.successMessage = 'Invoice generated successfully'; this.loadInvoices(); },
      error: (err) => this.errorMessage = err?.error?.message || 'Failed to generate invoice'
    });
  }

  // ─── Vehicles ─────────────────────────────────────────

  loadVehicles(): void {
    this.vehicleService.getAll().subscribe({
      next: (d) => this.vehicles = d,
      error: () => this.errorMessage = 'Failed to load vehicles'
    });
  }

  deleteVehicle(id: number): void {
    if (!confirm('Delete this vehicle?')) return;
    this.vehicleService.delete(id).subscribe({
      next: () => { this.successMessage = 'Vehicle deleted'; this.loadVehicles(); },
      error: () => this.errorMessage = 'Failed to delete vehicle'
    });
  }

  // ─── Helpers ──────────────────────────────────────────

  clearMessages(): void {
    this.errorMessage = '';
    this.successMessage = '';
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/']);
  }
}
