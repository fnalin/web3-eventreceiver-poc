import axios from 'axios';
import config from "../config";
import type {EventProcessResponse, EventProcess} from '../types/event';

const API_URL = config.apiBaseUrl + '/events';

export const getEvents = async (page = 1, pageSize = 10) => {
    const response = await axios.get<EventProcessResponse>(`${API_URL}?page=${page}&pageSize=${pageSize}`);
    return response.data;
};

export const getEventById = async (id: number) => {
    const response = await axios.get<EventProcess>(`${API_URL}/${id}`);
    return response.data;
};