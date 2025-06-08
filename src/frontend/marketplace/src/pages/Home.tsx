// src/pages/Home.tsx
import { useEffect, useState } from 'react';
import { getAllNfts, type Nft } from '../api/api';
import { Link } from 'react-router-dom';
import Loading from '../components/Loading';

export default function Home() {
    const [nfts, setNfts] = useState<Nft[]>([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {

        const fetchData = async () => {
            await  getAllNfts()
                .then(setNfts)
                .finally(() => setLoading(false));

        };

        fetchData(); // Carrega os NFTs ao montar o componente


        setInterval(fetchData, 10000); // atualiza a cada 10s

    }, []);

    if (loading) return <Loading message="Carregando NFTs..." />;

    return (
        <div className="container mt-4">
            <h1 className="mb-4">Marketplace de NFTs</h1>
            <div className="row">
                {nfts.map(nft => (
                    <div className="col-md-4 mb-4" key={nft.eventHash}>
                        <div className="card">
                            <div className="card-body">
                                <h5 className="card-title">NFT: {nft.eventHash.slice(0, 10)}...</h5>
                                <p className="card-text">{nft.description}</p>
                                <Link to={`/nft/${nft.eventHash}`} className="btn btn-primary">Ver detalhes</Link>
                            </div>
                        </div>
                    </div>
                ))}
            </div>
        </div>
    );
}