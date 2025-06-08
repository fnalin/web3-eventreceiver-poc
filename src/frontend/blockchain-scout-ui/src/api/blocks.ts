import axios from 'axios';
import config from "../config";

const api = axios.create({
    baseURL: config.apiBaseUrl,
});

export const getBlocks = async (page = 1, pageSize = 10) => {
    const response = await api.get('/blocks', {
        params: { page, pageSize },
    });
    return response.data;
};

export const getBlockByHash = async (hash: string) => {
    const response = await api.get(`/blocks/${hash}`);
    return response.data;
};