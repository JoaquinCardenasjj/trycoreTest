import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ProjectService } from '../../core/services/projects.service';
import {
    ActivityRequest,
    ActivityResponse,
    ProjectDetailResponse,
    ProjectRequest,
    ProjectSummaryResponse
} from '../../core/models/project-activity.model';

@Component({
    selector: 'app-dashboard',
    standalone: true,
    imports: [CommonModule, FormsModule],
    templateUrl: './dashboard.component.html',
    styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent {
    private readonly projectService = inject(ProjectService);

    public readonly projects = signal<ProjectSummaryResponse[]>([]);
    public readonly selectedProject = signal<ProjectDetailResponse | null>(null);
    public readonly activeProjectId = signal<string | null>(null);
    public readonly loadingProjects = signal(false);
    public readonly loadingProjectDetail = signal(false);

    public readonly showAddActivityModal = signal(false);
    public readonly showEditActivityModal = signal(false);
    public readonly showProjectModal = signal(false);
    public readonly projectModalMode = signal<'create' | 'rename'>('create');

    public readonly activityForm = signal<ActivityRequest>({
        name: null,
        budgetAtCompletion: 0,
        plannedPercentComplete: 0,
        actualPercentComplete: 0,
        actualCost: 0
    });

    public readonly projectForm = signal<ProjectRequest>({
        name: ''
    });

    public readonly selectedActivity = signal<ActivityResponse | null>(null);
    public readonly successMessage = signal<string | null>(null);
    public readonly errorMessage = signal<string | null>(null);

    constructor() {
        this.loadProjects();
    }

    loadProjects(): void {
        this.loadingProjects.set(true);
        this.projectService.obtenerProyectos().subscribe({
            next: (projects) => {
                this.projects.set(projects);
                if (!this.activeProjectId() && projects.length) {
                    this.selectProject(projects[0].id);
                }
            },
            error: (error) => {
                this.errorMessage.set('No se pudo cargar los proyectos.');
                console.error(error);
            },
            complete: () => this.loadingProjects.set(false)
        });
    }

    selectProject(projectId: string): void {
        if (this.activeProjectId() === projectId) {
            return;
        }
        this.activeProjectId.set(projectId);
        this.loadProjectDetail(projectId);
    }

    loadProjectDetail(projectId: string): void {
        this.loadingProjectDetail.set(true);
        this.projectService.obtenerProyectoPorId(projectId).subscribe({
            next: (project) => {
                this.selectedProject.set(project);
                this.clearMessages();
            },
            error: (error) => {
                this.errorMessage.set('No se pudo cargar el proyecto seleccionado.');
                console.error(error);
            },
            complete: () => this.loadingProjectDetail.set(false)
        });
    }

    openCreateProject(): void {
        this.projectModalMode.set('create');
        this.projectForm.set({ name: '' });
        this.showProjectModal.set(true);
    }

    openRenameProject(): void {
        const project = this.selectedProject();
        if (!project) {
            return;
        }
        this.projectModalMode.set('rename');
        this.projectForm.set({ name: project.name ?? '' });
        this.showProjectModal.set(true);
    }

    submitProjectModal(): void {
        const payload: ProjectRequest = { name: this.projectForm().name?.trim() || '' };

        if (this.projectModalMode() === 'create') {
            this.projectService.crearProyecto(payload).subscribe({
                next: (project) => {
                    this.successMessage.set('Proyecto creado correctamente.');
                    this.showProjectModal.set(false);
                    this.loadProjects();
                    this.selectedProject.set(project);
                    this.activeProjectId.set(project.id);
                },
                error: (error) => {
                    this.errorMessage.set('No se pudo crear el proyecto.');
                    console.error(error);
                }
            });
        } else {
            const projectId = this.activeProjectId();
            if (!projectId) {
                return;
            }
            this.projectService.renombrarProyecto(projectId, payload).subscribe({
                next: (project) => {
                    this.successMessage.set('Proyecto renombrado correctamente.');
                    this.showProjectModal.set(false);
                    this.loadProjects();
                    this.selectedProject.set(project);
                },
                error: (error) => {
                    this.errorMessage.set('No se pudo renombrar el proyecto.');
                    console.error(error);
                }
            });
        }
    }

    openAddActivity(): void {
        this.activityForm.set({
            name: null,
            budgetAtCompletion: 0,
            plannedPercentComplete: 0,
            actualPercentComplete: 0,
            actualCost: 0
        });
        this.selectedActivity.set(null);
        this.showAddActivityModal.set(true);
        this.clearMessages();
    }

    openEditActivity(activity: ActivityResponse): void {
        this.selectedActivity.set(activity);
        this.activityForm.set({
            name: activity.name ?? null,
            budgetAtCompletion: activity.budgetAtCompletion,
            plannedPercentComplete: activity.plannedPercentComplete,
            actualPercentComplete: activity.actualPercentComplete,
            actualCost: activity.actualCost
        });
        this.showEditActivityModal.set(true);
        this.clearMessages();
    }

    closeModals(): void {
        this.showAddActivityModal.set(false);
        this.showEditActivityModal.set(false);
        this.showProjectModal.set(false);
    }

    submitNewActivity(): void {
        const projectId = this.activeProjectId();
        if (!projectId) {
            return;
        }
        const payload = this.buildActivityPayload();
        this.projectService.agregarActividad(projectId, payload).subscribe({
            next: () => {
                this.successMessage.set('Actividad creada correctamente.');
                this.showAddActivityModal.set(false);
                this.loadProjectDetail(projectId);
            },
            error: (error) => {
                this.errorMessage.set('No se pudo crear la actividad.');
                console.error(error);
            }
        });
    }

    submitEditActivity(): void {
        const projectId = this.activeProjectId();
        const activity = this.selectedActivity();
        if (!projectId || !activity) {
            return;
        }
        const payload = this.buildActivityPayload();
        this.projectService.actualizarActividad(projectId, activity.id, payload).subscribe({
            next: () => {
                this.successMessage.set('Actividad actualizada correctamente.');
                this.showEditActivityModal.set(false);
                this.loadProjectDetail(projectId);
            },
            error: (error) => {
                this.errorMessage.set('No se pudo actualizar la actividad.');
                console.error(error);
            }
        });
    }

    deleteActivity(activity: ActivityResponse): void {
        const projectId = this.activeProjectId();
        if (!projectId || !window.confirm('¿Eliminar esta actividad?')) {
            return;
        }
        this.projectService.eliminarActividad(projectId, activity.id).subscribe({
            next: () => {
                this.successMessage.set('Actividad eliminada correctamente.');
                this.loadProjectDetail(projectId);
            },
            error: (error) => {
                this.errorMessage.set('No se pudo eliminar la actividad.');
                console.error(error);
            }
        });
    }

    private buildActivityPayload(): ActivityRequest {
        const form = this.activityForm();
        return {
            name: form.name?.trim() || null,
            budgetAtCompletion: Number(form.budgetAtCompletion),
            plannedPercentComplete: Number(form.plannedPercentComplete),
            actualPercentComplete: Number(form.actualPercentComplete),
            actualCost: Number(form.actualCost)
        };
    }

    getPerformanceClass(value: number | null): 'good' | 'warning' | 'critical' | 'neutral' {
        if (value === null || value === undefined) {
            return 'neutral';
        }
        if (value >= 1.05) {
            return 'good';
        }
        if (value >= 0.95) {
            return 'warning';
        }
        return 'critical';
    }

    chartWidth(value: number | null): number {
        const activities = this.selectedProject()?.activities ?? [];
        if (!activities.length || value === null || value === undefined) {
            return 8;
        }
        const allValues = activities.flatMap((item) => [
            item.indicators.plannedValue || 0,
            item.indicators.earnedValue || 0,
            item.indicators.actualCost || 0
        ]);
        const maxValue = Math.max(1, ...allValues);
        return Math.max(10, Math.round((value / maxValue) * 100));
    }

    private clearMessages(): void {
        this.successMessage.set(null);
        this.errorMessage.set(null);
    }
}
