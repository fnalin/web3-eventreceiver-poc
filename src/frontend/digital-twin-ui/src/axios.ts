import axios from "axios";
import config from "./config";

// Cria uma instância do axios com baseURL
const api = axios.create({
    baseURL: config.apiBaseUrl,
});


export default api;