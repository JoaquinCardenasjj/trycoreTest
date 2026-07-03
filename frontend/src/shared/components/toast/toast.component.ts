import { Component, inject } from '@angular/core';
import { NotificationService } from '../../../core/services/notification.service';

@Component({
    selector: 'app-toast',
    imports: [], // Angular 19 nativo
    templateUrl: './toast.component.html',
    styleUrls: ['./toast.component.scss']
})
export class ToastComponent {
    // Inyectamos el servicio para leer el Signal directamente en el HTML
    public readonly notificationService = inject(NotificationService);
}