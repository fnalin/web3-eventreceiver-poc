import { Routes, Route } from 'react-router-dom';
import Home from './pages/Home';
import NftDetail from './pages/NftDetail';
import Navbar from './components/Navbar';

export default function App() {
    return (
        <>
            <Navbar />
            <Routes>
                <Route path="/" element={<Home />} />
                <Route path="/nft/:eventHash" element={<NftDetail />} />
            </Routes>
        </>
    );
}