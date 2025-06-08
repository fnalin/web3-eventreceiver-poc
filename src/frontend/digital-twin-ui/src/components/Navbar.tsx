import { Link } from "react-router-dom";

export default function Navbar() {

    return (
        <nav className="navbar navbar-expand-lg navbar-dark bg-dark px-3">
            <Link className="navbar-brand" to="/">
                Fansoft Digital Twin
            </Link>

            <div className="collapse navbar-collapse">
                <ul className="navbar-nav me-auto">
                    {/*<li className="nav-item">*/}
                    {/*    <Link className="nav-link" to="/sobre">Sobre</Link>*/}
                    {/*</li>*/}
                    <li className="nav-item">
                        <Link className="nav-link" to="/eventos">Eventos</Link>
                    </li>
                    <li className="nav-item">
                        <Link className="nav-link" to="/eventos/novo">Enviar novo Evento</Link>
                    </li>
                </ul>

            </div>
        </nav>
    );
}