import { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import { getNftByHash, purchaseNft, checkNftAccess, downloadNftEvent, getWalletsByTokenId } from '../api/api';
import Loading from '../components/Loading';
import { useWallet } from '../hooks/useWallet';

export default function NftDetail() {
    const { eventHash } = useParams();
    const [nft, setNft] = useState<any>(null);
    const [loading, setLoading] = useState(true);
    const [purchasing, setPurchasing] = useState(false);
    const [success, setSuccess] = useState(false);
    const [error, setError] = useState('');
    const { address } = useWallet();
    const [hasAccess, setHasAccess] = useState(false);
    const [downloadData, setDownloadData] = useState<any>(null);
    const [downloadError, setDownloadError] = useState('');

    const checkAccess = async () => {
        if (!nft?.tokenId || !address) return;
        try {
            const { wallets } = await getWalletsByTokenId(nft.tokenId);
            setHasAccess(wallets.includes(address.toLowerCase()));
        } catch (err) {
            console.error('Erro ao verificar acesso:', err);
        }
    };


    useEffect(() => {
        if (!eventHash) return;

        setLoading(true);
        getNftByHash(eventHash)
            .then(async (data) => {
                setNft(data);
                if (address && data?.tokenId) {
                    const has = await checkNftAccess(parseInt(data.tokenId), address);
                    setHasAccess(has);
                }
            })
            .catch(() => setError('Erro ao carregar detalhes do NFT.'))
            .finally(() => setLoading(false));
    }, [eventHash, address]);

    const handlePurchase = async () => {
        if (!nft?.tokenId || !address) return;
        try {
            setPurchasing(true);
            setError('');
            await purchaseNft({
                tokenId: parseInt(nft.tokenId), // API espera um número
                buyerAddress: address,
            });
            setSuccess(true);
            await checkAccess(); // <- Revalida após compra
        } catch (err: any) {
            console.error(err);
            setError('Erro ao realizar compra.');
        } finally {
            setPurchasing(false);
        }
    };

    if (loading) return <Loading message="Carregando NFT..." />;

    if (!nft) return <p>NFT não encontrado.</p>;

    return (
        <div className="container mt-4">
            <h2>Detalhes do NFT</h2>
            <p><strong>Hash:</strong> {eventHash}</p>
            <p><strong>Token ID:</strong> {nft.tokenId}</p>
            <p><strong>Owner:</strong> {nft.dt}</p>
            <p><strong>Wallet Owner:</strong> {nft.walletOwner}</p>
            <p><strong>Descrição:</strong> {nft.description}</p>

            <h4>Atributos:</h4>
            <ul>
                {nft.attributes.map((attr: any, i: number) => (
                    <li key={i}>
                        {attr.name} ({attr.type})
                    </li>
                ))}
            </ul>

            {address ? (
                <>
                    <button
                        className="btn btn-success mt-3"
                        onClick={handlePurchase}
                        disabled={purchasing || success || hasAccess}
                    >
                        {purchasing
                            ? 'Processando...'
                            : hasAccess || success
                                ? '✅ Acesso Liberado'
                                : 'Comprar Acesso'}
                    </button>
                    {error && <div className="text-danger mt-2">{error}</div>}
                    {/*{hasAccess && (*/}
                    {/*    <button*/}
                    {/*        className="btn btn-primary mt-3 ms-2"*/}
                    {/*        onClick={async () => {*/}
                    {/*            try {*/}
                    {/*                const data = await downloadNftEvent(eventHash!, address);*/}
                    {/*                alert(JSON.stringify(data, null, 2)); // ou baixe como arquivo*/}
                    {/*            } catch (err) {*/}
                    {/*                setDownloadError('Erro ao baixar o evento.');*/}
                    {/*            }*/}
                    {/*        }}*/}
                    {/*    >*/}
                    {/*        📥 Baixar Evento*/}
                    {/*    </button>*/}
                    {/*)}*/}
                    {/*{downloadError && <div className="text-danger mt-2">{downloadError}</div>}*/}
                </>
            ) : (
                <p className="text-warning mt-3">Conecte sua carteira para comprar.</p>
            )}
        </div>
    );
}