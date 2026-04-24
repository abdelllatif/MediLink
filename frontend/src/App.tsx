import { Routes, Route } from 'react-router-dom';
import Home from './pages/Home';
import Login from './pages/auth/Login';
import Register from './pages/auth/Register';

// Nurse Dashboard
import DashboardLayout from './pages/nurse/DashboardLayout';
import Reception from './pages/nurse/Reception';
import WaitingQueue from './pages/nurse/WaitingQueue';
import Reservations from './pages/nurse/Reservations';

function App() {
  return (
    <Routes>
      {/* Public Routes */}
      <Route path="/" element={<Home />} />
      <Route path="/login" element={<Login />} />
      <Route path="/register" element={<Register />} />

      {/* Nurse Dashboard Routes */}
      <Route path="/nurse" element={<DashboardLayout />}>
        <Route index element={<Reception />} />
        <Route path="queue" element={<WaitingQueue />} />
        <Route path="reservations" element={<Reservations />} />
      </Route>

      {/* Fallback */}
      <Route path="*" element={<div className="p-10 text-center text-2xl font-bold text-gray-500">404 - Page Not Found</div>} />
    </Routes>
  );
}

export default App;
