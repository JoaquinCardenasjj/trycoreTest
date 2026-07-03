import { Component, inject, signal } from '@angular/core';



@Component({
    selector: 'app-dashboard',
    imports: [],
    templateUrl: './dashboard.component.html',
    styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent {


    // Kpis de la trycoretest usando Signals
    public readonly totalVehiculos = signal<number>(45);
    public readonly vehiculosOperativos = signal<number>(38);
    public readonly vehiculosAlerta = signal<number>(5);
    public readonly vehiculosBloqueados = signal<number>(2);



    // Historial rápido de acciones para auditoría inalterable
    public readonly logsRecientes = signal([
        { hora: '10:30 AM', usuario: 'Carlos Gómez', accion: 'Subió RTM aprobada', placa: 'SXZ-456' },
        { hora: '09:15 AM', usuario: 'Juan Taller', accion: 'Creó Orden de Trabajo #402', placa: 'TLK-789' },
        { hora: '06:00 AM', usuario: 'Sistema Despacho', accion: 'Bloqueo automático de despacho', placa: 'SXZ-456' }
    ]);
}