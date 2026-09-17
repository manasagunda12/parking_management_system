import { Component, OnInit, OnDestroy, PLATFORM_ID, inject } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, FormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { AuthService } from '../../Services/auth.service';
import { VehicleService } from '../../Services/vehicle.service';
import { ParkingLotService } from '../../Services/parking-lot.service';
import { ParkingSpaceService } from '../../Services/parking-space.service';
import { ParkingSessionService } from '../../Services/parking-session.service';
import { ReservationService } from '../../Services/reservation.service';
import { PayementService } from '../../Services/payement-service';
import { InvoiceService } from '../../Services/invoice.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css'
})
export class Dashboard implements OnInit, OnDestroy {

  activeSection = 'map';
  customerName = '';
  userId = 0;

  // Data
  vehicles: any[] = [];
  parkingLots: any[] = [];
  allSpaces: any[] = [];
  myReservations: any[] = [];
  myPayments: any[] = [];
  myInvoices: any[] = [];
  myInvoice: any = null;

  // Map state
  selectedLot: any = null;
  selectedFloor = 1;
  floors: number[] = [];
  floorSpaces: any[] = [];
  selectedSpace: any = null;
  selectedVehicleId: number | null = null;
  mapLoading = false;
  autoRefreshSub: Subscription | null = null;

  // Booking confirm modal
  showBookModal = false;
  bookingResult: any = null;

  // Reserve modal
  showReserveModal = false;
  reserveForm!: FormGroup;

  // Vehicle form
  showVehicleForm = false;
  editingVehicle: any = null;
  vehicleForm!: FormGroup;

  // Payment
  showPaymentForm = false;
  paymentForm!: FormGroup;

  // Invoice
  invoiceIdInput = '';

  isLoading = false;
  errorMessage = '';
  successMessage = '';

  private platformId = inject(PLATFORM_ID);

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private vehicleService: VehicleService,
    private parkingLotService: ParkingLotService,
    private parkingSpaceService: ParkingSpaceService,
    private parkingSessionService: ParkingSessionService,
    private reservationService: ReservationService,
    private paymentService: PayementService,
    private invoiceService: InvoiceService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.buildForms();
    this.loadUserInfo();
    this.loadVehicles();
    this.loadLots();
    this.loadMyReservations();
    this.loadMyPayments();
    this.loadMyInvoices();
    this.startAutoRefresh();
  }

  ngOnDestroy(): void {
    this.autoRefreshSub?.unsubscribe();
  }

  // ─── Auto Refresh every 15s ───────────────────────────

  startAutoRefresh(): void {
    // No auto-refresh needed — spaces reload after every action (book, reserve, cancel)
  }

  // ─── User Info ────────────────────────────────────────

  loadUserInfo(): void {
    if (!isPlatformBrowser(this.platformId)) return;
    const token = this.authService.getToken();
    if (!token) return;
    const payload = this.authService.decodeToken(token);
    this.customerName =
      payload?.['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'] ??
      payload?.name ?? 'Customer';
    this.userId = parseInt(
      payload?.['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'] ??
      payload?.sub ??
      payload?.nameid ??
      '0'
    );
  }

  buildForms(): void {
    this.vehicleForm = this.fb.group({
      vehicleNumber: ['', Validators.required],
      vehicleType: ['Car', Validators.required]
    });

    this.reserveForm = this.fb.group({
      vehicleId: ['', Validators.required],
      startTime: ['', Validators.required],
      endTime: ['', Validators.required],
      maxDate: ['']
    });

    this.paymentForm = this.fb.group({
      sessionId: ['', Validators.required],
      amount: ['', Validators.required],
      paymentMethod: ['Cash', Validators.required]
    });
  }

  setSection(section: string): void {
    this.activeSection = section;
    this.clearMessages();
    this.showVehicleForm = false;
    this.showPaymentForm = false;
    this.editingVehicle = null;
    if (section === 'vehicles') this.loadVehicles();
    if (section === 'reservations') this.loadMyReservations();
    if (section === 'payments') this.loadMyPayments();
    if (section === 'invoice') this.loadMyInvoices();
  }

  // ─── Parking Map ──────────────────────────────────────

  loadLots(): void {
    this.parkingLotService.getAll().subscribe({
      next: (data) => {
        this.parkingLots = data;
        if (data.length > 0) this.selectLot(data[0]);
      }
    });
  }

  selectLot(lot: any): void {
    this.selectedLot = lot;
    this.selectedSpace = null;
    this.selectedFloor = 1;
    const numFloors = lot.numberOfFloors ?? lot.NumberOfFloors ?? 1;
    this.floors = Array.from({ length: numFloors }, (_, i) => i + 1);
    this.loadFloorSpaces(lot.lotId ?? lot.LotId, 1);
  }

  selectFloor(floor: number): void {
    this.selectedFloor = floor;
    this.selectedSpace = null;
    this.loadFloorSpaces(this.selectedLot.lotId ?? this.selectedLot.LotId, floor);
  }

  loadFloorSpaces(lotId: number, floor: number): void {
    this.mapLoading = true;
    this.floorSpaces = [];
    this.allSpaces = [];
    this.parkingSpaceService.getByLotId(lotId).subscribe({
      next: (data) => {
        this.allSpaces = data;
        this.floorSpaces = data.filter(s => s.floorNumber === floor);
        this.mapLoading = false;
      },
      error: () => { this.mapLoading = false; }
    });
  }

  selectSpace(space: any): void {
    if (space.status !== 'Available') return;
    this.selectedSpace = this.selectedSpace?.spaceId === space.spaceId ? null : space;
  }

  getSpaceClass(space: any): string {
    if (space.status === 'Occupied') return 'space occupied';
    if (space.status === 'Reserved') return 'space reserved';
    if (this.selectedSpace?.spaceId === space.spaceId) return 'space selected';
    return 'space available';
  }

  getSpaceIcon(space: any): string {
    if (space.status === 'Occupied') return '🚗';
    if (space.status === 'Reserved') return '🔒';
    if (this.selectedSpace?.spaceId === space.spaceId) return '✅';
    return space.spaceType === 'Bike' ? '🏍️' : '🅿️';
  }

  get availableCount(): number {
    return this.floorSpaces.filter(s => s.status === 'Available').length;
  }

  get occupiedCount(): number {
    return this.floorSpaces.filter(s => s.status === 'Occupied').length;
  }

  get reservedCount(): number {
    return this.floorSpaces.filter(s => s.status === 'Reserved').length;
  }

  openBookModal(): void {
    if (!this.selectedSpace) return;
    if (this.vehicles.length === 0) {
      this.errorMessage = 'Please add a vehicle first before booking.';
      return;
    }
    this.selectedVehicleId = this.vehicles[0].vehicleId;
    this.showBookModal = true;
    this.clearMessages();
  }

  confirmBooking(): void {
    if (!this.selectedVehicleId || !this.selectedSpace) return;
    this.isLoading = true;
    this.parkingSessionService.bookSlot(this.selectedVehicleId, this.selectedSpace.spaceId).subscribe({
      next: (res) => {
        this.isLoading = false;
        this.bookingResult = res;
        this.showBookModal = false;
        this.successMessage = `✅ Slot ${this.selectedSpace.spaceName} booked! Session ID: ${res.sessionId}. Show this to the operator on arrival.`;
        this.selectedSpace = null;
        this.loadFloorSpaces(this.selectedLot.lotId, this.selectedFloor);
        this.loadLots();
      },
      error: (err) => {
        this.isLoading = false;
        const backendMessage = typeof err?.error === 'string'
          ? err.error
          : err?.error?.message;
        this.errorMessage = backendMessage || 'Booking failed';
        this.showBookModal = false;
      }
    });
  }

  openReserveModal(): void {
    if (!this.selectedSpace) return;
    if (this.vehicles.length === 0) {
      this.errorMessage = 'Please add a vehicle first before reserving.';
      return;
    }
    const now = new Date();
    const tomorrow = new Date(now.getTime() + 24 * 60 * 60 * 1000);
    const toLocal = (d: Date) => new Date(d.getTime() - d.getTimezoneOffset() * 60000).toISOString().slice(0, 16);
    this.reserveForm.reset({
      vehicleId: this.vehicles[0].vehicleId,
      startTime: toLocal(new Date(now.getTime() + 5 * 60 * 1000)),
      endTime: toLocal(new Date(now.getTime() + 65 * 60 * 1000)),
      maxDate: toLocal(tomorrow)
    });
    this.showReserveModal = true;
    this.clearMessages();
  }

  confirmReservation(): void {
    if (this.reserveForm.invalid) { this.reserveForm.markAllAsTouched(); return; }
    this.isLoading = true;
    const { vehicleId, startTime, endTime } = this.reserveForm.value;
    if (!this.selectedSpace) { this.errorMessage = 'No space selected for reservation.'; this.isLoading = false; return; }

    // Convert local datetime-local (YYYY-MM-DDTHH:mm) to full ISO string so backend parses reliably
    const toIso = (val: string) => {
      try { return new Date(val).toISOString(); } catch { return val; }
    };

    this.reservationService.create({
      vehicleId,
      spaceId: this.selectedSpace.spaceId,
      startTime: toIso(startTime),
      endTime: toIso(endTime)
    }).subscribe({
      next: (res) => {
        this.isLoading = false;
        this.showReserveModal = false;
        this.successMessage = `✅ Space ${this.selectedSpace.spaceName} reserved! Fee: ₹${res.reservationFee}`;
        this.selectedSpace = null;
        this.reserveForm.reset();
        this.loadFloorSpaces(this.selectedLot.lotId, this.selectedFloor);
        this.loadMyReservations();
      },
      error: (err) => {
        this.isLoading = false;
        const backendMessage = typeof err?.error === 'string'
          ? err.error
          : err?.error?.message;
        if (!backendMessage && err?.error?.errors) {
          this.errorMessage = JSON.stringify(err.error.errors);
        } else {
          this.errorMessage = backendMessage || 'Reservation failed';
        }
        this.showReserveModal = false;
      }
    });
  }

  // ─── Vehicles ─────────────────────────────────────────

  loadVehicles(): void {
    this.vehicleService.getAll().subscribe({
      next: (data) => this.vehicles = data.filter(v => v.userId === this.userId)
    });
  }

  openAddVehicle(): void {
    this.editingVehicle = null;
    this.vehicleForm.reset({ vehicleType: 'Car' });
    this.showVehicleForm = true;
  }

  openEditVehicle(v: any): void {
    this.editingVehicle = v;
    this.vehicleForm.patchValue({ vehicleNumber: v.vehicleNumber, vehicleType: v.vehicleType });
    this.showVehicleForm = true;
  }

  submitVehicle(): void {
    if (this.vehicleForm.invalid) { this.vehicleForm.markAllAsTouched(); return; }
    if (!this.userId) { this.errorMessage = 'User session expired. Please log in again.'; return; }
    this.isLoading = true;
    const request = this.editingVehicle
      ? this.vehicleService.update(this.editingVehicle.vehicleId, this.vehicleForm.value)
      : this.vehicleService.add({ ...this.vehicleForm.value, userId: this.userId });

    request.subscribe({
      next: () => {
        this.isLoading = false;
        this.successMessage = this.editingVehicle ? '✅ Vehicle updated successfully!' : '✅ Your vehicle has been added successfully!';
        this.showVehicleForm = false;
        this.editingVehicle = null;
        this.loadVehicles();
      },
      error: (err) => {
        this.isLoading = false;
        this.errorMessage = err?.error?.message || err?.error?.errors ? JSON.stringify(err?.error?.errors) : 'Failed to save vehicle';
      }
    });
  }

  deleteVehicle(id: number): void {
    if (!confirm('Delete this vehicle?')) return;
    this.vehicleService.delete(id).subscribe({
      next: () => { this.successMessage = 'Vehicle deleted'; this.loadVehicles(); },
      error: () => this.errorMessage = 'Failed to delete vehicle'
    });
  }

  // ─── Reservations ─────────────────────────────────────

  loadMyReservations(): void {
    this.reservationService.getMyReservations().subscribe({
      next: (data) => this.myReservations = data,
      error: () => this.errorMessage = 'Failed to load reservations'
    });
  }

  cancelReservation(id: number): void {
    if (!confirm('Cancel this reservation?')) return;
    this.reservationService.cancel(id).subscribe({
      next: () => { this.successMessage = 'Reservation cancelled'; this.loadMyReservations(); },
      error: (err) => this.errorMessage = err?.error?.message || 'Failed to cancel'
    });
  }

  // ─── Payments ─────────────────────────────────────────

  loadMyPayments(): void {
    this.paymentService.getMyPayments().subscribe({
      next: (data) => this.myPayments = data
    });
  }

  openCreatePayment(sessionId?: number, amount?: number): void {
    this.showPaymentForm = true;
    this.paymentForm.reset({ paymentMethod: 'Cash' });
    if (sessionId) this.paymentForm.patchValue({ sessionId, amount });
  }

  submitCreatePayment(): void {
    if (this.paymentForm.invalid) { this.paymentForm.markAllAsTouched(); return; }
    this.isLoading = true;
    this.paymentService.create(this.paymentForm.value).subscribe({
      next: (res) => {
        this.isLoading = false;
        this.successMessage = `✅ Payment created! ID: ${res.paymentId}. Click Pay Now to complete.`;
        this.showPaymentForm = false;
        this.paymentForm.reset({ paymentMethod: 'Cash' });
        this.loadMyPayments();
      },
      error: (err) => {
        this.isLoading = false;
        this.errorMessage = err?.error?.message || 'Failed to create payment';
      }
    });
  }

  payNow(paymentId: number, method: string): void {
    this.paymentService.pay(paymentId, method || 'Cash').subscribe({
      next: () => {
        this.successMessage = '✅ Payment successful! Check Invoice section for your receipt.';
        this.loadMyPayments();
        this.loadMyInvoices();
      },
      error: (err) => this.errorMessage = err?.error?.message || 'Payment failed'
    });
  }

  // ─── Invoice ──────────────────────────────────────────

  loadMyInvoices(): void {
    this.invoiceService.getMyInvoices().subscribe({
      next: (data) => this.myInvoices = data,
      error: () => this.errorMessage = 'Failed to load invoices'
    });
  }

  loadInvoice(id: number): void {
    if (!id) return;
    this.invoiceService.getById(id).subscribe({
      next: (data) => { this.myInvoice = data; this.clearMessages(); },
      error: () => this.errorMessage = 'Invoice not found'
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
