export interface EventProcess {
    id: number;
    status: number;
    eventHash: string;
    originalPayload: string;
    failureReason: string | null;
    createdAt: string;
    processedAt: string;
}

export interface EventProcessResponse {
    totalCount: number;
    page: number;
    pageSize: number;
    items: EventProcess[];
}