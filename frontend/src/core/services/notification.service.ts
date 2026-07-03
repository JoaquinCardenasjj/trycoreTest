import { Injectable, signal } from '@angular/core';

export interface ToastConfig {
    mensaje: string;
    tipo: 'success' | 'error' | 'warning';
    visible: boolean;
}

@Injectable({
    providedIn: 'root'
})
export class NotificationService {
    // Signal reactivo para manejar el estado del Toast global
    public readonly toast = signal<ToastConfig>({
        mensaje: '',
        tipo: 'success',
        visible: false
    });

    mostrar(mensaje: string, tipo: 'success' | 'error' | 'warning' = 'success', duracion: number = 4000) {
        this.toast.set({ mensaje, tipo, visible: true });

        // Auto-ocultar la alerta después del tiempo definido
        setTimeout(() => {
            this.toast.update(state => ({ ...state, visible: false }));
        }, duracion);
    }
}