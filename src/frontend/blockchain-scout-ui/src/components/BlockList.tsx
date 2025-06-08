import { useEffect, useState } from 'react';
import { getBlocks } from '../api/blocks';
import { Link } from 'react-router-dom';

type Block = {
    hash: string;
    number: number;
    timestamp: string;
    transactions: string[];
};

type PagedResult = {
    items: Block[];
    page: number;
    pageSize: number;
    totalItems: number;
};

export function BlockList() {
    const [blocks, setBlocks] = useState<PagedResult | null>(null);
    const [page, setPage] = useState(1);

    useEffect(() => {
        let mounted = true;

        const fetchData = async () => {
            const data = await getBlocks(page);
            if (mounted) setBlocks(data);
        };

        fetchData(); // carrega inicial

        const interval = setInterval(fetchData, 10000); // atualiza a cada 10s

        return () => {
            mounted = false;
            clearInterval(interval); // limpa no unmount
        };
    }, [page]);

    const totalPages = blocks ? Math.ceil(blocks.totalItems / blocks.pageSize) : 0;

    return (
        <div className="container mt-4">
            <h2>Blocos</h2>
            {blocks ? (
                <>
                    <table className="table table-bordered table-hover">
                        <thead>
                        <tr>
                            <th>Número</th>
                            <th>Hash</th>
                            <th>Timestamp</th>
                        </tr>
                        </thead>
                        <tbody>
                        {blocks.items.map((block) => (
                            <tr key={block.hash}>
                                <td>{block.number}</td>
                                <td>
                                    <Link to={`/blocks/${block.hash}`}>
                                        {block.hash.slice(0, 16)}...
                                    </Link>
                                </td>
                                <td>{new Date(block.timestamp).toLocaleString()}</td>
                            </tr>
                        ))}
                        </tbody>
                    </table>

                    <div className="d-flex justify-content-between">
                        <button
                            className="btn btn-primary"
                            disabled={page === 1}
                            onClick={() => setPage((p) => p - 1)}
                        >
                            Anterior
                        </button>
                        <span>Página {page} de {totalPages}</span>
                        <button
                            className="btn btn-primary"
                            disabled={page === totalPages}
                            onClick={() => setPage((p) => p + 1)}
                        >
                            Próxima
                        </button>
                    </div>
                </>
            ) : (
                <p>Carregando blocos...</p>
            )}
        </div>
    );
}