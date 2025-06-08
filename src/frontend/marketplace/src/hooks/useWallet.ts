import { useEffect, useState } from 'react';

declare global {
    interface Window {
        ethereum?: any;
    }
}

export function useWallet() {
    const [address, setAddress] = useState<string | null>(null);
    const [isConnecting, setIsConnecting] = useState(false);

    const connect = async () => {
        if (!window.ethereum) {
            alert('MetaMask não encontrada.');
            return;
        }

        try {
            setIsConnecting(true);

            // 🔹 Abre o modal de contas da MetaMask
            const accounts = await window.ethereum.request({ method: 'eth_requestAccounts' });
            setAddress(accounts[0]);
        } catch (err) {
            console.error('Erro ao conectar carteira:', err);
        } finally {
            setIsConnecting(false);
        }
    };

    const disconnect = () => {
        // setAddress(null);
        alert('Para remover o acesso totalmente, vá até a MetaMask > Conexões e remova este site.');
    };

    useEffect(() => {
        // 🔹 Detecta troca de conta via MetaMask e atualiza
        if (window.ethereum) {
            const handleAccountsChanged = (accounts: string[]) => {
                setAddress(accounts[0] || null);
            };

            window.ethereum.on('accountsChanged', handleAccountsChanged);

            // Auto conecta se já autorizado
            window.ethereum.request({ method: 'eth_accounts' })
                .then((accounts: string[]) => {
                    if (accounts.length > 0) setAddress(accounts[0]);
                });

            return () => {
                window.ethereum.removeListener('accountsChanged', handleAccountsChanged);
            };
        }
    }, []);

    return { address, isConnecting, connect, disconnect };
}