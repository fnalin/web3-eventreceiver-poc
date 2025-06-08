import { Link } from 'react-router-dom';
import { useWallet } from '../hooks/useWallet';

export default function Navbar() {
    const { address, isConnecting, connect, disconnect } = useWallet();

    const shortAddress = address
        ? `${address.slice(0, 6)}...${address.slice(-4)}`
        : null;

    return (
        <nav className="navbar navbar-expand-lg navbar-dark bg-dark">
            <div className="container">
                <Link className="navbar-brand" to="/">Marketplace NFT</Link>

                <div className="ms-auto">
                    {isConnecting ? (
                        <span className="text-light">Conectando...</span>
                    ) : address ? (
                        <div className="dropdown">
                            <button
                                className="btn btn-success dropdown-toggle"
                                type="button"
                                id="walletDropdown"
                                data-bs-toggle="dropdown"
                                aria-expanded="false"
                            >
                                🟢 {shortAddress}
                            </button>
                            <ul className="dropdown-menu dropdown-menu-end" aria-labelledby="walletDropdown">
                                <li>
                                    <button className="dropdown-item text-danger" onClick={disconnect}>
                                        🔌 Desconectar
                                    </button>
                                </li>
                            </ul>
                        </div>
                    ) : (
                        <button className="btn btn-outline-light" onClick={() => connect(true)}>
                            🔌 {address ? 'Trocar Carteira' : 'Conectar Carteira'}
                        </button>
                    )}
                </div>
            </div>
        </nav>
    );
}