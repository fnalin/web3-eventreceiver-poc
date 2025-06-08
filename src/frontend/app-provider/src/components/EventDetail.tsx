import { useEffect, useState } from 'react';
import { getEventById } from '../services/eventService';
import { useParams, Link } from 'react-router-dom';
import type {EventProcess} from '../types/event';

const EventDetail = () => {
    const { id } = useParams<{ id: string }>();
    const [event, setEvent] = useState<EventProcess | null>(null);

    useEffect(() => {
        const fetchDetail = async () => {
            if (id) {
                const data = await getEventById(parseInt(id));
                setEvent(data);
            }
        };
        fetchDetail();
    }, [id]);

    if (!event) return <p>Carregando...</p>;

    return (
        <div className="card shadow-sm">
            <div className="card-body">
                <h5 className="card-title">Evento #{event.id}</h5>
                <p className="card-text">
                    <strong>Status:</strong>{' '}
                    <span className={`badge bg-${event.status === 1 ? 'success' : 'secondary'}`}>
            {event.status === 1 ? 'Processado' : 'Pendente'}
          </span>
                </p>
                <p><strong>Hash:</strong> <code>{event.eventHash}</code></p>
                <p><strong>Criado em:</strong> {new Date(event.createdAt).toLocaleString()}</p>
                <p><strong>Processado em:</strong> {new Date(event.processedAt).toLocaleString()}</p>
                <p><strong>Payload:</strong></p>
                <pre className="bg-light p-3 rounded border">
          {JSON.stringify(JSON.parse(event.originalPayload), null, 2)}
        </pre>
                {event.failureReason && (
                    <div className="alert alert-danger mt-3">
                        <strong>Erro:</strong> {event.failureReason}
                    </div>
                )}
                <Link to="/" className="btn btn-secondary mt-3">Voltar</Link>
            </div>
        </div>
    );
};

export default EventDetail;