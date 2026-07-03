import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import {
    ActivityRequest,
    ActivityResponse,
    ProjectDetailResponse,
    ProjectRequest,
    ProjectSummaryResponse
} from '../models/project-activity.model';

@Injectable({
    providedIn: 'root'
})
export class ProjectService {
    private readonly http = inject(HttpClient);
    private readonly apiUrl = `${environment.apiUrl}/projects`;

    obtenerProyectos(): Observable<ProjectSummaryResponse[]> {
        return this.http.get<ProjectSummaryResponse[]>(this.apiUrl);
    }

    crearProyecto(proyecto: ProjectRequest): Observable<ProjectDetailResponse> {
        return this.http.post<ProjectDetailResponse>(this.apiUrl, proyecto);
    }

    obtenerProyectoPorId(projectId: string): Observable<ProjectDetailResponse> {
        return this.http.get<ProjectDetailResponse>(`${this.apiUrl}/${projectId}`);
    }

    renombrarProyecto(projectId: string, proyecto: ProjectRequest): Observable<ProjectDetailResponse> {
        return this.http.put<ProjectDetailResponse>(`${this.apiUrl}/${projectId}`, proyecto);
    }

    eliminarProyecto(projectId: string): Observable<void> {
        return this.http.delete<void>(`${this.apiUrl}/${projectId}`);
    }

    agregarActividad(projectId: string, actividad: ActivityRequest): Observable<ActivityResponse> {
        return this.http.post<ActivityResponse>(`${this.apiUrl}/${projectId}/activities`, actividad);
    }

    actualizarActividad(projectId: string, activityId: string, actividad: ActivityRequest): Observable<ActivityResponse> {
        return this.http.put<ActivityResponse>(`${this.apiUrl}/${projectId}/activities/${activityId}`, actividad);
    }

    eliminarActividad(projectId: string, activityId: string): Observable<void> {
        return this.http.delete<void>(`${this.apiUrl}/${projectId}/activities/${activityId}`);
    }
}
