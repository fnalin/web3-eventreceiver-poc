import { useState } from "react";
import api from "../axios";
import Loading from "../components/Loading";

export default function NovoEvento() {
    const [json, setJson] = useState("{\n  \"exemplo\": \"valor\"\n}");
    const [loading, setLoading] = useState(false);
    const [resultado, setResultado] = useState<{ internalId: number; externalId: string } | null>(null);
    const [erro, setErro] = useState("");

    const handleSubmit = async () => {
        setErro("");
        setResultado(null);

        let parsed;
        try {
            parsed = JSON.parse(json);
        } catch {
            setErro("JSON inválido.");
            return;
        }

        setLoading(true);
        try {
            const res = await api.post("/events", parsed);
            setResultado(res.data);
        } catch (err) {
            setErro("Falha ao enviar evento. Tente novamente.");
            console.error(err);
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="container mt-4">
            <h2>Enviar novo Evento</h2>
            <p>Insira abaixo o conteúdo do evento em formato JSON:</p>

            <textarea
                className="form-control mb-3"
                style={{ fontFamily: "monospace", minHeight: 200 }}
                value={json}
                onChange={(e) => setJson(e.target.value)}
            />

            <button className="btn btn-primary" onClick={handleSubmit} disabled={loading}>
                Enviar Evento
            </button>

            {loading && <Loading message="Enviando evento..." />}

            {erro && <div className="alert alert-danger mt-3">{erro}</div>}

            {resultado && (
                <div className="alert alert-success mt-3">
                    Evento enviado com sucesso!<br />
                    <strong>internalId:</strong> {resultado.internalId}<br />
                    <strong>externalId:</strong> {resultado.externalId}
                </div>
            )}
        </div>
    );
}