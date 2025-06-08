import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import Loading from "../components/Loading";
import api from "../axios";

interface Evento {
    id: number;
    externalId: string;
    createdAt: string;
    originalPayload: any;
}

interface PagedResult<T> {
    totalCount: number;
    page: number;
    pageSize: number;
    items: T[];
}

export default function Eventos() {
    const [eventos, setEventos] = useState<Evento[]>([]);
    const [page, setPage] = useState(1);
    const pageSize = 10;
    const [totalCount, setTotalCount] = useState(0);
    const [startDate, setStartDate] = useState("");
    const [endDate, setEndDate] = useState("");
    const [loading, setLoading] = useState(false);

    const fetchEventos = async () => {
        setLoading(true);
        try {
            const params: any = { page, pageSize };
            if (startDate) params.startDate = startDate;
            if (endDate) params.endDate = endDate;

            const res = await api.get<PagedResult<Evento>>("/events", { params });
            setEventos(res.data.items);
            setTotalCount(res.data.totalCount);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchEventos();
    }, [page]);

    const totalPages = Math.ceil(totalCount / pageSize);

    return (
        <div className="container mt-4">
            <h2>Lista de Eventos</h2>

            <div className="row g-3 align-items-end mb-3">
                <div className="col-auto">
                    <label>Data Início</label>
                    <input type="date" className="form-control" value={startDate} onChange={e => setStartDate(e.target.value)} />
                </div>
                <div className="col-auto">
                    <label>Data Fim</label>
                    <input type="date" className="form-control" value={endDate} onChange={e => setEndDate(e.target.value)} />
                </div>
                <div className="col-auto">
                    <button className="btn btn-primary" onClick={() => { setPage(1); fetchEventos(); }}>
                        Filtrar
                    </button>
                </div>
            </div>

            {loading ? (
                <Loading message="Carregando eventos..." />
            ) : (
                <>
                    <table className="table table-bordered">
                        <thead>
                        <tr>
                            <th>Id</th>
                            <th>ExternalId</th>
                            <th>Data</th>
                            <th>Conteúdo</th>
                        </tr>
                        </thead>
                        <tbody>
                        {eventos.map(ev => (
                            <tr key={ev.id}>
                                <td>{ev.id}</td>
                                {/*<td>*/}
                                {/*    <a href={`/eventos/${ev.externalId}`} target="_blank" rel="noopener noreferrer">*/}
                                {/*        {ev.externalId}*/}
                                {/*    </a>*/}
                                {/*</td>*/}
                                <td>
                                    <Link to={`/eventos/${ev.externalId}`}>{ev.externalId}</Link>
                                </td>
                                <td>{new Date(ev.createdAt).toLocaleString()}</td>
                                <td><pre className="mb-0">{JSON.stringify(ev.originalPayload, null, 2)}</pre></td>
                            </tr>
                        ))}
                        </tbody>
                    </table>

                    <nav>
                        <ul className="pagination">
                            <li className={`page-item ${page === 1 ? "disabled" : ""}`}>
                                <button className="page-link" onClick={() => setPage(page - 1)}>Anterior</button>
                            </li>
                            <li className="page-item disabled">
                                <span className="page-link">Página {page} de {totalPages || 1}</span>
                            </li>
                            <li className={`page-item ${page >= totalPages ? "disabled" : ""}`}>
                                <button className="page-link" onClick={() => setPage(page + 1)}>Próxima</button>
                            </li>
                        </ul>
                    </nav>
                </>
            )}
        </div>
    );
}