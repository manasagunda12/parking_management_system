import { Component, OnInit, OnDestroy, PLATFORM_ID, inject } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { interval, Subscription } from 'rxjs';
import { AuthService } from '../../Services/auth.service';
import { ParkingLotService } from '../../Services/parking-lot.service';
import { ParkingSpaceService } from '../../Services/parking-space.service';
import { ParkingSessionService } from '../../Services/parking-session.service';
import { ReservationService } from '../../Services/reservation.service';
import { InvoiceService } from '../../Services/invoice.service';

@Component({
  selector: 'app-operator-component',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './operator-component.html',
  styleUrl: './operator-component.css'
})
export class OperatorComponent implements OnInit, OnDestroy {

  activeSection = 'map';
  operatorName = '';

  // Data
  parkingLots: any[] = [];
  allSpaces: any[] = [];
  floorSpaces: any[] = [];
  sessions: any[] = [];
  reservations: any[] = [];

  // Map state
  selectedLot: any = null;
  selectedFloor = 1;
  floors: number[] = [];
  mapLoading = false;
  autoRefreshSub: Subscription | null = null;

  // Session action state
  sessionIdInput = '';
  exitResult: any = null;
  showExitModal = false;

  // Invoice
  invoicePaymentId = '';
  generatedInvoice: any = null;

  isLoading = false;
  errorMessage = '';
  successMessage = '';

  private platformId = inject(PLATFORM_ID);

  constructor(
    private authService: AuthService,
    private parkingLotService: ParkingLotService,
    private parkingSpaceService: ParkingSpaceService,
    private parkingSessionService: ParkingSessionService,
    private reservationService: ReservationService,
    private invoiceService: InvoiceService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadOperatorInfo();
    this.loadLots();
    this.loadSessions();
    this.loadReservations();
    this.startAutoRefresh();
  }

  ngOnDestroy(): void {
    this.autoRefreshSub?.unsubscribe();
  }

  startAutoRefresh(): void {
    // Sessions refresh after every operator action (occupy, exit)
    // No polling needed for the map
    this.autoRefreshSub = interval(15000).subscribe(() => {
      if (this.activeSection === 'sessions') this.loadSessions();
    });
  }

  loadOperatorInfo(): void {
    if (!isPlatformBrowser(this.platformId)) return;
    const token = this.authService.getToken();
    if (!token) return;
    const payload = this.authService.decodeToken(token);
    this.operatorName =
      payload?.['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'] ??
      payload?.name ?? 'Operator';
  }

  setSection(section: string): void {
    this.activeSection = section;
    this.clearMessages();
    this.exitResult = null;
    this.generatedInvoice = null;
    if (section === 'sessions') this.loadSessions();
    if (section === 'reservations') this.loadReservations();
    if (section === 'map') { this.loadLots(); }
  }

  // ─── Parking Map ──────────────────────────────────────

  loadLots(): void {
    this.parkingLotService.getAll().subscribe({
      next: (data) => {
        this.parkingLots = data;
        if (data.length > 0 && !this.selectedLot) this.selectLot(data[0]);
        else if (this.selectedLot) {
          const updated = data.find(l => l.lotId === this.selectedLot.lotId);
          if (updated) this.selectedLot = updated;
        }
      }
    });
  }

  selectLot(lot: any): void {
    this.selectedLot = lot;
    this.selectedFloor = 1;
    this.floors = Array.from({ length: lot.numberOfFloors }, (_, i) => i + 1);
    this.loadFloorSpaces(lot.lotId, 1);
  }

  selectFloor(floor: number): void {
    this.selectedFloor = floor;
    this.loadFloorSpaces(this.selectedLot.lotId, floor);
  }

  loadFloorSpaces(lotId: number, floor: number): void {
    this.mapLoading = true;
    this.parkingSpaceService.getByLotId(lotId).subscribe({
      next: (data) => {
        this.allSpaces = data;
        this.floorSpaces = data.filter(s => s.floorNumber === floor);
        this.mapLoading = false;
      },
      error: () => { this.mapLoading = false; }
    });
  }

  getSpaceClass(space: any): string {
    if (space.status === 'Occupied') return 'space occupied';
    if (space.status === 'Reserved') return 'space reserved';
    return 'space available';
  }

  getSpaceIcon(space: any): string {
    if (space.status === 'Occupied') return '🚗';
    if (space.status === 'Reserved') return '🔒';
    return space.spaceType === 'Bike' ? '🏍️' : '🅿️';
  }

  get availableCount(): number { return this.floorSpaces.filter(s => s.status === 'Available').length; }
  get occupiedCount(): number { return this.floorSpaces.filter(s => s.status === 'Occupied').length; }
  get reservedCount(): number { return this.floorSpaces.filter(s => s.status === 'Reserved').length; }

  updateSpaceStatus(spaceId: number, status: string): void {
    if (!status) return;
    this.parkingSpaceService.updateStatus(spaceId, status).subscribe({
      next: () => {
        this.successMessage = `Space status updated to ${status}`;
        this.loadFloorSpaces(this.selectedLot.lotId, this.selectedFloor);
        this.loadLots();
      },
      error: (err) => this.errorMessage = err?.error?.message || 'Failed to update status'
    });
  }

  // ─── Sessions ─────────────────────────────────────────

  loadSessions(): void {
    this.parkingSessionService.getAll().subscribe({
      next: (data) => this.sessions = data,
      error: () => this.errorMessage = 'Failed to load sessions'
    });
  }

  occupySlot(sessionId: number): void {
    if (!confirm(`Mark Session #${sessionId} as Occupied?`)) return;
    this.isLoading = true;
    this.parkingSessionService.occupySlot(sessionId).subscribe({
      next: () => {
        this.isLoading = false;
        this.successMessage = `Session #${sessionId} marked as occupied`;
        this.loadSessions();
        if (this.selectedLot) this.loadFloorSpaces(this.selectedLot.lotId, this.selectedFloor);
      },
      error: (err) => {
        this.isLoading = false;
        this.errorMessage = err?.error?.message || 'Failed to occupy slot';
      }
    });
  }

  exitVehicle(sessionId: number): void {
    if (!confirm(`Process exit for Session #${sessionId}?`)) return;
    this.isLoading = true;
    this.parkingSessionService.exitVehicle(sessionId).subscribe({
      next: (res) => {
        this.isLoading = false;
        this.exitResult = res;
        this.showExitModal = true;
        this.loadSessions();
        if (this.selectedLot) this.loadFloorSpaces(this.selectedLot.lotId, this.selectedFloor);
        this.loadLots();
      },
      error: (err) => {
        this.isLoading = false;
        this.errorMessage = err?.error?.message || 'Failed to process exit';
      }
    });
  }

  // ─── Reservations ─────────────────────────────────────

  loadReservations(): void {
    this.reservationService.getAll().subscribe({
      next: (data) => this.reservations = data,
      error: () => this.errorMessage = 'Failed to load reservations'
    });
  }

  cancelReservation(id: number): void {
    if (!confirm('Cancel this reservation?')) return;
    this.reservationService.cancel(id).subscribe({
      next: () => { this.successMessage = 'Reservation cancelled'; this.loadReservations(); },
      error: (err) => this.errorMessage = err?.error?.message || 'Failed to cancel'
    });
  }

  checkInReservation(id: number): void {
    if (!confirm(`Check in customer for Reservation #${id}?`)) return;
    this.isLoading = true;
    this.reservationService.checkIn(id).subscribe({
      next: (res) => {
        this.isLoading = false;
        this.successMessage = `✅ Check-in successful! Session ID: ${res.sessionId}. Vehicle is now Occupied.`;
        this.loadReservations();
        this.loadSessions();
        if (this.selectedLot) this.loadFloorSpaces(this.selectedLot.lotId, this.selectedFloor);
      },
      error: (err) => {
        this.isLoading = false;
        this.errorMessage = err?.error?.message || 'Check-in failed';
      }
    });
  }

  // ─── Invoice ──────────────────────────────────────────

  generateInvoice(): void {
    const id = parseInt(this.invoicePaymentId);
    if (!id) { this.errorMessage = 'Enter a valid Payment ID'; return; }
    this.isLoading = true;
    this.invoiceService.generate(id).subscribe({
      next: (res) => {
        this.isLoading = false;
        this.generatedInvoice = res;
        this.successMessage = 'Invoice generated successfully';
      },
      error: (err) => {
        this.isLoading = false;
        this.errorMessage = err?.error?.message || 'Failed to generate invoice';
      }
    });
  }

  // ─── Helpers ──────────────────────────────────────────

  getSessionStatus(s: any): string {
    return s.status ?? (s.exitTime ? 'Completed' : s.entryTime ? 'Occupied' : 'Reserved');
  }

  getSessionStatusClass(s: any): string {
    const status = this.getSessionStatus(s);
    return status.toLowerCase();
  }

  clearMessages(): void {
    this.errorMessage = '';
    this.successMessage = '';
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/']);
  }
}
