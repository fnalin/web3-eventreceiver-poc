
interface LoadingProps {
    message?: string;
}

export default function Loading({ message = 'Carregando...' }: LoadingProps) {
    return (
        <div className="container mt-4 text-center">
            <div className="spinner-border text-primary" role="status" />
            <p className="mt-2">{message}</p>
        </div>
    );
}