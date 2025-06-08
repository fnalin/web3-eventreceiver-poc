// src/api/api.ts
import axios from 'axios';
import config from "../config";

export const API_BASE_URL = config.apiBaseUrl;

export interface Nft {
    eventHash: string;
    description: string;
    tokenUri: string;
    tokenId: string;
    attributes: Array<{ name: string; type: string; value?: any }>;
}

export interface PurchaseDto {
    tokenId: number;
    buyerAddress: string;
}

export async function getAllNfts(): Promise<Nft[]> {
    const response = await axios.get(`${API_BASE_URL}/events`);
    return response.data;
}

export async function getNftByHash(hash: string): Promise<Nft> {
    const response = await axios.get(`${API_BASE_URL}/events/${hash}`);
    return response.data;
}

export async function purchaseNft(data: PurchaseDto): Promise<void> {
    await axios.post(`${API_BASE_URL}/nfts/purchase`, data);
}

export async function checkNftAccess(tokenId: number, wallet: string): Promise<boolean> {
    const response = await axios.get(`${API_BASE_URL}/nfts/wallets/${tokenId}`);
    const wallets: string[] = response.data.wallets;
    return wallets.some(w => w.toLowerCase() === wallet.toLowerCase());
}

export async function downloadNftEvent(eventHash: string, wallet: string): Promise<any> {
    const response = await axios.get(`${API_BASE_URL}/events/${eventHash}/download`, {
        params: { wallet }
    });
    return response.data;
}

export async function getWalletsByTokenId(tokenId: number): Promise<{ tokenId: number; wallets: string[] }> {
    const response = await axios.get(`${API_BASE_URL}/nfts/wallets/${tokenId}`);
    return response.data;
}

