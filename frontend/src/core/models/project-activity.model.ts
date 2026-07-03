export interface EvmIndicatorsDto {
    plannedValue: number;
    earnedValue: number;
    actualCost: number;
    costVariance: number;
    scheduleVariance: number;
    costPerformanceIndex: number | null;
    schedulePerformanceIndex: number | null;
    estimateAtCompletion: number | null;
    varianceAtCompletion: number | null;
    costInterpretation: string | null;
    scheduleInterpretation: string | null;
}

export interface ActivityRequest {
    name?: string | null;
    budgetAtCompletion: number;
    plannedPercentComplete: number;
    actualPercentComplete: number;
    actualCost: number;
}

export interface ActivityResponse {
    id: string;
    projectId: string;
    name?: string | null;
    budgetAtCompletion: number;
    plannedPercentComplete: number;
    actualPercentComplete: number;
    actualCost: number;
    indicators: EvmIndicatorsDto;
}

export interface ProjectSummaryResponse {
    id: string;
    name?: string | null;
    activityCount: number;
    consolidatedIndicators: EvmIndicatorsDto;
}

export interface ProjectRequest {
    name?: string | null;
}

export interface ProjectDetailResponse {
    id: string;
    name?: string | null;
    consolidatedIndicators: EvmIndicatorsDto;
    activities?: ActivityResponse[] | null;
}

export type ProjectActivity = ActivityResponse;
export type ProjectActivityList = ActivityResponse[];
