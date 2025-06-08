import { useEffect, useState } from 'react';
import { getEvents } from '../services/eventService';
import type {EventProcess} from '../types/event';
import { Link } from 'react-router-dom';

const EventList = () => {
    const [events, setEvents] = useState<EventProcess[]>([]);
    const [page, setPage] = useState(1);
    const [totalCount, setTotalCount] = useState(0);
    const pageSize = 10;

    const fetchEvents = async () => {
        const data = await getEvents(page, pageSize);
        setEvents(data.items);
        setTotalCount(data.totalCount);
    };

    useEffect(() => {
        fetchEvents();
        const interval = setInterval(fetchEvents, 10000);
        return () => clearInterval(interval);
    }, [page]);

    const totalPages = Math.ceil(totalCount / pageSize);

    return (
        <div className="w-100">
            <div className="table-responsive mb-3">
                <table className="table table-bordered table-hover w-100">
                    <thead className="table-light">
                    <tr>
                        <th>#</th>
                        <th>Status</th>
                        <th>Hash</th>
                        <th>Recebido</th>
                        <th>Ações</th>
                    </tr>
                    </thead>
                    <tbody>
                    {events.map((ev) => (
                        <tr key={ev.id}>
                            <td>{ev.id}</td>
                            <td>
                  <span className={`badge bg-${ev.status === 1 ? 'success' : 'secondary'}`}>
                    {ev.status === 1 ? 'Processado' : 'Pendente'}
                  </span>
                            </td>
                            <td><code>{ev.eventHash}</code></td>
                            <td>{new Date(ev.createdAt).toLocaleString()}</td>
                            <td>
                                <Link to={`/detail/${ev.id}`} className="btn btn-sm btn-outline-primary">
                                    Ver Detalhes
                                </Link>
                            </td>
                        </tr>
                    ))}
                    </tbody>
                </table>
            </div>

            <div className="d-flex justify-content-between align-items-center">
                <button
                    className="btn btn-outline-secondary"
                    disabled={page === 1}
                    onClick={() => setPage(page - 1)}
                >
                    Anterior
                </button>

                <span>
          Página {page} de {totalPages || 1}
        </span>

                <button
                    className="btn btn-outline-secondary"
                    disabled={page === totalPages || totalPages === 0}
                    onClick={() => setPage(page + 1)}
                >
                    Próxima
                </button>
            </div>
        </div>
    );
};

export default EventList;