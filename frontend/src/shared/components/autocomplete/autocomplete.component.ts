import { Component, OnInit, input, output, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';

@Component({
    selector: 'app-autocomplete-transversal',
    standalone: true,
    imports: [CommonModule],
    templateUrl: './autocomplete.component.ts.html',
    styleUrls: ['./autocomplete.component.ts.css']
})
export class AutocompleteTransversalComponent implements OnInit {

    // 📥 Parámetros de entrada (Inputs Modernos con Signal Inputs)
    placeholder = input<string>('🔍 Buscar...');
    label = input<string>('Buscar registro');
    // Nos permite saber qué propiedad del objeto mostrar en la lista (ej: 'placa' o 'nombre')
    campoMostrar = input.required<string>();
    // Nos ayuda a mostrar un subtexto secundario opcional (ej: 'marca' o 'cedula')
    subCampoMostrar = input<string>('');
    subCampoMostrarDos = input<string>('');

    labelMostrar = input.required<string>();
    subLabelMostrar = input<string>('');
    subLabelMostrarDos = input<string>('');

    // 📤 Eventos de salida (Outputs con la nueva sintaxis Angular 17+)
    alEscribir = output<string>();
    alSeleccionar = output<any>();

    // 📦 Estados locales de control con Signals
    busqueda = signal<string>('');
    mostrarSugerencias = signal<boolean>(false);

    // Lista de opciones que el componente padre nos inyectará dinámicamente desde el backend
    opciones = input<any[]>([]);

    // Control de ráfagas de teclado
    private deboncer = new Subject<string>();

    ngOnInit(): void {
        this.deboncer.pipe(
            debounceTime(350),         // Espera 350ms a que el operador deje de escribir
            distinctUntilChanged()     // Solo emite si el texto cambió
        ).subscribe(termino => {
            this.alEscribir.emit(termino);
        });
    }

    public onInput(event: Event): void {

        const valor = (event.target as HTMLInputElement).value;
        this.busqueda.set(valor);
        this.mostrarSugerencias.set(valor.trim().length > 0);
        this.deboncer.next(valor);
    }

    public seleccionarOpcion(opcion: any): void {
        // Seteamos el texto del input con el valor seleccionado
        this.busqueda.set(opcion[this.campoMostrar()] + ' - ' + opcion[this.subCampoMostrar()]);
        this.mostrarSugerencias.set(false);

        // Devolvemos el objeto completo (con su ID de la base de datos) al padre
        this.alSeleccionar.emit(opcion);
    }

    public limpiar(): void {
        this.busqueda.set('');
        this.mostrarSugerencias.set(false);
        this.alSeleccionar.emit(null);
    }

    // Cierra el panel de sugerencias si el usuario da clic afuera (Blur diferido)
    public onBlur(): void {
        setTimeout(() => this.mostrarSugerencias.set(false), 200);
    }
}