import EventDetail from "../components/EventDetail.tsx";


const HomePage = () => {
    return (
        <div className="bg-light w-100 min-vh-100 p-3">
            <h2>Detalhes do evento recebido</h2>
            <p className="text-muted">Atualizado automaticamente a cada 10 segundos</p>
            <EventDetail />
        </div>
    );
};

export default HomePage;