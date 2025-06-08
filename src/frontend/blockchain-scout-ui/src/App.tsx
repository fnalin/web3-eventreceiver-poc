import { BrowserRouter, Routes, Route } from 'react-router-dom';
import { BlockList } from './components/BlockList';
import { BlockDetail } from './components/BlockDetail';

function App() {
    return (
        <BrowserRouter>
            <Routes>
                <Route path="/" element={<BlockList />} />
                <Route path="/blocks/:hash" element={<BlockDetail />} />
            </Routes>
        </BrowserRouter>
    );
}

export default App;