import EventList from '../components/EventList';

const HomePage = () => {
    return (
        <div className="bg-light min-vh-100 px-4 py-3">
            <h2>Eventos Recebidos</h2>
            <p className="text-muted">Atualizado automaticamente a cada 10 segundos</p>
            <EventList />
        </div>
    );
};

export default HomePage;