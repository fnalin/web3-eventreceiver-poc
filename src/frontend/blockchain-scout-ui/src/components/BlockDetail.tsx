import { useEffect, useState } from 'react';
import { getBlockByHash } from '../api/blocks';
import { useParams, Link } from 'react-router-dom';

export function BlockDetail() {
    const { hash } = useParams<{ hash: string }>();
    const [block, setBlock] = useState<any>(null);

    useEffect(() => {
        if (hash) {
            getBlockByHash(hash).then(setBlock);
        }
    }, [hash]);

    if (!block) return <div className="container mt-4">Carregando bloco...</div>;

    return (
        <div className="container mt-4">
            <h2>Detalhes do Bloco #{block.number}</h2>
            <p><strong>Hash:</strong> {block.hash}</p>
            <p><strong>Data:</strong> {new Date(block.timestamp).toLocaleString()}</p>

            <h4>Transações</h4>
            <ul>
                {block.transactions.map((tx: string) => (
                    <li key={tx}>{tx}</li>
                ))}
            </ul>

            <Link to="/" className="btn btn-secondary mt-3">← Voltar</Link>
        </div>
    );
}