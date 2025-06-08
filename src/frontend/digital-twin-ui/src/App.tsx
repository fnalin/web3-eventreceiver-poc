import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import Dashboard from "./pages/Dashboard";
import Sobre from "./pages/Sobre";
import Eventos from "./pages/Eventos";
import NovoEvento from "./pages/NovoEvento";
import AppLayout from "./layouts/AppLayout";
import EventoDetalhe from "./pages/EventoDetalhe.tsx";

function App() {
    return (
        <BrowserRouter>
            <Routes>
                <Route path="/" element={<AppLayout />}>
                    <Route index element={<Dashboard />} />
                    <Route path="sobre" element={<Sobre />} />
                    <Route path="eventos" element={<Eventos />} />
                    <Route path="eventos/novo" element={<NovoEvento />} />
                    <Route path="eventos/:externalId" element={<EventoDetalhe />} />
                </Route>

                <Route path="*" element={<Navigate to="/" />} />
            </Routes>
        </BrowserRouter>
    );
}

export default App;