import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import api from "../axios";
import Loading from "../components/Loading";

export default function EventoDetalhe() {
    const { externalId } = useParams();
    const [conteudo, setConteudo] = useState<string | null>(null);
    const [erro, setErro] = useState("");
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const fetchEvento = async () => {
            try {
                const res = await api.get(`/events/${externalId}`);
                setConteudo(JSON.stringify(res.data, null, 2));
            } catch (err) {
                console.error(err);
                setErro("Falha ao carregar o evento.");
            } finally {
                setLoading(false);
            }
        };

        fetchEvento();
    }, [externalId]);

    return (
        <div className="container mt-4">
            <h2>Detalhes do Evento</h2>

            {loading && <Loading message="Carregando evento..." />}

            {erro && <div className="alert alert-danger">{erro}</div>}

            {conteudo && (
                <textarea
                    className="form-control"
                    style={{ minHeight: "400px", fontFamily: "monospace" }}
                    readOnly
                    value={conteudo}
                />
            )}
        </div>
    );
}