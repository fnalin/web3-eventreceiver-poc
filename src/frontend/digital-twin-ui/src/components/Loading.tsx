export default function Loading({ message = "Carregando..." }: { message?: string }) {
    return (
        <div className="d-flex flex-column justify-content-center align-items-center" style={{ minHeight: "60vh" }}>
            <div className="spinner-border text-primary" role="status" />
            <p className="mt-3">{message}</p>
        </div>
    );
}